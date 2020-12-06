using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.Utils.HelperUtil;

namespace RiskOfBulletstorm.Items
{
    public class DisarmingPersonality : Item_V2<DisarmingPersonality>
    {
        [AutoConfig("What is the base cost reduction of one Disarming Personality? (Default 0.1 = 10%)", AutoConfigFlags.PreventNetMismatch)]
        public float DisarmingPersonality_CostReductionAmount { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cost reduction per stack? (Default 0.05 = 5%)", AutoConfigFlags.PreventNetMismatch)]
        public float DisarmingPersonality_CostReductionAmountStack { get; private set; } = 0.05f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the maximum cost reduction? This limit is hyperbolic. (Default 0.6 = 60%)", AutoConfigFlags.PreventNetMismatch)]
        public float DisarmingPersonality_CostReductionAmountLimit { get; private set; } = 0.6f;

        public override string displayName => "Disarming Personality";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        private readonly string descText = "Reduces prices at shops";
        protected override string GetPickupString(string langID = null) => "For You?\n"+descText;

        protected override string GetDescString(string langid = null) => $"<style=cIsUtility>{descText} by {Pct(DisarmingPersonality_CostReductionAmount)}</style>" +
            $"\n<style=cStack>(+{Pct(DisarmingPersonality_CostReductionAmount)} hyperbolically per stack) up to {Pct(DisarmingPersonality_CostReductionAmountLimit )}</stack>" +
            $"\n <style=cSub>Chance is shared amongst players.</style>";

        protected override string GetLoreString(string langID = null) => "The Pilot is able to talk his way into almost anything, usually gunfights.";

        public DisarmingPersonality()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/DisarmingPersonality.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/DisarmingPersonalityIcon.png";
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
                if (InventoryCount > 0)
                {
                    //var ResultMultUnclamp = 1 - DisarmingPersonality_CostReductionAmount + DisarmingPersonality_CostReductionAmountStack * (InventoryCount - 1);
                    //var ResultMult = Mathf.Max(ResultMultUnclamp, 0);
                        //credit to harb

                    var ResultMult = (DisarmingPersonality_CostReductionAmount + (1 - DisarmingPersonality_CostReductionAmount) * (1 - (DisarmingPersonality_CostReductionAmountLimit / Mathf.Pow(InventoryCount + DisarmingPersonality_CostReductionAmountLimit, DisarmingPersonality_CostReductionAmountStack))));
                    var ResultAmt = (int)Mathf.Ceil(self.cost * ResultMult);
                    Debug.Log("Cost of chest reduced from" + self.cost + " to " + ResultAmt + " with multiplier "+ResultMult);
                    self.cost = ResultAmt;
                    self.Networkcost = ResultAmt;
                }
            }
        }
    }
}
