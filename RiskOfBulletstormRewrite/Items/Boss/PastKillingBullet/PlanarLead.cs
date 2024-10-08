﻿using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class PlanarLead : ItemBase<PlanarLead>
    {
        public override string ItemName => "Planar Lead";

        public override string ItemLangTokenName => "PLANARLEAD";

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
            return;
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