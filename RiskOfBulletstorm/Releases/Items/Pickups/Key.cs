using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class Key : Item_V2<Key>
    {
        public override string displayName => "Key";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        private readonly string desc = $"<style=cIsUtility>Can unlock chests.</style>\nConsumed on use.";
        protected override string GetPickupString(string langID = null) => desc;

        protected override string GetDescString(string langid = null) => desc;

        protected override string GetLoreString(string langID = null) => "";

        public Key()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Key.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/KeyIcon.png";
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
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            TrustyLockpicks.TrustyLockpickFailed attempted = self.gameObject.GetComponent<TrustyLockpicks.TrustyLockpickFailed>();

            if (!attempted)
            {
                CharacterBody characterBody = activator.GetComponent<CharacterBody>();
                if (characterBody)
                {
                    Inventory inventory = characterBody.inventory;
                    if (inventory)
                    {
                        if (self.isShrine == false && self.available && self.costType == CostTypeIndex.Money) //if not shrine, is available, and is not a lunar pod
                        {
                            int InventoryCount = characterBody.inventory.GetItemCount(catalogIndex);
                            if (InventoryCount > 0)
                            {
                                return Interactability.Available;
                            }
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
                TrustyLockpicks.TrustyLockpickFailed attempted = interactableObject.GetComponent<TrustyLockpicks.TrustyLockpickFailed>();
                if (!attempted)
                {
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
                                    purchaseInteraction.SetAvailable(false);
                                    purchaseInteraction.Networkavailable = false;

                                    purchaseInteraction.gameObject.GetComponent<ChestBehavior>().Open();

                                    //purchaseInteraction.cost = 0;
                                    //purchaseInteraction.Networkcost = 0;

                                    purchaseInteraction.onPurchase.Invoke(self);
                                    purchaseInteraction.lastActivator = self;

                                    inventory.RemoveItem(catalogIndex);
                                }
                            }
                        }
                    }
                }
            }
            orig(self, interactableObject);
        }
    }
}
