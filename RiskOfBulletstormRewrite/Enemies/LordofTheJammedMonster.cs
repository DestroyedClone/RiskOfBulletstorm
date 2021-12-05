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

namespace RiskOfBulletstormRewrite.Enemies
{
    public class LordofTheJammedMonster : MonsterBase
    {
        public static GameObject characterPrefab;
        public static GameObject masterPrefab;

        public static SkillDef primaryReplacement;
        public static SkillDef secondaryReplacement;
        public static SkillDef utilityReplacement;
        public static SkillDef specialReplacement;

        public override string MonsterName => throw new NotImplementedException();

        public override string MonsterLangTokenName => throw new NotImplementedException();

        public override GameObject BaseMonsterPrefab => throw new NotImplementedException();

        internal static void CreatePrefab()
        {
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/JellyfishBody"), "LordJammedBody", true);

            CharacterBody bodyComponent = characterPrefab.GetComponent<CharacterBody>();
            //bodyComponent.bodyIndex = -1; //def: 19
            bodyComponent.baseNameToken = "LORDOFTHEJAMMED_BODY_NAME"; // name token
            bodyComponent.subtitleNameToken = "LORDOFTHEJAMMED_BODY_SUBTITLE"; // subtitle token- used for umbras
            bodyComponent.bodyFlags = CharacterBody.BodyFlags.Mechanical;
            //bodyComponent.mainRootSpeed = 0;
            bodyComponent.baseMaxHealth = 10000;
            bodyComponent.levelMaxHealth = 10000;
            bodyComponent.baseRegen = 1000f;
            bodyComponent.levelRegen = 1000f;
            bodyComponent.baseMaxShield = 0;
            bodyComponent.levelMaxShield = 0;
            //bodyComponent.baseMoveSpeed = 12;
            bodyComponent.levelMoveSpeed = 0;
            //bodyComponent.baseAcceleration = 80;
            //bodyComponent.baseJumpPower = 0;
            bodyComponent.levelJumpPower = 0;
            //bodyComponent.baseDamage = 15;
            //bodyComponent.levelDamage = 1.5f;
            bodyComponent.baseAttackSpeed = 1;
            bodyComponent.levelAttackSpeed = 0;
            bodyComponent.baseCrit = 0;
            //bodyComponent.levelCrit = 0;
            bodyComponent.baseArmor = 10000; // 0.0099 damage multiplier
            bodyComponent.levelArmor = 0;
            bodyComponent.baseJumpCount = 0;
            bodyComponent.sprintingSpeedMultiplier = 1.45f;

            HealthComponent healthComponent = characterPrefab.GetComponent<HealthComponent>();
            healthComponent.dontShowHealthbar = true;
            healthComponent.godMode = true;

            characterPrefab.GetComponent<SphereCollider>().enabled = false;


        }

        public override void Init(ConfigFile config)
        {
            throw new NotImplementedException();
        }

        public class LordOfTheJammedAI : MonoBehaviour
        {
            public BaseAI baseAI;

            void Start()
            {
                if (baseAI)
                {

                }
            }
        }
    }
}
