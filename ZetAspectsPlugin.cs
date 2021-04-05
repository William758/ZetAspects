using BepInEx;
using RoR2;
using R2API;
using R2API.Utils;
using System.Reflection;
using UnityEngine;

namespace TPDespair.ZetAspects
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [R2APISubmoduleDependency(nameof(LanguageAPI))]

    public class ZetAspectsPlugin : BaseUnityPlugin
    {
        public const string ModVer = "2.0.1";
        public const string ModName = "ZetAspects";
        public const string ModGuid = "com.TPDespair.ZetAspects";



        public static AssetBundle Assets;



        public void Awake()
        {
            LoadAssets();
            Configuration.Init(Config);
            Hooks.Init();
            ChangeText();
            ZetAspectsContent.Init();
        }
        /*
        public void Update()
        {
            DebugDrops();
        }
        */


        private static void LoadAssets()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ZetAspects.zetaspectbundle"))
            {
                Assets = AssetBundle.LoadFromStream(stream);
            }
        }

        private static void ChangeText()
        {
            LanguageAPI.Add("ITEM_SEED_DESC", "Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>" + Configuration.LeechSeedHeal.Value + "</style> <style=cStack>(+" + Configuration.LeechSeedHeal.Value + " per stack)</style> <style=cIsHealing>health</style>.");
            LanguageAPI.Add("ITEM_HEADHUNTER_DESC", "Gain the <style=cIsDamage>power</style> of any killed elite monster for <style=cIsDamage>" + Configuration.HeadHunterBaseDuration.Value + "s</style> <style=cStack>(+" + Configuration.HeadHunterStackDuration.Value + "s per stack)</style>.");
        }



        public static float GetStackMagnitude(CharacterBody self, BuffDef buffDef)
        {
            if (self.inventory == null) return 1f;

            float aspect = self.inventory.GetItemCount(GetAspectItemIndex(buffDef));
            float hunter = self.inventory.GetItemCount(RoR2Content.Items.HeadHunter);

            if (HasAspectEquipment(self, buffDef))
            {
                if (self.teamComponent.teamIndex == TeamIndex.Player)
                {
                    aspect += Configuration.AspectEquipmentEffect.Value;
                }
                else aspect += 1f;
            }

            if (aspect == 0f)
            {
                aspect = 1f;
                hunter = Mathf.Max(0f, hunter - 1f);
            }

            if (Configuration.HeadHunterCountExtra.Value >= 0)
            {
                hunter = Mathf.Min(hunter, Configuration.HeadHunterCountExtra.Value);
                hunter *= Configuration.HeadHunterExtraEffect.Value;
            }

            return aspect + hunter;
        }

        public static ItemIndex GetAspectItemIndex(BuffDef buffDef)
        {
            BuffIndex buffIndex = buffDef.buffIndex;

            if (buffIndex == BuffIndex.None) return ItemIndex.None;

            if (buffIndex == RoR2Content.Buffs.AffixWhite.buffIndex) return ZetAspectsContent.Items.ZetAspectIce.itemIndex;
            if (buffIndex == RoR2Content.Buffs.AffixBlue.buffIndex) return ZetAspectsContent.Items.ZetAspectLightning.itemIndex;
            if (buffIndex == RoR2Content.Buffs.AffixRed.buffIndex) return ZetAspectsContent.Items.ZetAspectFire.itemIndex;
            if (buffIndex == RoR2Content.Buffs.AffixHaunted.buffIndex) return ZetAspectsContent.Items.ZetAspectCelestial.itemIndex;
            if (buffIndex == RoR2Content.Buffs.AffixPoison.buffIndex) return ZetAspectsContent.Items.ZetAspectMalachite.itemIndex;

            return ItemIndex.None;
        }

        public static bool HasAspectEquipment(CharacterBody self, BuffDef buffDef)
        {
            EquipmentIndex aspectEquip = GetAspectEquipmentIndex(buffDef);

            if (aspectEquip == EquipmentIndex.None) return false;
            if (self.inventory.currentEquipmentIndex == aspectEquip) return true;
            if (self.inventory.alternateEquipmentIndex == aspectEquip) return true;

            return false;
        }

        public static EquipmentIndex GetAspectEquipmentIndex(BuffDef buffDef)
        {
            BuffIndex buffIndex = buffDef.buffIndex;

            if (buffIndex == BuffIndex.None) return EquipmentIndex.None;

            if (buffIndex == RoR2Content.Buffs.AffixWhite.buffIndex) return RoR2Content.Equipment.AffixWhite.equipmentIndex;
            if (buffIndex == RoR2Content.Buffs.AffixBlue.buffIndex) return RoR2Content.Equipment.AffixBlue.equipmentIndex;
            if (buffIndex == RoR2Content.Buffs.AffixRed.buffIndex) return RoR2Content.Equipment.AffixRed.equipmentIndex;
            if (buffIndex == RoR2Content.Buffs.AffixHaunted.buffIndex) return RoR2Content.Equipment.AffixHaunted.equipmentIndex;
            if (buffIndex == RoR2Content.Buffs.AffixPoison.buffIndex) return RoR2Content.Equipment.AffixPoison.equipmentIndex;

            return EquipmentIndex.None;
        }



        public static string FormatSeconds(float seconds)
        {
            string s = seconds == 1.0f ? "" : "s";
            return seconds + " second" + s;
        }

        public static void RegisterLanguageToken(string token, string text)
        {
            LanguageAPI.Add(token, text);
        }


        /*
        private void DebugDrops()
        {
            bool debugDrops = true;

            if (debugDrops)
            {
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                    if (!Configuration.AspectEliteEquipment.Value)
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectIce.itemIndex), transform.position, transform.forward * 30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectLightning.itemIndex), transform.position, transform.forward * 60f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectFire.itemIndex), transform.position, transform.right * 30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectCelestial.itemIndex), transform.position, transform.forward * -30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(ZetAspectsContent.Items.ZetAspectMalachite.itemIndex), transform.position, transform.forward * -60f);
                    }
                    else
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixWhite.equipmentIndex), transform.position, transform.forward * 30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixBlue.equipmentIndex), transform.position, transform.forward * 60f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixRed.equipmentIndex), transform.position, transform.right * 30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixHaunted.equipmentIndex), transform.position, transform.forward * -30f);
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Equipment.AffixPoison.equipmentIndex), transform.position, transform.forward * -60f);
                    }
                }

                if (Input.GetKeyDown(KeyCode.F3))
                {
                    var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Items.ShieldOnly.itemIndex), transform.position, transform.forward * 30f);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Items.RepeatHeal.itemIndex), transform.position, transform.forward * -30f);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Items.HeadHunter.itemIndex), transform.position, transform.right * 30f);
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarDagger.itemIndex), transform.position, transform.right * -30f);
                }
            }
        }
        */
    }
}
