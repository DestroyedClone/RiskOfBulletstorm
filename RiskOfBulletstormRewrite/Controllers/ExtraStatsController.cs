using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using System;
using BepInEx;
using BepInEx.Configuration;
using static RiskOfBulletstormRewrite.Main;
using System.Net;
using static RiskOfBulletstormRewrite.Utils.ItemHelpers;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class ExtraStatsController : ControllerBase<ExtraStatsController>
    {
        public static bool ShotSpread_EnableDML { get; private set; } = true;
        public static bool ShotSpread_EnableLoader { get; private set; } = false;
        public static bool ShotSpread_WhitelistProjectiles { get; private set; } = true;
        public static bool AutoDownloadUpdates { get; private set; } = true;

        public static List<GameObject> WhitelistedProjectiles = new List<GameObject>
        {
            // Equipment/Items
            Load<GameObject>("RoR2/Base/Saw/Sawmerang.prefab"), //Sawmerang
            Load<GameObject>("RoR2/Base/LunarSkillReplacements/LunarNeedleProjectile.prefab"), // Visions of Heresy
            Load<GameObject>("RoR2/Base/LunarSkillReplacements/LunarSecondaryProjectile.prefab"), // Hooks of Heresy
            Load<GameObject>("RoR2/Base/EliteLunar/LunarMissileProjectile.prefab"), // Perfected
            Load<GameObject>("RoR2/DLC1/PrimarySkillShuriken/ShurikenProjectile.prefab"),
            Load<GameObject>("RoR2/DLC1/LunarSun/LunarSunProjectile.prefab"),
            Load<GameObject>("RoR2/DLC1/Molotov/MolotovClusterProjectile.prefab"),
            Load<GameObject>("RoR2/DLC1/Molotov/MolotovSingleProjectile.prefab"),

            // Survivors
                // Acrid
            Load<GameObject>("RoR2/Base/Croco/CrocoSpit.prefab"), // Ranged Secondary
            Load<GameObject>("RoR2/Base/Croco/CrocoDiseaseProjectile.prefab"), // Special
                // Artificer
            Load<GameObject>("RoR2/Base/Mage/MageFireboltBasic.prefab"),
            Load<GameObject>("RoR2/Base/Mage/MageLightningboltBasic.prefab"),
            Load<GameObject>("RoR2/Base/Mage/MageLightningBombProjectile.prefabe"),
            Load<GameObject>("RoR2/Base/Mage/MageIceBombProjectile.prefab"),
            Load<GameObject>("RoR2/Base/Mage/MageIcewallWalkerProjectile.prefab"),
            //MageIcewallPillarProjectile ??
                
                // Bandit2
            Load<GameObject>("RoR2/Base/Bandit2/Bandit2ShivProjectile.prefab"),


                // Captain
            Load<GameObject>("RoR2/Base/Captain/CaptainTazer.prefab"),

                // Commando
            Load<GameObject>("RoR2/Base/Commando/CommandoGrenadeProjectile.prefab"),
            //Load<GameObject>("Prefabs/Projectiles/FMJ"), //??
            Load<GameObject>("RoR2/Base/Commando/FMJRamping.prefab"),

                // Engineer
            Load<GameObject>("RoR2/Base/Engi/EngiHarpoon.prefab"),
            Load<GameObject>("RoR2/Base/Engi/EngiGrenadeProjectile.prefab"),
            Load<GameObject>("RoR2/Base/Engi/EngiMine.prefab"),
            Load<GameObject>("RoR2/Base/Engi/SpiderMine.prefab"),
            Load<GameObject>("RoR2/Base/Engi/EngiBubbleShield.prefab"),

                // Huntress
            //EntityStates.Huntress.HuntressWeapon.FireGlaive.projectilePrefab, //Glaive's an orb, so this isnt used

                // Loader
            Load<GameObject>("RoR2/Base/Loader/LoaderZapCone.prefab"), //It's a projectile, go figure.
            Load<GameObject>("RoR2/Base/Loader/LoaderPylon.prefab"),

                // Merc
            Load<GameObject>("RoR2/Base/Merc/EvisProjectile.prefab"), // Ranged special

                // MUL-T
            Load<GameObject>("RoR2/Base/Toolbot/ToolbotGrenadeLauncherProjectile.prefab"), // Scrap Launcher
            Load<GameObject>("RoR2/Base/Toolbot/CryoCanisterProjectile.prefab"), // Secondary bomb

                // Railgunner
            Load<GameObject>("RoR2/DLC1/Railgunner/RailgunnerPistolProjectile.prefab"),
            Load<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMine.prefab"),
            Load<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineAlt.prefab"),

                // REX
            Load<GameObject>("RoR2/Base/Treebot/SyringeProjectile.prefab"),
            Load<GameObject>("RoR2/Base/Treebot/SyringeProjectileHealing.prefab"), // Third syringe shot
            Load<GameObject>("RoR2/Base/Treebot/TreebotMortarRain.prefab"),
            Load<GameObject>("RoR2/Base/Treebot/TreebotMortar2.prefab"),
            Load<GameObject>("RoR2/Base/Treebot/TreebotFlowerSeed.prefab"),
            Load<GameObject>("RoR2/Base/Treebot/TreebotFruitSeedProjectile.prefab"),

                // Void Fiend
            Load<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterSmallProjectile.prefab"),
            Load<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterBigProjectile.prefab"),
            Load<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterBigProjectileCorrupted.prefab"),

            // Enemies
                // Beetle Queen
            Load<GameObject>("RoR2/Base/Beetle/BeetleQueenSpit.prefab"), // Beetle Spit
                // Mithrix
            Load<GameObject>("RoR2/Base/Brother/LunarShardProjectile.prefab"),
                // Clay Dunestrider
            Load<GameObject>("RoR2/Junk/ClayBoss/TarBall.prefab"), //junk?
            Load<GameObject>("RoR2/Base/ClayBoss/ClayPotProjectile.prefab"),
            //EntityStates.ClayBoss.ClayBossWeapon.FireBombardment.projectilePrefab, //
                // Grovetender
            //Load<GameObject>("prefabs/projectiles/GravekeeperHookProjectile"), //not used? used for HAND
            Load<GameObject>("RoR2/Base/Gravekeeper/GravekeeperHookProjectileSimple.prefab"), //????
            Load<GameObject>("RoR2/Base/Gravekeeper/GravekeeperTrackingFireball.prefab"),
                // Imp + Imp Overlord
            Load<GameObject>("RoR2/Base/ImpBoss/ImpVoidspikeProjectile.prefab"),
            EntityStates.ImpMonster.FireSpines.projectilePrefab, 
                // Elder Lemurian
            Load<GameObject>("RoR2/Base/LemurianBruiser/LemurianBigFireball.prefab"), 
                // Void Reaver
            Load<GameObject>("RoR2/Base/Nullifier/NullifierBombProjectile.prefab"),
                // Alloy Worship Unit / Solus Control Unit
            Load<GameObject>("RoR2/Base/RoboBallBoss/RoboBallProjectile.prefab"),
            Load<GameObject>("RoR2/Base/RoboBallBoss/SuperRoboBallProjectile.prefab"), //idk??
                // Scavenger
            Load<GameObject>("RoR2/Base/Scav/ScavEnergyCannonProjectile.prefab"),
            Load<GameObject>("RoR2/Base/Scav/ScavSackProjectile.prefab"), //thqibbs
                // Malachite Urchin
            Load<GameObject>("RoR2/Base/ElitePoison/UrchinSeekingProjectile.prefab"),
                // Wandering Vagrant
            EntityStates.VagrantMonster.Weapon.JellyBarrage.projectilePrefab,
                // Alloy Vulture
            Load<GameObject>("RoR2/Base/Vulture/WindbladeProjectile.prefab"), 
                // Beetle Guard
            Load<GameObject>("RoR2/Base/Beetle/Sunder.prefab"), 
                // Brass Contraption
            Load<GameObject>("RoR2/Base/Bell/BellBall.prefab"), 
                // Lemurian
            Load<GameObject>("RoR2/Base/Lemurian/Fireball.prefab"), //fireball
                // Hermit Crab
            //Load<GameObject>("RoR2/Base/HermitCrab/HermitCrabBombProjectile.prefab"), 
                // Lunar Golem
            Load<GameObject>("RoR2/Base/LunarGolem/LunarGolemTwinShotProjectile.prefab"), //check stick 
                // Lunar Wisp
            Load<GameObject>("RoR2/Base/LunarWisp/LunarWispTrackingBomb.prefab"), 
                // Lunar Exploder
            Load<GameObject>("RoR2/Base/LunarExploder/LunarExploderShardProjectile.prefab"),
                // Mini Mushrum
            Load<GameObject>("RoR2/Base/MiniMushroom/SporeGrenadeProjectile.prefab"), 
                // Void Reaver
            Load<GameObject>("RoR2/Base/Nullifier/NullifierPreBombProjectile.prefab"), 
                // Greater Wisp
            Load<GameObject>("RoR2/Base/GreaterWisp/WispCannon.prefab"),
                // Grandparent
            Load<GameObject>("RoR2/Base/Grandparent/GrandparentBoulder.prefab"),
            Load<GameObject>("RoR2/Base/Grandparent/GrandparentGravSphere.prefab"),

            Load<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/MinorConstructProjectile.prefab"),

            Load<GameObject>("RoR2/DLC1/FlyingVermin/VerminSpitProjectile.prefab"),

            Load<GameObject>("RoR2/DLC1/ClayGrenadier/ClayGrenadierMortarProjectile.prefab"),
            Load<GameObject>("RoR2/DLC1/ClayGrenadier/ClayGrenadierBarrelProjectile.prefab"),

            Load<GameObject>("RoR2/DLC1/VoidBarnacle/VoidBarnacleBullet.prefab"),

            Load<GameObject>("RoR2/DLC1/VoidJailer/VoidJailerDart.prefab"),

            Load<GameObject>("RoR2/DLC1/VoidMegaCrab/MegaCrabWhiteCannonProjectile.prefab"),
            Load<GameObject>("RoR2/DLC1/VoidMegaCrab/MegaCrabBlackCannonProjectile.prefab"),
            Load<GameObject>("RoR2/DLC1/VoidMegaCrab/MissileVoidBigProjectile.prefab"),
            Load<GameObject>("RoR2/DLC1/VoidRaidCrab/VoidRaidCrabMissileProjectile.prefab"),
        };

        public static List<string> ModdedWhitelistedProjectiles = new List<string>();

        private const string CategoryNameShotSpread = "ExtraStatsShotSpread";

        private const string projectileAddress = "https://raw.githubusercontent.com/DestroyedClone/RiskOfBulletstorm/master/RiskOfBulletstormRewrite/Controllers/ModdedProjectileNames.txt";

        //private static readonly GameObject DisposableMissileLauncherPrefab = Load<GameObject>("Prefabs/Projectiles/MissileProjectile");
        //private static readonly GameObject LoaderHookPrefab = Load<GameObject>("prefabs/projectiles/LoaderHook");
        //private static readonly GameObject LoaderYankHookPrefab = Load<GameObject>("prefabs/projectiles/LoaderYankHook");
        public static float SimpleSpread(float accuracy, float originalValue, float multiplier = 1f)
        {
            return originalValue == 0 ? accuracy * multiplier : originalValue * accuracy;
        }

        public override void Init(ConfigFile config)
        {
            SetupConfig(config);
            Hooks();
            SetupModdedProjectiles();
        }

        public void SetupModdedProjectiles()
        {
            string[] moddedProjectileStrings =
            {
                // LUAFUBUKI - GAUSS //
                "Gauss0a", //"prefabs/Gauss/Gauss0a"

                // LUAFUBUKI - LUNAR CHIMERA //
                "Prefabs/Custom/LunarWispBall",
                "Prefabs/Custom/FirePillar",
                "Assets/Custom/SoulRocketProjectile",
                "Assets/Custom/LunarShardProjectile",
                //"Prefabs/Custom/MoonWave"

                // LUAFUBUKI - VOID REAVER //
                "Prefabs/Custom/ShortPortalBomb",

                // KOMRADESPECTRE - AETHERIUM //
                "JarOfReshapingProjectile",
                "SwordProjectile",

                // ZERODOMAI - TRISTANA //
                "Cannonball",
                "Explosiveball",
                "Blastball",

                // DUCKDUCKGREYDUCK - ARTIFICEREXTENDED //
                "ThunderProjectile", //Rolling Thunder
                "NapalmSpit", //Napalm projectile
                //Napalm decal thing: NapalmFire
                "ShockwaveZapCone",
                "NanometeorImpact", //Meteor
                "mageFireballInner", //Flame Burst 1
                "mageFireballOuter", //Flame Burst 2

                // HELTER2 - ARTIFICER2 //
                "mageIceBombClone",
                "mageLightningBombClone",

                // ROB - DIRESEEKER //
                "DireseekerBossFireball",
                "DireseekerBossGroundFireball",

                // ROB - MINER's DIRESEEKER //
                "DireseekerFireball",
                "DireseekerGroundFireball",

                // ROB - PLAYABLETEMPLAR //
                "TemplarGrenadeProjectile",
                "TemplarRocketProjectile",

                // ROB - PALADIN //
                "PaladinLunarShard",
                "LightningSpear",
                "PaladinSwordBeam",

                // ROB - TWITCH //
                "Prefabs/Projectiles/CrossbowBoltProjectile",
                "Prefabs/Projectiles/ExpungeProjectile",
                "Prefabs/Projectiles/VenomCaskProjectile",
                "Prefabs/Projectiles/TwitchBazookaProjectile",

                // RYANPALLESAN - EXPANDEDSKILLS //
                "magefireboltcopy",

                // THEMYSTICSWORD - ASPECTABILITIES //
                "AspectAbilitiesFireMissile",

                // ENIGMA - CLOUDBURST //
                "MIRVEquipmentProjectile",
                "MIRVClusterEquipmentProjectile",
                "OverchargedProjectile",
                "BombardierRocketProjectile",
                "BombardierFireRocketProjectile",
                "BombardierSeekerRocketProjectile",

                // THINKINVIS - CLASSICITEMS //
                "CIScepCommandoGrenade",
                "CIScepLoaderThundercrash",

                // CHEN - CLASSICITEMS //
                //"InstantMine", drops at feet
                //"PanicMine",
                //"FootMine",
                "MortarProjectile",

                // STARSTORM //
                "NemmandoSwordBeam",
                "ChirrDart",
                "CyborgbfgProjectile",
                "WayfarerChainProjectile",
                "NucleatorProjectile"
            };

            _logger.LogMessage("[Risk of Bulletstorm] Projectile Whitelist: Adding modded projectiles.");
            int moddedProjectilesAdded = 0;
            foreach (string projectileString in moddedProjectileStrings)
            {
                var projectileIndex = ProjectileCatalog.FindProjectileIndex(projectileString);
                //failures to find defaults to -1
                if (projectileIndex > 0)
                {
                    WhitelistedProjectiles.Add(ProjectileCatalog.GetProjectilePrefab(projectileIndex));
                    _logger.LogMessage("[Risk of Bulletstorm] Projectile Whitelist: Added projectile = " + projectileString);
                    moddedProjectilesAdded++;
                }
            }
            _logger.LogMessage("[Risk of Bulletstorm] Projectile Whitelist: Added " + moddedProjectilesAdded + " modded projectiles.");
        }



        public static void SetupConfig(ConfigFile config)
        {
            ShotSpread_EnableDML = config.Bind(CategoryNameShotSpread, "Disposable Missile Launcher", true, "If enabled, Shot Spread will affect the Disposable Missile Launcher. " +
                "\nThis tends to make it fire forwards rather than vertically, but shouldn't affect its homing.").Value;
            ShotSpread_EnableLoader = config.Bind(CategoryNameShotSpread, "Loader Hooks", false, "If enabled, Shot Spread will affect Loader's Hooks.").Value;
            ShotSpread_WhitelistProjectiles = config.Bind(CategoryNameShotSpread, "Whitelisted Projectiles", true, "If enabled, Shot Spread will only tighten the spread of SPECIFIC projectiles." +
                "\nIt is HIGHLY recommended not to disable, because alot of projectiles could break otherwise.").Value;
            AutoDownloadUpdates = config.Bind(CategoryNameShotSpread, "Autodownload updates",
                true, "If enabled, then the mod will autodownload the latest modded projectile names on startup. These are added manually.").Value;

            if (ShotSpread_EnableDML)
                WhitelistedProjectiles.Add(Load<GameObject>("RoR2/Base/Common/MissileProjectile.prefab"));
            if (ShotSpread_EnableLoader)
            {
                WhitelistedProjectiles.Add(Load<GameObject>("RoR2/Base/Loader/LoaderHook.prefab"));
                WhitelistedProjectiles.Add(Load<GameObject>("RoR2/Base/Loader/LoaderYankHook.prefab"));
            }
            if (AutoDownloadUpdates)
            {
                //DownloadNewProjectileNames();
            }
        }

        public static void DownloadNewProjectileNames()
        {
            WebClient webClient = new WebClient();
            webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;
            //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
            webClient.DownloadFileAsync(new Uri(projectileAddress), RiskOfBulletstormRewrite.Main.LocationOfProgram);
        }

        private static void WebClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            _logger.LogMessage("Finished downloading latest projectiles list.");
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //progressBar.Value = e.ProgressPercentage;
        }

        public override void Hooks()
        {
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;

            // Accuracy //
            On.RoR2.BulletAttack.Fire += AdjustSpreadBullets;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileManager_FireProjectile_FireProjectileInfo;
            On.EntityStates.GenericBulletBaseState.FireBullet += GenericBulletBaseState_FireBullet;
        }
        private static void CharacterMaster_onStartGlobal(CharacterMaster obj)
        {
            if (obj && obj.inventory && !obj.gameObject.GetComponent<RBSExtraStatsController>())
            {
                var comp = obj.gameObject.AddComponent<RBSExtraStatsController>();
                comp.inventory = obj.inventory;
            }
        }

        private static void AdjustSpreadBullets(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            if (self.owner)
            {
                var body = self.owner.gameObject.GetComponent<CharacterBody>();
                if (body)
                {
                    var comp = body.masterObject.GetComponent<RBSExtraStatsController>();
                    if (comp)
                    {
                        var accuracy = comp.bulletAccuracy;

                        body.SetSpreadBloom(Mathf.Min(0, body.spreadBloomAngle * accuracy), false);

                        self.maxSpread = SimpleSpread(self.maxSpread, 1.15f);

                        self.minSpread = SimpleSpread(accuracy, self.minSpread);

                        if (body.inputBank) self.aimVector = Vector3.Lerp(self.aimVector, body.inputBank.aimDirection, 1 - accuracy);

                        self.spreadPitchScale *= SimpleSpread(accuracy, self.spreadPitchScale);
                        self.spreadYawScale *= SimpleSpread(accuracy, self.spreadYawScale);
                    }
                }
            }
            orig(self);
        }
        private static void ProjectileManager_FireProjectile_FireProjectileInfo(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo fireProjectileInfo)
        {
            if (fireProjectileInfo.owner)
            {
                var comp = fireProjectileInfo.owner.GetComponent<CharacterBody>()?.masterObject?.GetComponent<RBSExtraStatsController>();
                var inputBank = fireProjectileInfo.owner.GetComponent<CharacterBody>()?.inputBank;
                if (comp && inputBank)
                {
                    float accuracy = comp.projectileAccuracy;
                    Quaternion aimDirectionQuaternion = Util.QuaternionSafeLookRotation(inputBank.aimDirection);

                    //if (ShowAnnoyingDebugText) _logger.LogMessage("Projectile Fired: " + fireProjectileInfo.projectilePrefab.name);

                    // projectile check
                    if ((ShotSpread_WhitelistProjectiles && WhitelistedProjectiles.Contains(fireProjectileInfo.projectilePrefab)) || !ShotSpread_WhitelistProjectiles)
                    {
                        Quaternion UpdatedAngle;

                        if (accuracy >= 0)
                        {
                            UpdatedAngle = Quaternion.Lerp(fireProjectileInfo.rotation, aimDirectionQuaternion, accuracy);
                        }
                        else
                        {
                            accuracy *= -1;
                            //UpdatedAngle = Quaternion.LerpUnclamped(fireProjectileInfo.rotation, aimDirectionQuaternion, accuracy);

                            //CappedAccMult *= -1;
                            //This is a random dir in cone. USED FOR INACCURACY
                            //TODO
                            //Quaternion bulletDir = GetRandomInsideCone(BaseSpreadAngle, aimDirection);
                            //_logger.LogMessage(bulletDir);
                            //UpdatedAngle = Quaternion.SlerpUnclamped(bulletDir, aimDirectionQuaternion, CappedAccMult);
                            var minSpread = Mathf.Max(0f, 1f - accuracy);
                            UpdatedAngle = Util.QuaternionSafeLookRotation(Util.ApplySpread(fireProjectileInfo.rotation.eulerAngles, minSpread, accuracy, 1f, 1f));
                        }
                        fireProjectileInfo.rotation = UpdatedAngle;
                    }
                }
            }
            orig(self, fireProjectileInfo);
        }

        private static void GenericBulletBaseState_FireBullet(On.EntityStates.GenericBulletBaseState.orig_FireBullet orig, EntityStates.GenericBulletBaseState self, Ray aimRay)
        {
            //something here to tighten spread
            orig(self, aimRay);
        }

        public class RBSExtraStatsController : MonoBehaviour
        {
            private float Scope_SpreadReduction => Items.Scope.cfgSpreadReduction.Value;
            private float Scope_SpreadReductionStack => Items.Scope.cfgSpreadReductionPerStack.Value;
            //private float[,] SpiceBonusesConstant => Equipment.Spice.SpiceBonusesConstant;
            //private float[] SpiceBonusesAdditive => Equipment.Spice.SpiceBonusesAdditive;
            public Inventory inventory;
            public int itemCount_Scope = 0;
            public int itemCount_Spice = 0;
            public float idealizedAccuracyStat = 1f;
            public float bulletAccuracy = 1f;
            public float projectileAccuracy = 1f;

            public void Start()
            {
                if (inventory)
                {
                    inventory.onInventoryChanged += UpdateInventory;
                }
                UpdateInventory();
            }

            public void OnDestroy()
            {
                inventory.onInventoryChanged -= UpdateInventory;
            }

            public void UpdateInventory()
            {
                itemCount_Scope = inventory.GetItemCount(Items.Scope.instance.ItemDef);
                itemCount_Spice = inventory.GetItemCount(Items.SpiceTally.instance.ItemDef);
                RecalculateAccuracy();
            }

            private void RecalculateAccuracy()
            {
                float ScopeMult = 0f;
                float SpiceMult = 0f;

                if (itemCount_Scope > 0)
                    ScopeMult -= (Scope_SpreadReduction + Scope_SpreadReductionStack * (itemCount_Scope - 1));

                //switch case?
                switch (itemCount_Spice)
                {
                    case 0:
                    case 1:
                    case 2:
                        break;
                    case 3:
                    case 4:
                        //SpiceMult -= SpiceBonusesConstant[itemCount_Spice, 2];
                        break;
                    case 5: //fuck IT GOES FROM 0.15 to -0.2 WHYYYYYYYYYYYYYYY hardcoded stopgap time
                        SpiceMult -= 0f;
                        break;
                    default:
                        //SpiceMult -= SpiceBonusesConstant[5, 2] + SpiceBonusesAdditive[2] * (itemCount_Spice - 4);
                        break;
                }

                var accuracy = ScopeMult + SpiceMult;
                idealizedAccuracyStat = -accuracy;

                // Bullets get better the closer they are to zero starting at a multiplier of 1.0 (since we're multiplying the spread)
                // Projectiles get better the closer they are to 1 (due to LERP) starting at a multiplier of 0.0
                // When we get max scope amount, it's a value of ~-1.1
                // Here with projectiles we get a resulting value of 1.1 rounded to 1.
                //ResultMult = -ResultMult > 1 ? 1 : -ResultMult;
                projectileAccuracy = -accuracy;

                // With bullets we have to start at 1
                // Then we evaluate it (1 - ~1.1 = -0.1)
                // We clamp it at zero because a negative multiplier converges the spread on itself and actually increases the spread.
                bulletAccuracy = 1 + accuracy <= 0 ? 0 : 1 + accuracy;
            }
        }
    }
}
