﻿using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;
using RiskOfBulletstorm.Shared.Buffs;
using static RiskOfBulletstorm.BulletstormPlugin;


namespace RiskOfBulletstorm.Items
{
    public class EnragingPhoto : Item<EnragingPhoto>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many seconds should Enraging Photo's buff last up to with a single stack?", AutoConfigFlags.PreventNetMismatch)]
        public float BaseDuration { get; private set; } = 5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional seconds of buff should each Enraging Photo after the first give? ", AutoConfigFlags.PreventNetMismatch)]
        public float StackDuration { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the minimum amount of seconds that the buff will last for? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float MinimumDuration { get; private set; } = 1.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Minimum percent of health loss for lower bound? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float HealthThresholdMin { get; private set; } = 0.01f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Maximum percent of health loss for higher bound? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float HealthThresholdMax { get; private set; } = 0.10f;
        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("If true, damage to shield and barrier (from e.g. Personal Shield Generator, Topaz Brooch) will not count towards triggering Enraging Photo")]
        //public bool RequireHealth { get; private set; } = true;

        public override string displayName => "Enraging Photo";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>\"Don't Believe His Lies\"</b>\nDeal extra damage for a short time after receiving a hit.";

        protected override string GetDescString(string langid = null) => $"Gain a temporary <style=cIsDamage>+{Pct(BuffsController.Config_Enrage_Damage)} damage bonus</style> that can last between <style=cIsUtility>{MinimumDuration} and {BaseDuration} seconds</style> <style=cStack>(+{StackDuration} seconds duration per stack).</style>" +
            $" The duration scales from the amount of damage taken, from <style=cIsHealth>{Pct(HealthThresholdMin)} to {Pct(HealthThresholdMax)}</style> of your health.";

        protected override string GetLoreString(string langID = null) => "A photo that the Convict brought with her to the Gungeon.\nDeal extra damage for a short time after getting hit.\n\nOn the journey to the Breach, the Pilot once asked her why she always stared at this photo. Later, she was released from the brig.";

        public static GameObject ItemBodyModelPrefab;
        public EnragingPhoto()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/EnragingPhoto.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/EnragingPhoto.png");
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            if (Compat_ItemStats.enabled)
            {
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return BaseDuration + StackDuration * (count - 1); },
                    (value, inv, master) => { return $"Max Duration: {value} seconds"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return 0; },
                    (value, inv, master) => { return $"Health Threshold: {Pct(HealthThresholdMin)} to {Pct(HealthThresholdMax)}"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return BuffsController.Config_Enrage_Damage; },
                    (value, inv, master) => { return $"Damage Boost: +{Pct(value)}"; }
                ));
            }
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = modelResource;
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
localPos = new Vector3(-1.4015F, 2.0343F, 2.276F),
localAngles = new Vector3(0F, 90F, 0F),
localScale = new Vector3(0.6537F, 0.6537F, 0.6537F)
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
childName = "PlatformBase",
localPos = new Vector3(0.1301F, 0.0483F, 0.4469F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.0761F, 0.0761F, 0.0761F)
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
childName = "MuzzleHandR",
localPos = new Vector3(-0.9175F, -0.3632F, -3.1376F),
localAngles = new Vector3(351.6181F, 161.238F, 66.7744F),
localScale = new Vector3(0.8482F, 0.8482F, 0.8482F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.2304F, 0.1934F, 0.208F),
localAngles = new Vector3(356.6035F, 19.6068F, 346.2566F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandR",
localPos = new Vector3(0.0062F, 0.0937F, -0.022F),
localAngles = new Vector3(348.2784F, 121.5034F, 351.9108F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
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
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.1308F, 0.27964F, 0.13203F),
                    localAngles = new Vector3(353.2482F, 286.2403F, 5.12624F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.0008F, 0.0259F, 0.4558F),
localAngles = new Vector3(0F, 187.7865F, 0F),
localScale = new Vector3(0.1366F, 0.1366F, 0.1366F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(1.6752F, -2.3F, 1.8693F),
localAngles = new Vector3(0F, 35.5077F, 0F),
localScale = new Vector3(0.8F, 0.8F, 0.8F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(8.3363F, -6.3373F, 4.7439F),
localAngles = new Vector3(332.9239F, 93.2463F, 257.9585F),
localScale = new Vector3(2.2747F, 2.2747F, 2.2747F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.1964F, 0.4315F, -0.3215F),
                localAngles = new Vector3(357.2984F, 21.3766F, 17.3309F),
                localScale = new Vector3(0.1891F, 0.1891F, 0.1891F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(-1.8675F, 1.4199F, -1.295F),
                localAngles = new Vector3(303.7961F, 201.879F, 343.1813F),
                localScale = new Vector3(1.067F, 1.067F, 1.067F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(0.2357F, -0.6292F, 0.7553F),
                localAngles = new Vector3(345.5649F, 0F, 0F),
                localScale = new Vector3(0.3025F, 0.3025F, 0.3025F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(-0.7627F, -0.1567F, 0.2179F),
                localAngles = new Vector3(71.8031F, 0.9151F, 1.623F),
                localScale = new Vector3(0.5591F, 0.5591F, 0.5591F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Mask",
                localPos = new Vector3(0F, 0.6545F, 1.3633F),
                localAngles = new Vector3(72.6323F, 0F, 0F),
                localScale = new Vector3(0.5205F, 0.5205F, 0.5205F)
            });
            return rules;
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
            if (MinimumDuration > BaseDuration) MinimumDuration = BaseDuration;
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
        private void CalculateDamageReward(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var InventoryCount = GetCount(self.body);
            var oldHealth = self.health;
            orig(self, damageInfo);
            
            var healthLost = (oldHealth - self.health) / self.fullHealth;

            if (InventoryCount > 0 && healthLost >= HealthThresholdMin)
            {
                var maxDuration = BaseDuration + StackDuration * (InventoryCount - 1);
                var scale = Mathf.Min(GetPercentBetweenTwoValues(healthLost, HealthThresholdMin, HealthThresholdMax), HealthThresholdMax);
                var duration = maxDuration * scale;

                duration = Mathf.Min(duration, MinimumDuration);

                BulletstormPlugin._logger.LogMessage("EnragingPhotoScale = " + scale + " duration : "+duration);
                self.body.AddTimedBuffAuthority(BuffsController.Anger.buffIndex, duration);
            }
        }

        //https://math.stackexchange.com/questions/754130/find-what-percent-x-is-between-two-numbers
        //normalized or not?
        private float GetPercentBetweenTwoValues(float value, float min, float max)
        {
            return (value - min) / (max - min);
        }
    }
}