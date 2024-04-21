using RoR2;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class SharedComponents
    {
        public static EquipmentIndex[] keyTypeEquipmentArray;
        public static UnityAction<BarrelInteraction, Interactor> onBarrelInteraction;
        public static void Init()
        {
            On.RoR2.ChestBehavior.Awake += ChestBehavior_Awake;
            //On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability2NoHighlight;
            On.RoR2.PurchaseInteraction.GetContextString += ModifyContextStringBasedOnEquipment;
            StoneGateModification.Init();
            EquipmentCatalog.availability.CallWhenAvailable(() =>
            {
                keyTypeEquipmentArray = new EquipmentIndex[]
                {
                    Equipment.TrustyLockpicks.Instance.EquipmentDef.equipmentIndex,
                    Equipment.Drill.Instance.EquipmentDef.equipmentIndex
                };
            });
            On.RoR2.BarrelInteraction.GetContextString += BarrelInteraction_GetContextString;

        }

        private static void ChestBehavior_Awake(On.RoR2.ChestBehavior.orig_Awake orig, ChestBehavior self)
        {
            orig(self);
            PurchaseInteraction purchaseInteraction = self.GetComponent<PurchaseInteraction>();
            if (purchaseInteraction.costType != CostTypeIndex.PercentHealth)
            {
                var comp = self.gameObject.AddComponent<RBSChestLockInteraction>();
                comp.chestBehavior = self;
                comp.purchaseInteraction = purchaseInteraction;
                comp.StoreHighlightColor(self.GetComponent<Highlight>());
            }
        }

        private static Interactability PurchaseInteraction_GetInteractability2NoHighlight(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);
            if (self.gameObject.TryGetComponent(out RBSChestLockInteraction chestLock))
            {
                if (chestLock.isLockBroken) goto Done;
                if (!RBSBaseLockInteraction.InteractorHasValidEquipment(activator, out EquipmentIndex validEquipmentIndex)) goto Done;
                chestLock.UpdateItemDisplay(validEquipmentIndex);
                return Interactability.Available;
            }
            Done: 
            if (chestLock)
                chestLock.UpdateItemDisplay(EquipmentIndex.None);
            return original;
        }

        private static string BarrelInteraction_GetContextString(On.RoR2.BarrelInteraction.orig_GetContextString orig, BarrelInteraction self, Interactor activator)
        {
            var original = orig(self, activator);
            if (self.TryGetComponent(out StoneGateModification.RBSStoneGateLockInteraction gateLock))
            {
                if (RBSChestLockInteraction.InteractorHasValidEquipment(activator, out EquipmentIndex validEquipmentIndex))
                {
                    return gateLock.GetContextualString(original);
                }
            }
            return original;
        }

        private static string ModifyContextStringBasedOnEquipment(On.RoR2.PurchaseInteraction.orig_GetContextString orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);

            if (self.TryGetComponent(out RBSChestLockInteraction chestLock))
            {
                if (RBSChestLockInteraction.InteractorHasValidEquipment(activator, out EquipmentIndex validEquipmentIndex))
                {
                    return chestLock.GetContextualString(original);
                }
            }

            return original;
        }

        private static Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);
            var gameObject = self.gameObject;
            Highlight highlight = self.GetComponent<Highlight>();

            if (gameObject.TryGetComponent(out RBSChestLockInteraction chestLock))
            {
                if (chestLock.isLockBroken) goto RestoreOriginalHighlight;
                if (!RBSBaseLockInteraction.InteractorHasValidEquipment(activator, out EquipmentIndex validEquipmentIndex)) goto RestoreOriginalHighlight;
                //&& activator.GetComponent<CharacterBody>()?.inputBank?.activateEquipment.justPressed == true)

                //chestLock.ChangeHighlightColor(highlight, Highlight.HighlightColor.unavailable);
                if (original != Interactability.Disabled)
                    return Interactability.Available;

                RestoreOriginalHighlight:
                chestLock.ChangeHighlightColor(highlight, chestLock.originalHighlightColor);

                goto Done;
            }

            if (gameObject.TryGetComponent(out StoneGateModification.RBSStoneGateLockInteraction stoneGateLock))
            {
                if (stoneGateLock.isLockBroken) goto Done;
            }
        Done:
            //bulletstormChestInteractor.ChangeHighlightColor(highlight, bulletstormChestInteractor.originalHighlightColor);
            return original;
        }

        public interface IRBSKeyInteraction
        {
        }

        public class RBSBaseLockInteraction : MonoBehaviour, IRBSKeyInteraction
        {
            public const string attemptContextToken = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT";
            public const string failContextToken = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT_LOSE";
            public const string attemptContextTokenClient = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT_CLIENT";

            public bool isLockBroken = false;

            //client
            public GameObject itemDisplayInstance = null;
            public GameObject oldDisplayInstance = null;
            public EquipmentIndex displayEquipment = EquipmentIndex.None;
            public Transform displayTransform = null;

            public void OnEnable()
            {
                displayTransform = transform;
            }

            public void UpdateItemDisplay(EquipmentIndex equipmentIndex)
            {
                if (displayEquipment == equipmentIndex) return;
                displayEquipment = equipmentIndex;
                if (itemDisplayInstance)// && some condition )
                {
                    DestroyItemDisplay();
                }
                CreateItemDisplay();
            }

            public void DestroyItemDisplay()
            {
                if (!itemDisplayInstance) return;
                oldDisplayInstance = itemDisplayInstance;
                Destroy(oldDisplayInstance);
                itemDisplayInstance = null;
            }

            public void CreateItemDisplay()
            {
                if (displayEquipment == EquipmentIndex.None) return;
                var equipmentDef = EquipmentCatalog.GetEquipmentDef(displayEquipment);
                var display = equipmentDef.pickupModelPrefab;
                itemDisplayInstance = UnityEngine.Object.Instantiate(display, transform);
                itemDisplayInstance.transform.position = displayTransform.position;
            }

            public string GetContextualString(string original)
            {
                string formattingToken = isLockBroken ? failContextToken : attemptContextToken;
                if (NetworkClient.active)
                {
                    formattingToken = attemptContextTokenClient;
                }
                return Language.GetStringFormatted(formattingToken, original);
            }

            public static bool InteractorHasValidEquipment(Interactor interactor, out EquipmentIndex equipmentIndex)
            {
                equipmentIndex = EquipmentIndex.None;
                if (interactor.TryGetComponent(out CharacterBody characterBody)
                    && characterBody.equipmentSlot)
                {
                    equipmentIndex = characterBody.equipmentSlot.equipmentIndex;
                    if (keyTypeEquipmentArray.Contains(equipmentIndex))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Component for the purpose of storing information regarding chests.
        /// <para><b>Trusty Lockpicks</b>: Affects hasUsedLockpicks </para>
        /// <para><b>Drill</b>: Can't interact with Lockpicked Chests</para>
        /// </summary>
        public class RBSChestLockInteraction : RBSBaseLockInteraction
        {
            //public string nameModifier = "";
            //public string contextModifier = "";
            public PurchaseInteraction purchaseInteraction;

            public ChestBehavior chestBehavior;

            public Highlight.HighlightColor originalHighlightColor = Highlight.HighlightColor.interactive;
            private bool highlightColorStored = false;

            public void StoreHighlightColor(Highlight highlight)
            {
                if (!highlight) return;
                if (!highlightColorStored)
                {
                    originalHighlightColor = highlight.highlightColor; 
                    highlightColorStored = true;
                }
            }

            public void ChangeHighlightColor(Highlight highlight, Highlight.HighlightColor highlightColor)
            {
                highlight.highlightColor = highlightColor;
            }

            public void UpdateTokens()
            {
            }
        }

        public class RBSDestroyGameObjectDistance : MonoBehaviour
        {
            public int distance = 5;

            private float stopwatch = 5;
            public float cooldown;

            public void FixedUpdate()
            {
                stopwatch -= Time.fixedDeltaTime;
                if (stopwatch < 0)
                {
                    stopwatch = cooldown;

                }
            }
        }
    }
}