using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class AmmoBelt : ItemBase<AmmoBelt>
    {
        public static ConfigEntry<float> cfgPercentageStockAdditive;

        public override string ItemName => "Ammo Belt";

        public override string ItemLangTokenName => "AMMOBELT";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgPercentageStockAdditive),
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
            cfgPercentageStockAdditive = config.Bind(ConfigCategory, "Percentage", 0.09f, "This value is rounded when giving stocks.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }

        public override void Hooks()
        {
            On.RoR2.GenericSkill.SetBonusStockFromBody += AddAmmoBeltStocks;
        }

        public void AddAmmoBeltStocks(On.RoR2.GenericSkill.orig_SetBonusStockFromBody orig, GenericSkill self, int newBonusStockFromBody)
        {
            if (self.baseStock > 0)
            {
                var characterBody = self.characterBody;
                if (characterBody)
                {
                    var itemCount = GetCount(characterBody);
                    if (itemCount > 0)
                    {
                        int stocksToGive = 0;
                        if (itemCount == 1)
                        {
                            stocksToGive = 1;
                        } else
                        {
                            var multipliedValue = itemCount * cfgPercentageStockAdditive.Value;
                            newBonusStockFromBody = Mathf.RoundToInt(multipliedValue);
                        }
                        newBonusStockFromBody += stocksToGive;
                    }
                }
            }
            orig(self, newBonusStockFromBody);
        }
    }
}