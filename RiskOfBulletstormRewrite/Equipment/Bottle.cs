using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class Bottle : EquipmentBase<Bottle>
    {
        public static ConfigEntry<float> cfgExcessHealingPercentage;
        public static ConfigEntry<float> cfgMaxHealthPercentagePool;

        public override string EquipmentName => "Bottle";

        public override string EquipmentLangTokenName => "BOTTLE";

        public override string[] EquipmentFullDescriptionParams => new string[]
        {
            GetChance(cfgExcessHealingPercentage),
            GetChance(cfgMaxHealthPercentagePool)
        };

        public override GameObject EquipmentModel => Assets.NullModel;

        public override Sprite EquipmentIcon => Assets.NullSprite;

        public override float Cooldown => 15f;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateConfig(ConfigFile config)
        {
            cfgExcessHealingPercentage = config.Bind(ConfigCategory, "Excess Healing Percentage", 0.01f, "Percentage of healing to try to store.");
            cfgMaxHealthPercentagePool = config.Bind(ConfigCategory, "Max Health Percentage", 10f, "Percentage of max health stored in healing.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            var comp = slot.GetComponent<BottleStorageController>();
            if (comp)
            {
                if (comp.healthComponent)
                {
                    comp.ConsumeReserve();
                    return true;
                }
            }
            return false;
        }

        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            HealthComponent.onCharacterHealServer += HealthComponent_onCharacterHealServer;
        }

        private void HealthComponent_onCharacterHealServer(HealthComponent healthComponent, float amount, ProcChainMask procChainMask)
        {
            var comp = healthComponent.gameObject.GetComponent<BottleStorageController>();
            if (comp && comp.equipmentSlot && comp.equipmentSlot.stock > 0)
            {
                //Chat.AddMessage($"Bottle: {amount * cfgExcessHealingPercentage.Value} / {healthComponent.fullHealth * cfgMaxHealthPercentagePool.Value}");
                comp.AddReserve(amount * cfgExcessHealingPercentage.Value, healthComponent.fullHealth*cfgMaxHealthPercentagePool.Value);
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody obj)
        {
            if (NetworkServer.active)
            {
                var comp = obj.gameObject.GetComponent<BottleStorageController>();
                bool flag = obj.inventory.currentEquipmentIndex == EquipmentDef.equipmentIndex;
                if (flag != comp)
                {
                    if (flag)
                    {
                        comp = obj.gameObject.AddComponent<BottleStorageController>();
                        if (obj.equipmentSlot)
                            comp.equipmentSlot = obj.equipmentSlot;
                        comp.healthComponent = obj.healthComponent;
                        return;
                    }
                    UnityEngine.Object.Destroy(comp);
                }
            }
        }

        public class BottleStorageController : MonoBehaviour
        {
            public EquipmentSlot equipmentSlot;

            public void AddReserve(float amount, float max)
            {
                this.reserve = Mathf.Min(this.reserve + amount, max);
            }

            public void ConsumeReserve()
            {
                Util.PlaySound("item_use_blackhole_suckin_01", gameObject);
                healthComponent.Heal(reserve, default);
                reserve = 0;
            }

            public float reserve;

            public HealthComponent healthComponent;
        }
    }
}
