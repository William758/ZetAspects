using BepInEx;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TPDespair.ZetAspects
{
    [BepInPlugin(ModGuid, ModName, ModVer)]

    public class ZetAspectsPlugin : BaseUnityPlugin
    {
        public const string ModVer = "2.1.0";
        public const string ModName = "ZetAspects";
        public const string ModGuid = "com.TPDespair.ZetAspects";



        public static AssetBundle Assets;

        public static Dictionary<string, string> LangTokens = new Dictionary<string, string>();



        public void Awake()
        {
            RoR2Application.isModded = true;
            NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(ModGuid + ":" + ModVer);

            Configuration.Init(Config);
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;

            Hooks.Init();
            LanguageOverride();

            ChangeText();
        }
        /*
        public void Update()
        {
            DebugDrops();
        }
        */


        internal static void LoadAssets()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ZetAspects.zetaspectbundle"))
            {
                Assets = AssetBundle.LoadFromStream(stream);
            }
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ZetAspectsContent());
        }

        private static void LanguageOverride()
        {
            On.RoR2.Language.TokenIsRegistered += (orig, self, token) =>
            {
                if (LangTokens.ContainsKey(token)) return true;

                return orig(self, token);
            };

            On.RoR2.Language.GetString_string += (orig, token) =>
            {
                if (LangTokens.ContainsKey(token)) return LangTokens[token];

                return orig(token);
            };
        }



        private static void ChangeText()
        {
            string text;
            RegisterLanguageToken("ITEM_SEED_DESC", "Dealing damage <style=cIsHealing>heals</style> you for <style=cIsHealing>" + Configuration.LeechSeedHeal.Value + "</style> <style=cStack>(+" + Configuration.LeechSeedHeal.Value + " per stack)</style> <style=cIsHealing>health</style>.");
            RegisterLanguageToken("ITEM_HEADHUNTER_DESC", "Gain the <style=cIsDamage>power</style> of any killed elite monster for <style=cIsDamage>" + Configuration.HeadHunterBaseDuration.Value + "s</style> <style=cStack>(+" + Configuration.HeadHunterStackDuration.Value + "s per stack)</style>.");
            text = "Convert <style=cIsHealing>100%</style> of health into <style=cIsHealing>regenerating shields</style>.\nGain <style=cIsHealing>50%</style> <style=cStack>(+25% per stack)</style> extra <style=cIsHealing>shield</style> from conversion.";
            if (Configuration.TranscendenceRegen.Value > 0f)
            {
                text += "\nAt least <style=cIsHealing>";
                text += Configuration.TranscendenceRegen.Value * 100f + "%</style>";
                text += " of <style=cIsHealing>health regeneration</style> applies to <style=cIsHealing>shields</style>.";
            }
            RegisterLanguageToken("ITEM_SHIELDONLY_DESC", text);
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

            if (Configuration.HeadHunterCountExtra.Value != 0)
            {
                if (Configuration.HeadHunterCountExtra.Value > 0) 
                {
                    hunter = Mathf.Min(hunter, Configuration.HeadHunterCountExtra.Value);
                }
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
            if (buffIndex == RoR2Content.Buffs.AffixLunar.buffIndex) return ZetAspectsContent.Items.ZetAspectPerfect.itemIndex;

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
            if (buffIndex == RoR2Content.Buffs.AffixLunar.buffIndex) return RoR2Content.Equipment.AffixLunar.equipmentIndex;

            return EquipmentIndex.None;
        }



        public static string FormatSeconds(float seconds)
        {
            string s = seconds == 1.0f ? "" : "s";
            return seconds + " second" + s;
        }

        public static void RegisterLanguageToken(string token, string text)
        {
            //LanguageAPI.Add(token, text);
            if (!LangTokens.ContainsKey(token))
            {
                LangTokens.Add(token, text);
            }
            else
            {
                LangTokens[token] = text;
            }
        }


        /*
        private static void DebugDrops()
        {
            bool debugDrops = true;

            if (debugDrops)
            {
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

                    if (!Configuration.AspectEliteEquipment.Value)
                    {
                        CreateDroplet(ZetAspectsContent.Items.ZetAspectIce, transform.position + new Vector3(-5f,5f,5f));
                        CreateDroplet(ZetAspectsContent.Items.ZetAspectLightning, transform.position + new Vector3(0f, 5f, 7.5f));
                        CreateDroplet(ZetAspectsContent.Items.ZetAspectFire, transform.position + new Vector3(5f, 5f, 5f));
                        CreateDroplet(ZetAspectsContent.Items.ZetAspectCelestial, transform.position + new Vector3(-5f, 5f, -5f));
                        CreateDroplet(ZetAspectsContent.Items.ZetAspectMalachite, transform.position + new Vector3(0f, 5f, -7.5f));
                        CreateDroplet(ZetAspectsContent.Items.ZetAspectPerfect, transform.position + new Vector3(5f, 5f, -5f));
                    }
                    else
                    {
                        CreateDroplet(RoR2Content.Equipment.AffixWhite, transform.position + new Vector3(-5f, 5f, 5f));
                        CreateDroplet(RoR2Content.Equipment.AffixBlue, transform.position + new Vector3(0f, 5f, 7.5f));
                        CreateDroplet(RoR2Content.Equipment.AffixRed, transform.position + new Vector3(5f, 5f, 5f));
                        CreateDroplet(RoR2Content.Equipment.AffixHaunted, transform.position + new Vector3(-5f, 5f, -5f));
                        CreateDroplet(RoR2Content.Equipment.AffixPoison, transform.position + new Vector3(0f, 5f, -7.5f));
                        CreateDroplet(RoR2Content.Equipment.AffixLunar, transform.position + new Vector3(5f, 5f, -5f));
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

                if (Input.GetKeyDown(KeyCode.F4))
                {
                    var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(RoR2Content.Items.Knurl.itemIndex), transform.position, transform.forward * 30f);
                }
            }
        }

        private static void CreateDroplet(ItemDef def, Vector3 pos)
        {
            CreateDroplet(PickupCatalog.FindPickupIndex(def.itemIndex), pos);
        }

        private static void CreateDroplet(EquipmentDef def, Vector3 pos)
        {
            CreateDroplet(PickupCatalog.FindPickupIndex(def.equipmentIndex), pos);
        }

        private static void CreateDroplet(PickupIndex index, Vector3 pos)
        {
            PickupDropletController.CreatePickupDroplet(index, pos, Vector3.zero);
        }
        */
    }
}
