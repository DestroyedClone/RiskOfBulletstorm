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
    public class Bomb : Equipment_V2<Bomb>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage (Default: 100% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float Bomb_DamageDealt { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Cooldown (Default: 14.00 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 14.00f;

        public override string displayName => "Bomb";
        public string descText = "Throws a bomb that explodes after a short delay";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Use For Boom\n"+descText;

        protected override string GetDescString(string langid = null) => $"{descText}, dealing <style=cIsDamage>{Pct(Bomb_DamageDealt)} damage</style>.";

        protected override string GetLoreString(string langID = null) => "Countless experienced adventurers have brought Bombs to the Gungeon seeking secret doors, only to be foiled by the existence of Blanks. Still, explosives have their place.";

        public static GameObject BombPrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;

        public static string ProjectileModelPath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Bomb.prefab";

        public Bomb()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Bomb.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/BombIcon.png";
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            GameObject commandoGrenadePrefab = Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile");
            BombPrefab = commandoGrenadePrefab.InstantiateClone("Bulletstorm_Bomb");
            var BombScale = 1.15f;
            BombPrefab.transform.localScale = new Vector3(BombScale, BombScale, BombScale);
            BombPrefab.GetComponent<ProjectileSimple>().velocity = 25; //default 50
            BombPrefab.GetComponent<ProjectileSimple>().lifetime = 5; //default 5
            BombPrefab.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Item;
            BombPrefab.GetComponent<ProjectileImpactExplosion>().falloffModel = BlastAttack.FalloffModel.None;
            //Object.Destroy(BombPrefab.GetComponent<ApplyTorqueOnStart>());

            var model = Resources.Load<GameObject>(ProjectileModelPath);
            var modelScale = 0.25f;
            model.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();

            var controller = BombPrefab.GetComponent<ProjectileController>();
            controller.ghostPrefab = model;
            //controller.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            ProjectileCatalog.getAdditionalEntries += list => list.Add(BombPrefab);

            if (BombPrefab) PrefabAPI.RegisterNetworkPrefab(BombPrefab);

            Embryo_V2.instance.Compat_Register(catalogIndex);
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>("@RiskOfBulletstorm:Assets/Models/Prefabs/Bomb.prefab");
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
            GameObject gameObject = slot.gameObject;
            if (!gameObject || !body) return false;

            //int DeployCount = instance.CheckEmbryoProc(body) ? 2 : 1; //Embryo Check

            Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            FireBomb(body, gameObject);
            if (instance.CheckEmbryoProc(body)) FireBomb(body, gameObject, 1f);
            return true;
        }

        public void FireBomb(CharacterBody body, GameObject gameObject, float yOffset = 0)
        {
            var offset = body.characterMotor.capsuleCollider.height / 3;
            var position = body.corePosition;

            var newyOffset = Vector3.up * yOffset;
            var resultpos = position + Vector3.up * offset + newyOffset;

            InputBankTest input = body.inputBank;

            if (NetworkServer.active)
            {
                ProjectileManager.instance.FireProjectile(BombPrefab, resultpos, Util.QuaternionSafeLookRotation(input.aimDirection),
                                      gameObject, body.damage * Bomb_DamageDealt,
                                      0f, Util.CheckRoll(body.crit, body.master),
                                      DamageColorIndex.Item, null, -1f);
            }
        }
    }
}
