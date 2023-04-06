using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class BombCompanionApp : EquipmentBase<BombCompanionApp>
    {
        // Add more effects for use
        // Adjust damage and cooldown
        // Add assets (model, idrs, sound)
        //

        public static ConfigEntry<float> cfgRange;
        public static ConfigEntry<float> cfgTarDamageMultiplier;
        public static ConfigEntry<float> cfgJellyfishDamageMultiplier;
        public static ConfigEntry<float> cfgVagrantDamageMultiplier;
        public static ConfigEntry<float> cfgDunestriderDamageMultiplier;

        public static ConfigEntry<float> cfgCooldown;
        public override float Cooldown => cfgCooldown.Value;

        public override string EquipmentName => "iBomb Companion App";

        public override string EquipmentLangTokenName => "BOMBCOMPANIONAPP";

        public override string[] EquipmentFullDescriptionParams => new string[]
        {
            cfgRange.Value.ToString(),
            GetChance(cfgTarDamageMultiplier),
            GetChance(cfgJellyfishDamageMultiplier),
            GetChance(cfgVagrantDamageMultiplier),
            GetChance(cfgDunestriderDamageMultiplier)
        };

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        public GameObject ExplosionEffect => UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/BubbleShieldEndEffect.prefab").WaitForCompletion();

        public static GameObject[] explosiveProjectiles = new GameObject[]
        {
            // Arti
			LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageFireboltBasic"),
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageLightningboltBasic"),
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageLightningBombProjectile"),
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MageIceBombProjectile"),

			//Cap
			LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/CaptainTazer"),

			//Commando
            LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile"),

            // Engineer
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EngiHarpoon"),
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EngiGrenadeProjectile"),
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EngiMine"),
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/SpiderMine"),

			// MUL-T
            LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/ToolbotGrenadeLauncherProjectile"), // Scrap Launcher
            LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/CryoCanisterProjectile"), // Secondary bomb

			// Clay Dunestrider
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/Tarball"),
            EntityStates.ClayBoss.ClayBossWeapon.FireBombardment.projectilePrefab,

            // Clay Apothecary (DLC)
            LegacyResourcesAPI.Load<GameObject>("RoR2/DLC1/ClayGrenadier/ClayGrenadierMortarProjectile"),
            LegacyResourcesAPI.Load<GameObject>("RoR2/DLC1/ClayGrenadier/ClayGrenadierBarrelProjectile"),

			// Grovetender
			LegacyResourcesAPI.Load<GameObject>("GravekeeperTrackingFireball"),

                // Elder Lemurian
            LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/LemurianBigFireball"),
                // Void Reaver
            LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/NullifierBombProjectile"),

                // Scavenger
            LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/ScavEnergyCannonProjectile"),
			//get thqiwbbs too

                // Wandering Vagrant
            EntityStates.VagrantMonster.Weapon.JellyBarrage.projectilePrefab,
                // Hermit Crab
            LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/HermitCrabBombProjectile"),
                // Void Reaver
            LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/NullifierPreBombProjectile"),
                // Greater Wisp
            LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/WispCannon"),

                //other
            LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/MissileProjectile"),

                //SOTV
                //Railgunner
                //mines

                //Void Fiend
                //flood
                //corrupted flood

                //Void Devastator
                //black and white bombs

                //Voidling?
                //Larva?
                //Void Jailer's death bomb?

                //Spare Drone Parts: Missile
                //Egocentrism? molotov?
        };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        [RoR2.SystemInitializer(dependencies: typeof(RoR2.EntityStateCatalog))]
        public static void SetupExplosiveProjectiles()
        {
            foreach (var projectile in explosiveProjectiles)
            {
                if (!projectile) continue;
                if (!projectile.GetComponent<IsExplosiveProjectile>())
                {
                    projectile.AddComponent<IsExplosiveProjectile>();
                }
            }
        }

        protected override void CreateConfig(ConfigFile config)
        {
            cfgCooldown = config.Bind(ConfigCategory, CooldownName, 60f, CooldownDescription);
            cfgRange = config.Bind(ConfigCategory, "Range", 75f, "Radius of the activation");
            cfgTarDamageMultiplier = config.Bind(ConfigCategory, "Tar-based Enemy Damage", 2f, "Percentage of your damage dealt against this enemy type.");
            cfgJellyfishDamageMultiplier = config.Bind(ConfigCategory, "Jellyfish Damage", 3f, "Percentage of your damage dealt against this enemy type.");
            cfgVagrantDamageMultiplier = config.Bind(ConfigCategory, "Wandering Vagrant Damage", 1f, "Percentage of your damage dealt against this enemy type.");
            cfgDunestriderDamageMultiplier = config.Bind(ConfigCategory, "Clay Dunestrider Damage", 1.5f, "Percentage of your damage dealt against this enemy type.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = EquipmentModel;
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.05f, 0.05f, 0.05f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0, 0, 0)
                }
            });
            rules.Add("mdlCommandoDualies", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
          childName = "Stomach",
localPos = new Vector3(-0.0674F, 0.10596F, 0.12514F),
localAngles = new Vector3(1.2501F, 11.53722F, 357.2656F),
localScale = new Vector3(0.01538F, 0.01538F, 0.01538F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.09715F, 0.02186F, 0.04443F),
localAngles = new Vector3(4.55559F, 207.3729F, 166.9709F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.35843F, 1.91622F, 1.82309F),
localAngles = new Vector3(328.6926F, 65.2201F, 295.0608F),
localScale = new Vector3(0.21085F, 0.21085F, 0.21085F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.5f, 0.22f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 0.25f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale * 5f
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
             childName = "Pelvis",
localPos = new Vector3(0.14469F, 0.02385F, -0.08427F),
localAngles = new Vector3(14.93058F, 85.79215F, 177.3558F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.15453F, 0.04874F, -0.09673F),
localAngles = new Vector3(5.70684F, 178.0548F, 175.2968F),
localScale = new Vector3(0.02779F, 0.02779F, 0.02779F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HeadBase",
localPos = new Vector3(0F, 0.628F, -0.392F),
localAngles = new Vector3(344.4032F, 180F, 180F),
localScale = new Vector3(0.1341F, 0.1341F, 0.1341F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.13206F, 0.02457F, -0.1538F),
localAngles = new Vector3(346.3556F, 155.0156F, 165.693F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
        childName = "Pelvis",
localPos = new Vector3(-1.45112F, 0.53307F, -1.14919F),
localAngles = new Vector3(34.73383F, 175.0508F, 165.6034F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.20624F, 0.00558F, -0.0261F),
localAngles = new Vector3(356.287F, 184.8918F, 175.727F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0F, -0.0616F, 0.18F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
             childName = "Pelvis",
localPos = new Vector3(0F, -0.00002F, -0.17516F),
localAngles = new Vector3(353.103F, 128.0942F, 174.6221F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
           childName = "Pelvis",
localPos = new Vector3(-0.13713F, 0F, -0.13713F),
localAngles = new Vector3(9.72986F, 175.7365F, 163.5654F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.1f, 0.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 2f
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 2.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 10f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0F, 4.8685F, 0.0438F),
localAngles = new Vector3(288.4044F, 180F, 180F),
localScale = new Vector3(1F, 1F, 1F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.0013F, 0.1559F, -0.2403F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.1594F, 3.6456F, 0.0645F),
                localAngles = new Vector3(279.4401F, 195.4454F, 161.8801F),
                localScale = new Vector3(0.4099F, 0.4099F, 0.4099F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLB",
                localPos = new Vector3(-1.6752F, -0.2F, -0.468F),
                localAngles = new Vector3(2.6768F, 179.4175F, 179.4478F),
                localScale = new Vector3(0.1793F, 0.1793F, 0.1793F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(0.0002F, -0.189F, 1.9457F),
                localAngles = new Vector3(24.2706F, 0.0024F, 0.024F),
                localScale = new Vector3(0.2908F, 0.2908F, 0.2908F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Mask",
                localPos = new Vector3(0F, 0.0344F, -1.6055F),
                localAngles = new Vector3(88.6293F, 0F, 0F),
                localScale = new Vector3(0.425F, 0.425F, 0.425F)
            });
            return rules;
        }

        public static BodyIndex[] destructableBodyIndices = new BodyIndex[]
        {
            BodyCatalog.FindBodyIndex("Pot2Body"),
            //fusioncellbody
        };

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot.characterBody)
            {
                Util.PlaySound("drone_attack_v2_03", slot.gameObject);
                //HelfireController
                Collider[] array = Physics.OverlapSphere(slot.characterBody.corePosition, cfgRange.Value, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Collide);
                GameObject[] array2 = new GameObject[array.Length];
                int count = 0;
                for (int i = 0; i < array.Length; i++)
                {
                    CharacterBody characterBody = Util.HurtBoxColliderToBody(array[i]);
                    GameObject gameObject = characterBody ? characterBody.gameObject : null;
                    if (gameObject && Array.IndexOf<GameObject>(array2, gameObject, 0, count) == -1)
                    {
                        float damageMultiplier = 1f;
                        bool shouldExplode = false;
                        switch (characterBody.baseNameToken)
                        {
                            case "POT2_BODY_NAME":
                            case "FUSIONCELL_BODY_NAME":
                                shouldExplode = true;
                                break;

                            case "CLAYBRUISER_BODY_NAME":
                            case "CLAY_BODY_NAME":
                            case "CLAYGRENADIER_BODY_NAME":
                                damageMultiplier = cfgTarDamageMultiplier.Value;
                                break;

                            case "JELLYFISH_BODY_NAME":
                                damageMultiplier = cfgJellyfishDamageMultiplier.Value;
                                break;

                            case "VAGRANT_BODY_NAME":
                                damageMultiplier = cfgVagrantDamageMultiplier.Value;
                                break;

                            case "CLAYBOSS_BODY_NAME":
                                damageMultiplier = cfgDunestriderDamageMultiplier.Value;
                                break;

                            default:
                                continue;
                        }
                        if (shouldExplode)
                            characterBody.healthComponent.Suicide();
                        characterBody.healthComponent.TakeDamage(new DamageInfo()
                        {
                            attacker = slot.gameObject,
                            crit = slot.characterBody.RollCrit(),
                            damage = slot.characterBody.damage * damageMultiplier,
                            damageColorIndex = DamageColorIndex.Item,
                            damageType = DamageType.AOE,
                            inflictor = slot.gameObject,
                            procChainMask = default,
                            procCoefficient = 0f,
                            position = characterBody.corePosition,
                        });
                        array2[count++] = gameObject;
                    }
                }

                //DefenseMatrixOn
                DeleteNearbyProjectile(slot.characterBody);

                EffectData effectData = new EffectData
                {
                    origin = slot.characterBody.corePosition,
                    scale = cfgRange.Value,
                };
                EffectManager.SpawnEffect(
                    ExplosionEffect,
                    effectData,
                    true);

                slot.subcooldownTimer += 2.5f;
                return true;
            }
            return false;
        }

        private bool DeleteNearbyProjectile(CharacterBody characterBody)
        {
            Vector3 vector = characterBody ? characterBody.corePosition : Vector3.zero;
            TeamIndex teamIndex = characterBody ? characterBody.teamComponent.teamIndex : TeamIndex.None;
            float num = cfgRange.Value * cfgRange.Value;
            bool result = false;
            List<IsExplosiveProjectile> instancesList = InstanceTracker.GetInstancesList<IsExplosiveProjectile>();
            List<IsExplosiveProjectile> list = new List<IsExplosiveProjectile>();
            int num3 = 0;
            int count = instancesList.Count;
            while (num3 < count)
            {
                IsExplosiveProjectile isExplosiveProjectile = instancesList[num3];
                if (isExplosiveProjectile.projectileController.teamFilter.teamIndex != teamIndex && (isExplosiveProjectile.transform.position - vector).sqrMagnitude < num)
                {
                    list.Add(isExplosiveProjectile);
                }
                num3++;
            }
            int i = 0;
            int count2 = list.Count;
            while (i < count2)
            {
                IsExplosiveProjectile isExplosiveProjectile2 = list[i];
                if (isExplosiveProjectile2)
                {
                    result = true;
                    Vector3 position = isExplosiveProjectile2.transform.position;
                    EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                    {
                        origin = position,
                        scale = 3f,
                        start = vector //?
                    }, true);
                    EntityStates.EntityState.Destroy(isExplosiveProjectile2.gameObject);
                }
                i++;
            }
            return result;
        }

        private class IsExplosiveProjectile : MonoBehaviour
        {
            public ProjectileController projectileController;

            public void OnEnable()
            {
                if (!projectileController) projectileController = gameObject.GetComponent<ProjectileController>();
                InstanceTracker.Add(this);
            }

            public void OnDisable()
            {
                projectileController = null;
                InstanceTracker.Remove(this);
            }
        }
    }
}