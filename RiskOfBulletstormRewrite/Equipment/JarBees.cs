using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class JarBees : EquipmentBase<JarBees>
    {
        public static ConfigEntry<int> cfgBeeCount;
        public static ConfigEntry<float> cfgBeeDamageCoefficient;
        public static ConfigEntry<float> cfgBeeProcCoefficient;

        public override string EquipmentName => "Jar of Bees";

        public override string EquipmentLangTokenName => "JARBEES";

        public override string[] EquipmentFullDescriptionParams => new string[]
        {
            cfgBeeCount.Value.ToString(),
            GetChance(cfgBeeDamageCoefficient),
            cfgBeeProcCoefficient.Value.ToString()
        };

        public override GameObject EquipmentModel => Assets.NullModel;

        public override Sprite EquipmentIcon => Assets.NullSprite;

        public static GameObject jarProjectile;
        public static GameObject railgunnerPistolShotPrefab;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
            CreateProjectile();
        }

        public void CreateProjectile()
        {
            jarProjectile = PrefabAPI.InstantiateClone(Utils.ItemHelpers.Load<GameObject>("RoR2/Base/MiniMushroom/SporeGrenadeProjectile.prefab"), "Bulletstorm_BeeJarProjectile", true);
            railgunnerPistolShotPrefab = Utils.ItemHelpers.Load<GameObject>("RoR2/DLC1/Railgunner/RailgunnerPistolProjectile.prefab");
            var PIE = jarProjectile.GetComponent<ProjectileImpactExplosion>();
            Object.Destroy(PIE);
            jarProjectile.GetComponent<ProjectileSimple>().desiredForwardSpeed = 75; //50
            var beeJar = jarProjectile.AddComponent<BulletstormShatterIntoBeesBehavior>();
            beeJar.projectileController = jarProjectile.GetComponent<ProjectileController>();

            ApplyTorqueOnStart applyTorque = jarProjectile.AddComponent<ApplyTorqueOnStart>();
            applyTorque.randomize = true;
            applyTorque.localTorque = new Vector3(400f, 10f, 400f);

            var model = PrefabAPI.InstantiateClone(Utils.ItemHelpers.Load<GameObject>("RoR2/Base/ExplodeOnDeath/PickupWilloWisp.prefab"), "Bulletstorm_GlassJarCopy", true);
            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();
            //model.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            var controller = jarProjectile.GetComponent<ProjectileController>();
            controller.ghostPrefab = model;

            ContentAddition.AddProjectile(jarProjectile);
        }

        protected override void CreateConfig(ConfigFile config)
        {
            cfgBeeCount = config.Bind(ConfigCategory, "Bee Count", 20, "The amount of bees to spawn on impact.");
            cfgBeeDamageCoefficient = config.Bind(ConfigCategory, "Bee Damage Coefficient", 1f, "");
            cfgBeeProcCoefficient = config.Bind(ConfigCategory, "Bee Proc Coefficient", 6f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            Ray aimRay = slot.GetAimRay();
            ProjectileManager.instance.FireProjectile(jarProjectile, aimRay.origin, Quaternion.LookRotation(aimRay.direction), slot.gameObject, slot.characterBody.damage, 0f, Util.CheckRoll(slot.characterBody.crit, slot.characterBody.master), DamageColorIndex.Default, null, -1f);
            return true;
        }



        public class BulletstormShatterIntoBeesBehavior : MonoBehaviour, IProjectileImpactBehavior
        {
            public ProjectileController projectileController;
            public CharacterMaster ownerMaster;

            public void Start()
            {
                ownerMaster = projectileController.owner?.GetComponent<CharacterBody>()?.master;
            }

            public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
            {
                var body = projectileController.owner.GetComponent<CharacterBody>();
                if (!body)
                {
                    return;
                }
                for (int i = 0; i < cfgBeeCount.Value; i++)
                {
                    ProjectileManager.instance.FireProjectile(railgunnerPistolShotPrefab, impactInfo.estimatedPointOfImpact, Quaternion.Euler(impactInfo.estimatedImpactNormal), projectileController.owner, cfgBeeDamageCoefficient.Value, 0, RoR2.Util.CheckRoll(projectileController.owner.GetComponent<CharacterBody>().crit, ownerMaster));
                }

                if (NetworkServer.active)
                {
                    GlobalEventManager.instance.OnHitAll(new DamageInfo() {attacker = body.gameObject}, impactInfo.collider.gameObject);
                }
                Destroy(gameObject);
            }
        }
    }
}
