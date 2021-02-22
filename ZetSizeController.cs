using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace TPDespair.ZetAspects
{
    public class ZetSizeData : MonoBehaviour
    {
        public NetworkInstanceId netId;
        public Vector3 size;
        public float scale;
        public float target;
        public Vector3 offset;
    }

    public static class ZetSizeController
    {
        private static Vector3 cameraOffset = Vector3.zero;

        public static bool globalShrink = false;

        internal static void Init()
        {
            RecalculateStatsHook();
            FixedUpdateHook();
            OnDestroyHook();
            if (ZetAspectsPlugin.ZetEnableCameraModifyCfg.Value) CameraTargetPeramsHook();
        }

        private static void RecalculateStatsHook()
        {
            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);

                RecalculateStatsSize(self);
            };
        }

        private static void FixedUpdateHook()
        {
            On.RoR2.CharacterBody.FixedUpdate += (orig, self) =>
            {
                orig(self);

                FixedUpdateSize(self);
            };
        }

        private static void OnDestroyHook()
        {
            On.RoR2.CharacterBody.OnDestroy += (orig, self) =>
            {
                DestroySizeData(self);

                orig(self);
            };
        }

        private static void CameraTargetPeramsHook()
        {
            IL.RoR2.CameraTargetParams.Update += (il) =>
            {
                ILCursor c = new ILCursor(il);

                // store camera offset
                bool found = c.TryGotoNext(
                    x => x.MatchStfld<CameraTargetParams>("recoil")
                );

                if (found)
                {
                    c.Index += 1;

                    c.Emit(OpCodes.Ldarg, 0);
                    c.EmitDelegate<Action<CameraTargetParams>>((cameraParams) =>
                    {
                        CharacterBody body = cameraParams.GetComponent<CharacterBody>();
                        if (!body)
                        {
                            cameraOffset = Vector3.zero;
                            return;
                        }
                        ZetSizeData sizeData = body.GetComponent<ZetSizeData>();
                        if (!sizeData) cameraOffset = Vector3.zero;
                        else cameraOffset = sizeData.offset;
                    });
                }
                else
                {
                    Debug.LogWarning("ZetSizeController - Camera Hook Store Offset Failed");
                    Debug.LogWarning("ZetSizeController - Aborting Camera Hooks");
                    return;
                }

                // standard
                found = c.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CameraTargetParams>("cameraParams"),
                    x => x.MatchLdfld<CharacterCameraParams>("standardLocalCameraPos")
                );

                if (found)
                {
                    c.Index += 3;

                    c.EmitDelegate<Func<Vector3, Vector3>>((cameraPos) =>
                    {
                        return cameraPos + cameraOffset;
                    });
                }
                else
                {
                    Debug.LogWarning("ZetSizeController - Camera Hook 1 Failed");
                }

                // first person
                found = c.TryGotoNext(
                    x => x.MatchCall<Vector3>("get_zero")
                );

                if (found)
                {
                    c.Index += 1;

                    c.EmitDelegate<Func<Vector3, Vector3>>((cameraPos) =>
                    {
                        return cameraPos + cameraOffset;
                    });
                }
                else
                {
                    Debug.LogWarning("ZetSizeController - Camera Hook 2 Failed");
                }

                // aura
                found = c.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CameraTargetParams>("cameraParams"),
                    x => x.MatchLdfld<CharacterCameraParams>("standardLocalCameraPos")
                );

                if (found)
                {
                    c.Index += 3;

                    c.EmitDelegate<Func<Vector3, Vector3>>((cameraPos) =>
                    {
                        return cameraPos + cameraOffset;
                    });
                }
                else
                {
                    Debug.LogWarning("ZetSizeController - Camera Hook 3 Failed");
                }

                // sprinting
                found = c.TryGotoNext(
                    x => x.MatchLdarg(0),
                    x => x.MatchLdfld<CameraTargetParams>("cameraParams"),
                    x => x.MatchLdfld<CharacterCameraParams>("standardLocalCameraPos")
                );

                if (found)
                {
                    c.Index += 3;

                    c.EmitDelegate<Func<Vector3, Vector3>>((cameraPos) =>
                    {
                        return cameraPos + cameraOffset;
                    });
                }
                else
                {
                    Debug.LogWarning("ZetSizeController - Camera Hook 4 Failed");
                }

                // aim throw
                found = c.TryGotoNext(
                    x => x.MatchLdloc(1)
                );

                if (found)
                {
                    c.Index += 1;

                    c.EmitDelegate<Func<Vector3, Vector3>>((cameraPos) =>
                    {
                        return cameraPos + cameraOffset;
                    });
                }
                else
                {
                    Debug.LogWarning("ZetSizeController - Camera Hook 5 Failed");
                }
            };
        }



        private static void RecalculateStatsSize(CharacterBody self)
        {
            if (!self) return;

            bool newData = false;
            ZetSizeData sizeData;

            if (!self.gameObject.GetComponent<ZetSizeData>())
            {
                if (!HasModelSize(self)) return;

                newData = true;

                sizeData = self.gameObject.AddComponent<ZetSizeData>();
                sizeData.netId = self.netId;
                sizeData.size = self.modelLocator.modelTransform.localScale;
                sizeData.scale = 1f;
                sizeData.target = 1f;
                sizeData.offset = Vector3.zero;

                if (self.isPlayerControlled) Debug.LogWarning("Created Player ZetSizeData : " + sizeData.netId);
            }

            sizeData = self.gameObject.GetComponent<ZetSizeData>();

            float value = GetCharacterScale(self);

            if (sizeData.target != value) sizeData.target = value;

            if (newData) UpdateSize(self, true);
        }

        private static bool HasModelSize(CharacterBody self)
        {
            if (self.modelLocator)
            {
                if (self.modelLocator.modelTransform) return true;
            }

            return false;
        }

        private static float GetCharacterScale(CharacterBody self)
        {
            float value = 1f, factor, count;

            if (self.inventory)
            {
                factor = ZetAspectsPlugin.ZetSizeEffectBaseCfg.Value;
                count = self.inventory.GetItemCount(ItemIndex.Knurl);
                if (count > 0) value += factor * Mathf.Min(10f, Mathf.Sqrt(count));
                count = self.inventory.GetItemCount(ItemIndex.Pearl);
                if (count > 0) value += factor * count;
                count = self.inventory.GetItemCount(ItemIndex.ShinyPearl);
                if (count > 0) value += 2f * factor * count;

                if (self.teamComponent.teamIndex == TeamIndex.Player)
                {
                    factor = ZetAspectsPlugin.ZetSizeEffectAspectEquipmentCfg.Value;
                    EquipmentIndex equipIndex = self.inventory.currentEquipmentIndex;

                    if (ZetAspectsPlugin.StarVoidEquipIndex != EquipmentIndex.None && equipIndex == ZetAspectsPlugin.StarVoidEquipIndex)
                    {
                        value += factor;
                    }
                    if (ZetAspectsPlugin.StarEtherEquipIndex != EquipmentIndex.None && equipIndex == ZetAspectsPlugin.StarEtherEquipIndex)
                    {
                        value += factor;
                    }

                    switch (equipIndex)
                    {
                        case EquipmentIndex.AffixWhite:
                        case EquipmentIndex.AffixBlue:
                        case EquipmentIndex.AffixRed:
                        case EquipmentIndex.AffixHaunted:
                        case EquipmentIndex.AffixPoison:
                            value += factor;
                            break;
                    }
                }
            }

            factor = ZetAspectsPlugin.ZetSizeEffectAspectCfg.Value;

            if (ZetAspectsPlugin.HasValidBuff(self, ZetAspectsPlugin.StarVoidAffixBuffIndex)) value += factor;
            if (ZetAspectsPlugin.HasValidBuff(self, ZetAspectsPlugin.StarEtherAffixBuffIndex)) value += factor;

            if (self.HasBuff(BuffIndex.AffixWhite)) value += factor;
            if (self.HasBuff(BuffIndex.AffixBlue)) value += factor;
            if (self.HasBuff(BuffIndex.AffixRed)) value += factor;
            if (self.HasBuff(BuffIndex.AffixHaunted)) value += factor;
            if (self.HasBuff(BuffIndex.AffixPoison)) value += factor;

            if (self.HasBuff(ZetAspectsPlugin.ZetHeadHunterBuff))
            {
                factor = ZetAspectsPlugin.ZetSizeEffectHunterCfg.Value;
                value += factor * self.GetBuffCount(ZetAspectsPlugin.ZetHeadHunterBuff);
            }

            if (self.HasBuff(BuffIndex.TonicBuff))
            {
                factor = ZetAspectsPlugin.ZetSizeEffectTonicCfg.Value;
                value *= factor;
            }

            if (globalShrink) value *= 0.65f;

            float limit = ZetAspectsPlugin.ZetSizeLimitCfg.Value;
            limit = Mathf.Max(0.5f, limit);
            value = Mathf.Clamp(value, 0.5f, limit);

            return value;
        }



        private static void FixedUpdateSize(CharacterBody self)
        {
            UpdateSize(self, false);
        }

        private static void UpdateSize(CharacterBody self, bool instant)
        {
            if (!self) return;
            ZetSizeData sizeData = self.gameObject.GetComponent<ZetSizeData>();
            if (!sizeData) return;
            bool update = false;

            if (sizeData.scale != sizeData.target)
            {
                update = true;

                if (instant)
                {
                    sizeData.scale = sizeData.target;

                    if (self.isPlayerControlled) Debug.LogWarning("Player Size : " + sizeData.netId + " - " + sizeData.scale);
                }
                else
                {
                    float delta = Time.fixedDeltaTime * sizeData.scale * Mathf.Abs(ZetAspectsPlugin.ZetSizeChangeRateCfg.Value);

                    if (sizeData.scale < sizeData.target)
                    {
                        sizeData.scale = Mathf.Min(sizeData.scale + delta, sizeData.target);
                    }
                    else
                    {
                        sizeData.scale = Mathf.Max(sizeData.scale - delta, sizeData.target);
                    }

                    if (self.isPlayerControlled) Debug.LogWarning("Player Size : " + sizeData.netId + " - " + sizeData.scale + " => " + sizeData.target);
                }
            }

            if (update)
            {
                Vector3 size = sizeData.size;
                float scale = sizeData.scale;

                self.modelLocator.modelTransform.localScale = new Vector3(size.x * scale, size.y * scale, size.z * scale);

                sizeData.offset.y = (scale - 1f) * 2.75f;
                sizeData.offset.z = (scale - 1f) * -4f;
            }
        }



        private static void DestroySizeData(CharacterBody self)
        {
            if (!self) return;
            ZetSizeData sizeData = self.gameObject.GetComponent<ZetSizeData>();
            if (!sizeData) return;

            if (self.isPlayerControlled) Debug.LogWarning("Destroying Player ZetSizeData : " + sizeData.netId);
            UnityEngine.Object.Destroy(sizeData);
        }
    }
}
