using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class PrimePrimer : ItemBase<PrimePrimer>
    {
        public override string ItemName => "Prime Primer";

        public override string ItemLangTokenName => "PRIMEPRIMER";

        public override ItemTier Tier => ItemTier.Boss;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();
        public override string ParentItemName => "Past Killing Bullet";

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.WorldUnique,
            ItemTag.CannotSteal,
            ItemTag.CannotCopy,
            ItemTag.CannotDuplicate
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
        }
    }
}