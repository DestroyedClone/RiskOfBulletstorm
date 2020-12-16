using RiskOfBulletstorm.Utils;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Meatbun : Equipment_V2<Meatbun>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of maximum health should the Meatbun heal? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Meatbun_HealAmount { get; private set; } = 0.33f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much should the damage be increased by after using the Meatbun? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Meatbun_DamageBonus { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the max amount of buffs that Meatbun can give?", AutoConfigFlags.PreventNetMismatch)]
        public int Meatbun_BuffLimit { get; private set; } = 5;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the minimum percentage of health lost from a single hit to remove the buffs? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Meatbun_HealthThreshold { get; private set; } = 0.05f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 90f;

        public override string displayName => "Meatbun";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            var desc = "On A Roll\n";
            var doHeal = Meatbun_HealAmount > 0;
            var canBuff = Meatbun_BuffLimit > 0 && Meatbun_DamageBonus > 0;
            if (!doHeal && !canBuff)
                return desc + "Does nothing.";
            if (doHeal) desc += "Heals for a small amount. ";
            if (canBuff) desc += "Increases damage dealt until injured again.";
            return desc;
        }

        protected override string GetDescString(string langid = null)
        {
            if (Meatbun_HealAmount <= 0 && Meatbun_BuffLimit <= 0)
                return $"Does nothing.";

            var desc = $"";
            // heal amount //
            if (Meatbun_HealAmount > 0) desc += $"<style=cIsHealing>Heals for {Pct(Meatbun_HealAmount)} health</style>, and i";
            else desc += $"I";

            //damage bonus //
            if (Meatbun_BuffLimit > 0)
            {
                desc += $"ncreases <style=cIsDamage>damage by +{Pct(Meatbun_DamageBonus)}</style> until damaged";

                // health threshold
                if (Meatbun_HealthThreshold > 0)
                    desc += $"by at least {Pct(Meatbun_HealthThreshold)} health";
            }

            desc += $".";

            desc += $"<style=cStack>Buff stacks up to {Meatbun_BuffLimit} time{(Meatbun_BuffLimit > 1 ? "s" : "")}</style>";
            if (Meatbun_BuffLimit > 1)
                desc += $"for a max of <style=cIsDamage>+{Pct(Meatbun_BuffLimit * Meatbun_DamageBonus)} damage.</style>";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "A delicious, freshly baked roll! Sometimes, things just work out.";

        public BuffIndex MeatbunBoost { get; private set; }

        public static GameObject ItemBodyModelPrefab;

        public Meatbun()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Meatbun.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/MeatbunIcon.png";
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();

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
            var dmgBuff = new CustomBuff(
            new BuffDef
            {
                buffColor = Color.white,
                canStack = true,
                isDebuff = false,
                name = "Meatbun Bonus",
            });
            MeatbunBoost = BuffAPI.Add(dmgBuff);
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
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
                    childName = "Head",
                    localPos = new Vector3(0f, 0.2f, 0.22f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.1f, 0.2f, 0.13f),
                    localAngles = new Vector3(0f, 0f, 90f),
                    localScale = new Vector3(0.08f, 0.08f, 0.08f)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 3.4f, -1.3f),
                    localAngles = new Vector3(60f, 0f, 0f),
                    localScale = generalScale * 16f
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.5f, 0.22f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 0.25f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale * 5f
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 0.13f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.1f, 0.2f),
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
                    childName = "Head",
                    localPos = new Vector3(0f, 0.05f, 0.15f),
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
                    childName = "Head",
                    localPos = new Vector3(0f, 5.2f, 0.3f),
                    localAngles = new Vector3(90f, 0f, 0f),
                    localScale = generalScale * 8
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.04f, 0.18f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
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
                    childName = "Head",
                    localPos = new Vector3(0f, 0.15f, 0.12f),
                    localAngles = new Vector3(-20f, 0f, 0f),
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
                    localPos = new Vector3(0f, 0.1f, 0.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 2f
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 2.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 10f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 5f, 0f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = generalScale * 20f
                }
            });
            //rules.Add("mdlSniper", new ItemDisplayRule[]
            //{
            //    new ItemDisplayRule
            //    {
            //        ruleType = ItemDisplayRuleType.ParentedPrefab,
            //        followerPrefab = ItemBodyModelPrefab,
            //        childName = "Head",
            //        localPos = new Vector3(0f, -0.14f, 0.1f),
            //        localAngles = new Vector3(-90f, 0f, 0f),
            //        localScale = generalScale
            //    }
            //});
            return rules;
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.HealthComponent.TakeDamage += RemoveBuffs;
            On.RoR2.HealthComponent.TakeDamage += IncreaseDmg;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= RemoveBuffs;
            On.RoR2.HealthComponent.TakeDamage -= IncreaseDmg;
        }

        private void IncreaseDmg(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                CharacterBody body = attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    int MeatBunBoostCount = body.GetBuffCount(MeatbunBoost);
                    if (MeatBunBoostCount > 0)
                    {
                        //var olddmg = (float)damageInfo.damage;
                        damageInfo.damage *= 1 + (MeatBunBoostCount * Meatbun_DamageBonus);
                        //Debug.Log("Meatbun: Increased damage from " + olddmg + " to " + damageInfo.damage + " with " + MeatBunBoostCount + " stacks");
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void RemoveBuffs(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var oldHealth = self.health;
            orig(self, damageInfo);
            var healthCompare = (oldHealth - self.health) / self.fullHealth;
            var body = self.body;
            if (body)
            {
                if (healthCompare >= Meatbun_HealthThreshold)
                {
                    int MeatBunBoostCount = body.GetBuffCount(MeatbunBoost);
                    for (int i = 0; i < MeatBunBoostCount; i++) body.RemoveBuff(MeatbunBoost);
                }
            }
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;

            int MeatBunBoostCount = body.GetBuffCount(MeatbunBoost);

            if (MeatBunBoostCount < Meatbun_BuffLimit)
                body.AddBuff(MeatbunBoost);

            if (Meatbun_HealAmount > 0)
            {
                health.HealFraction(Meatbun_HealAmount, default);
                if (ClassicItemsCompat.enabled && ClassicItemsCompat.CheckEmbryoProc(instance, body)) health.HealFraction(Meatbun_HealAmount, default);
            }
            return true;
        }
    }
}
