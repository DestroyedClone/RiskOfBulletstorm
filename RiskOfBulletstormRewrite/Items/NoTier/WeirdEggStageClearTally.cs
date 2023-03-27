using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class WeirdEggStageClearTally : ItemBase<WeirdEggStageClearTally>
    {
        public override string ItemName => "WeirdEggStageClearTally";

        public override string ItemLangTokenName => "WeirdEggStageClearTally";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override string ParentEquipmentName => "WeirdEgg";

        public override bool Hidden => true;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
        };

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
        }
    }
}