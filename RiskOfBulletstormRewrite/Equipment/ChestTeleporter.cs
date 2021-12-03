using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using System;
using System.Collections.Generic;
using HG;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class ChestTeleporter : EquipmentBase<ChestTeleporter>
    {
        public static ConfigEntry<int> cfgNormalKeys;
        public static ConfigEntry<int> cfgMediumKeys;
        public static ConfigEntry<int> cfgLegendaryKeys;


        public override string EquipmentName => "Chest Teleporter";

        public override string EquipmentLangTokenName => "CHESTTELEPORTER";

        public override string[] EquipmentFullDescriptionParams => new string[]
        {
            cfgNormalKeys.Value.ToString(),
            cfgMediumKeys.Value.ToString(),
            cfgLegendaryKeys.Value.ToString()
        };

        public override GameObject EquipmentModel => MainAssets.LoadAsset<GameObject>("ExampleEquipmentPrefab.prefab");

        public override Sprite EquipmentIcon => MainAssets.LoadAsset<Sprite>("ExampleEquipmentIcon.png");

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateConfig(ConfigFile config)
        {
            cfgNormalKeys = config.Bind(ConfigCategory, "Normal Chest Key Amount", 1, "The amount of Rusty Keys to give upon use.");
            cfgMediumKeys = config.Bind(ConfigCategory, "Medium Chest Key Amount", 3, "The amount of Rusty Keys to give upon use.");
            cfgLegendaryKeys = config.Bind(ConfigCategory, "Legendary Chest Key Amount", 6, "The amount of Rusty Keys to give upon use.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }
        public static bool PurchaseInteractionIsValidTarget(PurchaseInteraction purchaseInteraction)
        {
            return purchaseInteraction && (purchaseInteraction.costType == CostTypeIndex.Money && purchaseInteraction.cost > 0) && purchaseInteraction.available;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            //Hacking Main State
            SphereSearch sphereSearch = new SphereSearch
            {
                origin = slot.characterBody.footPosition,
                mask = LayerIndex.CommonMasks.interactable,
                queryTriggerInteraction = QueryTriggerInteraction.Collide,
                radius = 3f
            };

            List<Collider> list = CollectionPool<Collider, List<Collider>>.RentCollection();
            sphereSearch.ClearCandidates();
            sphereSearch.RefreshCandidates();
            sphereSearch.FilterCandidatesByColliderEntities();
            sphereSearch.OrderCandidatesByDistance();
            sphereSearch.FilterCandidatesByDistinctColliderEntities();
            sphereSearch.GetColliders(list);
            PurchaseInteraction result = null;
            int i = 0;
            int count = list.Count;
            int keyAmount = 0;
            while (i < count)
            {
                PurchaseInteraction component = list[i].GetComponent<EntityLocator>().entity.GetComponent<PurchaseInteraction>();
                if (EntityStates.CaptainSupplyDrop.HackingMainState.PurchaseInteractionIsValidTarget(component))
                {
                    switch (component.displayNameToken)
                    {
                        case "CHEST1_NAME":
                        case "CHEST1_STEALTHED_NAME":
                            keyAmount = cfgNormalKeys.Value;
                            break;
                        case "CHEST2_NAME":
                            keyAmount = cfgMediumKeys.Value;
                            break;
                        case "GOLDCHEST_NAME":
                            keyAmount = cfgLegendaryKeys.Value;
                            break;
                        default:
                            continue;
                    };
                    result = component;
                    break;
                }
                i++;
            }
            CollectionPool<Collider, List<Collider>>.ReturnCollection(list);

            ScrapChest(result, keyAmount);


            return false;
        }

        private void ScrapChest(PurchaseInteraction purchaseInteraction, int keyAmount)
        {
            purchaseInteraction.Networkavailable = false;

            PickupIndex pickupIndex2 = PickupCatalog.FindPickupIndex(RoR2Content.Items.TreasureCache.itemIndex);

            for (int i = 0; i < keyAmount; i++)
            {
                PickupDropletController.CreatePickupDroplet(pickupIndex2, purchaseInteraction.transform.position, Vector3.up * i);
            }
        }

    }
}
