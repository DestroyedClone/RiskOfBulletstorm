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
using static RiskOfBulletstorm.Shared.HelperUtil;

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

        protected override string GetDescString(string langid = null) => $"{descText} by {Pct(CostReductionAmount)} (+{Pct(CostReductionAmount)} per stack)";

        protected override string GetLoreString(string langID = null) => "The Pilot is able to talk his way into almost anything, usually gunfights.";

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
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PurchaseInteraction.Awake -= LowerCosts;
        }

        private void LowerCosts(On.RoR2.PurchaseInteraction.orig_Awake orig, PurchaseInteraction self)
        {
            orig(self);
            var chest = self.GetComponent<ChestBehavior>();
            int InventoryCount = GetPlayersItemCount(catalogIndex);

            if (chest)
            {
                Debug.Log("DisarmPerson: Chest Found!", self);
                if (InventoryCount > 0)
                {
                    Debug.Log("DisarmPerson: Inventory Count works!", self);
                    var ResultMult = Mathf.Min(1 - CostReductionAmount + CostReductionAmountStack * (InventoryCount - 1), 0);
                    Debug.Log("DisarmPerson: Cost(" + self.cost.ToString()+")=>"+ ((int)Mathf.Ceil(self.cost * ResultMult)).ToString(), self);
                    self.cost = (int)Mathf.Ceil(self.cost * ResultMult);
                }
            }
        }
    }
}
