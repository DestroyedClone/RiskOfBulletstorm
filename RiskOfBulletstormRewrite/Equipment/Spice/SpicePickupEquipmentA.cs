using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    internal class SpicePickupEquipmentA : EquipmentBase<SpicePickupEquipmentA>
    {
        public override string EquipmentName => "SpicePickupEquipmentA";

        public override string EquipmentLangTokenName => "SPICEPICKUPEQUIPMENTA";

        public override string EquipmentPickupToken => "RISKOFBULLETSTORM_EQUIPMENT_SPICE_PICKUP_FORMAT";
        public override string[] EquipmentPickupDescParams => new string[] { "RISKOFBULLETSTORM_EQUIPMENT_SPICE_PICKUP_USES_1" };
        public override string EquipmentDescriptionToken => "RISKOFBULLETSTORM_EQUIPMENT_SPICE_DESCRIPTION";

        public override GameObject EquipmentModel => LoadModel("SPICE");

        public override Sprite EquipmentIcon => LoadSprite("SPICE");

        public override bool CanDrop => false;
        public override bool CanBeRandomlyTriggered => false;
        public override string ParentEquipmentName => "Spice";

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            return false;
        }
    }
}
