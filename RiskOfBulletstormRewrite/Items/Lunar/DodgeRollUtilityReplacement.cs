using EntityStates;
using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using System;

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

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Cleansable,
            ItemTag.Utility,
            ItemTag.AIBlacklist, //AI can't use it effectively,
            ItemTag.CannotCopy
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
        public string[] rollSkillDefParams => new string[]
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
            Language.DeferToken(rollSkillDef.skillDescriptionToken, rollSkillDefParams);

            Language.DeferLateTokens(ItemDescriptionToken, new string[]{rollSkillDef.skillDescriptionToken});
            Language.DeferLateTokens(ItemDescriptionLogbookToken, new string[]{
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
            return new ItemDisplayRuleDict();
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
