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
    internal class SpicePickupEquipmentC : EquipmentBase<SpicePickupEquipmentC>
    {
        public override string EquipmentName => "SpicePickupEquipmentC";

        public override string EquipmentLangTokenName => "SPICEC";

        public override string EquipmentPickupToken => "RISKOFBULLETSTORM_EQUIPMENT_SPICE_PICKUP_C";

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
            slot.inventory.SetEquipmentIndex(Spice2.Instance.EquipmentDef.equipmentIndex);
            return false;
        }
    }
}
