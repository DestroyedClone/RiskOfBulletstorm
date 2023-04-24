using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    internal class SpicePickupEquipmentA : EquipmentBase<SpicePickupEquipmentA>
    {
        public override string EquipmentName => "SpicePickupEquipmentA";

        public override string EquipmentLangTokenName => "SPICEA";

        public override string EquipmentPickupToken => "RISKOFBULLETSTORM_EQUIPMENT_SPICE_PICKUP_A";

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