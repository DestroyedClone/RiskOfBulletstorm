using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class Antibody : ItemBase<Antibody>
    {
        public static float cfgChance = 25;
        public static float cfgMultiplier = 0.33f;
        public static float cfgMultiplierPerStack = 0.11f;

        public override string ItemName => "Antibody";

        public override string ItemLangTokenName => "ANTIBODY";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            cfgChance.ToString(),
            ToPct(cfgMultiplier),
            ToPct(cfgMultiplierPerStack)
        };

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();
        public override string WikiLink => "https://thunderstore.io/package/DestroyedClone/RiskOfBulletstorm/wiki/178-item-antibody/";

        public override ItemTag[] ItemTags => new ItemTag[] {
            ItemTag.Any,
            ItemTag.Healing
        };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
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
                    childName = "UpperArmL",
localPos = new Vector3(0.03696F, 0.03879F, 0.11967F),
localAngles = new Vector3(294.4059F, 202.7271F, 344.3388F),
localScale = new Vector3(0.03F, 0.03F, 0.03F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmR",
localPos = new Vector3(0.05134F, 0.15001F, -0.14282F),
localAngles = new Vector3(28.11056F, 307.9089F, 128.8022F),
localScale = new Vector3(0.03F, 0.03F, 0.03F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmL",
localPos = new Vector3(-1.27011F, 0.40538F, 0.30338F),
localAngles = new Vector3(294.2434F, 62.18093F, 69.83989F),
localScale = new Vector3(0.3033F, 0.3033F, 0.3033F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "UpperArmR",
localPos = new Vector3(-0.14691F, 0.0489F, -0.00965F),
localAngles = new Vector3(316.9126F, 58.43765F, 43.06916F),
localScale = new Vector3(0.02002F, 0.02002F, 0.02002F)
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.47152F, 0.88665F, 0.25F),
localAngles = new Vector3(0F, 1F, 359.94F),
localScale = new Vector3(0.0778F, 0.0778F, 0.0778F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
               childName = "UpperArmR",
localPos = new Vector3(-0.10909F, 0.06809F, -0.02717F),
localAngles = new Vector3(1.93448F, 16.38417F, 93.94479F),
localScale = new Vector3(0.02532F, 0.02532F, 0.02532F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmR",
localPos = new Vector3(-0.0906F, -0.10807F, -0.05289F),
localAngles = new Vector3(334.6645F, 111.681F, 230.2663F),
localScale = new Vector3(0.02081F, 0.02081F, 0.02081F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmL",
localPos = new Vector3(-0.06164F, -0.27058F, 0.22201F),
localAngles = new Vector3(346.3498F, 164.1298F, 196.4944F),
localScale = new Vector3(0.0686F, 0.0686F, 0.0686F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmL",
localPos = new Vector3(0.02207F, 0.24836F, -0.13911F),
localAngles = new Vector3(336.4906F, 345.2345F, 3.09565F),
localScale = new Vector3(0.02524F, 0.02524F, 0.02524F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                  childName = "UpperArmR",
localPos = new Vector3(2.58761F, 1.41212F, 0.84252F),
localAngles = new Vector3(33.43788F, 280.2613F, 240.5571F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmL",
localPos = new Vector3(0.12407F, 0.04983F, -0.22365F),
localAngles = new Vector3(328.222F, 15.50348F, 306.8981F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlVoidSurvivor", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "UpperArmL",
                localPos = new Vector3(0.19303F, 0.05146F, 0.01563F),
                localAngles = new Vector3(338.9442F, 304.2008F, 284.155F),
                localScale = new Vector3(0.03824F, 0.03824F, 0.03824F)
            });
            rules.Add("mdlRailGunner", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "UpperArmL",
                localPos = new Vector3(0.03415F, -0.04347F, 0.12286F),
                localAngles = new Vector3(310.498F, 195.5293F, 0F),
                localScale = new Vector3(0.0291F, 0.0291F, 0.0291F)
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmR",
localPos = new Vector3(-0.12727F, 0.01563F, -0.19531F),
localAngles = new Vector3(323.7542F, 353.3896F, 67.82253F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
     childName = "UpperArmL",
localPos = new Vector3(-0.05304F, 0.04264F, -0.15704F),
localAngles = new Vector3(310.5252F, 357.3296F, 348.4008F),
localScale = new Vector3(0.03224F, 0.03224F, 0.03224F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
          childName = "UpperArmL",
localPos = new Vector3(0.19172F, -0.06428F, -0.03686F),
localAngles = new Vector3(17.49957F, 279.5374F, 204.2599F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
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
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
        }

        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            if (!nonRegen) goto EarlyReturn;

            if (GetCount(self.body) <= 0) goto EarlyReturn;

            if (Util.CheckRoll(cfgChance))
            {
                var itemCount = self.body.inventory.GetItemCount(ItemDef);
                var multiplier = 1f + cfgMultiplier + cfgMultiplierPerStack * (itemCount - 1);
                amount *= multiplier;
            }
        EarlyReturn:
            return orig(self, amount, procChainMask, nonRegen);
        }
    }
}