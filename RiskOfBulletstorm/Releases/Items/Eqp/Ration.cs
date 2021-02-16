using RiskOfBulletstorm.Utils;
using RoR2;
using R2API;
using TILER2;
using UnityEngine;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Ration : Equipment_V2<Ration>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of maximum health should the Ration heal? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Ration_HealAmount { get; private set; } = 0.4f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the Ration be consumed to save the holder from death?", AutoConfigFlags.PreventNetMismatch)]
        public bool Ration_SaveFromDeath { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Does the Ration need to be active to be consumed to save the holder from death? Requires the previous config value to be enabled.", AutoConfigFlags.PreventNetMismatch)]
        public bool Ration_SaveFromDeathAnySlot { get; private set; } = true;

        public override float cooldown { get; protected set; } = 0f;

        public override string displayName => "Ration";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            var desc = "Calories, Mate\n";
            if (Ration_HealAmount > 0)
            {
                desc += "Provides healing on use. ";
                if (Ration_SaveFromDeath) desc += "If equipped, will be used automatically upon fatal damage.";
            }
            else return "Someone ate this before you did.";
            return desc;
        }

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Throws away this empty Ration.";
            if (Ration_HealAmount > 0)
            {
                desc = $"Heals for <style=cIsHealing>{Pct(Ration_HealAmount)} health.</style>";
                if (Ration_SaveFromDeath)
                    desc += $"\n<style=cIsUtility>Automatically used upon fatal damage. " +
                            $"\n</style><style=cDeath>One-Time Use.</style>";
            }
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "This MRE comes in the form of a dry and dense cookie. It doesn't taste great, but it delivers the calories the body needs.";


        public static GameObject ItemBodyModelPrefab;

        public Ration()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Ration.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Ration.png";
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
childName = "Pelvis",
localPos = new Vector3(0.0961F, 0.3304F, 0.0824F),
localAngles = new Vector3(325.1511F, 20.5061F, 124.823F),
localScale = new Vector3(0.0331F, 0.0331F, 0.0331F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.1f, 0.1f, -0.1f),
                    localAngles = new Vector3(260, 190, 150),
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
                    localPos = new Vector3(0f, 1.8f, 2.5f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0.04f, 0.04f, 0.04f)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.5601F, 0.3104F, -0.0595F),
localAngles = new Vector3(299.6462F, 303.5128F, 89.2772F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.121F, -0.0138F, 0.0454F),
localAngles = new Vector3(280F, 0F, 254.7718F),
localScale = new Vector3(0.0232F, 0.0232F, 0.0232F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.0684F, 0.0723F, -0.1989F),
localAngles = new Vector3(275.0739F, 184.5336F, 162.9035F),
localScale = new Vector3(0.0277F, 0.0277F, 0.0277F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Base",
                    localPos = new Vector3(0f, 0.9f, -1.3f),
                    localAngles = new Vector3(210f, 180f, 180f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Stomach",
localPos = new Vector3(-0.0039F, 0.1639F, 0.2031F),
localAngles = new Vector3(85.1778F, 0.3966F, 1.1393F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.5f, -2.5f),
                    localAngles = new Vector3(-77f, 0f, 0f),
                    localScale = new Vector3(0.4f, 0.4f, 0.4f)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0F, -0.0417F, 0.2049F),
localAngles = new Vector3(83.4425F, 0F, 0F),
localScale = new Vector3(0.0384F, 0.0384F, 0.0384F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmL",
localPos = new Vector3(0.1265F, 0.1693F, 0.0163F),
localAngles = new Vector3(356.6359F, 338.8943F, 261.3554F),
localScale = new Vector3(0.0294F, 0.0288F, 0.0294F)
                }
});
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Stomach",
localPos = new Vector3(0.0118F, 0.0868F, 0.1562F),
localAngles = new Vector3(73.0916F, 0F, 0F),
localScale = new Vector3(0.0248F, 0.0248F, 0.0248F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Root",
localPos = new Vector3(0.0521F, 1.6578F, 0.1099F),
localAngles = new Vector3(87.6026F, 180.0001F, 158.4953F),
localScale = new Vector3(0.125F, 0.125F, 0.125F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.0001F, 2.5833F, 2.6498F),
localAngles = new Vector3(83.282F, 0.0017F, 0.0038F),
localScale = new Vector3(0.2475F, 0.1862F, 0.2475F)
                }
            }) ;
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(6.6899F, 4.7724F, -4.3672F),
localAngles = new Vector3(314.1633F, 0.8247F, 304.1193F),
localScale = new Vector3(0.4916F, 0.4916F, 0.4916F)
                }
            });
            rules.Add("mdlEquipmentDrone", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunBarrelBase",
                    localPos = new Vector3(0f, 0f, 1.7f),
                    localAngles = new Vector3(0f, 0f, 90f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
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
                childName = "MuzzleLT",
                localPos = new Vector3(-0.1679F, 1.4223F, -0.2243F),
                localAngles = new Vector3(87.3634F, 16.5595F, 16.7779F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(0F, 0.6062F, 0.7879F),
                localAngles = new Vector3(28.1657F, 0F, 0F),
                localScale = new Vector3(0.1924F, 0.1924F, 0.1924F)
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
            if (Ration_SaveFromDeath)
            {
                if (Ration_SaveFromDeathAnySlot)
                    On.RoR2.HealthComponent.TakeDamage += TankHitAnySlot;
                else
                    On.RoR2.HealthComponent.TakeDamage += TankHit;
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            if (Ration_SaveFromDeath)
            {
                if (Ration_SaveFromDeathAnySlot)
                    On.RoR2.HealthComponent.TakeDamage -= TankHitAnySlot;
                else
                    On.RoR2.HealthComponent.TakeDamage -= TankHit;
            }
        }

        //slightly more expensive because it iterates through the equipment slots
        private void TankHitAnySlot(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var body = self.body;
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    var equipmentStateSlots = inventory.equipmentStateSlots;
                    if (equipmentStateSlots.Length > 0)
                    {
                        var endHealth = self.combinedHealth - damageInfo.damage;
                        if ((endHealth <= 0) && (!damageInfo.rejected))
                        {
                            for (int i = 0; i <= equipmentStateSlots.Length-1; i++)
                            {
                                if (equipmentStateSlots[i].equipmentIndex == catalogIndex)
                                {
                                    damageInfo.rejected = true;
                                    RationUse(self, inventory, i);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void TankHit(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var body = self.body;
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    if (inventory.GetEquipmentIndex() == catalogIndex)
                    {
                        var endHealth = self.combinedHealth - damageInfo.damage;
                        if ((endHealth <= 0) && (!damageInfo.rejected))
                        {
                            damageInfo.rejected = true;
                            RationUse(self, inventory, inventory.activeEquipmentSlot);
                        }
                    }
                }

            }
            orig(self, damageInfo);
        }

        private void RationUse(HealthComponent health, Inventory inventory, int equipmentSlot)
        {
            if (Ration_HealAmount > 0)
            {
                health.HealFraction(Ration_HealAmount, default);
            }
            inventory.equipmentStateSlots[equipmentSlot].equipmentIndex = EquipmentIndex.None;
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;
            Inventory inventory = body.inventory;
            if (!inventory) return false;

            RationUse(health, inventory, slot.activeEquipmentSlot);

            return false;
        }
    }
}
