using RiskOfBulletstorm.Utils;
using R2API;
using RoR2;
using TILER2;
using UnityEngine;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Medkit : Equipment_V2<Medkit>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of maximum health should the Medkit heal? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Medkit_HealAmount { get; private set; } = 0.75f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of barrier should the Meatbun give? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Medkit_BarrierAmount { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 100f;

        public override string displayName => "Medkit";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            if (Medkit_HealAmount > 0 && Medkit_BarrierAmount > 0) return "<b>Heals</b>\nMedkits provides substantial healing when used.";
            else return "Seems salvaged?";
        }

        protected override string GetDescString(string langid = null)
        {
            var canHeal = Medkit_HealAmount > 0;
            var canBarrier = Medkit_BarrierAmount > 0;
            if (!canHeal && !canBarrier) return $"Everything inside was emptied, it does nothing.";
            var desc = $"";
            if (canHeal) desc += $"Heals for <style=cIsHealing>{Pct(Medkit_HealAmount)} health</style>. ";
            if (canBarrier) desc += $"Gives a <style=cIsUtility>temporary barrier for {Pct(Medkit_BarrierAmount)} of your max health.</style>";
            return desc;
        }

        protected override string GetLoreString(string langID = null)
        {
            var desc = "";
            if (Medkit_HealAmount > 0)
                desc += "Contains";
            else desc += "Used to contain";
            desc += " a small piece of fairy." +
                "\nSeeking a place that would provide a near constant flow of the desperate and injured, Médecins Sans Diplôme recognized the Gungeon as the perfect place to found their practice.";
            return desc;
        }

        public static GameObject ItemBodyModelPrefab;

        public Medkit()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Medkit.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Medkit.png";
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
childName = "ThighR",
localPos = new Vector3(0.0183F, 0.0669F, 0.1308F),
localAngles = new Vector3(352.5534F, 0F, 275.1813F),
localScale = new Vector3(0.2222F, 0.2222F, 0.2222F)
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
                    childName = "GunBarrelBase",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
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

            var BarrierAmt = health.fullBarrier * Medkit_BarrierAmount;

            health.HealFraction(Medkit_HealAmount, default);
            health.AddBarrier(BarrierAmt);
            return true;
        }
    }
}
