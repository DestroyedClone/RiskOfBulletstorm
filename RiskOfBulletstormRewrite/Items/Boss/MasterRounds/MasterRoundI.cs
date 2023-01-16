using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using static RiskOfBulletstormRewrite.Utils.ItemHelpers;
using static RiskOfBulletstormRewrite.Controllers.MasterRoundController;

namespace RiskOfBulletstormRewrite.Items
{
    public class MasterRoundI : ItemBase<MasterRoundI>
    {
        public override string ItemName => "Master Round I";

        public override string ItemLangTokenName => "MASTERROUNDI";

        public override ItemTier Tier => ItemTier.Boss;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Healing,
            ItemTag.WorldUnique
        };

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }
        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }
    }
}