using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Equipment;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static AkMIDIEvent;

namespace RiskOfBulletstormRewrite.Items
{
    public class SpiceReplacementDisplay : ItemBase<SpiceReplacementDisplay>
    {
        public override string ItemName => "SpiceReplacementDisplay";

        public override string ItemLangTokenName => "SPICE";

        public override ItemTier Tier => ItemTier.NoTier;
        public override GameObject ItemModel => Assets.LoadObject("SPICE.prefab");

        public override Sprite ItemIcon => Assets.LoadSprite($"EQUIPMENT_SPICE");

        public override string ParentEquipmentName => "SPICE";

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateItem();
        }
    }
}
