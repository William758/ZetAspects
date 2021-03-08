using RoR2;
using UnityEngine;
using System.Security;
using System.Security.Permissions;
using System;

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
                        ZetAspectsPlugin.StarVoidAffixBuffIndex = EquipmentCatalog.GetEquipmentDef(ZetAspectsPlugin.StarVoidEquipIndex).passiveBuff;

                        Debug.LogWarning("ZetAspect : StarCompat - AffixVoid EquipmentIndex : " + ZetAspectsPlugin.StarVoidEquipIndex);
                        Debug.LogWarning("ZetAspect : StarCompat - AffixVoid EliteIndex : " + ZetAspectsPlugin.StarVoidEliteIndex);
                        Debug.LogWarning("ZetAspect : StarCompat - AffixVoid BuffIndex : " + ZetAspectsPlugin.StarVoidAffixBuffIndex);
                    }
                    else
                    {
                        Debug.LogWarning("ZetAspect : StarCompat - Failed to find AffixVoid EquipmentIndex");
                    }
                }

                if (ZetAspectsPlugin.StarVoidSlowBuffIndex == BuffIndex.None)
                {
                    ZetAspectsPlugin.StarVoidSlowBuffIndex = BuffCatalog.FindBuffIndex("VoidSlow");
                    if (ZetAspectsPlugin.StarVoidSlowBuffIndex != BuffIndex.None)
                    {
                        Debug.LogWarning("ZetAspect : StarCompat - VoidSlow BuffIndex : " + ZetAspectsPlugin.StarVoidSlowBuffIndex);
                    }
                    else
                    {
                        Debug.LogWarning("ZetAspect : StarCompat - Failed to find VoidSlow BuffIndex");
                    }
                }
                /*
                if (ZetAspectsPlugin.StarEtherEquipIndex == EquipmentIndex.None)
                {
                    ZetAspectsPlugin.StarEtherEquipIndex = EquipmentCatalog.FindEquipmentIndex("AffixEthereal");
                    if (ZetAspectsPlugin.StarEtherEquipIndex != EquipmentIndex.None)
                    {
                        ZetAspectsPlugin.StarEtherEliteIndex = EliteCatalog.GetEquipmentEliteIndex(ZetAspectsPlugin.StarEtherEquipIndex);
                        ZetAspectsPlugin.StarEtherAffixBuffIndex = EquipmentCatalog.GetEquipmentDef(ZetAspectsPlugin.StarEtherEquipIndex).passiveBuff;

                        Debug.LogWarning("ZetAspect : StarCompat - AffixEthereal EquipmentIndex : " + ZetAspectsPlugin.StarEtherEquipIndex);
                        Debug.LogWarning("ZetAspect : StarCompat - AffixEthereal EliteIndex : " + ZetAspectsPlugin.StarEtherEliteIndex);
                        Debug.LogWarning("ZetAspect : StarCompat - AffixEthereal BuffIndex : " + ZetAspectsPlugin.StarEtherAffixBuffIndex);
                    }
                    else
                    {
                        Debug.LogWarning("ZetAspect : StarCompat - Failed to find AffixEthereal EquipmentIndex");
                    }
                }
                */
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
                        Debug.LogWarning("ZetAspect : StarCompat - Failed to find ErraticGadget ItemIndex");
                    }
                }
            }
        }

        private static int VoidBehavior = -1;

        public static void UpdateAffixVoidAuraItemBehavior(CharacterBody self)
        {
            if (ZetAspectsPlugin.StarVoidAffixBuffIndex == BuffIndex.None) return;

            if (VoidBehavior == -1)
            {
                try
                {
                    self.AddItemBehavior<Starstorm2.Cores.Elites.VoidElite.AffixVoidBehavior>(self.HasBuff(ZetAspectsPlugin.StarVoidAffixBuffIndex) ? 1 : 0);
                    Debug.LogWarning("ZetAspect : StarCompat - VoidBehavior Validated");
                    VoidBehavior = 1;
                }
                catch (Exception e)
                {
                    Debug.LogWarning("ZetAspect : StarCompat - VoidBehavior Exception");
                    VoidBehavior = 0;
                    Debug.LogError(e);
                }
            }
            else if (VoidBehavior == 1)
            {
                self.AddItemBehavior<Starstorm2.Cores.Elites.VoidElite.AffixVoidBehavior>(self.HasBuff(ZetAspectsPlugin.StarVoidAffixBuffIndex) ? 1 : 0);
            }
        }
    }
}
