﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;
using static RiskOfBulletstorm.Utils.HelperUtil;
using RoR2.Projectile;

namespace RiskOfBulletstorm.Items
{
    public class BulletstormExtraStatsController : Item_V2<BulletstormExtraStatsController>
    {
        public override string displayName => "BulletstormExtraStatsController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        private static readonly GameObject DisposableMissileLauncherPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MissileProjectile");

        private float Scope_SpreadReduction;
        private float Scope_SpreadReductionStack;
        private bool Scope_EnableDML;
        private bool Scope_WhitelistProjectiles;
        private ItemIndex ItemIndex_Scope;
        private ItemIndex ItemIndex_SpiceTally;

        private float[,] SpiceBonuses;
        private float[] SpiceBonusesAdditive;
        private float[] SpiceBonusesConstantMaxed;

        public static List<GameObject> WhitelistedProjectiles = new List<GameObject>
        {
            Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang"), //Saw
            Resources.Load<GameObject>("prefabs/projectiles/BeetleQueenSpit"), // Beetle Spit
            Resources.Load<GameObject>("prefabs/projectiles/LunarShardProjectile"),
            Resources.Load<GameObject>("Prefabs/Projectiles/CaptainTazer"),
            Resources.Load<GameObject>("Prefabs/Projectiles/Tarball"),
            EntityStates.ClayBoss.ClayBossWeapon.FireBombardment.projectilePrefab, //
            Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile"),
            Resources.Load<GameObject>("Prefabs/Projectiles/EngiHarpoon"),
            Resources.Load<GameObject>("Prefabs/Projectiles/LunarNeedleProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/GravekeeperHookProjectile"), //not used?
            Resources.Load<GameObject>("prefabs/projectiles/GravekeeperHookProjectileSimple"), //????
            Resources.Load<GameObject>("GravekeeperTrackingFireball"),
            Resources.Load<GameObject>("prefabs/projectiles/ImpVoidspikeProjectile"),
            EntityStates.ImpMonster.FireSpines.projectilePrefab, 
            Resources.Load<GameObject>("prefabs/projectiles/LemurianBigFireball"), 
            Resources.Load<GameObject>("prefabs/projectiles/NullifierBombProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/RoboBallProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SuperRoboBallProjectile"), //idk??
            Resources.Load<GameObject>("prefabs/projectiles/ScavEnergyCannonProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SyringeProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SyringeProjectileHealing"),
            Resources.Load<GameObject>("prefabs/projectiles/UrchinSeekingProjectile"),
            EntityStates.VagrantMonster.Weapon.JellyBarrage.projectilePrefab,
            Resources.Load<GameObject>("prefabs/projectiles/WindbladeProjectile"),

            // Toggleables
            DisposableMissileLauncherPrefab,
        };


        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            if (!Scope_EnableDML)
                WhitelistedProjectiles.Remove(DisposableMissileLauncherPrefab);

        }
        public override void SetupLate()
        {
            base.SetupLate();
            // SCOPE //
            Scope_SpreadReduction = Scope.Scope_SpreadReduction;
            Scope_SpreadReductionStack = Scope.Scope_SpreadReductionStack;
            Scope_EnableDML = Scope.Scope_EnableDML;
            Scope_WhitelistProjectiles = Scope.Scope_WhitelistProjectiles;

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
            //Debug.Log("Adding modded projectiles");
            foreach (string projectileString in moddedProjectileStrings)
            {
                var projectileIndex = ProjectileCatalog.FindProjectileIndex(projectileString);
                //failures to find defaults to -1
                if (projectileIndex > 0) WhitelistedProjectiles.Add(ProjectileCatalog.GetProjectilePrefab(projectileIndex));
            }
        }

        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            // ACCURACY //
            On.RoR2.BulletAttack.Fire += AdjustSpreadBullets;
            On.EntityStates.BaseNailgunState.FireBullet += AdjustSpreadBullets_Nailgun;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += AdjustSpreadProjectiles;

            // SPEED //
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += AdjustSpeedEnemyProjectile;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            // ACCURACY //
            On.RoR2.BulletAttack.Fire -= AdjustSpreadBullets;
            On.EntityStates.BaseNailgunState.FireBullet += AdjustSpreadBullets_Nailgun;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= AdjustSpreadProjectiles;

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
            if (ItemCount_Spice > 0)
            {
                if (ItemCount_Spice > 2 && ItemCount_Spice <= 4)
                {
                    SpiceMult -= SpiceBonuses[ItemCount_Spice, 2];
                    // -0.15 + 
                }
                else if (ItemCount_Spice > 4)
                {
                    SpiceMult -= SpiceBonusesConstantMaxed[2] + SpiceBonusesAdditive[2] + SpiceBonusesAdditive[2] * (ItemCount_Spice - 4);
                    // +0.15 (+) -0.10 (+) -0.10*additionalstacks
                }
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


                            bool isProjectileAllowed = WhitelistedProjectiles.Contains(projectilePrefab);

                            if ((Scope_WhitelistProjectiles && isProjectileAllowed) || !Scope_WhitelistProjectiles)
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

                                }
                            }
                        }
                    }
                }
            }
            orig(self, fireProjectileInfo);
        }

        private void AdjustSpreadBullets_Nailgun(On.EntityStates.BaseNailgunState.orig_FireBullet orig, EntityStates.BaseNailgunState self, Ray aimRay, int bulletCount, float spreadPitchScale, float spreadYawScale)
        {
            //MULT you upset me
            var characterBody = self.characterBody;
            //var updateBloom = false;
            //float ResultMult = 1f;
            if (characterBody)
            {
                var inventory = characterBody.inventory;
                if (inventory)
                {
                    var ResultMult = CalculateSpreadMultiplier(inventory, false);
                    characterBody.SetSpreadBloom(Mathf.Min(0, characterBody.spreadBloomAngle * ResultMult), false);
                    //updateBloom = true;

                    spreadPitchScale = Mathf.Min(0, spreadPitchScale * ResultMult);
                    spreadYawScale = Mathf.Min(0, spreadYawScale * ResultMult);

                }
            }
            orig(self, aimRay, bulletCount, spreadPitchScale, spreadYawScale);
            //if (updateBloom)
                //characterBody.SetSpreadBloom(Mathf.Min(0, characterBody.spreadBloomAngle * ResultMult), false);
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

                    Debug.Log("Bulletscope: maxspread: "+self.maxSpread+" multiplier: "+ResultMult+" result: "+ Mathf.Max(self.maxSpread * ResultMult, 0));
                    self.maxSpread = self.maxSpread * ResultMult;

                    self.minSpread = self.minSpread * ResultMult;

                    self.spreadPitchScale = self.spreadPitchScale * ResultMult;
                    self.spreadYawScale = self.spreadYawScale * ResultMult;

                    //self.owner.GetComponent<CharacterBody>().SetSpreadBloom(ResultMult, false);

                }
            }
            orig(self);
        }
    }
}
