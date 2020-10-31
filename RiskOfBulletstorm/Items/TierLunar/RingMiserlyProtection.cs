﻿//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;


namespace RiskOfBulletstorm.Items
{
    public class RingMiserlyProtection : Item_V2<RingMiserlyProtection>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Health Increase", AutoConfigFlags.PreventNetMismatch)]
        public float HealthBonus { get; private set; } = 1.0f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Health Increase Stack", AutoConfigFlags.PreventNetMismatch)]
        public float HealthBonusStack { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Enable item synergy? (Default: True)", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableSynergy { get; private set; } = true;
        public bool RecentPurchase = false;
        public override string displayName => "Ring of Miserly Protection";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Aids The Fiscally Responsible\nIncreases health substantially. Any purchases will shatter the ring.";

        protected override string GetDescString(string langid = null) => $"Grants +{Pct(HealthBonus)} (+{Pct(HealthBonusStack)} per stack) health <style=cIsDeath>...but breaks completely upon purchasing.</style> ";

        protected override string GetLoreString(string langID = null) => "Before the Shopkeep opened his shop, he was an avaricious and miserly man. He remains careful about any expenditures, but through capitalism he has purged himself of negative emotion.";

        public override void SetupBehavior()
        {

        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.PurchaseInteraction.OnInteractionBegin += On_InteractionBegin;
            GetStatCoefficients += BoostHealth;
            On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PurchaseInteraction.OnInteractionBegin -= On_InteractionBegin;
            GetStatCoefficients -= BoostHealth;
            On.RoR2.CharacterBody.FixedUpdate -= CharacterBody_FixedUpdate;
        }
        private void On_InteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            RecentPurchase = true;
            Chat.AddMessage(activator.name);
            

            orig(self, activator);
        }
        private void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            var InventoryCount = GetCount(self);
            if (!RecentPurchase || InventoryCount < 1)
            {
                return;
            }
            self.inventory.RemoveItem(catalogIndex, InventoryCount);
            RecentPurchase = false;
            orig(self);
        }
        private void BoostHealth(CharacterBody sender, StatHookEventArgs args)
        {
            var invCount = GetCount(sender);
            if (invCount > 0)
            { args.healthMultAdd += HealthBonus + HealthBonusStack * (invCount - 1); }
        }
    }
}
