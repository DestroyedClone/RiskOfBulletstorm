using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class CoinCrownVoid : ItemBase<CoinCrownVoid>
    {
        public override string ItemName => "Coin Crown";

        public override string ItemLangTokenName => "COINCROWNVOID";

        public override ItemTier Tier => ItemTier.VoidTier2;

        public override GameObject ItemModel => LoadModel();

        public override string[] ItemFullDescriptionParams => new string[]
        {
            cfgCashAdder.Value.ToString(),
            cfgCashAdderStack.Value.ToString()
        };

        public override ItemDef ContagiousOwnerItemDef => CoinCrown.instance.ItemDef;

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.Any,
            ItemTag.Utility
        };

        public static ConfigEntry<int> cfgCashAdder;
        public static ConfigEntry<int> cfgCashAdderStack;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgCashAdder = config.Bind(ConfigCategory, "Cash Additive", 5, "The amount of extra money to get on kill.");
            cfgCashAdderStack = config.Bind(ConfigCategory, "Cash Additive Per Stack", 10, "The amount of extra money to get on kill per stack.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return CoinCrown.instance.CreateItemDisplayRules();
        }

        public override void Hooks()
        {
            TeleporterInteraction.onTeleporterBeginChargingGlobal += StartCharging;
            TeleporterInteraction.onTeleporterChargedGlobal += TeleporterInteraction_onTeleporterChargedGlobal;
            Stage.onServerStageBegin += StartStage;
            Inventory.onInventoryChangedGlobal += UpdateItemCount;
        }

        public void UpdateItemCount(Inventory inventory)
        {
            itemCount = (uint)RoR2.Util.GetItemCountForTeam(TeamIndex.Player, ItemDef.itemIndex, false);
        }

        private void StartStage(Stage stage)
        {
            On.RoR2.CharacterMaster.GiveMoney -= IncreaseMoney;
        }

        private void StartCharging(TeleporterInteraction teleporterInteraction)
        {
            On.RoR2.CharacterMaster.GiveMoney += IncreaseMoney;
        }

        //set by UpdateItemCount
        public uint itemCount = 0;

        private void IncreaseMoney(On.RoR2.CharacterMaster.orig_GiveMoney orig, CharacterMaster self, uint amount)
        {
            if (itemCount > 0)
                amount += (uint)GetStack(cfgCashAdder.Value, cfgCashAdderStack.Value, (int)itemCount);
            orig(self, amount);
        }

        private void TeleporterInteraction_onTeleporterChargedGlobal(TeleporterInteraction teleporterInteraction)
        {
            On.RoR2.CharacterMaster.GiveMoney -= IncreaseMoney;
        }
    }
}