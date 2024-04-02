using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class SpiceConsumed : EquipmentBase<SpiceConsumed>
    {
        public override float Cooldown => 30;

        public override string EquipmentName => "Spice (Consumed)";

        public override string EquipmentLangTokenName => "SPICECONSUMED";

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        public override bool IsLunar => false;

        public override bool CanDrop => false;

        public override bool CanBeRandomlyTriggered => false;
        public override string ParentEquipmentName => "Spice";

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
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

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot.characterBody)
            {
                slot.subcooldownTimer = 5f;
            }
            return true;
        }
    }
}