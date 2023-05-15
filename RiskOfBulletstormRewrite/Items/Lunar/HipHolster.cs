using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Items
{
    public class HipHolster : ItemBase<HipHolster>
    {
        public static ConfigEntry<float> cfgFreeStockChance;
        public static ConfigEntry<float> cfgFreeStockChancePerStack;

        public override string ItemName => "Hip Holster";

        public override string ItemLangTokenName => "HIPHOLSTER";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgFreeStockChance),
            GetChance(cfgFreeStockChancePerStack)
        };

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Cleansable,
            ItemTag.Utility
        };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgFreeStockChance = config.Bind(ConfigCategory, "Chance of Free Shot", 0.1f, "");
            cfgFreeStockChancePerStack = config.Bind(ConfigCategory, "Chance of Free Shot Per Stack", 0.1f, "Hyperbolic");
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
      childName = "Stomach",
localPos = new Vector3(0.0883F, 0.04358F, 0.05855F),
localAngles = new Vector3(343.6707F, 83.0621F, 337.4149F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.06311F, -0.09981F, 0.00124F),
localAngles = new Vector3(27.567F, 69.44801F, 163.8226F),
localScale = new Vector3(0.0733F, 0.0733F, 0.0733F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Hip",
localPos = new Vector3(0.14827F, 0.62807F, 0.74695F),
localAngles = new Vector3(16.64241F, 352.0331F, 119.2978F),
localScale = new Vector3(0.68137F, 0.71426F, 0.68137F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.13122F, -0.0001F, -0.01835F),
localAngles = new Vector3(19.27725F, 85.19704F, 145.0748F),
localScale = new Vector3(0.12243F, 0.11417F, 0.1148F)
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Base",
localPos = new Vector3(0.11878F, 1.3892F, 0.05592F),
localAngles = new Vector3(339.9832F, 91.42525F, 319.8334F),
localScale = new Vector3(0.13749F, 0.13749F, 0.13749F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.10036F, -0.10851F, 0.00613F),
localAngles = new Vector3(20.62196F, 92.59741F, 170.2556F),
localScale = new Vector3(0.08868F, 0.08868F, 0.08868F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.12369F, 0.03177F, -0.04705F),
localAngles = new Vector3(18.30224F, 89.34419F, 138.0586F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HeadBase",
localPos = new Vector3(0.19012F, 0.06155F, -0.49279F),
localAngles = new Vector3(30.72254F, 121.4533F, 174.6666F),
localScale = new Vector3(0.1341F, 0.1341F, 0.1341F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.08664F, -0.00845F, 0.05259F),
localAngles = new Vector3(15.08299F, 90.70071F, 159.6231F),
localScale = new Vector3(0.13278F, 0.13278F, 0.13278F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
      childName = "Hip",
localPos = new Vector3(0.98113F, 1.0147F, -0.95194F),
localAngles = new Vector3(8.3527F, 84.3969F, 124.4845F),
localScale = new Vector3(0.83998F, 0.83998F, 0.83998F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.07887F, -0.11967F, -0.09726F),
localAngles = new Vector3(22.26467F, 97.54887F, 152.8677F),
localScale = new Vector3(0.11337F, 0.11337F, 0.11337F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.13972F, -0.08546F, -0.00947F),
localAngles = new Vector3(23.02333F, 82.0136F, 150.631F),
localScale = new Vector3(0.12569F, 0.12569F, 0.12569F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
    childName = "Pelvis",
localPos = new Vector3(0.06269F, -0.03343F, -0.05874F),
localAngles = new Vector3(13.14823F, 123.7456F, 153.1889F),
localScale = new Vector3(0.11685F, 0.11685F, 0.11685F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
             childName = "Pelvis",
localPos = new Vector3(0.06196F, -0.01626F, 0.00386F),
localAngles = new Vector3(14.67509F, 86.43238F, 154.4437F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlVoidSurvivor", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Pelvis",
                localPos = new Vector3(0.01192F, 0.04854F, 0.08877F),
                localAngles = new Vector3(22.60713F, 349.1809F, 138.0614F),
                localScale = new Vector3(0.08201F, 0.08201F, 0.08201F)
            });
            rules.Add("mdlRailGunner", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Pelvis",
                localPos = new Vector3(0.06763F, 0.04185F, 0.01322F),
                localAngles = new Vector3(20.46132F, 81.72948F, 140.0435F),
                localScale = new Vector3(0.07687F, 0.07687F, 0.07687F)
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

        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody obj)
        {
            if (obj.hasEffectiveAuthority)
            {
                var comp = obj.gameObject.GetComponent<RBSHipHolsterController>();
                bool flag = GetCount(obj) > 0;
                if (flag != comp)
                {
                    if (flag)
                    {
                        if (obj.skillLocator)
                        {
                            comp = obj.gameObject.AddComponent<RBSHipHolsterController>();
                            comp.skillLocator = obj.skillLocator;
                            comp.inventory = obj.inventory;
                            comp.characterBody = obj;
                        }
                        return;
                    }
                    UnityEngine.Object.Destroy(comp);
                }
            }
        }

        public class RBSHipHolsterController : MonoBehaviour
        {
            public CharacterBody characterBody;
            public Inventory inventory;
            public SkillLocator skillLocator;
            public GenericSkill[] genericSkills;
            public int[] lastStocks;

            public void Start()
            {
                List<GenericSkill> genericSkillsList = new List<GenericSkill>();
                List<int> lastStockList = new List<int>();
                if (skillLocator.primary)
                {
                    genericSkillsList.Add(skillLocator.primary);
                    lastStockList.Add(skillLocator.primary.stock);
                }
                if (skillLocator.secondary)
                {
                    genericSkillsList.Add(skillLocator.secondary);
                    lastStockList.Add(skillLocator.secondary.stock);
                }
                if (skillLocator.utility)
                {
                    genericSkillsList.Add(skillLocator.utility);
                    lastStockList.Add(skillLocator.utility.stock);
                }
                if (skillLocator.special)
                {
                    genericSkillsList.Add(skillLocator.special);
                    lastStockList.Add(skillLocator.special.stock);
                }
                genericSkills = genericSkillsList.ToArray();
                lastStocks = lastStockList.ToArray();
            }

            public void FixedUpdate()
            {
                int i = 0;
                while (i < genericSkills.Length)
                {
                    var gs = genericSkills[i];
                    if (gs)
                    {
                        if (lastStocks[i] < gs.stock)
                        {
                            if (RoR2.Util.CheckRoll(100 * Utils.ItemHelpers.GetHyperbolicValue(cfgFreeStockChance.Value, cfgFreeStockChancePerStack.Value, HipHolster.instance.GetCount(characterBody))))
                            {
                                gs.stock += gs.skillDef.requiredStock;
                                gs.ExecuteIfReady();
                            }
                        }
                        lastStocks[i] = gs.stock;
                    }
                    i++;
                }
            }
        }
    }
}