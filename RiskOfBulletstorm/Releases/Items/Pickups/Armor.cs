using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using RoR2;
using R2API;
using TILER2;
using UnityEngine;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.Shared.Blanks.MasterBlankItem;


namespace RiskOfBulletstorm.Items
{
    public class Armor : Item_V2<Armor>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should Armor activate a blank when broken?", AutoConfigFlags.PreventNetMismatch)]
        public bool Armor_ActivateBlank { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the minimum amount of damage that Armor should be consumed for? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Armor_HealthThreshold { get; private set; } = 0.20f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should Armor protect against fatal damage? This takes priority over the required minimum amount of damage.")]
        public bool Armor_ProtectDeath { get; private set; } = false;

        public override string displayName => "Armor";
        public string descText = "Prevents a single hit";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Protect Body\n"+descText+" from heavy hits";

        protected override string GetDescString(string langid = null)
        {
            string descString = $"<style=cIsUtility>{descText}</style> that would have exceeded <style=cIsDamage>{Pct(Armor_HealthThreshold)} health</style>";
            if (Armor_ActivateBlank) descString += $"\nand <style=cIsUtility>activates a Blank</style>";
            if (Armor_ProtectDeath) descString += $"\n </style=cIsUtility>Also protects from death.</style>" +
                    $"\nConsumed on use.";
            return descString;
        }

        protected override string GetLoreString(string langID = null) => "The blue of this shield was formed from the shavings of a Blank." +
            "Dents into the weak, aluminium metal from bullets and projectiles trigger the power of the Blank.";

        public static GameObject ItemBodyModelPrefab;

        public Armor()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Armor.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/ArmorIcon.png";
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
                    localPos = new Vector3(0f, 0.3f, -0.2f),
                    localAngles = new Vector3(-20, 180, 0),
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
                    localPos = new Vector3(0f, 0.1f, -0.15f),
                    localAngles = new Vector3(0, 170, 0),
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
                    localPos = new Vector3(0f, 1.6f, 2.5f),
                    localAngles = new Vector3(270f, 0f, 0f),
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
                    localPos = new Vector3(0f, 0.2f, -0.3f),
                    localAngles = new Vector3(0f, 180f, 0f),
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
                    localPos = new Vector3(0f, -0.05f, 0.16f),
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
                    localPos = new Vector3(0f, 0.1f, -0.2f),
                    localAngles = new Vector3(0f, 180f, 0f),
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
                    localAngles = new Vector3(0f, 90f, 0f),
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
                    localPos = new Vector3(0f, 0.025f, -0.37f),
                    localAngles = new Vector3(-5f, 180f, 0f),
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
                    localPos = new Vector3(0f, 0.5f, -2.4f),
                    localAngles = new Vector3(-15f, 180f, 0f),
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
                    localPos = new Vector3(0f, 0.2f, -0.25f),
                    localAngles = new Vector3(0f, 180f, 0f),
                    localScale = generalScale
                }
            });
            /*rules.Add("mdlBrother", new ItemDisplayRule[] //blacklisted
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
            rules.Add("mdlBandit", new ItemDisplayRule[] //tpdp
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "None",
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
                    childName = "None",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 2.5f
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[] //todo
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "None",
                    localPos = new Vector3(0f, 0f, -2f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 8f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[] //todo
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "None",
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
            On.RoR2.HealthComponent.TakeDamage += TankHit;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= TankHit;
        }
        private void TankHit(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var InventoryCount = GetCount(self.body);
            var health = self.combinedHealth;
            var endHealth = health - damageInfo.damage;

            if (InventoryCount > 0)
            {
                if (
                    (
                        (Armor_ProtectDeath && endHealth <= 0 ) || // If it protects from death and you would have died, *OR*
                        (endHealth / self.fullHealth >= Armor_HealthThreshold) ) &&  // the damage dealt exceeds armor threshold
                        (!damageInfo.rejected) //and its not rejected
                    )
                {
                    damageInfo.rejected = true;
                    self.body.inventory.RemoveItem(catalogIndex);

                    if (Armor_ActivateBlank) FireBlank(self.body, self.body.corePosition, 6f, 1f, -1);
                }
            }
            orig(self, damageInfo);
        }
    }
}
