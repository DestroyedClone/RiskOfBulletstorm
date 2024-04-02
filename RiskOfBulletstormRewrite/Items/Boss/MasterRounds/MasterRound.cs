using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class MasterRound : ItemBase<MasterRound>
    {
        public override string ItemName => "Master Round";

        public override string ItemLangTokenName => "MASTERROUND";

        public override ItemTier Tier => ItemTier.Boss;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public static float healthMultiplierAdditive = 0.5f;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Any,
            ItemTag.AIBlacklist,
            ItemTag.BrotherBlacklist,
            ItemTag.CannotCopy,
            ItemTag.CannotSteal,
            ItemTag.CannotDuplicate,
            ItemTag.WorldUnique
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
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var itemCount = GetCount(sender);
            if (itemCount > 0)
            {
                args.healthMultAdd += healthMultiplierAdditive * itemCount;
            }
        }
    }
}