using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class TrustyLockpicks : Equipment_V2<TrustyLockpicks>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the chance that Trusty Lockpicks will unlock the chest? (Default: 50% chance)", AutoConfigFlags.PreventNetMismatch)]
        public float TrustyLockpicks_UnlockChance { get; private set; } = 50f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much will the price by multiplied by if you fail the unlock? (Default: 200% multiplier)", AutoConfigFlags.PreventNetMismatch)]
        public float TrustyLockpicks_PriceHike { get; private set; } = 2.0f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the Trusty Lockpicks break the lock instead? This prevents it from opening. (Default: false)", AutoConfigFlags.PreventNetMismatch)]
        public bool TrustyLockpicks_KillChest { get; private set; } = false;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds? (Default: 60 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 60f;

        public override string displayName => "Trusty Lockpicks";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            var desc = "Who Needs Keys?\n";
            if (TrustyLockpicks_UnlockChance > 0)
                desc += "Chance to pick chest locks.";
            else desc += "Guaranteed chance to fail to pick a chest's lock.";
            desc += "Can only be used once per lock.";
            return desc;
        }

        protected override string GetDescString(string langid = null)
        {
            var desc = $"";
            // pick chance //
            if (TrustyLockpicks_UnlockChance <= 0) desc += $"<style=cDeath>Guaranteed to fail";
            else desc += $"<style=cIsUtility>{TrustyLockpicks_UnlockChance}% chance";

            // break effect //
            desc += $"</style> to <style=cIsUtility>unlock a chest</style>. <style=cDeath>On fail,</style> it ";
            if (TrustyLockpicks_KillChest)
                desc += $"breaks the chest.";
            // price hike effect //
            else
            {
                // is it equal to 1? //
                if (TrustyLockpicks_PriceHike == 1)
                    desc += $"prevents another attempt.";
                else
                {
                    //  Is it greater than 1? //
                    if (TrustyLockpicks_PriceHike > 1) desc += $"increases";

                    // Is it less than 0? //
                    else if (TrustyLockpicks_PriceHike < 0)
                        desc += $"makes it free";
                    // Is it less than 1? //
                    else 
                        desc += $"decreases";
                    desc += $" the price by {Pct(TrustyLockpicks_PriceHike)}";
                }
            }
            desc += $"</style>";
            return desc;
        }

        protected override string GetLoreString(string langID = null)
        {
            var desc = "These lockpicks have never let the Pilot down, except for";
            if (TrustyLockpicks_UnlockChance <= 0) desc += "every time";
                else desc += "the many times";
            desc += " they did.";
            return desc;
        }

        private readonly string unlockSound = EntityStates.Engi.EngiWeapon.FireMines.throwMineSoundString;
        private readonly GameObject UnlockEffect = Resources.Load<GameObject>("prefabs/effects/LevelUpEffect");
        private readonly GameObject Fail_DestroyEffect = Resources.Load<GameObject>("prefabs/effects/ShieldBreakEffect");
        private readonly GameObject Fail_LockEffect = Resources.Load<GameObject>("prefabs/effects/LevelUpEffectEnemy");
        //prefabs/effects/WarCryEffect
        private readonly string prefix = "BULLETSTORM_";
        private readonly string suffixBroken = " (Failed Unlock)";
        private readonly string contextUnlock = "[EQP] Unlock with Trusty Lockpicks?\n";
        public TrustyLockpicks()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/TrustyLockpicks.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/TrustyLockpicksIcon.png";
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();

            if (ClassicItemsCompat.enabled)
                ClassicItemsCompat.RegisterEmbryo(catalogIndex);
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

            LanguageAPI.Add(prefix + "CHEST1_STEALTHED_NAME", "Cloaked Chest" + suffixBroken);
            LanguageAPI.Add(prefix + "CHEST1_NAME", "Chest"+ suffixBroken);
            LanguageAPI.Add(prefix + "CATEGORYCHEST_HEALING_NAME", "Chest - Healing" + suffixBroken);
            LanguageAPI.Add(prefix + "CATEGORYCHEST_DAMAGE", "Chest - Damage" + suffixBroken);
            LanguageAPI.Add(prefix + "CATEGORYCHEST_UTILITY_NAME", "Chest - Utility" + suffixBroken);
            LanguageAPI.Add(prefix + "CHEST2_NAME", "Large Chest" + suffixBroken);
            LanguageAPI.Add(prefix + "GOLDCHEST_NAME", "Legendary Chest" + suffixBroken);
            LanguageAPI.Add(prefix + "EQUIPMENTBARREL_NAME", "Equipment Barrel" + suffixBroken);

            LanguageAPI.Add(prefix + "CHEST1_STEALTHED_NAME", contextUnlock+"Open cloaked chest");
            LanguageAPI.Add(prefix + "CHEST1_NAME", contextUnlock + "Open chest");
            LanguageAPI.Add(prefix + "CATEGORYCHEST_HEALING_NAME", contextUnlock + "Open Chest - Healing");
            LanguageAPI.Add(prefix + "CATEGORYCHEST_DAMAGE", contextUnlock + "Open Chest - Damage");
            LanguageAPI.Add(prefix + "CATEGORYCHEST_UTILITY_NAME", contextUnlock + "Open Chest - Utility");
            LanguageAPI.Add(prefix + "CHEST2_NAME", contextUnlock + "Open large chest");
            LanguageAPI.Add(prefix + "GOLDCHEST_NAME", contextUnlock + "Open Legendary Chest");
            LanguageAPI.Add(prefix + "EQUIPMENTBARREL_NAME", contextUnlock + "Open equipment barrel");
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
            if (ClassicItemsCompat.enabled && ClassicItemsCompat.CheckEmbryoProc(instance, body)) newUnlockChance *= 1.3f;

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
            if (!purchaseInteraction) return false;
            TrustyLockpickFailed attempted = chestObject.GetComponent<TrustyLockpickFailed>();
            if (attempted) return false;
            if (!interactionDriver) return false;

            GameObject selectedEffect;
            Vector3 offset = Vector3.up * 1f;


            if (!purchaseInteraction.isShrine && purchaseInteraction.available && purchaseInteraction.costType == CostTypeIndex.Money)
            {
                Interactor interactor = interactionDriver.interactor;
                //interactionDriver.interactor.AttemptInteraction(chestObject);
                if (Util.CheckRoll(UnlockChance))
                {
                    purchaseInteraction.SetAvailable(false);
                    purchaseInteraction.Networkavailable = false;

                    purchaseInteraction.gameObject.GetComponent<ChestBehavior>().Open();

                    //purchaseInteraction.cost = 0;
                    //purchaseInteraction.Networkcost = 0;

                    purchaseInteraction.onPurchase.Invoke(interactor);
                    purchaseInteraction.lastActivator = interactor;
                    Util.PlaySound(unlockSound, interactor.gameObject);
                    EffectManager.SimpleEffect(UnlockEffect, chestObject.transform.position + offset, Quaternion.identity, true);

                } else
                {
                    var displaynamecomponent = chestObject.GetComponent<GenericDisplayNameProvider>();
                    if (displaynamecomponent) displaynamecomponent.displayToken = prefix + displaynamecomponent.displayToken;
                    if (TrustyLockpicks_KillChest)
                    {
                        purchaseInteraction.costType = CostTypeIndex.None;
                        purchaseInteraction.SetAvailable(false);

                        //Object.Destroy(purchaseInteraction);
                        //Object.Destroy(highlight);

                        selectedEffect = Fail_DestroyEffect;
                    }
                    else
                    {
                        purchaseInteraction.cost = Mathf.CeilToInt(purchaseInteraction.cost * TrustyLockpicks_PriceHike);
                        purchaseInteraction.Networkcost = purchaseInteraction.cost;
                        selectedEffect = Fail_LockEffect;
                    }
                    purchaseInteraction.displayNameToken = (prefix + purchaseInteraction.displayNameToken);
                    chestObject.AddComponent<TrustyLockpickFailed>();
                    EffectManager.SimpleEffect(selectedEffect, chestObject.transform.position + offset, Quaternion.identity, true);
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
            var gameObject = self.gameObject;
            Highlight highlight = gameObject.GetComponent<Highlight>();
            //if (!highlight) return orig(self, activator);

            TrustyLockpickFailed attempted = gameObject.GetComponent<TrustyLockpickFailed>();
            //if (attempted) return orig(self, activator);
            if (attempted)
            {
                if (highlight) highlight.highlightColor = Highlight.HighlightColor.teleporter;
                return orig(self, activator);
            }

            PurchaseInteraction purchaseInteraction = gameObject.GetComponent<PurchaseInteraction>();

            var nameComponent = gameObject.GetComponent<TrustyLockpickRememberedName>();
            if (!nameComponent) nameComponent = gameObject.AddComponent<TrustyLockpickRememberedName>();

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
                            if (inventory.GetEquipmentRestockableChargeCount(0) > 0)
                            if (highlight) highlight.highlightColor = Highlight.HighlightColor.pickup;
                            if (purchaseInteraction)
                            {
                                var resultContext = (prefix + purchaseInteraction.contextToken);
                                if (nameComponent.oldContext == "") nameComponent.oldContext = purchaseInteraction.contextToken;

                                purchaseInteraction.contextToken = resultContext;
                            }
                            return Interactability.Available;
                        }
                    }
                }
            }
            if (highlight) highlight.highlightColor = Highlight.HighlightColor.interactive;
            if (nameComponent && nameComponent.oldContext != "") purchaseInteraction.contextToken = nameComponent.oldContext;
            return orig(self, activator);
        }

        public class TrustyLockpickFailed : MonoBehaviour
        {

        }
        public class TrustyLockpickRememberedName : MonoBehaviour
        {
            public string oldContext = "";
        }
    }
}
