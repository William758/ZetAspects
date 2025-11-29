using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

using static TPDespair.ZetAspects.Catalog;

namespace TPDespair.ZetAspects
{
	public static class AspectPacks
	{
		public static void Init()
		{
			AspectPackDefOf.RiskOfRain = new AspectPack
			{
				identifier = "RiskOfRain",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectWhite",
						FindEquipmentDef = () => { return RoR2Content.Equipment.AffixWhite; },
						itemName = "ZetAspectWhite",
						displayPriority = 2500,
						BaseIcon = () => Sprites.AffixWhite,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectWhite.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixWhite = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixWhite = buffDef,
					},
					new AspectDef()
					{
						identifier = "ZetAspectBlue",
						FindEquipmentDef = () => { return RoR2Content.Equipment.AffixBlue; },
						itemName = "ZetAspectBlue",
						displayPriority = 2501,
						BaseIcon = () => Sprites.AffixBlue,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectBlue.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixBlue = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixBlue = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectRed",
						FindEquipmentDef = () => { return RoR2Content.Equipment.AffixRed; },
						itemName = "ZetAspectRed",
						displayPriority = 2502,
						BaseIcon = () => Sprites.AffixRed,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectRed.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixRed = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixRed = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectHaunted",
						FindEquipmentDef = () => { return RoR2Content.Equipment.AffixHaunted; },
						itemName = "ZetAspectHaunted",
						displayPriority = 7500,
						BaseIcon = () => Sprites.AffixHaunted,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectHaunted.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixHaunted = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixHaunted = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectPoison",
						FindEquipmentDef = () => { return RoR2Content.Equipment.AffixPoison; },
						itemName = "ZetAspectPoison",
						displayPriority = 7501,
						BaseIcon = () => Sprites.AffixPoison,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectPoison.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixPoison = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixPoison = buffDef,
						bodyBlacklist = new List<string>(){ "UrchinTurretBody", "AspectAbilitiesMalachiteUrchinOrbitalBody" }
					},
					new AspectDef()
					{
						identifier = "ZetAspectLunar",
						FindEquipmentDef = () => { return RoR2Content.Equipment.AffixLunar; },
						itemName = "ZetAspectLunar",
						displayPriority = 7550,
						BaseIcon = () => Sprites.AffixLunar,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectLunar.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixLunar = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixLunar = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectEarth",
						equipmentName = "EliteEarthEquipment",
						itemName = "ZetAspectEarth",
						displayPriority = 2525,
						BaseIcon = () => Sprites.AffixEarth,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectEarth.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixEarth = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixEarth = buffDef,
						bodyBlacklist = new List<string>(){ "AffixEarthHealerBody" }
					},
					new AspectDef()
					{
						identifier = "ZetAspectVoid",
						equipmentName = "EliteVoidEquipment",
						itemName = "ZetAspectVoid",
						displayPriority = 12500,
						disableDisplayOnDeath = true,
						copyEquipmentPrefab = false,
						BaseIcon = () => Sprites.AffixVoid,
						OutlineIcon = () => Configuration.AspectVoidContagiousItem.Value ? Sprites.OutlineVoid : Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectVoid.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixVoid = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixVoid = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectAurelionite",
						FindEquipmentDef = () => { return DLC2Content.Equipment.EliteAurelioniteEquipment; },
						itemName = "ZetAspectAurelionite",
						displayPriority = 3750,
						BaseIcon = () => Sprites.AffixAurelionite,
						OutlineIcon = () => Catalog.AspectVoidContagious ? Sprites.OutlineVoid : Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectAurelionite.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixAurelionite = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixAurelionite = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectBead",
						FindEquipmentDef = () => { return DLC2Content.Equipment.EliteBeadEquipment; },
						itemName = "ZetAspectBead",
						displayPriority = 7560,
						BaseIcon = () => Sprites.AffixBead,
						OutlineIcon = () => Catalog.AspectVoidContagious ? Sprites.OutlineVoid : Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectBead.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixBead = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixBead = buffDef
					}
				},
				PostInit = () =>
				{
					mithrixBodyIndex = BodyCatalog.FindBodyIndex("BrotherBody");
					voidlingBodyIndex = BodyCatalog.FindBodyIndex("VoidRaidCrabBody");
					//urchinTurretBodyIndex = BodyCatalog.FindBodyIndex("UrchinTurretBody");
					//healOrbBodyIndex = BodyCatalog.FindBodyIndex("AffixEarthHealerBody");
					artifactShellBodyIndex = BodyCatalog.FindBodyIndex("ArtifactShellBody");
					goldenTitanBodyIndex = BodyCatalog.FindBodyIndex("TitanGoldBody");

					BuffDef buffDef = RoR2Content.Buffs.AffixHauntedRecipient;
					buffDef.buffColor = Color.white;
					buffDef.iconSprite = Sprites.HauntCloak;
				}
			}.Register();



			AspectPackDefOf.SpikeStrip = new AspectPack
			{
				identifier = "SpikeStrip",
				dependency = "com.groovesalad.GrooveSaladSpikestripContent",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectPlated",
						equipmentName = "EQUIPMENT_AFFIXPLATED",
						itemName = "ZetAspectPlated",
						displayPriority = 3000,
						BaseIcon = () => Sprites.AffixPlated,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectPlated.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixPlated = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixPlated = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectWarped",
						equipmentName = "EQUIPMENT_AFFIXWARPED",
						itemName = "ZetAspectWarped",
						displayPriority = 3001,
						BaseIcon = () => Sprites.AffixWarped,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectWarped.SetupTokens,
						RefEquipmentDef =(equipmentDef) => EquipDefOf.AffixWarped = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixWarped = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectVeiled",
						equipmentName = "EQUIPMENT_AFFIXVEILED",
						itemName = "ZetAspectVeiled",
						displayPriority = 3002,
						BaseIcon = () => Sprites.AffixVeiled,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectVeiled.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixVeiled = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixVeiled = buffDef,
						OnRefresh = (body) =>
						{
							if (body.HasBuff(veiledBuffer))
							{
								body.AddTimedBuff(RoR2Content.Buffs.Cloak, 5f);
							}
						}
					},
					new AspectDef()
					{
						identifier = "ZetAspectAragonite",
						equipmentName = "EQUIPMENT_AFFIXARAGONITE",
						itemName = "ZetAspectAragonite",
						displayPriority = 9000,
						BaseIcon = () => Sprites.AffixAragonite,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectAragonite.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixAragonite = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixAragonite = buffDef
					}
				},
				PostInit = () =>
				{
					if (EquipDefOf.AffixVeiled != null)
					{
						// - if our cloak is expiring and we are veiled, refresh veiled to sync timers ???
						EliteBuffManager.MonitorBuff(RoR2Content.Buffs.Cloak.buffIndex);
						EliteBuffManager.MonitorTriggered += (manager, buffIndex) =>
						{
							if (buffIndex == RoR2Content.Buffs.Cloak.buffIndex && manager.body.HasBuff(BuffDefOf.AffixVeiled))
							{
								manager.AddBuffToRefresh(BuffDefOf.AffixVeiled.buffIndex);
							}
						};
					}
				}
			}.Register();



			AspectPackDefOf.GoldenCoastPlus = new AspectPack
			{
				identifier = "GoldenCoastPlus",
				CustomDependency = () => { return PluginLoaded("com.Skell.GoldenCoastPlus") || PluginLoaded("com.Phreel.GoldenCoastPlusRevived"); },
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectGold",
						equipmentName = "EliteGoldEquipment",
						itemName = "ZetAspectGold",
						displayPriority = 500,
						copyEquipmentPrefab = false,
						BaseIcon = () => Sprites.AffixGold,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectGold.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixGold = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixGold = buffDef
					}
				}
			}.Register();



			AspectPackDefOf.Aetherium = new AspectPack
			{
				identifier = "Aetherium",
				dependency = "com.KomradeSpectre.Aetherium",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectSanguine",
						equipmentName = "AETHERIUM_ELITE_EQUIPMENT_AFFIX_SANGUINE",
						itemName = "ZetAspectSanguine",
						displayPriority = 7000,
						BaseIcon = () => Sprites.AffixSanguine,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectSanguine.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixSanguine = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixSanguine = buffDef
					}
				}
			}.Register();



			AspectPackDefOf.Bubbet = new AspectPack
			{
				identifier = "Bubbet",
				dependency = "bubbet.bubbetsitems",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectSepia",
						equipmentName = "EquipmentDefSepiaElite",
						itemName = "ZetAspectSepia",
						buffName = "BuffDefSepia",
						displayPriority = 750,
						copyEquipmentPrefab = false,
						BaseIcon = () => Sprites.AffixSepia,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectSepia.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixSepia = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixSepia = buffDef
					}
				},
				PostInit = () =>
				{
					BuffDef buffDef = BuffDefOf.AffixSepia;
					if (buffDef)
					{
						buffDef.buffColor = Color.white;
						buffDef.iconSprite = Sprites.SepiaElite;
					}
				}
			}.Register();



			AspectPackDefOf.WarWisp = new AspectPack
			{
				identifier = "WarWisp",
				dependency = "com.PopcornFactory.WispMod",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectNullifier",
						equipmentName = "WARFRAMEWISP_ELITE_EQUIPMENT_AFFIX_NULLIFIER",
						itemName = "ZetAspectNullifier",
						displayPriority = 6500,
						BaseIcon = () => Sprites.AffixNullifier,
						OutlineIcon = () => Sprites.OutlineNull,
						SetupTokens = Items.ZetAspectNullifier.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixNullifier = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixNullifier = buffDef
					}
				}
			}.Register();



			AspectPackDefOf.Blighted = new AspectPack
			{
				identifier = "Blighted",
				dependency = "com.Moffein.BlightedElites",
				alwaysValidate = true,
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectBlighted",
						equipmentName = "AffixBlightedMoffein",
						itemName = "ZetAspectBlighted",
						displayPriority = 15000,
						BaseIcon = () => Sprites.AffixBlighted,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectBlighted.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixBlighted = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixBlighted = buffDef
					}
				},
				PostInit = () =>
				{
					if (Configuration.BlightedHooks.Value)
					{
						Compat.Blighted.LateSetup();
					}
				}
			}.Register();



			AspectPackDefOf.GOTCE = new AspectPack
			{
				identifier = "GOTCE",
				dependency = "com.TheBestAssociatedLargelyLudicrousSillyheadGroup.GOTCE",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectBackup",
						equipmentName = "ELITE_EQUIPMENT_BACKUP",
						itemName = "ZetAspectBackup",
						displayPriority = 1000,
						BaseIcon = () => Sprites.AffixBackup,
						OutlineIcon = () => Sprites.OutlineCracked,
						SetupTokens = Items.ZetAspectBackup.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixBackup = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixBackup = buffDef
					}
				},
				PostInit = () =>
				{
					BuffIndex index = BuffCatalog.FindBuffIndex("Backuped");
					if (index != BuffIndex.None) BuffDefOf.BackupDebuff = BuffCatalog.GetBuffDef(index);

					BuffDef buffDef = BuffDefOf.BackupDebuff;
					if (buffDef)
					{
						buffDef.buffColor = Color.white;
						buffDef.iconSprite = Sprites.BackupDebuff;
					}
				}
			}.Register();



			AspectPackDefOf.Thalasso = new AspectPack
			{
				identifier = "Thalasso",
				dependency = "com.jt_hehe.Thalassophobia",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectPurity",
						equipmentName = "THALASSOPHOBIA_ELITE_EQUIPMENT_AFFIX_PURE",
						itemName = "ZetAspectPurity",
						displayPriority = 3500,
						copyEquipmentPrefab = false,
						BaseIcon = () => Sprites.AffixPurity,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectPurity.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixPurity = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixPurity = buffDef
					}
				}
			}.Register();



			AspectPackDefOf.RisingTides = new AspectPack
			{
				identifier = "RisingTides",
				dependency = "com.themysticsword.risingtides",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectBarrier",
						equipmentName = "RisingTides_AffixBarrier",
						itemName = "ZetAspectBarrier",
						displayPriority = 10250,
						BaseIcon = () => Sprites.AffixBarrier,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectBarrier.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixBarrier = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixBarrier = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectBlackHole",
						equipmentName = "RisingTides_AffixBlackHole",
						itemName = "ZetAspectBlackHole",
						displayPriority = 10000,
						BaseIcon = () => Sprites.AffixBlackHole,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectBlackHole.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixBlackHole = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixBlackHole = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectMoney",
						equipmentName = "RisingTides_AffixMoney",
						itemName = "ZetAspectMoney",
						displayPriority = 4501,
						BaseIcon = () => Sprites.AffixMoney,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectMoney.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixMoney = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixMoney = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectNight",
						equipmentName = "RisingTides_AffixNight",
						itemName = "ZetAspectNight",
						displayPriority = 4500,
						BaseIcon = () => Sprites.AffixNight,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectNight.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixNight = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixNight = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectWater",
						equipmentName = "RisingTides_AffixWater",
						itemName = "ZetAspectWater",
						displayPriority = 10001,
						BaseIcon = () => Sprites.AffixWater,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectWater.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixWater = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixWater = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectRealgar",
						equipmentName = "RisingTides_AffixImpPlane",
						itemName = "ZetAspectRealgar",
						displayPriority = 10002,
						BaseIcon = () => Sprites.AffixRealgar,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectRealgar.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixRealgar = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixRealgar = buffDef
					}
				},
				PostInit = () =>
				{
					BuffIndex index = BuffCatalog.FindBuffIndex("RisingTides_NightSpeedBoost");
					if (index != BuffIndex.None) BuffDefOf.NightSpeed = BuffCatalog.GetBuffDef(index);

					index = BuffCatalog.FindBuffIndex("RisingTides_NightReducedVision");
					if (index != BuffIndex.None) BuffDefOf.NightBlind = BuffCatalog.GetBuffDef(index);
				}
			}.Register();



			AspectPackDefOf.NemRisingTides = new AspectPack
			{
				identifier = "NemRisingTides",
				dependency = "prodzpod.NemesisRisingTides",
				alwaysValidate = true,
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectBuffered",
						FindEquipmentDef = () =>
						{
							Compat.NemRisingTides.PrepareEquipmentCheck();

							EquipmentIndex index = EquipmentCatalog.FindEquipmentIndex("NemesisRisingTides_AffixBuffered");
							if (index != EquipmentIndex.None)
							{
								if (Compat.NemRisingTides.GetBufferedEnabled())
								{
									return EquipmentCatalog.GetEquipmentDef(index);
								}
							}

							return null;
						},
						itemName = "ZetAspectBuffered",
						displayPriority = 4000,
						BaseIcon = () => Sprites.AffixBuffered,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectBuffered.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixBuffered = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixBuffered = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectOppressive",
						FindEquipmentDef = () =>
						{
							Compat.NemRisingTides.PrepareEquipmentCheck();

							EquipmentIndex index = EquipmentCatalog.FindEquipmentIndex("NemesisRisingTides_AffixOppressive");
							if (index != EquipmentIndex.None)
							{
								if (Compat.NemRisingTides.GetOppressiveEnabled())
								{
									return EquipmentCatalog.GetEquipmentDef(index);
								}
							}

							return null;
						},
						itemName = "ZetAspectOppressive",
						displayPriority = 11000,
						BaseIcon = () => Sprites.AffixOppressive,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectOppressive.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixOppressive = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixOppressive = buffDef
					}
				}
			}.Register();



			AspectPackDefOf.MoreElites = new AspectPack
			{
				identifier = "MoreElites",
				CustomDependency = () => { return PluginLoaded("com.Nuxlar.MoreElites") || PluginLoaded("com.score.MoreElites"); },
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectEmpowering",
						equipmentName = "AffixEmpowering",
						itemName = "ZetAspectEmpowering",
						displayPriority = 1501,
						BaseIcon = () => Sprites.AffixBuffing,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectEmpowering.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixEmpowering = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixEmpowering = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectFrenzied",
						equipmentName = "AffixFrenziedNuxlar",
						itemName = "ZetAspectFrenzied",
						displayPriority = 1500,
						BaseIcon = () => Sprites.AffixFrenzied,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectFrenzied.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixFrenzied = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixFrenzied = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectVolatile",
						equipmentName = "AffixVolatile",
						itemName = "ZetAspectVolatile",
						displayPriority = 1502,
						BaseIcon = () => Sprites.AffixVolatile,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectVolatile.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixVolatile = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixVolatile = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectEcho",
						equipmentName = "AffixEcho",
						itemName = "ZetAspectEcho",
						displayPriority = 8000,
						BaseIcon = () => Sprites.AffixEcho,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectEcho.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixEcho = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixEcho = buffDef,
						AllowAffix = (body, inventory) =>
						{
							return inventory != null ? inventory.GetItemCount(summonedEcho) == 0 : false;
						},
						PreBuffGrant = (body, inventory) =>
						{
							body.AddTimedBuff(BuffDefOf.ZetEchoPrimer, 0.1f);

							return false;
						}
					}
				}
			}.Register();



			AspectPackDefOf.EliteVariety = new AspectPack
			{
				identifier = "EliteVariety",
				dependency = "com.themysticsword.elitevariety",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectArmor",
						equipmentName = "EliteVariety_AffixArmored",
						itemName = "ZetAspectArmor",
						displayPriority = 5001,
						BaseIcon = () => Sprites.AffixArmored,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectArmor.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixArmored = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixArmored = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectBanner",
						equipmentName = "EliteVariety_AffixBuffing",
						itemName = "ZetAspectBanner",
						displayPriority = 5100,
						BaseIcon = () => Sprites.AffixBuffing,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectBanner.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixBuffing = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixBuffing = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectImpale",
						equipmentName = "EliteVariety_AffixImpPlane",
						itemName = "ZetAspectImpale",
						displayPriority = 9500,
						BaseIcon = () => Sprites.AffixImpPlane,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectImpale.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixImpPlane = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixImpPlane = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectGolden",
						equipmentName = "EliteVariety_AffixPillaging",
						itemName = "ZetAspectGolden",
						displayPriority = 5000,
						BaseIcon = () => Sprites.AffixPillaging,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectGolden.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixPillaging = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixPillaging = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectCyclone",
						equipmentName = "EliteVariety_AffixSandstorm",
						itemName = "ZetAspectCyclone",
						displayPriority = 5200,
						BaseIcon = () => Sprites.AffixSandstorm,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectCyclone.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixSandstorm = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixSandstorm = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectTinker",
						equipmentName = "EliteVariety_AffixTinkerer",
						itemName = "ZetAspectTinker",
						displayPriority = 5250,
						BaseIcon = () => Sprites.AffixTinkerer,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectTinker.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixTinkerer = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixTinkerer = buffDef,
						bodyBlacklist = new List<string>(){ "EliteVariety_TinkererDroneBody" }
					}
				},
				PostInit = () =>
				{
					if (Configuration.EliteVarietyHooks.Value)
					{
						Compat.EliteVariety.LateSetup();
					}



					tinkerDroneBodyIndex = BodyCatalog.FindBodyIndex("EliteVariety_TinkererDroneBody");

					BuffIndex index = BuffCatalog.FindBuffIndex("EliteVariety_SandstormBlind");
					if (index != BuffIndex.None) BuffDefOf.SandBlind = BuffCatalog.GetBuffDef(index);
				}
			}.Register();



			AspectPackDefOf.Augmentum = new AspectPack
			{
				identifier = "Augmentum",
				dependency = "com.BrandonRosa.Augmentum",
				alwaysValidate = true,
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectAdaptive",
						equipmentName = "BRANS_ELITE_EQUIPMENT_AFFIX_ADAPTIVE",
						itemName = "ZetAspectAdaptive",
						displayPriority = 6000,
						BaseIcon = () => Sprites.AffixAdaptive,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectAdaptive.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixAdaptive = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixAdaptive = buffDef
					}
				},
				PostInit = () =>
				{
					if (Configuration.AugmentumHooks.Value)
					{
						Compat.Augmentum.LateSetup();
					}



					if (Compat.Augmentum.DeactivateAdaptiveDrop)
					{
						ItemIndex itemIndex = ItemCatalog.FindItemIndex("ITEM_ZETAFFIX_ADAPTIVE");
						ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
						if (itemDef != null)
						{
							Logger.Warn("Augmentum AspectPack PostInit - Deactivating : " + itemDef.name);

							if (itemDef._itemTierDef == Catalog.BossItemTier)
							{
								if (ItemCatalog.tier3ItemList.Contains(itemDef.itemIndex))
								{
									ItemCatalog.tier3ItemList.Remove(itemDef.itemIndex);
								}
							}

							itemDef.tier = ItemTier.NoTier;
							Catalog.AssignDepricatedTier(itemDef, ItemTier.NoTier);
							itemDef.hidden = true;
							if (itemDef.DoesNotContainTag(ItemTag.WorldUnique))
							{
								ItemTag[] tags = itemDef.tags;
								int index = tags.Length;

								Array.Resize(ref tags, index + 1);
								tags[index] = ItemTag.WorldUnique;

								itemDef.tags = tags;
							}
						}
					}
				}
			}.Register();



			AspectPackDefOf.Sandswept = new AspectPack
			{
				identifier = "Sandswept",
				dependency = "com.TeamSandswept.Sandswept",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectMotivator",
						equipmentName = "ELITE_EQUIPMENT_MOTIVATING",
						itemName = "ZetAspectMotivator",
						displayPriority = 5750,
						BaseIcon = () => Sprites.AffixMotivator,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectMotivator.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixMotivator = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixMotivator = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectOsmium",
						equipmentName = "ELITE_EQUIPMENT_OSMIUM",
						itemName = "ZetAspectOsmium",
						displayPriority = 13000,
						BaseIcon = () => Sprites.AffixOsmium,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectOsmium.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixOsmium = equipmentDef,
						RefBuffDef =(buffDef) => BuffDefOf.AffixOsmium = buffDef
					}
				}
			}.Register();



			AspectPackDefOf.Starstorm = new AspectPack
			{
				identifier = "Starstorm",
				dependency = "com.TeamMoonstorm",
				alwaysValidate = true,
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectEmpyrean",
						equipmentName = "AffixEmpyrean",
						itemName = "ZetAspectEmpyrean",
						displayPriority = 18000,
						BaseIcon = () => Sprites.AffixEmpyrean,
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectEmpyrean.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixEmpyrean = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixEmpyrean = buffDef
					}
				},
				PostInit = () =>
				{
					if (Configuration.StarstormHooks.Value)
					{
						Compat.Starstorm.LateSetup();
					}
				}
			}.Register();
		}
	}
}
