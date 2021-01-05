using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;
using static RiskOfBulletstorm.Utils.HelperUtil;
using RoR2.Projectile;
using static RiskOfBulletstorm.RiskofBulletstorm;
using static EntityStates.FireNailgun;

namespace RiskOfBulletstorm.Items
{
    public class BulletstormExtraStatsController : Item_V2<BulletstormExtraStatsController>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("If enabled, Shot Spread will affect the Disposable Missile Launcher." +
            "\nThis tends to make it fire forwards rather than vertically, but shouldn't affect its homing.", AutoConfigFlags.PreventNetMismatch)]
        public static bool ShotSpread_EnableDML { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("If enabled, Shot Spread will affect Loader's Hooks.", AutoConfigFlags.PreventNetMismatch)]
        public static bool ShotSpread_EnableLoader { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("If enabled, Shot Spread will only tighten the spread of specific projectiles." +
            "\nIt is HIGHLY recommended not to disable, because alot of projectiles could break otherwise.", AutoConfigFlags.PreventNetMismatch)]
        public static bool ShotSpread_WhitelistProjectiles { get; private set; } = true;

        public override string displayName => "BulletstormExtraStatsController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        private static readonly GameObject DisposableMissileLauncherPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MissileProjectile");
        private static readonly GameObject LoaderHookPrefab = Resources.Load<GameObject>("prefabs/projectiles/LoaderHook");
        private static readonly GameObject LoaderYankHookPrefab = Resources.Load<GameObject>("prefabs/projectiles/LoaderYankHook");

        private float Scope_SpreadReduction;
        private float Scope_SpreadReductionStack;
        private ItemIndex ItemIndex_Scope;
        private ItemIndex ItemIndex_SpiceTally;

        private float[,] SpiceBonuses;
        private float[] SpiceBonusesAdditive;
        private float[] SpiceBonusesConstantMaxed;

        public static List<GameObject> WhitelistedProjectiles = new List<GameObject>
        {
            // Equipment/Items
            Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang"), //Saw
            Resources.Load<GameObject>("Prefabs/Projectiles/LunarNeedleProjectile"), // Visions of Heresy

            // Survivors
                // Acrid
            Resources.Load<GameObject>("prefabs/projectiles/CrocoSpit"),
            Resources.Load<GameObject>("prefabs/projectiles/CrocoDiseaseProjectile"),
                // Artificer
            Resources.Load<GameObject>("Prefabs/Projectiles/MageFireboltBasic"),
            Resources.Load<GameObject>("Prefabs/Projectiles/MageLightningboltBasic"),
            Resources.Load<GameObject>("Prefabs/Projectiles/MageLightningBombProjectile"),
            Resources.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile"),
            Resources.Load<GameObject>("Prefabs/Projectiles/MageIcewallWalkerProjectile"),
            //MageIcewallPillarProjectile

                // Captain
            Resources.Load<GameObject>("Prefabs/Projectiles/CaptainTazer"),
            Resources.Load<GameObject>("Prefabs/Projectiles/FMJ"), //??

                // Commando
            Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile"),
            Resources.Load<GameObject>("Prefabs/Projectiles/CaptainTazer"),

                // Engineer
            Resources.Load<GameObject>("Prefabs/Projectiles/EngiHarpoon"), //engineer harpoon
            Resources.Load<GameObject>("Prefabs/Projectiles/EngiGrenadeProjectile"),
            Resources.Load<GameObject>("Prefabs/Projectiles/EngiMine"),
            Resources.Load<GameObject>("Prefabs/Projectiles/SpiderMine"),
            Resources.Load<GameObject>("prefabs/projectiles/EngiBubbleShield"),

                // Huntress
            EntityStates.Huntress.HuntressWeapon.FireGlaive.projectilePrefab,

                // Loader
            Resources.Load<GameObject>("prefabs/projectiles/LoaderZapCone"),
            Resources.Load<GameObject>("prefabs/projectiles/LoaderPylon"),

                // Merc
            Resources.Load<GameObject>("prefabs/projectiles/EvisProjectile"),

                // MUL-T
            Resources.Load<GameObject>("prefabs/projectiles/ToolbotGrenadeLauncherProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/CryoCanisterProjectile"),

                // REX
            Resources.Load<GameObject>("prefabs/projectiles/SyringeProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SyringeProjectileHealing"),
            Resources.Load<GameObject>("prefabs/projectiles/TreebotMortarRain"),
            Resources.Load<GameObject>("prefabs/projectiles/TreebotMortar2"),
            Resources.Load<GameObject>("prefabs/projectiles/TreebotFlowerSeed"),

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
                // Lunar Chimera
            Resources.Load<GameObject>("prefabs/projectiles/LunarGolemTwinShotProjectile"), 
                // Lunar Wisp
            Resources.Load<GameObject>("prefabs/projectiles/LunarWispTrackingBomb"), 
                // Mini Mushrum
            Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectile"), 
                // Void Reaver
            Resources.Load<GameObject>("prefabs/projectiles/NullifierPreBombProjectile"), 
                // Greater Wisp
            Resources.Load<GameObject>("prefabs/projectiles/WispCannon"),
        };
        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }
        public override void SetupLate()
        {
            base.SetupLate();
            // SCOPE //
            Scope_SpreadReduction = Scope.Scope_SpreadReduction;
            Scope_SpreadReductionStack = Scope.Scope_SpreadReductionStack;

            // ITEM COUNTS //
            ItemIndex_Scope = Scope.instance.catalogIndex;
            ItemIndex_SpiceTally = Spice.SpiceTally;

            // SPICE //
            SpiceBonuses = Spice.SpiceBonuses;
            SpiceBonusesAdditive = Spice.SpiceBonusesAdditive;
            SpiceBonusesConstantMaxed = Spice.SpiceBonusesConstantMaxed;

            // MODDED //
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

                // ROB - MINER's DIRESEEKER // why is it different
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

            };
            // MODDED //
            _logger.LogMessage("Projectile Whitelist: Adding modded projectiles.");

            int moddedProjectilesAdded = 0;
            foreach (string projectileString in moddedProjectileStrings)
            {
                var projectileIndex = ProjectileCatalog.FindProjectileIndex(projectileString);
                //failures to find defaults to -1
                if (projectileIndex > 0)
                {
                    WhitelistedProjectiles.Add(ProjectileCatalog.GetProjectilePrefab(projectileIndex));
                    _logger.LogMessage("Projectile Whitelist: Added projectile = "+projectileString);
                    moddedProjectilesAdded++;
                }
            }
            _logger.LogMessage("Projectile Whitelist: Added "+moddedProjectilesAdded+" modded projectiles.");
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
        public override void Install()
        {
            base.Install();
            // ACCURACY //
            On.RoR2.BulletAttack.Fire += AdjustSpreadBullets;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += AdjustSpreadProjectiles;
            On.EntityStates.BaseNailgunState.FireBullet += BaseNailgunState_FireBullet;

            // SPEED //
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += AdjustSpeedEnemyProjectile;
        }

        //EntityStates.BaseNailgunState nailgunState = new EntityStates.BaseNailgunState();

        private void BaseNailgunState_FireBullet(On.EntityStates.BaseNailgunState.orig_FireBullet orig, EntityStates.BaseNailgunState self, Ray aimRay, int bulletCount, float spreadPitchScale, float spreadYawScale)
        {
            var characterBody = self.characterBody;
            if (characterBody)
            {
                var inventory = characterBody.inventory;
                if (inventory)
                {
                    var ResultMult = CalculateSpreadMultiplier(inventory, false);
                    characterBody.SetSpreadBloom(Mathf.Min(0, characterBody.spreadBloomAngle * ResultMult), false);


                    spreadPitchScale = Mathf.Min(0, spreadPitchScale * ResultMult);
                    spreadYawScale = Mathf.Min(0, spreadYawScale * ResultMult);

                    // fuck it,, we overriding
                    // fuck IL too

                    self.fireNumber++;
                    self.StartAimMode(aimRay, 3f, false);
                    if (self.isAuthority)
                    {
                        new BulletAttack
                        {
                            aimVector = aimRay.direction,
                            origin = aimRay.origin,
                            owner = self.gameObject,
                            weapon = self.gameObject,
                            bulletCount = (uint)bulletCount,
                            damage = self.damageStat * EntityStates.BaseNailgunState.damageCoefficient,
                            damageColorIndex = DamageColorIndex.Default,
                            damageType = DamageType.Generic,
                            falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                            force = EntityStates.BaseNailgunState.force,
                            HitEffectNormal = false,
                            procChainMask = default,
                            procCoefficient = EntityStates.BaseNailgunState.procCoefficient,
                            maxDistance = EntityStates.BaseNailgunState.maxDistance,
                            radius = 0f,
                            isCrit = self.RollCrit(),
                            muzzleName = EntityStates.BaseNailgunState.muzzleName,
                            minSpread = 0f,
                            hitEffectPrefab = EntityStates.BaseNailgunState.hitEffectPrefab,
                            maxSpread = self.characterBody.spreadBloomAngle, //THIS
                            smartCollision = false,
                            sniper = false,
                            spreadPitchScale = spreadPitchScale * spreadPitchScale,
                            spreadYawScale = spreadYawScale * spreadYawScale,
                            tracerEffectPrefab = EntityStates.BaseNailgunState.tracerEffectPrefab
                        }.Fire();
                    }
                    if (self.characterBody && ResultMult > 0)
                    {
                        var bloomLerp = EntityStates.BaseNailgunState.spreadBloomValue * ResultMult;
                        self.characterBody.AddSpreadBloom(bloomLerp);
                    }
                    Util.PlaySound(EntityStates.BaseNailgunState.fireSoundString, self.gameObject);
                    EffectManager.SimpleMuzzleFlash(EntityStates.BaseNailgunState.muzzleFlashPrefab, self.gameObject, EntityStates.BaseNailgunState.muzzleName, false);
                    self.PlayAnimation("Gesture, Additive", "FireNailgun");
                }
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            // ACCURACY //
            On.RoR2.BulletAttack.Fire -= AdjustSpreadBullets;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= AdjustSpreadProjectiles;
            On.EntityStates.BaseNailgunState.FireBullet -= BaseNailgunState_FireBullet;

            // SPEED //
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= AdjustSpeedEnemyProjectile;
        }

        private void AdjustSpeedEnemyProjectile(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo fireProjectileInfo)
        {
            GameObject owner = fireProjectileInfo.owner;
            if (owner)
            {
                CharacterBody cb = owner.GetComponent<CharacterBody>();
                if (cb)
                {
                    var teamComponent = cb.teamComponent;
                    if (teamComponent)
                    {
                        var teamIndex = teamComponent.teamIndex;
                        if (teamIndex != TeamIndex.Player) //Enemies only
                        {
                            var prefab = fireProjectileInfo.projectilePrefab;
                            if (prefab)
                            {
                                ProjectileSimple projectileSimple = prefab.GetComponent<ProjectileSimple>();
                                if (projectileSimple)
                                {
                                    //var oldSpeed = projectileSimple.velocity;
                                    var speedMult = CalculateEnemyProjectileSpeedMultiplier();

                                    projectileSimple.velocity *= 1f + speedMult;
                                }
                            }
                        }
                        //fireProjectileInfo._speedOverride = fireProjectileInfo.
                    }
                }
            }
            orig(self, fireProjectileInfo);
        }

        private float CalculateEnemyProjectileSpeedMultiplier()
        {
            var characterBodyWithMostSpice = GetPlayerWithMostItemIndex(ItemIndex_SpiceTally);
            float SpiceMult = 0f;
            if (characterBodyWithMostSpice)
            {
                int ItemCount_Spice = characterBodyWithMostSpice.inventory.GetItemCount(ItemIndex_SpiceTally);

                if (ItemCount_Spice > 0)
                {
                    if (ItemCount_Spice > 4)
                    {
                        SpiceMult = SpiceBonusesConstantMaxed[3];
                    }
                    else
                    {
                        SpiceMult = SpiceBonuses[ItemCount_Spice, 3];
                    }
                }
            }
            return SpiceMult;
        }

        private float CalculateSpreadMultiplier(Inventory inventory, bool isProjectile)
        {
            int ItemCount_Scope = inventory.GetItemCount(ItemIndex_Scope);
            int ItemCount_Spice = inventory.GetItemCount(ItemIndex_SpiceTally);
            float SpiceMult = 0f;
            float ScopeMult = 0f;

            if (ItemCount_Scope > 0)
                ScopeMult -= (Scope_SpreadReduction + Scope_SpreadReductionStack * (ItemCount_Scope - 1));

            //switch case?
            switch (ItemCount_Spice)
            {
                case 0:
                case 1:
                case 2:
                    break;
                case 3:
                case 4:
                    SpiceMult -= SpiceBonuses[ItemCount_Spice, 2];
                    break;
                case 5: //fuck IT GOES FROM 0.15 to -0.2 WHYYYYYYYYYYYYYYY hardcoded stopgap time
                    SpiceMult -= 0f;
                    break;
                default:
                    SpiceMult -= SpiceBonusesConstantMaxed[2] + SpiceBonusesAdditive[2] * (ItemCount_Spice - 4);
                    break;
            }

            /*float Clamp(float value, float min = 0f, float max = 1f)
            {
                if (value <= min) return min;
                else if (value > max) return max;
                else return value;
            }*/

            float ResultMult = ScopeMult + SpiceMult;

            if (isProjectile)
            {
                // Bullets get better the closer they are to zero starting at a multiplier of 1.0 (multiplying spread)
                // Projectiles get better the closer they are to 1 (due to LERP) starting at a multiplier of 0.0
                // When we get max scope amount, it's a value of ~-1.1
                // Here with projectiles we get a resulting value of 1.1 rounded to 1.
                //ResultMult = -ResultMult > 1 ? 1 : -ResultMult;
                ResultMult *= -1;
            } else
            {
                // With bullets we have to start at 1
                // Then we evaluate it (1 - ~1.1 = -0.1)
                // We clamp it at zero because a negative multiplier might result in a weird inverse increase in spread.
                ResultMult = 1 + ResultMult <= 0 ? 0 : 1 + ResultMult;
            }

            //ResultMult = Clamp(ResultMult);
            //ResultMult = ResultMult;

            //Debug.Log("Scope: "+ ResultMult);
            Debug.Log("Scope: [isProjectile"+ isProjectile+ "] Scope: "+ ScopeMult + " + SpiceMult: " + SpiceMult + " = " + ResultMult);
            return ResultMult;
        }

        private void AdjustSpreadProjectiles(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo fireProjectileInfo)
        {
            GameObject owner = fireProjectileInfo.owner;
            if (owner)
            {
                CharacterBody cb = owner.GetComponent<CharacterBody>();
                if (cb)
                {
                    Inventory inventory = cb.inventory;
                    if (inventory)
                    {
                        InputBankTest input = cb.inputBank;
                        if (input)
                        {
                            float ResultMult = CalculateSpreadMultiplier(inventory, true);
                            GameObject projectilePrefab = fireProjectileInfo.projectilePrefab;
                            Quaternion aimDir = Util.QuaternionSafeLookRotation(input.aimDirection);
                            Quaternion rotation = fireProjectileInfo.rotation;

                            //_logger.LogMessage("Projectile Fired: "+ fireProjectileInfo.projectilePrefab.name);
                            bool isProjectileAllowed = WhitelistedProjectiles.Contains(projectilePrefab);

                            if ((ShotSpread_WhitelistProjectiles && isProjectileAllowed) || !ShotSpread_WhitelistProjectiles)
                            {
                                if (ResultMult >= 0)
                                {
                                    Quaternion UpdatedAngle = Quaternion.Lerp(rotation, aimDir, ResultMult);
                                    fireProjectileInfo.rotation = UpdatedAngle;
                                    //Debug.Log("Projectile Fired: " + projectilePrefab.name + " at angle "+ fireProjectileInfo.rotation+" => "+ UpdatedAngle) ;
                                    //Chat.AddMessage("Scope Lerp: " + aimDir + " and " + rotation + " resulting " + UpdatedAngle);
                                } else
                                {
                                    ResultMult = Mathf.Abs(ResultMult);
                                    var lessThan = 360 - 360 * ResultMult;
                                    var moreThan = 360 + 360 * ResultMult;
                                    float rand1 = Random.Range(rotation.x * lessThan, rotation.x * moreThan);
                                    float rand2 = Random.Range(rotation.y * lessThan, rotation.y * moreThan);
                                    float rand3 = Random.Range(rotation.z * lessThan, rotation.z * moreThan);
                                    //var rand4 = UnityEngine.Random.Range(deviation4, -deviation4);
                                    var tempdev = Util.QuaternionSafeLookRotation(new Vector3(rand1, rand2, rand3));
                                    //var tempdev = new Vector3(rand1, rand2, rand3);
                                    //RiskofBulletstorm._logger.LogInfo(printDifference(fireProjectileInfo.rotation.x);

                                    //Debug.Log("Scope: " + fireProjectileInfo.rotation + " => " + tempdev.x + " " + tempdev.y + " " + tempdev.z + " " + tempdev.w);

                                    //int directionModifier = Util.CheckRoll(50) ? 1 : -1;

                                    //fireProjectileInfo.rotation = Quaternion.Euler(tempdev);
                                    fireProjectileInfo.rotation *= tempdev;
                                }
                            }
                        }
                    }
                }
            }
            orig(self, fireProjectileInfo);
        }

        private void AdjustSpreadBullets(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            //doesn't work on MULT?????
            CharacterBody characterBody = self.owner.gameObject.GetComponent<CharacterBody>();
            if (characterBody)
            {
                var inventory = characterBody.inventory;
                if (inventory)
                {
                    var ResultMult = CalculateSpreadMultiplier(inventory, false);

                    characterBody.SetSpreadBloom(Mathf.Min(0, characterBody.spreadBloomAngle * ResultMult), false);

                    //Debug.Log("Bulletscope: maxspread: "+self.maxSpread+" multiplier: "+ResultMult+" result: "+ Mathf.Max(self.maxSpread * ResultMult, 0));

                    if (self.radius == 0) //(maybe?)
                        self.radius = ResultMult;

                    self.maxSpread *= ResultMult;

                    self.minSpread *= ResultMult;

                    self.spreadPitchScale *= ResultMult;
                    self.spreadYawScale *= ResultMult;

                    //self.owner.GetComponent<CharacterBody>().SetSpreadBloom(ResultMult, false);

                }
            }
            orig(self);
        }
    }
}
