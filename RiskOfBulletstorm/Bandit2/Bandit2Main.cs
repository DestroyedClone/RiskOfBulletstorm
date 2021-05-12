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
        //public static GameObject myCharacter = Resources.Load<GameObject>("prefabs/characterbodies/Bandit2");
        public static GameObject myCharacter;
        //public static BodyIndex bodyIndex = myCharacter.GetComponent<CharacterBody>().bodyIndex;

        public static void Init()
        {
            //SetupSkills();
        }

        private static void SetupSkills()
        {
            SkillDef skillDef = Resources.Load<SkillDef>("skilldefs/bandit2body/FireShotgun2");
            var mySkillDef = GameObject.Instantiate<SkillDef>(skillDef);

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
}
