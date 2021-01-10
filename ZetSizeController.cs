using RoR2;
using UnityEngine;

namespace TPDespair.ZetAspects
{
    public static class ZetSizeController
    {
        internal static void Init()
        {
            RecalculateStatsHook();
            FixedUpdateHook();
            OnDestroyHook();
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



        private static void RecalculateStatsSize(CharacterBody self)
        {
            if (!self) return;

            bool newData = false;
            ZetSizeData sizeData;

            if (!self.gameObject.GetComponent<ZetSizeData>())
            {
                if (!HasModelSize(self) || !HasCamera(self)) return;
                
                newData = true;

                sizeData = self.gameObject.AddComponent<ZetSizeData>();
                sizeData.netId = self.netId;
                sizeData.size = self.modelLocator.modelTransform.localScale;
                sizeData.camera = self.gameObject.GetComponent<CameraTargetParams>().cameraParams.standardLocalCameraPos;
                sizeData.scale = 1f;
                sizeData.target = 1f;
                sizeData.playerControlled = self.isPlayerControlled;
                sizeData.cameraModified = false;

                if (self.isPlayerControlled) Debug.LogWarning("Created Player ZetSizeData : " + sizeData.netId);
            }

            sizeData = self.gameObject.GetComponent<ZetSizeData>();

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
                    switch (self.inventory.currentEquipmentIndex)
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

            float limit = ZetAspectsPlugin.ZetSizeLimitCfg.Value;
            limit = Mathf.Max(0.5f, limit);
            value = Mathf.Clamp(value, 0.5f, limit);

            if (sizeData.target != value) sizeData.target = value;

            if (newData)
            {
                if (sizeData.playerControlled) Debug.LogWarning("Camera Saved [" + sizeData.netId + "] : " + sizeData.camera);
                UpdateSize(self, true);
            }
        }

        private static void FixedUpdateSize(CharacterBody self)
        {
            if (!self) return;

            UpdateSize(self);
        }

        private static void UpdateSize(CharacterBody self)
        {
            UpdateSize(self, false);
        }

        private static void UpdateSize(CharacterBody self, bool instant)
        {
            if (!self.gameObject.GetComponent<ZetSizeData>()) return;

            ZetSizeData sizeData = self.gameObject.GetComponent<ZetSizeData>();
            bool update = false;

            if (!sizeData.playerControlled && self.isPlayerControlled)
            {
                Debug.LogWarning("Player Control - ZetSizeData : " + sizeData.netId);
                sizeData.playerControlled = true;
            }

            if (sizeData.scale != sizeData.target)
            {
                update = true;

                if (instant)
                {
                    sizeData.scale = sizeData.target;

                    if (sizeData.playerControlled) Debug.LogWarning("Player Size : " + sizeData.netId + " - " + sizeData.scale);
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

                    if (sizeData.playerControlled) Debug.LogWarning("Player Size : " + sizeData.netId + " - " + sizeData.scale + " => " + sizeData.target);
                }
            }

            if (update)
            {
                Vector3 size = sizeData.size;
                float scale = sizeData.scale;

                self.modelLocator.modelTransform.localScale = new Vector3(size.x * scale, size.y * scale, size.z * scale);

                if (sizeData.playerControlled)
                {
                    Vector3 camera = sizeData.camera;
                    self.gameObject.GetComponent<CameraTargetParams>().cameraParams.standardLocalCameraPos = new Vector3(camera.x + 0f, camera.y + ((scale - 1f) * 2.75f), camera.z - ((scale - 1f) * 4f));
                    if (!sizeData.cameraModified) sizeData.cameraModified = true;
                }
            }
        }

        private static void DestroySizeData(CharacterBody self)
        {
            if (!self) return;

            if (!self.gameObject.GetComponent<ZetSizeData>()) return;

            ZetSizeData sizeData = self.gameObject.GetComponent<ZetSizeData>();

            if (sizeData.cameraModified) self.gameObject.GetComponent<CameraTargetParams>().cameraParams.standardLocalCameraPos = sizeData.camera;
            if (sizeData.playerControlled) Debug.LogWarning("Destroying Player ZetSizeData : " + sizeData.netId);
            Object.Destroy(sizeData);
        }

        private static bool HasModelSize(CharacterBody self)
        {
            if (self.modelLocator)
            {
                if (self.modelLocator.modelTransform)
                {
                    if (self.modelLocator.modelTransform.localScale != null) return true;
                }
            }

            return false;
        }

        private static bool HasCamera(CharacterBody self)
        {
            if (!self.gameObject.GetComponent<CameraTargetParams>()) return false;

            var camera = self.gameObject.GetComponent<CameraTargetParams>();

            if (camera.cameraParams)
            {
                if (camera.cameraParams.standardLocalCameraPos != null) return true;
            }

            return false;
        }
    }
}
