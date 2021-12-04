using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class ExampleEquipment : EquipmentBase<>
    {
        public static ConfigEntry<float> cfg;

        public override string EquipmentName => "Deprecate Me Equipment";

        public override string EquipmentLangTokenName => "DEPRECATE_ME_EQUIPMENT";

        public override string[] EquipmentFullDescriptionParams => new string[]
        {

        };

        public override GameObject EquipmentModel => MainAssets.LoadAsset<GameObject>("ExampleEquipmentPrefab.prefab");

        public override Sprite EquipmentIcon => MainAssets.LoadAsset<Sprite>("ExampleEquipmentIcon.png");

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            return false;
        }


    }
}
