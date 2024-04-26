using BepInEx;
using R2API;
using RoR2;
using System;
using System.ComponentModel;
using System.Globalization;
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

        public static GameObject EquipmentPreviewPrefab;
        public static void Init()
        {
            On.RoR2.ChestBehavior.Awake += ChestBehavior_Awake;
            //On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            //On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability2NoHighlight;
            On.RoR2.PurchaseInteraction.GetInteractability += ShowEquipmentPrefabIfValid;
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

            var go = new GameObject();
            EquipmentPreviewPrefab = PrefabAPI.InstantiateClone(go, "RBS_EquipmentPreviewPrefab", false);
            UnityEngine.Object.Destroy(go);
            var a = EquipmentPreviewPrefab.AddComponent<PickupDisplay>();
            var b = EquipmentPreviewPrefab.AddComponent<PickupDisplayDestroyIfOwnerAway>();
            b.pickupDisplay = a;
        }

        private static Interactability ShowEquipmentPrefabIfValid(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);
            var userEquipmentIndex = EquipmentIndex.None;
            if (self.gameObject.TryGetComponent(out RBSChestLockInteraction chestLock))
            {
                RBSBaseLockInteraction.InteractorHasValidEquipment(activator, out userEquipmentIndex);
            }
            if (chestLock)
                chestLock.UpdateItemDisplay(userEquipmentIndex, activator);
            return original;
        }

        private class PickupDisplayDestroyIfOwnerAway : MonoBehaviour
        {
            public CharacterBody owner;
            public PickupDisplay pickupDisplay;
            public Transform baseTransform;
            public float maxDistance = 100;

            public float stopwatch = 1;
            public float duration = 1;

            public void FixedUpdate()
            {
                stopwatch -= Time.fixedDeltaTime;
                if (stopwatch > 0)
                {
                    return;
                }
                stopwatch = duration;
                if (!owner)
                {
                    Destroy(gameObject);
                    return;
                }
                if (Vector3.Distance(transform.position, owner.inputBank.aimOrigin) > maxDistance)
                {
                    Destroy(gameObject);
                }
            }
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
            var userEquipmentIndex = EquipmentIndex.None;
            if (self.gameObject.TryGetComponent(out RBSChestLockInteraction chestLock))
            {
                if (chestLock.isLockBroken) goto Done;
                if (!RBSBaseLockInteraction.InteractorHasValidEquipment(activator, out userEquipmentIndex)) goto Done;
                //return Interactability.Available;
                return original;
            }
            Done: 
            if (chestLock)
                chestLock.UpdateItemDisplay(userEquipmentIndex, activator);
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
                Highlight.HighlightColor chosenColor = chestLock.originalHighlightColor;
                if (chestLock.isLockBroken) goto RestoreOriginalHighlight;
                if (!RBSBaseLockInteraction.InteractorHasValidEquipment(activator, out EquipmentIndex validEquipmentIndex)) goto RestoreOriginalHighlight;
                //&& activator.GetComponent<CharacterBody>()?.inputBank?.activateEquipment.justPressed == true)

                //chestLock.ChangeHighlightColor(highlight, Highlight.HighlightColor.unavailable);
                if (original != Interactability.Disabled)
                    return Interactability.Available;

                RestoreOriginalHighlight:
                chestLock.ChangeHighlightColor(highlight, chosenColor);

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

        /// <summary>
        /// Component for the purpose of storing information regarding things that have a locked condition.
        /// <para><b>Trusty Lockpicks</b>: Affects hasUsedLockpicks </para>
        /// <para><b>Drill</b>: Can't interact with Lockpicked Chests</para>
        /// </summary>
        public class RBSBaseLockInteraction : MonoBehaviour, IRBSKeyInteraction
        {
            public const string attemptContextToken = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT";
            public const string failContextToken = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT_LOSE";
            public const string attemptContextTokenClient = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT_CLIENT";

            public bool isLockBroken = false;

            //client
            public GameObject itemDisplayInstance = null;
            //public GameObject oldDisplayInstance = null;
            public EquipmentIndex displayEquipment = EquipmentIndex.None;
            public Transform displayTransform = null;

            public PurchaseInteraction purchaseInteraction;

            public void Awake()
            {
                var go = new GameObject("displaytransform");
                go.transform.parent = transform;
                go.transform.localPosition = Vector3.up * 4;
                displayTransform = go.transform;
            }

            public void OnEnable()
            {
            }

            public void UpdateItemDisplay(EquipmentIndex equipmentIndex, Interactor interactor)
            {
                if (!interactor) return;
                displayEquipment = equipmentIndex;
                if (itemDisplayInstance)
                {
                    if (EquipmentIsValid(displayEquipment))
                        itemDisplayInstance.GetComponent<PickupDisplay>().SetPickupIndex(PickupCatalog.FindPickupIndex(displayEquipment));
                    else
                        DestroyItemDisplay();
                    return;
                }
                CreateItemDisplay(interactor);

                return;
                if (displayEquipment == equipmentIndex) return;
                displayEquipment = equipmentIndex;
                if (itemDisplayInstance)
                    DestroyItemDisplay();
                CreateItemDisplay(interactor);
            }

            private void DestroyItemDisplay()
            {
                if (!itemDisplayInstance) return;
                Destroy(itemDisplayInstance);
                itemDisplayInstance = null;
            }

            private void CreateItemDisplay(Interactor interactor)
            {
                //if (displayEquipment == EquipmentIndex.None) return;
                itemDisplayInstance = UnityEngine.Object.Instantiate(EquipmentPreviewPrefab, displayTransform);
                itemDisplayInstance.GetComponent<PickupDisplay>().SetPickupIndex(PickupCatalog.FindPickupIndex(displayEquipment));
                var dest = itemDisplayInstance.GetComponent<PickupDisplayDestroyIfOwnerAway>();
                dest.owner = interactor.gameObject.GetComponent<CharacterBody>();
                dest.maxDistance = interactor.maxInteractionDistance;
                dest.baseTransform = GetComponent<ModelLocator>().modelTransform;
                itemDisplayInstance.transform.localPosition = Vector3.zero;
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

            public static bool EquipmentIsValid(EquipmentIndex equipmentIndex)
            {
                return keyTypeEquipmentArray.Contains(equipmentIndex);
            }
        }

        public class RBSChestLockInteraction : RBSBaseLockInteraction
        {
            //public string nameModifier = "";
            //public string contextModifier = "";

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
    }
}