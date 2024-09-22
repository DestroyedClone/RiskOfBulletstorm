using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class StolenItemTally : ItemBase<StolenItemTally>
    {
        public override string ItemName => "StolenItemTally";

        public override string ItemLangTokenName => "STOLENITEMTALLY";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Modules.Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override bool Hidden => true;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
        };

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateItem();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }
    }
}