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
    public class Key : Item_V2<Key>
    {
        public override string displayName => "Key";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Can unlock chests. Consumed on use.";

        protected override string GetDescString(string langid = null) => $"Can unlock chests. Consumed on use.";

        protected override string GetLoreString(string langID = null) => "";

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
            On.RoR2.PurchaseInteraction.OnInteractionBegin += InteractWithChest;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PurchaseInteraction.OnInteractionBegin -= InteractWithChest;
        }


        private void InteractWithChest(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            //bool CanAfford = self.CanBeAffordedByInteractor(activator);

            CharacterBody characterBody = activator.GetComponent<CharacterBody>();
            if (characterBody)
            {
                Inventory inventory = characterBody.inventory;
                if (inventory)
                {
                    int InventoryCount = characterBody.inventory.GetItemCount(catalogIndex);
                    if (InventoryCount > 0)
                    {
                        if (self.isShrine == false && self.available && self.costType == CostTypeIndex.Money) //if not shrine, is available, and is not a lunar pod
                        {
                            Debug.Log("Key Triggered!");
                            self.SetAvailable(false);
                            self.Networkavailable = false;

                            self.gameObject.GetComponent<ChestBehavior>().Open();

                            self.cost = 0;
                            self.Networkcost = 0;

                            self.onPurchase.Invoke(activator);
                            self.lastActivator = activator;

                            inventory.RemoveItem(catalogIndex);
                        }
                    }
                }
            }
            orig(self, activator);
        }
    }
}
