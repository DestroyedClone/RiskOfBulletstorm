using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Enemies
{
    public abstract class MonsterBase<T> : MonsterBase where T : MonsterBase<T>
    {
        //This, which you will see on all the -base classes, will allow both you and other modders to enter through any class with this to access internal fields/properties/etc as if they were a member inheriting this -Base too from this class.
        public static T instance { get; private set; }

        public MonsterBase()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting MonsterBase was instantiated twice");
            instance = this as T;
        }

        internal void Init()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class MonsterBase
    {
        public abstract string MonsterName { get; }
        public abstract string MonsterLangTokenName { get; }
        public abstract string MonsterLangTokenSubtitle { get; }
        public GameObject MonsterBodyPrefab => CreateBodyPrefab();
        public string TokenPrefix = "RISKOFBULLETSTORM_";
        public virtual SkillLocator MonsterSkillLocator { get; set; }

        public virtual string ConfigCategory { get; set; }

        public virtual List<SkillDef> primarySkillDefs { get; set; }
        public virtual List<SkillDef> secondarySkillDefs { get; set; }
        public virtual List<SkillDef> utilitySkillDefs { get; set; }
        public virtual List<SkillDef> specialSkillDefs { get; set; }

        /// <summary>
        /// This method structures your code execution of this class. An example implementation inside of it would be:
        /// <para>CreateConfig(config);</para>
        /// <para>CreateLang();</para>
        /// <para>CreateItem();</para>
        /// <para>Hooks();</para>
        /// <para>This ensures that these execute in this order, one after another, and is useful for having things available to be used in later methods.</para>
        /// <para>P.S. CreateItemDisplayRules(); does not have to be called in this, as it already gets called in CreateItem();</para>
        /// </summary>
        /// <param name="config">The config file that will be passed into this from the main class.</param>
        public virtual void Init(ConfigFile config)
        {
            SetupDefaults();
            SetupConfig(config);
            SetupAssets();
            SetupLanguage();
            SetupSkills();
            Hooks();
        }

        public virtual GameObject CreateBodyPrefab()
        {
            return null;
        }

        public void SetupDefaults()
        {
            if (ConfigCategory.IsNullOrWhiteSpace())
                ConfigCategory = "Monster: " + MonsterName;
        }

        public virtual void Hooks()
        {
        }

        public virtual void SetupConfig(ConfigFile config)
        { }

        public virtual void SetupLanguage()
        { }

        public void SetupAssets()
        {
        }

        public void SetupSkills()
        {
            primarySkillDefs = new List<SkillDef>();
            secondarySkillDefs = new List<SkillDef>();
            utilitySkillDefs = new List<SkillDef>();
            specialSkillDefs = new List<SkillDef>();

            SetupPassive();
            SetupPrimary();
            SetupSecondary();
            SetupUtility();
            SetupSpecial();
        }

        public virtual void SetupDefaultLoadout(int primaryIndex, int secondaryIndex, int utilityIndex, int specialIndex)
        {
            bool Check(int index, List<SkillDef> list)
            {
                if (index == -1)
                {
                    return false;
                }
                else
                {
                    if (list.Count > 0)
                    {
                        if (list.Count - 1 < index)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            if (Check(primaryIndex, primarySkillDefs)) MonsterSkillLocator.primary.AssignSkill(primarySkillDefs[primaryIndex]);
            if (Check(secondaryIndex, secondarySkillDefs)) MonsterSkillLocator.secondary.AssignSkill(secondarySkillDefs[secondaryIndex]);
            if (Check(utilityIndex, utilitySkillDefs)) MonsterSkillLocator.utility.AssignSkill(utilitySkillDefs[utilityIndex]);
            if (Check(specialIndex, specialSkillDefs)) MonsterSkillLocator.special.AssignSkill(specialSkillDefs[specialIndex]);
        }

        public virtual void SetupPassive()
        {
            //Setup skills add to List
            //Then call base
            //
        }

        public void CreateAndSetNewSkillFamily(GenericSkill skillSlot, string slotName)
        {
            SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (skillFamily as ScriptableObject).name = MonsterLangTokenName + "_" + slotName + "_SKILLFAMILY";
            skillSlot._skillFamily = skillFamily;
            skillFamily.variants = new SkillFamily.Variant[0];
            ContentAddition.AddSkillFamily(skillFamily);
        }

        private void AddSkillToSkillFamily(SkillDef skillDef, SkillFamily skillFamily)
        {
            HG.ArrayUtils.ArrayAppend(ref Main.ContentPack.entityStateTypes, skillDef.activationState);
            ContentAddition.AddSkillDef(skillDef);

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
            };
        }

        private void AddSkillsToSkillFamily(List<SkillDef> skillDefs, SkillFamily skillFamily)
        {
            foreach (var skillDef in skillDefs)
            {
                AddSkillToSkillFamily(skillDef, skillFamily);
            }
        }

        public virtual void SetupPrimary()
        {
            AddSkillsToSkillFamily(primarySkillDefs, MonsterSkillLocator.primary.skillFamily);
        }

        public virtual void SetupSecondary()
        {
            AddSkillsToSkillFamily(secondarySkillDefs, MonsterSkillLocator.secondary.skillFamily);
        }

        public virtual void SetupUtility()
        {
            AddSkillsToSkillFamily(utilitySkillDefs, MonsterSkillLocator.utility.skillFamily);
        }

        public virtual void SetupSpecial()
        {
            AddSkillsToSkillFamily(specialSkillDefs, MonsterSkillLocator.special.skillFamily);
        }
    }
}