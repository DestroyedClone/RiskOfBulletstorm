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
        protected override string GetPickupString(string langID = null) => "<style=cIsUtility>Can unlock chests.</style>\nConsumed on use.";

        protected override string GetDescString(string langid = null) => $"<style=cIsUtility>Can unlock chests.</style>\nConsumed on use.";

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
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            //Chat.AddMessage("Key: Entered Hook with "+self+" and "+activator);
            CharacterBody characterBody = activator.GetComponent<CharacterBody>();
            if (characterBody)
            {
                //Chat.AddMessage("Key: Entered CB");
                Inventory inventory = characterBody.inventory;
                if (inventory)
                {
                    //Chat.AddMessage("Key: Entered Inv");
                    if (self.isShrine == false && self.available && self.costType == CostTypeIndex.Money) //if not shrine, is available, and is not a lunar pod
                    {
                        //Chat.AddMessage("Key: Entered Primary Check");
                        int InventoryCount = characterBody.inventory.GetItemCount(catalogIndex);
                        if (InventoryCount > 0)
                        {
                            Chat.AddMessage("Key: Entered Worked!");
                            return Interactability.Available;
                        }
                    }
                }
            }
            return orig(self, activator);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.Interactor.PerformInteraction -= Interactor_PerformInteraction;
            On.RoR2.PurchaseInteraction.GetInteractability -= PurchaseInteraction_GetInteractability;
        }

        private void Interactor_PerformInteraction(On.RoR2.Interactor.orig_PerformInteraction orig, Interactor self, GameObject interactableObject)
        {
            PurchaseInteraction purchaseInteraction = interactableObject.GetComponent<PurchaseInteraction>();
            if (purchaseInteraction)
            {
                //bool CanAfford = self.CanBeAffordedByInteractor(activator);

                CharacterBody characterBody = self.GetComponent<CharacterBody>();
                if (characterBody)
                {
                    Inventory inventory = characterBody.inventory;
                    if (inventory)
                    {
                        int InventoryCount = characterBody.inventory.GetItemCount(catalogIndex);
                        if (InventoryCount > 0)
                        {
                            purchaseInteraction.GetInteractability(self);

                            if (purchaseInteraction.isShrine == false && purchaseInteraction.available && purchaseInteraction.costType == CostTypeIndex.Money) //if not shrine, is available, and is not a lunar pod
                            {
                                Debug.Log("Key Triggered!");
                                purchaseInteraction.SetAvailable(false);
                                purchaseInteraction.Networkavailable = false;

                                purchaseInteraction.gameObject.GetComponent<ChestBehavior>().Open();

                                purchaseInteraction.cost = 0;
                                purchaseInteraction.Networkcost = 0;

                                purchaseInteraction.onPurchase.Invoke(self);
                                purchaseInteraction.lastActivator = self;

                                inventory.RemoveItem(catalogIndex);
                            }
                        }
                    }
                }
            }
            orig(self, interactableObject);
        }
    }
}
