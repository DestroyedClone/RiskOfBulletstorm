using RiskOfBulletstorm.Utils;
using R2API;
using System.Collections.ObjectModel;
using RoR2;
using TILER2;
using UnityEngine;
using static TILER2.StatHooks;

//TY Harb

namespace RiskOfBulletstorm.Items
{
    public class Unity : Item_V2<Unity>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("By how much will your base damage increase with a single Unity?" +
            "\nKeep in mind that this number is MULTIPLIED by the amount of TOTAL items.",
            AutoConfigFlags.PreventNetMismatch)]
        public float RingUnity_DamageBonus { get; private set; } = 0.2f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("By how much will your base damage increase on subsequent stacks?" +
            "\nKeep in mind that this number is MULTIPLIED by the amount of TOTAL items.", AutoConfigFlags.PreventNetMismatch)]
        public float RingUnity_DamageBonusStack { get; private set; } = 0.05f;
        public override string displayName => "Unity";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>Our Powers Combined</b>\nIncreased combat effectiveness per item.";

        protected override string GetDescString(string langid = null) => $"<style=cIsDamage>+{RingUnity_DamageBonus} base damage</style> per item in inventory." +
            $"\n<style=cStack>(+{RingUnity_DamageBonusStack} base damage per stack)</style>";

        protected override string GetLoreString(string langID = null) => "This ring takes a small amount of power from each gun carried and adds it to the currently equipped gun.";

        public static GameObject ItemBodyModelPrefab;

        public Unity()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/RingUnity.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/RingUnity.png";
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
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
childName = "Finger22R",
localPos = new Vector3(-0.003F, -0.01F, -0.05F),
localAngles = new Vector3(29.7892F, 4.6513F, 172.172F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Finger22R",
localPos = new Vector3(0.0001F, -0.0274F, 0.0246F),
localAngles = new Vector3(45F, 2.8787F, 180F),
localScale = new Vector3(0.012F, 0.0097F, 0.012F)
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
childName = "Finger22R",
localPos = new Vector3(-0.0001F, -0.0179F, -0.0136F),
localAngles = new Vector3(300F, 1F, 185.6411F),
localScale = new Vector3(0.0108F, 0.0108F, 0.0108F)
                },
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Finger22R",
localPos = new Vector3(-0.0047F, -0.05F, -0.0161F),
localAngles = new Vector3(90F, 5F, 0F),
localScale = new Vector3(0.011F, 0.011F, 0.011F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger22R",
                    localPos = new Vector3(-0.003f, 0.04f, -0.03f),
                    localAngles = new Vector3(-90f, 0f, 0f),
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
                    childName = "MechFinger23R",
                    localPos = new Vector3(0f, 0.02f, 0.06f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = generalScale * 3
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[] //todo
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger21L",
                    localPos = new Vector3(0f, 0.3f, 0f),
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
                    childName = "Finger22R",
                    localPos = new Vector3(-0.002f, 0.01f, 0.005f),
                    localAngles = new Vector3(-90f, 20f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandL",
localPos = new Vector3(0.0579F, 0.1776F, -0.0293F),
localAngles = new Vector3(71.3149F, 200.815F, 129.9964F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandL",
localPos = new Vector3(-0.0169F, 0.1184F, 0.0178F),
localAngles = new Vector3(50F, 350F, 349.5197F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Muzzle",
                    localPos = new Vector3(0.1f, 0.44f, -2.85f),
                    localAngles = new Vector3(35f, -1.042f, -135f),
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
                    localPos = new Vector3(0.4f, 2.3f, 0.4f),
                    localAngles = new Vector3(90f, 90f, 0f),
                    localScale = generalScale * 20f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Finger11L",
localPos = new Vector3(0.0585F, 0.2059F, -0.0867F),
localAngles = new Vector3(296.9756F, 52.6793F, 49.2558F),
localScale = new Vector3(0.2351F, 0.2351F, 0.2351F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "HandL",
                localPos = new Vector3(0.0348F, -0.1827F, -0.0022F),
                localAngles = new Vector3(66.5591F, 260.8141F, 350.4267F),
                localScale = new Vector3(0.0822F, 0.0822F, 0.0822F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(-1.5903F, 1.6368F, 0.0529F),
                localAngles = new Vector3(8.3597F, 294.0991F, 14.2234F),
                localScale = new Vector3(0.2198F, 0.2198F, 0.2198F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLT",
                localPos = new Vector3(-0.0876F, -0.2105F, -0.3732F),
                localAngles = new Vector3(0F, 0F, 180F),
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
                childName = "Head",
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
            GetStatCoefficients += BoostDamage;
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients -= BoostDamage;
            CharacterMaster.onStartGlobal -= CharacterMaster_onStartGlobal;
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster obj)
        {
            if (obj && obj.inventory && !obj.gameObject.GetComponent<RingUnityTracker>())
            {
                var tracker = obj.gameObject.AddComponent<RingUnityTracker>();
                tracker.inventory = obj.inventory;
            }
        }

        private void BoostDamage(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.master || !sender.master.inventory) return;
            var UnityInventoryCount = sender.master.inventory.GetItemCount(catalogIndex);
            var component = sender.masterObject.GetComponent<RingUnityTracker>();
            if (UnityInventoryCount > 0 && component)
            {
                var totalCount = component.itemCount;
                args.baseDamageAdd += totalCount * (RingUnity_DamageBonus + (RingUnity_DamageBonusStack * (UnityInventoryCount - 1)));
            }
        }
        private class RingUnityTracker : MonoBehaviour
        {
            public int itemCount = 0;
            public Inventory inventory;

            public void Start()
            {
                if (inventory)
                    inventory.onInventoryChanged += Inventory_onInventoryChanged;
                else
                    BulletstormPlugin._logger.LogMessage("Inventory not found!");
            }

            private void Inventory_onInventoryChanged()
            {
                itemCount = HelperUtil.GetTotalItemCount(inventory);
            }
        }
    }
}
