//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using static R2API.DirectorAPI;
using static RiskOfBulletstorm.Utils.HelperUtil;
using RiskOfBulletstorm.Utils;
using RoR2.Projectile;
using RiskOfBulletstorm.Items;

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

        //private static readonly GameObject REXPrefab = EntityStates.Treebot.Weapon.FireSyringe.projectilePrefab;
        //private static readonly GameObject SawPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang");
        private static readonly GameObject DisposableMissileLauncherPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/MissileProjectile");
        //private static readonly GameObject BeetleSpitPrefab = Resources.Load<GameObject>("prefabs/projectiles/BeetleQueenSpit.prefab");

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
            EntityStates.BeetleQueenMonster.FireSpit.projectilePrefab, // Beetle Spit
            EntityStates.BrotherMonster.Weapon.FireLunarShards.projectilePrefab,
            EntityStates.Captain.Weapon.FireTazer.projectilePrefab,
            EntityStates.ClayBoss.FireTarball.projectilePrefab,
            EntityStates.ClayBoss.ClayBossWeapon.FireBombardment.projectilePrefab,
            Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile"),
            EntityStates.Engi.EngiMissilePainter.Fire.projectilePrefab,
            EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.projectilePrefab,
            EntityStates.GravekeeperBoss.FireHook.projectilePrefab,
            EntityStates.GravekeeperMonster.Weapon.GravekeeperBarrage.projectilePrefab,
            EntityStates.ImpBossMonster.FireVoidspikes.projectilePrefab,
            EntityStates.ImpMonster.FireSpines.projectilePrefab,
            EntityStates.LemurianBruiserMonster.FireMegaFireball.projectilePrefab,
            EntityStates.NullifierMonster.FirePortalBomb.portalBombProjectileEffect,
            Resources.Load<GameObject>("prefabs/projectiles/RoboBallProjectile"),
            Resources.Load<GameObject>("prefabs/projectiles/SuperRoboBallProjectile"), //idk??
            EntityStates.ScavMonster.FireEnergyCannon.projectilePrefab,
            EntityStates.Treebot.Weapon.FireSyringe.projectilePrefab, //REX
            EntityStates.UrchinTurret.Weapon.MinigunFire.projectilePrefab,
            EntityStates.VagrantMonster.Weapon.JellyBarrage.projectilePrefab,
            EntityStates.Vulture.Weapon.FireWindblade.projectilePrefab,

            // Toggleables
            DisposableMissileLauncherPrefab,
        };

        public static List<string> WhitelistedProjectilesString = new List<string>
        {
            "Sawmerang",
            "BeetleQueenSpit",
            "LunarShardProjectile",
            "LunarNeedleProjectile",
            //EntityStates.Captain.Weapon.FireTazer.projectilePrefab.name,
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

            /*foreach (string projectileString in WhitelistedProjectilesString)
            {
                var projectileIndex = ProjectileCatalog.FindProjectileIndex("Prefabs/Projectiles/"+projectileString);
                if (projectileIndex > 0) WhitelistedProjectiles.Add(ProjectileCatalog.GetProjectilePrefab(projectileIndex));
            }*/

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
            foreach (string projectileString in moddedProjectileStrings)
            {
                var projectileIndex = ProjectileCatalog.FindProjectileIndex(projectileString);
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
            On.RoR2.BulletAttack.Fire += AdjustSpreadBullets;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += AdjustSpreadProjectiles;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += AdjustSpeedEnemyProjectile;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.BulletAttack.Fire -= AdjustSpreadBullets;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= AdjustSpreadProjectiles;
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
            var ResultMult = Mathf.Clamp(ScopeMult + SpiceMult,0,1);
            Debug.Log("ScopeMult: " + ScopeMult + " + SpiceMUlt: " + SpiceMult + " = " + ResultMult);

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

                        characterBody.SetSpreadBloom(Mathf.Min(0, characterBody.spreadBloomAngle * ResultMult), false); //should affect MULT

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
