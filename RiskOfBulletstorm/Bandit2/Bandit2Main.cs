using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using System;
using EntityStates;
using R2API;
using RoR2.Skills;

namespace RiskOfBulletstorm.Bandit2
{
    public class Bandit2Main
    {
        public static GameObject myCharacter = Resources.Load<GameObject>("prefabs/characterbodies/Bandit2");
        public static BodyIndex bodyIndex = myCharacter.GetComponent<CharacterBody>().bodyIndex;

        public static void Init()
        {
            try
            {
                SetupSkills();
            }
            catch (Exception)
            {
                Debug.Log("First Failed");
                throw;
            }
            try
            {
                SetupSkills2();
            }
            catch (Exception)
            {
                Debug.Log("Second Failed");
                throw;
            }

        }

        private static void SetupSkills()
        {
            ReloadSkillDef skillDef = Resources.Load<ReloadSkillDef>("skilldefs/bandit2body/FireShotgun2");

            var mySkillDef = UnityEngine.Object.Instantiate<ReloadSkillDef>(skillDef);

            if (mySkillDef)
            {
                mySkillDef.activationState = new SerializableEntityStateType(typeof(FireShotgunRBS));
                LoadoutAPI.AddSkillDef(mySkillDef);

                var skillLocator = myCharacter.GetComponent<SkillLocator>();
                var skillFamily = skillLocator.primary.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableDef = null,
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                };
            }
        }
        private static void SetupSkills2()
        {
            var mycharacter = Resources.Load<GameObject>("prefabs/characterbodies/Bandit2");
            var mySkillDef = ScriptableObject.CreateInstance<ReloadSkillDef>();
            // ADD: some comment about how the machines work
            // ADD: some comment about looking at the prefabs for the name of the machine
            //      (EntityStateMachine) in the prefabs
            mySkillDef.graceDuration = 0.4f;
            mySkillDef.skillDescriptionToken = "BANDIT2_PRIMARYRBS_DESC";
            mySkillDef.skillName = "FireShotgun2RBS";
            mySkillDef.skillNameToken = "BANDIT2_PRIMARYRBS_NAME";
            mySkillDef.icon = Resources.Load<Sprite>("textures/achievementicons/texAttackSpeedIcon");
            mySkillDef.activationState = new SerializableEntityStateType(typeof(FireShotgunRBS));
            mySkillDef.interruptPriority = InterruptPriority.Skill;
            mySkillDef.baseRechargeInterval = 0f;
            mySkillDef.baseMaxStock = 4;
            mySkillDef.rechargeStock = 0;
            mySkillDef.requiredStock = 1;
            mySkillDef.stockToConsume = 1;
            mySkillDef.resetCooldownTimerOnUse = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.dontAllowPastMaxStocks = false;
            mySkillDef.cancelSprintingOnActivation = true;
            mySkillDef.forceSprintDuringState = false;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = true;
            mySkillDef.icon = Resources.Load<Sprite>("textures/achievementicons/texAttackSpeedIcon");
            mySkillDef.keywordTokens = new string[] { "KEYWORD_RAPIDFIRE" };

            LanguageAPI.Add("BANDIT2_PRIMARYRBS_NAME","Burst (Bulletstorm)");
            LanguageAPI.Add("BANDIT2_PRIMARYRBS_DESC", "This is a variant that allows Risk of Bulletstorm accuracy modifiers to properly affect it.");

            if (mySkillDef)
            {
                LoadoutAPI.AddSkillDef(mySkillDef);

                var skillLocator = myCharacter.GetComponent<SkillLocator>();
                var skillFamily = skillLocator.primary.skillFamily;

                Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
                skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
                {
                    skillDef = mySkillDef,
                    unlockableDef = null,
                    viewableNode = new ViewablesCatalog.Node(mySkillDef.skillNameToken, false, null)
                };
            }
        }
    }
}
