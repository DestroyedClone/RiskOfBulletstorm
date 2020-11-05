//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;

namespace RiskOfBulletstorm.Items
{
    public class Mustache : Item_V2<Mustache>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance to heal? (Default: 25%)", AutoConfigFlags.PreventNetMismatch)]
        public float HealChance { get; private set; } = 25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance to heal? (Default: 5%)", AutoConfigFlags.PreventNetMismatch)]
        public float HealChanceStack { get; private set; } = 5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Heal Percentage? (Default: 0.25 = 25%)", AutoConfigFlags.PreventNetMismatch)]
        public float HealAmount { get; private set; } = 0.25f;

        public override string displayName => "Mustache";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "<b>A Familiar Face</b>" +
            "\n<style=\"text-align: center;\">Spending money soothes the soul.</style>";

        protected override string GetDescString(string langid = null) => $"{Pct(HealChance)} (+{Pct(HealChanceStack)} per stack) chance to heal for {Pct(HealAmount)} health.";

        protected override string GetLoreString(string langID = null) => "The power of commerce fills your veins... and your follicles! This mustache vertically integrates your purchasing synergies, giving you a chance to be healed on every transaction.";

        private int InventoryCount = 0;

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
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            On.RoR2.CharacterBody.OnInventoryChanged += UpdateInvCount;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PurchaseInteraction.OnInteractionBegin -= PurchaseInteraction_OnInteractionBegin;
            On.RoR2.CharacterBody.OnInventoryChanged -= UpdateInvCount;
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            if (InventoryCount > 0)
            {
                var ResultChance = HealChance + HealChanceStack * (InventoryCount - 1);
                if (Util.CheckRoll(ResultChance))
                {
                    CharacterBody component = activator.GetComponent<CharacterBody>();
                    component.healthComponent.Heal(HealAmount, default, true);
                }
            }
            orig(self, activator);
        }
        private void UpdateInvCount(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            InventoryCount = GetCount(self);
            orig(self);
        }
    }
}
