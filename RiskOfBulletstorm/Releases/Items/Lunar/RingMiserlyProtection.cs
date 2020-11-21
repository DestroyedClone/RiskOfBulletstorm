//using System;
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
    public class RingMiserlyProtection : Item_V2<RingMiserlyProtection> //switch to a buff system due to broken
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Health Increase", AutoConfigFlags.PreventNetMismatch)]
        public float RingMiserlyProtection_HealthBonus { get; private set; } = 1.0f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Health Increase Stack", AutoConfigFlags.PreventNetMismatch)]
        public float RingMiserlyProtection_HealthBonusStack { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("[unimplemented] Enable item synergy? (Default: True)", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableSynergy { get; private set; } = true;
        public override string displayName => "Ring of Miserly Protection";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.Cleansable });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Aids The Fiscally Responsible\nIncreases health substantially. Any shrine purchases will shatter a ring.";

        protected override string GetDescString(string langid = null) => $"Grants +{Pct(RingMiserlyProtection_HealthBonus)} (+{Pct(RingMiserlyProtection_HealthBonusStack)} per stack) health <style=cIsDeath>...but breaks a ring completely upon using a shrine.</style> ";

        protected override string GetLoreString(string langID = null) => "Before the Shopkeep opened his shop, he was an avaricious and miserly man. He remains careful about any expenditures, but through capitalism he has purged himself of negative emotion.";

        public RingMiserlyProtection()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/RingMiserlyProtection.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/RingMiserlyProtectionIcon.png";
        }
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
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PurchaseInteraction.OnInteractionBegin -= On_InteractionBegin;
            GetStatCoefficients -= BoostHealth;
        }
        private void On_InteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            var body = activator.gameObject.GetComponent<CharacterBody>();
            var InventoryCount = body.inventory.GetItemCount(catalogIndex);

            if (self.isShrine && !self.GetComponent<ShrineCombatBehavior>())
            {
                if (InventoryCount > 0)
                {
                    body.inventory.RemoveItem(catalogIndex);
                    body.RecalculateStats();
                }
            }
            //Chat.AddMessage(activator.name);
            orig(self, activator);
        }
        private void BoostHealth(CharacterBody sender, StatHookEventArgs args)
        {
            var invCount = GetCount(sender);
            if (invCount > 0) args.healthMultAdd += RingMiserlyProtection_HealthBonus + RingMiserlyProtection_HealthBonusStack * (invCount - 1);
        }
    }
}
