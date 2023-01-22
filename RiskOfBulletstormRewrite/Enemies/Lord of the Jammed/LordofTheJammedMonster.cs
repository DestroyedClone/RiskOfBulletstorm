using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Enemies
{
    public class LordofTheJammedMonster : MonsterBase<LordofTheJammedMonster>
    {
        public static GameObject characterPrefab;
        public static GameObject masterPrefab;

        public static SkillDef primaryReplacement;
        public static SkillDef secondaryReplacement;
        public static SkillDef utilityReplacement;
        public static SkillDef specialReplacement;

        public override string MonsterName => "LordJammed";

        public override string MonsterLangTokenName => "LORDOFTHEJAMMED";
        public override string MonsterLangTokenSubtitle => "LORDOFTHEJAMMED_BODY_SUBTITLE";

        public override GameObject CreateBodyPrefab()
        {
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/characterbodies/JellyfishBody"), MonsterName + "Body", true);
            ContentAddition.AddBody(characterPrefab);
            CharacterBody bodyComponent = characterPrefab.GetComponent<CharacterBody>();
            //bodyComponent.bodyIndex = -1; //def: 19
            bodyComponent.baseNameToken = TokenPrefix + MonsterLangTokenName + "_BODY_NAME"; // name token
            bodyComponent.subtitleNameToken = TokenPrefix + MonsterLangTokenName + "_BODY_SUBTITLE"; // subtitle token- used for umbras
            //bodyComponent.bodyFlags = CharacterBody.BodyFlags.;
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
            return characterPrefab;
        }

        public override void SetupPassive()
        {
            MonsterSkillLocator.passiveSkill.skillNameToken = TokenPrefix + "PASSIVE_NAME";
            MonsterSkillLocator.passiveSkill.skillDescriptionToken = TokenPrefix + "PASSIVE_DESC";
            base.SetupPassive();
        }

        public override void SetupSecondary()
        {
            CreateAndSetNewSkillFamily(MonsterSkillLocator.secondary, "SECONDARY");

            var mySkillDef = ScriptableObject.CreateInstance<SkillDef>();
            mySkillDef.activationState = new SerializableEntityStateType(typeof(LOTJSlash));
            mySkillDef.activationStateMachineName = "Weapon";
            mySkillDef.baseMaxStock = 1;
            mySkillDef.baseRechargeInterval = 3;
            mySkillDef.beginSkillCooldownOnSkillEnd = false;
            mySkillDef.canceledFromSprinting = false;
            mySkillDef.fullRestockOnAssign = true;
            mySkillDef.interruptPriority = InterruptPriority.PrioritySkill;
            mySkillDef.isCombatSkill = true;
            mySkillDef.mustKeyPress = true;
            mySkillDef.rechargeStock = 1;
            mySkillDef.requiredStock = 1;
            mySkillDef.stockToConsume = 1;
            //mySkillDef.icon = SurvivorSkillLocator.utility.skillDef.icon;
            mySkillDef.skillName = TokenPrefix + "_SECONDARY";
            mySkillDef.skillNameToken = $"{mySkillDef.skillName}_NAME";
            mySkillDef.skillDescriptionToken = $"{mySkillDef.skillName}_DESC";
            (mySkillDef as ScriptableObject).name = mySkillDef.skillName;
            mySkillDef.keywordTokens = new string[] { };
            secondarySkillDefs.Add(mySkillDef);
            base.SetupSecondary();
        }
    }
}