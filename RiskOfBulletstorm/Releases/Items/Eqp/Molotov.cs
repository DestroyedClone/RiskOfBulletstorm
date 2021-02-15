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
    public class Molotov : Equipment_V2<Molotov>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much damage should the Molotov's area of effect deal? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public static float Molotov_Damage { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How frequent should each damage tick happen?" +
            "\n(Default: 60 = Every 1/60th of a second) (Minimum: 1, Maximum: 60)", AutoConfigFlags.PreventNetMismatch)]
        public static float Molotov_Frequency { get; private set; } = 60f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How long should the Molotov's area of effect last?", AutoConfigFlags.PreventNetMismatch)]
        public float Molotov_Duration { get; private set; } = 8f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("[ClassicItems Support] Beating Embryo: What is the damage multiplier if it procs? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Molotov_BeatingEmbryo { get; private set; } = 2.0f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 55f;

        public override string displayName => "Molotov";
        public string descText = "Upon use, throws a Molotov that ";
        public string durationNormal = "sets an area on fire";
        public string durationZero = "would have set an area on fire if you actually lit it.";

        public Molotov()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Molotov.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Molotov.png";
        }
        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            var desc = "<b>Feel the Burn</b>\n";
            if (Molotov_Duration <= 0)
                desc += durationZero;
            else desc += durationNormal;
            return desc;
        }

        //private readonly float DPS = Molotov_Damage * (Molotov_Frequency);
        protected override string GetDescString(string langid = null)
        {
            var desc = $"Upon use, throws a molotov that ";

            if (Molotov_Duration == 0)
            {
                desc += $"{durationZero}";
                return desc;
            }
            else
            {
                desc += $"{durationNormal} dealing <style=cIsDamage>";
                // damage //
                if (Molotov_Damage > 0) desc += $"{Pct(Molotov_Damage)}";
                else desc += $"absolutely no";

                desc += $" damage per ";
                // frequency //
                desc += $"[1/{Mathf.Clamp(Molotov_Frequency,0f,60f)}]th seconds";
                desc += $" </style> for <style=cIsDamage>";

                //duration
                if (Molotov_Duration > 0) desc += $"{Molotov_Duration} seconds";
                else desc += $"a moment in time";
                desc += $"</style>.";
                return desc;
            }
        }

        protected override string GetLoreString(string langID = null)
        {
            var desc = "Molotov cocktails aren't guns, and so they are frowned upon by long-dwelling Gungeoneers. They get the job done regardless." +
            "\nKnowing the Hegemony wouldn't let her bring her own weaponry to the Gungeon, the Convict smuggled these few bottles in with the transport's cargo.";
            if (Molotov_Duration <= 0) desc += "Unfortunately, the Convict forgot to bring a lighter.";
            return desc;
        }

        public static GameObject MolotovPrefab { get; private set; }
        public static GameObject MolotovDotZonePrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;

        public static GameObject GlassBreakEffect = Resources.Load<GameObject>("prefabs/effects/ShieldBreakEffect");
        public static string ProjectileModelPath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Projectiles/Molotov.prefab";

        public override void Install()
        {
            base.Install();
            On.RoR2.PurchaseInteraction.GetInteractability += PreventEquipmentDroneGive;
        }
        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PurchaseInteraction.GetInteractability -= PreventEquipmentDroneGive;
        }
        private Interactability PreventEquipmentDroneGive(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
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

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            //needs to be declared first
            GameObject sporeGrenadeDotZonePrefab = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectileDotZone");
            MolotovDotZonePrefab = sporeGrenadeDotZonePrefab.InstantiateClone("Bulletstorm_MolotovDotZone");
            MolotovDotZonePrefab.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
            ProjectileDotZone projectileDotZone = MolotovDotZonePrefab.GetComponent<ProjectileDotZone>();
            projectileDotZone.damageCoefficient = Molotov_Damage;
            projectileDotZone.resetFrequency = Mathf.Clamp(Molotov_Frequency,0f,60f);
            projectileDotZone.lifetime = Molotov_Duration;
            projectileDotZone.impactEffect = GlassBreakEffect;

            GameObject sporeGrenadePrefab = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectile");
            MolotovPrefab = sporeGrenadePrefab.InstantiateClone("Bulletstorm_Molotov");
            MolotovPrefab.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Item;

            var PIE = MolotovPrefab.GetComponent<ProjectileImpactExplosion>();
            if (Molotov_Duration > 0) PIE.childrenProjectilePrefab = MolotovDotZonePrefab; 
                else Object.Destroy(PIE);
            MolotovPrefab.GetComponent<ProjectileSimple>().velocity = 35; //50

            ApplyTorqueOnStart applyTorque = MolotovPrefab.AddComponent<ApplyTorqueOnStart>();
            applyTorque.randomize = true;
            applyTorque.localTorque = new Vector3(400f, 10f, 400f);

            var model = Resources.Load<GameObject>(ProjectileModelPath);
            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();
            model.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            var controller = MolotovPrefab.GetComponent<ProjectileController>();
            controller.ghostPrefab = model;

            ProjectileCatalog.getAdditionalEntries += list => list.Add(MolotovPrefab);
            ProjectileCatalog.getAdditionalEntries += list => list.Add(MolotovDotZonePrefab);

            if (MolotovPrefab) PrefabAPI.RegisterNetworkPrefab(MolotovPrefab);
            if (MolotovDotZonePrefab) PrefabAPI.RegisterNetworkPrefab(MolotovDotZonePrefab);

            //if (HelperPlugin.ClassicItemsCompat.enabled)
                //HelperPlugin.ClassicItemsCompat.RegisterEmbryo(catalogIndex);
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
localPos = new Vector3(0.3774F, 0.3064F, -0.0251F),
localAngles = new Vector3(357.5796F, 275.4139F, 120.2752F),
localScale = new Vector3(0.0395F, 0.0395F, 0.0395F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.194F, 0.0085F, -0.0482F),
localAngles = new Vector3(359.5396F, 358.5087F, 145.6839F),
localScale = new Vector3(0.0481F, 0.0481F, 0.0481F)
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
childName = "HandR",
localPos = new Vector3(-0.1211F, 0.3016F, -0.0327F),
localAngles = new Vector3(331.7263F, 191.556F, 180F),
localScale = new Vector3(0.0556F, 0.0556F, 0.0556F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "LowerArmR",
localPos = new Vector3(0.0605F, 0.3991F, -0.0337F),
localAngles = new Vector3(351.7308F, 355.9752F, 281.1017F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "LowerArmR",
localPos = new Vector3(0.0334F, 0.4329F, 0.0284F),
localAngles = new Vector3(0.6461F, 349.6371F, 80.3874F),
localScale = new Vector3(0.0672F, 0.0672F, 0.0672F)
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
childName = "Pelvis",
localPos = new Vector3(0.2644F, 0.2079F, -0.0232F),
localAngles = new Vector3(27.2354F, 123.9368F, 160.7983F),
localScale = new Vector3(0.0597F, 0.0597F, 0.0597F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(1.6463F, 0.6539F, -0.9624F),
localAngles = new Vector3(355.6559F, 332.0683F, 261.8693F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandR",
localPos = new Vector3(0.0149F, 0.1615F, -0.0084F),
localAngles = new Vector3(87.0354F, 0F, 0F),
localScale = new Vector3(0.0587F, 0.0587F, 0.0587F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[] 
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.1972F, -0.0935F, -0.0374F),
localAngles = new Vector3(7.6113F, 51.273F, 210.9014F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Stomach",
localPos = new Vector3(-0.1384F, 0.0762F, 0.0726F),
localAngles = new Vector3(9.6482F, 301.1407F, 358.7223F),
localScale = new Vector3(0.0606F, 0.0606F, 0.0425F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0F, 0F, 0.0935F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighL",
localPos = new Vector3(-0.6801F, -0.2937F, 0.4023F),
localAngles = new Vector3(8.3053F, 78.6191F, 133.4121F),
localScale = new Vector3(0.3992F, 0.3992F, 0.3992F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(8.5F, -6.758F, 5.1964F),
localAngles = new Vector3(353.9065F, 75.2851F, 214.7639F),
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
                    localPos = new Vector3(0f, 0f, 1.8f),
                    localAngles = new Vector3(270f, 0f, 0f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "UpperArmL",
                localPos = new Vector3(-0.1028F, 0.35F, 0.201F),
                localAngles = new Vector3(356.0924F, 154.3724F, 136.4113F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Hip",
                localPos = new Vector3(-2.8484F, 1.6876F, 0.0615F),
                localAngles = new Vector3(326.8837F, 82.4952F, 226.8136F),
                localScale = new Vector3(0.6082F, 0.6082F, 0.6082F)
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
        } //todo
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;

            GameObject gameObject = slot.gameObject;

            Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            var damageMult = 1f;
            var angle = Util.QuaternionSafeLookRotation(slot.GetAimRay().direction);
            FireMolotov(body, gameObject, angle, damageMult);
            return true;
        }

        public void FireMolotov(CharacterBody body, GameObject gameObject, Quaternion throwAngle, float damageMultiplier = 1f)
        {
            var offset = body.characterMotor.capsuleCollider.height / 3;
            var position = body.corePosition;
            var resultpos = position + Vector3.up * offset;

            if (NetworkServer.active)
            {
                ProjectileManager.instance.FireProjectile(MolotovPrefab, resultpos, throwAngle,
                                      gameObject, body.damage * Molotov_Damage * damageMultiplier,
                                      0f, Util.CheckRoll(body.crit, body.master),
                                      DamageColorIndex.Item, null, -1f);
            }
        }
    }
}
