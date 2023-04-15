using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Equipment;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;
using static AkMIDIEvent;

namespace RiskOfBulletstormRewrite.Items
{
    public class SpiceTally : ItemBase<SpiceTally>
    {
        public override string ItemName => "SpiceTally";

        public override string ItemLangTokenName => "SPICETALLY";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.LoadSprite($"EQUIPMENT_SPICE");

        public override bool Hidden => false;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
        };

        public override void Init(ConfigFile config)
        {
            CreateItem();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }
    }
}