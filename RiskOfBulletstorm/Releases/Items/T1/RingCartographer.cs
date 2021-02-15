using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using System.Linq;
using static RiskOfBulletstorm.Utils.HelperUtil;

namespace RiskOfBulletstorm.Items
{
    public class RingCartographer : Item_V2<RingCartographer>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the base chance for the stage to be scanned with one Cartographer's Ring? (Value: Direct Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float CartographerRing_ScanChance { get; private set; } = 10f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is that chance for additional Cartographer's Ring for stage to scan? (Value: Direct Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float CartographerRing_ScanChanceStack { get; private set; } = 5f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How long should the scan last in seconds? (Setting it to zero automatically sets it to 27 hours)", AutoConfigFlags.PreventNetMismatch)]
        public float CartographerRing_ScanDuration { get; private set; } = 0f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the Cartographer's Ring continue to pulse scans after the stage starts?", AutoConfigFlags.PreventNetMismatch)]
        public bool CartographerRing_KeepScanningPastStart { get; private set; } = false;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the scanners be destroyed upon starting the teleporter?", AutoConfigFlags.PreventNetMismatch)]
        public bool CartographerRing_DestroyOnTeleporterStart { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("When scanning the stage, hould indicators on interactables be hidden after using them?", AutoConfigFlags.PreventNetMismatch)]
        public bool CartographerRing_HideNotif { get; private set; } = true;

        public override string displayName => "Cartographer's Ring";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "<b>Some Floors Are Familiar</b>\nSometimes reveals the floor.";
        protected override string GetDescString(string langid = null) => $"<style=cIsUtility>{CartographerRing_ScanChance}% chance of revealing all interactables upon stage start.</style>" +
            $"\n <style=cStack>(+{CartographerRing_ScanChanceStack}% per stack)</style>." +
            $"\n <style=cSub>Chance is shared amongst players.</style>";

        protected override string GetLoreString(string langID = null) => "The Gungeon is unmappable, but it was not always so. It is said that in his youth, the great cartographer Woban has created four great maps, one for each floor of the Gungeon. While working on the fifth and final map, the walls suddenly began to shift strangely; they continue to do so to this day.";

        public static GameObject PermanentScannerPrefab;

        public static GameObject ItemBodyModelPrefab;
        public RingCartographer()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/RingCartographer.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/RingCartographer.png"; //todo fix in unity
        }


        public override void SetupBehavior()
        {
            base.SetupBehavior();
            PermanentScannerPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/NetworkedObjects/ChestScanner"), "Bulletstorm_ChestScanner");
            ChestRevealer chestRevealer = PermanentScannerPrefab.GetComponent<ChestRevealer>();
            chestRevealer.radius = 1000;
            chestRevealer.pulseTravelSpeed = 1000;
            chestRevealer.revealDuration = 1;
            chestRevealer.pulseInterval = 10;
            chestRevealer.pulseEffectScale = 0;
            chestRevealer.pulseEffectPrefab = null; //light mode users

            if (CartographerRing_ScanDuration <= 0)
            {
                chestRevealer.revealDuration = 99999; //~27 hours
            }
            else
            {
                chestRevealer.revealDuration = Mathf.Max(1, CartographerRing_ScanDuration);
            }

            DestroyOnTimer destroyOnTimer = PermanentScannerPrefab.GetComponent<DestroyOnTimer>();

            if (CartographerRing_KeepScanningPastStart)
            {
                //UnityEngine.Object.Destroy(destroyOnTimer);
                destroyOnTimer.duration = 99999;
            }
            else
            {
                destroyOnTimer.duration = CartographerRing_ScanDuration;
            }

            if (PermanentScannerPrefab) PrefabAPI.RegisterNetworkPrefab(PermanentScannerPrefab);
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>(modelResourcePath);
                displayRules = GenerateItemDisplayRules();
            }

            base.SetupAttributes();
        }
        public static ItemDisplayRuleDict GenerateItemDisplayRules() //THIS SUCKS
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.01f, 0.01f, 0.01f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Finger42R",
localPos = new Vector3(-0.0005F, -0.0148F, -0.0165F),
localAngles = new Vector3(10F, 1F, 170F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Finger42R",
localPos = new Vector3(-0.0313F, -0.0297F, -0.0205F),
localAngles = new Vector3(46.8051F, 359.0542F, 190.2538F),
localScale = new Vector3(0.012F, 0.01F, 0.012F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[] // TODO
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleNailgun",
                    localPos = new Vector3(0f, 3.4f, -1.3f),
                    localAngles = new Vector3(60f, 0f, 0f),
                    localScale = generalScale * 16f
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleSpear",
                    localPos = new Vector3(0f, 0.5f, 0.22f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleBuzzsaw",
                    localPos = new Vector3(0f, 0.5f, 0.22f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Finger42R",
localPos = new Vector3(0.0308F, 0.0142F, 0.0086F),
localAngles = new Vector3(338.9612F, 358.592F, 173.5894F),
localScale = new Vector3(0.0105F, 0.0105F, 0.0105F)
                },
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger42R",
                    localPos = new Vector3(0.03f, -0.02f, 0.01f),
                    localAngles = new Vector3(90f, 5f, 0f),
                    localScale = new Vector3(0.011f, 0.011f, 0.011f)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger42R",
                    localPos = new Vector3(-0.0035f, 0.04f, -0.05f),
                    localAngles = new Vector3(270f, 0f, 0f),
                    localScale = new Vector3(0.011f, 0.01f, 0.01f)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[] //todo
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "WeaponPlatform",
                    localPos = new Vector3(0f, 1.2f, 0.2f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = generalScale * 8f
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "MechFinger33R",
localPos = new Vector3(-0.15F, -0.0141F, 0.02F),
localAngles = new Vector3(0F, 0F, 180F),
localScale = new Vector3(0.03F, 0.03F, 0.03F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger31L",
                    localPos = new Vector3(-0.3f, 0.3f, 0f),
                    localAngles = new Vector3(-90f, 90f, 0f),
                    localScale = generalScale * 25
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger42R",
                    localPos = new Vector3(-0.0035f, 0.01f, -0.001f),
                    localAngles = new Vector3(255f, 195f, 180f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 0.18f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandL",
localPos = new Vector3(0.0489F, 0.1243F, 0.016F),
localAngles = new Vector3(50F, 10F, 6.6411F),
localScale = new Vector3(0.0127F, 0.0127F, 0.0127F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Muzzle",
                    localPos = new Vector3(0.13f, 0.45f, -2.8f),
                    localAngles = new Vector3(35f, 0f, 270f),
                    localScale = generalScale * 3f
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(-0.7f, 2.3f, 0.1f),
                    localAngles = new Vector3(270f, 90f, 0f),
                    localScale = generalScale * 20f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Finger32L",
localPos = new Vector3(0.6733F, -0.3061F, -0.0698F),
localAngles = new Vector3(341.6847F, 277.146F, 167.8782F),
localScale = new Vector3(0.2583F, 0.2601F, 0.2583F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "HandL",
                localPos = new Vector3(-0.1F, -0.4034F, 0.032F),
                localAngles = new Vector3(281.6431F, 228.4214F, 219.9374F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            return rules;
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            Stage.onServerStageComplete += StageEnd_DestroyComponent;
            if (CartographerRing_DestroyOnTeleporterStart)
                TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterCharged_DestroyComponent;
            if (CartographerRing_HideNotif)
                GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;
        }

        private void GlobalEventManager_OnInteractionsGlobal(Interactor interactor, IInteractable interactable, GameObject gameObject)
        {
            var comp = gameObject.GetComponent<ChestRevealer.RevealedObject>();
            if (comp)
                comp.enabled = false;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            Stage.onStageStartGlobal -= Stage_onStageStartGlobal;
            Stage.onServerStageComplete -= StageEnd_DestroyComponent;
            if (CartographerRing_DestroyOnTeleporterStart)
                TeleporterInteraction.onTeleporterBeginChargingGlobal -= TeleporterCharged_DestroyComponent;
            if (CartographerRing_HideNotif)
                GlobalEventManager.OnInteractionsGlobal -= GlobalEventManager_OnInteractionsGlobal;
        }

        private void TeleporterCharged_DestroyComponent(TeleporterInteraction obj)
        {
            DestroyIndicators();
        }
        private void StageEnd_DestroyComponent(Stage obj)
        {
            DestroyIndicators();
        }

        private void DestroyIndicators()
        {
            if (PermanentScannerPrefab)
            {
                //https://stackoverflow.com/questions/604831/collection-was-modified-enumeration-operation-may-not-execute
                foreach (var comp in ChestRevealer.RevealedObject.currentlyRevealedObjects.ToList())
                {
                    comp.Value.enabled = false;
                }
            }
        }

        //ty Ghor for the hook
        private void Stage_onStageStartGlobal(Stage obj)
        {
            if (NetworkServer.active)
            {
                int InventoryCount = Util.GetItemCountForTeam(TeamIndex.Player, catalogIndex, true, true);

                var ResultChance = InventoryCount > 0 ? (CartographerRing_ScanChance + CartographerRing_ScanChanceStack * (InventoryCount - 1)) : 0;
                if (Util.CheckRoll(ResultChance))
                {
                    var clone = UnityEngine.Object.Instantiate(PermanentScannerPrefab);
                    NetworkServer.Spawn(clone);
                }
            }
        }
    }
}
