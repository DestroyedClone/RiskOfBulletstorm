﻿using System.Collections.Generic;
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

namespace RiskOfBulletstorm.Items
{
    public class BulletstormExtraStatsController : Item<BulletstormExtraStatsController>
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

        private readonly bool ShowAnnoyingDebugText = true;

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
        private ItemDef itemDef_Scope => Scope.instance.itemDef;
        private ItemDef itemDef_SpiceTally => Spice.SpiceTally;

        private float[,] SpiceBonusesConstant;
        private float[] SpiceBonusesAdditive;

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
        public override void SetupLate()
        {
            base.SetupLate();
            // SCOPE //
            Scope_SpreadReduction = Scope.Scope_SpreadReduction;
            Scope_SpreadReductionStack = Scope.Scope_SpreadReductionStack;

            // ITEM COUNTS //
            //ItemIndex_Scope = Scope.instance.catalogIndex;
            //ItemIndex_SpiceTally = Spice.SpiceTally.itemIndex;

            // SPICE //
            SpiceBonusesConstant = Spice.SpiceBonusesConstant;
            SpiceBonusesAdditive = Spice.SpiceBonusesAdditive;

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
            // MODDED //
            _logger.LogMessage("[Risk of Bulletstorm] Projectile Whitelist: Adding modded projectiles.");

            int moddedProjectilesAdded = 0;
            foreach (string projectileString in moddedProjectileStrings)
            {
                var projectileIndex = ProjectileCatalog.FindProjectileIndex(projectileString);
                //failures to find defaults to -1
                if (projectileIndex > 0)
                {
                    WhitelistedProjectiles.Add(ProjectileCatalog.GetProjectilePrefab(projectileIndex));
                    _logger.LogMessage("[Risk of Bulletstorm] Projectile Whitelist: Added projectile = "+projectileString);
                    moddedProjectilesAdded++;
                }
            }
            _logger.LogMessage("[Risk of Bulletstorm] Projectile Whitelist: Added " + moddedProjectilesAdded+" modded projectiles.");
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
            On.EntityStates.Bandit2.Weapon.FireShotgun2.FireBullet += FireShotgun2_FireBullet;

            // SPEED //
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += AdjustSpeedEnemyProjectile;
        }


        private void FireShotgun2_FireBullet(On.EntityStates.Bandit2.Weapon.FireShotgun2.orig_FireBullet orig, EntityStates.Bandit2.Weapon.FireShotgun2 self, Ray aimRay)
        {
            self.StartAimMode(aimRay, 3f, false);
            self.DoFireEffects();
            self.PlayFireAnimation();
            self.AddRecoil(-1f * self.recoilAmplitudeY, -1.5f * self.recoilAmplitudeY, -1f * self.recoilAmplitudeX, 1f * self.recoilAmplitudeX);
            if (self.isAuthority)
            {
                Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                Vector3 axis = Vector3.Cross(aimRay.direction, rhs);
                float spreadBloom = self.characterBody.spreadBloomAngle;

                float angle = 0f;
                float num2 = 0f;
                if (self.bulletCount > 1)
                { //here
                    var ResultMult = CalculateSpreadMultiplier(self.characterBody.inventory, false);
                    num2 = UnityEngine.Random.Range(SimpleSpread(ResultMult, self.minFixedSpreadYaw + spreadBloom),
                        SimpleSpread(ResultMult, self.maxFixedSpreadYaw + spreadBloom)) * 2f;
                    angle = num2 / (self.bulletCount - 1);
                }
                Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
                Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                Ray aimRay2 = new Ray(aimRay.origin, direction);
                for (int i = 0; i < self.bulletCount; i++)
                {
                    BulletAttack bulletAttack = self.GenerateBulletAttack(aimRay2);
                    self.ModifyBullet(bulletAttack);
                    bulletAttack.Fire();
                    aimRay2.direction = rotation * aimRay2.direction;
                }
            }
        }

        //EntityStates.BaseNailgunState nailgunState = new EntityStates.BaseNailgunState();

        public override void Uninstall()
        {
            base.Uninstall();
            // ACCURACY //
            On.RoR2.BulletAttack.Fire -= AdjustSpreadBullets;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= AdjustSpreadProjectiles;
            On.EntityStates.Bandit2.Weapon.FireShotgun2.FireBullet -= FireShotgun2_FireBullet;

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
                                    fireProjectileInfo.speedOverride = projectileSimple.desiredForwardSpeed * (1f + speedMult);
                                    fireProjectileInfo.useSpeedOverride = true;
                                    //Debug.Log("vel " + projectileSimple.velocity + " speedoverride " + fireProjectileInfo.speedOverride);
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
            var characterBodyWithMostSpice = GetPlayerWithMostItemDef(itemDef_SpiceTally);
            float SpiceMult = 0f;
            if (characterBodyWithMostSpice)
            {
                int ItemCount_Spice = characterBodyWithMostSpice.inventory.GetItemCount(itemDef_SpiceTally);

                if (ItemCount_Spice > 0)
                {
                    if (ItemCount_Spice > 4)
                    {
                        SpiceMult = SpiceBonusesConstant[5, 3];
                    }
                    else
                    {
                        SpiceMult = SpiceBonusesConstant[ItemCount_Spice, 3];
                    }
                }
            }
            return SpiceMult;
        }

        private float CalculateSpreadMultiplier(Inventory inventory, bool isProjectile)
        {
            int ItemCount_Scope = inventory.GetItemCount(itemDef_Scope);
            int ItemCount_Spice = inventory.GetItemCount(itemDef_SpiceTally);
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
                    SpiceMult -= SpiceBonusesConstant[ItemCount_Spice, 2];
                    break;
                case 5: //fuck IT GOES FROM 0.15 to -0.2 WHYYYYYYYYYYYYYYY hardcoded stopgap time
                    SpiceMult -= 0f;
                    break;
                default:
                    SpiceMult -= SpiceBonusesConstant[5, 2] + SpiceBonusesAdditive[2] * (ItemCount_Spice - 4);
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

            if (ShowAnnoyingDebugText)
                _logger.LogMessage("Scope: [isProjectile"+ isProjectile+ "] Scope: "+ ScopeMult + " + SpiceMult: " + SpiceMult + " = " + ResultMult);
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
                            float AccMult = CalculateSpreadMultiplier(inventory, true);
                            GameObject projectilePrefab = fireProjectileInfo.projectilePrefab;
                            Vector3 aimDirection = input.aimDirection;
                            Quaternion aimDirectionQuaternion = Util.QuaternionSafeLookRotation(aimDirection);

                            _logger.LogMessage("Projectile Fired: "+ fireProjectileInfo.projectilePrefab.name);
                            bool isProjectileAllowed = WhitelistedProjectiles.Contains(projectilePrefab);

                            if ((ShotSpread_WhitelistProjectiles && isProjectileAllowed) || !ShotSpread_WhitelistProjectiles)
                            {
                                //float BaseSpreadAngle = 15;

                                //Note Clamp Acc at 1 for 100% acc or else it overflows
                                float CappedAccMult = Mathf.Min(AccMult, 1);
                                Quaternion UpdatedAngle;

                                if (AccMult >= 0)
                                {
                                    UpdatedAngle = Quaternion.Lerp(fireProjectileInfo.rotation, aimDirectionQuaternion, CappedAccMult);
                                } else
                                {
                                    //AGONY
                                    AccMult *= -1;
                                    //CappedAccMult *= -1;
                                    //This is a random dir in cone. USED FOR INACCURACY
                                    //TODO
                                    //Quaternion bulletDir = GetRandomInsideCone(BaseSpreadAngle, aimDirection);
                                    //_logger.LogMessage(bulletDir);
                                    //UpdatedAngle = Quaternion.SlerpUnclamped(bulletDir, aimDirectionQuaternion, CappedAccMult);
                                    var minSpread = Mathf.Max(0f, 1f - AccMult);
                                    UpdatedAngle = Util.QuaternionSafeLookRotation(Util.ApplySpread(fireProjectileInfo.rotation.eulerAngles, minSpread, AccMult, 1f, 1f));
                                }
                                fireProjectileInfo.rotation = UpdatedAngle;
                                //UpdatedAngle = Quaternion.LerpUnclamped(fireProjectileInfo.rotation, aimDirectionQuaternion, CappedAccMult);
                                //fireProjectileInfo.rotation = UpdatedAngle;
                            }
                        }
                    }
                }
            }
            orig(self, fireProjectileInfo);
        }

        Quaternion GetRandomInsideCone(float conicAngle, Vector3 forward)
        {
            // random tilt right (which is a random angle around the up axis)
            Quaternion randomTilt = Quaternion.AngleAxis(Random.Range(0f, conicAngle), Vector3.up);

            // random spin around the forward axis
            Quaternion randomSpin = Quaternion.AngleAxis(Random.Range(0f, 360f), forward);

            // tilt then spin
            return (randomSpin * randomTilt);
        }

        float SimpleSpread(float ResultMult, float original, float multiplier = 1f)
        {
            return original == 0 ? ResultMult * multiplier : original * ResultMult;
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

                    //if (self.radius == 0) //(maybe?)
                        //self.radius = ResultMult;

                    self.maxSpread = SimpleSpread(self.maxSpread,1.15f);

                    self.minSpread = SimpleSpread(ResultMult, self.minSpread);

                    self.spreadPitchScale *= SimpleSpread(ResultMult, self.spreadPitchScale);
                    self.spreadYawScale *= SimpleSpread(ResultMult, self.spreadYawScale);

                    //self.owner.GetComponent<CharacterBody>().SetSpreadBloom(ResultMult, false);

                }
            }
            orig(self);
        }
    }
}
