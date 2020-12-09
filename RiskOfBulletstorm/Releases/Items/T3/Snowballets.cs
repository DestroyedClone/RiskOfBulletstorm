//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
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
    public class Snowballets : Item_V2<Snowballets>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Per how many meters should Snowballets affect the projectile? (Default: 20m)", AutoConfigFlags.PreventNetMismatch)]
        public static float Snowballets_BaseMeters { get; private set; } = 20f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many meters is the requirement reduced by per item stack? (Default: 1m reduction)", AutoConfigFlags.PreventNetMismatch)]
        public static float Snowballets_StackMeters { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much should the projectile resize per meter amount? (Default: +0.1 = 10% size)", AutoConfigFlags.PreventNetMismatch)]
        public static float Snowballets_SizeMultiplier { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much damage should the projectile deal per meter amount? (Default: 0.1 = 10% damage)", AutoConfigFlags.PreventNetMismatch)]
        public static float Snowballets_DamageMultiplier { get; private set; } = 0.1f;
        public override string displayName => "Snowballets";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Powder Power\nBullets grow in size and damage as they travel.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Every {Snowballets_BaseMeters} meters <style=cStack>-{Snowballets_StackMeters} meters per stack</style>, increases projectile size by +{Pct(Snowballets_SizeMultiplier)}" +
                $" and damage by +{Pct(Snowballets_DamageMultiplier)}.";

            return desc;
        }

        protected override string GetLoreString(string langID = null) => "These bullets draw ambient bullet particles from the Gungeon's atmosphere, steadily increasing in size. Bullet Kin have been seen stacking them for fun. Arguments frequently break out over the sole scarf in the Gungeon.";

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
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ApplyComponent;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= ApplyComponent;
        }
        private void ApplyComponent(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo fireProjectileInfo)
        {
            var owner = fireProjectileInfo.owner;
            if (owner && owner.GetComponent<CharacterBody>() && owner.GetComponent<CharacterBody>().inventory)
            {
                var inventoryCount = owner.GetComponent<CharacterBody>().inventory.GetItemCount(catalogIndex);
                if (inventoryCount > 0)
                {
                    var component = fireProjectileInfo.projectilePrefab.AddComponent<Bulletstorm_SnowballetsComponent>();
                    component.meterAmount = Mathf.Min(Snowballets_BaseMeters - Snowballets_StackMeters * (inventoryCount - 1),1);
                    component.sizeMultiplier = 1 + Snowballets_SizeMultiplier;
                    component.damageMultiplier = 1 + Snowballets_DamageMultiplier;
                }
            }
            orig(self, fireProjectileInfo);
        }
        private class Bulletstorm_SnowballetsComponent : MonoBehaviour
        {
            public float meterAmount = 20f;
            public float sizeMultiplier = 1;
            public float damageMultiplier = 1;
            private Vector3 lastPosition = Vector3.zero;
            private ProjectileDamage projectileDamage;
            private ProjectileController projectileController;

            private int maxStacks = 10;
            private int currentStacks = 0;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Engine")]
            void OnEnable()
            {
                lastPosition = gameObject.transform.position;
                projectileDamage = gameObject.GetComponent<ProjectileDamage>();
                projectileController = gameObject.GetComponent<ProjectileController>();
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Engine")]
            void FixedUpdate()
            {
                if (currentStacks < maxStacks)
                {
                    var currentPosition = gameObject.transform.position;
                    var distance = Vector3.Distance(lastPosition, currentPosition);
                    Debug.Log("Snowballets: Distance " + distance);
                    if (distance >= meterAmount)
                    {
                        lastPosition = currentPosition;
                        currentStacks++;

                        if (projectileDamage) projectileDamage.damage *= 1 + damageMultiplier;
                        if (projectileController) ResizeProjectile(sizeMultiplier);
                    }
                }
            }

            private void ResizeProjectile(float scale)
            {
                gameObject.transform.localScale *= scale;
                projectileController.transform.localScale *= scale;
                projectileController.ghostPrefab.transform.localScale *= scale;
            }
        }
    }
}
