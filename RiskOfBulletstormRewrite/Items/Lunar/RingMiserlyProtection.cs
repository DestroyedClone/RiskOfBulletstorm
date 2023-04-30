using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

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
        public override GameObject ItemModel => LoadModel();
        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Healing,
            ItemTag.Cleansable,
            ItemTag.InteractableRelated
        };

        private readonly GameObject ShatterEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/ShieldBreakEffect");

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgMaxHealthPctAdded = config.Bind(ConfigCategory, Assets.cfgMaxHealthAdditiveMultKey, 1f, Assets.cfgMaxHealthAdditiveMultDesc);
            cfgMaxHealthPctAddedStack = config.Bind(ConfigCategory, Assets.cfgMaxHealthAdditiveMultPerStackKey, 0.5f, Assets.cfgMaxHealthAdditiveMultPerStackDesc);
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