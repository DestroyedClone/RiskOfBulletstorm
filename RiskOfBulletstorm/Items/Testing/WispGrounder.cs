//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using TILER2;
using System;
using static RiskOfBulletstorm.Shared.HelperUtil;
using RoR2.Projectile;
using UnityEngine;

namespace RiskOfBulletstorm.Items
{
    public class GungeonArtifact : Item_V2<GungeonArtifact>
    {
        public override string displayName => "Projectile Artifact";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Forces enemies to shoot projectiles instead of bullets";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public static GameObject GungeonBulletPrefab { get; private set; }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            GameObject RoboBallProjectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/RoboBallProjectile");
            GungeonBulletPrefab = RoboBallProjectilePrefab.InstantiateClone("GungeonBullet");
            //GungeonBulletPrefab.GetComponent<ProjectileSimple>().velocity = 1; //default 50
            //UnityEngine.Object.Destroy(GungeonBulletPrefab.GetComponent<ApplyTorqueOnStart>());
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            //On.EntityStates.Wisp1Monster.FireEmbers.OnEnter += FireEmbers_OnEnter;
            //On.RoR2.BulletAttack.Fire += BulletAttack_Fire;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            //On.EntityStates.Wisp1Monster.FireEmbers.OnEnter -= FireEmbers_OnEnter;
            //On.RoR2.BulletAttack.Fire -= BulletAttack_Fire;
        }

        private void BulletAttack_Fire(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            var body = self.owner.GetComponent<CharacterBody>();

            if (body)
            {
                if (body.baseNameToken == "WISP_BODY_NAME")
                {
                    var health = body.GetComponent<HealthComponent>();
                    if (health)
                    {
                        ProjectileImpactExplosion projectileImpactExplosion = GungeonBulletPrefab.GetComponent<ProjectileImpactExplosion>();
                        ProjectileDamage projectileDamage = GungeonBulletPrefab.GetComponent<ProjectileDamage>();
                        ProjectileSimple projectileSimple = GungeonBulletPrefab.GetComponent<ProjectileSimple>();

                        GungeonBulletPrefab.GetComponent<ProjectileController>().owner = self.owner;
                        //weapon
                        //origin
                        //aimVector
                        //minSpread
                        //maxSpread
                        projectileDamage.damage = self.damage * 3; //*3 because it normally fires 3 bullets
                        projectileDamage.force = self.force;
                        //tracereffectprefab self.tracerEffectPrefab
                        //muzzlename
                        projectileImpactExplosion.impactEffect = self.hitEffectPrefab;
                        projectileDamage.crit = self.isCrit;
                        //falloffmodel
                        //hiteffectnormal
                        //radius
                        //proc chance
                        //
                        projectileImpactExplosion.blastRadius = 1;
                        projectileSimple.velocity = 100;

                        //https://stackoverflow.com/questions/36781086/how-to-convert-vector3-to-quaternion
                        //var V3 = self.aimVector;
                        //Quaternion quaternion = Quaternion.Euler(V3.x, V3.y, V3.z);
                        Quaternion quaternion = Util.QuaternionSafeLookRotation(self.aimVector);

                        self.damage = 0;
                        self.procCoefficient = 0;
                        self.bulletCount = 0;
                        self.hitEffectPrefab = null;
                        self.tracerEffectPrefab = null;

                        //ProjectileManager.instance.FireProjectile();
                        ProjectileManager.instance.FireProjectile(GungeonBulletPrefab, self.origin, quaternion, self.owner, projectileDamage.damage, projectileDamage.force, projectileDamage.crit);
                    }
                }
            }
            orig(self);
        }
    }
}
