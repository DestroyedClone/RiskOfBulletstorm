using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class Scope : ItemBase<Scope>
    {
        public static ConfigEntry<float> cfgSpreadReduction;
        public static ConfigEntry<float> cfgSpreadReductionPerStack;

        public override string ItemName => "Scope";

        public override string ItemLangTokenName => "SCOPE";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgSpreadReduction),
            GetChance(cfgSpreadReductionPerStack),
        };

        public int GetStackCountForMaxEffect(float baseAmount, float stackAmount)
        {
            var result = ((1 - baseAmount) / stackAmount) + 1;
            return Mathf.CeilToInt(result);
        }

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[] { ItemTag.Any, ItemTag.Damage };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgSpreadReduction = config.Bind(ConfigCategory, Assets.cfgAccuracyKey, 0.2f, Assets.cfgAccuracyDesc);
            cfgSpreadReductionPerStack = config.Bind(ConfigCategory, Assets.cfgAccuracyPerStackKey, 0.02f, Assets.cfgAccuracyPerStackDesc);
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = ItemModel;
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.05f, 0.05f, 0.05f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0, 0, 0)
                }
            });
            rules.Add("mdlCommandoDualies", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunR",
localPos = new Vector3(-0.22835F, 0.02406F, -0.00058F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.03F, 0.03F, 0.03F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunL",
localPos = new Vector3(0.24626F, 0.03119F, 0.00712F),
localAngles = new Vector3(0F, 270F, 0F),
localScale = new Vector3(0.03F, 0.03F, 0.03F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "BowBase",
localPos = new Vector3(0.00078F, -0.08F, -0.03406F),
localAngles = new Vector3(272.8367F, 74.7951F, 38.67729F),
localScale = new Vector3(0.03F, 0.03F, 0.03F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "LeftArmController",
localPos = new Vector3(-0.31686F, 3.75264F, -2.42226F),
localAngles = new Vector3(12.70004F, 18.40282F, 4.22352F),
localScale = new Vector3(0.3033F, 0.3033F, 0.3033F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "MuzzleLeft",
localPos = new Vector3(0F, -0.22027F, -0.13953F),
localAngles = new Vector3(357.4059F, 180.0658F, -0.00186F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "MuzzleRight",
localPos = new Vector3(0F, -0.22027F, -0.13953F),
localAngles = new Vector3(352.8865F, 186.6039F, 359.4171F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0F, 0.76762F, 1.86576F),
localAngles = new Vector3(9.50273F, 178.6152F, 359.7113F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
             childName = "LowerArmL",
localPos = new Vector3(0.00001F, 0.25214F, -0.14674F),
localAngles = new Vector3(81.09268F, 0F, 0F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
        childName = "LowerArmR",
localPos = new Vector3(0.00004F, 0.25047F, 0.13323F),
localAngles = new Vector3(81.65327F, 180F, 180F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandL",
localPos = new Vector3(-0.01658F, 0.1126F, 0.06455F),
localAngles = new Vector3(72.42754F, 164.3863F, 146.7127F),
localScale = new Vector3(0.02899F, 0.02899F, 0.02899F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Eye",
localPos = new Vector3(0F, 0.91694F, 0.00614F),
localAngles = new Vector3(84.86214F, 179.9999F, 179.9999F),
localScale = new Vector3(0.1341F, 0.1341F, 0.1341F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "MechHandL",
localPos = new Vector3(-0.06069F, 0.1403F, 0.14408F),
localAngles = new Vector3(75.53233F, 174.1189F, 175.7154F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "MechHandR",
localPos = new Vector3(0.09906F, 0.16478F, 0.15977F),
localAngles = new Vector3(75.53233F, 174.1189F, 175.7154F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
          childName = "Head",
localPos = new Vector3(-0.00143F, 4.9128F, 1.66573F),
localAngles = new Vector3(90F, 0F, 0F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandL",
localPos = new Vector3(-0.00007F, 0.00063F, -0.00058F),
localAngles = new Vector3(86.88178F, 321.9138F, 11.08363F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.00002F, -0.0012F, 0.14268F),
localAngles = new Vector3(352.7716F, 180F, 180F),
localScale = new Vector3(0.02347F, 0.02347F, 0.02347F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
               childName = "MuzzleShotgun",
localPos = new Vector3(-0.00937F, 0.07351F, -0.02894F),
localAngles = new Vector3(348.3293F, 180.7753F, 357.0103F),
localScale = new Vector3(0.0264F, 0.0264F, 0.0264F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
               childName = "Head",
localPos = new Vector3(0.00868F, 0.05918F, 0.12342F),
localAngles = new Vector3(351.1826F, 181.3567F, 159.167F),
localScale = new Vector3(0.02749F, 0.02749F, 0.02749F)
                }
            });
            rules.Add("mdlVoidSurvivor", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.13178F, 0.14355F, 0.02685F),
                localAngles = new Vector3(36.80917F, 254.6062F, 167.761F),
                localScale = new Vector3(0.03354F, 0.03354F, 0.04885F)
            });
            rules.Add("mdlRailGunner", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "TopRail",
                localPos = new Vector3(-0.16041F, 0.53142F, -0.14223F),
                localAngles = new Vector3(75.83019F, 119.1037F, 132.2404F),
                localScale = new Vector3(0.02471F, 0.02471F, 0.02471F)
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
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
                    childName = "Head",
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
childName = "Head",
localPos = new Vector3(0F, 4.8685F, 0.0438F),
localAngles = new Vector3(288.4044F, 180F, 180F),
localScale = new Vector3(1F, 1F, 1F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.0013F, 0.1559F, -0.2403F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.1594F, 3.6456F, 0.0645F),
                localAngles = new Vector3(279.4401F, 195.4454F, 161.8801F),
                localScale = new Vector3(0.4099F, 0.4099F, 0.4099F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLB",
                localPos = new Vector3(-1.6752F, -0.2F, -0.468F),
                localAngles = new Vector3(2.6768F, 179.4175F, 179.4478F),
                localScale = new Vector3(0.1793F, 0.1793F, 0.1793F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(0.0002F, -0.189F, 1.9457F),
                localAngles = new Vector3(24.2706F, 0.0024F, 0.024F),
                localScale = new Vector3(0.2908F, 0.2908F, 0.2908F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Mask",
                localPos = new Vector3(0F, 0.0344F, -1.6055F),
                localAngles = new Vector3(88.6293F, 0F, 0F),
                localScale = new Vector3(0.425F, 0.425F, 0.425F)
            });
            return rules;
        }
    }
}