using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class BattleStandard : ItemBase<BattleStandard>
    {
        public override string ItemName => "Battle Standard";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgCompanionDamageMultiplier),
            GetChance(cfgCompanionDamageMultiplierPerStack)
        };

        public override string ItemLangTokenName => "BATTLESTANDARD";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

        public static ConfigEntry<float> cfgCompanionDamageMultiplier;
        public static ConfigEntry<float> cfgCompanionDamageMultiplierPerStack;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.Damage,
        };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgCompanionDamageMultiplier = config.Bind(ConfigCategory, "Companion Damage Multiplier", 0.05f, "");
            cfgCompanionDamageMultiplierPerStack = config.Bind(ConfigCategory, "Companion Damage Multiplier Per Stack", 0.02f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.master?.minionOwnership?.ownerMaster)
            {
                var count = GetCount(sender.master.minionOwnership.ownerMaster);
                if (count > 0)
                {
                    args.damageMultAdd += cfgCompanionDamageMultiplier.Value + cfgCompanionDamageMultiplierPerStack.Value * (count - 1);
                }
            }
        }
    }
}
