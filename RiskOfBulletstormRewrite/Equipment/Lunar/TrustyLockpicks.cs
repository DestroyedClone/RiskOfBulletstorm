using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Controllers.SharedComponents;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class TrustyLockpicks : EquipmentBase<TrustyLockpicks>
    {
        // TODO: Context string edits
        public static ConfigEntry<float> cfgUnlockChance;

        public static ConfigEntry<float> cfgPriceMultiplier;

        public override string EquipmentName => "Trusty Lockpicks";

        public override string EquipmentLangTokenName => "TRUSTYLOCKPICKS";

        public override string[] EquipmentFullDescriptionParams => new string[]
        {
            cfgUnlockChance.Value.ToString(), //direct percentage so...
            GetChance(cfgPriceMultiplier)
        };

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        private readonly string UnlockSound = EntityStates.Engi.EngiWeapon.FireMines.throwMineSoundString;
        private readonly GameObject UnlockEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/LevelUpEffect");

        //private readonly GameObject Fail_DestroyEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/ShieldBreakEffect");
        private readonly GameObject Fail_LockEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/LevelUpEffectEnemy");

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateConfig(ConfigFile config)
        {
            cfgUnlockChance = config.Bind(ConfigCategory, "Chest Unlock Chance", 50f, "What is the chance to unlock the chest?" +
                "\n50 = 50%, 0.5 = 0.5%");
            cfgPriceMultiplier = config.Bind(ConfigCategory, "Fail Cost Multiplier", 2f, "If you fail to unlock the chest, what will the price be multiplied by?");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = EquipmentModel;
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
              childName = "Chest",
localPos = new Vector3(-0.16424F, -0.11802F, 0.06013F),
localAngles = new Vector3(10.66484F, 130.7512F, 192.8399F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0F, -0.03013F, -0.13374F),
localAngles = new Vector3(347.7289F, 298.6138F, 3.84973F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Hip",
localPos = new Vector3(1.59179F, 1.0558F, 1.04578F),
localAngles = new Vector3(22.55995F, 169.5176F, 30.46456F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
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
        childName = "Pelvis",
localPos = new Vector3(0F, 0.01179F, -0.16423F),
localAngles = new Vector3(4.39836F, 315.4362F, 352.628F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0F, 0.07386F, -0.19466F),
localAngles = new Vector3(350.5761F, 322.086F, 23.27923F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HeadBase",
localPos = new Vector3(0F, 0.628F, -0.392F),
localAngles = new Vector3(344.4032F, 180F, 180F),
localScale = new Vector3(0.1341F, 0.1341F, 0.1341F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.00153F, 0.00127F, 0.23284F),
localAngles = new Vector3(352.9141F, 240.0227F, 184.4485F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
           childName = "HandL",
localPos = new Vector3(4.53146F, 0.43625F, 0.84055F),
localAngles = new Vector3(4.67429F, 345.5549F, 296.7546F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.16754F, -0.09953F, -0.12617F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0F, -0.0616F, 0.18F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
           childName = "Pelvis",
localPos = new Vector3(-0.07889F, 0.00295F, -0.20718F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
            childName = "Pelvis",
localPos = new Vector3(-0.10437F, 0.01401F, -0.16134F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
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
localPos = new Vector3(0F, 4.8685F, 0.0438F),
localAngles = new Vector3(288.4044F, 180F, 180F),
localScale = new Vector3(1F, 1F, 1F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.0013F, 0.1559F, -0.2403F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.1594F, 3.6456F, 0.0645F),
                localAngles = new Vector3(279.4401F, 195.4454F, 161.8801F),
                localScale = new Vector3(0.4099F, 0.4099F, 0.4099F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLB",
                localPos = new Vector3(-1.6752F, -0.2F, -0.468F),
                localAngles = new Vector3(2.6768F, 179.4175F, 179.4478F),
                localScale = new Vector3(0.1793F, 0.1793F, 0.1793F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(0.0002F, -0.189F, 1.9457F),
                localAngles = new Vector3(24.2706F, 0.0024F, 0.024F),
                localScale = new Vector3(0.2908F, 0.2908F, 0.2908F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Mask",
                localPos = new Vector3(0F, 0.0344F, -1.6055F),
                localAngles = new Vector3(88.6293F, 0F, 0F),
                localScale = new Vector3(0.425F, 0.425F, 0.425F)
            });
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
            On.RoR2.PurchaseInteraction.GetContextString += PurchaseInteraction_GetContextString;
        }

        private string PurchaseInteraction_GetContextString(On.RoR2.PurchaseInteraction.orig_GetContextString orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);

            BulletstormChestInteractorComponent bulletstormChestInteractor = self.GetComponent<BulletstormChestInteractorComponent>();
            if (bulletstormChestInteractor)
            {
                if (bulletstormChestInteractor.InteractorHasValidEquipment(activator))
                {
                    return bulletstormChestInteractor.GetContextualString(original);
                }
            }

            return original;
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            var gameObject = self.gameObject;
            Highlight highlight = gameObject.GetComponent<Highlight>();
            BulletstormChestInteractorComponent bulletstormChestInteractor = gameObject.GetComponent<BulletstormChestInteractorComponent>();

            if (bulletstormChestInteractor
                && bulletstormChestInteractor.hasUsedLockpicks
                && activator.GetComponent<EquipmentSlot>()?.equipmentIndex == EquipmentDef.equipmentIndex
                && activator.GetComponent<CharacterBody>()?.inputBank?.activateEquipment.justPressed == true)
            {
                if (highlight)
                    highlight.highlightColor = Highlight.HighlightColor.teleporter;
                return orig(self, activator);
            }

            return orig(self, activator);
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot.characterBody)
            {
                var interactionDriver = slot.characterBody.GetComponent<InteractionDriver>();
                if (interactionDriver)
                {
                    var bestInteractableObject = slot.characterBody.GetComponent<InteractionDriver>().FindBestInteractableObject();
                    if (bestInteractableObject)
                    {
                        var purchaseInteraction = bestInteractableObject.GetComponent<PurchaseInteraction>();
                        if (purchaseInteraction)
                        {
                            if (!purchaseInteraction.GetComponent<ShopTerminalBehavior>())
                                if (AttemptUnlock(bestInteractableObject, interactionDriver, cfgUnlockChance.Value))
                                {
                                    return true;
                                }
                        }
                    }
                }
            }
            return false;
        }

        private bool AttemptUnlock(GameObject chestObject, InteractionDriver interactionDriver, float UnlockChance)
        {
            if (!interactionDriver) return false;
            var chestBehavior = chestObject.GetComponent<ChestBehavior>();
            if (!chestBehavior) return false;
            Highlight highlight = chestObject.GetComponent<Highlight>();
            PurchaseInteraction purchaseInteraction = chestObject.GetComponent<PurchaseInteraction>();
            if (!highlight || !purchaseInteraction) return false;
            BulletstormChestInteractorComponent chestComponent = chestObject.GetComponent<BulletstormChestInteractorComponent>();
            if (chestComponent && chestComponent.hasUsedLockpicks) return false;
            Vector3 offset = Vector3.up * 1f;

            if (!purchaseInteraction.isShrine && purchaseInteraction.available && purchaseInteraction.costType == CostTypeIndex.Money)
            {
                Interactor interactor = interactionDriver.interactor;
                //interactionDriver.interactor.AttemptInteraction(chestObject);
                if (Util.CheckRoll(UnlockChance))
                {
                    purchaseInteraction.SetAvailable(false);
                    //purchaseInteraction.Networkavailable = false;

                    chestBehavior.Open();

                    //purchaseInteraction.cost = 0;
                    //purchaseInteraction.Networkcost = 0;

                    purchaseInteraction.onPurchase.Invoke(interactor);
                    purchaseInteraction.lastActivator = interactor;
                    Util.PlaySound(UnlockSound, interactor.gameObject);
                    EffectManager.SimpleEffect(UnlockEffect, chestObject.transform.position + offset, Quaternion.identity, true);
                }
                else
                {
                    var newCost = Mathf.CeilToInt(purchaseInteraction.cost * cfgPriceMultiplier.Value);
                    //purchaseInteraction.cost = newCost;
                    purchaseInteraction.Networkcost = newCost;
                    GameObject selectedEffect = Fail_LockEffect;

                    //purchaseInteraction.displayNameToken = (prefix + purchaseInteraction.displayNameToken);
                    chestObject.AddComponent<BulletstormChestInteractorComponent>().hasUsedLockpicks = true; //does this even work? lol
                    EffectManager.SimpleEffect(selectedEffect, chestObject.transform.position + offset, Quaternion.identity, true);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}