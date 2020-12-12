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
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/MolotovIcon.png";
        }
        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            var desc = "Feel the Burn\n";
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
                desc += $" </style>.";
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

            var model = Resources.Load<GameObject>(modelResourcePath);
            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();
            model.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);

            var controller = MolotovPrefab.GetComponent<ProjectileController>();
            controller.ghostPrefab = model;

            ProjectileCatalog.getAdditionalEntries += list => list.Add(MolotovPrefab);
            ProjectileCatalog.getAdditionalEntries += list => list.Add(MolotovDotZonePrefab);

            if (MolotovPrefab) PrefabAPI.RegisterNetworkPrefab(MolotovPrefab);
            if (MolotovDotZonePrefab) PrefabAPI.RegisterNetworkPrefab(MolotovDotZonePrefab);

            if (ClassicItemsCompat.enabled)
                ClassicItemsCompat.RegisterEmbryo(catalogIndex);
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

            Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            var damageMult = 1f;
            if (ClassicItemsCompat.enabled && ClassicItemsCompat.CheckEmbryoProc(instance, body)) damageMult = Molotov_BeatingEmbryo;
            FireMolotov(body, gameObject, damageMult);
            return true;
        }

        public void FireMolotov(CharacterBody body, GameObject gameObject, float damageMultiplier = 1f)
        {
            InputBankTest input = body.inputBank;
            
            var offset = body.characterMotor.capsuleCollider.height / 3;
            var position = body.corePosition;
            var resultpos = position + Vector3.up * offset;

            if (NetworkServer.active)
            {
                ProjectileManager.instance.FireProjectile(MolotovPrefab, resultpos, Util.QuaternionSafeLookRotation(input.aimDirection),
                                      gameObject, body.damage * Molotov_Damage * damageMultiplier,
                                      0f, Util.CheckRoll(body.crit, body.master),
                                      DamageColorIndex.Item, null, -1f);
            }
        }
    }
}
