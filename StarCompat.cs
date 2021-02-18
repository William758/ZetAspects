using RoR2;
using UnityEngine;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace TPDespair.ZetAspects
{
    public static class StarCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2");
                }
                return (bool)_enabled;
            }
        }



        public static bool EliteCoreEnabled()
        {
            return Starstorm2.Starstorm.EnableElites.Value;
        }

        public static void GetIndexes()
        {
            if (Starstorm2.Starstorm.EnableElites.Value)
            {
                if (ZetAspectsPlugin.StarVoidEquipIndex == EquipmentIndex.None)
                {
                    ZetAspectsPlugin.StarVoidEquipIndex = EquipmentCatalog.FindEquipmentIndex("AffixVoid");
                    if (ZetAspectsPlugin.StarVoidEquipIndex != EquipmentIndex.None)
                    {
                        ZetAspectsPlugin.StarVoidEliteIndex = EliteCatalog.GetEquipmentEliteIndex(ZetAspectsPlugin.StarVoidEquipIndex);
                        EquipmentDef eDef = EquipmentCatalog.GetEquipmentDef(ZetAspectsPlugin.StarVoidEquipIndex);
                        ZetAspectsPlugin.StarVoidAffixBuffIndex = eDef.passiveBuff;
                        ZetAspectsPlugin.StarVoidSlowBuffIndex = ZetAspectsPlugin.StarVoidAffixBuffIndex + 1;

                        Debug.LogWarning("ZetAspect : StarCompat - AffixVoid EquipmentIndex : " + ZetAspectsPlugin.StarVoidEquipIndex);
                        Debug.LogWarning("ZetAspect : StarCompat - AffixVoid EliteIndex : " + ZetAspectsPlugin.StarVoidEliteIndex);
                        Debug.LogWarning("ZetAspect : StarCompat - AffixVoid Affix BuffIndex : " + ZetAspectsPlugin.StarVoidAffixBuffIndex);
                        Debug.LogWarning("ZetAspect : StarCompat - AffixVoid Slow BuffIndex : " + ZetAspectsPlugin.StarVoidSlowBuffIndex);
                    }
                    else
                    {
                        Debug.LogWarning("ZetAspect : StarCompat -  Failed to find AffixVoid EquipmentIndex");
                    }
                }
            }
            if (Starstorm2.Starstorm.EnableItems.Value)
            {
                if (ZetAspectsPlugin.StarCritMultiItemIndex == ItemIndex.None)
                {
                    ZetAspectsPlugin.StarCritMultiItemIndex = ItemCatalog.FindItemIndex("ErraticGadget");
                    if (ZetAspectsPlugin.StarCritMultiItemIndex != ItemIndex.None)
                    {
                        Debug.LogWarning("ZetAspect : StarCompat - ErraticGadget ItemIndex : " + ZetAspectsPlugin.StarCritMultiItemIndex);
                    }
                    else
                    {
                        Debug.LogWarning("ZetAspect : StarCompat -  Failed to find ErraticGadget ItemIndex");
                    }
                }
            }
        }

        public static void UpdateAffixVoidAuraItemBehavior(CharacterBody self)
        {
            if (ZetAspectsPlugin.StarVoidAffixBuffIndex == BuffIndex.None) return;
            self.AddItemBehavior<Starstorm2.Cores.EliteCore.AffixVoidBehavior>(self.HasBuff(ZetAspectsPlugin.StarVoidAffixBuffIndex) ? 1 : 0);
        }
    }
}
