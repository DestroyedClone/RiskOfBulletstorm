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
        [AutoConfig("What is the chance for the Orange to spawn? (Default: 80.00% chance to spawn)" +
            "\n((When it is chosen by the game, it does a roll. By default, it has a 80% not to get rerolled.))", AutoConfigFlags.PreventNetMismatch)]
        public float Orange_Rarity { get; private set; } = 80.00f;

        public override string displayName => "Orange";
        public override float cooldown { get; protected set; } = 0f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "You're Not Alexander\nPermanently increases stats upon consumption.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"<style=cIsHealing>Heals for 100% health.</style> <style=cIsHealth>Permanently increases max health by 10%</style> and <style=cIsUtility>reduces equipment recharge rate by 10%</style>"+
              $"\n<style=cDeath>One-time Use.</style>";
            if (Orange_Rarity < 100f)
                desc += $"<style=cWorldEvent>{Orange_Rarity}% chance to spawn.</style>";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "With this orange, your style... it's impetuous. Your defense, impregnable.";

        public static GameObject ItemBodyModelPrefab;
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
        }
        private static ItemDisplayRuleDict GenerateItemDisplayRules()
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
            On.RoR2.PickupDropletController.CreatePickupDroplet += PickupDropletController_CreatePickupDroplet;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PickupDropletController.CreatePickupDroplet -= PickupDropletController_CreatePickupDroplet;
        }

        private void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            var body = PlayerCharacterMasterController.instances[0].master.GetBody();
            if (pickupIndex == PickupCatalog.FindPickupIndex(catalogIndex)) //if it's the orange
            {
                if (!Util.CheckRoll(Orange_Rarity, body.master)) //rarity roll
                {
                    PickupIndex loot = Run.instance.treasureRng.NextElementUniform(Run.instance.availableEquipmentDropList);
                    PickupDef def = PickupCatalog.GetPickupDef(loot);
                    pickupIndex = PickupCatalog.FindPickupIndex(def.equipmentIndex);
                }
            }
            orig(pickupIndex, position, velocity);
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;
            Inventory inventory = body.inventory;
            if (!inventory) return false;

            //int DeployCount = HelperPlugin.ClassicItemsCompat.enabled && HelperPlugin.ClassicItemsCompat.CheckEmbryoProc(instance, body) ? 2 : 1; //Embryo Check
            int DeployCount = 1;

            for (int i = 0; i < DeployCount; i++)
            {
                inventory.GiveItem(ItemIndex.BoostHp);
                inventory.GiveItem(ItemIndex.BoostEquipmentRecharge);
                health.HealFraction(1, default);
            }
            inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico

            return false;
        }
    }
}
