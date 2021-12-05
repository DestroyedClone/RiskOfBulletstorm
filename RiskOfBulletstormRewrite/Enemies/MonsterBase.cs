using BepInEx;
using R2API;
using R2API.Utils;
using RiskOfBulletstormRewrite.Artifact;
using RiskOfBulletstormRewrite.Controllers;
using RiskOfBulletstormRewrite.Equipment;
using RiskOfBulletstormRewrite.Equipment.EliteEquipment;
using RiskOfBulletstormRewrite.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zio;
using Zio.FileSystems;
using Path = System.IO.Path;
using RoR2;
using EntityStates;
using RoR2.Skills;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine.Networking;
using EntityStates.ScavMonster;
using EntityStates.Engi.EngiWeapon;
//using Aetherium.Utils;
using RoR2.CharacterAI;
using System.Net;
using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RiskOfBulletstormRewrite.Enemies
{
    public abstract class MonsterBase
    {
        public abstract string MonsterName { get; }
        public abstract string MonsterLangTokenName { get; }
        public abstract GameObject BaseMonsterPrefab { get; }

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
        public abstract void Init(ConfigFile config);


    }
}
