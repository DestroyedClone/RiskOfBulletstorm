using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System;
using RoR2;
using UnityEngine;
using TILER2;
using static RiskOfBulletstorm.Utils.HelperUtil;
using RoR2.Projectile;
using static RiskOfBulletstorm.BulletstormPlugin;
using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine.Networking;

namespace RiskOfBulletstorm.Items
{
    public class NuBulletstormExtraStatsController : Item<NuBulletstormExtraStatsController>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("If enabled, Shot Spread will affect the Disposable Missile Launcher." +
            "\nThis tends to make it fire forwards rather than vertically, but shouldn't affect its homing.", AutoConfigFlags.PreventNetMismatch)]
        public static bool ShotSpread_EnableDML { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("If enabled, Shot Spread will affect Loader's Hooks.", AutoConfigFlags.PreventNetMismatch)]
        public static bool ShotSpread_EnableLoader { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("If enabled, Shot Spread will only tighten the spread of SPECIFIC projectiles." +
            "\nIt is HIGHLY recommended not to disable, because alot of projectiles could break otherwise.", AutoConfigFlags.PreventNetMismatch)]
        public static bool ShotSpread_WhitelistProjectiles { get; private set; } = true;

        public override string displayName => "BulletstormExtraStatsController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";


        public static List<GameObject> WhitelistedProjectiles = new List<GameObject>
        {
            // Equipment/Items
            Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang"), // Saw
            Resources.Load<GameObject>("Prefabs/Projectiles/LunarNeedleProjectile"), // Visions of Heresy
            Resources.Load<GameObject>("prefabs/projectiles/LunarSecondaryProjectile"), // Hooks of Heresy
            Resources.Load<GameObject>("prefabs/projectiles/LunarMissileProjectile"), // Perfected

            // Survivors
                // Acrid
            Resources.Load<GameObject>("prefabs/projectiles/CrocoSpit"), // Ranged Secondary
            Resources.Load<GameObject>("prefabs/projectiles/CrocoDiseaseProjectile"), // Special
                // Artificer
            Resources.Load<GameObject>("Prefabs/Projectiles/MageFireboltBasic"),
            Resources.Load<GameObject>("Prefabs/Projectiles/MageLightningboltBasic"),
            Resources.Load<GameObject>("Prefabs/Projectiles/MageLightningBombProjectile"),
            Resources.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile"),
            Resources.Load<GameObject>("Prefabs/Projectiles/MageIcewallWalkerProjectile"),
            //MageIcewallPillarProjectile ??
                
                // Bandit2
            Resources.Load<GameObject>("prefabs/projectiles/Bandit2ShivProjectile"),


                // Captain
            Resources.Load<GameObject>("Prefabs/Projectiles/CaptainTazer"),

                // Commando
            Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile"),
            Resources.Load<GameObject>("Prefabs/Projectiles/FMJ"), //??
            Resources.Load<GameObject>("Prefabs/Projectiles/FMJRamping"),

                // Engineer
            Resources.Load<GameObject>("Prefabs/Projectiles/EngiHarpoon"),
            Resources.Load<GameObject>("Prefabs/Projectiles/EngiGrenadeProjectile"),
            Resources.Load<GameObject>("Prefabs/Projectiles/EngiMine"),
            Resources.Load<GameObject>("Prefabs/Projectiles/SpiderMine"),
            Resources.Load<GameObject>("prefabs/projectiles/EngiBubbleShield"),

                // Huntress
            EntityStates.Huntress.HuntressWeapon.FireGlaive.projectilePrefab,

                // Loader
            Resources.Load<GameObject>("prefabs/projectiles/LoaderZapCone"), //It's a projectile, go figure.
            Resources.Load<GameObject>("prefabs/projectiles/LoaderPylon"),

                // Merc
            Resources.Load<GameObject>("prefabs/projectiles/EvisProjectile"), // Ranged special

                // MUL-T
            Resources.Load<GameObject>("prefabs/projectiles/ToolbotGrenadeLauncherProjectile"), // Scrap Launcher
            Resources.Load<GameObject>("prefabs/projectiles/CryoCanisterProjectile"), // Secondary bomb

                // REX
            Resources.Load<GameObject>("prefabs/projectiles/SyringeProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SyringeProjectileHealing"), // Third syringe shot
            Resources.Load<GameObject>("prefabs/projectiles/TreebotMortarRain"),
            Resources.Load<GameObject>("prefabs/projectiles/TreebotMortar2"),
            Resources.Load<GameObject>("prefabs/projectiles/TreebotFlowerSeed"),
            Resources.Load<GameObject>("prefabs/projectiles/TreebotFruitSeedProjectile"),

            // Enemies
                // Beetle Queen
            Resources.Load<GameObject>("prefabs/projectiles/BeetleQueenSpit"), // Beetle Spit
                // Mithrix
            Resources.Load<GameObject>("prefabs/projectiles/LunarShardProjectile"),
                // Clay Dunestrider
            Resources.Load<GameObject>("Prefabs/Projectiles/Tarball"),
            EntityStates.ClayBoss.ClayBossWeapon.FireBombardment.projectilePrefab, //
                // Grovetender
            Resources.Load<GameObject>("prefabs/projectiles/GravekeeperHookProjectile"), //not used?
            Resources.Load<GameObject>("prefabs/projectiles/GravekeeperHookProjectileSimple"), //????
            Resources.Load<GameObject>("GravekeeperTrackingFireball"),
                // Imp + Imp Overlord
            Resources.Load<GameObject>("prefabs/projectiles/ImpVoidspikeProjectile"),
            EntityStates.ImpMonster.FireSpines.projectilePrefab, 
                // Elder Lemurian
            Resources.Load<GameObject>("prefabs/projectiles/LemurianBigFireball"), 
                // Void Reaver
            Resources.Load<GameObject>("prefabs/projectiles/NullifierBombProjectile"),
                // Alloy Worship Unit / Solus Control Unit
            Resources.Load<GameObject>("prefabs/projectiles/RoboBallProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SuperRoboBallProjectile"), //idk??
                // Scavenger
            Resources.Load<GameObject>("prefabs/projectiles/ScavEnergyCannonProjectile"),
                // Malachite Urchin
            Resources.Load<GameObject>("prefabs/projectiles/UrchinSeekingProjectile"),
                // Wandering Vagrant
            EntityStates.VagrantMonster.Weapon.JellyBarrage.projectilePrefab,
                // Alloy Vulture
            Resources.Load<GameObject>("prefabs/projectiles/WindbladeProjectile"), 
                // Beetle Guard
            Resources.Load<GameObject>("prefabs/projectiles/Sunder"), 
                // Brass Contraption
            Resources.Load<GameObject>("prefabs/projectiles/BellBall"), 
                // Lemurian
            Resources.Load<GameObject>("prefabs/projectiles/Fireball"), //fireball
                // Hermit Crab
            Resources.Load<GameObject>("prefabs/projectiles/HermitCrabBombProjectile"), 
                // Lunar Golem
            Resources.Load<GameObject>("prefabs/projectiles/LunarGolemTwinShotProjectile"), 
                // Lunar Wisp
            Resources.Load<GameObject>("prefabs/projectiles/LunarWispTrackingBomb"), 
                // Lunar Exploder
            Resources.Load<GameObject>("prefabs/projectiles/LunarExploderShardProjectile"),
                // Mini Mushrum
            Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectile"), 
                // Void Reaver
            Resources.Load<GameObject>("prefabs/projectiles/NullifierPreBombProjectile"), 
                // Greater Wisp
            Resources.Load<GameObject>("prefabs/projectiles/WispCannon"),
                // Grandparent
            Resources.Load<GameObject>("prefabs/projectiles/GrandparentBoulder"),
            Resources.Load<GameObject>("prefabs/projectiles/GrandparentGravSphere"),
        };

        #region SetupProjectiles
        private static readonly GameObject DisposableMissileLauncherPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MissileProjectile");
        private static readonly GameObject LoaderHookPrefab = Resources.Load<GameObject>("prefabs/projectiles/LoaderHook");
        private static readonly GameObject LoaderYankHookPrefab = Resources.Load<GameObject>("prefabs/projectiles/LoaderYankHook");

        public override void SetupLate()
        {
            base.SetupLate();
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
        public override void SetupConfig()
        {
            base.SetupConfig();
            if (ShotSpread_EnableDML)
                WhitelistedProjectiles.Add(DisposableMissileLauncherPrefab);
            if (ShotSpread_EnableLoader)
            {
                WhitelistedProjectiles.Add(LoaderHookPrefab);
                WhitelistedProjectiles.Add(LoaderYankHookPrefab);
            }
        }
        #endregion

        public override void Install()
        {
            base.Install();
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;

            // Accuracy //
            On.RoR2.BulletAttack.Fire += AdjustSpreadBullets;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileManager_FireProjectile_FireProjectileInfo;
            On.EntityStates.GenericBulletBaseState.FireBullet += RedirectDirection;
        }
        public override void Uninstall()
        {
            base.Uninstall();
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;

            // Accuracy //
            On.RoR2.BulletAttack.Fire -= AdjustSpreadBullets;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= ProjectileManager_FireProjectile_FireProjectileInfo;
            On.EntityStates.GenericBulletBaseState.FireBullet -= RedirectDirection;
        }

        private void RedirectDirection(On.EntityStates.GenericBulletBaseState.orig_FireBullet orig, GenericBulletBaseState self, Ray aimRay)
        {
            //something here to tighten spread
            orig(self,aimRay);
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster obj)
        {
            if (obj && obj.inventory && !obj.gameObject.GetComponent<RBSExtraStatsController>())
            {
                var comp = obj.gameObject.AddComponent<RBSExtraStatsController>();
                comp.inventory = obj.inventory;
            }
        }

        private void ProjectileManager_FireProjectile_FireProjectileInfo(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo fireProjectileInfo)
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
                            UpdatedAngle = Quaternion.LerpUnclamped(fireProjectileInfo.rotation, aimDirectionQuaternion, accuracy);

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

        private void AdjustSpreadBullets(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
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

        public static float SimpleSpread(float accuracy, float originalValue, float multiplier = 1f)
        {
            return originalValue == 0 ? accuracy * multiplier : originalValue * accuracy;
        }

        public class RBSExtraStatsController : MonoBehaviour
        {
            private float Scope_SpreadReduction => Scope.Scope_SpreadReduction;
            private float Scope_SpreadReductionStack => Scope.Scope_SpreadReductionStack;
            private float[,] SpiceBonusesConstant => Spice.SpiceBonusesConstant; 
            private float[] SpiceBonusesAdditive => Spice.SpiceBonusesAdditive;
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

            public void UpdateInventory()
            {
                itemCount_Scope = inventory.GetItemCount(Scope.instance.itemDef);
                itemCount_Spice = inventory.GetItemCount(Spice.SpiceTally);
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
                        SpiceMult -= SpiceBonusesConstant[itemCount_Spice, 2];
                        break;
                    case 5: //fuck IT GOES FROM 0.15 to -0.2 WHYYYYYYYYYYYYYYY hardcoded stopgap time
                        SpiceMult -= 0f;
                        break;
                    default:
                        SpiceMult -= SpiceBonusesConstant[5, 2] + SpiceBonusesAdditive[2] * (itemCount_Spice - 4);
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
