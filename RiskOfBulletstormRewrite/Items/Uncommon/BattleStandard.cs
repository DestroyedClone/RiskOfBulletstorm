using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class BattleStandard : ItemBase<BattleStandard>
    {
        public static ConfigEntry<float> cfgDamage;

        public override string ItemName => "Battle Standard";

        public override string ItemLangTokenName => "BATTLESTANDARD";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgDamage)
        };

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();

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
            cfgDamage = config.Bind(ConfigCategory, "Damage Percentage", 0.1f, "");
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
                    args.damageMultAdd += count * cfgDamage.Value;
                }
            }
        }
    }
}