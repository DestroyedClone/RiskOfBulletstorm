using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class HeavyBullets : ItemBase<HeavyBullets>
    {
        public static ConfigEntry<float> cfgBaseDamageMultiplier;
        public static ConfigEntry<float> cfgStackDamageMultiplier;
        public static ConfigEntry<float> cfgProjectileSpeedReduction;

        public override string ItemName => "Heavy Bullets";

        public override string ItemLangTokenName => "BULLETHEAVY";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgBaseDamageMultiplier),
            GetChance(cfgStackDamageMultiplier),
            GetChance(cfgProjectileSpeedReduction)
        };

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => RoR2Content.Items.BossDamageBonus.pickupModelPrefab;

        public override Sprite ItemIcon => RoR2Content.Items.BossDamageBonus.pickupIconSprite;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgBaseDamageMultiplier = config.Bind(ConfigCategory, "Damage Multiplier", 0.25f, "");
            cfgStackDamageMultiplier = config.Bind(ConfigCategory, "Damage Multiplier Per Stack", 0.25f, "");
            cfgProjectileSpeedReduction = config.Bind(ConfigCategory, "Projectile Speed Reduction Multiplier", 0.25f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileManager_FireProjectile_FireProjectileInfo;
        }

        private void ProjectileManager_FireProjectile_FireProjectileInfo(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, RoR2.Projectile.ProjectileManager self, RoR2.Projectile.FireProjectileInfo fireProjectileInfo)
        {
            var projectileController = fireProjectileInfo.projectilePrefab.GetComponentInChildren<RoR2.Projectile.ProjectileController>();
            Vector3 oldProjectileSize = projectileController.transform.localScale;
            Vector3 oldGhostSize = projectileController.ghost.transform.localScale;
            if (fireProjectileInfo.owner && fireProjectileInfo.owner.GetComponent<CharacterBody>())
            {
                var itemCount = GetCount(fireProjectileInfo.owner.GetComponent<CharacterBody>());
                if (itemCount > 0)
                {
                    if (!fireProjectileInfo.useSpeedOverride)
                    {
                        fireProjectileInfo.speedOverride = fireProjectileInfo.projectilePrefab.GetComponent<RoR2.Projectile.ProjectileSimple>().desiredForwardSpeed;
                    }
                    var oldSpeed = fireProjectileInfo.speedOverride;
                    var fraction = 1f - cfgProjectileSpeedReduction.Value;
                    fireProjectileInfo.speedOverride *= Mathf.Pow(fraction, itemCount);
                    Chat.AddMessage($"Speed changed from {oldSpeed} to {fireProjectileInfo.speedOverride} ({(oldSpeed - fireProjectileInfo.speedOverride)} difference)");
                }
            }

            orig(self, fireProjectileInfo);
            projectileController.transform.localScale = oldProjectileSize;
            projectileController.ghost.transform.localScale = oldGhostSize;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var itemCount = GetCount(sender);
            if (itemCount > 0)
            {
                args.damageMultAdd += cfgBaseDamageMultiplier.Value + cfgStackDamageMultiplier.Value * (itemCount - 1);
            }
        }
    }
}
