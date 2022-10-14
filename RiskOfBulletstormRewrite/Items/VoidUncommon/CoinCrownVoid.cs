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

        public override GameObject ItemModel => Assets.NullModel;

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgCashAdder),
            GetChance(cfgCashAdderStack)
        };

        public override ItemDef ContagiousOwnerItemDef => CoinCrown.instance.ItemDef;


        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.Any,
            ItemTag.Utility
        };

        public static ConfigEntry<float> cfgCashAdder;
        public static ConfigEntry<float> cfgCashAdderStack;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgCashAdder = config.Bind(ConfigCategory, "Cash Multiplier", 0.1f, "The percentage of extra money to get on completing the teleporter event.");
            cfgCashAdderStack = config.Bind(ConfigCategory, "Cash Multiplier Per Stack", 0.05f, "The percentage of extra money PER ITEM STACK to get on completing the teleporter event.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
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

        public uint itemCount = 0;

        private void IncreaseMoney(On.RoR2.CharacterMaster.orig_GiveMoney orig, CharacterMaster self, uint amount)
        {
            amount += (uint)GetStack(cfgCashAdder.Value, cfgCashAdderStack.Value, (int)itemCount);
            orig(self, amount);
        }

        private void TeleporterInteraction_onTeleporterChargedGlobal(TeleporterInteraction teleporterInteraction)
        {
            
            On.RoR2.CharacterMaster.GiveMoney -= IncreaseMoney;
        }
    }
}
