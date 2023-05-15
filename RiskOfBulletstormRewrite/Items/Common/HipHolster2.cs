using BepInEx.Configuration;
using HarmonyLib;
using Mono.Cecil.Cil;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Items.Common
{
    public class HipHolster : ItemBase<HipHolster>
    {
        public static ConfigEntry<float> cfgChanceHyperbolic;

        public override string ItemName => "Hip Holster";

        public override string ItemLangTokenName => "HIPHOLSTER";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgChanceHyperbolic).ToString()
        };

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Damage
        };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public static GameObject Load(string path)
        {
            return Assets.LoadAddressable<GameObject>(path);
        }
        public static SkillDef LoadSkill(string path)
        {
            return Assets.LoadAddressable<SkillDef>(path);
        }

        public static Dictionary<SkillDef, GameObject> skilldef_to_projectile = new Dictionary<SkillDef, GameObject>()
        {{ LoadSkill("RoR2/Base/Bandit2/Bandit2SerratedShivs.asset"), Load("RoR2/Base/Bandit2/Bandit2ShivProjectile.prefab") },
            { LoadSkill("RoR2/Base/Beetle/BeetleGuardBodySunder.asset"), Load("RoR2/Base/Beetle/Sunder.prefab") },
            { LoadSkill("RoR2/Base/Beetle/BeetleQueen2BodySpawnWards.asset"), Load("RoR2/Base/Beetle/BeetleWard.prefab") }, //?
            { LoadSkill("RoR2/Base/Beetle/BeetleQueen2BodySpit.asset"), Load("RoR2/Base/Beetle/BeetleQueenSpit.prefab") },
            { LoadSkill("RoR2/Base/Beetle/BeetleQueen2BodySummonEggs.asset"), Load("RoR2/Base/Beetle/BeetleQueenSpit.prefab") }, //
            { LoadSkill("RoR2/Base/Bell/BellBodyBellBlast.asset"), Load("RoR2/Base/Bell/BellBall.prefab") },
            { LoadSkill("RoR2/Base/Brother/FireLunarShards.asset"), Load("RoR2/Base/Brother/LunarShardProjectile.prefab") },
            { LoadSkill("RoR2/Base/Brother/FireLunarShardsHurt.asset"), Load("RoR2/Base/Brother/LunarShardProjectile.prefab") },
            { LoadSkill("RoR2/Base/Brother/FistSlam.asset"), Load("RoR2/Base/Brother/BrotherSunderWave.prefab") }, //?
            //{ LoadSkill("RoR2/Base/Brother/BrotherBody.prefab"), Load("RoR2/Base/Brother/BrotherSunderWave.prefab") },
            { LoadSkill("RoR2/Base/BrotherHaunt/FireRandomProjectiles.asset"), Load("RoR2/Base/BrotherHaunt/BrotherUltLineProjectileStatic.prefab") },
            { LoadSkill("RoR2/Base/Captain/CaptainTazer.asset"), Load("RoR2/Base/Captain/CaptainTazer.prefab") },
            { LoadSkill("RoR2/Base/Captain/CallAirstrike.asset"), Load("RoR2/Base/Captain/CaptainAirstrikeProjectile1.prefab") },
            { LoadSkill("RoR2/Base/Captain/CallAirstrikeAlt.asset"), Load("RoR2/Base/Captain/CaptainAirstrikeAltProjectile.prefab") },
            { LoadSkill("RoR2/Base/ClayBoss/ClayBossBodyCharge Bombardment.asset"), Load("RoR2/Base/ClayBoss/ClayPotProjectile.prefab") }, 
            { LoadSkill("RoR2/Base/ClayBoss/ClayBossBodyTarball.asset"), Load("RoR2/Base/ClayBoss/TarSeeker.prefab") }, //?
            { LoadSkill("RoR2/Base/Commando/CommandoBodyFireFMJ.asset"), Load("RoR2/Base/Commando/FMJRamping.prefab" )},
            { LoadSkill("RoR2/Base/Commando/ThrowGrenade.asset"), Load("RoR2/Base/Commando/CommandoGrenadeProjectile.prefab" )},
            { LoadSkill("RoR2/Base/Croco/CrocoDisease.asset"), Load("RoR2/Base/Croco/CrocoDiseaseProjectile.prefab") },
            { LoadSkill("RoR2/Base/Croco/CrocoSpit.asset"), Load("RoR2/Base/Croco/CrocoSpit.prefab") },
            { LoadSkill("RoR2/Base/Drones/MegaDroneBodyRocket.asset"), Load("RoR2/Base/Drones/PaladinRocket.prefab") },
            { LoadSkill("RoR2/Base/Drones/MissileDroneBodyGun.asset"), Load("RoR2/Base/Drones/MicroMissileProjectile.prefab") },
            //{ LoadSkill("RoR2/Base/ElectricWorm/ElectricWormBody.prefab"), Load("RoR2/Base/ElectricWorm/ElectricOrbProjectile.prefab") },
            //{ LoadSkill("RoR2/Base/Engi/EngiBodyPlaceBubbleShield.asset"), Load("RoR2/Base/Engi/EngiGrenadeProjectile.prefab") },
            { LoadSkill("RoR2/Base/Engi/EngiBodyFireGrenade.asset"), Load("RoR2/Base/Engi/EngiGrenadeProjectile.prefab") },
            { LoadSkill("RoR2/Base/Engi/EngiBodyPlaceMine.asset"), Load("RoR2/Base/Engi/EngiMine.prefab") }, 
            { LoadSkill("RoR2/Base/Engi/EngiBodyPlaceSpiderMine.asset"), Load("RoR2/Base/Engi/SpiderMine.prefab") }, 
            { LoadSkill("RoR2/Base/Engi/EngiHarpoons.asset"), Load("RoR2/Base/Engi/EngiGrenadeProjectile.prefab") },
            { LoadSkill("RoR2/Base/Grandparent/GrandParentGroundSwipe.asset"), Load("RoR2/Base/Grandparent/GrandparentBoulder.prefab") },
            { LoadSkill("RoR2/Base/Gravekeeper/GravekeeperBodyBarrage.asset"), Load("RoR2/Base/Gravekeeper/GravekeeperTrackingFireball.prefab") },
            { LoadSkill("RoR2/Base/Gravekeeper/GravekeeperBodyPrepHook.asset"), Load("RoR2/Base/Gravekeeper/GravekeeperHookProjectileSimple.prefab") },
            { LoadSkill("RoR2/Base/GreaterWisp/GreaterWispBodyCannons.asset"), Load("RoR2/Base/GreaterWisp/WispCannon.prefab") },
            //{ LoadSkill("RoR2/Base/Heretic/HereticBody.prefab"), Load("") },
            { LoadSkill("RoR2/Base/HermitCrab/HermitCrabBodyBurrowMortar.asset"), Load("RoR2/Base/HermitCrab/HermitCrabBombProjectile.prefab") },
            //{ LoadSkill("RoR2/Base/Huntress/HuntressBody.prefab"), Load("") },
            //{ LoadSkill("RoR2/Base/Imp/ImpBody.prefab"), Load("") },
            { LoadSkill("RoR2/Base/ImpBoss/ImpBossBodyFireVoidspikes.asset"), Load("RoR2/Base/ImpBoss/ImpVoidspikeProjectile.prefab") },
            { LoadSkill("RoR2/Base/Lemurian/LemurianBodyFireball.asset"), Load("RoR2/Base/Lemurian/Fireball.prefab") },
            { LoadSkill("RoR2/Base/LemurianBruiser/LemurianBruiserBodyPrimary.asset"), Load("RoR2/Base/LemurianBruiser/LemurianBigFireball.prefab") },
            //{ LoadSkill("RoR2/Base/Loader/ChargeZapFist.asset"), Load("RoR2/Base/Loader/LoaderBody.prefab") },
            { LoadSkill("RoR2/Base/LunarExploder/FireExploderShards.asset"), Load("RoR2/Base/LunarExploder/LunarExploderShardProjectile.prefab") },
            { LoadSkill("RoR2/Base/LunarGolem/LunarGolemBodyTwinShot.asset"), Load("RoR2/Base/LunarGolem/LunarGolemTwinShotProjectile.prefab") },
            { LoadSkill("RoR2/Base/LunarWisp/LunarWispBodySeekingBomb.asset"), Load("RoR2/Base/LunarWisp/LunarWispTrackingBomb.prefab") },
            { LoadSkill("RoR2/Base/Mage/MageBodyFireFirebolt.asset"), Load("RoR2/Base/Mage/MageFireboltBasic.prefab") },
            { LoadSkill("RoR2/Base/Mage/MageBodyFireLightningBolt.asset"), Load("RoR2/Base/Mage/MageLightningboltBasic.prefab") },
            { LoadSkill("RoR2/Base/Mage/MageBodyIceBomb.asset"), Load("RoR2/Base/Mage/MageIceBombProjectile.prefab") },
            { LoadSkill("RoR2/Base/Mage/MageBodyNovaBomb.asset"), Load("RoR2/Base/Mage/MageLightningBombProjectile.prefab") },
            { LoadSkill("RoR2/Base/Mage/MageBodyWall.asset"), Load("RoR2/Base/Mage/MageIcewallPillarProjectile.prefab") }, //whats walkerprojectile?
            //{ LoadSkill("RoR2/Base/MagmaWorm/MagmaWormBody.prefab"), Load("RoR2/Base/MagmaWorm/MagmaOrbProjectile.prefab") },
            { LoadSkill("RoR2/Base/Merc/MercBodyEvisProjectile.asset"), Load("RoR2/Base/Merc/EvisProjectile.prefab") },
            { LoadSkill("RoR2/Base/Nullifier/FireNullifier.asset"), Load("RoR2/Base/MiniMushroom/SporeGrenadeProjectile.prefab") },
            { LoadSkill("RoR2/Base/MiniMushroom/MiniMushroomSporeGrenade.asset"), Load("RoR2/Base/Nullifier/NullifierBombProjectile.prefab") },
            { LoadSkill("RoR2/Base/RoboBallBoss/EyeBlast.asset"), Load("RoR2/Base/RoboBallBoss/RoboBallProjectile.prefab") },
            { LoadSkill("RoR2/Base/RoboBallBoss/SuperEyeblast.asset"), Load("RoR2/Base/RoboBallBoss/SuperRoboBallProjectile.prefab") }, 
            { LoadSkill("RoR2/Base/Scav/PrepEnergyCannon.asset"), Load("RoR2/Base/Scav/ScavEnergyCannonProjectile.prefab") }, 
            { LoadSkill("RoR2/Base/Scav/PrepSack.asset"), Load("RoR2/Base/Scav/ScavSackProjectile.prefab") },  //left
            { LoadSkill("RoR2/Base/Titan/TitanBodyRechargeRocks.asset"), Load("RoR2/Base/Titan/TitanRockProjectile.prefab") },
            { LoadSkill("RoR2/Base/Toolbot/ToolbotBodyFireGrenadeLauncher.asset"), Load("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab") },
            { LoadSkill("RoR2/Base/Toolbot/ToolbotBodyStunDrone.asset"), Load("RoR2/Base/Toolbot/CryoCanisterBombletsProjectile.prefab") },
            { LoadSkill("RoR2/Base/Treebot/TreebotBodyFireSyringe.asset"), Load("RoR2/Base/Treebot/SyringeProjectileHealing.prefab") },
            { LoadSkill("RoR2/Base/Treebot/TreebotBodyFireFlower2.asset"), Load("RoR2/Base/Treebot/TreebotFlowerSeed.prefab") },
            { LoadSkill("RoR2/Base/Treebot/TreebotBodyFireFruitSeed.asset"), Load("RoR2/Base/Treebot/TreebotFruitSeedProjectile.prefab") },
            { LoadSkill("RoR2/Base/Vagrant/VagrantBodyJellyBarrage.asset"), Load("RoR2/Base/Vagrant/VagrantCannon.prefab") },
            { LoadSkill("RoR2/Base/Vagrant/VagrantBodyTrackingBomb.asset"), Load("RoR2/Base/Vagrant/VagrantTrackingBomb.prefab") },
            { LoadSkill("RoR2/Base/Vulture/ChargeWindblade.asset"), Load("RoR2/Base/Vulture/WindbladeProjectile.prefab") },
            //{ LoadSkill("RoR2/DLC1/Assassin2/Assassin2Body.prefab"), Load("RoR2/DLC1/Assassin2/AssassinShurikenProjectile.prefab") },
            { LoadSkill("RoR2/DLC1/ClayGrenadier/ThrowBarrel.asset"), Load("RoR2/DLC1/ClayGrenadier/ClayGrenadierBarrelProjectile.prefab") },
            //{ LoadSkill("RoR2/DLC1/DroneCommander/DroneCommanderBody.prefab"), Load("") },
            { LoadSkill("RoR2/DLC1/FlyingVermin/Spit.asset"), Load("RoR2/DLC1/FlyingVermin/VerminSpitProjectile.prefab") },
            //{ LoadSkill("RoR2/DLC1/Railgunner/RailgunnerBodyFirePistol.asset"), Load("RoR2/DLC1/Railgunner/RailgunnerPistolProjectile.prefab") }, doesnt reload
            { LoadSkill("RoR2/DLC1/Railgunner/RailgunnerBodyFireMineConcussive.asset"), Load("RoR2/DLC1/Railgunner/RailgunnerMine.prefab") },
            { LoadSkill("RoR2/DLC1/Railgunner/RailgunnerBodyFireMineBlinding.asset"), Load("RoR2/DLC1/Railgunner/RailgunnerMineAlt.prefab") },
            { LoadSkill("RoR2/DLC1/VoidBarnacle/VoidBarnacleFire.asset"), Load("RoR2/DLC1/VoidBarnacle/VoidBarnacleBullet.prefab") },
            { LoadSkill("RoR2/DLC1/VoidJailer/VoidJailerChargeFire.asset"), Load("RoR2/DLC1/VoidJailer/VoidJailerDart.prefab") },
            { LoadSkill("RoR2/DLC1/VoidMegaCrab/FireCrabBlackCannon.asset"), Load("RoR2/DLC1/VoidMegaCrab/MegaCrabBlackCannonProjectile.prefab") },
            { LoadSkill("RoR2/DLC1/VoidMegaCrab/FireCrabWhiteCannon.asset"), Load("RoR2/DLC1/VoidMegaCrab/MegaCrabBlackCannonProjectile.prefab") },
            { LoadSkill("RoR2/DLC1/VoidMegaCrab/FireVoidMissiles.asset"), Load("RoR2/DLC1/VoidMegaCrab/MissileVoidBigProjectile.prefab") },
            //{ LoadSkill("RoR2/DLC1/VoidSurvivor/ChargeMegaBlaster.asset"), Load("") },
            //{ LoadSkill("RoR2/DLC1/VoidSurvivor/FireCorruptDisk.asset"), Load("RoR2/DLC1/VoidSurvivor/VoidSurvivorTwinBlasterProjectile1.prefab") },
        };

        public override void CreateConfig(ConfigFile config)
        {
            cfgChanceHyperbolic = config.Bind(ConfigCategory, Assets.cfgChanceIntegerKey, 0.15f, Assets.cfgChanceIntegerDesc);
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = ItemModel;
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.05f, 0.05f, 0.05f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0, 0, 0)
                }
            });
            rules.Add("mdlCommandoDualies", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
      childName = "Stomach",
localPos = new Vector3(0.0883F, 0.04358F, 0.05855F),
localAngles = new Vector3(343.6707F, 83.0621F, 337.4149F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.06311F, -0.09981F, 0.00124F),
localAngles = new Vector3(27.567F, 69.44801F, 163.8226F),
localScale = new Vector3(0.0733F, 0.0733F, 0.0733F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Hip",
localPos = new Vector3(0.14827F, 0.62807F, 0.74695F),
localAngles = new Vector3(16.64241F, 352.0331F, 119.2978F),
localScale = new Vector3(0.68137F, 0.71426F, 0.68137F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.13122F, -0.0001F, -0.01835F),
localAngles = new Vector3(19.27725F, 85.19704F, 145.0748F),
localScale = new Vector3(0.12243F, 0.11417F, 0.1148F)
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Base",
localPos = new Vector3(0.11878F, 1.3892F, 0.05592F),
localAngles = new Vector3(339.9832F, 91.42525F, 319.8334F),
localScale = new Vector3(0.13749F, 0.13749F, 0.13749F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.10036F, -0.10851F, 0.00613F),
localAngles = new Vector3(20.62196F, 92.59741F, 170.2556F),
localScale = new Vector3(0.08868F, 0.08868F, 0.08868F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.12369F, 0.03177F, -0.04705F),
localAngles = new Vector3(18.30224F, 89.34419F, 138.0586F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HeadBase",
localPos = new Vector3(0.19012F, 0.06155F, -0.49279F),
localAngles = new Vector3(30.72254F, 121.4533F, 174.6666F),
localScale = new Vector3(0.1341F, 0.1341F, 0.1341F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.08664F, -0.00845F, 0.05259F),
localAngles = new Vector3(15.08299F, 90.70071F, 159.6231F),
localScale = new Vector3(0.13278F, 0.13278F, 0.13278F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
      childName = "Hip",
localPos = new Vector3(0.98113F, 1.0147F, -0.95194F),
localAngles = new Vector3(8.3527F, 84.3969F, 124.4845F),
localScale = new Vector3(0.83998F, 0.83998F, 0.83998F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.07887F, -0.11967F, -0.09726F),
localAngles = new Vector3(22.26467F, 97.54887F, 152.8677F),
localScale = new Vector3(0.11337F, 0.11337F, 0.11337F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.13972F, -0.08546F, -0.00947F),
localAngles = new Vector3(23.02333F, 82.0136F, 150.631F),
localScale = new Vector3(0.12569F, 0.12569F, 0.12569F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
    childName = "Pelvis",
localPos = new Vector3(0.06269F, -0.03343F, -0.05874F),
localAngles = new Vector3(13.14823F, 123.7456F, 153.1889F),
localScale = new Vector3(0.11685F, 0.11685F, 0.11685F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
             childName = "Pelvis",
localPos = new Vector3(0.06196F, -0.01626F, 0.00386F),
localAngles = new Vector3(14.67509F, 86.43238F, 154.4437F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlVoidSurvivor", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Pelvis",
                localPos = new Vector3(0.01192F, 0.04854F, 0.08877F),
                localAngles = new Vector3(22.60713F, 349.1809F, 138.0614F),
                localScale = new Vector3(0.08201F, 0.08201F, 0.08201F)
            });
            rules.Add("mdlRailGunner", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Pelvis",
                localPos = new Vector3(0.06763F, 0.04185F, 0.01322F),
                localAngles = new Vector3(20.46132F, 81.72948F, 140.0435F),
                localScale = new Vector3(0.07687F, 0.07687F, 0.07687F)
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.1f, 0.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 2f
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 2.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 10f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0F, 4.8685F, 0.0438F),
localAngles = new Vector3(288.4044F, 180F, 180F),
localScale = new Vector3(1F, 1F, 1F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.0013F, 0.1559F, -0.2403F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.1594F, 3.6456F, 0.0645F),
                localAngles = new Vector3(279.4401F, 195.4454F, 161.8801F),
                localScale = new Vector3(0.4099F, 0.4099F, 0.4099F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLB",
                localPos = new Vector3(-1.6752F, -0.2F, -0.468F),
                localAngles = new Vector3(2.6768F, 179.4175F, 179.4478F),
                localScale = new Vector3(0.1793F, 0.1793F, 0.1793F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(0.0002F, -0.189F, 1.9457F),
                localAngles = new Vector3(24.2706F, 0.0024F, 0.024F),
                localScale = new Vector3(0.2908F, 0.2908F, 0.2908F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Mask",
                localPos = new Vector3(0F, 0.0344F, -1.6055F),
                localAngles = new Vector3(88.6293F, 0F, 0F),
                localScale = new Vector3(0.425F, 0.425F, 0.425F)
            });
            return rules;
        }

        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody obj)
        {
            if (obj.hasEffectiveAuthority)
            {
                var comp = obj.gameObject.GetComponent<RBSHipHolsterController>();
                bool hasItem = GetCount(obj) > 0;
                if (comp == null && hasItem)
                {
                    if (obj.skillLocator)
                    {
                        comp = obj.gameObject.AddComponent<RBSHipHolsterController>();
                        comp.skillLocator = obj.skillLocator;
                        comp.inventory = obj.inventory;
                        comp.characterBody = obj;
                    }
                }
                else if (comp != null && !hasItem)
                {
                    Object.Destroy(comp);
                }
            }
        }

        public class RBSHipHolsterController : MonoBehaviour
        {
            public CharacterBody characterBody;
            public Inventory inventory;
            public SkillLocator skillLocator;
            public GenericSkill[] genericSkills;
            public int[] lastStocks;
            public GameObject[] associatedProjectiles;

            public float reductionPercentage;

            public void FireProjectile(GameObject projectilePrefab)
            {
                if (!projectilePrefab) return;
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    crit = characterBody.RollCrit(),
                    damage = characterBody.damage * 1f,
                    damageColorIndex = DamageColorIndex.Default,
                    damageTypeOverride = DamageType.Generic,
                    owner = characterBody.gameObject,
                    position = characterBody.aimOrigin,
                    rotation = characterBody.inputBank ? Util.QuaternionSafeLookRotation(characterBody.inputBank.GetAimRay().direction) : Quaternion.identity,
                    procChainMask = default,
                    projectilePrefab = projectilePrefab,
                    target = characterBody.master && characterBody.master.TryGetComponent(out BaseAI bot) ? bot.currentEnemy?.gameObject : null,
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);

            }

            public void Start()
            {
                //allSkills does GetComponents<GenericSkill>(), which is private but we have publicized
                //so redoing it has no point
                List<GenericSkill> genericSkillsList = new List<GenericSkill>();
                List<int> lastStockList = new List<int>();
                List<GameObject> projectileList = new List<GameObject>();

                foreach (var skill in skillLocator.allSkills)
                {
                    if (!skill)
                        continue;
                    genericSkillsList.Add(skill);
                    lastStockList.Add(skill.stock);
                    projectileList.Add(EvaluateSkillForProjectile(skill.skillDef));
                }
                genericSkills = genericSkillsList.ToArray();
                lastStocks = lastStockList.ToArray();
                associatedProjectiles = projectileList.ToArray();
                if (inventory)
                    inventory.onInventoryChanged += Inventory_onInventoryChanged;
            }

            private void Inventory_onInventoryChanged()
            {
                var itemCount = instance.GetCount(characterBody);
                reductionPercentage = Util.ConvertAmplificationPercentageIntoReductionPercentage(cfgChanceHyperbolic.Value * itemCount);
            }

            public void OnDestroy()
            {
                if (inventory)
                    inventory.onInventoryChanged -= Inventory_onInventoryChanged;
            }

            public GameObject EvaluateSkillForProjectile(SkillDef skillDef)
            {
                skilldef_to_projectile.TryGetValue(skillDef, out GameObject projectile);
                return projectile;
            }

            public void FixedUpdate()
            {
                int i = 0;
                while (i < genericSkills.Length)
                {
                    var gs = genericSkills[i];
                    if (gs)
                    {
                        if (lastStocks[i] < gs.stock)
                        {
                            if (Util.CheckRoll(reductionPercentage, characterBody.master))
                            {
                                FireProjectile(associatedProjectiles[i]);
                            }
                        }
                        lastStocks[i] = gs.stock;
                    }
                    i++;
                }
            }
        }
    }
}