﻿using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;
using RiskOfBulletstorm.Shared.Buffs;


namespace RiskOfBulletstorm.Items
{
    public class EnragingPhoto : Item_V2<EnragingPhoto>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many seconds should Enraging Photo's buff last with a single stack?", AutoConfigFlags.PreventNetMismatch)]
        public float EnragingPhoto_BaseDuration { get; private set; } = 2f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional seconds of buff should each Enraging Photo after the first give? ", AutoConfigFlags.PreventNetMismatch)]
        public float EnragingPhoto_StackDuration { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Minimum percent of health loss for lower bound? (Value: Percentage", AutoConfigFlags.PreventNetMismatch)]
        public float EnragingPhoto_HealthThresholdMin { get; private set; } = 0.01f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Maximum percent of health loss for higher bound? (Value: Percentage", AutoConfigFlags.PreventNetMismatch)]
        public float EnragingPhoto_HealthThresholdMax { get; private set; } = 0.10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much should your damage be increased when Enraging Photo activates? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float EnragingPhoto_DmgBoost { get; private set; } = 1.00f;
        
        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("If true, damage to shield and barrier (from e.g. Personal Shield Generator, Topaz Brooch) will not count towards triggering Enraging Photo")]
        //public bool RequireHealth { get; private set; } = true;

        public override string displayName => "Enraging Photo";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>\"Don't Believe His Lies\"</b>\nDeal extra damage for a short time after receiving a hit.";

        protected override string GetDescString(string langid = null) => $"Gain a temporary <style=cIsDamage>{Pct(EnragingPhoto_DmgBoost)} damage bonus</style> that can last up to {EnragingPhoto_BaseDuration} seconds <style=cStack>(+{EnragingPhoto_StackDuration} seconds duration per stack).</style>" +
            $"\nThe duration scales from the amount of damage taken, from <style=cIsHealth>{Pct(EnragingPhoto_HealthThresholdMin)} to {Pct(EnragingPhoto_HealthThresholdMax)}.";

        protected override string GetLoreString(string langID = null) => "A photo that the Convict brought with her to the Gungeon.\nDeal extra damage for a short time after getting hit.\n\nOn the journey to the Breach, the Pilot once asked her why she always stared at this photo. Later, she was released from the brig.";

        public static GameObject ItemBodyModelPrefab;
        public EnragingPhoto()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/EnragingPhoto.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/EnragingPhoto.png";
        }

        public override void SetupBehavior()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>(modelResourcePath);
                displayRules = GenerateItemDisplayRules();
            }

            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>("@RiskOfBulletstorm:Assets/Models/Prefabs/EnragingPhoto.prefab");
                displayRules = GenerateItemDisplayRules();
            }
            base.SetupAttributes();
        }
        public static ItemDisplayRuleDict GenerateItemDisplayRules()
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
                    localPos = new Vector3(0.1f, 0.3f, 0.2f),
                    localAngles = new Vector3(-20, 0, 0),
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
                    localPos = new Vector3(0.07f, 0.18f, 0.18f),
                    localAngles = new Vector3(-20, 0, 0),
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
                    localPos = new Vector3(2f, 1.2f, 3.3f),
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
                    localPos = new Vector3(0.2f, 0.2f, 0.23f),
                    localAngles = new Vector3(0f, 30f, 0f),
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
                    localPos = new Vector3(0f, 0.26f, 0.06f),
                    localAngles = new Vector3(-20f, 0f, 0f),
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
                    localPos = new Vector3(0.1f, 0.2f, 0.19f),
                    localAngles = new Vector3(-10f, 0f, 0f),
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
                    localPos = new Vector3(0.3f, 0.96f, 0.1f),
                    localAngles = new Vector3(-90f, 0f, 0f),
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
                    localPos = new Vector3(0.1f, 0.34f, 0.23f),
                    localAngles = new Vector3(0f, 180f, 0f),
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
                    localPos = new Vector3(-1f, 0.5f, -2.6f),
                    localAngles = new Vector3(0f, 180f, 0f),
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
                    localPos = new Vector3(0.13f, 0.3f, 0.15f),
                    localAngles = new Vector3(-25f, 0f, 0f),
                    localScale = generalScale
                }
            });
            /*rules.Add("mdlBrother", new ItemDisplayRule[] //TODO
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
                    localPos = new Vector3(0f, 0.28f, 0.2f),
                    localAngles = new Vector3(110f, 0f, 180f),
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
                    localPos = new Vector3(0f, 0.1f, 0.45f),
                    localAngles = new Vector3(0f, 180f, 0f),
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
                    localPos = new Vector3(1.7f, -2.3f, 1.9f),
                    localAngles = new Vector3(0f, 40f, 0f),
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
                    localPos = new Vector3(8.5f, -6f, 4f),
                    localAngles = new Vector3(0f, 90f, 90f),
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
            On.RoR2.HealthComponent.TakeDamage += CalculateDamageReward;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= CalculateDamageReward;
        }
        //https://math.stackexchange.com/questions/754130/find-what-percent-x-is-between-two-numbers
        private void CalculateDamageReward(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var InventoryCount = GetCount(self.body);
            var oldHealth = self.health;
            orig(self, damageInfo);
            
            var healthLost = (oldHealth - self.health) / self.fullHealth;

            if (InventoryCount > 0 && healthLost >= EnragingPhoto_HealthThresholdMin)
            {
                var maxDuration = EnragingPhoto_BaseDuration + EnragingPhoto_StackDuration * (InventoryCount - 1);
                var scale = GetClampedPercentBetweenTwoValues(healthLost, EnragingPhoto_HealthThresholdMin, EnragingPhoto_HealthThresholdMax);

                self.body.AddTimedBuffAuthority(BuffsController.Anger, maxDuration * scale);
            }
        }

        //normalized or not?
        private float GetClampedPercentBetweenTwoValues(float value, float min, float max)
        {
            return Mathf.Clamp((value - min) / (max - min), min, max);
        }
    }
}