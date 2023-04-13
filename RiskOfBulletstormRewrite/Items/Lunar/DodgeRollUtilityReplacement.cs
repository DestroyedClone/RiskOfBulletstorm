using BepInEx.Configuration;
using EntityStates;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class DodgeRollUtilityReplacement : ItemBase<DodgeRollUtilityReplacement>
    {
        public static ConfigEntry<float> cfgDamageVulnerabilityMultiplier;
        public static ConfigEntry<float> cfgDamageVulnerabilityMultiplierPerStack;
        public static ConfigEntry<float> cfgDamageVulnerabilityDuration;
        public static ConfigEntry<float> cfgDamageVulnerabilityDurationDecreasePerStack;
        public static ConfigEntry<float> cfgDamageVulnerabilityDurationMinimum;

        public override string ItemName => "DodgeRollUtilityReplacement";

        public override string ItemLangTokenName => "DODGEROLLUTILITYREPLACEMENT";

        public override ItemTier Tier => ItemTier.Lunar;
        public override bool IsSkillReplacement => true;
        public override bool ItemDescriptionLogbookOverride => true;

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Cleansable,
            ItemTag.Utility,
            ItemTag.AIBlacklist, //AI can't use it effectively,
            ItemTag.CannotCopy,
            ItemTag.Utility
        };

        public static SkillDef rollSkillDef;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSkillDef();
            CreateLang();
            CreateItem();
            Hooks();
        }

        public string[] RollSkillDefParams => new string[]
        {
            GetChance(cfgDamageVulnerabilityMultiplier),
            GetChance(cfgDamageVulnerabilityMultiplierPerStack),
            cfgDamageVulnerabilityDuration.Value.ToString(),
            cfgDamageVulnerabilityDurationDecreasePerStack.Value.ToString(),
            cfgDamageVulnerabilityDurationMinimum.Value.ToString()
        };

        //todo fix this??? kinda cringe
        public static float GetDuration(int stacks)
        {
            if (stacks <= 1) return cfgDamageVulnerabilityDuration.Value;
            return cfgDamageVulnerabilityDurationMinimum.Value
            + ((cfgDamageVulnerabilityDuration.Value - cfgDamageVulnerabilityDurationMinimum.Value) / (1 + cfgDamageVulnerabilityDurationDecreasePerStack.Value * stacks));
        }

        protected override void CreateLang()
        {
            base.CreateLang();
            LanguageOverrides.DeferToken(rollSkillDef.skillDescriptionToken, RollSkillDefParams);

            LanguageOverrides.DeferLateTokens(ItemDescriptionToken, new string[] { rollSkillDef.skillDescriptionToken });
            LanguageOverrides.DeferLateTokens(ItemDescriptionLogbookToken, new string[]{
                ItemPickupToken,
                rollSkillDef.skillNameToken,
                rollSkillDef.skillDescriptionToken});
        }

        public void CreateSkillDef()
        {
            rollSkillDef = ScriptableObject.CreateInstance<GroundedSkillDef>();
            rollSkillDef.activationState = new SerializableEntityStateType(typeof(DodgeRollState));
            rollSkillDef.activationStateMachineName = "Body";
            rollSkillDef.baseMaxStock = 1;
            rollSkillDef.baseRechargeInterval = 8;
            rollSkillDef.beginSkillCooldownOnSkillEnd = true;
            rollSkillDef.canceledFromSprinting = false;
            rollSkillDef.cancelSprintingOnActivation = false;
            rollSkillDef.dontAllowPastMaxStocks = false;
            rollSkillDef.forceSprintDuringState = true;
            rollSkillDef.fullRestockOnAssign = true;
            rollSkillDef.icon = Assets.LoadSprite("SKILL_DODGEROLL");
            rollSkillDef.interruptPriority = InterruptPriority.Vehicle;
            rollSkillDef.isCombatSkill = false;
            rollSkillDef.keywordTokens = new string[]
            {
                "KEYWORD_AGILE"
            };
            rollSkillDef.mustKeyPress = true;
            rollSkillDef.rechargeStock = 1;
            rollSkillDef.requiredStock = 1;
            rollSkillDef.resetCooldownTimerOnUse = true;
            rollSkillDef.skillDescriptionToken = "RISKOFBULLETSTORM_SKILL_DODGEROLL_DESCRIPTION";
            rollSkillDef.skillName = "RiskOfBulletstorm_DodgeRoll";
            rollSkillDef.skillNameToken = "RISKOFBULLETSTORM_SKILL_DODGEROLL_NAME";
            (rollSkillDef as ScriptableObject).name = rollSkillDef.skillName;
            rollSkillDef.stockToConsume = 1;

            //HG.ArrayUtils.ArrayAppend(ref Main.ContentPack.entityStateTypes, rollSkillDef.activationState);
            ContentAddition.AddSkillDef(rollSkillDef);
            ContentAddition.AddEntityState<DodgeRollState>(out bool wasAdded);
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgDamageVulnerabilityMultiplier = config.Bind(ConfigCategory, "Damage Vulnerability Multiplier", .2f, "");
            cfgDamageVulnerabilityMultiplierPerStack = config.Bind(ConfigCategory, "Damage Vulnerability Multiplier Per Stack", .1f, "");
            cfgDamageVulnerabilityDuration = config.Bind(ConfigCategory, "Damage Vulnerability Duration", 2f, "");
            cfgDamageVulnerabilityDurationDecreasePerStack = config.Bind(ConfigCategory, "Damage Vulnerability Duration Decrease Per Stack", -0.1f);
            cfgDamageVulnerabilityDurationMinimum = config.Bind(ConfigCategory, "Damage Vulnerability Duration Minimum", 0.5f);
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
                    localPos = new Vector3(0.0001F, 0.1785F, 0.2069F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.1F, 0.1849F, 0.0903F),
localAngles = new Vector3(339.1738F, 0F, 90F),
localScale = new Vector3(0.08F, 0.08F, 0.08F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.0642F, 3.5153F, -0.51F),
localAngles = new Vector3(60F, 0F, 0F),
localScale = new Vector3(0.3033F, 0.3033F, 0.3033F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
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
                    childName = "Head",
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
childName = "Head",
localPos = new Vector3(0F, 0.1F, 0.1705F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HeadBase",
localPos = new Vector3(0F, 0.628F, -0.392F),
localAngles = new Vector3(344.4032F, 180F, 180F),
localScale = new Vector3(0.1341F, 0.1341F, 0.1341F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0F, 0.0501F, 0.1545F),
localAngles = new Vector3(0F, 0F, 0F),
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
childName = "Head",
localPos = new Vector3(0.0027F, 0.0418F, 0.1528F),
localAngles = new Vector3(0F, 0F, 0F),
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
localPos = new Vector3(0F, -0.0616F, 0.18F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.15f, 0.12f),
                    localAngles = new Vector3(-20f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.00001F, 0.12369F),
                    localAngles = new Vector3(0F, 0F, 0F),
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
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            On.RoR2.HealthComponent.TakeDamage += TakeDamage;
        }

        private void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body && self.body.HasBuff(Utils.Buffs.DodgeRollBuff))
            {
                damageInfo.damage *= 1 +
                GetStack(cfgDamageVulnerabilityMultiplier, cfgDamageVulnerabilityMultiplierPerStack, GetCount(self.body));
            }
            orig(self, damageInfo);
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (characterBody.skillLocator)
            {
                characterBody.ReplaceSkillIfItemPresent(characterBody.skillLocator.utility, ItemDef.itemIndex, rollSkillDef);
            }
        }
    }
}