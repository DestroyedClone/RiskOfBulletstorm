using RiskOfBulletstorm.Utils;
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class Molotov : Equipment<Molotov>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much damage should the Molotov's area of effect deal? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public static float PercentDamagePerTick { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How frequent should each damage tick happen?" +
            "\n(Default: 60 = Every 1/60th of a second) (Minimum: 1, Maximum: 60)", AutoConfigFlags.PreventNetMismatch)]
        public static float FrequencyOfTicks { get; private set; } = 60f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How long should the Molotov's area of effect last?", AutoConfigFlags.PreventNetMismatch)]
        public float DurationAoE { get; private set; } = 8f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("[ClassicItems Support] Beating Embryo: What is the damage multiplier if it procs? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        //public float Molotov_BeatingEmbryo { get; private set; } = 2.0f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 55f;

        public override string displayName => "Molotov";
        public string descText = "Upon use, throws a Molotov that ";
        public string durationNormal = "ets an area on fire";
        public string durationZero = "would have set an area on fire if it was lit.";

        public Molotov()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/Molotov.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/Molotov.png");
        }
        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            var desc = "<b>Feel the Burn</b>\n";
            desc += DurationAoE <= 0 ? durationZero : "S"+durationNormal;
            return desc;
        }

        //private readonly float DPS = Molotov_Damage * (Molotov_Frequency);
        protected override string GetDescString(string langid = null)
        {
            var desc = $"Upon use, throws a molotov that ";

            if (DurationAoE == 0)
            {
                desc += $"{durationZero}";
                return desc;
            }
            else
            {
                desc += $"s{durationNormal} dealing <style=cIsDamage>";
                // damage //
                if (PercentDamagePerTick > 0) desc += $"{Pct(PercentDamagePerTick)}";
                else desc += $"absolutely no";

                desc += $" damage per ";
                // frequency //
                desc += $"[1/{Mathf.Clamp(FrequencyOfTicks,0f,60f)}]th seconds";
                desc += $"</style> for <style=cIsDamage>";

                //duration
                if (DurationAoE > 0) desc += $"{DurationAoE} seconds";
                else desc += $"a moment in time";
                desc += $"</style>.";
                return desc;
            }
        }

        protected override string GetLoreString(string langID = null)
        {
            var desc = "Molotov cocktails aren't guns, and so they are frowned upon by long-dwelling Gungeoneers. They get the job done regardless." +
            "\nKnowing the Hegemony wouldn't let her bring her own weaponry to the Gungeon, the Convict smuggled these few bottles in with the transport's cargo.";
            if (DurationAoE <= 0) desc += "Unfortunately, the Convict brought an empty bottle.";
            return desc;
        }

        public static GameObject MolotovPrefab { get; private set; }
        public static GameObject MolotovDotZonePrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;

        public static GameObject GlassBreakEffect = Resources.Load<GameObject>("prefabs/effects/ShieldBreakEffect");
        public static string ProjectileModelPath = "Assets/Models/Prefabs/Projectiles/Molotov.prefab";

        public override void Install()
        {
            base.Install();
        }
        public override void Uninstall()
        {
            base.Uninstall();
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            //needs to be declared first
            GameObject sporeGrenadeDotZonePrefab = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectileDotZone");
            MolotovDotZonePrefab = sporeGrenadeDotZonePrefab.InstantiateClone("Bulletstorm_MolotovDotZone",true);
            MolotovDotZonePrefab.GetComponent<ProjectileDamage>().damageType = DamageType.IgniteOnHit;
            ProjectileDotZone projectileDotZone = MolotovDotZonePrefab.GetComponent<ProjectileDotZone>();
            projectileDotZone.damageCoefficient = PercentDamagePerTick;
            projectileDotZone.resetFrequency = Mathf.Clamp(FrequencyOfTicks,0f,60f);
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

            var model = assetBundle.LoadAsset<GameObject>(ProjectileModelPath);
            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();
            model.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            var controller = MolotovPrefab.GetComponent<ProjectileController>();
            controller.ghostPrefab = model;

            ProjectileAPI.Add(MolotovPrefab);
            ProjectileAPI.Add(MolotovDotZonePrefab);

            if (MolotovPrefab) PrefabAPI.RegisterNetworkPrefab(MolotovPrefab);
            if (MolotovDotZonePrefab) PrefabAPI.RegisterNetworkPrefab(MolotovDotZonePrefab);

            //if (HelperPlugin.ClassicItemsCompat.enabled)
                //HelperPlugin.ClassicItemsCompat.RegisterEmbryo(catalogIndex);
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = modelResource;
                displayRules = GenerateItemDisplayRules();
            }
            base.SetupAttributes();
        }
        public static ItemDisplayRuleDict GenerateItemDisplayRules()
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

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
childName = "HandR",
localPos = new Vector3(0.2859F, 0.912F, -0.2396F),
localAngles = new Vector3(336.3715F, 272.5471F, 263.6667F),
localScale = new Vector3(0.863F, 0.863F, 0.863F)
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
childName = "Chest",
localPos = new Vector3(0.1905F, 1.1851F, 0.2843F),
localAngles = new Vector3(311.3504F, 322.5735F, 314.3914F),
localScale = new Vector3(0.2945F, 0.2945F, 0.2945F)
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
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.18546F, 0.02517F, -0.11988F),
                    localAngles = new Vector3(11.62578F, 276.8582F, 209.6971F),
                    localScale = new Vector3(0.07F, 0.07F, 0.07F)
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
                childName = "MuzzleLB",
                localPos = new Vector3(0.3856F, 0.0024F, -0.6015F),
                localAngles = new Vector3(3.6458F, 283.1641F, 168.1485F),
                localScale = new Vector3(0.1425F, 0.1425F, 0.1425F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "BackFootL",
                localPos = new Vector3(-0.4915F, -2.4969F, -0.2926F),
                localAngles = new Vector3(351.0637F, 48.5957F, 181.0117F),
                localScale = new Vector3(0.1966F, 0.1966F, 0.1966F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "HandL",
                localPos = new Vector3(0.4853F, 0.7371F, -0.0246F),
                localAngles = new Vector3(26.5597F, 104.8015F, 280.8058F),
                localScale = new Vector3(0.4917F, 0.4816F, 0.4371F)
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
