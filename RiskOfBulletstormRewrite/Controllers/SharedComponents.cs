using RoR2;
using System.Linq;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class SharedComponents
    {
        public static void Init()
        {
            On.RoR2.ChestBehavior.Start += ChestBehavior_Start;
            //On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            On.RoR2.PurchaseInteraction.GetContextString += PurchaseInteraction_GetContextString;
        }
        private static string PurchaseInteraction_GetContextString(On.RoR2.PurchaseInteraction.orig_GetContextString orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);

            BulletstormChestInteractorComponent bulletstormChestInteractor = self.GetComponent<BulletstormChestInteractorComponent>();
            if (bulletstormChestInteractor)
            {
                if (bulletstormChestInteractor.InteractorHasValidEquipment(activator))
                {
                    return bulletstormChestInteractor.GetContextualString(original);
                }
            }

            return original;
        }

        private static Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);
            var gameObject = self.gameObject;
            Highlight highlight = gameObject.GetComponent<Highlight>();
            BulletstormChestInteractorComponent bulletstormChestInteractor = gameObject.GetComponent<BulletstormChestInteractorComponent>();

            if (bulletstormChestInteractor
                && bulletstormChestInteractor.hasUsedLockpicks
                && bulletstormChestInteractor.InteractorHasValidEquipment(activator)
                && activator.GetComponent<CharacterBody>()?.inputBank?.activateEquipment.justPressed == true)
            {
                bulletstormChestInteractor.ChangeHighlightColor(highlight, Highlight.HighlightColor.unavailable);
                if (original != Interactability.Disabled)
                    return Interactability.Available;
            }

            bulletstormChestInteractor.ChangeHighlightColor(highlight, bulletstormChestInteractor.originalHighlightColor);
            return original;
        }


        private static void ChestBehavior_Start(On.RoR2.ChestBehavior.orig_Start orig, ChestBehavior self)
        {
            orig(self);
            var comp = self.gameObject.AddComponent<BulletstormChestInteractorComponent>();
            comp.chestBehavior = self;
            comp.purchaseInteraction = self.GetComponent<PurchaseInteraction>();
            comp.StoreHighlightColor(self.GetComponent<Highlight>());
        }

        /// <summary>
        /// Component for the purpose of storing information regarding chests.
        /// <para><b>Trusty Lockpicks</b>: Affects hasUsedLockpicks </para>
        /// <para><b>Drill</b>: Can't interact with Lockpicked Chests</para>
        /// </summary>
        public class BulletstormChestInteractorComponent : MonoBehaviour
        {
            public bool hasUsedLockpicks = false;
            //public string nameModifier = "";
            //public string contextModifier = "";
            public PurchaseInteraction purchaseInteraction;
            public ChestBehavior chestBehavior;

            public Highlight.HighlightColor originalHighlightColor = Highlight.HighlightColor.interactive;
            bool highlightColorStored = false;

            public void StoreHighlightColor(Highlight highlight)
            {
                if (!highlight) return;
                if (!highlightColorStored)
                {
                    originalHighlightColor = highlight.highlightColor; highlightColorStored = true;
                }
            }

            public void ChangeHighlightColor(Highlight highlight, Highlight.HighlightColor highlightColor)
            {
                highlight.highlightColor = highlightColor;
            }

            private readonly string attemptContextToken = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT";
            private readonly string failContextToken = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT_LOSE";

            public static EquipmentIndex[] allowedEquips = new EquipmentIndex[]
            {
                Equipment.TrustyLockpicks.Instance.EquipmentDef.equipmentIndex,
                Equipment.Drill.Instance.EquipmentDef.equipmentIndex
            };

            public bool InteractorHasValidEquipment(Interactor interactor)
            {
                if (interactor.TryGetComponent(out CharacterBody characterBody)
                    && characterBody.equipmentSlot)
                {
                    if (allowedEquips.Contains(characterBody.equipmentSlot.equipmentIndex))
                    {
                        return true;
                    }
                }
                return false;
            }

            public string GetContextualString(string original)
            {
                string formattingToken = hasUsedLockpicks ? failContextToken : attemptContextToken;
                return Language.GetStringFormatted(formattingToken, original);
            }

            public void UpdateTokens()
            {
            }
        }
    }
}