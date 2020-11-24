using EntityStates.Treebot.Weapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;
using ThinkInvisible.ClassicItems;

namespace RiskOfBulletstorm.Items
{
    public class TrustyLockpicks : Equipment_V2<TrustyLockpicks>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Unlock chance: 50%", AutoConfigFlags.PreventNetMismatch)]
        public float TrustyLockpicks_UnlockChance { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Price increase on fail: 200%", AutoConfigFlags.PreventNetMismatch)]
        public float TrustyLockpicks_PriceHike { get; private set; } = 2.0f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public float TrustyLockpicks_Cooldown { get; private set; } = 8f;

        public override string displayName => "Portable Table Device";
        public override float cooldown { get; protected set; } = 35f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Who Needs Keys?\nChance to pick locks. Can only be used once per lock.";

        protected override string GetDescString(string langid = null) => $"{Pct(TrustyLockpicks_UnlockChance)} to unlock a chest. If it fails, it increases the price by {Pct(TrustyLockpicks_PriceHike)}";

        protected override string GetLoreString(string langID = null) => "These lockpicks have never let the Pilot down, except for the many times they did.";

        private readonly PickupIndex BFGPickupIndex = PickupCatalog.FindPickupIndex(EquipmentIndex.BFG);
        private readonly PickupIndex SyringePickupIndex = PickupCatalog.FindPickupIndex(ItemIndex.Syringe);

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            Embryo_V2.instance.Compat_Register(catalogIndex);
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
            //On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            //On.RoR2.Interactor.PerformInteraction -= Interactor_PerformInteraction;
            On.RoR2.PurchaseInteraction.GetInteractability -= PurchaseInteraction_GetInteractability;
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            InteractionDriver interactionDriver = body.GetComponent<InteractionDriver>();
            if (!interactionDriver) return false;
            GameObject BestInteractableObject = interactionDriver.FindBestInteractableObject();
            if (!BestInteractableObject) return false;
            PurchaseInteraction purchaseInteraction = BestInteractableObject.GetComponent<PurchaseInteraction>();
            if (!purchaseInteraction) return false;

            //interactionDriver.interactor.interactableCooldown = 0.25f;

            if (!AttemptUnlock(BestInteractableObject, interactionDriver))
            {
                if (instance.CheckEmbryoProc(body))
                {
                    AttemptUnlock(BestInteractableObject, interactionDriver);
                    Util.PlaySound(EntityStates.Engi.EngiWeapon.FireMines.throwMineSoundString, body.gameObject);
                }
            } else
            {
                Util.PlaySound(EntityStates.Engi.EngiWeapon.FireMines.throwMineSoundString, body.gameObject);
            }
            return true;
        }

        private bool AttemptUnlock(GameObject chestObject, InteractionDriver interactionDriver)
        {
            Highlight highlight = chestObject.GetComponent<Highlight>();
            PurchaseInteraction purchaseInteraction = chestObject.GetComponent<PurchaseInteraction>();
            if (!highlight) return false;
            if (highlight.pickupIndex != BFGPickupIndex) return false;
            if (!purchaseInteraction) return false;
            if (!purchaseInteraction.isShrine && purchaseInteraction.available && purchaseInteraction.costType == CostTypeIndex.Money)
            {
                interactionDriver.interactor.AttemptInteraction(chestObject);
                return true;
            }
            else
            {
                return false;
            }
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            Highlight highlight = self.gameObject.GetComponent<Highlight>();
            if (!highlight) return orig(self, activator);

            PickupIndex h_pickupIndex = highlight.pickupIndex;
            if (h_pickupIndex != BFGPickupIndex) return orig(self, activator);

            CharacterBody characterBody = activator.GetComponent<CharacterBody>();
            if (characterBody)
            {
                Inventory inventory = characterBody.inventory;
                if (inventory)
                {
                    if (self.isShrine == false && self.available && self.costType == CostTypeIndex.Money) //if not shrine, is available, and is not a lunar pod
                    {
                        EquipmentIndex equipmentIndex = inventory.GetEquipmentIndex();
                        if (equipmentIndex == catalogIndex)
                        {
                            highlight.pickupIndex = BFGPickupIndex;
                            return Interactability.Available;
                        }
                    }
                }
            }
            highlight.pickupIndex = SyringePickupIndex;
            return orig(self, activator);
        }

        private void Interactor_PerformInteraction(On.RoR2.Interactor.orig_PerformInteraction orig, Interactor self, GameObject interactableObject)
        {
            Highlight highlight = interactableObject.GetComponent<Highlight>();
            if (!highlight)
            {
                orig(self, interactableObject);
            }

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
                        EquipmentIndex equipmentIndex = inventory.GetEquipmentIndex();
                        if (equipmentIndex == catalogIndex)
                        {
                            purchaseInteraction.GetInteractability(self);

                            if (purchaseInteraction.isShrine == false && purchaseInteraction.available && purchaseInteraction.costType == CostTypeIndex.Money ) //if not shrine, is available, and is not a lunar pod
                            {
                                purchaseInteraction.SetAvailable(false);
                                purchaseInteraction.Networkavailable = false;

                                purchaseInteraction.gameObject.GetComponent<ChestBehavior>().Open();

                                purchaseInteraction.cost = 0;
                                purchaseInteraction.Networkcost = 0;

                                purchaseInteraction.onPurchase.Invoke(self);
                                purchaseInteraction.lastActivator = self;

                                inventory.SetEquipmentIndex(EquipmentIndex.None);
                            }
                        }
                    }
                }
            }
            orig(self, interactableObject);
        }
    }
}
