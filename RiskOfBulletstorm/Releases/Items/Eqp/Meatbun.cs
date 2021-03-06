﻿using RiskOfBulletstorm.Utils;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class Meatbun : Equipment<Meatbun>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of maximum health should the Meatbun heal? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float PercentMaxHealthHealAmount { get; private set; } = 0.33f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much should the damage be increased by after using the Meatbun? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float AdditivePercentDamageBonus { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the max amount of buffs that Meatbun can give?", AutoConfigFlags.PreventNetMismatch)]
        public int MaxBuffLimit { get; private set; } = 5;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the minimum percentage of health lost from a single hit to remove the buffs? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float LoseBuffMaxHealthPercentage { get; private set; } = 0.01f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 90f;

        public override string displayName => "Meatbun";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            var desc = "<b>On A Roll</b>\n";
            var doHeal = PercentMaxHealthHealAmount > 0;
            var canBuff = MaxBuffLimit > 0 && AdditivePercentDamageBonus > 0;
            if (!doHeal && !canBuff)
                return desc + "Does nothing.";
            if (doHeal) desc += "Heals for a small amount. ";
            if (canBuff) desc += "Increases damage dealt until injured again.";
            return desc;
        }

        protected override string GetDescString(string langid = null)
        {
            if (PercentMaxHealthHealAmount <= 0 && MaxBuffLimit <= 0)
                return $"Does nothing.";

            var desc = $"";
            // heal amount //
            if (PercentMaxHealthHealAmount > 0) desc += $"<style=cIsHealing>Heals for {Pct(PercentMaxHealthHealAmount)} health</style>, and i";
            else desc += $"I";

            //damage bonus //
            if (MaxBuffLimit > 0)
            {
                desc += $"ncreases <style=cIsDamage>damage by +{Pct(AdditivePercentDamageBonus)}</style> until damaged";

                // health threshold
                if (LoseBuffMaxHealthPercentage > 0)
                    desc += $" by at least {Pct(LoseBuffMaxHealthPercentage)} health";
            }

            desc += $". ";

            desc += $"<style=cStack>Buff stacks up to {MaxBuffLimit} time{(MaxBuffLimit > 1 ? "s " : "")}</style>";
            if (MaxBuffLimit > 1)
                desc += $"for a max of <style=cIsDamage>+{Pct(MaxBuffLimit * AdditivePercentDamageBonus)} damage.</style>";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "A delicious, freshly baked roll! Sometimes, things just work out.";

        public BuffDef MeatbunBoost { get; private set; }

        public static GameObject ItemBodyModelPrefab;

        public Meatbun()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/Meatbun.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/Meatbun.png");
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = modelResource;
                displayRules = GenerateItemDisplayRules();
            }

            base.SetupAttributes();
            MeatbunBoost = Shared.Buffs.BuffsController.RegisterBuff(Color.white, 
                true, 
                false, 
                "Assets/Textures/Icons/Buffs/Meatbun.png",
                "Meatbun Bonus");
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
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
                    childName = "ThighR",
                    localPos = new Vector3(-0.07f, -0.12f, 0.08f),
                    localAngles = new Vector3(0, 0, 180),
                    localScale = new Vector3(0.02f, 0.02f, 0.02f)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0.1535F, -0.0758F, -0.0188F),
localAngles = new Vector3(0F, 0F, 180F),
localScale = new Vector3(0.0212F, 0.0212F, 0.0212F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0F, 1.7862F, 1.8077F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.2466F, 0.2466F, 0.2466F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.1255F, -0.0688F, 0F),
localAngles = new Vector3(2.8891F, 180F, 180F),
localScale = new Vector3(0.0173F, 0.0173F, 0.0173F)
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
childName = "ThighR",
localPos = new Vector3(-0.0535F, -0.0164F, 0.0854F),
localAngles = new Vector3(4.5671F, 180.5318F, 210.7856F),
localScale = new Vector3(0.0249F, 0.0249F, 0.0249F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.066F, -0.0156F, 0.0707F),
localAngles = new Vector3(19.4785F, 180F, 180F),
localScale = new Vector3(0.0157F, 0.0157F, 0.0157F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.2397F, -0.0008F, 0.5334F),
localAngles = new Vector3(23.1439F, 0F, 0F),
localScale = new Vector3(0.0579F, 0.0579F, 0.0579F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.0761F, 0.0803F, 0.0994F),
localAngles = new Vector3(352.0834F, 22.1066F, 123.7646F),
localScale = new Vector3(0.0375F, 0.0375F, 0.0375F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(-0.8162F, 4.4803F, 0.106F),
localAngles = new Vector3(69.4329F, 0F, 0F),
localScale = new Vector3(0.4F, 0.1555F, 0.4F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.0635F, 0.017F, 0.1588F),
localAngles = new Vector3(62.5587F, 144.0533F, 180.2169F),
localScale = new Vector3(0.0216F, 0.0216F, 0.0216F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.0508F, -0.0993F, 0.1202F),
localAngles = new Vector3(38.4163F, 212.1016F, 232.6041F),
localScale = new Vector3(0.0268F, 0.0268F, 0.0268F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(0.0202F, -0.0598F, 0.1263F),
localAngles = new Vector3(336.2439F, 353.2884F, 193.0212F),
localScale = new Vector3(0.0188F, 0.0188F, 0.0188F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.17205F, 0.03411F, -0.09819F),
                    localAngles = new Vector3(340.8231F, 180F, 136.0817F),
                    localScale = new Vector3(0.02F, 0.02F, 0.02F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Root",
localPos = new Vector3(0.1038F, 1.6327F, -0.0708F),
localAngles = new Vector3(0F, 0F, 343.2228F),
localScale = new Vector3(0.0535F, 0.0535F, 0.0535F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.3384F, 3.0393F, 1.483F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1195F, 0.1195F, 0.1195F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-6.5838F, -4.8704F, -4.3014F),
localAngles = new Vector3(356.0911F, 195.7111F, 231.5835F),
localScale = new Vector3(0.7244F, 0.7244F, 0.7244F)
                }
            });
            rules.Add("mdlEquipmentDrone", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunBarrelBase",
                    localPos = new Vector3(0f, 0f, 1.6f),
                    localAngles = new Vector3(270f, 0f, 0f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
        }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.465F, 0.5933F, -0.0299F),
                localAngles = new Vector3(330.3268F, 318.3562F, 349.8106F),
                localScale = new Vector3(0.0572F, 0.0572F, 0.0572F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Hip",
                localPos = new Vector3(2.2783F, 1.3377F, 0.1629F),
                localAngles = new Vector3(341.5744F, 50.5453F, 187.6904F),
                localScale = new Vector3(0.1833F, 0.1833F, 0.1833F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleRB",
                localPos = new Vector3(-0.0036F, -0.3107F, -0.7541F),
                localAngles = new Vector3(7.0745F, 180.1862F, 181.0716F),
                localScale = new Vector3(0.044F, 0.044F, 0.044F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(0F, 0.547F, 0.9434F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "FootL",
                localPos = new Vector3(-0.6172F, -0.7222F, 4.4737F),
                localAngles = new Vector3(319.6212F, 292.4456F, 290.8471F),
                localScale = new Vector3(0.165F, 0.165F, 0.165F)
            });
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
                        damageInfo.damage *= 1 + (MeatBunBoostCount * AdditivePercentDamageBonus);
                        //RiskOfBulletstorm.RiskofBulletstorm._logger.LogDebug("Meatbun: Increased damage from " + olddmg + " to " + damageInfo.damage + " with " + MeatBunBoostCount + " stacks");
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
                if (healthCompare >= LoseBuffMaxHealthPercentage)
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

            if (MeatBunBoostCount < MaxBuffLimit)
                body.AddBuff(MeatbunBoost);

            if (PercentMaxHealthHealAmount > 0)
            {
                health.HealFraction(PercentMaxHealthHealAmount, default);
            }
            return true;
        }
    }
}
