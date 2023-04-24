using BepInEx.Configuration;
using R2API;
using Rewired.Utils;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class AmmoBelt : ItemBase<AmmoBelt>
    {
        public static ConfigEntry<float> cfgPercentageStockAdditive;
        public static ConfigEntry<int> cfgAmmoPackAdditionalRecharge;
        public static ConfigEntry<int> cfgAmmoPackAdditionalRechargeStack;

        public override string ItemName => "Ammo Belt";

        public override string ItemLangTokenName => "AMMOBELT";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgPercentageStockAdditive),
            cfgAmmoPackAdditionalRecharge.Value.ToString(),
            cfgAmmoPackAdditionalRechargeStack.Value.ToString()
        };

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Utility
        };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgPercentageStockAdditive = config.Bind(ConfigCategory, "Percentage", 0.15f, "This value is rounded when giving stocks.");
            cfgAmmoPackAdditionalRecharge = config.Bind(ConfigCategory, "Ammo Pack Additional Stock", 1, "This value is rounded when giving stocks.");
            cfgAmmoPackAdditionalRechargeStack = config.Bind(ConfigCategory, "Ammo Pack Additional Stock", 1, "This value is rounded when giving stocks.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }

        public override void Hooks()
        {
            On.RoR2.GenericSkill.SetBonusStockFromBody += AddAmmoBeltStocks;
            On.RoR2.SkillLocator.ApplyAmmoPack += SkillLocator_ApplyAmmoPack;
        }

        private void SkillLocator_ApplyAmmoPack(On.RoR2.SkillLocator.orig_ApplyAmmoPack orig, SkillLocator self)
        {
            throw new System.NotImplementedException();
        }

        public void AddAmmoBeltStocks(On.RoR2.GenericSkill.orig_SetBonusStockFromBody orig, GenericSkill self, int newBonusStockFromBody)
        {
            if (self.baseStock > 1)
            {
                var characterBody = self.characterBody;
                if (characterBody)
                {
                    var itemCount = GetCount(characterBody);
                    if (itemCount > 0)
                    {
                        int stocksToGive;
                        if (itemCount == 1)
                        {
                            stocksToGive = 1;
                        } else
                        {
                            var multipliedValue = itemCount * cfgPercentageStockAdditive.Value;
                            stocksToGive = Mathf.RoundToInt(multipliedValue);
                            stocksToGive = Mathf.Max(1, stocksToGive);
                        }
                        newBonusStockFromBody += stocksToGive;
                    }
                }
            }
            orig(self, newBonusStockFromBody);
        }
    }
}