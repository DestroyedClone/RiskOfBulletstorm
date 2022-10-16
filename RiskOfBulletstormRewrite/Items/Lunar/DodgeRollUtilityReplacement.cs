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
        public static ConfigEntry<float> cfgVulnDuration;
        //public static ConfigEntry<float> cfgVulnDurationReduction;
        public static ConfigEntry<float> cfgVulnDamageMultiplier;
        public static ConfigEntry<float> cfgVulnDamageMultiplierStack;
        
        public override string ItemName => "DodgeRollUtilityReplacement";

        public override string ItemLangTokenName => "DODGEROLLUTILITYREPLACEMENT";

        public override ItemTier Tier => ItemTier.Lunar;
        public override bool IsSkillReplacement => true;
        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgVulnDuration),
            GetChance(cfgVulnDamageMultiplierStack),
            GetChance(cfgVulnDuration),
            GetChance(cfgVulnDamageMultiplierStack)
        };
        public override string[] ItemLogbookDescriptionParams => ItemFullDescriptionParams;

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

        protected override void CreateLang()
        {
            base.CreateLang();
            Language.DeferToken(rollSkillDef.skillDescriptionToken, ItemFullDescriptionParams);
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
            cfgVulnDamageMultiplier = config.Bind(ConfigCategory, "Base Damage Vulnerability", 0.1f);
            cfgVulnDamageMultiplierStack = config.Bind(ConfigCategory, "Damage Vulnerability Per Stack", 0.1f);
            cfgVulnDuration = config.Bind(ConfigCategory, "Base Damage Vulnerability Duration Percentage", 0.15f);
            //cfgVulnDurationReduction = config.Bind(ConfigCategory, "Damage Vulnerability Duration Percentage Per Stack", 0.05f);
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
                GetStack(cfgVulnDamageMultiplier, cfgVulnDamageMultiplierStack, GetCount(self.body));
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
