using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class CoinCrown : ItemBase<CoinCrown>
    {
        public override string ItemName => "Coin Crown";

        public override string ItemLangTokenName => "COINCROWN";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => Assets.NullModel;

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgCashMultiplier),
            GetChance(cfgCashMultiplierPerStack)
        };

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.Any,
            ItemTag.Utility
        };

        public static ConfigEntry<float> cfgCashMultiplier;
        public static ConfigEntry<float> cfgCashMultiplierPerStack;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgCashMultiplier = config.Bind(ConfigCategory, "Cash Multiplier", 0.1f, "The percentage of extra money to get on completing the teleporter event.");
            cfgCashMultiplierPerStack = config.Bind(ConfigCategory, "Cash Multiplier Per Stack", 0.05f, "The percentage of extra money PER ITEM STACK to get on completing the teleporter event.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            TeleporterInteraction.onTeleporterChargedGlobal += TeleporterInteraction_onTeleporterChargedGlobal;
        }

        private void TeleporterInteraction_onTeleporterChargedGlobal(TeleporterInteraction teleporterInteraction)
        {
            foreach (var pcmc in PlayerCharacterMasterController.instances)
            {
                var itemCount = GetCount(pcmc.master);
                if (itemCount > 0)
                {
                    uint moneyToGive = (uint)(pcmc.master.money * (cfgCashMultiplier.Value + cfgCashMultiplierPerStack.Value * (itemCount - 1)));
                    //Chat.AddMessage("Gave money: " + moneyToGive);
                    pcmc.master.GiveMoney(moneyToGive);
                }
            }
        }
    }
}
