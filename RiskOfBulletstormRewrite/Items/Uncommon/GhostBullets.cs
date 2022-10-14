using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class GhostBullets : ItemBase<GhostBullets>
    {
        public override string ItemName => "Ghost Bullets";

        public override string ItemLangTokenName => "GHOSTBULLETS";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => Assets.NullModel;

        public override string[] ItemFullDescriptionParams => new string[]
        {
        };


        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.Any,
            ItemTag.Utility
        };

        public override void Init(ConfigFile config)
        {
            return;
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }
        public static ConfigEntry<float> cfgDamageReduction;
        public override void CreateConfig(ConfigFile config)
        {
            cfgDamageReduction = config.Bind(ConfigCategory, "Damage Reduction", 0.5f);
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            //On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ApplyGhost;
            On.RoR2.UI.MainMenu.MainMenuController.Start += TempHook;
            //On.RoR2.BulletAttack.Fire += RefireBullet;
        }

        public void RefireBullet(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            if (self.owner)
            {
                var itemCount = GetCount(self.owner.GetComponent<CharacterBody>());
                if (itemCount > 0)
                {

                }
            }
            orig(self);
        }

        public void TempHook(On.RoR2.UI.MainMenu.MainMenuController.orig_Start orig, RoR2.UI.MainMenu.MainMenuController self)
        {
            orig(self);
            foreach (var projectile in ProjectileCatalog.projectilePrefabs)
            {
                if (projectile.GetComponent<ProjectileController>() && projectile.GetComponent<ProjectileDamage>())
                {
                    projectile.AddComponent<RBS_GhostBulletBehaviour>();
                }
            }
            On.RoR2.UI.MainMenu.MainMenuController.Start -= TempHook;
        }

        public void ApplyGhost(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, RoR2.Projectile.ProjectileManager self,  RoR2.Projectile.FireProjectileInfo info)
        {
            if (info.owner)
            {
                var comp = info.projectilePrefab.GetComponent<RBS_GhostBulletBehaviour>();
                if (!comp)
                {
                    var cb = info.owner.GetComponent<CharacterBody>();
                    if (cb && cb.inventory)
                    {
                        var itemCount = cb.inventory.GetItemCount(ItemDef);
                        if (itemCount > 0)
                        {
                            //info.projectilePrefab
                        }
                    }
                } else {
                    
                }
            }
            orig(self, info);
        }

        public class RBS_GhostBulletBehaviour : MonoBehaviour, IProjectileImpactBehavior
        {
            public CharacterBody owner;
            public ProjectileController projectileController;
            public ProjectileDamage projectileDamage;
            public ProjectileTargetComponent projectileTargetComponent;

            public int remainingShots = 0;
            bool hasRun = false;

            public void Start()
            {
                projectileController = gameObject.GetComponent<ProjectileController>();
                projectileDamage = gameObject.GetComponent<ProjectileDamage>();
                if (!projectileController || !projectileDamage)
                {
                    enabled = false;
                    return;
                }
                
                projectileTargetComponent = gameObject.GetComponent<ProjectileTargetComponent>();
                GetPasses();
            }

            public void GetPasses()
            {
                if (projectileController && projectileController.owner)
                    remainingShots = GhostBullets.instance.GetCount(projectileController.owner.GetComponent<CharacterBody>());
            }

            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                if (hasRun) return;
                hasRun = true;
                if (impactInfo.collider)
                {
                    var hurtBox = impactInfo.collider.gameObject.GetComponent<HurtBox>();
                    if (hurtBox && hurtBox.healthComponent)
                    {
                        if (remainingShots > 0)
                        {
                            Transform target = null;
                            if (projectileTargetComponent)
                            {
                                target = projectileTargetComponent.target;
                            }

                            remainingShots--;
                            Vector3 offset = Vector3.zero;
                            {
                                var scale = transform.localScale;
                                var projectileSize = Mathf.Max(scale.x,scale.y,scale.z);
                                var rot = transform.forward;
                                offset = projectileSize * rot * 1.1f;
                            }

                            ProjectileManager.instance.FireProjectile(gameObject,
                            transform.position + offset, transform.rotation,
                            projectileController.owner,
                            projectileDamage.damage * cfgDamageReduction.Value,
                            projectileDamage.force * cfgDamageReduction.Value,
                            projectileDamage.crit,
                            projectileDamage.damageColorIndex,
                            target ? target.gameObject : null);
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }
}
