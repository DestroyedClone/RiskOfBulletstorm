using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Controllers.SharedComponents;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class Drill : EquipmentBase<Drill>
    {
        public static float ChestCostCommonDirectorMultiplier = 2;
        public static float ChestCostUncommonDirectorMultiplier = 3;
        public static float ChestCostLegendaryDirectorMultiplier = 4;
        public override float Cooldown => 60;

        public override string EquipmentName => "Drill";

        public override string EquipmentLangTokenName => "DRILL";

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        public override bool IsLunar => true;

        private readonly string UnlockSound = EntityStates.Engi.EngiWeapon.FireMines.throwMineSoundString;
        public override bool CanBeRandomlyTriggered => false;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules() => EquipmentDisplays.Drill(ref ItemBodyModelPrefab, EquipmentModel);

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
                                if (ActivateDrillEffectOnChest(bestInteractableObject, interactionDriver))
                                {
                                    return true;
                                }
                            }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Activate's the <b>Drill</b>'s effects on Chests.
        /// </summary>
        /// <param name="chestObject">The GameObject of the chest</param>
        /// <param name="interactionDriver">The InteractionDriver of the user</param>
        /// <returns></returns>
        private bool ActivateDrillEffectOnChest(GameObject chestObject, InteractionDriver interactionDriver)
        {
            if (!interactionDriver) return false;
            Highlight highlight = chestObject.GetComponent<Highlight>();
            PurchaseInteraction purchaseInteraction = chestObject.GetComponent<PurchaseInteraction>();
            if (!highlight || !purchaseInteraction) return false;
            RBSChestInteractorComponent chestComponent = chestObject.GetComponent<RBSChestInteractorComponent>();
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
                        creditMultiplier = ChestCostCommonDirectorMultiplier;
                        break;

                    case "CHEST2_NAME":
                    case "CATEGORYCHEST2_HEALING_NAME":
                    case "CATEGORYCHEST2_DAMAGE_NAME":
                    case "CATEGORYCHEST2_UTILITY_NAME":
                        creditMultiplier = ChestCostUncommonDirectorMultiplier;
                        break;

                    case "GOLDCHEST_NAME":
                        creditMultiplier = ChestCostLegendaryDirectorMultiplier;
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

        /// <summary>
        /// Helper method to emulate the spawn of a CombatShrine
        /// </summary>
        /// <param name="interactor">The Interactor of the interactor</param>
        /// <param name="effectTransform">The transform where the effect spawns</param>
        /// <param name="creditMultiplier">The Credit multiplier for the director of the Combat Shrine.</param>
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