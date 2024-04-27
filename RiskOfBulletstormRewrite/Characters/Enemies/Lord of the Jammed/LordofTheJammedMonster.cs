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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using KinematicCharacterController;

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

        public static GameObject skull;
        public static GameObject gunSword;
        public static GameObject crown;

        private static readonly Material blackMat = Assets.LoadAddressable<Material>("RoR2/Base/goolake/matGoolake.mat");

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

            SetupCharacterDisplay();
        }

        private void SetupCharacterDisplay()
        {
            LOTJDisplayController locator = BodyPrefab.AddComponent<LOTJDisplayController>();

            var yellowRedMat = Assets.LoadAddressable<Material>("RoR2/Base/Beetle/matSulfurBeetle.mat");
            void SetMaterial(GameObject gameObject, Material material)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (!skinnedMeshRenderer)
                {
                    //Main._logger.LogError($"SkinnedMeshRenderer Not Found For {gameObject.name}!"); <- this one
                    MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                    if (!meshRenderer)
                    {
                        //Main._logger.LogError($"MeshRenderer also falied.");
                    }
                    else
                    {
                        meshRenderer.SetMaterial(material);
                    }
                    return;
                }
                skinnedMeshRenderer.SetMaterial(material);
            }

            GameObject clone(string path, string name)
            {
                return PrefabAPI.InstantiateClone(Assets.LoadAddressable<GameObject>(path), name, false);
            }

            skull = clone("RoR2/Base/DeathMark/mdlDeathMark.fbx", "LOTJSkull");
            SetMaterial(skull, yellowRedMat);

            //scythe = clone("RoR2/Base/HealOnCrit/mdlScythe.fbx", "LOTJWeapon_Scythe");
            //var scytheMat = Assets.LoadAddressable<Material>("RoR2/Base/HealOnCrit/matScythe.mat");
            //SetMaterial(scythe, scytheMat);

            gunSword = clone("RoR2/Base/Bandit2/BanditShotgun.fbx", "LOTJWeapon_Shotgun");
            var shotgunMat = Assets.LoadAddressable<Material>("RoR2/Base/Bandit2/matBandit2Shotgun.mat");
            SetMaterial(gunSword, shotgunMat);

            var scy = UnityEngine.Object.Instantiate(Assets.LoadAddressable<GameObject>("RoR2/Base/HealOnCrit/mdlScythe.fbx"));
            scy.transform.parent = gunSword.transform;
            scy.transform.localPosition = new Vector3(0,0,15);
            scy.transform.localRotation = Quaternion.Euler(0, 350, 0);
            scy.transform.localScale = new Vector3(5,5,5);
            var scytheMat = Assets.LoadAddressable<Material>("RoR2/Base/HealOnCrit/matScythe.mat");
            SetMaterial(scy, scytheMat);

            crown = clone("RoR2/Base/artifactworld/mdlArtifactCrown.fbx", "LOTJCrown");
            UnityEngine.Object.Destroy(crown.transform.Find("ArtifactCrownJewelMesh").gameObject);
            SetMaterial(crown.transform.Find("ArtifactCrownMesh").gameObject, yellowRedMat);

            Transform modelTransform = BodyPrefab.GetComponent<ModelLocator>().modelTransform;
            modelTransform.Find("BrotherHammerConcrete").gameObject.SetActive(false);
            modelTransform.Find("BrotherStibPieces").gameObject.SetActive(false);
            modelTransform.Find("BrotherClothPieces").gameObject.SetActive(false);
           // modelTransform.Find("BrotherHeadArmorMesh").gameObject.SetActive(false);


            var charModel = modelTransform.GetComponent<CharacterModel>();
            locator.meshRenderer = modelTransform.Find("BrotherBodyMesh").GetComponent<SkinnedMeshRenderer>();

            void SetDisplay(GameObject prefab, string childName, Vector3 pos, Vector3 angle, Vector3 scale, string childType)
            {
                var child = charModel.GetComponent<ChildLocator>().FindChild(childName);
                var copy = UnityEngine.Object.Instantiate(prefab, child);
                copy.name += $"( {childType})";
                copy.transform.localPosition = pos;
                copy.transform.localRotation = Quaternion.Euler(angle);
                copy.transform.localScale = scale;
                switch (childType)
                {
                    case "idle":
                        locator.idleChildren.Add(copy);
                        break;
                    case "active":
                        locator.activeChildren.Add(copy);
                        copy.SetActive(false);
                        break;
                    case "static":
                        locator.staticChildren.Add(copy);
                        break;
                }
            }

            SetDisplay(skull, "Head", new Vector3(0.00142F, 0.05512F, 0.07683F), new Vector3(286.3012F, 14.24653F, 351.1197F), Vector3.one * 0.1f, "static");
            SetDisplay(crown, "Head", new Vector3(0.5987F, 0.33382F, 0.07866F), new Vector3(0F, 0F, 0F), new Vector3(0.1F, 0.1F, 0.1F), "static");

            SetDisplay(gunSword, "HandL", new Vector3(-0.0173f, 0.1263f, 0.1854f), new Vector3(10f, 310f, 330f), new Vector3(0.04F, 0.04F, 0.04F), "active");

            SetDisplay(gunSword, "HandL", new Vector3(0.1313f, 0.479f, 0.3097f), new Vector3(329.6324F, 9.65174F, 355.6856F), new Vector3(0.04F, 0.04F, 0.04F), "idle");

        }

        public class LOTJDisplayController : MonoBehaviour
        {
            public List<GameObject> idleChildren = new List<GameObject>();
            public List<GameObject> activeChildren = new List<GameObject>();
            public List<GameObject> staticChildren = new List<GameObject>();

            private bool isAttacking = false;
            private float stopwatch = 0;
            private float duration = 0.5f;

            public SkinnedMeshRenderer meshRenderer;
            public static Material materialToSet { get; set; } = blackMat;

            public void Attack()
            {
                if (isAttacking) return;
                stopwatch = duration;
                ToggleDisplay(true);
            }
            public void FixedUpdate()
            {
                meshRenderer.SetMaterial(materialToSet);

                if (!isAttacking) return;
                stopwatch -= Time.fixedDeltaTime;
                if (stopwatch < 0)
                {
                    ToggleDisplay(false);
                }
            }
            public void ToggleDisplay(bool attacking)
            {
                isAttacking = attacking;
                foreach (var child in activeChildren)
                {
                    child.SetActive(attacking);
                }
                foreach (var child in idleChildren)
                {
                    child.SetActive(!attacking);
                }
            }
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
            public void Start()
            {
                if (!NetworkServer.active) return;

                var inv = gameObject.GetComponent<Inventory>();
                inv.GiveItem(RoR2Content.Items.TeleportWhenOob);
                inv.GiveItem(RoR2Content.Items.Ghost);
                inv.GiveItem(RoR2Content.Items.Syringe, 30);
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

            public LOTJDisplayController displayController;

            public void Awake()
            {
                if (!lordBody)
                    lordBody = GetComponent<CharacterBody>();
                if (!displayController)
                    displayController = GetComponent<LOTJDisplayController>();
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
                displayController.Attack();
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

        public class NewtKickFromShopIfLOTJBehaviour : MonoBehaviour
        {
            public float age = 0;
            public float duration = 5f;

            public CharacterBody newtBody;

            public void Start()
            {
                newtBody = GetComponent<CharacterBody>();
            }

            public void FixedUpdate()
            {
                age += Time.fixedDeltaTime;
                if (age >= duration)
                {
                    if (InstanceTracker.GetInstancesList<LordOfTheJammedBodyBehaviour>().Count > 0)
                    {
                        MechanicStealing.ForceNewtToKickPlayersFromShop(newtBody, true);
                        enabled = false;
                    }
                    age = 0;
                }
            }
        }
    }
}