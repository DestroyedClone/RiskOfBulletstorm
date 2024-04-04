using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Controllers;
using RiskOfBulletstormRewrite.Modules;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Controllers.SharedComponents;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class TrustyLockpicks : EquipmentBase<TrustyLockpicks>
    {
        public static float cfgUnlockChance = 50;

        public static float cfgPriceMultiplier = 2;
        public override float Cooldown => 60;
        public override bool CanBeRandomlyTriggered => false;

        public override string EquipmentName => "Trusty Lockpicks";

        public override string EquipmentLangTokenName => "TRUSTYLOCKPICKS";

        public override string[] EquipmentFullDescriptionParams => new string[]
        {
            cfgUnlockChance.ToString(), //direct percentage so...
            ToPct(cfgPriceMultiplier)
        };

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        private readonly string UnlockSound = EntityStates.Engi.EngiWeapon.FireMines.throwMineSoundString;
        private readonly GameObject UnlockEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/LevelUpEffect");
        private readonly GameObject Fail_LockEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/LevelUpEffectEnemy");

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
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
localPos = new Vector3(-0.15448F, -0.0799F, 0.06537F),
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
localPos = new Vector3(0F, -0.07072F, -0.13187F),
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
localPos = new Vector3(1.59179F, 0.54197F, 1.10515F),
localAngles = new Vector3(356.9828F, 170.3142F, 35.03957F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.14249F, 0.03412F, -0.14402F),
localAngles = new Vector3(344.6386F, 329.0468F, 9.83051F),
localScale = new Vector3(0.01592F, 0.01592F, 0.01592F)
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
        childName = "Head",
localPos = new Vector3(-0.11723F, 0.19499F, -0.03279F),
localAngles = new Vector3(319.9121F, 89.4791F, 270.2824F),
localScale = new Vector3(0.0743F, 0.0743F, 0.0743F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
   childName = "Pelvis",
localPos = new Vector3(0F, -0.02465F, -0.16027F),
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
localPos = new Vector3(0F, 0.03478F, -0.19774F),
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
localPos = new Vector3(0.21917F, -0.85993F, -0.39199F),
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
localPos = new Vector3(-0.00157F, 0.05473F, 0.23858F),
localAngles = new Vector3(314.9529F, 179.3808F, 277.7054F),
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
localPos = new Vector3(1.4486F, 1.97312F, -2.41149F),
localAngles = new Vector3(352.6598F, 9.29752F, 296.1912F),
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
localPos = new Vector3(-0.16762F, -0.10058F, -0.11993F),
localAngles = new Vector3(22.35304F, 359.8799F, 0.23974F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmL",
localPos = new Vector3(-0.00517F, 0.20177F, 0.1264F),
localAngles = new Vector3(2.82866F, 148.0161F, 0F),
localScale = new Vector3(0.01566F, 0.01566F, 0.01566F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
       childName = "Pelvis",
localPos = new Vector3(-0.07889F, -0.03313F, -0.20718F),
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
localPos = new Vector3(-0.09179F, 0.01644F, -0.17294F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlVoidSurvivor", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "ShoulderR",
                localPos = new Vector3(0.20174F, 0.30555F, 0.00774F),
                localAngles = new Vector3(0F, 0F, 177.4769F),
                localScale = new Vector3(0.04171F, 0.03477F, 0.02275F)
            });
            rules.Add("mdlRailGunner", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.08927F, 0.06509F, 0.04795F),
                localAngles = new Vector3(344.6819F, 65.45361F, 330.4876F),
                localScale = new Vector3(0.02202F, 0.02202F, 0.02202F)
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

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (!slot.characterBody) return false;

            var interactionDriver = slot.characterBody.GetComponent<InteractionDriver>();
            if (!interactionDriver) return false;

            var bestInteractableObject = slot.characterBody.GetComponent<InteractionDriver>().FindBestInteractableObject();
            if (!bestInteractableObject) return false;

            if (StoneGateModification.srv_isGoolake)
            {
                if (!bestInteractableObject.TryGetComponent(out StoneGateModification.RBSStoneGateLockInteraction gateLock))
                    goto NotStoneGate;
                if (gateLock.isLockBroken)
                    return false;
                AttemptUnlockStoneGate(gateLock);
                return true;
            }
        NotStoneGate:

            var purchaseInteraction = bestInteractableObject.GetComponent<PurchaseInteraction>();
            if (!purchaseInteraction) return false;

            if (purchaseInteraction.GetComponent<ShopTerminalBehavior>()) return false;

            return AttemptUnlockChest(bestInteractableObject, interactionDriver, purchaseInteraction, cfgUnlockChance);
        }

        private void AttemptUnlockStoneGate(StoneGateModification.RBSStoneGateLockInteraction gateLock)
        {
            if (Util.CheckRoll(cfgUnlockChance))
            {
                gateLock.OpenStoneGate();
                Util.PlaySound(UnlockSound, gateLock.gameObject);
                EffectManager.SimpleEffect(UnlockEffect, gateLock.transform.position, Quaternion.identity, true);
            }
            else
            {
                gateLock.isLockBroken = true;
                EffectManager.SimpleEffect(Fail_LockEffect, gateLock.transform.position, Quaternion.identity, true);
            }
        }

        private bool AttemptUnlockChest(GameObject chestObject, InteractionDriver interactionDriver, PurchaseInteraction purchaseInteraction, float UnlockChance)
        {
            if (!chestObject.TryGetComponent(out ChestBehavior chestBehavior)) return false;

            RBSChestLockInteraction chestComponent = chestObject.GetComponent<RBSChestLockInteraction>();
            if (chestComponent && chestComponent.isLockBroken) return false;

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
                    var newCost = Mathf.CeilToInt(purchaseInteraction.cost * cfgPriceMultiplier);
                    //purchaseInteraction.cost = newCost;
                    purchaseInteraction.Networkcost = newCost;
                    GameObject selectedEffect = Fail_LockEffect;

                    //purchaseInteraction.displayNameToken = (prefix + purchaseInteraction.displayNameToken);
                    chestObject.AddComponent<RBSChestLockInteraction>().isLockBroken = true; //does this even work? lol
                    EffectManager.SimpleEffect(selectedEffect, chestObject.transform.position + offset, Quaternion.identity, true);
                }
                chestComponent.isLockBroken = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}