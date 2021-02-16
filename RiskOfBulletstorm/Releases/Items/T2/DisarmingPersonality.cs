using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using RoR2;
using R2API;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.Utils.HelperUtil;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class DisarmingPersonality : Item_V2<DisarmingPersonality>
    {
        [AutoConfig("What is the base cost reduction of one Disarming Personality? (Value: Subtractive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float DisarmingPersonality_CostReductionAmount { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cost reduction per stack? (Value: Subtractive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float DisarmingPersonality_CostReductionAmountStack { get; private set; } = 0.05f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the maximum cost reduction? This limit is hyperbolic. (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float DisarmingPersonality_CostReductionAmountLimit { get; private set; } = 0.6f;

        public override string displayName => "Disarming Personality";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        private readonly string descText = "Reduces prices at shops";
        protected override string GetPickupString(string langID = null) => "<b>For You?</b>\n"+descText;

        protected override string GetDescString(string langid = null) => $"<style=cIsUtility>{descText} by {Pct(DisarmingPersonality_CostReductionAmount)}</style>." +
            $"\n<style=cStack>(+{Pct(DisarmingPersonality_CostReductionAmount)} hyperbolically per stack) up to {Pct(DisarmingPersonality_CostReductionAmountLimit)}</stack>" +
            $"\n <style=cSub>Chance is shared amongst players.</style>";

        protected override string GetLoreString(string langID = null) => "The Pilot is able to talk his way into almost anything, usually gunfights.";

        public static GameObject ItemBodyModelPrefab;

        public DisarmingPersonality()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/DisarmingPersonality.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/DisarmingPersonality.png";
        }

        public override void SetupBehavior()
        {

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
        public static ItemDisplayRuleDict GenerateItemDisplayRules()
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.02f, 0.02f, 0.02f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.17f, 0.2f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = generalScale
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.17f, 0.17f),
                    localAngles = new Vector3(-30, 0, 0),
                    localScale = generalScale
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.6341F, 1.1967F, 3.2459F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.2f, 0.3f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.2f, 0.14f),
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
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.25f, 0.15f),
                    localAngles = new Vector3(-30f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HeadBase",
localPos = new Vector3(0.2207F, -0.0464F, -0.4079F),
localAngles = new Vector3(0F, 157.975F, 180F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.25f, 0.15f),
                    localAngles = new Vector3(-30f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 1.5f, -2.1f),
                    localAngles = new Vector3(0f, 180f, 0f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.3f, 0.2f),
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
childName = "Head",
localPos = new Vector3(0F, -0.1164F, 0.1293F),
localAngles = new Vector3(20.5612F, 353.0514F, 359.6537F),
localScale = new Vector3(0.0116F, 0.0116F, 0.0116F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0F, 0.342F, 0.1123F),
localAngles = new Vector3(332.6455F, 0F, 0F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.2969F, -0.0081F, 0.1993F),
localAngles = new Vector3(-0.0029F, 69.3793F, 359.7483F),
localScale = new Vector3(0.0219F, 0.0219F, 0.0219F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0F, -2.3615F, 2.3136F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1431F, 0.1431F, 0.1431F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(8F, -9.3284F, 3.3312F),
localAngles = new Vector3(9.4533F, 90F, 270F),
localScale = new Vector3(0.6487F, 0.6487F, 0.6487F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(-0.2262F, 0.4278F, 0.3746F),
                localAngles = new Vector3(329.5599F, 324.5887F, 19.809F),
                localScale = new Vector3(0.0601F, 0.0601F, 0.0601F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0F, 1.939F, -1.258F),
                localAngles = new Vector3(333.9384F, 178.0904F, 3.8147F),
                localScale = new Vector3(0.1544F, 0.1544F, 0.1544F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(0.3375F, -0.5619F, 0.408F),
                localAngles = new Vector3(296.0211F, 48.5699F, 308.4177F),
                localScale = new Vector3(0.0778F, 0.0778F, 0.0778F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(0.6068F, -0.085F, 0.6972F),
                localAngles = new Vector3(80.9292F, 80.0311F, 80.6376F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.6988F, -0.9787F, 1.2316F),
                localAngles = new Vector3(47.38F, 12.5803F, 11.1295F),
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
            On.RoR2.PurchaseInteraction.Awake += LowerCosts;
        }


        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PurchaseInteraction.Awake -= LowerCosts;
        }

        private void LowerCosts(On.RoR2.PurchaseInteraction.orig_Awake orig, PurchaseInteraction self)
        {
            var chest = self.GetComponent<ChestBehavior>();
            int InventoryCount = Util.GetItemCountForTeam(TeamIndex.Player, catalogIndex, false, true);

            if (chest)
            {
                if (InventoryCount > 0)
                {
                    //var ResultMultUnclamp = 1 - DisarmingPersonality_CostReductionAmount + DisarmingPersonality_CostReductionAmountStack * (InventoryCount - 1);
                    //var ResultMult = Mathf.Max(ResultMultUnclamp, 0);
                        //credit to harb+bord listam
                    var ResultMult = (DisarmingPersonality_CostReductionAmount + (1 - DisarmingPersonality_CostReductionAmount) * (1 - (DisarmingPersonality_CostReductionAmountLimit / Mathf.Pow(InventoryCount + DisarmingPersonality_CostReductionAmountLimit, DisarmingPersonality_CostReductionAmountStack))));
                    var ResultAmt = (int)Mathf.Ceil(self.cost * ResultMult);
                    //Chat.AddMessage("Cost of chest reduced from" + self.cost + " to " + ResultAmt + " with multiplier "+ResultMult);
                    self.cost = ResultAmt;
                    self.Networkcost = ResultAmt;
                }
            }
            orig(self);
        }
    }
}
