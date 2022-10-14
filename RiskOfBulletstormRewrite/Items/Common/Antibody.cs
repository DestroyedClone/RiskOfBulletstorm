using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class Antibody : ItemBase<Antibody>
    {
        public static ConfigEntry<float> cfgChance;
        public static ConfigEntry<float> cfgMultiplier;
        public static ConfigEntry<float> cfgMultiplierPerStack;

        public override string ItemName => "Antibody";

        public override string ItemLangTokenName => "ANTIBODY";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            cfgChance.Value.ToString(),
            GetChance(cfgMultiplier),
            GetChance(cfgMultiplierPerStack)
        };

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

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
            cfgChance = config.Bind(ConfigCategory, "Heal Chance", 25f, "What is the chance in percent of the item activating?");
            cfgMultiplier = config.Bind(ConfigCategory, "Multiplier", 0.33f, "What is the amount of healing increased by?");
            cfgMultiplierPerStack = config.Bind(ConfigCategory, "Multiplier per stack", 0.11f, "What is the amount of healing increased by per stack?");
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
            //_logger.LogMessage($"Heal: regen: {nonRegen}");
            if (nonRegen)
            {
                if (GetCount(self.body) > 0)
                {
                    //timothy: 0.25f chance = 0.0025% chance. 25f = 25% chance. got it?
                    if (Util.CheckRoll(cfgChance.Value, self.body.master))
                    {
                        var itemCount = self.body.inventory.GetItemCount(ItemDef);
                        var multiplier = 1f + cfgMultiplier.Value + cfgMultiplierPerStack.Value * (itemCount - 1);
                        //var oldHeal = amount;
                        amount *= multiplier;
                        //_logger.LogMessage($"roll won: {oldHeal} -> {amount}");
                    }
                    else
                    {
                        //_logger.LogMessage("roll failed");
                    }
                }
            }
            return orig(self, amount, procChainMask, nonRegen);
        }
    }
}