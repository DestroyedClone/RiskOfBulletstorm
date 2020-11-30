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
        public float TrustyLockpicks_UnlockChance { get; private set; } = 50f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Price increase on fail: 200%", AutoConfigFlags.PreventNetMismatch)]
        public float TrustyLockpicks_PriceHike { get; private set; } = 2.0f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Lock the chest instead? Takes priority over increasing the price. Default: false", AutoConfigFlags.PreventNetMismatch)]
        public bool TrustyLockpicks_KillChest { get; private set; } = false;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 60f;

        public override string displayName => "Trusty Lockpicks";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Who Needs Keys?\nChance to pick locks. Can only be used once per lock.";

        protected override string GetDescString(string langid = null)
        {
            string desc = $"<style=cIsUtility>{TrustyLockpicks_UnlockChance}% chance</style> to <style=cIsUtility>unlock a chest</style>. <style=cDeath>On fail,</style> it ";
            if (TrustyLockpicks_KillChest)
                desc += $"breaks the chest.";
            else
                desc += $"increases the price by {Pct(TrustyLockpicks_PriceHike)}";
            desc += $"</style>";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "These lockpicks have never let the Pilot down, except for the many times they did.";

        private readonly PickupIndex BFGPickupIndex = PickupCatalog.FindPickupIndex(EquipmentIndex.BFG);
        private readonly PickupIndex SyringePickupIndex = PickupCatalog.FindPickupIndex(ItemIndex.Syringe);
        private readonly string unlockSound = EntityStates.Engi.EngiWeapon.FireMines.throwMineSoundString;
        public TrustyLockpicks()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/TrustyLockpicks.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/TrustyLockpicksIcon.png";
        }
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
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
        }

        public override void Uninstall()
        {
            base.Uninstall();
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

            float newUnlockChance = TrustyLockpicks_UnlockChance;
            if (instance.CheckEmbryoProc(body)) newUnlockChance *= 1.3f;

            //interactionDriver.interactor.interactableCooldown = 0.25f;

            if (AttemptUnlock(BestInteractableObject, interactionDriver, newUnlockChance)) 
            {
                return true;
            } else
            {
                return false;
            }
        }

        private bool AttemptUnlock(GameObject chestObject, InteractionDriver interactionDriver, float UnlockChance)
        {
            Highlight highlight = chestObject.GetComponent<Highlight>();
            PurchaseInteraction purchaseInteraction = chestObject.GetComponent<PurchaseInteraction>();
            if (!highlight) return false;
            //if (highlight.pickupIndex != BFGPickupIndex) return false;
            if (!purchaseInteraction) return false;
            TrustyLockpickFailed attempted = chestObject.GetComponent<TrustyLockpickFailed>();
            if (attempted) return false;

            if (!purchaseInteraction.isShrine && purchaseInteraction.available && purchaseInteraction.costType == CostTypeIndex.Money)
            {
                Interactor interactor = interactionDriver.interactor;
                //interactionDriver.interactor.AttemptInteraction(chestObject);
                Chat.AddMessage("Rolled: "+ UnlockChance);
                if (Util.CheckRoll(UnlockChance))
                {
                    purchaseInteraction.SetAvailable(false);
                    purchaseInteraction.Networkavailable = false;

                    purchaseInteraction.gameObject.GetComponent<ChestBehavior>().Open();

                    //purchaseInteraction.cost = 0;
                    //purchaseInteraction.Networkcost = 0;

                    purchaseInteraction.onPurchase.Invoke(interactor);
                    purchaseInteraction.lastActivator = interactor;
                    Util.PlaySound(unlockSound, RoR2Application.instance.gameObject);

                } else
                {
                    if (TrustyLockpicks_KillChest)
                    {
                        Object.Destroy(purchaseInteraction);
                    }
                    else
                    {
                        purchaseInteraction.cost = Mathf.CeilToInt(purchaseInteraction.cost * TrustyLockpicks_PriceHike);
                        purchaseInteraction.Networkcost = purchaseInteraction.cost;
                        chestObject.AddComponent<TrustyLockpickFailed>();
                    }
                }
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
            if (h_pickupIndex == BFGPickupIndex) return orig(self, activator);

            TrustyLockpickFailed attempted = self.gameObject.GetComponent<TrustyLockpickFailed>();
            if (attempted) return orig(self, activator);

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

        public class TrustyLockpickFailed : MonoBehaviour
        {

        }
    }
}
