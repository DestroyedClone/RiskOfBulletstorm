using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;


namespace RiskOfBulletstorm.Items
{
    public class RingMiserlyProtection : Item_V2<RingMiserlyProtection> //switch to a buff system due to broken
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much maximum health is multiplied by per Ring of Miserly Protection? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float RingMiserlyProtection_HealthBonus { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional maximum health is multiplied by per subsequent stacks of Ring of Miserly Protection? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float RingMiserlyProtection_HealthBonusStack { get; private set; } = 0.5f;
        public override string displayName => "Ring of Miserly Protection";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.Cleansable });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>Aids The Fiscally Responsible</b>\n<style=cHealth>Increases health substantially.</style> <style=cDeath>Any shrine purchases will shatter a ring.</style>";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"";
            var incHealth = RingMiserlyProtection_HealthBonus > 0;
            var incStack = RingMiserlyProtection_HealthBonusStack > 0;
            if (!incHealth && !incStack)
                desc += $"It does nothing.";
            if (incHealth)
                desc += $"Grants <style=cIsHealth>+{Pct(RingMiserlyProtection_HealthBonus)}</style> health.";
            if (incStack)
                desc += $"<style=cStack>(+{Pct(RingMiserlyProtection_HealthBonusStack)} per stack)</style>";
            desc += $"\n <style=cDeath>...but breaks a stack completely upon using a shrine.</style> ";

            return desc;
        }

        protected override string GetLoreString(string langID = null) => "Before the Shopkeep opened his shop, he was an avaricious and miserly man. He remains careful about any expenditures, but through capitalism he has purged himself of negative emotion.";

        private readonly GameObject ShatterEffect = Resources.Load<GameObject>("prefabs/effects/ShieldBreakEffect");

        public static GameObject ItemBodyModelPrefab;

        public RingMiserlyProtection()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/RingMiserlyProtection.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/RingMiserlyProtection.png";
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>("@RiskOfBulletstorm:Assets/Models/Prefabs/RingMiserlyProtectionWorn.prefab");
                displayRules = GenerateItemDisplayRules();
            }

            base.SetupAttributes();
        }
        public static ItemDisplayRuleDict GenerateItemDisplayRules() //THIS SUCKS
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.01f, 0.01f, 0.01f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Finger22R",
localPos = new Vector3(0.0259F, -0.0247F, -0.0562F),
localAngles = new Vector3(30F, 5F, 180F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Finger22R",
localPos = new Vector3(-0.0254F, -0.0258F, 0.0208F),
localAngles = new Vector3(60F, 20F, 180F),
localScale = new Vector3(0.0162F, 0.0167F, 0.0162F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[] // TODO
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleNailgun",
                    localPos = new Vector3(0f, 3.4f, -1.3f),
                    localAngles = new Vector3(60f, 0f, 0f),
                    localScale = generalScale * 16f
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleSpear",
                    localPos = new Vector3(0f, 0.5f, 0.22f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleBuzzsaw",
                    localPos = new Vector3(0f, 0.5f, 0.22f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger22R",
                    localPos = new Vector3(0.03f, -0.02f, -0.01f),
                    localAngles = new Vector3(-60f, 1f, 180f),
                    localScale = generalScale
                },
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger22R",
                    localPos = new Vector3(0.02f, -0.055f, -0.01f),
                    localAngles = new Vector3(110f, 185f, 180f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger22R",
                    localPos = new Vector3(-0.035f, 0.03f, -0.035f),
                    localAngles = new Vector3(270f, 0f, 0f),
                    localScale = new Vector3(0.013f, 0.013f, 0.013f)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[] //todo
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "WeaponPlatform",
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
                    childName = "MechFinger23R",
                    localPos = new Vector3(0.075f, -0.01f, 0.05f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = generalScale * 3
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger11L",
                    localPos = new Vector3(-0.3f, 0.4f, 0f),
                    localAngles = new Vector3(270f, 90f, 0f),
                    localScale = generalScale * 25
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger22R",
                    localPos = new Vector3(-0.035f, 0.01f, 0.012f),
                    localAngles = new Vector3(270f, 20f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[] //todo
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger22R",
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
                    childName = "HandL",
                    localPos = new Vector3(0.015f, 0.135f, 0.022f),
                    localAngles = new Vector3(40f, 26f, 6.6411f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[] //todo new child?
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Muzzle",
                    localPos = new Vector3(0.1f, 0.4f, -2.8f),
                    localAngles = new Vector3(35f, -1.042f, -135f),
                    localScale = generalScale * 3f
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(0.6f, 2.3f, -0.3f),
                    localAngles = new Vector3(90f, 90f, 0f),
                    localScale = generalScale * 20f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[] //todo child
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Finger22R",
                    localPos = new Vector3(0f, 5f, 0f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = generalScale * 20f
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
            ShrineChanceBehavior.onShrineChancePurchaseGlobal += ShrineChanceBehavior_onShrineChancePurchaseGlobal;
            GetStatCoefficients += BoostHealth;
        }

        private void ShrineChanceBehavior_onShrineChancePurchaseGlobal(bool gaveItem, Interactor interactor)
        {
            var body = interactor.gameObject.GetComponent<CharacterBody>();
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    var InventoryCount = body.inventory.GetItemCount(catalogIndex);
                    if (InventoryCount > 0)
                    {
                        body.inventory.RemoveItem(catalogIndex);
                        Util.PlaySound("Play_char_glass_death", body.gameObject);
                        EffectManager.SimpleEffect(ShatterEffect, body.gameObject.transform.position, Quaternion.identity, true);
                        body.RecalculateStats();
                    }
                }
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            ShrineChanceBehavior.onShrineChancePurchaseGlobal -= ShrineChanceBehavior_onShrineChancePurchaseGlobal;
            GetStatCoefficients -= BoostHealth;
        }

        private void BoostHealth(CharacterBody sender, StatHookEventArgs args)
        {
            var invCount = GetCount(sender);
            if (invCount > 0) args.healthMultAdd += RingMiserlyProtection_HealthBonus + RingMiserlyProtection_HealthBonusStack * (invCount - 1);
        }
    }
}
