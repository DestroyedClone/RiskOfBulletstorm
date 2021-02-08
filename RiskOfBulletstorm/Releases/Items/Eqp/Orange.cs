using RiskOfBulletstorm.Utils;
using RoR2;
using R2API;
using UnityEngine;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class Orange : Equipment_V2<Orange>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the benefits of Orange show in your inventory?", AutoConfigFlags.PreventNetMismatch)]
        public bool Orange_ShowConsumed { get; private set; } = true;
        public override string displayName => "Orange";
        public override float cooldown { get; protected set; } = 0f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>You're Not Alexander</b>\nPermanently increases stats upon consumption.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"<style=cIsHealing>Heals for 100% health,</style> <style=cIsHealth>permanently increases max health by 10%</style>, and <style=cIsUtility>reduces equipment recharge rate by 10%</style>.";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "With this orange, your style... it's impetuous. Your defense, impregnable.";

        public static ItemIndex OrangeConsumedIndex;
        public static GameObject ItemBodyModelPrefab;
        public float healthMultAdd = 0.1f;
        public float cooldownReduction = 0.1f;
        public Orange()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Orange.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Orange.png";
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();

           // if (HelperPlugin.ClassicItemsCompat.enabled)
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
                    childName = "ThighL",
                    localPos = new Vector3(1.7f, 0.5f, 0f),
                    localAngles = new Vector3(90f, 45f, 0f),
                    localScale = generalScale * 8
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.17f, 0.16f, 0.15f),
                    localAngles = new Vector3(-20f, 0f, 180f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.11f, 0f, 0.11f),
                    localAngles = new Vector3(0, 0, 180),
                    localScale = generalScale
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
                    localPos = new Vector3(-0.06f, 0.7f, 0f),
                    localAngles = new Vector3(0f, -50f, 0f),
                    localScale = new Vector3(0.43f, 0.43f, 0.43f)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(-0.8f, 0f, 0.4f),
                    localAngles = new Vector3(130f, 0f, 180f),
                    localScale = generalScale * 10f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(3f, 0f, 0f),
                    localAngles = new Vector3(-90f, 0f, 190f),
                    localScale = generalScale * 20f
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

        private float Inventory_CalculateEquipmentCooldownScale(On.RoR2.Inventory.orig_CalculateEquipmentCooldownScale orig, Inventory self)
        {
            return orig(self) * Mathf.Pow(cooldownReduction, self.GetItemCount(OrangeConsumedIndex));
        }

        private void StatHooks_GetStatCoefficients(CharacterBody sender, StatHooks.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var itemCount = sender.inventory.GetItemCount(OrangeConsumedIndex);
                if (itemCount > 0)
                {
                    args.healthMultAdd += 0.1f * itemCount;
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
