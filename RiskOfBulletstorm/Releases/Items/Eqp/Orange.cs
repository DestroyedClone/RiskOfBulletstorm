using RiskOfBulletstorm.Utils;
using RoR2;
using R2API;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Orange : Equipment_V2<Orange>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the consumed amount of Orange show in your inventory?", AutoConfigFlags.PreventNetMismatch)]
        public bool Orange_ShowConsumed { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the percentage of health to heal per use?", AutoConfigFlags.PreventNetMismatch)]
        public float Orange_HealAmount { get; private set; } = 1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the percentage of max health to increase per use?", AutoConfigFlags.PreventNetMismatch)]
        public float Orange_HealthMultAdd { get; private set; } = 0.1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the equipment recharge reduction per use?", AutoConfigFlags.PreventNetMismatch)]
        public float Orange_EquipmentReduce { get; private set; } = 0.1f;
        public override string displayName => "Orange";
        public override float cooldown { get; protected set; } = 0f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>You're Not Alexander</b>\nPermanently increases stats upon consumption.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"<style=cIsHealing>Heals for {Pct(Orange_HealAmount)} health,</style> <style=cIsHealth>increases max health by {Pct(Orange_HealthMultAdd)}</style>, and <style=cIsUtility>reduces equipment recharge rate by {Pct(Orange_EquipmentReduce)}</style>.";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "With this orange, your style... it's impetuous. Your defense, impregnable.";

        public static ItemIndex OrangeConsumedIndex;
        public static GameObject ItemBodyModelPrefab;
        public Orange()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Orange.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Orange.png";
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>(modelResourcePath);
                displayRules = GenerateItemDisplayRules();
            }
            base.SetupAttributes();

            var OrangeConsumedDef = new CustomItem(new ItemDef
            {
                hidden = !Orange_ShowConsumed,
                name = "OrangeTally",
                tier = ItemTier.NoTier,
                pickupIconPath = iconResourcePath,
                canRemove = false
            }, new ItemDisplayRuleDict(null));
            OrangeConsumedIndex = ItemAPI.Add(OrangeConsumedDef);
        }
        public static ItemDisplayRuleDict GenerateItemDisplayRules()
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.05f, 0.05f, 0.05f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.1f, -0.22f, 0.1f),
                    localAngles = new Vector3(0, 0, 180),
                    localScale = new Vector3(0.08f, 0.08f, 0.08f)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.15f, 0f, 0.15f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = new Vector3(0.08f, 0.08f, 0.08f)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(1f, 0.5f, 1.4f),
                    localAngles = new Vector3(350f, 270f, -90f),
                    localScale = new Vector3(0.08f, 0.08f, 0.08f)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.2f, -0.15f, 0.1f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.1f, 0f, 0.17f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.07f, -0.05f, 0.16f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[] //RDY
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Base",
                    localPos = new Vector3(0f, 1.2f, 0.2f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = generalScale * 8f
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.08f, 0f, 0.2f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(-0.1532F, 5.1731F, -0.0492F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.4F, 0.4F, 0.2444F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighL",
localPos = new Vector3(0.0925F, -0.0508F, 0.131F),
localAngles = new Vector3(344.9872F, 42.5367F, 166.6307F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighL",
localPos = new Vector3(0.11F, -0.1057F, 0.1511F),
localAngles = new Vector3(337.0038F, 0F, 180F),
localScale = new Vector3(0.1145F, 0.1145F, 0.1145F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0f, -0.08f, 0.12f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(-0.06F, 0.7F, 0F),
localAngles = new Vector3(0F, 310F, 0F),
localScale = new Vector3(0.4428F, 0.4428F, 0.4428F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighL",
localPos = new Vector3(-1.0361F, -0.7711F, 1.0911F),
localAngles = new Vector3(323.5937F, 14.1549F, 146.2097F),
localScale = new Vector3(0.5F, 0.5F, 0.5F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighL",
localPos = new Vector3(3.4671F, 0.0899F, 0.0734F),
localAngles = new Vector3(329.9547F, 357.365F, 205.222F),
localScale = new Vector3(1.4194F, 1.4194F, 1.4194F)
                }
            });
            rules.Add("mdlEquipmentDrone", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunBarrelBase",
                    localPos = new Vector3(-0.07f, 0f, 1.3f),
                    localAngles = new Vector3(270f, 0f, 0f),
                    localScale = new Vector3(0.4f, 0.4f, 0.4f)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.0181F, 0.1226F, -0.2116F),
                localAngles = new Vector3(55.2748F, 16.6397F, 343.2494F),
                localScale = new Vector3(0.0838F, 0.0838F, 0.0838F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Hip",
                localPos = new Vector3(-1.9763F, -0.4931F, 0.0051F),
                localAngles = new Vector3(7.3442F, 60.5384F, 170.2101F),
                localScale = new Vector3(1.009F, 1.009F, 1.009F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(0F, -0.0948F, 0.618F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.1405F, 0.1405F, 0.1363F)
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
            StatHooks.GetStatCoefficients += StatHooks_GetStatCoefficients;
            On.RoR2.Inventory.CalculateEquipmentCooldownScale += Inventory_CalculateEquipmentCooldownScale;
        }

        public override void InstallLanguage()
        {
            base.InstallLanguage();
            LanguageAPI.Add("ITEM_ORANGETALLY_NAME", "Oranges (Consumed)");
            LanguageAPI.Add("ITEM_ORANGETALLY_DESC", "Per stack, grants <style=cIsHealth>+" + Pct(Orange_HealthMultAdd) + " maximum health</style> and <style=cIsUtility>+" + Pct(Orange_EquipmentReduce) + " reduced equipment recharge rate</style>.");
        }
        private float Inventory_CalculateEquipmentCooldownScale(On.RoR2.Inventory.orig_CalculateEquipmentCooldownScale orig, Inventory self)
        {
            return orig(self) * Mathf.Pow(1f-Orange_EquipmentReduce, self.GetItemCount(OrangeConsumedIndex));
        }

        private void StatHooks_GetStatCoefficients(CharacterBody sender, StatHooks.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var itemCount = sender.inventory.GetItemCount(OrangeConsumedIndex);
                if (itemCount > 0)
                {
                    args.healthMultAdd += Orange_HealthMultAdd * itemCount;
                }
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            StatHooks.GetStatCoefficients -= StatHooks_GetStatCoefficients;
            On.RoR2.Inventory.CalculateEquipmentCooldownScale -= Inventory_CalculateEquipmentCooldownScale;
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;
            Inventory inventory = body.inventory;
            if (!inventory) return false;

            inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico
            inventory.GiveItem(OrangeConsumedIndex);
            health.HealFraction(1, default);

            return false;
        }
    }
}
