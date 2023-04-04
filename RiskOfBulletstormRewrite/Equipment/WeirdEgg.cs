using BepInEx;
using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using System.Text;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class WeirdEgg : EquipmentBase<WeirdEgg>
    {
        public static ConfigEntry<float> cfgCooldown;
        public override float Cooldown => cfgCooldown.Value;

        public override string EquipmentName => "Weird Egg";

        public override string EquipmentLangTokenName => "WEIRDEGG";

        public override string[] EquipmentFullDescriptionParams => new string[]
        {
        };

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        

        /*  Needs to be able to be dropped, so install YEET? or implement our own?
         *      //do our own is fine.
        *   1. When Shot: Drop item based on carry time vs chamber time?
        *   2. When dropped into fire source: fire source vs fire weapon?
            3. Needs to be fed -> transform into snake or use modelswap ES?
            4. pick up as item, disable item drop for weird egg afterwards.

            Requirement: CharacterBody (pot?) w/ custom entitystates
        */

        public override void Hooks()
        {
            Stage.onServerStageComplete += Stage_onServerStageComplete;
        }


        private void Stage_onServerStageComplete(Stage stage)
        {
            foreach (var player in PlayerCharacterMasterController.instances)
            {
                if (player
                    && player.master
                    && player.master.inventory)
                {
                    bool hasEgg = false;
                    foreach (var slot in player.master.inventory.equipmentStateSlots)
                    {
                        if (slot.equipmentDef == EquipmentDef)
                        {
                            hasEgg = true;
                            break;
                        }
                    }
                    if (hasEgg)
                    {
                        player.master.inventory.GiveItem(Items.WeirdEggStageClearTally.instance.ItemDef);
                    }
                    else
                    {
                        var itemCount = player.master.inventory.GetItemCount(Items.WeirdEggStageClearTally.instance.ItemDef);
                        player.master.inventory.RemoveItem(Items.WeirdEggStageClearTally.instance.ItemDef, itemCount);
                    }
                }
            }
        }

        protected override void CreateConfig(ConfigFile config)
        {
            base.CreateConfig(config);
            cfgCooldown = config.Bind(ConfigCategory, CooldownName, 25f, CooldownDescription);
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
localPos = new Vector3(0.01985F, -0.03309F, -0.24363F),
localAngles = new Vector3(347.7642F, 0.0088F, 359.9185F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.07458F, -0.11516F, 0.11977F),
localAngles = new Vector3(16.03564F, 317.6382F, 175.0079F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.75015F, 2.23362F, 2.00361F),
localAngles = new Vector3(357.5546F, 67.51758F, 348.0091F),
localScale = new Vector3(0.5F, 0.5F, 0.5F)
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
localPos = new Vector3(0.12428F, 0.03152F, -0.11144F),
localAngles = new Vector3(342.3648F, 180F, 180F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(0F, 0.12396F, 0.15182F),
localAngles = new Vector3(20.98186F, 180F, 180F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
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
childName = "Pelvis",
localPos = new Vector3(0.14304F, 0.02105F, -0.13166F),
localAngles = new Vector3(11.58481F, 92.54247F, 189.2837F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
      childName = "Head",
localPos = new Vector3(-0.00033F, -0.96352F, 1.60106F),
localAngles = new Vector3(5.98044F, 87.96165F, 147.5727F),
localScale = new Vector3(1F, 1F, 1F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.18256F, -0.04705F, -0.22638F),
localAngles = new Vector3(342.1707F, 62.32212F, 4.39398F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
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
localPos = new Vector3(0.22389F, -0.00003F, -0.00001F),
localAngles = new Vector3(8.46545F, 180F, 180F),
localScale = new Vector3(0.11226F, 0.11226F, 0.11226F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
              childName = "Pelvis",
localPos = new Vector3(-0.13679F, 0.02986F, -0.14214F),
localAngles = new Vector3(20.0726F, 264.6954F, 176.4028F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
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

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (Util.HasEffectiveAuthority(slot.gameObject))
            {
                if (slot?.characterBody?.healthComponent?.health < slot.characterBody.healthComponent.fullHealth)
                {
                    slot.characterBody.healthComponent.CallCmdHealFull();

                    slot.inventory.SetEquipmentIndex(EquipmentIndex.None);
                    CharacterMasterNotificationQueue.PushEquipmentTransformNotification(slot.characterBody.master, WeirdEgg.Instance.EquipmentDef.equipmentIndex, EquipmentIndex.None, CharacterMasterNotificationQueue.TransformationType.Default);
                    return true;
                }
            }
            return false;
        }
    }
}