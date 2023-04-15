using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class SpiceConsumed : EquipmentBase<SpiceConsumed>
    {
        public static ConfigEntry<float> cfgCooldown;
        public override float Cooldown => cfgCooldown.Value;

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

        protected override void CreateConfig(ConfigFile config)
        {
            cfgCooldown = config.Bind(ConfigCategory, CooldownName, 5f, CooldownDescription);
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot.characterBody)
            {
                slot.subcooldownTimer = 1f;
            }
            return true;
        }
    }
}