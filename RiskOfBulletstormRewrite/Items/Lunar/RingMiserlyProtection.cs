using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class RingMiserlyProtection : ItemBase<RingMiserlyProtection>
    {
        public static ConfigEntry<float> cfgMaxHealthPctAdded;
        public static ConfigEntry<float> cfgMaxHealthPctAddedStack;

        public override string ItemName => "Ring of Miserly Protection";

        public override string ItemLangTokenName => "RINGMISERLYPROTECTION";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            Utils.ItemHelpers.Pct(cfgMaxHealthPctAdded.Value),
            Utils.ItemHelpers.Pct(cfgMaxHealthPctAddedStack.Value)
        };

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("Assets/Models/Prefabs/RingMiserlyProtection.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("Assets/Textures/Icons/RingMiserlyProtection.png");

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Healing,
            ItemTag.Cleansable
        };

        private readonly GameObject ShatterEffect = Resources.Load<GameObject>("prefabs/effects/ShieldBreakEffect");

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgMaxHealthPctAdded = config.Bind(ConfigCategory, "Max Health Additive Multiplier", 1f, "How much maximum health is multiplied by per Ring of Miserly Protection?");
            cfgMaxHealthPctAddedStack = config.Bind(ConfigCategory, "Max Health Additive Multiplier per Stack", 0.5f, "How much additional maximum health is multiplied by per subsequent stacks of Ring of Miserly Protection?");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            ShrineChanceBehavior.onShrineChancePurchaseGlobal += ShatterRing;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var invCount = GetCount(sender);
            if (invCount > 0) args.healthMultAdd += cfgMaxHealthPctAdded.Value + cfgMaxHealthPctAddedStack.Value * (invCount - 1);
        }

        private void ShatterRing(bool gaveItem, Interactor interactor)
        {
            var body = interactor.gameObject.GetComponent<CharacterBody>();
            if (body && body.inventory)
            {
                var InventoryCount = body.inventory.GetItemCount(ItemDef);
                if (InventoryCount > 0)
                {
                    if (UnityEngine.Networking.NetworkServer.active)
                    {
                        body.inventory.RemoveItem(ItemDef);
                        body.RecalculateStats();
                    }
                    Util.PlaySound("Play_char_glass_death", body.gameObject);
                    EffectManager.SimpleEffect(ShatterEffect, body.gameObject.transform.position, Quaternion.identity, true);
                }
            }
        }
    }
}