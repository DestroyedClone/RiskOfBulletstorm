using RoR2;
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
            On.RoR2.ChestBehavior.Start += ChestBehavior_Start_AddRBSChestComponent;
            //On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            On.RoR2.PurchaseInteraction.GetContextString += ModifyContextStringBasedOnEquipment;
            //Stage.onServerStageBegin += Stage_onServerStageBegin;
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

        private static string BarrelInteraction_GetContextString(On.RoR2.BarrelInteraction.orig_GetContextString orig, BarrelInteraction self, Interactor activator)
        {
            var original = orig(self, activator);
            if (self.TryGetComponent(out StoneGateModification.RBSStoneGateLockInteraction gateLock))
            {
                if (RBSChestLockInteraction.InteractorHasValidEquipment(activator))
                {
                    return gateLock.GetContextualString(original);
                }
            }
            return original;
        }

        private static void Stage_onServerStageBegin(Stage stage)
        {
            if (!stage.sceneDef || stage.sceneDef.cachedName != "goolake") return;
            var entrance = GameObject.Find("HOLDER: Secret Ring Area Content/Entrance");
            var GLRuinGate = entrance.transform.Find("GLRuinGate");

            var nameProvider = GLRuinGate.gameObject.AddComponent<GenericDisplayNameProvider>();
            nameProvider.SetDisplayToken("Mysterious Gate");
            var highlight = GLRuinGate.gameObject.AddComponent<Highlight>();
            highlight.displayNameProvider = nameProvider;
            highlight.targetRenderer = GLRuinGate.transform.Find("BbRuinGate_LOD0").GetComponent<MeshRenderer>();
            var interaction = GLRuinGate.gameObject.AddComponent<PurchaseInteraction>();
            interaction.displayNameToken = "Mysterious Gate";
            var pingInfo = GLRuinGate.gameObject.AddComponent<PingInfoProvider>();
            pingInfo.pingIconOverride = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
        }

        private static string ModifyContextStringBasedOnEquipment(On.RoR2.PurchaseInteraction.orig_GetContextString orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);

            if (self.TryGetComponent(out RBSChestLockInteraction chestLock))
            {
                if (RBSChestLockInteraction.InteractorHasValidEquipment(activator))
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
            Highlight highlight = gameObject.GetComponent<Highlight>();

            if (gameObject.TryGetComponent(out RBSChestLockInteraction chestLock))
            {
                if (chestLock.isLockBroken) goto Done;
                if (!RBSLockInteraction.InteractorHasValidEquipment(activator)) goto Done;
                //&& activator.GetComponent<CharacterBody>()?.inputBank?.activateEquipment.justPressed == true)

                chestLock.ChangeHighlightColor(highlight, Highlight.HighlightColor.unavailable);
                if (original != Interactability.Disabled)
                    return Interactability.Available;

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

        private static void ChestBehavior_Start_AddRBSChestComponent(On.RoR2.ChestBehavior.orig_Start orig, ChestBehavior self)
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

        public interface IRBSKeyInteraction
        {
        }

        public class RBSLockInteraction : MonoBehaviour, IRBSKeyInteraction
        {
            public const string attemptContextToken = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT";
            public const string failContextToken = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT_LOSE";
            public const string attemptContextTokenClient = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT_CLIENT";

            public bool isLockBroken = false;

            public string GetContextualString(string original)
            {
                string formattingToken = isLockBroken ? failContextToken : attemptContextToken;
                if (NetworkClient.active)
                {
                    formattingToken = attemptContextTokenClient;
                }
                return Language.GetStringFormatted(formattingToken, original);
            }

            public static bool InteractorHasValidEquipment(Interactor interactor)
            {
                if (interactor.TryGetComponent(out CharacterBody characterBody)
                    && characterBody.equipmentSlot)
                {
                    if (keyTypeEquipmentArray.Contains(characterBody.equipmentSlot.equipmentIndex))
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
        public class RBSChestLockInteraction : RBSLockInteraction
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
                    originalHighlightColor = highlight.highlightColor; highlightColorStored = true;
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