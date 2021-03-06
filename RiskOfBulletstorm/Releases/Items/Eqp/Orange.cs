﻿using RiskOfBulletstorm.Utils;
using RoR2;
using R2API;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.BulletstormPlugin;
using static R2API.RecalculateStatsAPI;

namespace RiskOfBulletstorm.Items
{
    public class Orange : Equipment<Orange>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the consumed amount of Orange show in your inventory?", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableShowConsumed { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the percentage of health to heal per use?", AutoConfigFlags.PreventNetMismatch)]
        public float PercentMaxHealthHeal { get; private set; } = 1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the percentage of max health to increase per use?", AutoConfigFlags.PreventNetMismatch)]
        public float PercentMaxHealthAdd { get; private set; } = 0.1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the equipment recharge reduction per use?", AutoConfigFlags.PreventNetMismatch)]
        public float PercentEquipmentRecharge { get; private set; } = 0.1f;
        public override string displayName => "Orange";
        public override float cooldown { get; protected set; } = 0f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>You're Not Alexander</b>\nPermanently increases stats upon consumption.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"<style=cIsHealing>Heals for {Pct(PercentMaxHealthHeal)} health,</style> <style=cIsHealth>increases max health by {Pct(PercentMaxHealthAdd)}</style>, and <style=cIsUtility>reduces equipment recharge rate by {Pct(PercentEquipmentRecharge)}</style>. <style=cIsUtility>Consumes</style> on use.";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "With this orange, your style... it's impetuous. Your defense, impregnable.";

        public static ItemDef OrangeConsumedDef;
        public static GameObject ItemBodyModelPrefab;
        public Orange()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/Orange.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/Orange.png");
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = modelResource;
                displayRules = GenerateItemDisplayRules();
            }
            base.SetupAttributes();

            OrangeConsumedDef = ScriptableObject.CreateInstance<ItemDef>();
            OrangeConsumedDef.hidden = !EnableShowConsumed;
            OrangeConsumedDef.name = modInfo.shortIdentifier + "ORANGETALLY_NAME";
            OrangeConsumedDef.tier = ItemTier.NoTier;
            OrangeConsumedDef.canRemove = false;
            OrangeConsumedDef.nameToken = OrangeConsumedDef.name;
            OrangeConsumedDef.descriptionToken = modInfo.shortIdentifier + "_ORANGETALLY_DESC";
            OrangeConsumedDef.pickupToken = OrangeConsumedDef.descriptionToken;
            OrangeConsumedDef.loreToken = "";
            OrangeConsumedDef.pickupIconSprite = iconResource;
            ItemAPI.Add(new CustomItem(OrangeConsumedDef, new ItemDisplayRuleDict()));
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
localPos = new Vector3(1F, 0.5F, 1.4F),
localAngles = new Vector3(357.4331F, 345.2709F, 260.3318F),
localScale = new Vector3(0.7409F, 0.7391F, 0.7391F)
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
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "FlowerBase",
localPos = new Vector3(0.8143F, 1.4498F, 0.672F),
localAngles = new Vector3(0.4932F, 333.8098F, 282.4889F),
localScale = new Vector3(0.1515F, 0.1515F, 0.1515F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "FlowerBase",
localPos = new Vector3(-0.0186F, 1.4495F, 0.8039F),
localAngles = new Vector3(62.9394F, 74.8923F, 0F),
localScale = new Vector3(0.095F, 0.095F, 0.095F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "FlowerBase",
localPos = new Vector3(0.5237F, 1.4812F, -0.9807F),
localAngles = new Vector3(18.5778F, 63.8499F, 246.666F),
localScale = new Vector3(0.172F, 0.172F, 0.172F)
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
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.19969F, 0.42845F, -0.15987F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
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
                localPos = new Vector3(0F, 1.414F, 0.7129F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.434F, 0.434F, 0.434F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleJar",
                localPos = new Vector3(2.4934F, -2.4108F, 3.2535F),
                localAngles = new Vector3(0F, 0F, 225.9641F),
                localScale = new Vector3(1.0394F, 1.0394F, 1.0394F)
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
            GetStatCoefficients += StatHooks_GetStatCoefficients;
            On.RoR2.Inventory.CalculateEquipmentCooldownScale += Inventory_CalculateEquipmentCooldownScale;
        }

        public override void InstallLanguage()
        {
            base.InstallLanguage();
            LanguageAPI.Add(OrangeConsumedDef.nameToken, "Oranges (Consumed)");
            LanguageAPI.Add(OrangeConsumedDef.descriptionToken, "Per stack, grants <style=cIsHealing>+" + Pct(PercentMaxHealthAdd) + " maximum health</style> and <style=cIsUtility>+" + Pct(PercentEquipmentRecharge) + " reduced equipment recharge rate</style>.");
        }
        private float Inventory_CalculateEquipmentCooldownScale(On.RoR2.Inventory.orig_CalculateEquipmentCooldownScale orig, Inventory self)
        {
            return orig(self) * Mathf.Pow(1f-PercentEquipmentRecharge, self.GetItemCount(OrangeConsumedDef));
        }

        private void StatHooks_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var itemCount = sender.inventory.GetItemCount(OrangeConsumedDef);
                if (itemCount > 0)
                {
                    args.healthMultAdd += PercentMaxHealthAdd * itemCount;
                }
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients -= StatHooks_GetStatCoefficients;
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
            inventory.GiveItem(OrangeConsumedDef);
            health.HealFraction(1, default);

            return false;
        }
    }
}
