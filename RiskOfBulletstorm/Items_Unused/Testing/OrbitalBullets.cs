
using System.Collections.Generic;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;

namespace RiskOfBulletstorm.Items
{
    public class OrbitalBullets : Item_V2<OrbitalBullets>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much projectiles can orbit the player?", AutoConfigFlags.PreventNetMismatch)]
        public int OrbitalBullets_OrbitMax { get; private set; } = 5;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional projectiles can orbit the player per item stack?", AutoConfigFlags.PreventNetMismatch)]
        public int OrbitalBullets_OrbitMaxStack { get; private set; } = 1;
        public override string displayName => "Orbital Bullets";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Deadly Revolution\nMissed shots orbit.";

        protected override string GetDescString(string langid = null) => $"Non-parent projectiles";

        protected override string GetLoreString(string langID = null) => "Development of these bullets began immediately after the field tests of the Mr. Accretion Jr.";

        public override void SetupBehavior()
        {
            base.SetupBehavior();
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
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileManager_FireProjectile_FireProjectileInfo;
        }

        private void ProjectileManager_FireProjectile_FireProjectileInfo(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo fireProjectileInfo)
        {
            var owner = fireProjectileInfo.owner;
            if (owner)
            {
                var characterBody = owner.GetComponent<CharacterBody>();
                if (characterBody)
                {
                    var inventory = characterBody.inventory;
                    if (inventory && inventory.GetItemCount(catalogIndex) > 0)
                    {
                        var component = self.gameObject.GetComponent<OrbitalBulletsComponent>();
                        if (!component) component = self.gameObject.AddComponent<OrbitalBulletsComponent>();
                        component.enabled = true;
                        component.target = owner;
                        component.projectileImpactExplosion = self.gameObject.GetComponent<ProjectileImpactExplosion>();
                        component.projectileSimple = self.gameObject.GetComponent<ProjectileSimple>();
                        component.radius = owner.GetComponent<CharacterMotor>().capsuleRadius + 3f;
                    }
                }
            }
            orig(self, fireProjectileInfo);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= ProjectileManager_FireProjectile_FireProjectileInfo;
        }

        public class OrbitalBulletsComponent : NetworkBehaviour, IProjectileImpactBehavior
        {
            public ProjectileImpactExplosion projectileImpactExplosion;
            public ProjectileSimple projectileSimple;
            public GameObject target;
            public float radius = 6f;
            private bool activated = false;

            public void OnEnable()
            {
                if (projectileImpactExplosion)
                {
                    projectileImpactExplosion.destroyOnWorld = false;
                    projectileImpactExplosion.timerAfterImpact = false;
                }
            }

            public void Update()
            {
                if (activated)
                    transform.RotateAround(target.transform.position, Vector3.up, 20 * Time.deltaTime);
            }

            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                if (!enabled) return;
                    HurtBox component = impactInfo.collider.GetComponent<HurtBox>();
                if (!component)
                {
                    Activate();
                    projectileImpactExplosion.CancelInvoke("OnProjectileImpact");
                }
            }

            public void Activate()
            {
                activated = true;
                gameObject.transform.position = target.transform.position + Vector3.forward * radius;
                projectileSimple.stopwatch /= 2;
            }
        }
    }
}
