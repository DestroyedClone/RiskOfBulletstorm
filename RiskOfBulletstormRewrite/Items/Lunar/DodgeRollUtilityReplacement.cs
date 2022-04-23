using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;
using RoR2.Skills;
using EntityStates;
using UnityEngine.AddressableAssets;

namespace RiskOfBulletstormRewrite.Items
{
    public class DodgeRollUtilityReplacement : ItemBase<DodgeRollUtilityReplacement>
    {
        public override string ItemName => "DodgeRollUtilityReplacement";

        public override string ItemLangTokenName => "DODGEROLLUTILITYREPLACEMENT";

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Cleansable,
            ItemTag.Utility
        };

        public static SkillDef rollSkillDef;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
            CreateSkillDef();
        }

        public void CreateSkillDef()
        {
            rollSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            rollSkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Commando.CombatDodge));
            rollSkillDef.activationStateMachineName = "Slide";
            rollSkillDef.baseMaxStock = 1;
            rollSkillDef.baseRechargeInterval = 12;
            rollSkillDef.beginSkillCooldownOnSkillEnd = true;
            rollSkillDef.canceledFromSprinting = false;
            rollSkillDef.cancelSprintingOnActivation = false;
            rollSkillDef.dontAllowPastMaxStocks = false;
            rollSkillDef.forceSprintDuringState = true;
            rollSkillDef.fullRestockOnAssign = true;
            rollSkillDef.icon = Assets.NullSprite;
            rollSkillDef.interruptPriority = InterruptPriority.Pain;
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

            ContentAddition.AddSkillDef(rollSkillDef);
        }

        public override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
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
