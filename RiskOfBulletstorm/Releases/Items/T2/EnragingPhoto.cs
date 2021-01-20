using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;


namespace RiskOfBulletstorm.Items
{
    public class EnragingPhoto : Item_V2<EnragingPhoto>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many seconds should Enraging Photo's buff last with a single stack?", AutoConfigFlags.PreventNetMismatch)]
        public float EnragingPhoto_BaseDuration { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional seconds of buff should each Enraging Photo after the first give? ", AutoConfigFlags.PreventNetMismatch)]
        public float EnragingPhoto_StackDuration { get; private set; } = 0.25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of health lost should activate the Enraging Photo's damage bonus? (Value: Percentage", AutoConfigFlags.PreventNetMismatch)]
        public float EnragingPhoto_HealthThreshold { get; private set; } = 0.11f;

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

        protected override string GetPickupString(string langID = null) => "\"Don't Believe His Lies\"\nDeal extra damage for a short time after receiving a heavy hit.";

        protected override string GetDescString(string langid = null) => $"Gain a temporary <style=cIsHealth>{Pct(EnragingPhoto_DmgBoost)} damage bonus</style> upon taking <style=cIsHealth>{Pct(EnragingPhoto_HealthThreshold)} </style> of your health that lasts {EnragingPhoto_BaseDuration} seconds." +
            $"\n<style=cStack>(+{EnragingPhoto_StackDuration} second duration per additional Enraging Photo.)</style>";

        protected override string GetLoreString(string langID = null) => "A photo that the Convict brought with her to the Gungeon.\nDeal extra damage for a short time after getting hit.\n\nOn the journey to the Breach, the Pilot once asked her why she always stared at this photo. Later, she was released from the brig.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject ItemBodyModelPrefab;
        public static readonly BuffIndex AngerBuff = GungeonBuffController.Anger;
        public EnragingPhoto()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/EnragingPhoto.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/EnragingPhotoIcon.png";
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
        private void CalculateDamageReward(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var InventoryCount = GetCount(self.body);
            var oldHealth = self.health;
            orig(self, damageInfo);
            var healthCompare = (oldHealth - self.health) / self.fullHealth;
            if (healthCompare >= EnragingPhoto_HealthThreshold)
            {
                if (InventoryCount > 0 && self.body.GetBuffCount(AngerBuff) == 0)
                {
                    self.body.AddTimedBuffAuthority(GungeonBuffController.Anger, (EnragingPhoto_BaseDuration + EnragingPhoto_StackDuration * (InventoryCount - 1)));
                }
            }
        }
    }
}