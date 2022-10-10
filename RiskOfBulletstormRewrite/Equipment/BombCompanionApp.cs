using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class BombCompanionApp : EquipmentBase<BombCompanionApp>
    {
        // TODO: Add Visualizer for explosion radius
        // Add more effects for use
        // Adjust damage and cooldown
        // Add assets (model, idrs, sound)
        // 
        public static ConfigEntry<float> cfgCooldown;
        public static ConfigEntry<float> cfgRange;
        public static ConfigEntry<float> cfgTarDamageMultiplier;
        public static ConfigEntry<float> cfgJellyfishDamageMultiplier;
        public static ConfigEntry<float> cfgVagrantDamageMultiplier;
        public static ConfigEntry<float> cfgDunestriderDamageMultiplier;

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

        public override GameObject EquipmentModel => LegacyResourcesAPI.Load<GameObject>("RoR2/Base/QuestVolatileBattery/PickupBatteryArray");

        public override Sprite EquipmentIcon => Assets.NullSprite;

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
            cfgCooldown = config.Bind(ConfigCategory, "Cooldown", 90f, "What is the cooldown of this equipment?");
            cfgRange = config.Bind(ConfigCategory, "Range", 75f, "Radius of the activation");
            cfgTarDamageMultiplier = config.Bind(ConfigCategory, "Tar-based Enemy Damage", 2f, "Percentage of your damage dealt against this enemy type.");
            cfgJellyfishDamageMultiplier = config.Bind(ConfigCategory, "Jellyfish Damage", 3f, "Percentage of your damage dealt against this enemy type.");
            cfgVagrantDamageMultiplier = config.Bind(ConfigCategory, "Wandering Vagrant Damage", 1f, "Percentage of your damage dealt against this enemy type.");
            cfgDunestriderDamageMultiplier = config.Bind(ConfigCategory, "Clay Dunestrider Damage", 1.5f, "Percentage of your damage dealt against this enemy type.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
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