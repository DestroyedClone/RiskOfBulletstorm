using RiskOfBulletstorm.Utils;
using R2API;
using RoR2;
using TILER2;
using UnityEngine;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class Medkit : Equipment<Medkit>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of maximum health should the Medkit heal? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float PercentHealAmount { get; private set; } = 1.00f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of barrier should the Meatbun give? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float PercentBarrierAmount { get; private set; } = 1.00f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many seconds should the user become immune for?", AutoConfigFlags.PreventNetMismatch)]
        public float ImmuneDuration { get; private set; } = 3.00f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 0f;

        public override string displayName => "Medkit";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            if (PercentHealAmount > 0 && PercentBarrierAmount > 0) return "<b>Heals</b>\nMedkits provides substantial healing when used.";
            else return "Seems salvaged? There's nothing in it.";
        }

        protected override string GetDescString(string langid = null)
        {
            var canHeal = PercentHealAmount > 0;
            var canBarrier = PercentBarrierAmount > 0;
            if (!canHeal && !canBarrier) return $"Everything inside was emptied, it does nothing.";
            var desc = $"";
            if (canHeal) desc += $"Heals for <style=cIsHealing>{Pct(PercentHealAmount)} health</style>.";
            if (canBarrier) desc += $" Gives a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>{Pct(PercentBarrierAmount)} of your max health.</style>";
            desc += $" <style=cIsUtility>Consumes</style> on use.";
            return desc;
        }

        protected override string GetLoreString(string langID = null)
        {
            var desc = "";
            desc += (PercentHealAmount > 0 ? "Contains" : "Used to contain");
            desc += " a small piece of fairy." +
                "\nSeeking a place that would provide a near constant flow of the desperate and injured, Médecins Sans Diplôme recognized the Gungeon as the perfect place to found their practice.";
            return desc;
        }

        public static GameObject ItemBodyModelPrefab;

        public Medkit()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/Medkit.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/Medkit.png");
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
        }
        public static ItemDisplayRuleDict GenerateItemDisplayRules() //TODO new model (too big)
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
localPos = new Vector3(-0.0311F, -0.0272F, 0.0812F),
localAngles = new Vector3(0.6604F, 319.3021F, 8.2947F),
localScale = new Vector3(0.2133F, 0.2133F, 0.2133F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.1155F, 0.2018F, 0.0952F),
localAngles = new Vector3(2.5327F, 279.1555F, 107.9749F),
localScale = new Vector3(0.1303F, 0.1303F, 0.1303F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(0.2547F, 0.6413F, 0.6974F),
localAngles = new Vector3(0F, 0F, 252.5814F),
localScale = new Vector3(1.704F, 1.704F, 1.704F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.1043F, 0.2921F, 0.0137F),
localAngles = new Vector3(353.135F, 291.1656F, 359.7365F),
localScale = new Vector3(0.2055F, 0.2055F, 0.2055F)
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
childName = "Chest",
localPos = new Vector3(0.0116F, 0.2348F, 0.0686F),
localAngles = new Vector3(332.2583F, 5.0858F, 4.1172F),
localScale = new Vector3(0.113F, 0.2127F, 0.2127F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(0F, 0.1F, 0.2F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1164F, 0.1164F, 0.1164F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighBackR",
localPos = new Vector3(0.2404F, 0.3208F, 0.3327F),
localAngles = new Vector3(0F, 318.7551F, 326.2984F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "MechBase",
localPos = new Vector3(-0.0244F, 0.104F, 0.4198F),
localAngles = new Vector3(356.3706F, 354.1119F, 0F),
localScale = new Vector3(0.1679F, 0.1679F, 0.1679F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(0F, 2.4939F, 1.2197F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(1.8018F, 1.8018F, 1.8018F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.0282F, 0.0495F, 0.1747F),
localAngles = new Vector3(355.2061F, 342.1062F, 2.1824F),
localScale = new Vector3(0.1736F, 0.1736F, 0.1736F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(0.0051F, 0.1103F, 0.155F),
localAngles = new Vector3(5.2901F, 346.5381F, 269.9601F),
localScale = new Vector3(0.1396F, 0.1396F, 0.1396F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(0.0183F, 0.0669F, 0.1308F),
localAngles = new Vector3(352.5534F, 0F, 275.1813F),
localScale = new Vector3(0.2222F, 0.2222F, 0.2222F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(-0.02494F, 0.2865F, 0.10847F),
                    localAngles = new Vector3(0F, 0F, 257.8669F),
                    localScale = new Vector3(0.23737F, 0.23737F, 0.23737F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Root",
localPos = new Vector3(0.1471F, 1.6344F, -0.2342F),
localAngles = new Vector3(0F, 85.2886F, 0F),
localScale = new Vector3(0.3073F, 0.3073F, 0.3064F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0F, 2.923F, 1.6859F),
localAngles = new Vector3(276.1396F, 312.2873F, 20.0286F),
localScale = new Vector3(1.0196F, 1.0196F, 1.0196F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-1.8823F, 1.8605F, 0.6263F),
localAngles = new Vector3(343.5735F, 280.0705F, 15.9606F),
localScale = new Vector3(2.0264F, 2.0264F, 2.0264F)
                }
            });
            rules.Add("mdlEquipmentDrone", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "GunAxis",
localPos = new Vector3(0.0115F, 2.1641F, 0F),
localAngles = new Vector3(6.8322F, 87.3985F, 69.0961F),
localScale = new Vector3(0.7392F, 0.7392F, 0.7392F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.3146F, 0.4452F, -0.6225F),
                localAngles = new Vector3(354.1583F, 159.5557F, 268.4077F),
                localScale = new Vector3(0.4267F, 0.4267F, 0.4267F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "RightCalf",
                localPos = new Vector3(0.4601F, 0.803F, 0.2929F),
                localAngles = new Vector3(2.8788F, 88.404F, 60.98F),
                localScale = new Vector3(0.5148F, 0.5148F, 0.5148F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLB",
                localPos = new Vector3(-0.3293F, 0.0789F, -0.7595F),
                localAngles = new Vector3(359.9472F, 269.8208F, 359.7251F),
                localScale = new Vector3(0.7111F, 0.7111F, 0.7111F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(0F, 0.0001F, 1.4699F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.7618F, 0.7618F, 0.7618F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "FootL",
                localPos = new Vector3(-0.23F, 1.4024F, 1.3911F),
                localAngles = new Vector3(284.7958F, 189.5977F, 154.2454F),
                localScale = new Vector3(0.6946F, 0.6946F, 1.1682F)
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
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;

            var BarrierAmt = health.fullBarrier * PercentBarrierAmount;

            health.HealFraction(PercentHealAmount, default);
            health.AddBarrier(BarrierAmt);
            body.AddTimedBuff(RoR2Content.Buffs.Immune, ImmuneDuration);
            return true;
        }
    }
}
