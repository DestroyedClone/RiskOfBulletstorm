using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class CurseTally : ItemBase<CurseTally>
    {
        public override string ItemName => "CurseTally";

        public override string ItemLangTokenName => "CURSETALLY";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Modules.Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite(); //sprite is fine, its gonna show up in inv

        public override ItemTag[] ItemTags => PlayerOnlyItemTags;

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void Hooks()
        {
            base.Hooks();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }
    }
}