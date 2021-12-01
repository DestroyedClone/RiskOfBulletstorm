﻿using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class RingChestFriendship : ItemBase<RingChestFriendship>
    {
        public static ConfigEntry<float> cfgCreditMultiplier;
        public static ConfigEntry<float> cfgCreditMultiplierPerStack;

        public override string ItemName => "Ring of Chest Friendship";

        public override string ItemLangTokenName => "RINGOFCHESTFRIENDSHIP";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            (cfgCreditMultiplier.Value * 100).ToString(),
            (cfgCreditMultiplierPerStack.Value * 100).ToString()
        };

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("ExampleItemPrefab.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("ExampleItemIcon.png");

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgCreditMultiplier = config.Bind(ConfigCategory, "Config Credit Multiplier", 0.5f, "");
            cfgCreditMultiplierPerStack = config.Bind(ConfigCategory, "Config Credit Multiplier", 0.25f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            R2API.DirectorAPI.StageSettingsActions += DirectorAPI_StageSettingsActions;
        }

        private void DirectorAPI_StageSettingsActions(DirectorAPI.StageSettings arg1, DirectorAPI.StageInfo arg2)
        {
            var itemCount = Util.GetItemCountForTeam(TeamIndex.Player, ItemDef.itemIndex, false, true);
            //_logger.LogMessage($"Item Count: {itemCount}");
            if (itemCount > 0)
            {
                var value = 1 + (cfgCreditMultiplier.Value + cfgCreditMultiplierPerStack.Value * (itemCount - 1));
                var oldValue = arg1.SceneDirectorInteractableCredits;
                arg1.SceneDirectorInteractableCredits = Mathf.RoundToInt(arg1.SceneDirectorInteractableCredits * value);
                //_logger.LogMessage($"Credits increased from {oldValue} to {arg1.SceneDirectorInteractableCredits}");
            }
        }
    }
}
