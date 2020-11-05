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
    public class DisarmingPersonality : Item_V2<DisarmingPersonality>
    {
        [AutoConfig("Base cost reduction? Default 0.15 15%", AutoConfigFlags.PreventNetMismatch)]
        public float CostReductionAmount { get; private set; } = 0.15f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Stack cost reduction? Default 0.05 5%", AutoConfigFlags.PreventNetMismatch)]
        public float CostReductionAmountStack { get; private set; } = 0.05f;

        public override string displayName => "Disarming Personality";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        private readonly string descText = "Reduces prices at shops";
        protected override string GetPickupString(string langID = null) => "For You?\n"+descText;

        protected override string GetDescString(string langid = null) => $"{descText} by ";

        protected override string GetLoreString(string langID = null) => "The Pilot is able to talk his way into almost anything, usually gunfights.";

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
            On.RoR2.PurchaseInteraction.Awake += LowerCosts;
            On.RoR2.CharacterBody.OnInventoryChanged += UpdateInvCount;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PurchaseInteraction.Awake -= LowerCosts;
            On.RoR2.CharacterBody.OnInventoryChanged -= UpdateInvCount;
        }

        private void LowerCosts(On.RoR2.PurchaseInteraction.orig_Awake orig, PurchaseInteraction self)
        {
            orig(self);
            var chest = self.GetComponent<ChestBehavior>();
            if (chest)
            {
                if (InventoryCount > 0)
                {
                    var ResultMult = CostReductionAmount + CostReductionAmountStack * (InventoryCount - 1);
                    self.cost = (int)Mathf.Ceil(self.cost * ResultMult);
                }
            }
        }
        private void UpdateInvCount(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            InventoryCount = GetCount(self);
            orig(self);
        }
    }
}
