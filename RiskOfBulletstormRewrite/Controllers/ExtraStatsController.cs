using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using static RiskOfBulletstormRewrite.Utils.ItemHelpers;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class ExtraStatsController : ControllerBase<ExtraStatsController>
    {
        /// <summary> Config Value to enable <b>Disposable Missile Launcher's rockets</b> being affected by RBSAccuracy. </summary>
        public static bool cfgShotSpread_EnableDML { get; private set; } = true;

        /// <summary> Config Value to enable the Survivor, <b>Loader</b>, being affected by RBSAccuracy. </summary>
        public static bool cfgShotSpread_EnableLoader { get; private set; } = false;

        /// <summary> Config Value to enable <b>only specific projectiles</b>, being affected by RBSAccuracy. </summary>
        public static bool cfgShotSpread_WhitelistProjectiles { get; private set; } = true;

        /// <summary> List of whitelisted <b>vanilla</b> projectiles for RBSAccuracy </summary>
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
            Load<GameObject>("RoR2/Base/Mage/MageLightningBombProjectile.prefab"),
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

        /// <summary> List of whitelisted <b>modded</b> projectiles for RBSAccuracy </summary>
        public static List<string> ModdedWhitelistedProjectiles = new List<string>();

        public static ConfigEntry<float> cfgDamageMultiplierPerStack;

        private const string CategoryNameShotSpread = "ExtraStatsShotSpread";

        /// <summary> Method to return a float from the  </summary>
        /// <param name="accuracy"></param>
        /// <param name="multiplier"></param>
        /// <param name="originalValue"></param>
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

        /// <summary>
        /// Method which checks for a large list of modded projectiles to cache the names of projectiles that are actually available
        /// </summary>
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
            cfgShotSpread_EnableDML = config.Bind(CategoryNameShotSpread, "Disposable Missile Launcher", true, "If enabled, Shot Spread will affect the Disposable Missile Launcher. " +
                "\nThis tends to make it fire forwards rather than vertically, but shouldn't affect its homing.").Value;
            cfgShotSpread_EnableLoader = config.Bind(CategoryNameShotSpread, "Loader Hooks", false, "If enabled, Shot Spread will affect Loader's Hooks.").Value;
            cfgShotSpread_WhitelistProjectiles = config.Bind(CategoryNameShotSpread, "Whitelisted Projectiles", true, "If enabled, Shot Spread will only tighten the spread of SPECIFIC projectiles." +
                "\nIt is HIGHLY recommended not to disable, because alot of projectiles could break otherwise.").Value;
            cfgDamageMultiplierPerStack = config.Bind(CategoryNameShotSpread, "Damage Multiplier Past Max Accuracy", 0.01f, "How much should the damage be multiplied per stack past max accuracy?");


            if (cfgShotSpread_EnableDML)
                WhitelistedProjectiles.Add(Load<GameObject>("RoR2/Base/Common/MissileProjectile.prefab"));
            if (cfgShotSpread_EnableLoader)
            {
                WhitelistedProjectiles.Add(Load<GameObject>("RoR2/Base/Loader/LoaderHook.prefab"));
                WhitelistedProjectiles.Add(Load<GameObject>("RoR2/Base/Loader/LoaderYankHook.prefab"));
            }
            R2API.RecalculateStatsAPI.GetStatCoefficients += ApplyDamageIncreaseToMaxAccuracy;
        }


        private static void ApplyDamageIncreaseToMaxAccuracy(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.master && sender.master.TryGetComponent(out RBSExtraStatsController extraStatsController))
            {
                var difference = extraStatsController.unboundAccuracyStat - 1;
                if (difference > 1)
                {
                    args.damageMultAdd += difference * cfgDamageMultiplierPerStack.Value;
                }
            }
        }
        public override void Hooks()
        {
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
            // Accuracy //
            On.RoR2.BulletAttack.Fire += AdjustSpreadBullets;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileManager_FireProjectile_FireProjectileInfo;
            On.EntityStates.GenericBulletBaseState.FireBullet += GenericBulletBaseState_FireBullet;
        }

        /// <summary>
        /// Adds the AccuracyController component to the character master
        /// </summary>
        /// <param name="characterMaster">The characterMaster to add the controller to</param>
        private static void CharacterMaster_onStartGlobal(CharacterMaster characterMaster)
        {
            if (characterMaster && characterMaster.inventory && !characterMaster.gameObject.GetComponent<RBSExtraStatsController>())
            {
                var comp = characterMaster.gameObject.AddComponent<RBSExtraStatsController>();
                comp.inventory = characterMaster.inventory;
            }
        }

        /// <summary>
        /// Override method to affect <b>BulletAttacks</b> using RBSAccuracy
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private static void AdjustSpreadBullets(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            if (!self.owner) goto EarlyReturn;

            var body = self.owner.gameObject.GetComponent<CharacterBody>();
            if (!body) goto EarlyReturn;

            var comp = body.masterObject.GetComponent<RBSExtraStatsController>();
            if (!comp) goto EarlyReturn;

            var accuracy = comp.bulletAccuracy;

            body.SetSpreadBloom(Mathf.Min(0, body.spreadBloomAngle * accuracy), false);

            self.maxSpread = SimpleSpread(self.maxSpread, 1.15f);

            self.minSpread = SimpleSpread(accuracy, self.minSpread);

            if (body.inputBank) self.aimVector = Vector3.Lerp(self.aimVector, body.inputBank.aimDirection, 1 - accuracy);

            self.spreadPitchScale *= SimpleSpread(accuracy, self.spreadPitchScale);
            self.spreadYawScale *= SimpleSpread(accuracy, self.spreadYawScale);
        EarlyReturn:
            orig(self);
        }

        private static bool IsProjectileAffected(FireProjectileInfo fireProjectileInfo)
        {
            return (cfgShotSpread_WhitelistProjectiles && WhitelistedProjectiles.Contains(fireProjectileInfo.projectilePrefab)) || !cfgShotSpread_WhitelistProjectiles;
        }

        /// <summary>
        /// Override method to affect <b>Projectiles</b> using RBSAccuracy
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="fireProjectileInfo"></param>
        private static void ProjectileManager_FireProjectile_FireProjectileInfo(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo fireProjectileInfo)
        {
            if (!fireProjectileInfo.owner) goto EarlyReturn; 
            if (!fireProjectileInfo.owner.TryGetComponent(out CharacterBody body) || !body.master) goto EarlyReturn;
            if (!body.master.TryGetComponent(out RBSExtraStatsController comp)) goto EarlyReturn;

            var inputBank = fireProjectileInfo.owner.GetComponent<CharacterBody>()?.inputBank;
            if (!inputBank) goto EarlyReturn; 
            
            float accuracy = comp.projectileAccuracy;

            //if (ShowAnnoyingDebugText) _logger.LogMessage("Projectile Fired: " + fireProjectileInfo.projectilePrefab.name);

            // projectile check
            if (IsProjectileAffected(fireProjectileInfo))
            {
                Quaternion aimDirectionQuaternion = Util.QuaternionSafeLookRotation(inputBank.aimDirection);
                Quaternion UpdatedAngle;

                if (accuracy >= 0)
                {
                    UpdatedAngle = Quaternion.Lerp(fireProjectileInfo.rotation, aimDirectionQuaternion, accuracy);
                }
                else
                {
                    //accuracy *= -1;
                    //UpdatedAngle = Quaternion.LerpUnclamped(fireProjectileInfo.rotation, aimDirectionQuaternion, accuracy);

                    //CappedAccMult *= -1;
                    //This is a random dir in cone. USED FOR INACCURACY
                    //TODO
                    //Quaternion bulletDir = GetRandomInsideCone(BaseSpreadAngle, aimDirection);
                    //_logger.LogMessage(bulletDir);
                    //UpdatedAngle = Quaternion.SlerpUnclamped(bulletDir, aimDirectionQuaternion, CappedAccMult);
                    /*var minSpread = Mathf.Max(0f, 1f - accuracy);
                    UpdatedAngle = Util.QuaternionSafeLookRotation(Util.ApplySpread(fireProjectileInfo.rotation.eulerAngles, minSpread, accuracy, 1f, 1f));*/
                    UpdatedAngle = Quaternion.LerpUnclamped(fireProjectileInfo.rotation, aimDirectionQuaternion, 1 + accuracy);
                }
                fireProjectileInfo.rotation = UpdatedAngle;
            }
        EarlyReturn:
            orig(self, fireProjectileInfo);
        }

        private static void GenericBulletBaseState_FireBullet(On.EntityStates.GenericBulletBaseState.orig_FireBullet orig, EntityStates.GenericBulletBaseState self, Ray aimRay)
        {
            //something here to tighten spread
            orig(self, aimRay);
        }

        /// <summary>
        /// Component to control a CharacterMaster's RBSAccuracy
        /// </summary>
        public class RBSExtraStatsController : MonoBehaviour
        {
            private float Scope_SpreadReduction => Items.Scope.cfgSpreadReduction;
            private float Scope_SpreadReductionStack => Items.Scope.cfgSpreadReductionPerStack;
            private float Spice_SpreadReduction => Equipment.Spice2.cfgStatAccuracy;
            private float Spice_SpreadReductionStack => Equipment.Spice2.cfgStatAccuracyStack;

            //private float[,] SpiceBonusesConstant => Equipment.Spice.SpiceBonusesConstant;
            //private float[] SpiceBonusesAdditive => Equipment.Spice.SpiceBonusesAdditive;
            public Inventory inventory;

            public int itemCount_Scope = 0;
            public int itemCount_Spice = 0;

            /// <summary>
            /// "Pretty" accuracy stat, effective for most cases. 0.78 = 78% more accurate
            /// </summary>
            public float idealizedAccuracyStat = 1f;

            public float unboundAccuracyStat = 1;

            public float bulletAccuracy = 1f;
            public float projectileAccuracy = 1f;
            public float itemCount_Curse = 0f;
            public float curse = 0;

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
                itemCount_Curse = inventory.GetItemCount(Items.CurseTally.instance.ItemDef);
                RecalculateAccuracy();
                RecalculateCurse();
            }

            public void RecalculateCurse()
            {
                curse = 0f;
                //curse addition
                curse += itemCount_Curse * 0.5f;

                //curse removal
                //curse -= itemCount_CurseReduction * 0.5f;
            }

            public float scopeMult = 0;
            public float spiceMult = 0;

            private void RecalculateAccuracy()
            {
                //A reduction is tighter
                //An increase is looser

                float ScopeMult = 0f;
                float SpiceMult = 0f;

                if (itemCount_Scope > 0)
                    ScopeMult -= (Scope_SpreadReduction + Scope_SpreadReductionStack * (itemCount_Scope - 1));
                scopeMult = ScopeMult;

                if (itemCount_Spice > 0)
                    SpiceMult += (-Spice_SpreadReduction - Spice_SpreadReductionStack * (itemCount_Spice - 1));
                spiceMult = SpiceMult;

                unboundAccuracyStat = ScopeMult + SpiceMult;
                idealizedAccuracyStat = -unboundAccuracyStat;

                // Bullets get better the closer they are to zero starting at a multiplier of 1.0 (since we're multiplying the spread)
                // Projectiles get better the closer they are to 1 (due to LERP) starting at a multiplier of 0.0
                // When we get max scope amount, it's a value of ~-1.1
                // Here with projectiles we get a resulting value of 1.1 rounded to 1.
                //ResultMult = -ResultMult > 1 ? 1 : -ResultMult;
                projectileAccuracy = -unboundAccuracyStat;

                // With bullets we have to start at 1
                // Then we evaluate it (1 - ~1.1 = -0.1)
                // We clamp it at zero because a negative multiplier converges the spread on itself and actually increases the spread. Learned that from tf2 custom weapons, assuming it applies here.
                bulletAccuracy = 1 + unboundAccuracyStat <= 0 ? 0 : 1 + unboundAccuracyStat;
            }
        }
    }
}