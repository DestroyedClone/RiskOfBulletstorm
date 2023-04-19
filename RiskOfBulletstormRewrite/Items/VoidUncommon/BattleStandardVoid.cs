using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class BattleStandardVoid : ItemBase<BattleStandardVoid>
    {
        public static ConfigEntry<float> cfgDamage;

        public override string ItemName => "Leaders Standard";

        public override string ItemLangTokenName => "BATTLESTANDARDVOID";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgDamage)
        };

        public override ItemTier Tier => ItemTier.VoidTier2;

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();

        public override ItemDef ContagiousOwnerItemDef => Items.BattleStandard.instance.ItemDef;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.Damage,
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
            cfgDamage = config.Bind(ConfigCategory, "Damage Percentage Per Ally", 0.1f, "");
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
childName = "Head",
localPos = new Vector3(-0.19571F, 0.5994F, -0.16212F),
localAngles = new Vector3(358.1001F, 44.97617F, 359.12F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.11526F, 0.75213F, -0.32746F),
localAngles = new Vector3(353.7985F, 1.80247F, 343.7585F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-1.33212F, 7.83889F, -1.02276F),
localAngles = new Vector3(0F, 45F, 0F),
localScale = new Vector3(2F, 2F, 2F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.10752F, 1.01017F, -0.25267F),
localAngles = new Vector3(0F, 1F, 359.94F),
localScale = new Vector3(0.19624F, 0.19624F, 0.19624F)
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Base",
localPos = new Vector3(-0.27212F, 3.43912F, -0.46885F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.5097F, 0.5097F, 0.5097F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
           childName = "Chest",
localPos = new Vector3(-0.23242F, 0.84376F, -0.24947F),
localAngles = new Vector3(353.8821F, 45.01846F, 356.2268F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.21888F, 0.92655F, -0.1778F),
localAngles = new Vector3(0F, 45F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "FlowerBase",
localPos = new Vector3(-0.39877F, 2.14933F, -1.06253F),
localAngles = new Vector3(346.4405F, 43.86411F, 347.3012F),
localScale = new Vector3(0.7071F, 0.7071F, 0.7071F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(-0.20909F, 0.66215F, -0.25415F),
localAngles = new Vector3(0F, 45F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-1.44365F, -3.51738F, 8.36825F),
localAngles = new Vector3(33.61577F, 121.9793F, 147.1112F),
localScale = new Vector3(3F, 3F, 3F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.3245F, 0.99165F, -0.12177F),
localAngles = new Vector3(350F, 63F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "chest",
localPos = new Vector3(-0.2351F, 1.02573F, -0.29683F),
localAngles = new Vector3(350F, 45F, 350F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
        childName = "Head",
localPos = new Vector3(0.09112F, 0.49517F, -0.30612F),
localAngles = new Vector3(343.9335F, 353.6192F, 352.535F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
          childName = "Chest",
localPos = new Vector3(-0.07771F, 0.96831F, -0.13345F),
localAngles = new Vector3(0F, 45F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlVoidSurvivor", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(-0.20274F, 0.79457F, -0.45227F),
                localAngles = new Vector3(351.0997F, 8.83873F, 17.70547F),
                localScale = new Vector3(0.25F, 0.25F, 0.25F)
            });
            rules.Add("mdlRailGunner", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.13209F, 0.47492F, -0.31386F),
                localAngles = new Vector3(350.4582F, 20.10735F, 328.4164F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
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
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.master)
            { //kingednerbrine
                var count = GetCount(sender);
                if (count > 0)
                {
                    //var minions = CharacterMaster.readOnlyInstancesList.Where(el => el.minionOwnership.ownerMaster == sender.master);
                    var minionGroup = MinionOwnership.MinionGroup.FindGroup(sender.master.netId);
                    if (minionGroup != null)
                        args.damageMultAdd += count * cfgDamage.Value * minionGroup.memberCount;
                }
            }
        }
    }
}