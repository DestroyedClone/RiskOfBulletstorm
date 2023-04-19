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
            rollSkillDef.baseRechargeInterval = 4;
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
                    childName = "ThighL",
localPos = new Vector3(-0.01844F, -0.03754F, 0.07669F),
localAngles = new Vector3(354.615F, 90.89241F, 185.6028F),
localScale = new Vector3(0.03543F, 0.03543F, 0.03543F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.07121F, -0.00494F, -0.10624F),
localAngles = new Vector3(359.9231F, 290.6678F, 188.3928F),
localScale = new Vector3(0.04506F, 0.04506F, 0.04506F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.4921F, 1.59384F, 2.50297F),
localAngles = new Vector3(358.5541F, 49.25265F, 83.23964F),
localScale = new Vector3(0.3033F, 0.3033F, 0.3033F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.23988F, 0.01436F, -0.00001F),
localAngles = new Vector3(1.45926F, 343.714F, 184.9592F),
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
localPos = new Vector3(0F, 0F, 0.25F),
localAngles = new Vector3(3.80394F, 90.19054F, 169.778F),
localScale = new Vector3(0.14457F, 0.14457F, 0.14457F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
      childName = "Pelvis",
localPos = new Vector3(0.1492F, 0.07004F, -0.06086F),
localAngles = new Vector3(352.2094F, 221.9753F, 181.7055F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.19136F, 0.07801F, -0.01198F),
localAngles = new Vector3(339.3308F, 355.9757F, 195.854F),
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
localPos = new Vector3(0.22172F, -0.04705F, -0.39197F),
localAngles = new Vector3(2.84832F, 195.341F, 79.73347F),
localScale = new Vector3(0.10504F, 0.10504F, 0.10504F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.00027F, 0.30632F, 0.11407F),
localAngles = new Vector3(37.33253F, 351.9308F, 84.97629F),
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
localPos = new Vector3(-1.22296F, 0.3113F, -0.05838F),
localAngles = new Vector3(342.8196F, 9.97568F, 173.4556F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighL",
localPos = new Vector3(-0.00065F, 0.06614F, 0.1752F),
localAngles = new Vector3(340.6839F, 108.6362F, 166.0999F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighL",
localPos = new Vector3(-0.00409F, -0.06094F, 0.15395F),
localAngles = new Vector3(23.43049F, 107.352F, 175.7763F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
             childName = "Pelvis",
localPos = new Vector3(-0.16911F, -0.04023F, 0.00163F),
localAngles = new Vector3(14.33741F, 355.2548F, 180.4994F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                  childName = "Pelvis",
localPos = new Vector3(-0.17825F, 0.00706F, -0.08601F),
localAngles = new Vector3(15.2784F, 334.3979F, 187.1288F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlVoidSurvivor", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Pelvis",
                localPos = new Vector3(0.10922F, 0.00805F, -0.06576F),
                localAngles = new Vector3(14.79906F, 214.7281F, 190.0404F),
                localScale = new Vector3(0.06285F, 0.06285F, 0.06285F)
            });
            rules.Add("mdlRailGunner", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Pelvis",
                localPos = new Vector3(-0.05438F, -0.02487F, -0.10113F),
                localAngles = new Vector3(8.46059F, 316.0911F, 177.9461F),
                localScale = new Vector3(0.06106F, 0.06106F, 0.06106F)
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