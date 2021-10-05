using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using RoR2.Projectile;
using UnityEngine.Networking;
using EntityStates.Engi.EngiWeapon;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class Molotov : EquipmentBase
    {
        public static float PercentDamagePerTick { get; private set; } = 0.1f;
        public static float FrequencyOfTicks { get; private set; } = 60f;
        public float DurationAoE { get; private set; } = 8f;
        public override float Cooldown => 55f;

        public override string EquipmentName => "Molotov";

        public override string EquipmentLangTokenName => "MOLOTOV";

        public override string EquipmentPickupDesc => "Upon use, throws a molotov that sets an area on fire.";

        public override string EquipmentFullDescription => "Throws a <style=cIsDamage>molotov</style> that <style=cIsDamage>explodes</style> into a <style=cIsDamage>";

        public override string EquipmentLore => "Molotov cocktails aren't guns, and so they are frowned upon by long-dwelling Gungeoneers. They get the job done regardless." +
            "\nKnowing the Hegemony wouldn't let her bring her own weaponry to the Gungeon, the Convict smuggled these few bottles in with the transport's cargo.";

        public override GameObject EquipmentModel => MainAssets.LoadAsset<GameObject>("ExampleEquipmentPrefab.prefab");

        public override Sprite EquipmentIcon => MainAssets.LoadAsset<Sprite>("ExampleEquipmentIcon.png");
        public static GameObject MolotovPrefab { get; private set; }
        public static GameObject MolotovDotZonePrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;

        public static GameObject GlassBreakEffect = Resources.Load<GameObject>("prefabs/effects/ShieldBreakEffect");
        public static string ProjectileModelPath = "Assets/Models/Prefabs/Projectiles/Molotov.prefab";

        public override void Init(ConfigFile config)
        {
            CreateProjectiles();
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }
        public void CreateProjectiles()
        {
            //needs to be declared first
            GameObject sporeGrenadeDotZonePrefab = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectileDotZone");
            MolotovDotZonePrefab = sporeGrenadeDotZonePrefab.InstantiateClone("Bulletstorm_MolotovDotZone", true);
            MolotovDotZonePrefab.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
            ProjectileDotZone projectileDotZone = MolotovDotZonePrefab.GetComponent<ProjectileDotZone>();
            projectileDotZone.damageCoefficient = PercentDamagePerTick;
            projectileDotZone.resetFrequency = Mathf.Clamp(FrequencyOfTicks, 0f, 60f);
            projectileDotZone.lifetime = DurationAoE;
            projectileDotZone.impactEffect = GlassBreakEffect;

            GameObject sporeGrenadePrefab = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectile");
            MolotovPrefab = sporeGrenadePrefab.InstantiateClone("Bulletstorm_Molotov", true);
            MolotovPrefab.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Item;

            var PIE = MolotovPrefab.GetComponent<ProjectileImpactExplosion>();
            if (DurationAoE > 0) PIE.childrenProjectilePrefab = MolotovDotZonePrefab;
            else Object.Destroy(PIE);
            MolotovPrefab.GetComponent<ProjectileSimple>().desiredForwardSpeed = 35; //50

            ApplyTorqueOnStart applyTorque = MolotovPrefab.AddComponent<ApplyTorqueOnStart>();
            applyTorque.randomize = true;
            applyTorque.localTorque = new Vector3(400f, 10f, 400f);

            GameObject model = MainAssets.LoadAsset<GameObject>(ProjectileModelPath);
            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();
            model.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            var controller = MolotovPrefab.GetComponent<ProjectileController>();
            controller.ghostPrefab = model;

            ProjectileAPI.Add(MolotovPrefab);
            ProjectileAPI.Add(MolotovDotZonePrefab);

            if (MolotovPrefab) PrefabAPI.RegisterNetworkPrefab(MolotovPrefab);
            if (MolotovDotZonePrefab) PrefabAPI.RegisterNetworkPrefab(MolotovDotZonePrefab);
        }

        protected override void CreateConfig(ConfigFile config)
        {
            PercentDamagePerTick = config.Bind("EQUIPMENT: " + EquipmentName, "Percent Damage Per Tick", 0.1f, "How much damage should the Molotov's area of effect deal? (Value: Percentage)").Value;
            FrequencyOfTicks = config.Bind("EQUIPMENT: " + EquipmentName, "Tick Frequency", 60f, "How frequently should each damage tick happen?" +
                "\n(60 = Every 1/60th of a second)").Value;
            DurationAoE = config.Bind("EQUIPMENT: " + EquipmentName, "Duration", 8f, "How long should the Molotov's area of effect last?").Value;
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;

            GameObject gameObject = slot.gameObject;

            Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            var damageMult = 1f;
            var angle = Util.QuaternionSafeLookRotation(slot.inputBank ? slot.GetAimRay().direction : gameObject.transform.forward);
            FireMolotov(body, gameObject, angle, damageMult);
            return true;
        }

        public void FireMolotov(CharacterBody body, GameObject gameObject, Quaternion throwAngle, float damageMultiplier = 1f)
        {
            float offset = 0f;
            if (body.characterMotor)
            {
                offset = body.characterMotor.capsuleCollider.height / 3;
            }
            var position = body.corePosition;
            var resultpos = position + Vector3.up * offset;

            if (NetworkServer.active)
            {
                ProjectileManager.instance.FireProjectile(MolotovPrefab, resultpos, throwAngle,
                                      gameObject, body.damage * PercentDamagePerTick * damageMultiplier,
                                      0f, Util.CheckRoll(body.crit, body.master),
                                      DamageColorIndex.Item, null, -1f);
            }
        }

    }
}
