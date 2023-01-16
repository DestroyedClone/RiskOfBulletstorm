using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class OrangeConsumed : EquipmentBase<OrangeConsumed>
    {
        //public static ConfigEntry<float> cfg;

        public override string EquipmentName => "Orange (Consumed)";

        public override string EquipmentLangTokenName => "ORANGECONSUMED";
        public override string[] EquipmentFullDescriptionParams => Equipment.Orange.instance.EquipmentFullDescriptionParams;

        public override GameObject EquipmentModel => Assets.NullModel;

        public override Sprite EquipmentIcon => LoadSprite();

        public override bool IsLunar => false;

        public override float Cooldown => 5f;

        public override bool CanDrop => false;

        public override bool CanBeRandomlyTriggered => false;
        public override string ParentEquipmentName => "Orange";


        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Init(ConfigFile config)
        {
            CreateEquipment();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            return true;
        }


    }
}
