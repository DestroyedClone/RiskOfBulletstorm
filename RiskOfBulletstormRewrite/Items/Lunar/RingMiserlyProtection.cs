using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class RingMiserlyProtection : ItemBase<RingMiserlyProtection>
    {
        public static float cfgMaxHealthPctAdded = 1;
        public static float cfgMaxHealthPctAddedStack = 0.5f;

        public override string ItemName => "Ring of Miserly Protection";

        public override string ItemLangTokenName => "RINGMISERLYPROTECTION";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            Utils.ItemHelpers.Pct(cfgMaxHealthPctAdded),
            Utils.ItemHelpers.Pct(cfgMaxHealthPctAddedStack)
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

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }
        public override string WikiLink => "https://thunderstore.io/package/DestroyedClone/RiskOfBulletstorm/wiki/190-item-ring-of-miserly-protection/";

        public override void Hooks()
        {
            ShrineChanceBehavior.onShrineChancePurchaseGlobal += ShatterRing;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var invCount = GetCount(sender);
            if (invCount > 0) args.healthMultAdd += cfgMaxHealthPctAdded + cfgMaxHealthPctAddedStack * (invCount - 1);
        }

        private void ShatterRing(bool gaveItem, Interactor interactor)
        {
            if (!interactor.gameObject.TryGetComponent(out CharacterBody body)) return;
            if (!body.inventory) return;

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