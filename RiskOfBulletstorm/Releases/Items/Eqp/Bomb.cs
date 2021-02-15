using RiskOfBulletstorm.Utils;
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Bomb : Equipment_V2<Bomb>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much damage should the Bomb deal upon explosion? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Bomb_DamageDealt { get; private set; } = 3f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 14.00f;

        public override string displayName => "<b>Bomb</b>";
        public string descText = "Throws a bomb that explodes after a short delay";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Use For Boom\n" + descText;

        protected override string GetDescString(string langid = null)
        {
            var desc = $"{descText}, dealing <style=cIsDamage>";
            if (Bomb_DamageDealt > 0) desc += $"{Pct(Bomb_DamageDealt)}";
            else desc += $"no";
            desc += $" damage</style>.";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "Countless experienced adventurers have brought Bombs to the Gungeon seeking secret doors, only to be foiled by the existence of Blanks. Still, explosives have their place.";

        public static GameObject BombPrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;

        public static string ProjectileModelPath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Projectiles/Bomb.prefab";

        public Bomb()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Bomb.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Bomb.png";
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
            var modelScale = 0.20f;
            model.transform.localScale = new Vector3(modelScale, modelScale, modelScale);
            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();

            var controller = BombPrefab.GetComponent<ProjectileController>();
            controller.ghostPrefab = model;
            //controller.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            ProjectileCatalog.getAdditionalEntries += list => list.Add(BombPrefab);

            if (BombPrefab) PrefabAPI.RegisterNetworkPrefab(BombPrefab);
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
        public static ItemDisplayRuleDict GenerateItemDisplayRules()
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
                    childName = "Pelvis",
                    localPos = new Vector3(0.3601F, 0.3967F, -0.0525F),
                    localAngles = new Vector3(333.7637F, 301.6419F, 65.6547F),
                    localScale = new Vector3(0.0302F, 0.0302F, 0.0302F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.1752F, -0.0196F, -0.041F),
localAngles = new Vector3(339.6875F, 3.2063F, 113.7526F),
localScale = new Vector3(0.0361F, 0.0361F, 0.0361F)
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
childName = "Pelvis",
localPos = new Vector3(-0.1596F, -0.0177F, -0.1006F),
localAngles = new Vector3(356.6746F, 209.6055F, 164.0859F),
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
localPos = new Vector3(-0.1683F, 0.0595F, -0.0981F),
localAngles = new Vector3(351.5492F, 6.7232F, 103.0274F),
localScale = new Vector3(0.0344F, 0.0344F, 0.0344F)
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
localPos = new Vector3(-0.0005F, 0.1335F, -0.3255F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.0473F, 0.0473F, 0.0473F)
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
childName = "Pelvis",
localPos = new Vector3(-0.2039F, -0.1275F, -0.0498F),
localAngles = new Vector3(332.4332F, 178.3598F, 181.2502F),
localScale = new Vector3(0.051F, 0.051F, 0.051F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
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
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.0016F, -0.1114F, -0.1126F),
localAngles = new Vector3(24.7509F, 126.4483F, 184.6075F),
localScale = new Vector3(0.038F, 0.038F, 0.038F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(-0.0065F, 0.3158F, 0.0871F),
localAngles = new Vector3(359.9398F, 359.856F, 310.3862F),
localScale = new Vector3(0.0705F, 0.0705F, 0.0705F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Hip",
localPos = new Vector3(-1.2477F, 0.7287F, -1.2218F),
localAngles = new Vector3(9.4161F, 352.0297F, 99.5426F),
localScale = new Vector3(0.2601F, 0.2601F, 0.2601F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(7.7047F, -5.0561F, 5.2153F),
localAngles = new Vector3(14.1626F, 97.674F, 0.9547F),
localScale = new Vector3(1F, 1F, 1F)
                }
            });
            rules.Add("mdlEquipmentDrone", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunBarrelBase",
                    localPos = new Vector3(0f, 0f, 1.5f),
                    localAngles = new Vector3(-135f, 0f, 0f),
                    localScale = generalScale * 2
                }
            });
            rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Hip",
                localPos = new Vector3(2.0102F, 1.266F, -0.1872F),
                localAngles = new Vector3(19.2145F, 302.6854F, 126.0682F),
                localScale = new Vector3(0.4502F, 0.4502F, 0.4502F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Root",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
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
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            SummonMasterBehavior summonMasterBehavior = self.gameObject.GetComponent<SummonMasterBehavior>();
            if (summonMasterBehavior && summonMasterBehavior.callOnEquipmentSpentOnPurchase)
            {
                CharacterBody characterBody = activator.GetComponent<CharacterBody>();
                if (characterBody && characterBody.inventory)
                {
                    if (characterBody.inventory.currentEquipmentIndex == catalogIndex)
                    {
                        return Interactability.ConditionsNotMet;
                    }
                }
            }
            return orig(self, activator);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PurchaseInteraction.GetInteractability -= PurchaseInteraction_GetInteractability;
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            GameObject gameObject = slot.gameObject;
            if (!gameObject || !body) return false;

            Util.PlaySound(FireMines.throwMineSoundString, gameObject);

            var angle = Util.QuaternionSafeLookRotation(slot.GetAimRay().direction);

            FireBomb(body, gameObject, angle);
            return true;
        }

        public void FireBomb(CharacterBody body, GameObject gameObject, Quaternion throwAngle, float yOffset = 0)
        {
            var offset = body.characterMotor.capsuleCollider.height / 3;
            var position = body.corePosition;

            var newyOffset = Vector3.up * yOffset;
            var resultpos = position + Vector3.up * offset + newyOffset;

            if (NetworkServer.active)
            {
                ProjectileManager.instance.FireProjectile(BombPrefab, resultpos, throwAngle,
                                      gameObject, body.damage * Bomb_DamageDealt,
                                      0f, Util.CheckRoll(body.crit, body.master),
                                      DamageColorIndex.Item, null, -1f);
            }
        }
    }
}
