using RiskOfBulletstorm.Utils;
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using ThinkInvisible.ClassicItems;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Molotov : Equipment_V2<Molotov>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage?? (Default: 0.2 = 20% dps)", AutoConfigFlags.PreventNetMismatch)]
        public float Molotov_Damage { get; private set; } = 0.2f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Frequency of damage ticks? Formula is (1 / Molotov_Frequency)" +
            "\n(Default: 40 = Every 1/40th of a second)", AutoConfigFlags.PreventNetMismatch)]
        public float Molotov_Frequency { get; private set; } = 40f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Duration?? (Default: 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public float Molotov_Duration { get; private set; } = 8f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        //public float Cooldown_config { get; private set; } = 8f;

        public override string displayName => "Molotov";
        public string descText = "Upon use, throws a Molotov that sets an area on fire";
        public override float cooldown { get; protected set; } = 35f;
        public Molotov()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Molotov.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/MolotovIcon.png";
        }
        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Feel The Burn\n" + descText;

        protected override string GetDescString(string langid = null) => $"{descText}, dealing <style=cIsDamage>{Pct(Molotov_Damage)} damage per {Molotov_Frequency}th of a second for {Molotov_Duration} seconds</style>.";

        protected override string GetLoreString(string langID = null) => "Molotov cocktails aren't guns, and so they are frowned upon by long-dwelling Gungeoneers. They get the job done regardless." +
            "\nKnowing the Hegemony wouldn't let her bring her own weaponry to the Gungeon, the Convict smuggled these few bottles in with the transport's cargo.";

        public static GameObject MolotovPrefab { get; private set; }
        public static GameObject MolotovDotZonePrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;



        public override void SetupBehavior()
        {
            base.SetupBehavior();

            //needs to be declared first
            GameObject sporeGrenadeDotZonePrefab = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectileDotZone");
            MolotovDotZonePrefab = sporeGrenadeDotZonePrefab.InstantiateClone("MolotovDotZone");
            MolotovDotZonePrefab.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
            ProjectileDotZone projectileDotZone = MolotovDotZonePrefab.GetComponent<ProjectileDotZone>();
            projectileDotZone.damageCoefficient = Molotov_Damage;
            projectileDotZone.resetFrequency = Molotov_Frequency;
            projectileDotZone.lifetime = Molotov_Duration;

            GameObject sporeGrenadePrefab = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectile");
            MolotovPrefab = sporeGrenadePrefab.InstantiateClone("Molotov");
            MolotovPrefab.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Item;
            MolotovPrefab.GetComponent<ProjectileImpactExplosion>().childrenProjectilePrefab = MolotovDotZonePrefab;
            //Object.Destroy(MolotovPrefab.GetComponent<ApplyTorqueOnStart>());

            var model = Resources.Load<GameObject>(modelResourcePath);
            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();

            var controller = MolotovPrefab.GetComponent<ProjectileController>();
            controller.ghostPrefab = model;

            ProjectileCatalog.getAdditionalEntries += list => list.Add(MolotovPrefab);
            ProjectileCatalog.getAdditionalEntries += list => list.Add(MolotovDotZonePrefab);

            if (MolotovPrefab) PrefabAPI.RegisterNetworkPrefab(MolotovPrefab);
            if (MolotovDotZonePrefab) PrefabAPI.RegisterNetworkPrefab(MolotovDotZonePrefab);

            Embryo_V2.instance.Compat_Register(catalogIndex);
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>(modelResourcePath);
                displayRules = GenerateItemDisplayRules();
            }
            base.SetupAttributes();
        }
        private static ItemDisplayRuleDict GenerateItemDisplayRules()
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.1f, 0.1f, 0.1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.4f, -0.25f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = generalScale
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.2f, 0.2f, -0.2f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = generalScale
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 2.3f, 2.5f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 4
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.3f, -0.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, -0.05f, 0.2f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 0.5f
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, -0.1f, -0.3f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Base",
                    localPos = new Vector3(0f, 0.4f, -1.6f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.025f, -0.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.5f, -2.6f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 4
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.2f, -0.35f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            /*rules.Add("mdlBrother", new ItemDisplayRule[] //Mithrix doesnt use equips
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 0.18f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = generalScale
                }
            });*/
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, -0.3f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[] //TODO
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 2.5f
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, -2f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 8f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(8.5f, -6f, 7f),
                    localAngles = new Vector3(0f, 90f, 0f),
                    localScale = generalScale * 10f
                }
            });
            return rules;
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;

            GameObject gameObject = slot.gameObject;

            //int DeployCount = instance.CheckEmbryoProc(body) ? 2 : 1; //Embryo Check

            Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            FireMolotov(body, gameObject);
            if (instance.CheckEmbryoProc(body)) FireMolotov(body, gameObject, 1f);
            return true;
        }

        public void FireMolotov(CharacterBody body, GameObject gameObject, float yOffset = 0)
        {
            Vector3 corePos = Util.GetCorePosition(body);
            Vector3 offset = new Vector3(0f, yOffset, 0f);
            InputBankTest input = body.inputBank;

            if (NetworkServer.active)
            {
                ProjectileManager.instance.FireProjectile(MolotovPrefab, corePos+offset, Util.QuaternionSafeLookRotation(input.aimDirection),
                                      gameObject, body.damage * Molotov_Damage,
                                      0f, Util.CheckRoll(body.crit, body.master),
                                      DamageColorIndex.Item, null, -1f);
            }
        }
    }
}
