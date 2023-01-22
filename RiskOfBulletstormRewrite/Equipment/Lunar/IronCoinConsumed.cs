using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class IronCoinConsumed : EquipmentBase<IronCoinConsumed>
    {
        //public static ConfigEntry<float> cfg;

        public override string EquipmentName => "Iron Coin (Spent)";

        public override string EquipmentLangTokenName => "IRONCOINCONSUMED";

        public override GameObject EquipmentModel => Assets.NullModel;

        public override Sprite EquipmentIcon => LoadSprite();

        public override bool IsLunar => false;

        public override float Cooldown => 60f;

        public override bool CanDrop => false;

        public override bool CanBeRandomlyTriggered => false;
        public override string ParentEquipmentName => "Iron Coin";

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
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot.characterBody)
            {
                //RISKOFBULLETSTORM_EQUIPMENT_IRONCOINCONSUMED_FLAVORSIDE maybe
                var result = Util.CheckRoll(50f) ? "RISKOFBULLETSTORM_EQUIPMENT_IRONCOINCONSUMED_FLAVORHEADS" : "RISKOFBULLETSTORM_EQUIPMENT_IRONCOINCONSUMED_FLAVORTAILS";
                Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
                {
                    baseToken = "RISKOFBULLETSTORM_EQUIPMENT_IRONCOINCONSUMED_FLAVOR",
                    paramTokens = new string[] { result },
                    subjectAsCharacterBody = slot.characterBody
                });
                slot.subcooldownTimer = 1f;
            }
            return true;
        }
    }
}