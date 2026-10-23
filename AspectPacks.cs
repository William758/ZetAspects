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
						OutlineIcon = () => Sprites.OutlineStandard,
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
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectBead.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixBead = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixBead = buffDef
					},
					new AspectDef()
					{
						identifier = "ZetAspectCollective",
						FindEquipmentDef = () => { return DLC3Content.Equipment.EliteCollectiveEquipment; },
						itemName = "ZetAspectCollective",
						displayPriority = 3600,
						BaseIcon = () => 
						{
							Sprite sprite = Catalog.TryUseOwnSprite();

							return Catalog.TrimSpriteEdge(sprite, 8, 24);
						},
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = Items.ZetAspectCollective.SetupTokens,
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixCollective = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixCollective = buffDef
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
			}.With(
				new SimpleAspect()
				{
					identifier = "ZetAspectStygian",
					stats = new[] { new AspectStat(AspectStatType.Armor, 20f, 10f) },
					affix = "STYGIAN",
					FindEquipmentDef = () => { return DLC4Content.Equipment.EliteDeathEquipment; },
					displayPriority = 7600,
					// brighter N'kuhana's Retort
					icon = () => AdjustSprite(Sprites.AffixPoison, null, 1.3f),
					aspectOf = "<style=cDeath>Aspect of Death</style> :",
					passive = "\nCharacters that die near you <style=cIsUtility>rise again as ghosts</style> that fight for you for a short time."
				}
			).Register();



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
						Promoter = (body) =>
						{
							if (body.HasBuff(veiledBuffer))
							{
								BuffIndex[] buffs = { RoR2Content.Buffs.Cloak.buffIndex };
								return buffs;
							}

							return null;
						},
						OnRefresh = (body) =>
						{
							if (body.HasBuff(veiledBuffer))
							{
								body.AddTimedBuff(RoR2Content.Buffs.Cloak, 5f);
							}
						},
						OnExpire = (body) =>
						{
							for (int i = 0; i < 2; i++)
							{
								if (body.HasBuff(veiledBuffer))
								{
									body.RemoveBuff(veiledBuffer);
								}
							}

							if (body.HasBuff(RoR2Content.Buffs.Cloak))
							{
								body.ClearTimedBuffs(RoR2Content.Buffs.Cloak);
							}

							if (body.HasBuff(BuffDefOf.ZetElusive))
							{
								body.SetBuffCount(BuffDefOf.ZetElusive.buffIndex, 0);
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
				CustomDependency = () => { return AnyPluginLoaded("com.Skell.GoldenCoastPlus", "com.Phreel.GoldenCoastPlusRevived", "com.TechDebtCollector.GoldenCoastPlus"); },
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
				// Nullifying elites were split out of Thalassophobia into their own mod
				identifier = "Thalasso",
				CustomDependency = () => { return AnyPluginLoaded("Phreel.NullifyingElite", "com.jt_hehe.Thalassophobia"); },
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectPurity",
						FindEquipmentDef = () =>
						{
							EquipmentIndex index = EquipmentCatalog.FindEquipmentIndex("EliteNullifyingEquipment");
							if (index == EquipmentIndex.None) index = EquipmentCatalog.FindEquipmentIndex("THALASSOPHOBIA_ELITE_EQUIPMENT_AFFIX_PURE");

							return EquipmentCatalog.GetEquipmentDef(index);
						},
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
			}.With(
				new SimpleAspect()
				{
					identifier = "ZetAspectPhenakite",
					stats = new[] { new AspectStat(AspectStatType.MoveSpeed, 0.15f, 0.10f) },
					affix = "PHENAKITE",
					equipmentName = "RisingTides_AffixMirror",
					displayPriority = 10003,
					fallbackIcon = () => Sprites.AffixHaunted,
					name = "Hall of Mirrors",
					pickup = "Become an aspect of illusion.",
					aspectOf = "<style=cDeath>Aspect of Illusion</style> :",
					passive = "\nSpawn with <style=cIsUtility>{0} illusory clones</style> of yourself.\nAttacks have a <style=cIsUtility>{1}%</style> chance to <style=cIsUtility>steal an item</style> from enemies you hit.",
					passiveArgs = () => new object[] {
						ModConfig("com.themysticsword.risingtides", "Elites: Phenakite", "Clone Count", 3),
						ModConfig("com.themysticsword.risingtides", "Elites: Phenakite", "Item Steal Chance", 100f)
					}
				}
			).Register();



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
							return inventory != null ? inventory.GetItemCountEffective(summonedEcho) == 0 : false;
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
						BaseIcon = () => Sprites.AffixBuffing_EV,
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
				CustomDependency = () => { return AnyPluginLoaded(Compat.Starstorm.GUIDs); },
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
			}.With(
				new SimpleAspect()
				{
					identifier = "ZetAspectEthereal",
					stats = new[] { new AspectStat(AspectStatType.Health, 0.15f, 0.10f) },
					affix = "SS2ETHEREAL",
					equipmentName = "AffixEthereal",
					displayPriority = 17000,
					icon = () => AdjustSprite(Sprites.AffixHaunted, new Color(0.2f, 0.8f, 0.3f), 0.5f),
					name = "Unstable Remnant",
					pickup = "Become an aspect of the unknown.",
					aspectOf = "<style=cDeath>Aspect of the Unknown</style> :",
					passive = "\nPeriodically create <style=cIsDamage>ethereal circles</style> nearby that detonate on enemies."
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectToxic",
					stats = new[] { new AspectStat(AspectStatType.Damage, 0.12f, 0.08f) },
					affix = "SS2TOXIC",
					equipmentName = "ElitePurpleEquipment",
					displayPriority = 7502,
					fallbackIcon = () => Sprites.AffixPoison,
					name = "Malignant Resolve",
					pickup = "Become an aspect of toxicity.",
					aspectOf = "<style=cDeath>Aspect of Toxicity</style> :",
					passive = "\nAttacks <style=cIsDamage>explode</style> into a toxic cloud that <style=cIsDamage>poisons</style> enemies, dealing more damage the healthier they are.\nPeriodically lob <style=cIsDamage>toxic bombs</style> at nearby enemies."
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectTectonic",
					stats = new[] { new AspectStat(AspectStatType.Armor, 25f, 15f) },
					affix = "SS2TECTONIC",
					equipmentName = "EliteSuperEarthEquipment",
					displayPriority = 16000,
					defence = true,
					// far darker His Reassurance
					icon = () => AdjustSprite(Sprites.AffixEarth, null, 0.45f),
					name = "His Radiant Passion",
					pickup = "Become a harbinger of earth.",
					aspectOf = "<style=cDeath>Harbinger of Earth</style> :",
					passive = "\nGain the power of <style=cIsHealing>Mending</style> elites.\nEvery <style=cIsDamage>8 seconds</style>, send out <style=cIsDamage>3 sundering waves</style> for <style=cIsDamage>100%</style> damage.",
					configure = (aspectDef) => aspectDef.GrantsBuffs(() => DLC1Content.Buffs.EliteEarth)
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectInfernal",
					stats = new[] { new AspectStat(AspectStatType.Damage, 0.20f, 0.10f) },
					affix = "SS2INFERNAL",
					equipmentName = "EliteSuperFireEquipment",
					displayPriority = 16001,
					// darker Ifrit's Distinction
					icon = () => AdjustSprite(Sprites.AffixRed, null, 0.8f),
					name = "Royal Flame",
					pickup = "Become a harbinger of fire.",
					aspectOf = "<style=cDeath>Harbinger of Fire</style> :",
					passive = "\nGain the power of <style=cIsDamage>Blazing</style> elites.\nEvery <style=cIsDamage>8 seconds</style>, launch a <style=cIsDamage>giant fireball</style> for <style=cIsDamage>200%</style> damage.",
					configure = (aspectDef) => aspectDef.GrantsBuffs(() => RoR2Content.Buffs.AffixRed)
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectBoreal",
					stats = new[] { new AspectStat(AspectStatType.Health, 0.20f, 0.10f) },
					affix = "SS2BOREAL",
					equipmentName = "EliteSuperIceEquipment",
					displayPriority = 16002,
					// similar to Of One Mind
					icon = () => EquipmentIcon(DLC3Content.Equipment.EliteCollectiveEquipment) ?? Sprites.AffixHaunted,
					name = "Her Cosmic Brilliance",
					pickup = "Become a harbinger of ice.",
					aspectOf = "<style=cDeath>Harbinger of Ice</style> :",
					passive = "\nGain the power of <style=cIsUtility>Glacial</style> elites.\nEvery <style=cIsDamage>4 seconds</style>, call down an <style=cIsDamage>ice meteor</style> nearby for <style=cIsDamage>100%</style> damage.",
					configure = (aspectDef) => aspectDef.GrantsBuffs(() => RoR2Content.Buffs.AffixWhite)
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectSupercharged",
					stats = new[] { new AspectStat(AspectStatType.AttackSpeed, 0.20f, 0.10f) },
					affix = "SS2SUPERCHARGED",
					equipmentName = "EliteSuperLightningEquipment",
					displayPriority = 16003,
					// slightly darker His Spiteful Boon
					icon = () => AdjustSprite(Sprites.AffixBead, null, 0.9f),
					name = "Eye of the Storm",
					pickup = "Become a harbinger of lightning.",
					aspectOf = "<style=cDeath>Harbinger of Lightning</style> :",
					passive = "\nGain the power of <style=cIsDamage>Overloading</style> elites.\nEvery <style=cIsUtility>4 seconds</style>, supercharge a nearby ally <style=cStack>(or yourself)</style>, granting <style=cIsDamage>+100% attack speed</style>, <style=cIsUtility>+100% movement speed</style> and <style=cIsUtility>greatly reduced cooldowns</style> for <style=cIsUtility>13 seconds</style>.",
					configure = (aspectDef) => aspectDef.GrantsBuffs(() => RoR2Content.Buffs.AffixBlue)
				}
			).Register();



			AspectPackDefOf.Meltdown = new AspectPack
			{
				identifier = "Meltdown",
				dependency = "com.pittabread.Meltdown",
				aspectDefs = new List<AspectDef>()
				{
					new AspectDef()
					{
						identifier = "ZetAspectNuclear",
						equipmentName = "ELITE_EQUIPMENT_NUCLEAR",
						itemName = "ZetAspectNuclear",
						displayPriority = 235,
						// Meltdown's icon is a recolored N'kuhana's Retort
						BaseIcon = () => Catalog.OwnSpriteOrRecolor(() => Sprites.AffixEarth, new Color(0.8f, 0.95f, 0.5f)),
						OutlineIcon = () => Sprites.OutlineStandard,
						SetupTokens = () =>
						{
							Catalog.SetupTokenBase("ZetAspectNuclear", "Nuclear");
							Catalog.AspectDescriptionBuilder((combine) =>
							{
								string output = Language.TextFragment("ASPECT_OF_RADIATION");
								output += Language.TextFragment("SLOWBLAST_NEARBY");
								output += String.Format(
									Language.TextFragment("IRRADIATE_DOT"),
									Language.ScalingText(0.5f, 0f, "percent", "cIsDamage", combine),
									Language.TextFragment("TOTAL_DAMAGE"),
									Language.SecondText(4f, "over")
								);

								return output;
							});
						},
						RefEquipmentDef = (equipmentDef) => EquipDefOf.AffixNuclear = equipmentDef,
						RefBuffDef = (buffDef) => BuffDefOf.AffixNuclear = buffDef
					}
				},
				ContentLoad = () =>
				{
					Language.RegisterFragmentNoOverwrite("AFFIX_NUCLEAR_NAME", "Elephant's Foot");
					Language.RegisterFragmentNoOverwrite("AFFIX_NUCLEAR_PICKUP", "Become an aspect of radiation.");
					Language.RegisterFragmentNoOverwrite("AFFIX_NUCLEAR_ACTIVE", "<style=cStack>(???)</style>");
					Language.RegisterFragmentNoOverwrite("ASPECT_OF_RADIATION", "<style=cDeath>Aspect of Radiation</style> :");
					Language.RegisterFragmentNoOverwrite("SLOWBLAST_NEARBY", "\nPeriodically blast nearby enemies, slowing them.");
					Language.RegisterFragmentNoOverwrite("IRRADIATE_DOT", "\nAttacks <style=cIsDamage>irradiate</style> on hit for {0} {1} damage {2}.");

					ItemDef ZetAspectNuclear = Catalog.CreateItemTemplate("ZetAspectNuclear", true, false);
					ItemDefOf.ZetAspectNuclear = ZetAspectNuclear;

					ZetAspectsContent.itemDefs.Add(ZetAspectNuclear);
					transformableAspectItemDefs.Add(ZetAspectNuclear);
				}
			}.Register();



			AspectPackDefOf.RecoveredAndReformed = new AspectPack
			{
				identifier = "RecoveredAndReformed",
				dependency = "prodzpod.RecoveredAndReformed"
			}.With(
				new SimpleAspect()
				{
					identifier = "ZetAspectSpeedy",
					stats = new[] { new AspectStat(AspectStatType.AttackSpeed, 0.15f, 0.10f) },
					affix = "SECRETSPEED",
					equipmentName = "EliteSecretSpeedEquipment",
					displayPriority = 2510,
					// keep the bunny headband from the vanilla icon, but make its orange outline yellow
					icon = () => ReplaceEdgeHue(GetOwnSprite() ?? Sprites.AffixRed, 0.03f, 0.12f, 0.15f, 6),
					configure = (aspectDef) => aspectDef.useOutline = false,
					name = "Beyond the Limits",
					pickup = "Become an aspect of speed.",
					aspectOf = "<style=cDeath>Aspect of Speed</style> :",
					passive = "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>{0}%</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>{1}%</style>.",
					passiveArgs = () => new object[] {
						100f * ModStaticField("prodzpod.RecoveredAndReformed", "RecoveredAndReformed.Recovered", "SecretSpeedMoveSpeed", 1f),
						100f * ModStaticField("prodzpod.RecoveredAndReformed", "RecoveredAndReformed.Recovered", "SecretSpeedAttackSpeed", 0.5f)
					}
				}
			).Register();



			AspectPackDefOf.Expedition = new AspectPack
			{
				identifier = "Expedition",
				dependency = "ExpeditionCrew.EXPEDITION"
			}.With(
				new SimpleAspect()
				{
					identifier = "ZetAspectPrimal",
					stats = new[] { new AspectStat(AspectStatType.AttackSpeed, 0.15f, 0.10f) },
					affix = "INSTINCT",
					equipmentName = "ELITE_EQUIPMENT_INSTINCT_NAME",
					displayPriority = 2520,
					fallbackIcon = () => Sprites.AffixRed,
					name = "Primal Call",
					pickup = "Become an aspect of wildness.",
					aspectOf = "<style=cDeath>Aspect of Wildness</style> :",
					passive = "\nAttacks <style=cIsUtility>injure</style> enemies, preventing them from sprinting for <style=cIsUtility>2 seconds</style>.\nWhile below <style=cIsHealth>50% health</style>, gain <style=cIsDamage>+50% attack speed</style>, <style=cIsUtility>+50% movement speed</style> and become <style=cIsUtility>immune to stuns</style>."
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectUnstable",
					stats = new[] { new AspectStat(AspectStatType.Armor, 20f, 10f) },
					affix = "EXPUNSTABLE",
					equipmentName = "ELITE_EQUIPMENT_UNSTABLE_NAME",
					displayPriority = 14000,
					offense = false,
					defence = true,
					fallbackIcon = () => Sprites.AffixHaunted,
					name = "Localized Anomaly",
					pickup = "Become an aspect of chaos.",
					aspectOf = "<style=cDeath>Aspect of Chaos</style> :",
					passive = "\nPeriodically <style=cIsUtility>teleport</style> a nearby ally to your target."
				}
			).Register();



			AspectPackDefOf.LostInTransitLegacy = new AspectPack
			{
				identifier = "LostInTransitLegacy",
				dependency = "Phreelosu.LostInTransitLegacy"
			}.With(
				new SimpleAspect()
				{
					identifier = "ZetAspectFrenzying",
					stats = new[] { new AspectStat(AspectStatType.MoveSpeed, 0.15f, 0.10f) },
					affix = "LITFRENZIED",
					equipmentName = "LITLegacyAffixFRENZIED",
					displayPriority = 1550,
					fallbackIcon = () => Sprites.AffixRed,
					name = "Primordial Rage",
					pickup = "Become an aspect of seething.",
					aspectOf = "<style=cDeath>Aspect of Seething</style> :",
					passive = "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>{0}%</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>{1}%</style>.\nReduces <style=cIsUtility>skill cooldowns</style> by <style=cIsUtility>{2}%</style>.\nEvery <style=cIsUtility>{3} seconds</style>, press <style=cIsUtility>{4}</style> to <style=cIsUtility>blink</style> forward.",
					passiveArgs = () => new object[] {
						100f * ModConfig("Phreelosu.LostInTransitLegacy", "Elite: Frenzying", "Movement Speed Bonus", 1f),
						100f * ModConfig("Phreelosu.LostInTransitLegacy", "Elite: Frenzying", "Attack Speed Bonus", 1f),
						100f * ModConfig("Phreelosu.LostInTransitLegacy", "Elite: Frenzying", "Cooldown Reduction", 0.5f),
						ModConfig("Phreelosu.LostInTransitLegacy", "Elite: Frenzying", "Blink Cooldown", 10f),
						ModConfig("Phreelosu.LostInTransitLegacy", "Elite: Frenzying", "Player Blink Key", KeyCode.F)
					}
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectLeeching",
					stats = new[] { new AspectStat(AspectStatType.Regen, 4f, 2f) },
					affix = "LITLEECHING",
					equipmentName = "LITLegacyAffixLEECHING",
					displayPriority = 1551,
					defence = true,
					fallbackIcon = () => Sprites.AffixPoison,
					name = "Guttural Whimpers",
					pickup = "Become an aspect of parasitism.",
					aspectOf = "<style=cDeath>Aspect of Parasitism</style> :",
					passive = "\n<style=cIsHealing>Heal</style> for <style=cIsHealing>{0}%</style> of the damage you deal.\nEvery <style=cIsUtility>{1} seconds</style>, charge a burst that <style=cIsHealing>regenerates</style> up to <style=cIsHealing>{2}%</style> of the maximum health of injured allies within <style=cIsUtility>{3}m</style>.",
					passiveArgs = () => new object[] {
						100f * ModConfig("Phreelosu.LostInTransitLegacy", "Elite: Leeching", "Player Lifesteal", 0.5f),
						ModConfig("Phreelosu.LostInTransitLegacy", "Elite: Leeching", "Time Between Bursts", 10f),
						ModConfig("Phreelosu.LostInTransitLegacy", "Elite: Leeching", "Burst Heal Percentage", 10f),
						ModConfig("Phreelosu.LostInTransitLegacy", "Elite: Leeching", "Burst Radius", 25f)
					}
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectLITVolatile",
					stats = new[] { new AspectStat(AspectStatType.Damage, 0.15f, 0.10f) },
					affix = "LITVOLATILE",
					equipmentName = "LITLegacyAffixVOLATILE",
					displayPriority = 1552,
					fallbackIcon = () => Sprites.AffixRed,
					name = "Lingering Torment",
					pickup = "Become an aspect of instability.",
					aspectOf = "<style=cDeath>Aspect of Instability</style> :",
					passive = "\nAttacks <style=cIsDamage>explode</style> on hit for <style=cIsDamage>{0}%</style> of the damage dealt.\nTaking heavy damage drops <style=cIsDamage>spite bombs</style> around you.",
					passiveArgs = () => new object[] {
						100f * ModConfig("Phreelosu.LostInTransitLegacy", "Elite: Volatile", "Explosion Damage", 0.3f)
					}
				}
			).Register();



			AspectPackDefOf.SivsContentPack = new AspectPack
			{
				identifier = "SivsContentPack",
				dependency = "Sivelos.SivsContentPack"
			}.With(
				new SimpleAspect()
				{
					identifier = "ZetAspectObscuring",
					stats = new[] { new AspectStat(AspectStatType.MoveSpeed, 0.15f, 0.10f) },
					affix = "SIVSHADOW",
					equipmentName = "eqEliteShadow",
					displayPriority = 7700,
					offense = false,
					defence = true,
					fallbackIcon = () => Sprites.AffixHaunted,
					name = "What Waits In The Shadows",
					pickup = "Become an aspect of darkness.",
					aspectOf = "<style=cDeath>Aspect of Darkness</style> :",
					passive = "\nAttacks <style=cIsUtility>blind</style> enemies for <style=cIsUtility>{0} seconds</style>.\nWhile out of combat, <style=cIsUtility>turn invisible</style>.",
					passiveArgs = () => new object[] {
						ModStaticField("Sivelos.SivsContentPack", "SivsContentPack.Config.Configuration+Elites+Obscuring", "blindnessDuration", 10f)
					}
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectTarSymbiont",
					stats = new[] { new AspectStat(AspectStatType.Health, 0.15f, 0.10f) },
					affix = "SIVTAR",
					equipmentName = "eqEliteTar",
					displayPriority = 7701,
					defence = true,
					fallbackIcon = () => Sprites.AffixPoison,
					name = "Aphelian Sin",
					pickup = "Become an aspect of tar.",
					aspectOf = "<style=cDeath>Aspect of Tar</style> :",
					passive = "\nAttacks cover enemies in <style=cIsUtility>tar</style> for <style=cIsUtility>{0} seconds</style>.\n<style=cIsHealing>Heal</style> for <style=cIsHealing>{1}%</style> of the damage you deal.",
					passiveArgs = () => new object[] {
						ModStaticField("Sivelos.SivsContentPack", "SivsContentPack.Config.Configuration+Elites+Tar", "tarDuration", 5f),
						100f * ModStaticField("Sivelos.SivsContentPack", "SivsContentPack.Config.Configuration+Elites+Tar", "lifeStealCoefficient", 1f)
					}
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectPrototype",
					stats = new[] { new AspectStat(AspectStatType.Damage, 0.20f, 0.10f) },
					affix = "SIVPROTOTYPE",
					equipmentName = "eqEliteUnstable",
					displayPriority = 7702,
					// darker Shared Design
					icon = () => AdjustSprite(Sprites.AffixLunar, null, 0.65f),
					name = "Unfinished Design",
					pickup = "Become an aspect of design.",
					aspectOf = "<style=cDeath>Aspect of Design</style> :",
					passive = "\nIncreases <style=cIsDamage>damage</style> by <style=cIsDamage>{0}%</style>, but <style=cDeath>reduces maximum health by {1}%</style>.\nEvery <style=cIsUtility>{2} seconds</style>, launch <style=cIsDamage>{3}</style> fireballs for <style=cIsDamage>{4}%</style> damage each.",
					passiveArgs = () => new object[] {
						100f * ModStaticField("Sivelos.SivsContentPack", "SivsContentPack.Config.Configuration+Elites+Unstable", "damageMult", 1f),
						// curse divides max health by (1 + curse)
						Mathf.Round(100f * (1f - 1f / (1f + ModStaticField("Sivelos.SivsContentPack", "SivsContentPack.Config.Configuration+Elites+Unstable", "healthMult", 0.5f)))),
						ModStaticField("Sivelos.SivsContentPack", "SivsContentPack.Config.Configuration+Elites+Unstable", "fireballInterval", 5f),
						ModStaticField("Sivelos.SivsContentPack", "SivsContentPack.Config.Configuration+Elites+Unstable", "fireBallCount", 3),
						100f * ModStaticField("Sivelos.SivsContentPack", "SivsContentPack.Config.Configuration+Elites+Unstable", "fireBallDamageCoefficient", 1.5f)
					}
				}
			).Register();



			AspectPackDefOf.EnemiesReturns = new AspectPack
			{
				identifier = "EnemiesReturns",
				dependency = "com.Viliger.EnemiesReturns"
			}.With(
				new SimpleAspect()
				{
					identifier = "ZetAspectAeonian",
					stats = new[] { new AspectStat(AspectStatType.Armor, 30f, 15f) },
					affix = "AEONIAN",
					// EnemiesReturns prefixes its content names with ER_
					FindEquipmentDef = () => FindEquipmentDefByNames("ER_EliteAeonianEquipment", "EliteAeonianEquipment"),
					displayPriority = 19000,
					defence = true,
					icon = () => Sprites.AffixWhite,
					name = "Arraign's Decree",
					pickup = "Become an aspect of judgement.",
					aspectOf = "<style=cDeath>Aspect of Judgement</style> :",
					passive = "\nBecome <style=cIsUtility>immune</style> to <style=cIsDamage>attack speed</style> and <style=cIsUtility>movement speed</style> reductions.\nReduces the duration of <style=cIsUtility>stuns</style> and <style=cIsUtility>freezes</style> on you by <style=cIsUtility>{0}%</style>.",
					passiveArgs = () => new object[] {
						100f * Mathf.Clamp01(ModStaticField("com.Viliger.EnemiesReturns", "EnemiesReturns.Configuration.Judgement.Judgement", "AeonianEliteStunAndFreezeReduction", 0.5f))
					}
				}
			).Register();



			AspectPackDefOf.SwanSong = new AspectPack
			{
				identifier = "SwanSong",
				dependency = "com.RiskOfBrainrot.SwanSongExtended"
			}.With(
				new SimpleAspect()
				{
					identifier = "ZetAspectHallowed",
					stats = new[] { new AspectStat(AspectStatType.Cooldown, 0.15f, 0.10f) },
					affix = "SSHALLOWED",
					equipmentName = "BORBO_ELITE_EQUIPMENT_AFFIX_SPEED",
					displayPriority = 2530,
					fallbackIcon = () => Sprites.AffixBlue,
					name = "Chir's Tempo",
					pickup = "Become an aspect of velocity.",
					aspectOf = "<style=cDeath>Aspect of Velocity</style> :",
					passive = "\nIncreases <style=cIsUtility>movement speed</style> by <style=cIsUtility>{0}%</style> and <style=cIsDamage>base attack speed</style> by <style=cIsDamage>{1}</style>.\nReduces <style=cIsUtility>skill cooldowns</style> by <style=cIsUtility>{2}%</style>.",
					passiveArgs = () => new object[] {
						100f * ModStaticField("com.RiskOfBrainrot.SwanSongExtended", "SwanSongExtended.Elites.FrenziedAspect", "moveSpeedBonus", 1.2f),
						ModStaticField("com.RiskOfBrainrot.SwanSongExtended", "SwanSongExtended.Elites.FrenziedAspect", "atkSpeedBonus", 1.8f),
						100f * ModStaticField("com.RiskOfBrainrot.SwanSongExtended", "SwanSongExtended.Elites.FrenziedAspect", "cooldownReduction", 0.4f)
					}
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectSSVolatile",
					stats = new[] { new AspectStat(AspectStatType.Damage, 0.15f, 0.10f) },
					affix = "SSVOLATILE",
					equipmentName = "BORBO_ELITE_EQUIPMENT_AFFIX_EXPLOSIVE",
					displayPriority = 2531,
					fallbackIcon = () => Sprites.AffixRed,
					name = "Bava's Refrain",
					pickup = "Become an aspect of force.",
					aspectOf = "<style=cDeath>Aspect of Force</style> :",
					passive = "\nAttacks <style=cIsDamage>explode</style> on hit, <style=cIsUtility>knocking back</style> enemies.\nPeriodically fire a <style=cIsDamage>mortar</style>."
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectMirrored",
					stats = new[] { new AspectStat(AspectStatType.Crit, 10f, 5f) },
					affix = "SSMIRRORED",
					equipmentName = "BORBO_ELITE_EQUIPMENT_MIRRORED",
					displayPriority = 7800,
					offense = false,
					defence = true,
					fallbackIcon = () => Sprites.AffixHaunted,
					name = "Fractured Belief",
					pickup = "Become an aspect of vanity.",
					aspectOf = "<style=cDeath>Aspect of Vanity</style> :",
					passive = "\nBecome <style=cIsUtility>immune to hitstun</style>, but <style=cDeath>greatly reduces maximum health</style>.\nGain extra <style=cIsHealing>health gates</style>. Triggering one makes you briefly <style=cIsUtility>invulnerable</style> and <style=cIsDamage>shattered</style>, increasing <style=cIsUtility>speed</style> and reducing <style=cIsUtility>cooldowns</style>."
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectSurging",
					stats = new[] { new AspectStat(AspectStatType.MoveSpeed, 0.20f, 0.10f) },
					affix = "SSSURGING",
					equipmentName = "BORBO_ELITE_EQUIPMENT_AFFIX_FLOOD",
					displayPriority = 15500,
					fallbackIcon = () => Sprites.AffixBlue,
					name = "A Dormant Force",
					pickup = "Become an aspect of flood.",
					aspectOf = "<style=cDeath>Aspect of Flood</style> :",
					passive = "\nPeriodically <style=cIsUtility>surge</style> toward enemies, releasing <style=cIsDamage>waves</style> that drag them in a <style=cIsUtility>riptide</style>."
				},
				new SimpleAspect()
				{
					identifier = "ZetAspectHowling",
					stats = new[] { new AspectStat(AspectStatType.AttackSpeed, 0.20f, 0.10f) },
					affix = "SSHOWLING",
					equipmentName = "BORBO_ELITE_EQUIPMENT_AFFIX_SQUALL",
					displayPriority = 15501,
					fallbackIcon = () => Sprites.AffixHaunted,
					name = "Twisted Stare",
					pickup = "Become an aspect of squall.",
					aspectOf = "<style=cDeath>Aspect of Squall</style> :",
					passive = "\nAttacks <style=cIsDamage>strip armor</style> from enemies.\nPeriodically fire <style=cIsDamage>wind missiles</style> and a <style=cIsDamage>squall beam</style>."
				}
			).Register();



			AspectPackDefOf.Deltarune = new AspectPack
			{
				identifier = "Deltarune",
				dependency = "AGU.DeltaruneMod"
			}.With(
				new SimpleAspect()
				{
					identifier = "ZetAspectNEO",
					affix = "DRNEO",
					FindEquipmentDef = () => FindEquipmentDefByNames("AffixN.E.O.") ?? FindEquipmentDefByTokenOrBuff("Armor of N.E.O.", "N.E.O. Armor"),
					displayPriority = 2540,
					fallbackIcon = () => Sprites.AffixHaunted,
					name = "Armor of N.E.O.",
					pickup = "Become an aspect of N.E.O.",
					aspectOf = "<style=cDeath>Aspect of N.E.O.</style> :",
					passive = "\nIncreases <style=cIsDamage>damage</style>, but <style=cDeath>reduces health and armor</style>.\nGain <style=cIsUtility>{0} random buffs</style> while active.",
					passiveArgs = () => new object[] {
						ModStaticField("AGU.DeltaruneMod", "DeltaruneMod.Elites.NeoElite", "maxBuffs", 2)
					}
				}
			).Register();
		}
	}
}
