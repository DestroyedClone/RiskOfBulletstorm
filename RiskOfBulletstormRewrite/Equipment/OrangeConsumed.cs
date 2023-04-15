using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class OrangeConsumed : EquipmentBase<OrangeConsumed>
    {
        //public static ConfigEntry<float> cfg;
        public static ConfigEntry<float> cfgCooldown;
        public override float Cooldown => cfgCooldown.Value;

        public override string EquipmentName => "Orange (Consumed)";

        public override string EquipmentLangTokenName => "ORANGECONSUMED";
        public override string[] EquipmentFullDescriptionParams => Equipment.Orange.Instance.EquipmentFullDescriptionParams;

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        public override bool IsLunar => false;

        public override bool CanDrop => false;

        public override bool CanBeRandomlyTriggered => false;
        public override string ParentEquipmentName => "Orange";

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
        }

        protected override void CreateConfig(ConfigFile config)
        {
            base.CreateConfig(config);
            cfgCooldown = config.Bind(ConfigCategory, CooldownName, 5f, CooldownDescription);
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            return true;
        }
    }
}