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
        [AutoConfig("Does the Ration need to be active to be consumed to save the holder from death?", AutoConfigFlags.PreventNetMismatch)]
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
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.13f, -0.24f),
                    localAngles = new Vector3(-90, 0, 0),
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
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.3f, -0.35f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = new Vector3(0.01f, 0.01f, 0.01f)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, -0.01f, -0.35f),
                    localAngles = new Vector3(-80f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, -0.1f, -0.15f),
                    localAngles = new Vector3(-130f, 180f, 180f),
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
                    childName = "Chest",
                    localPos = new Vector3(0f, 0f, -0.4f),
                    localAngles = new Vector3(-80f, 0f, 0f),
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
                    localPos = new Vector3(0f, 0.2f, -0.28f),
                    localAngles = new Vector3(-80f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[] //RDY
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0f, -0.3f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[] //RDY
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Root",
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
                    localPos = new Vector3(0f, -2f, -2.7f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = generalScale * 10
                }
            }) ;
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(7.4f, 0f, 6f),
                    localAngles = new Vector3(0f, -5f, -90f),
                    localScale = new Vector3(1f, 1f, 1f)
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
            return rules;
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            if (Ration_SaveFromDeathAnySlot)
                On.RoR2.HealthComponent.TakeDamage += TankHitAnySlot;
            else
                On.RoR2.HealthComponent.TakeDamage += TankHit;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            if (Ration_SaveFromDeathAnySlot)
                On.RoR2.HealthComponent.TakeDamage -= TankHitAnySlot;
            else
                On.RoR2.HealthComponent.TakeDamage -= TankHit;
        }

        //slightly more expensive because it iterates through the equipment slots
        private void TankHitAnySlot(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (Ration_SaveFromDeath)
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
                            for (int i = 0; i < equipmentStateSlots.Length; i++)
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
            if (Ration_SaveFromDeath)
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
