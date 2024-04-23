using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using RoR2.Skills;
using RiskOfBulletstormRewrite.Utils;
using System.Reflection;
using RiskOfBulletstormRewrite.Modules;
using R2API;
using BepInEx.Configuration;
using System.Diagnostics;
using System.Linq;

namespace RiskOfBulletstormRewrite.Characters
{
    public abstract class CharacterBase<T> : CharacterBase where T : CharacterBase<T>
    {
        public static T Instance { get; private set; }

        public CharacterBase()
        {
            if (Instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting CharacterBase was instantiated twice");
            Instance = this as T;
        }
    }

    //from enforcermod
    public abstract class CharacterBase
    {
        public abstract string CharacterName { get; }
        public abstract string CharacterLangTokenName { get; }
        public virtual string CharacterNameToken
        {
            get
            {
                return LanguageOverrides.LanguageTokenPrefix + CharacterLangTokenName + "_NAME";
            }
        }
        public virtual string CharacterSubtitleToken
        {
            get
            {
                return LanguageOverrides.LanguageTokenPrefix + CharacterLangTokenName + "_SUBTITLE";
            }
        }
        public virtual string CharacterDescriptionToken
        {
            get
            {
                return LanguageOverrides.LanguageTokenPrefix + CharacterLangTokenName + "_DESCRIPTION";
            }
        }
        public virtual string CharacterLoreToken
        {
            get
            {
                return LanguageOverrides.LanguageTokenPrefix + CharacterLangTokenName + "_LORE";
            }
        }
        public abstract Sprite CharacterIcon { get; }
        public virtual GameObject BodyPrefab { get; set; }
        public virtual GameObject MasterPrefab { get; set; }
        public virtual SkillLocator SkillLocator { get; set; }
        public virtual string ConfigCategory { get; set; }

        public virtual List<SkillDef> PrimarySkillDefs { get; set; } = new List<SkillDef>();
        public virtual List<SkillDef> SecondarySkillDefs { get; set; } = new List<SkillDef>();
        public virtual List<SkillDef> UtilitySkillDefs { get; set; } = new List<SkillDef>();
        public virtual List<SkillDef> SpecialSkillDefs { get; set; } = new List<SkillDef>();
        public virtual List<SkillFamily> SkillFamilies { get; set; } = new List<SkillFamily>();

        //public abstract ItemDisplaysBase itemDisplays { get; }


        public virtual void Init(ConfigFile config)
        {
            InitializeCharacterBody();
            InitializeCharacterMaster();

            InitializeEntityStateMachine();
            InitializeSkills();

            AddContentManagement();
        }

        public virtual void AddContentManagement()
        {
            ContentAddition.AddBody(BodyPrefab);
            ContentAddition.AddMaster(MasterPrefab);
            foreach (var skills in new List<SkillDef>[] { PrimarySkillDefs, SecondarySkillDefs, UtilitySkillDefs, SpecialSkillDefs })
                foreach (var skill in skills)
                {
                    ContentAddition.AddSkillDef(skill);
                }
            foreach (var skillFamily in SkillFamilies)
            {
                ContentAddition.AddSkillFamily(skillFamily);
            }
        }

        protected virtual void InitializeCharacterBody() { }
        protected virtual void InitializeCharacterMaster()
        {
            if (!MasterPrefab) return;
            
            MasterPrefab.GetComponent<CharacterMaster>().bodyPrefab = BodyPrefab;
        }
        protected virtual void InitializeEntityStateMachine() { }
        public virtual void InitializeSkills()
        {
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

            if (Check(primaryIndex, PrimarySkillDefs)) SkillLocator.primary.AssignSkill(PrimarySkillDefs[primaryIndex]);
            if (Check(secondaryIndex, SecondarySkillDefs)) SkillLocator.secondary.AssignSkill(SecondarySkillDefs[secondaryIndex]);
            if (Check(utilityIndex, UtilitySkillDefs)) SkillLocator.utility.AssignSkill(UtilitySkillDefs[utilityIndex]);
            if (Check(specialIndex, SpecialSkillDefs)) SkillLocator.special.AssignSkill(SpecialSkillDefs[specialIndex]);
        }

        public virtual void SetupPassive()
        {
            //Setup skills add to List
            //Then call base
            //
        }

        public void CreateAndSetNewSkillFamily(GenericSkill skillSlot, string slotName)
        {
            if (!skillSlot)
            {
                UnityEngine.Debug.LogError($"no skillSlot!");
            }
            SkillFamily skillFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (skillFamily as ScriptableObject).name = LanguageOverrides.LanguageTokenPrefix + CharacterLangTokenName + "_" + slotName + "_SKILLFAMILY";
            skillSlot._skillFamily = skillFamily;
            skillFamily.variants = new SkillFamily.Variant[0];
            SkillFamilies.Add(skillFamily);
        }

        private void AddSkillToSkillFamily(SkillDef skillDef, SkillFamily skillFamily)
        {
            HG.ArrayUtils.ArrayAppend(ref Main.ContentPack.entityStateTypes, skillDef.activationState);

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[skillFamily.variants.Length - 1] = new SkillFamily.Variant
            {
                skillDef = skillDef,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null),
            };
            if (!skillFamily.defaultSkillDef)
                skillFamily.defaultVariantIndex = 0;
        }

        private void AddSkillsToSkillFamily(List<SkillDef> skillDefs, SkillFamily skillFamily)
        {
            if (!skillFamily) return;
            if (skillDefs.Count == 0) return;
            foreach (var skillDef in skillDefs)
            {
                AddSkillToSkillFamily(skillDef, skillFamily);
            }
        }

        public virtual void SetupPrimary()
        {
            if (SkillLocator.primary)
            AddSkillsToSkillFamily(PrimarySkillDefs, SkillLocator.primary.skillFamily);
        }

        public virtual void SetupSecondary()
        {
            if (SkillLocator.secondary)
                AddSkillsToSkillFamily(SecondarySkillDefs, SkillLocator.secondary.skillFamily);
        }

        public virtual void SetupUtility()
        {
            if (SkillLocator.utility)
                AddSkillsToSkillFamily(UtilitySkillDefs, SkillLocator.utility.skillFamily);
        }

        public virtual void SetupSpecial()
        {
            if (SkillLocator.special)
                AddSkillsToSkillFamily(SpecialSkillDefs, SkillLocator.special.skillFamily);
        }

    }
}
