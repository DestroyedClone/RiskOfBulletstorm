using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Controllers.SharedComponents;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class Drill : EquipmentBase<Drill>
    {
        public static ConfigEntry<float> cfgChestCostCommon;
        public static ConfigEntry<float> cfgChestCostUncommon;
        public static ConfigEntry<float> cfgChestCostLegendary;

        public override string EquipmentName => "Drill";

        public override string EquipmentLangTokenName => "DRILL";

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        public override bool IsLunar => true;

        private readonly string UnlockSound = EntityStates.Engi.EngiWeapon.FireMines.throwMineSoundString;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateConfig(ConfigFile config)
        {
            string text = "The multiplier for the amount of credits given to the Combat Director for spawning the equipment's enemies.\nApplies to ";

            cfgChestCostCommon = config.Bind(ConfigCategory, "Common Chest Credit Multiplier", 2f, text + "Chests, Utility Chests, Damage Chests, Healing Chests, and Equipment Barrels.");
            cfgChestCostUncommon = config.Bind(ConfigCategory, "Large Chest Credit Multiplier", 3f, text + "Large Chests");
            cfgChestCostLegendary = config.Bind(ConfigCategory, "Legendary Credit Multiplier", 4f, text + "Legendary Chests");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
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
                        if (!purchaseInteraction.GetComponent<ShopTerminalBehavior>())
                            if (purchaseInteraction)
                            {
                                if (UnlockChest(bestInteractableObject, interactionDriver))
                                {
                                    return true;
                                }
                            }
                    }
                }
            }
            return false;
        }

        private bool UnlockChest(GameObject chestObject, InteractionDriver interactionDriver)
        {
            if (!interactionDriver) return false;
            Highlight highlight = chestObject.GetComponent<Highlight>();
            PurchaseInteraction purchaseInteraction = chestObject.GetComponent<PurchaseInteraction>();
            if (!highlight || !purchaseInteraction) return false;
            BulletstormChestInteractorComponent chestComponent = chestObject.GetComponent<BulletstormChestInteractorComponent>();
            if (chestComponent && chestComponent.hasUsedLockpicks) return false;

            if (!purchaseInteraction.isShrine && purchaseInteraction.available && purchaseInteraction.costType == CostTypeIndex.Money)
            {
                Interactor interactor = interactionDriver.interactor;
                //interactionDriver.interactor.AttemptInteraction(chestObject);
                purchaseInteraction.SetAvailable(false);
                //purchaseInteraction.Networkavailable = false;

                purchaseInteraction.gameObject.GetComponent<ChestBehavior>().Open();

                //purchaseInteraction.cost = 0;
                //purchaseInteraction.Networkcost = 0;

                float creditMultiplier = 1f;

                switch (purchaseInteraction.displayNameToken)
                {
                    case "CHEST1_STEALTHED_NAME":
                    case "CHEST1_NAME":
                    case "CATEGORYCHEST_HEALING_NAME":
                    case "CATEGORYCHEST_DAMAGE_NAME":
                    case "CATEGORYCHEST_UTILITY_NAME":
                    case "EQUIPMENTBARREL_NAME":
                        creditMultiplier = cfgChestCostCommon.Value;
                        break;

                    case "CHEST2_NAME":
                    case "CATEGORYCHEST2_HEALING_NAME":
                    case "CATEGORYCHEST2_DAMAGE_NAME":
                    case "CreateSpawner_NAME":
                        creditMultiplier = cfgChestCostUncommon.Value;
                        break;

                    case "GOLDCHEST_NAME":
                        creditMultiplier = cfgChestCostLegendary.Value;
                        break;
                }

                purchaseInteraction.onPurchase.Invoke(interactor);
                purchaseInteraction.lastActivator = interactor;
                Util.PlaySound(UnlockSound, interactor.gameObject);
                SpawnCombat(interactor, purchaseInteraction.transform, creditMultiplier);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SpawnCombat(Interactor interactor, Transform effectTransform, float creditMultiplier = 1f)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/Encounters/MonstersOnShrineUseEncounter"), effectTransform.position, Quaternion.identity);
            NetworkServer.Spawn(gameObject);
            CombatDirector component5 = gameObject.GetComponent<CombatDirector>();
            float monsterCredit = 40f * Stage.instance.entryDifficultyCoefficient * creditMultiplier;
            DirectorCard directorCard = component5.SelectMonsterCardForCombatShrine(monsterCredit);
            if (directorCard != null)
            {
                component5.CombatShrineActivation(interactor, monsterCredit, directorCard);
                EffectData effectData = new EffectData
                {
                    origin = effectTransform.position,
                    rotation = effectTransform.rotation
                };
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MonstersOnShrineUse"), effectData, true);
                return;
            }
            NetworkServer.Destroy(gameObject);
        }
    }
}