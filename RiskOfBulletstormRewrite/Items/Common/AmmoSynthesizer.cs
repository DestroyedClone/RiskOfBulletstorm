using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items.Common
{
    internal class AmmoSynthesizer : ItemBase<AmmoSynthesizer>
    {
        public override string ItemName => "Ammo Synthesizer";

        public override string ItemLangTokenName => "SYNTHESIZERAMMO";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }


    }
}
