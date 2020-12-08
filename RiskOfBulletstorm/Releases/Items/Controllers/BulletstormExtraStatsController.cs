using System.Collections.Generic;
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
        public override string displayName => "BulletstormAccuracyController";
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
                // MINER - DIRESEEKER //
                "DireseekerFireball",
                "DireseekerGroundFireball",

                // AETHERIUM //
                "JarOfReshapingProjectile",
                "SplinteringProjectile",
            };

            // VANILLA //
            //game doesnt like this RIP
            /*foreach (string projectileString in WhitelistedProjectilesString)
            {
                var projectileIndex = ProjectileCatalog.FindProjectileIndex("Prefabs/Projectiles/"+projectileString);
                if (projectileIndex > 0) WhitelistedProjectiles.Add(ProjectileCatalog.GetProjectilePrefab(projectileIndex));
            }*/
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

        private float CalculateSpreadMultiplier(Inventory inventory)
        {
            int ItemCount_Scope = inventory.GetItemCount(ItemIndex_Scope);
            int ItemCount_Spice = inventory.GetItemCount(ItemIndex_SpiceTally);
            float SpiceMult = 0f;
            float ScopeMult = 0f;

            if (ItemCount_Scope > 0)
                ScopeMult = (Scope_SpreadReduction + Scope_SpreadReductionStack * (ItemCount_Scope - 1));


            if (ItemCount_Spice > 0)
            {
                if (ItemCount_Spice > 2 && ItemCount_Spice <= 4)
                {
                    SpiceMult = SpiceBonuses[ItemCount_Spice, 2];
                    // -0.15 + 
                }
                else if (ItemCount_Spice > 4)
                {
                    SpiceMult = SpiceBonusesConstantMaxed[2] + SpiceBonusesAdditive[2] + SpiceBonusesAdditive[2] * (ItemCount_Spice - 4);
                    // +0.15 (+) -0.10 (+) -0.10*additionalstacks
                }
            }
            var ResultMult = Mathf.Min(ScopeMult + SpiceMult,0);
            //Debug.Log("ScopeMult: " + ScopeMult + " + SpiceMUlt: " + SpiceMult + " = " + ResultMult);

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
                            float ResultMult = CalculateSpreadMultiplier(inventory);
                            GameObject projectilePrefab = fireProjectileInfo.projectilePrefab;
                            Quaternion aimDir = Util.QuaternionSafeLookRotation(input.aimDirection);
                            Quaternion rotation = fireProjectileInfo.rotation;

                            Quaternion UpdatedAngle = Quaternion.Lerp(rotation, aimDir, ResultMult);

                            if (Scope_WhitelistProjectiles)
                            {
                                if (WhitelistedProjectiles.Contains(projectilePrefab))
                                {
                                    Debug.Log("Projectile Fired: "+ projectilePrefab.name);
                                    fireProjectileInfo.rotation = UpdatedAngle;
                                }
                                //Chat.AddMessage("Scope Lerp: " + aimDir + " and " + rotation + " resulting " + UpdatedAngle);
                            }
                            else
                            {
                                fireProjectileInfo.rotation = UpdatedAngle;
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
            var updateBloom = false;
            float ResultMult = -1f;
            if (characterBody)
            {
                var inventory = characterBody.inventory;
                if (inventory)
                {
                    int InventoryCount = characterBody.inventory.GetItemCount(catalogIndex);
                    if (InventoryCount > 0)
                    {
                        ResultMult = CalculateSpreadMultiplier(inventory);
                        updateBloom = true;

                        spreadPitchScale = Mathf.Min(0, spreadPitchScale * ResultMult);
                        spreadYawScale = Mathf.Min(0, spreadYawScale * ResultMult);
                    }
                }
            }
            orig(self, aimRay, bulletCount, spreadPitchScale, spreadYawScale);
            if (updateBloom)
                characterBody.SetSpreadBloom(Mathf.Min(0, characterBody.spreadBloomAngle * ResultMult), false);
        }

        private void AdjustSpreadBullets(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            //doesn't work on MULT?????
            CharacterBody characterBody = self.owner.GetComponent<CharacterBody>();
            if (characterBody)
            {
                var inventory = characterBody.inventory;
                if (inventory)
                {
                    int InventoryCount = characterBody.inventory.GetItemCount(catalogIndex);
                    if (InventoryCount > 0)
                    {
                        float ResultMult = CalculateSpreadMultiplier(inventory);

                        characterBody.SetSpreadBloom(Mathf.Min(0, characterBody.spreadBloomAngle * ResultMult), false);

                        self.maxSpread = Mathf.Max(self.maxSpread * ResultMult, 0);

                        self.minSpread = Mathf.Min(0, self.minSpread * ResultMult);

                        self.spreadPitchScale = Mathf.Min(0, self.spreadPitchScale * ResultMult);
                        self.spreadYawScale = Mathf.Min(0, self.spreadYawScale * ResultMult);

                        //self.owner.GetComponent<CharacterBody>().SetSpreadBloom(ResultMult, false);
                    }
                }
            }
            orig(self);
        }
    }
}
