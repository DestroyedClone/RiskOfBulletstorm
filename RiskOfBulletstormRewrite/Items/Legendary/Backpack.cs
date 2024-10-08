﻿using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using RoR2.Items;
using RoR2.UI;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Items
{
    public class Backpack : ItemBase<Backpack>
    {
        public KeyCode ModifierButton { get; private set; } = KeyCode.LeftShift;
        public KeyCode CycleLeftButton { get; private set; } = KeyCode.F;
        public KeyCode CycleRightButton { get; private set; } = KeyCode.G;
        public static bool UseScrollWheel { get; private set; } = true;
        public static ConfigEntry<bool> cfgToolbotSwapsModded;

        public override string ItemName => "Backpack";

        public override string ItemLangTokenName => "BACKPACK";

        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] ItemTags => new[] { ItemTag.Utility, ItemTag.EquipmentRelated, ItemTag.AIBlacklist };

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();

        public static KeyCode ModifierKey_KB = KeyCode.None;
        public static KeyCode CycleLeftKey_KB = KeyCode.None;
        public static KeyCode CycleRightKey_KB = KeyCode.None;

        public KeyCode CycleLeftKey_GP = KeyCode.None;
        public KeyCode CycleRightKey_GP = KeyCode.None;

        public static BodyIndex ToolbotBodyIndex;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            ModifierButton = config.Bind(ConfigCategory, "What button should be held down to choose which slot to select with number keys?", KeyCode.LeftShift, "").Value;
            CycleLeftKey_KB = config.Bind(ConfigCategory, "What button should be pressed down to cycle to the previous equipment slot?", KeyCode.F, "").Value;
            CycleRightKey_KB = config.Bind(ConfigCategory, "What button should be pressed down to cycle to the next equipment slot", KeyCode.G, "").Value;
            UseScrollWheel = config.Bind(ConfigCategory, "Should the scrollwheel be used in addition to the cycle buttons?", true, "").Value;
            cfgToolbotSwapsModded = config.Bind(ConfigCategory, "MUL-T swaps modded", true, "If true, then MUL-T Retool will swap between the last selected slot.");

            ModifierKey_KB = ModifierButton;
            CycleLeftKey_KB = CycleLeftButton;
            CycleRightKey_KB = CycleRightButton;
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = ItemModel;
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
localPos = new Vector3(0.0064F, 0.417F, -0.26227F),
localAngles = new Vector3(325.2865F, 233.8595F, 322.9989F),
localScale = new Vector3(0.15374F, 0.15374F, 0.15374F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.16342F, 0.19965F, -0.14882F),
localAngles = new Vector3(0F, 180F, 0F),
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
localPos = new Vector3(-0.07737F, 1.00593F, -2.34323F),
localAngles = new Vector3(-0.00008F, 220.7458F, 0.00001F),
localScale = new Vector3(1F, 1F, 1F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.00252F, 0.2011F, -0.38373F),
localAngles = new Vector3(354.7345F, 223.2252F, 350.3122F),
localScale = new Vector3(0.15178F, 0.15178F, 0.15178F)
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0F, -0.00004F, -0.48534F),
localAngles = new Vector3(0F, 221.3043F, 0F),
localScale = new Vector3(0.5F, 0.5F, 0.5F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
     childName = "Chest",
localPos = new Vector3(-0.02328F, 0.23717F, -0.26346F),
localAngles = new Vector3(349.3948F, 228.0375F, 355.2831F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.0056F, 0.29002F, -0.22241F),
localAngles = new Vector3(349.4298F, 227.4896F, 350.5667F),
localScale = new Vector3(0.07786F, 0.07786F, 0.07786F)
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
localAngles = new Vector3(344.4032F, 137.7126F, 180F),
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
localPos = new Vector3(0.00212F, 0.28659F, -0.32409F),
localAngles = new Vector3(350.1622F, 232.4422F, 355.0652F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
 childName = "Chest",
localPos = new Vector3(0F, -0.00009F, -1.99399F),
localAngles = new Vector3(0F, 230.4314F, 0F),
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
localPos = new Vector3(-0.03924F, 0.22635F, -0.18913F),
localAngles = new Vector3(1.49579F, 239.8228F, 358.5154F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "chest",
localPos = new Vector3(0.02159F, 0.35501F, -0.1903F),
localAngles = new Vector3(341.6615F, 218.0503F, 0F),
localScale = new Vector3(0.10951F, 0.10951F, 0.10951F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
   childName = "Chest",
localPos = new Vector3(-0.00001F, 0.2799F, -0.19949F),
localAngles = new Vector3(0F, 227.7147F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
           childName = "Chest",
localPos = new Vector3(0F, 0.27205F, -0.15553F),
localAngles = new Vector3(0F, 232.7527F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlRailGunner", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
            childName = "Chest",
localPos = new Vector3(0.13045F, -0.03158F, 0.00752F),
localAngles = new Vector3(29.93956F, 129.4898F, 343.6404F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlVoidSurvivor", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
          childName = "Chest",
localPos = new Vector3(-0.00538F, 0.11036F, -0.20132F),
localAngles = new Vector3(11.38334F, 221.3227F, 8.18337F),
localScale = new Vector3(0.10787F, 0.10787F, 0.10787F)
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
            CharacterMaster.onStartGlobal += GiveComponent;
            //On.RoR2.UI.EquipmentIcon.Update += EquipmentIcon_Update;
            On.RoR2.BodyCatalog.Init += GetBodyIndex;
            //On.RoR2.UI.EquipmentIcon.Update += ShowCurrentSlot;

            if (cfgToolbotSwapsModded.Value)
            {
                On.EntityStates.Toolbot.ToolbotStanceBase.SetEquipmentSlot += ToolbotStanceBase_SetEquipmentSlot;
            }
        }

        private void ToolbotStanceBase_SetEquipmentSlot(On.EntityStates.Toolbot.ToolbotStanceBase.orig_SetEquipmentSlot orig, EntityStates.Toolbot.ToolbotStanceBase self, byte i)
        {
            CharacterBody characterBody = self.outer.commonComponents.characterBody;
            BackpackComponent backpackComponent = null;
            bool v = (bool)(characterBody?.master?.TryGetComponent(out backpackComponent));
            if (backpackComponent)
            {
                backpackComponent.SetActiveEquipmentSlot_Retool(orig, self, i, backpackComponent.lastEquipmentSlot);
            }
            else
            {
                orig(self, i);
            }
            //bool isFirstStance = self is ToolbotStanceA;
        }

        private void ShowCurrentSlot(On.RoR2.UI.EquipmentIcon.orig_Update orig, EquipmentIcon self)
        {
            orig(self);
            if (self.targetInventory)
            {
                var itemCount = self.targetInventory.GetItemCount(ItemDef);
                if (itemCount > 0)
                {
                    if (!self.displayAlternateEquipment) //aka main
                    {
                        if (self.stockText)
                        {
                            self.stockText.gameObject.SetActive(true);
                            StringBuilder stringBuilder2 = HG.StringBuilderPool.RentStringBuilder();
                            //var equipmentSlotCount = self.targetInventory.GetEquipmentSlotCount();

                            stringBuilder2.Append($"Active: [{self.targetEquipmentSlot.activeEquipmentSlot}]\n");

                            //stringBuilder2.AppendInt(self.currentDisplayData.stock, 1U, uint.MaxValue);
                            stringBuilder2.Append($"x{self.currentDisplayData.stock}");

                            self.stockText.SetText(stringBuilder2);
                            HG.StringBuilderPool.ReturnStringBuilder(stringBuilder2);
                        }
                    }
                }
            }
        }

        public void GetBodyIndex(On.RoR2.BodyCatalog.orig_Init orig)
        {
            orig();
            ToolbotBodyIndex = BodyCatalog.FindBodyIndex("ToolbotBody");
        }

        private void GiveComponent(CharacterMaster characterMaster)
        {
            if (characterMaster.hasAuthority && characterMaster.playerCharacterMasterController)
            {
                BackpackComponent backpackComponent = characterMaster.GetComponent<BackpackComponent>();
                if (!backpackComponent) { backpackComponent = characterMaster.gameObject.AddComponent<BackpackComponent>(); }
                //backpackComponent.characterBody = characterMaster.GetBody();
                backpackComponent.localUser = characterMaster.playerCharacterMasterController.networkUser.localUser;
                backpackComponent.inventory = characterMaster.inventory;
                backpackComponent.characterMaster = characterMaster;
                backpackComponent.RunOnce();
            }
        }

        public static uint LoopAround(uint value, uint min, uint max)
        {
            if (value < min) value = max;
            else if (value > max) value = min;
            return value;
        }

        /*
        public class RBSBackpackComponent : BaseItemBodyBehavior
        {
            [ItemDefAssociation(useOnClient = true, useOnServer = true)]
            public static ItemDef GetItemDef() => instance.ItemDef;

            public LocalUser localUser;
            public CharacterMaster characterMaster;
            public Inventory inventory;
            private uint itemCount = 0U;
            private byte naturalExtraSlotCount = 0;
            public byte selectableSlot = 0;
            public byte lastEquipmentSlot = 0;

            public void OnEnable()
            {
                if (!body.isPlayerControlled)
                {
                    enabled = false;
                    return;
                }
                localUser = LocalUserManager.GetFirstLocalUser();
                characterMaster = body.master;
                inventory = body.inventory;

                if (body.bodyIndex == ToolbotBodyIndex)
                    naturalExtraSlotCount = 1;
            }

            public void OnDisable()
            {
            }

            private void Inventory_onInventoryChanged()
            {
                UpdateCount();
                if (inventory.activeEquipmentSlot > selectableSlot)
                    SetActiveEquipmentSlot(selectableSlot);
            }
            public void UpdateCount()
            {
                itemCount = (byte)inventory.GetItemCount(Backpack.instance.ItemDef);
                selectableSlot = (byte)(itemCount + naturalExtraSlotCount);
            }
        }*/

        public class BackpackComponent : MonoBehaviour
        {
            public LocalUser localUser;
            public CharacterMaster characterMaster;
            public Inventory inventory;
            private byte itemCount = 0;
            private byte extraSlotCount = 0;
            public byte selectableSlot = 0;
            //private readonly string equipmentPrefix = "Selected slot ";

            public byte lastEquipmentSlot = 0;

            public void RunOnce()
            {
                Subscribe();
                OnInventoryChanged();
                OnBodyStart(characterMaster.GetBody());
            }

            public void Subscribe()
            {
                inventory.onInventoryChanged += OnInventoryChanged;
                characterMaster.onBodyStart += OnBodyStart;
            }

            public void OnDestroy()
            {
                inventory.onInventoryChanged -= OnInventoryChanged;
                characterMaster.onBodyStart -= OnBodyStart;
            }

            public void OnInventoryChanged()
            {
                UpdateCount();
                if (inventory.activeEquipmentSlot > selectableSlot)
                    SetActiveEquipmentSlot(selectableSlot);
            }

            public void SetActiveEquipmentSlot(byte slot)
            {
                //var oldEquipmentSlot = lastEquipmentSlot;
                lastEquipmentSlot = inventory.activeEquipmentSlot;
                //var newEquipmentSlot = lastEquipmentSlot;
                //Chat.AddMessage($"LastEquipment: {oldEquipmentSlot}->{newEquipmentSlot}, Current set to {slot}");
                Server_SetActiveEquipmentSlot(slot);
            }

            public void Server_SetActiveEquipmentSlot(byte slot)
            {
                if (NetworkServer.active)
                    inventory.SetActiveEquipmentSlot(slot);
                else
                {
                    Console.instance.SubmitCmd(characterMaster.playerCharacterMasterController.networkUser, $"rbs_backpacksetslot {slot}", true);
                }
            }

            public void SetActiveEquipmentSlot_Retool(On.EntityStates.Toolbot.ToolbotStanceBase.orig_SetEquipmentSlot orig, EntityStates.Toolbot.ToolbotStanceBase self, byte originalRequestedSlot, byte replaceRequestedSlot)
            {
                lastEquipmentSlot = inventory.activeEquipmentSlot;
                orig(self, originalRequestedSlot);
                Server_SetActiveEquipmentSlot(replaceRequestedSlot);
            }

            public void OnBodyStart(CharacterBody characterBody)
            {
                if (characterBody)
                    extraSlotCount = (byte)(characterBody.bodyIndex == ToolbotBodyIndex ? 1 : 0);
            }

            public void UpdateCount()
            {
                itemCount = (byte)inventory.GetItemCount(Backpack.instance.ItemDef);
                selectableSlot = (byte)(itemCount + extraSlotCount);
            }

            public void Update()
            {
                if (localUser != null && !localUser.isUIFocused) //TY KingEnderBrine for the help
                {
                    if (inventory && inventory.GetItemCount(instance.ItemDef) > 0)
                    {
                        if (Input.GetKey(instance.ModifierButton))
                        {
                            for (byte i = 0; i < 10; i++)
                            {
                                if (Input.GetKeyDown(KeyCode.Alpha1 + i)) { SetEquipmentSlot(i); break; }
                            };
                            if (Input.GetKeyDown(KeyCode.Alpha0)) { SetEquipmentSlot(9); }

                            if (Input.GetKeyDown(KeyCode.Equals))
                            {
                                Console.instance.SubmitCmd(characterMaster.playerCharacterMasterController.networkUser, "rbs_backpacklist", true);
                            }
                        }

                        if (Input.GetKeyDown(CycleLeftKey_KB))
                            CycleSlot(false);
                        if (Input.GetKeyDown(CycleRightKey_KB))
                            CycleSlot(true);

                        if (UseScrollWheel)
                        {
                            var dir = Input.mouseScrollDelta.y;
                            if (dir < 0)
                                CycleSlot(false);
                            else if (dir == 0)
                                return;
                            else
                                CycleSlot(true);
                        }
                    }
                }
            }

            private void SetEquipmentSlot(byte i)
            {
                if (i > selectableSlot)
                {
                    return;
                }
                //Chat.AddMessage(equipmentPrefix + i);
                SetActiveEquipmentSlot(i);
            }

            private void CycleSlot(bool cycleRight)
            {
                var currentSlot = inventory.activeEquipmentSlot + (cycleRight ? 1 : -1);
                var newValue = (byte)LoopAround(currentSlot, 0, selectableSlot);
                SetActiveEquipmentSlot(newValue);
                //Chat.AddMessage(equipmentPrefix + newValue);
            }

            public static float LoopAround(float value, float min, float max)
            {
                if (value < min) value = max;
                else if (value > max) value = min;
                return value;
            }
        }
    }
}