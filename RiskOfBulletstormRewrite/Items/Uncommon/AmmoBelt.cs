using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;

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

        public override Sprite ItemIcon => Assets.NullSprite;

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
            cfgPercentageStockAdditive = config.Bind(ConfigCategory, "Percentage", 0.05f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            On.RoR2.GenericSkill.SetBonusStockFromBody += AddAmmoBeltStocks;
            //On.RoR2.GenericSkill.RecalculateMaxStock += RecalculateAmmoBeltStocks;
        }

        public void RecalculateAmmoBeltStocks(On.RoR2.GenericSkill.orig_RecalculateMaxStock orig, GenericSkill self)
        {
            orig(self);
            //self.maxStock += (int)Mathf.Min(1, 0);
        }

        public void AddAmmoBeltStocks(On.RoR2.GenericSkill.orig_SetBonusStockFromBody orig, GenericSkill self, int newBonusStockFromBody)
        {
            var characterBody = self.characterBody;
            if (characterBody)
            {
                var itemCount = GetCount(characterBody);
                if (itemCount > 0)
                {
                    var stocksToGive = (int)Mathf.Min(1, itemCount * cfgPercentageStockAdditive.Value);
                    newBonusStockFromBody += stocksToGive;
                }
            }

            orig(self, newBonusStockFromBody);
        }

    }
}
