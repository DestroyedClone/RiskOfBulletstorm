using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class OrangeConsumed : ItemBase<OrangeConsumed>
    {
        public override string ItemName => "Orange (Consumed)";

        public override string ItemLangTokenName => "ORANGECONSUMED";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            ToPct(Equipment.Orange.cfgMaxHealthIncrease),
            ToPct(Equipment.Orange.cfgChargeRateReduction)
        };

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Modules.Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override string ParentEquipmentName => "Orange";

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Healing
        };

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += Orange_StatCoefficients;
            On.RoR2.Inventory.CalculateEquipmentCooldownScale += ReduceCooldowns;
        }

        private float ReduceCooldowns(On.RoR2.Inventory.orig_CalculateEquipmentCooldownScale orig, Inventory self)
        {
            return orig(self) * Mathf.Pow(1f - Equipment.Orange.cfgChargeRateReduction, self.GetItemCount(ItemDef));
        }

        private void Orange_StatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                var itemCount = GetCount(sender);
                if (itemCount > 0)
                {
                    //args.baseHealthAdd += Equipment.Orange.cfgMaxHealthIncrease * itemCount;
                    args.healthMultAdd += Equipment.Orange.cfgMaxHealthIncrease * itemCount;
                }
            }
        }
    }
}