using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class ExampleItem : ItemBase<ExampleItem>
    {
        public static ConfigEntry<float> cfgChance;
        public static ConfigEntry<float> cfgMultiplier;
        public static ConfigEntry<float> cfgMultiplierPerStack;

        public override string ItemName => "Antibody";

        public override string ItemLangTokenName => "ANTIBODY";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgChance),
            GetChance(cfgMultiplier),
            GetChance(cfgMultiplierPerStack)
        };

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("ExampleItemPrefab.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("ExampleItemIcon.png");

        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Any, ItemTag.Healing };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgChance = config.Bind(ConfigCategory, "Heal Chance", 0.25f, "What is the chance in percent of the item activating?");
            cfgMultiplier = config.Bind(ConfigCategory, "Multiplier", 0.33f, "What is the amount of healing increased by?");
            cfgMultiplierPerStack = config.Bind(ConfigCategory, "Multiplier per stack", 0.07f, "What is the amount of healing increased by per stack?");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
        }

        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            if (nonRegen)
            {
                if (self.body?.inventory?.GetItemCount(ItemDef) > 0)
                {
                    if (Util.CheckRoll(cfgChance.Value, self.body.master))
                    {
                        var itemCount = self.body.inventory.GetItemCount(ItemDef);
                        var multiplier = 1f + cfgMultiplier.Value + cfgMultiplierPerStack.Value * (itemCount - 1);
                        var oldHeal = amount;
                        amount *= multiplier;
                        _logger.LogMessage($"roll won: {oldHeal} -> {amount}");
                    } else
                    {
                        _logger.LogMessage("roll failed");
                    }
                }
            }
            return orig(self, amount, procChainMask, nonRegen);
        }
    }
}
