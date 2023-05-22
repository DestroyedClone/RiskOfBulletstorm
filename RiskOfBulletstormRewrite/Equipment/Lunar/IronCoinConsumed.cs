using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class IronCoinConsumed : EquipmentBase<IronCoinConsumed>
    {
        public static ConfigEntry<bool> cfgResultPrintByPlayersOnly;
        public override float Cooldown => 30f;

        public override string EquipmentName => "Iron Coin (Spent)";

        public override string EquipmentLangTokenName => "IRONCOINCONSUMED";

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        public override bool IsLunar => false;

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
            cfgResultPrintByPlayersOnly = config.Bind(ConfigCategory, "Only Players Can Print Result", false, "If true, then only players can print the result of the flip.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = EquipmentModel;
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.05f, 0.05f, 0.05f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0, 0, 0)
                }
            });
            return rules;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot.characterBody)
            {
                var result = Util.CheckRoll(1f)
                    ? "RISKOFBULLETSTORM_EQUIPMENT_IRONCOINCONSUMED_FLAVORSIDE"
                    : Util.CheckRoll(50f)
                    ? "RISKOFBULLETSTORM_EQUIPMENT_IRONCOINCONSUMED_FLAVORHEADS"
                    : "RISKOFBULLETSTORM_EQUIPMENT_IRONCOINCONSUMED_FLAVORTAILS";

                if ((cfgResultPrintByPlayersOnly.Value && slot.characterBody.isPlayerControlled)
                    || !cfgResultPrintByPlayersOnly.Value)
                {
                    Chat.SendBroadcastChat(new Chat.SubjectFormatChatMessage
                    {
                        baseToken = "RISKOFBULLETSTORM_EQUIPMENT_IRONCOINCONSUMED_FLAVOR",
                        paramTokens = new string[] { result },
                        subjectAsCharacterBody = slot.characterBody
                    });
                }
                slot.subcooldownTimer = 1f;
            }
            return true;
        }
    }
}