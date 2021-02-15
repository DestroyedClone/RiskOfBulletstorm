using RiskOfBulletstorm.Utils;
using System.Collections;
using System.Collections.Generic;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class PortableBarrelDevice : Equipment_V2<PortableBarrelDevice>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How long should the barrel stay around after being spawned?", AutoConfigFlags.PreventNetMismatch)]
        public static float PortableTableDevice_Lifetime { get; private set; } = 16;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How long should the barrel stay around after being opened?", AutoConfigFlags.PreventNetMismatch)]
        public static float PortableTableDevice_UseLifetime { get; private set; } = 3f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Max barrels per player?", AutoConfigFlags.PreventNetMismatch)]
        public static int PortableTableDevice_MaxBarrelsPerPlayer { get; private set; } = 20;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 30f;

        public override string displayName => "Portable Barrel Device";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            var desc = "<b>Know When To Fold 'Em</b>\n";
            if (PortableTableDevice_MaxBarrelsPerPlayer > 0 && PortableTableDevice_Lifetime > 0)
                desc += "Places a barrel.";
            else desc += "Faulty user input prevents this device from functioning.";
            return desc;
        }

        protected override string GetDescString(string langid = null)
        {
            var canBarrel = PortableTableDevice_MaxBarrelsPerPlayer > 0;
            var canLive = PortableTableDevice_Lifetime > 0;
            var isLifeLongerThanOne = PortableTableDevice_Lifetime > 1;
            var desc = $"";
            if (canBarrel && canLive)
            {
                // duration //
                desc += $"Places a <style=cIsUtility>barrel</style> at your feet." +
                    $"\nEach barrel lasts for ";
                if (isLifeLongerThanOne) desc += $"{PortableTableDevice_Lifetime} seconds.";
                else desc += $"a second. ";

                // barrel count //

                desc += $"At most, ";
                if (PortableTableDevice_MaxBarrelsPerPlayer == 1) desc += $"a single barrel";
                else if (PortableTableDevice_MaxBarrelsPerPlayer > 1) desc += $"{PortableTableDevice_MaxBarrelsPerPlayer} barrels";

                desc += $" can be placed per person.";
            }
            else return $"Unsuccesfully attempts to place a barrel.";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "Advanced polymers reinforce this state-of-the-art ballistic bin.";

        private static InteractableSpawnCard iscBarrelNew;
        private static InteractableSpawnCard iscBarrel;
        private static GameObject BarrelPrefab;
        private static DirectorPlacementRule placementRule;

        public static GameObject ItemBodyModelPrefab;

        public PortableBarrelDevice()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/PortableBarrelDevice.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/PortableBarrelDevice.png";
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            iscBarrel = (InteractableSpawnCard)Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscBarrel1");
            iscBarrelNew = UnityEngine.Object.Instantiate(iscBarrel); //remove?
            BarrelPrefab = iscBarrelNew.prefab;
            BarrelPrefab = BarrelPrefab.InstantiateClone($"Bulletstorm_Barrel");
            BarrelInteraction barrelInteraction = BarrelPrefab.GetComponent<BarrelInteraction>();
            barrelInteraction.expReward = 0;
            barrelInteraction.goldReward = 0;
            BarrelDestroyOnInteraction barrelDestroyOnInteraction = BarrelPrefab.AddComponent<BarrelDestroyOnInteraction>();
            barrelDestroyOnInteraction.lifetime = PortableTableDevice_Lifetime;
            barrelDestroyOnInteraction.uselifetime = PortableTableDevice_UseLifetime;
            iscBarrelNew.prefab = BarrelPrefab;

            if (BarrelPrefab) PrefabAPI.RegisterNetworkPrefab(BarrelPrefab);

            //if (HelperPlugin.ClassicItemsCompat.enabled)
                //HelperPlugin.ClassicItemsCompat.RegisterEmbryo(catalogIndex);
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>(modelResourcePath);
                displayRules = GenerateItemDisplayRules();
            }

            base.SetupAttributes();
            placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                maxDistance = 100f,
                minDistance = 20f,
                preventOverhead = true
            };
        }
        public static ItemDisplayRuleDict GenerateItemDisplayRules() //TODO new textures, too bright in game
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.05f, 0.05f, 0.05f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
localPos = new Vector3(-0.0506F, 0.0031F, 0.1144F),
localAngles = new Vector3(353.1398F, 207.1862F, 190.5688F),
localScale = new Vector3(0.044F, 0.044F, 0.044F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.1425F, 0.1042F, 0.0987F),
localAngles = new Vector3(0F, 0F, 180F),
localScale = new Vector3(0.0546F, 0.0546F, 0.0546F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0f, 3.4f, -1.3f),
                    localAngles = new Vector3(60f, 0f, 0f),
                    localScale = generalScale * 16f
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.1508F, 0.3661F, 0F),
localAngles = new Vector3(0F, 190.6273F, 180F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 0.25f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale * 5f
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.0968F, 0.1398F, 0.1242F),
localAngles = new Vector3(2.3691F, 180F, 180F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.0118F, 0.1012F, 0.1775F),
localAngles = new Vector3(353.3091F, 233.8497F, 174.6806F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighBackR",
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
childName = "ThighR",
localPos = new Vector3(-0.103F, 0.133F, 0.1735F),
localAngles = new Vector3(2.8738F, 227.481F, 187.8498F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-1.3673F, 1.2234F, 0.7895F),
localAngles = new Vector3(24.8931F, 196.8494F, 175.3119F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.0581F, 0.0822F, 0.2173F),
localAngles = new Vector3(347.7406F, 205.059F, 174.3302F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
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
childName = "ThighR",
localPos = new Vector3(-0.0311F, 0.0754F, 0.1382F),
localAngles = new Vector3(3.6139F, 262.0628F, 187.8798F),
localScale = new Vector3(0.0338F, 0.0338F, 0.0338F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(-0.0001F, 0.2231F, 0.1346F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Hip",
localPos = new Vector3(0.809F, 0.4511F, -0.8086F),
localAngles = new Vector3(359.4634F, 112.5602F, 184.1675F),
localScale = new Vector3(0.2206F, 0.2206F, 0.2206F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-5.5129F, -3.9632F, -2.368F),
localAngles = new Vector3(349.2849F, 269.3258F, 208.099F),
localScale = new Vector3(1F, 1F, 1F)
                }
            });
            rules.Add("mdlEquipmentDrone", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "GunBarrelBase",
localPos = new Vector3(0.1063F, 0.2238F, 1.6999F),
localAngles = new Vector3(13.9629F, 84.7981F, 261.6667F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4933F, 0.2219F, 0.3349F),
                localAngles = new Vector3(26.0356F, 182.4862F, 40.3566F),
                localScale = new Vector3(0.0791F, 0.0791F, 0.0791F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Hip",
                localPos = new Vector3(-3.0394F, 1.665F, 0.4317F),
                localAngles = new Vector3(12.8761F, 197.0765F, 158.2343F),
                localScale = new Vector3(0.6484F, 0.6484F, 0.6484F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Root",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
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
            On.RoR2.BarrelInteraction.OnInteractionBegin += DestroyBarrel;
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
            On.RoR2.PurchaseInteraction.GetInteractability += PreventEquipmentDroneGive;
        }

        private Interactability PreventEquipmentDroneGive(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            SummonMasterBehavior summonMasterBehavior = self.gameObject.GetComponent<SummonMasterBehavior>();
            if (summonMasterBehavior && summonMasterBehavior.callOnEquipmentSpentOnPurchase)
            {
                CharacterBody characterBody = activator.GetComponent<CharacterBody>();
                if (characterBody && characterBody.inventory)
                {
                    if (characterBody.inventory.currentEquipmentIndex == catalogIndex)
                    {
                        return Interactability.ConditionsNotMet;
                    }
                }
            }
            return orig(self, activator);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.BarrelInteraction.OnInteractionBegin -= DestroyBarrel;
            CharacterMaster.onStartGlobal -= CharacterMaster_onStartGlobal;
            On.RoR2.PurchaseInteraction.GetInteractability -= PreventEquipmentDroneGive;
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster obj)
        {
            if (obj && !obj.gameObject.GetComponent<BarrelTracker>())
            {
                obj.gameObject.AddComponent<BarrelTracker>();
            }
        }
        private void DestroyBarrel(On.RoR2.BarrelInteraction.orig_OnInteractionBegin orig, BarrelInteraction self, Interactor activator)
        {
            orig(self, activator);
            var component = self.gameObject.GetComponent<BarrelDestroyOnInteraction>();
            if (component)
            {
                component.used = true;
            }
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            return TryPlaceBarrel(slot.characterBody);
        }
        private bool TryPlaceBarrel(CharacterBody characterBody)
        {
            bool success = false;
            if (characterBody && characterBody.masterObject)
            {
                var tracker = characterBody.masterObject.GetComponent<BarrelTracker>();
                if (tracker)
                {
                    var position = characterBody.corePosition;
                    var spawnBarrel = iscBarrelNew.DoSpawn(position, characterBody.transform.rotation, new DirectorSpawnRequest(
                        iscBarrelNew, placementRule, RoR2Application.rng));
                    success = spawnBarrel.success;

                    if (spawnBarrel.success)
                    {
                        if (tracker.spawnedBarrels.Count >= PortableTableDevice_MaxBarrelsPerPlayer)
                        {
                            UnityEngine.Object.Destroy(tracker.spawnedBarrels[0].gameObject);
                            tracker.spawnedBarrels.RemoveAt(0);
                        }
                        tracker.spawnedBarrels.Add(spawnBarrel.spawnedInstance.gameObject);
                    } else
                    {
                        //Debug.Log("barrel failed!");
                    }
                } else
                {
                    //Debug.Log("No tracker!");
                }
            }
            return success;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Engine")]
        private class BarrelDestroyOnInteraction : MonoBehaviour
        {
            public float lifetime = 16;
            public float uselifetime = 1;
            public bool used = false;

            private void OnEnable()
            {
                if (Physics.Raycast(base.transform.position, Vector3.down, out RaycastHit raycastHit, 500f, LayerIndex.world.mask))
                {
                    base.transform.position = raycastHit.point;
                    base.transform.up = raycastHit.normal;
                }
            }

            private void DestroyBarrel(On.RoR2.BarrelInteraction.orig_OnInteractionBegin orig, BarrelInteraction self, Interactor activator)
            {
                orig(self, activator);
                var component = self.gameObject.GetComponent<BarrelDestroyOnInteraction>();
                if (component)
                {
                    component.used = true;
                }
            }

            private void FixedUpdate()
            {
                if (used) //shorten the lifetime to the use time.
                {
                    used = false;
                    if (lifetime > uselifetime)
                        lifetime = uselifetime;
                }

                lifetime -= Time.fixedDeltaTime;
                if (lifetime <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }
        public class BarrelTracker : MonoBehaviour
        {
            public List<GameObject> spawnedBarrels = new List<GameObject>(PortableTableDevice_MaxBarrelsPerPlayer);
        }
    }
}
