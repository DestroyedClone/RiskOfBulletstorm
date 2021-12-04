﻿using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
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
            GetChance(cfgUnlockChance),
            GetChance(cfgPriceMultiplier)
        };

        public override GameObject EquipmentModel => RoR2Content.Items.TreasureCache.pickupModelPrefab;

        public override Sprite EquipmentIcon => RoR2Content.Items.TreasureCache.pickupIconSprite;

        private readonly string UnlockSound = EntityStates.Engi.EngiWeapon.FireMines.throwMineSoundString;
        private readonly GameObject UnlockEffect = Resources.Load<GameObject>("prefabs/effects/LevelUpEffect");
        //private readonly GameObject Fail_DestroyEffect = Resources.Load<GameObject>("prefabs/effects/ShieldBreakEffect");
        private readonly GameObject Fail_LockEffect = Resources.Load<GameObject>("prefabs/effects/LevelUpEffectEnemy");

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateConfig(ConfigFile config)
        {
            cfgUnlockChance = config.Bind(ConfigCategory, "Chest Unlock Chance", 50f, "What is the chance to unlock the chest? (Exact Percentage)");
            cfgPriceMultiplier = config.Bind(ConfigCategory, "Fail Cost Multiplier", 2f, "If you fail to unlock the chest, what will the price be multiplied by?");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            //On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            var gameObject = self.gameObject;
            Highlight highlight = gameObject.GetComponent<Highlight>();
            BulletstormChestInteractorComponent bulletstormChestInteractor = gameObject.GetComponent<BulletstormChestInteractorComponent>();

            if (bulletstormChestInteractor && bulletstormChestInteractor.hasUsedLockpicks)
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
            Highlight highlight = chestObject.GetComponent<Highlight>();
            PurchaseInteraction purchaseInteraction = chestObject.GetComponent<PurchaseInteraction>();
            if (!highlight || !purchaseInteraction) return false;
            BulletstormChestInteractorComponent chestComponent = chestObject.GetComponent<BulletstormChestInteractorComponent>();
            if (chestComponent && chestComponent.hasUsedLockpicks) return false;

            GameObject selectedEffect = UnlockEffect;
            Vector3 offset = Vector3.up * 1f;


            if (!purchaseInteraction.isShrine && purchaseInteraction.available && purchaseInteraction.costType == CostTypeIndex.Money)
            {
                Interactor interactor = interactionDriver.interactor;
                //interactionDriver.interactor.AttemptInteraction(chestObject);
                if (Util.CheckRoll(UnlockChance))
                {
                    purchaseInteraction.SetAvailable(false);
                    //purchaseInteraction.Networkavailable = false;

                    purchaseInteraction.gameObject.GetComponent<ChestBehavior>().Open();

                    //purchaseInteraction.cost = 0;
                    //purchaseInteraction.Networkcost = 0;

                    purchaseInteraction.onPurchase.Invoke(interactor);
                    purchaseInteraction.lastActivator = interactor;
                    Util.PlaySound(UnlockSound, interactor.gameObject);
                    EffectManager.SimpleEffect(UnlockEffect, chestObject.transform.position + offset, Quaternion.identity, true);
                }
                else
                {
                    purchaseInteraction.cost = Mathf.CeilToInt(purchaseInteraction.cost * cfgPriceMultiplier.Value);
                    //https://discord.com/channels/562704639141740588/562704639569428506/916819003064864788
                    purchaseInteraction.Networkcost = purchaseInteraction.cost; //weaver generated shit ^
                    selectedEffect = Fail_LockEffect;

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