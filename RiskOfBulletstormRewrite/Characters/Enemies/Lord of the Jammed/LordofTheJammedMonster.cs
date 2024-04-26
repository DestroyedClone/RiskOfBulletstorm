using EntityStates;
using R2API;
//using RiskOfBulletstormRewrite.Characters.Enemies;
using RiskOfBulletstormRewrite.Characters;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RiskOfBulletstormRewrite.Characters.Enemies.Lord_of_the_Jammed;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine.Networking;
using RiskOfBulletstormRewrite.GameplayAdditions;
using RoR2.Projectile;

namespace RiskOfBulletstormRewrite.Characters.Enemies
{
    public class LordofTheJammedMonster : CharacterBase<LordofTheJammedMonster>
    {
        public static SkillDef primaryReplacement;
        public static SkillDef secondaryReplacement;
        public static SkillDef utilityReplacement;
        public static SkillDef specialReplacement;

        public override string CharacterName => "LordJammed";

        public override string CharacterLangTokenName => "LORDOFTHEJAMMED";

        public override Sprite CharacterIcon => Assets.NullSprite;

        protected override void InitializeCharacterBody()
        {
            BodyPrefab = PrefabAPI.InstantiateClone(UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/BrotherGlass/BrotherGlassBody.prefab").WaitForCompletion(), "RBS_LordOfTheJammedBody", true);
            CharacterBody bodyComponent = BodyPrefab.GetComponent<CharacterBody>();
            //bodyComponent.bodyIndex = -1; //def: 19
            bodyComponent.baseNameToken = CharacterNameToken; // name token
            bodyComponent.subtitleNameToken = CharacterSubtitleToken; // subtitle token- used for umbras
            bodyComponent.bodyFlags =
                CharacterBody.BodyFlags.ImmuneToGoo
                | CharacterBody.BodyFlags.IgnoreFallDamage
                | CharacterBody.BodyFlags.ImmuneToVoidDeath
                | CharacterBody.BodyFlags.OverheatImmune;
            //bodyComponent.mainRootSpeed = 0;
            bodyComponent.baseMaxHealth = 9999;
            bodyComponent.levelMaxHealth = 0;
            bodyComponent.baseRegen = 9999f;
            bodyComponent.levelRegen = 0f;
            bodyComponent.baseMaxShield = 0;
            bodyComponent.levelMaxShield = 0;
            bodyComponent.baseMoveSpeed = 12;
            bodyComponent.levelMoveSpeed = 0;
            //bodyComponent.baseAcceleration = 80;
            //bodyComponent.baseJumpPower = 0;
            //bodyComponent.levelJumpPower = 0;
            //bodyComponent.baseDamage = 15;
            //bodyComponent.levelDamage = 1.5f;
            bodyComponent.baseAttackSpeed = 1;
            //bodyComponent.levelAttackSpeed = 0;
            //bodyComponent.baseCrit = 0;
            //bodyComponent.levelCrit = 0;
            bodyComponent.baseArmor = 10000; // 0.0099 damage multiplier
            bodyComponent.levelArmor = 0;
            bodyComponent.baseJumpCount = 0;
            bodyComponent.sprintingSpeedMultiplier = 1f;

            HealthComponent healthComponent = BodyPrefab.GetComponent<HealthComponent>();
            healthComponent.dontShowHealthbar = true;

            var bodyBehaviour = BodyPrefab.AddComponent<LordOfTheJammedBodyBehaviour>();
            bodyBehaviour.lordBody = bodyComponent;
        }

        public override void SetupPassive()
        {
            SkillLocator = BodyPrefab.GetComponent<SkillLocator>();
            SkillLocator.passiveSkill = new SkillLocator.PassiveSkill
            {
                skillNameToken = "RISKOFBULLETSTORM_LORDOFTHEJAMMED_PASSIVE_NAME",
                skillDescriptionToken = "RISKOFBULLETSTORM_LORDOFTHEJAMMED_PASSIVE_DESC",
                enabled = true,
            };
            base.SetupPassive();
        }

        public override void SetupUtility()
        {
            SkillLocator.utility.enabled = false;
        }

        protected override void InitializeEntityStateMachine()
        {
            ContentAddition.AddEntityState<LOTJSpawnState>(out bool _);
            ContentAddition.AddEntityState<ForceUngroundCharacterMain>(out bool _);

            foreach (EntityStateMachine entityStateMachine in BodyPrefab.GetComponents<EntityStateMachine>())
            {
                if (entityStateMachine.customName == "Body")
                {
                    entityStateMachine.initialStateType = new SerializableEntityStateType(typeof(LOTJSpawnState));
                    entityStateMachine.mainStateType = new SerializableEntityStateType(typeof(ForceUngroundCharacterMain));
                    return;
                }
            }
            base.InitializeEntityStateMachine();
        }

        protected override void InitializeCharacterMaster()
        {
            MasterPrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/BrotherGlass/BrotherGlassMaster.prefab").WaitForCompletion(), "RBS_LordOfTheJammedMaster", true);

            MasterPrefab.AddComponent<LordOfTheJammedMasterBehaviour>();

            var skillDrivers = MasterPrefab.GetComponents<AISkillDriver>();
            foreach (var skillDriver in skillDrivers)
            {
                if (skillDriver.customName == "SprintBash")
                {
                    skillDriver.maxDistance = 100;
                    //skillDriver.skillSlot = SkillSlot.Secondary;
                    //skillDriver.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous;
                }
                else if (skillDriver.customName == "Walk After Target Off Nodegraph")
                {
                    skillDriver.maxDistance = Mathf.Infinity;
                }
                    
                else
                    skillDriver.enabled = false;
            }


            base.InitializeCharacterMaster();
        }

        public class LordOfTheJammedMasterBehaviour : MonoBehaviour
        {
            //public GameObject jetpack;
            public CharacterMaster characterMaster;
            public void Start()
            {
                if (!NetworkServer.active) return;

                var inv = gameObject.GetComponent<Inventory>();
                inv.GiveItem(RoR2Content.Items.TeleportWhenOob);
                inv.GiveItem(RoR2Content.Items.Ghost);
                inv.GiveItem(RoR2Content.Items.Syringe, 30);
                inv.GiveItem(Items.LordOfTheJammedIdentifierItem.instance.ItemDef);

                characterMaster = GetComponent<CharacterMaster>();
            }
        }

        public class LordOfTheJammedBodyBehaviour : MonoBehaviour
        {
            public CharacterMaster lordMaster;
            public CharacterBody lordBody;

            public static GameObject ProjectilePrefab => Assets.LoadAddressable<GameObject>("RoR2/DLC1/VoidBarnacle/VoidBarnacleBullet.prefab");
            private const float damageCoefficient = 1f;
            private const int verticalIntensity = 2;
            private const float horizontalIntensity = 1f;

            public void Awake()
            {
                if (!lordBody)
                    lordBody = GetComponent<CharacterBody>();
            }

            public void OnEnable()
            {
                if (lordBody)
                    lordBody.onSkillActivatedServer += LordBody_onSkillActivatedServer;
                InstanceTracker.Add(this);
            }

            public void OnDisable()
            {
                if (lordBody)
                    lordBody.onSkillActivatedServer -= LordBody_onSkillActivatedServer;
                InstanceTracker.Remove(this);
            }

            private void LordBody_onSkillActivatedServer(GenericSkill genericSkill)
            {
                Throw(genericSkill.characterBody);
            }

            //https://github.com/GnomeModder/ChefMod/blob/main/thecook-vs/CHEFMod/States/Primary/Boosted/Mince.cs
            private void Throw(CharacterBody characterBody)
            {
                bool isCrit = characterBody.RollCrit();
                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = ProjectilePrefab,
                    position = characterBody.corePosition,
                    owner = characterBody.gameObject,
                    //damage = base.characterBody.damage * (4f / (chefPlugin.minceHorizontolIntensity.Value + intensity)),
                    damage = characterBody.damage * damageCoefficient,
                    force = 0f,
                    crit = isCrit,
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                };

                Vector3 aimDirection = characterBody.inputBank.GetAimRay().direction;
                aimDirection.y = 0f;
                aimDirection.Normalize();
                float orientation = Mathf.Atan2(aimDirection.z, aimDirection.x);

                for (int i = -1 * verticalIntensity; i <= verticalIntensity; i++)
                {
                    float phi;
                    if (verticalIntensity != 0) phi = i * (1f / (2f * verticalIntensity)) * Mathf.PI;
                    float r = Mathf.Cos(phi);
                    int circum = Mathf.Max(1, Mathf.FloorToInt(horizontalIntensity * Mathf.PI * 2 * r));
                    for (int j = 0; j < circum; j++)
                    {
                        float theta = orientation + 2 * Mathf.PI * ((float)j / (float)circum);
                        Vector3 direction = new Vector3(r * Mathf.Cos(theta), Mathf.Sin(phi), r * Mathf.Sin(theta));

                        info.rotation = Util.QuaternionSafeLookRotation(direction);

                        ProjectileManager.instance.FireProjectile(info);
                    }
                }
            }

        }
    }
}