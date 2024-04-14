using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Items;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class Spice2 : EquipmentBase<Spice2>
    {
        //public static float cfgCurseAmount;
        public static bool cfgSpiceReplacement = true;

        public static float cfgStatAccuracy = 0.2f;
        public static float cfgStatAccuracyStack = -0.1f;
        public static float cfgStatDamage = 0.2f;
        public static float cfgStatDamageStack = 0.1f;
        public static float cfgStatRORCurse = 0.1f;
        public static float cfgStatRORCurseStack = 0.05f;
        public override float Cooldown => 60;

        public override string EquipmentName => "Spice";

        public override string EquipmentLangTokenName => "SPICE";

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        public override bool IsLunar => true;
        public override bool CanBeRandomlyTriggered => false;
        public override string WikiLink => "https://thunderstore.io/package/DestroyedClone/RiskOfBulletstorm/wiki/212-equipment-spice/";

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateLang()
        {
            base.CreateLang();
            //LanguageOverrides.DeferLateUniqueTokens("RISKOFBULLETSTORM_EQUIPMENT_SPICE_PICKUP", "RISKOFBULLETSTORM_EQUIPMENT_SPICE_PICKUP_FORMAT", "RISKOFBULLETSTORM_EQUIPMENT_SPICE_PICKUP_USES_0");
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
                    childName = "Pelvis",
localPos = new Vector3(-0.10568F, -0.09541F, -0.10228F),
localAngles = new Vector3(0F, 180F, 180F),
localScale = new Vector3(0.01876F, 0.01876F, 0.01876F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.00001F, 0.29148F, 0.05432F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.04361F, 0.04361F, 0.04361F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.0642F, 1.80766F, -0.51001F),
localAngles = new Vector3(340.9353F, 354.7346F, 348.4178F),
localScale = new Vector3(0.3033F, 0.3033F, 0.3033F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "MuzzleLeft",
localPos = new Vector3(0F, 0.08624F, -0.242F),
localAngles = new Vector3(357.9388F, 180.0819F, 173.9026F),
localScale = new Vector3(0.02336F, 0.02336F, 0.02336F)
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.27961F, 0.17621F, -0.00666F),
localAngles = new Vector3(0F, 1F, 359.94F),
localScale = new Vector3(0.0721F, 0.0721F, 0.0721F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.09761F, 0.30536F, 0.00779F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01808F, 0.01808F, 0.01808F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.00012F, 0.07296F, 0.16069F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.00832F, 0.00832F, 0.00832F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HeadBase",
localPos = new Vector3(0F, -0.75128F, -0.39199F),
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
localPos = new Vector3(-0.11589F, 0.29434F, 0.07296F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.02911F, 0.02911F, 0.02911F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
            childName = "Head",
localPos = new Vector3(0F, 3.17947F, -0.55798F),
localAngles = new Vector3(90F, 0F, 0F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.18805F, 0.02174F, 0.12103F),
localAngles = new Vector3(49.44635F, 323.1183F, 289.0086F),
localScale = new Vector3(0.02352F, 0.02352F, 0.02352F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.01647F, -0.17858F, 0.14206F),
localAngles = new Vector3(23.57573F, 65.52239F, 79.68043F),
localScale = new Vector3(0.03218F, 0.03218F, 0.03218F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
              childName = "Chest",
localPos = new Vector3(-0.16944F, 0.31544F, 0.06118F),
localAngles = new Vector3(31.30424F, 10.58948F, 38.20667F),
localScale = new Vector3(0.03183F, 0.03183F, 0.03183F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
       childName = "Chest",
localPos = new Vector3(-0.16475F, 0.06523F, 0.04655F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01826F, 0.01826F, 0.01826F)
                }
            });
            rules.Add("mdlVoidSurvivor", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.05987F, 0.05987F, 0.05987F)
            });
            rules.Add("mdlRailGunner", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.03451F, -0.04685F, 0.01902F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.02564F, 0.02564F, 0.02564F)
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
            base.Hooks();
            On.RoR2.CharacterMasterNotificationQueue.PushNotification += CharacterMasterNotificationQueue_PushNotification;
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            Run.onRunStartGlobal += EnsureSpiceIsInAllDropLists;
        }

        private void EnsureSpiceIsInAllDropLists(Run run)
        {
            var pickupIndex = PickupCatalog.FindPickupIndex(EquipmentDef.equipmentIndex);
            foreach (var dropList in new List<List<PickupIndex>>
            {
                //run.availableBossDropList,
                run.availableEquipmentDropList,
                //run.availableLunarCombinedDropList,
                run.availableLunarEquipmentDropList,
                //run.availableLunarItemDropList,
                //run.availableTier1DropList,
                //run.availableTier2DropList,
                //run.availableTier3DropList,
                //run.availableVoidBossDropList,
                //run.availableVoidTier1DropList,
                //run.availableVoidTier2DropList,
                //run.availableVoidTier3DropList,
            })
            {
                if (dropList.Contains(pickupIndex)) continue;
                dropList.Add(pickupIndex);
            }
            BasicPickupDropTable.RegenerateAll(run);
            ExplicitPickupDropTable.RegenerateAll(run);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var itemCount = SpiceTally.instance.GetCount(sender);
            if (itemCount > 0)
            {
                args.damageMultAdd += SpiceTally.instance.GetStack(cfgStatDamage, cfgStatDamageStack, itemCount);
                args.baseCurseAdd += SpiceTally.instance.GetStack(cfgStatRORCurse, cfgStatRORCurseStack, itemCount);
            }
        }

        public void BloatDropListsWithSpice()
        {
            if (!Run.instance) return;
            var run = Run.instance;
            var pickupIndex = PickupCatalog.FindPickupIndex(EquipmentDef.equipmentIndex);
            run.availableBossDropList.Add(pickupIndex);
            run.availableEquipmentDropList.Add(pickupIndex);
            run.availableLunarCombinedDropList.Add(pickupIndex);
            run.availableLunarEquipmentDropList.Add(pickupIndex);
            run.availableLunarItemDropList.Add(pickupIndex);
            run.availableTier1DropList.Add(pickupIndex);
            run.availableTier2DropList.Add(pickupIndex);
            run.availableTier3DropList.Add(pickupIndex);
            run.availableVoidBossDropList.Add(pickupIndex);
            run.availableVoidTier1DropList.Add(pickupIndex);
            run.availableVoidTier2DropList.Add(pickupIndex);
            run.availableVoidTier3DropList.Add(pickupIndex);

            BasicPickupDropTable.RegenerateAll(run);
            ExplicitPickupDropTable.RegenerateAll(run);
        }

        private void CharacterMasterNotificationQueue_PushNotification(On.RoR2.CharacterMasterNotificationQueue.orig_PushNotification orig, CharacterMasterNotificationQueue self, CharacterMasterNotificationQueue.NotificationInfo info, float duration)
        {
            EquipmentDef equipmentInfo = info.data as EquipmentDef;
            if (!equipmentInfo || equipmentInfo != EquipmentDef) goto EarlyReturn;

            var master = self.master;
            if (!master || !master.inventory) goto EarlyReturn;

            var spiceConsumedCount = GetSpiceConsumedCount(master);
            EquipmentIndex replacementEquipmentIndex = EquipmentDef.equipmentIndex;
            switch (spiceConsumedCount)
            {
                case 0:
                    break;

                case 1:
                    replacementEquipmentIndex = SpicePickupEquipmentA.Instance.EquipmentDef.equipmentIndex;
                    break;

                case 2:
                    replacementEquipmentIndex = SpicePickupEquipmentB.Instance.EquipmentDef.equipmentIndex;
                    break;

                default:
                    replacementEquipmentIndex = SpicePickupEquipmentC.Instance.EquipmentDef.equipmentIndex;
                    break;
            }
            info.data = EquipmentCatalog.GetEquipmentDef(replacementEquipmentIndex);
        EarlyReturn:
            orig(self, info, duration);
        }

        private static int GetSpiceConsumedCount(CharacterMaster master)
        {
            return master.inventory.GetItemCount(Items.SpiceTally.instance.ItemDef);
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            CharacterMasterNotificationQueue.PushEquipmentTransformNotification(slot.characterBody.master, slot.characterBody.inventory.currentEquipmentIndex, SpiceConsumed.Instance.EquipmentDef.equipmentIndex, CharacterMasterNotificationQueue.TransformationType.Default);
            slot.inventory.SetEquipmentIndex(SpiceConsumed.Instance.EquipmentDef.equipmentIndex);
            slot.inventory.GiveItem(SpiceTally.instance.ItemDef);
            slot.inventory.GiveItem(CurseTally.instance.ItemDef);

            if (!cfgSpiceReplacement)
                return true;
            if (slot.characterBody
                && slot.characterBody.teamComponent
                && slot.characterBody.isPlayerControlled
                && slot.characterBody.teamComponent.teamIndex == TeamIndex.Player
                )
            {
                BloatDropListsWithSpice();
            }
            return true;
        }
    }
}