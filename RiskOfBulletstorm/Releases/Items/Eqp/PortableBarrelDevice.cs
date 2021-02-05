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
        public static float PortableTableDevice_UseLifetime { get; private set; } = 4f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many barrels should be allowed in the world? (Set to -1 for infinite)", AutoConfigFlags.PreventNetMismatch)]
        public static int PortableTableDevice_MaxBarrels { get; private set; } = 100;
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
            if (PortableTableDevice_MaxBarrels > 0 && PortableTableDevice_Lifetime > 0)
                desc += "Places a barrel.";
            else desc += "Faulty user input prevents this device from functioning.";
            return desc;
        }

        protected override string GetDescString(string langid = null)
        {
            var canBarrel = PortableTableDevice_MaxBarrels > 0;
            var isInfiniteBarrel = PortableTableDevice_MaxBarrels == -1;
            var canLive = PortableTableDevice_Lifetime > 0;
            var isLifeLongerThanOne = PortableTableDevice_Lifetime > 1;
            var desc = $"";
            if (canBarrel && canLive)
            {
                // duration //
                desc += $"Places a <style=cIsUtility>barrel</style> at your feet." +
                    $"\nEach barrel lasts for ";
                if (isLifeLongerThanOne) desc += $"{PortableTableDevice_Lifetime} seconds.";
                else desc += $"a second.";

                // barrel count //
                if (isInfiniteBarrel)
                    desc += $"There is no limit on the amount of barrels that";
                else
                {
                    desc += $"At most, ";
                    if (PortableTableDevice_MaxBarrels == 1) desc += $"a single barrel";
                    else if (PortableTableDevice_MaxBarrels > 1) desc += $"{PortableTableDevice_MaxBarrels} barrels";
                }
                desc += $" can be placed in the world.";
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
            iscBarrelNew = UnityEngine.Object.Instantiate(iscBarrel);
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
                    localPos = new Vector3(0f, 0.2f, 0.22f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0.1f, 0.2f, 0.13f),
                    localAngles = new Vector3(0f, 0f, 90f),
                    localScale = new Vector3(0.08f, 0.08f, 0.08f)
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
                    localPos = new Vector3(0f, 0.5f, 0.22f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale
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
                    localPos = new Vector3(0f, 0f, 0.13f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0f, 0.1f, 0.2f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
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
                    localPos = new Vector3(0f, 0.05f, 0.15f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0f, 5.2f, 0.3f),
                    localAngles = new Vector3(90f, 0f, 0f),
                    localScale = generalScale * 8
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0f, 0.04f, 0.18f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
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
                    localPos = new Vector3(0f, 0.15f, 0.12f),
                    localAngles = new Vector3(-20f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Root",
                    localPos = new Vector3(0f, 0.1f, 0.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 2f
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0f, 0f, 2.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 10f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0f, 5f, 0f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = generalScale * 20f
                }
            });
            rules.Add("mdlEquipmentDrone", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunBarrelBase",
                    localPos = new Vector3(0f, 0f, 1.7f),
                    localAngles = new Vector3(0f, 0f, 90f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
                }
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
            RoR2.CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody obj)
        {
            if (obj)
            {
                if (!obj.gameObject.GetComponent<BarrelTracker>())
                    obj.gameObject.AddComponent<BarrelTracker>();
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.BarrelInteraction.OnInteractionBegin -= DestroyBarrel;
            RoR2.CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
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
            CharacterBody body = slot.characterBody;
            if (!body) return false;

            return TryPlaceBarrel(body);
        }

        private bool PlaceTableOld(CharacterBody characterBody)
        {
            var barrels = UnityEngine.Object.FindObjectsOfType<BarrelDestroyOnInteraction>();
            var barrelAmt = barrels.Length;
            var maxBarrels = PortableTableDevice_MaxBarrels;

            bool success = false;

            if (barrelAmt < maxBarrels || maxBarrels == -1 )
            {
                //var yOffset = characterBody.characterMotor.capsuleCollider.height / 2;
                //var randomoffset = new Vector3(Random.Range(-randomValue, randomValue), 0f, Random.Range(-randomValue, randomValue));
                var position = characterBody.corePosition;

                var spawnBarrel = iscBarrelNew.DoSpawn(position, characterBody.transform.rotation, new DirectorSpawnRequest(
                    iscBarrelNew, placementRule, RoR2Application.rng));
                success = spawnBarrel.success;
            }
            return success;
        }
        private bool TryPlaceBarrel(CharacterBody characterBody)
        {
            bool success = false;
            var tracker = characterBody.gameObject.GetComponent<BarrelTracker>();
            var position = characterBody.corePosition;
            if (tracker)
            {
                var spawnBarrel = iscBarrelNew.DoSpawn(position, characterBody.transform.rotation, new DirectorSpawnRequest(
                    iscBarrelNew, placementRule, RoR2Application.rng));
                success = spawnBarrel.success;

                if (spawnBarrel.success)
                {
                    if (tracker.spawnedBarrels.Count >= PortableTableDevice_MaxBarrelsPerPlayer)
                    {
                        tracker.spawnedBarrels.RemoveAt(0);
                        UnityEngine.Object.Destroy(tracker.spawnedBarrels[0]);
                    }
                    tracker.spawnedBarrels.Add(spawnBarrel.spawnedInstance);
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
            public List<GameObject> spawnedBarrels;
        }
    }
}
