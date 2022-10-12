using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using UnityEngine.Networking;
using System.Text;
using RoR2.UI;

namespace RiskOfBulletstormRewrite.Items
{
    public class Backpack : ItemBase<Backpack>
    {
        public KeyCode ModifierButton { get; private set; } = KeyCode.LeftShift;
        public KeyCode CycleLeftButton { get; private set; } = KeyCode.F;
        public KeyCode CycleRightButton { get; private set; } = KeyCode.G;
        public static bool UseScrollWheel { get; private set; } = true;

        public override string ItemName => "Backpack";

        public override string ItemLangTokenName => "BACKPACK";

        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] ItemTags => new[] { ItemTag.Utility, ItemTag.EquipmentRelated, ItemTag.AIBlacklist };

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("Assets/Models/Prefabs/Backpack.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("Assets/Textures/Icons/Backpack.png");

        public static KeyCode ModifierKey_KB = KeyCode.None;
        public static KeyCode CycleLeftKey_KB = KeyCode.None;
        public static KeyCode CycleRightKey_KB = KeyCode.None;

        public KeyCode CycleLeftKey_GP = KeyCode.None;
        public KeyCode CycleRightKey_GP = KeyCode.None;

        public static GameObject ItemBodyModelPrefab;
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


            ModifierKey_KB = ModifierButton;
            CycleLeftKey_KB = CycleLeftButton;
            CycleRightKey_KB = CycleRightButton;
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            CharacterMaster.onStartGlobal += GiveComponent;
            //On.RoR2.UI.EquipmentIcon.Update += EquipmentIcon_Update;
            On.RoR2.BodyCatalog.Init += GetBodyIndex;
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
                backpackComponent.localUser = LocalUserManager.readOnlyLocalUsersList[0];
                backpackComponent.inventory = characterMaster.inventory;
                backpackComponent.characterMaster = characterMaster;
                backpackComponent.RunOnce();
            }
        }

        //fuck this
        private void EquipmentIcon_Update(On.RoR2.UI.EquipmentIcon.orig_Update orig, EquipmentIcon self)
        {
            orig(self);
            if (self.targetInventory)
            {
                var itemCount = self.targetInventory.GetItemCount(ItemDef);
                if (itemCount > 0)
                {
                    self.stockText.gameObject.SetActive(true);
                    StringBuilder stringBuilder2 = HG.StringBuilderPool.RentStringBuilder();
                    var equipmentSlotCount = self.targetInventory.GetEquipmentSlotCount();
                    if (self.displayAlternateEquipment)
                    {
                        uint nextSlot = LoopAround(self.targetEquipmentSlot.activeEquipmentSlot++, 0, (uint)(equipmentSlotCount - 1));
                        self.currentDisplayData.equipmentDef = self.targetInventory.equipmentStateSlots[nextSlot].equipmentDef;
                        stringBuilder2.Append($"[Slot:{nextSlot}/Ct:{equipmentSlotCount}]\n");
                        stringBuilder2.AppendInt(self.currentDisplayData.stock, 1U, uint.MaxValue);

                        Texture texture = null;
                        Color color = Color.clear;
                        if (self.currentDisplayData.equipmentDef != null)
                        {
                            color = ((self.currentDisplayData.stock > 0) ? Color.white : Color.gray);
                            texture = self.currentDisplayData.equipmentDef.pickupIconTexture;
                        }
                        self.iconImage.texture = texture;
                        self.iconImage.color = color;
                    } else
                    {
                        stringBuilder2.Append($"[Slot:{self.targetEquipmentSlot.activeEquipmentSlot}]\n");
                        stringBuilder2.AppendInt(self.currentDisplayData.stock, 1U, uint.MaxValue);
                    }
                    self.stockText.SetText(stringBuilder2);
                    HG.StringBuilderPool.ReturnStringBuilder(stringBuilder2);
                }
            }
        }

        public static uint LoopAround(uint value, uint min, uint max)
        {
            if (value < min) value = max;
            else if (value > max) value = min;
            return value;
        }

        public class BackpackComponent : MonoBehaviour
        {
            public LocalUser localUser;
            public CharacterMaster characterMaster;
            public CharacterBody characterBody;
            public Inventory inventory;
            private byte itemCount = 0;
            private byte extraSlotCount = 0;
            public byte selectableSlot = 0;
            readonly string equipmentPrefix = "Selected slot ";

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
                    inventory.SetActiveEquipmentSlot(selectableSlot);
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
                if (!localUser.isUIFocused) //TY KingEnderBrine for the help
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
                                var equipmentStateSlots = inventory.equipmentStateSlots;
                                if (equipmentStateSlots.Length > 0)
                                {
                                    for (int i = 0; i <= selectableSlot; i++)
                                    {
                                        var eqpName = "None";
                                        var charges = -6;
                                        var cooldown = -7;
                                        if (i < equipmentStateSlots.Length) //prevents out of bounds error from unset slots
                                        {
                                            var eqp = equipmentStateSlots[i];
                                            if (eqp.equipmentIndex != EquipmentIndex.None)
                                            {
                                                eqpName = RoR2.Language.GetString(eqp.equipmentDef.nameToken);
                                            }
                                            charges = eqp.charges;
                                            cooldown = eqp.isPerfomingRecharge ? Mathf.Max((int)eqp.chargeFinishTime.timeUntil, 0) : cooldown;
                                        }
                                        // Slot 0: "[1] Bomb 5x CD:10"
                                        Chat.AddMessage(
                                            "[" + (i) + "] " +
                                            eqpName +
                                            (charges == -6 ? "" : " " + charges + "x") +
                                            (cooldown == -7 ? "" : " CD:" + cooldown + " ")
                                            );
                                    }
                                }
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
                inventory.SetActiveEquipmentSlot(i);
            }
            private void CycleSlot(bool cycleRight)
            {
                var currentSlot = inventory.activeEquipmentSlot + (cycleRight ? 1 : -1);
                var newValue = (byte)LoopAround(currentSlot, 0, selectableSlot);
                inventory.SetActiveEquipmentSlot(newValue);
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
