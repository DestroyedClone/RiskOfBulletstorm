using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class BannedFromBazaarTally : ItemBase<BannedFromBazaarTally>
    {
        public override string ItemName => "BannedFromBazaarTally";

        public override string ItemLangTokenName => "BANNEDFROMBAZAARTALLY";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override bool Hidden => true;

        public override ItemTag[] ItemTags => PlayerOnlyItemTags;

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