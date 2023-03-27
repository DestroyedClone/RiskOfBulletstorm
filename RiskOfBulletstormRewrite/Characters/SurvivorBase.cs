using BepInEx.Configuration;
using JetBrains.Annotations;
using R2API;
using RiskOfBulletstormRewrite.Characters;
using RiskOfBulletstormRewrite.Items;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfBulletstormRewrite
{
    public abstract class SurvivorBase<T> : SurvivorBase where T : SurvivorBase<T>
    {        //This, which you will see on all the -base classes, will allow both you and other modders to enter through any class with this to access internal fields/properties/etc as if they were a member inheriting this -Base too from this class.
        public static T Instance { get; private set; }

        public SurvivorBase()
        {
            if (Instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting SurvivorBase was instantiated twice");
            Instance = this as T;
        }
    }

    public abstract class SurvivorBase : CharacterBase
    {
        public abstract Color SurvivorColor { get; }

        public SurvivorDef SurvivorDef;

        public string ConfigCategory
        {
            get
            {
                return "Survivor: " + CharacterName;
            }
        }

        public string ItemPickupToken
        {
            get
            {
                return "RISKOFBULLETSTORM_SURVIVOR_" + CharacterLangTokenName + "_PICKUP";
            }
        }

        public string ItemDescriptionToken
        {
            get
            {
                return "RISKOFBULLETSTORM_SURVIVOR_" + CharacterLangTokenName + "_DESCRIPTION";
            }
        }

        public virtual string ItemDescriptionLogbookToken
        {
            get
            {
                return "RISKOFBULLETSTORM_SURVIVOR_" + CharacterLangTokenName + "_LOGBOOK_DESCRIPTION";
            }
        }

    }
}
