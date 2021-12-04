using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using UnityEngine.Networking;
using System.Linq;
using RoR2.Projectile;
using EntityStates.Captain.Weapon;
using System.Collections.Generic;

namespace RiskOfBulletstormRewrite.Items
{
    public class ClownMask : ItemBase<ClownMask>
    {
        public static ConfigEntry<float> cfgCooldown;
        public static ConfigEntry<float> cfgCooldownPerStack;
        public static ConfigEntry<float> cfgCooldownCap;
        public static ConfigEntry<float> cfgWolfRadius;
        public static ConfigEntry<float> cfgHoustonRadius;
        public static ConfigEntry<int> cfgChainsPelletCount;
        public static ConfigEntry<float> cfgChainsDamageCoefficient;

        public override string ItemName => "Clown Mask";

        public override string ItemLangTokenName => "CLOWNMASK";

        public override ItemTier Tier => ItemTier.Boss;

        public override string[] ItemFullDescriptionParams => new string[]
        {
            cfgCooldown.Value.ToString(),
            cfgCooldownPerStack.Value.ToString(),
            cfgCooldownCap.Value.ToString(),
            cfgWolfRadius.Value.ToString(),
            cfgHoustonRadius.Value.ToString(),
            cfgChainsPelletCount.Value.ToString(),
            GetChance(cfgChainsDamageCoefficient)
        };

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("ExampleItemPrefab.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("ExampleItemIcon.png");

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Any, ItemTag.Damage
        };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgCooldown = config.Bind(ConfigCategory, "Cooldown", 30f, "What is the cooldown between each help?");
            cfgCooldownPerStack = config.Bind(ConfigCategory, "Cooldown Per Stack", 2f, "What is the cooldown reduction per stack?");
            cfgCooldownCap = config.Bind(ConfigCategory, "Minimum Cooldown", 1f, "What is the minimum cooldown that can be reached?");
            cfgWolfRadius = config.Bind(ConfigCategory, "Wolf Taser Radius", 30f, "What is the radius of detection for Wolf's Taser to target?");
            cfgHoustonRadius = config.Bind(ConfigCategory, "Houston Clear Radius", 20f, "What is the radius of detection for Houston's projectiles to clear?");
            cfgChainsPelletCount = config.Bind(ConfigCategory, "Chains Bullet Count", 8, "What is the amount of bullets fired from Chains' Shotgun?");
            cfgChainsDamageCoefficient = config.Bind(ConfigCategory, "Chains Damage Coefficient", 0.8f, "What is the damage coefficient from Chains' Shotgun?");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody obj)
        {
            if (!NetworkServer.active || !obj.master || !obj.master.inventory) return;
            var com = obj.gameObject.AddComponent<ClownMaskHelper>();
            com.inventory = obj.inventory;
            com.characterMaster = obj.master;
        }

        public class ClownMaskHelper : MonoBehaviour
        {
            private BullseyeSearch targetFinder = new BullseyeSearch();
            public Inventory inventory;
            public int itemCount;
            public TeamIndex teamIndex;
            public CharacterMaster characterMaster;

            public float cooldown = cfgCooldown.Value;

            public float age;

            public void Start()
            {
                inventory.onInventoryChanged += Inventory_onInventoryChanged;
                Inventory_onInventoryChanged();
            }

            private void Inventory_onInventoryChanged()
            {
                itemCount = inventory.GetItemCount(ClownMask.instance.ItemDef);

                var calculatedCooldown = cfgCooldown.Value - cfgCooldownPerStack.Value * (itemCount - 1);
                cooldown = Mathf.Max(calculatedCooldown, cfgCooldownCap.Value);

                teamIndex = characterMaster.teamIndex;
            }

            public void OnDestroy()
            {
                inventory.onInventoryChanged -= Inventory_onInventoryChanged;
            }

            private void FixedUpdate()
            {
                age += Time.fixedDeltaTime;
                if (age > cooldown)
                {
                    if (itemCount == 0)
                    {
                        age = 0;
                        return;
                    }
                    switch (Random.Range(0, 3))
                    {
                        case 0:
                            Chat.AddMessage("Tasing");
                            TaseTarget();
                            break;

                        case 1:
                            Chat.AddMessage("Clearing");
                            ClearNearbyProjectiles();
                            break;

                        case 2:
                            Chat.AddMessage("Shooting");
                            ShootAtTarget();
                            break;
                    }
                    age = 0;
                }
            }

            private void FindTarget()
            {
                this.targetFinder.teamMaskFilter = TeamMask.allButNeutral;
                this.targetFinder.teamMaskFilter.RemoveTeam(this.teamIndex);
                this.targetFinder.sortMode = BullseyeSearch.SortMode.Distance;
                this.targetFinder.filterByLoS = true;
                Ray ray = CameraRigController.ModifyAimRayIfApplicable(characterMaster.GetBody().inputBank.GetAimRay(), base.gameObject, out float num);
                this.targetFinder.searchOrigin = ray.origin;
                this.targetFinder.searchDirection = ray.direction;
                //this.targetFinder.maxAngleFilter = 10f;
                this.targetFinder.viewer = characterMaster.GetBody();
                this.targetFinder.teamMaskFilter = TeamMask.GetUnprotectedTeams(teamIndex);
            }

            private void TaseTarget()
            {
                FindTarget();
                this.targetFinder.maxDistanceFilter = cfgWolfRadius.Value;
                this.targetFinder.RefreshCandidates();
                this.targetFinder.FilterOutGameObject(base.gameObject);

                var result = targetFinder.GetResults().FirstOrDefault<HurtBox>();
                if (result)
                {
                    result.healthComponent.TakeDamage(
                        new DamageInfo()
                        {
                            attacker = characterMaster.GetBodyObject(),
                            crit = false,
                            damage = 1f,
                            damageColorIndex = DamageColorIndex.Item,
                            damageType = DamageType.Shock5s,
                            force = Vector3.zero,
                            position = result.healthComponent.body.corePosition,
                            inflictor = gameObject,
                            procCoefficient = 0
                        });
                    Vector3 position = result.healthComponent.body.corePosition;
                    Vector3 start = characterMaster.GetBody().corePosition;
                    if (EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.tracerEffectPrefab)
                    {
                        EffectData effectData = new EffectData
                        {
                            origin = position,
                            start = start
                        };
                        EffectManager.SpawnEffect(EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.tracerEffectPrefab, effectData, true);
                    }
                    /*
                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = EntityStates.Captain.Weapon.FireTazer.projectilePrefab,
                        position = characterMaster.GetBody().corePosition,
                        owner = characterMaster.GetBodyObject(),
                        damage = 0f,
                        force = FireTazer.force,
                    };
                    var lookAtRot = fireProjectileInfo.position - result.healthComponent.body.corePosition;
                    fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(lookAtRot);
                    //fireProjectileInfo.crit = Util.CheckRoll(this.critStat, base.characterBody.master);
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);*/
                }
            }

            private void ShootAtTarget()
            {
                FindTarget();
                //this.targetFinder.maxDistanceFilter = cfgWolfRadius.Value;
                this.targetFinder.RefreshCandidates();
                this.targetFinder.FilterOutGameObject(base.gameObject);

                var result = targetFinder.GetResults().FirstOrDefault<HurtBox>();
                if (result)
                {
                    BulletAttack bulletAttack = new BulletAttack()
                    {
                        aimVector = characterMaster.GetBody().corePosition - result.healthComponent.body.corePosition,
                        bulletCount = (uint)cfgChainsPelletCount.Value,
                        damage = cfgChainsDamageCoefficient.Value * characterMaster.GetBody().damage,
                        damageType = DamageType.Generic,
                        owner = gameObject,
                        falloffModel = BulletAttack.FalloffModel.Buckshot,
                        tracerEffectPrefab = EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.tracerEffectPrefab
                    };
                    bulletAttack.Fire();
                }

            }

            private void ClearNearbyProjectiles()
            {
                float num = cfgHoustonRadius.Value;
                float num2 = num * num;
                List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
                List<ProjectileController> list = new List<ProjectileController>();
                int i = 0;
                int count = instancesList.Count;
                while (i < count)
                {
                    ProjectileController projectileController = instancesList[i];
                    if (projectileController.teamFilter.teamIndex != teamIndex && (projectileController.transform.position - characterMaster.GetBody().corePosition).sqrMagnitude < num2)
                    {
                        list.Add(projectileController);
                    }
                    i++;
                }
                int j = 0;
                int count2 = list.Count;
                while (j < count2)
                {
                    ProjectileController projectileController2 = list[j];
                    if (projectileController2)
                    {
                        UnityEngine.Object.Destroy(projectileController2.gameObject);
                    }
                    j++;
                }
            }
        }
    }
}