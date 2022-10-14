using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class Orange : EquipmentBase<Orange>
    {
        //Item bloat makes chance reduction even bigger of a pain in the ass.
        //public static ConfigEntry<float> cfgChanceReduction;
        public static ConfigEntry<float> cfgChargeRateReduction;
        public static ConfigEntry<float> cfgMaxHealthIncrease;
        public static ConfigEntry<float> cfgHealPercentage;

        public override string EquipmentName => "Orange";

        public override string EquipmentLangTokenName => "ORANGE";

        public override string[] EquipmentFullDescriptionParams => new string[]
        {
            GetChance(cfgMaxHealthIncrease),
            GetChance(cfgHealPercentage),
            GetChance(cfgChargeRateReduction)
        };

        public override GameObject EquipmentModel => Assets.NullModel;

        public override Sprite EquipmentIcon => LoadSprite();

        public override float Cooldown => 45f;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateConfig(ConfigFile config)
        {
            cfgMaxHealthIncrease = config.Bind(ConfigCategory, "Max Health Increase", 0.1f);
            cfgHealPercentage = config.Bind(ConfigCategory, "Heal Percentage", 1f);
            cfgChargeRateReduction = config.Bind(ConfigCategory, "Equipment Charge Rate Reduction", 0.1f);
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            slot.equipmentIndex = EquipmentIndex.None;
            slot.inventory.GiveItem(Items.OrangeConsumed.instance.ItemDef);
            slot.healthComponent.HealFraction(cfgHealPercentage.Value, default);
            return true;
        }

        public override void Hooks()
        {
        }

    }
}
