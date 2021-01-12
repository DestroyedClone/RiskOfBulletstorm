
using System.Collections.Generic;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using RoR2.UI;
using Unity;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using BepInEx;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using static RiskOfBulletstorm.HelperPlugin;
using static RiskOfBulletstorm.RiskofBulletstorm;

namespace RiskOfBulletstorm.Items
{
    public class Backpack : Item_V2<Backpack>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What button should be held down to choose which slot to select with number keys?", AutoConfigFlags.None)]
        public KeyCode Backpack_ModifierButton { get; private set; } = KeyCode.LeftShift;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What button should be pressed down to cycle to the previous equipment slot?", AutoConfigFlags.None)]
        public KeyCode Backpack_CycleLeftButton { get; private set; } = KeyCode.F;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What button should be pressed down to cycle to the next equipment slot", AutoConfigFlags.None)]
        public KeyCode Backpack_CycleRightButton { get; private set; } = KeyCode.G;
        //https://docs.unity3d.com/ScriptReference/KeyCode.html
        public override string displayName => "Backpack";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.EquipmentRelated, ItemTag.AIBlacklist });
        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Item Capacity Up!\nThe Backpack grants you the use of another Active Item. Useful, but cumbersome.";

        //protected override string GetDescString(string langid = null) => $"Grants one extra equipment slot." +
            //$"\nUse your modifier key+number keys or your cycle keys to switch slots.";

        protected override string GetLoreString(string langID = null) => "";
        public static KeyCode ModifierKey_KB = KeyCode.None;
        public static KeyCode CycleLeftKey_KB = KeyCode.None;
        public static KeyCode CycleRightKey_KB = KeyCode.None;

        public KeyCode CycleLeftKey_GP = KeyCode.None;
        public KeyCode CycleRightKey_GP = KeyCode.None;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

        }
        public override void SetupConfig()
        {
            base.SetupConfig();
            /*
            if (RiskOfOptionsCompat.enabled)
            {
                ModifierKey_KB = (KeyCode)Enum.Parse(typeof(KeyCode), RiskOfOptionsCompat.getOptionValue("BACKPACK: Modifier (Keyboard)"));
                CycleLeftKey_KB = (KeyCode)Enum.Parse(typeof(KeyCode), RiskOfOptionsCompat.getOptionValue("BACKPACK: Modifier (Keyboard)"));
                CycleRightKey_KB = (KeyCode)Enum.Parse(typeof(KeyCode), RiskOfOptionsCompat.getOptionValue("BACKPACK: Modifier (Keyboard)"));

                CycleLeftKey_GP = (KeyCode)Enum.Parse(typeof(KeyCode), RiskOfOptionsCompat.getOptionValue("BACKPACK: Modifier (Gamepad)"));
                CycleRightKey_GP = (KeyCode)Enum.Parse(typeof(KeyCode), RiskOfOptionsCompat.getOptionValue("BACKPACK: Modifier (Gamepad)"));
            } else*/

            ModifierKey_KB = Backpack_ModifierButton;
                CycleLeftKey_KB = Backpack_CycleLeftButton;
                CycleRightKey_KB = Backpack_CycleRightButton;
            
        }
        protected override string GetDescString(string langid = null) => $"Grants one extra equipment slot." +
            $"\nHold {ModifierKey_KB} and press 1-0 to switch equipment slots." +
            $"\nPress {CycleLeftKey_KB}/{CycleLeftKey_GP} or {CycleRightKey_KB}/{CycleRightKey_GP} to cycle slots";


        public override void Install()
        {
            base.Install();
            On.RoR2.CharacterBody.Start += CharacterBody_Start;
            On.RoR2.PickupDropletController.CreatePickupDroplet += PickupDropletController_CreatePickupDroplet;
            On.RoR2.Inventory.GiveItem += PreventMULTFromGrabbing;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.Start -= CharacterBody_Start;
            On.RoR2.PickupDropletController.CreatePickupDroplet -= PickupDropletController_CreatePickupDroplet;
            On.RoR2.Inventory.GiveItem -= PreventMULTFromGrabbing;
        }

        private void PreventMULTFromGrabbing(On.RoR2.Inventory.orig_GiveItem orig, Inventory self, ItemIndex itemIndex, int count)
        {
            var body = self.gameObject.GetComponent<PlayerCharacterMasterController>()?.body;
            if (body && body.baseNameToken.ToUpper().Contains("TOOLBOT"))
            {
                var loot = Run.instance.treasureRng.NextElementUniform(Run.instance.availableTier3DropList);
                PickupDropletController.CreatePickupDroplet(loot, self.transform.position, body.inputBank.aimDirection * 5f);
                itemIndex = ItemIndex.None;
            }

            orig(self, itemIndex, count);
        }

        private void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            //var body = PlayerCharacterMasterController.instances[0].master.GetBody();
            if (IsThereARobotInThisSinglePlayerGame())
            {
                if (pickupIndex != PickupCatalog.FindPickupIndex(ItemIndex.ArtifactKey) && pickupIndex == instance.pickupIndex)
                {
                    PickupIndex loot = Run.instance.treasureRng.NextElementUniform(Run.instance.availableTier3DropList);
                    pickupIndex = PickupCatalog.GetPickupDef(loot).pickupIndex;
                    Chat.AddMessage("Sorry, MUL-T breaks the backpack, so I replaced it with a random red!");
                }
            }
            orig(pickupIndex, position, velocity);
        }

        private bool IsThereARobotInThisSinglePlayerGame()
        {
            var list = PlayerCharacterMasterController.instances;
            if (list.Count != 1) return false;

            if (list[0].body.baseNameToken.ToUpper().Contains("TOOLBOT"))
                return true;
            return false;
        }
        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active && self.isPlayerControlled)
            {
                var masterObj = self.masterObject;
                BackpackComponent backpackComponent = masterObj.GetComponent<BackpackComponent>();
                if (!backpackComponent) { backpackComponent = masterObj.AddComponent<BackpackComponent>(); }
                backpackComponent.characterBody = self;
                backpackComponent.localUser = LocalUserManager.readOnlyLocalUsersList[0];
                backpackComponent.inventory = self.inventory;
                //backpackComponent.UpdateDefault();
            }
        }


        public class BackpackComponent : MonoBehaviour
        {
            public LocalUser localUser;
            public CharacterBody characterBody;
            public Inventory inventory;
            public byte maxAvailableSlot = 0;
            bool canDrop = false;
            //private bool isToolbot = false;
            //private byte defaultMax = 0;
            public List<EquipmentIndex> equipmentDropQueue = new List<EquipmentIndex>();
            private readonly float dropCooldown = 1f;
            private float stopwatch = 9999f;


            public void Start()
            {
                stopwatch = dropCooldown;
                characterBody.onInventoryChanged += CharacterBody_onInventoryChanged;
                //defaultMax = (byte)(characterBody.baseNameToken.ToUpper().Contains("TOOLBOT") ? 1 : 0);

                //if (isToolbot)


                //isToolbot = characterBody.baseNameToken.ToUpper().Contains("TOOLBOT");
            }

            public void OnDisable()
            {
                if (characterBody)
                    characterBody.onInventoryChanged -= CharacterBody_onInventoryChanged;
            }

            private void CharacterBody_onInventoryChanged()
            {
                var invcount = (byte)inventory.GetItemCount(instance.catalogIndex);

                if (maxAvailableSlot > invcount)
                {
                    var difference = maxAvailableSlot - invcount;
                    Chat.AddMessage("Backpack: Difference was " + difference);
                    for (int i = 1; i <= difference; i++)
                    {
                        var slot = (byte)(invcount + i);
                        if (inventory.equipmentStateSlots.Length > i)
                        {
                            Chat.AddMessage("Backpack: Queueing Slot " + slot);
                            QueueDrop(slot);
                        }
                    }

                    // Check here so if they're under it doesn't force them, only if they're above
                    if (inventory.activeEquipmentSlot > invcount)
                        inventory.SetActiveEquipmentSlot(invcount);
                }
                if (maxAvailableSlot != invcount)
                {
                    maxAvailableSlot = invcount;
                }
            }

            public void Update()
            {
                if (!localUser.isUIFocused) //TY KingEnderBrine for the help
                {
                    var inventory = characterBody.inventory;
                    if (inventory && inventory.GetItemCount(instance.catalogIndex) > 0)
                    {
                        if (Input.GetKey(instance.Backpack_ModifierButton))
                        {
                            for (byte i = 0; i < 10; i++)
                            {
                                if (Input.GetKeyDown(KeyCode.Alpha1 + i)) { SetEquipmentSlot(i); break; }
                            };

                            if (Input.GetKeyDown(KeyCode.Equals))
                            {
                                var equipmentStateSlots = inventory.equipmentStateSlots;
                                if (equipmentStateSlots.Length > 0)
                                {
                                    for (int i = 0; i <= maxAvailableSlot; i++)
                                    {
                                        var eqpName = "None";
                                        var charges = -6;
                                        var cooldown = -7;
                                        if (i < equipmentStateSlots.Length) //prevents out of bounds error from unset slots
                                        {
                                            var eqp = equipmentStateSlots[i];
                                            if (eqp.equipmentIndex != EquipmentIndex.None)
                                            {
                                                eqpName = eqp.equipmentDef.name;
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
                    }
                }
            }

            public void FixedUpdate()
            {
                // Stopwatch //
                stopwatch -= Time.deltaTime;
                if (canDrop)
                {
                    canDrop = false;
                    for (byte i = 0; i <= equipmentDropQueue.Count - 1; i++)
                    {
                        if (equipmentDropQueue[i] != EquipmentIndex.None)
                        {
                            _logger.LogMessage("Slot" + i + " selected for drop");
                            CreateDropletFromEquipmentSlot(i);
                        }
                    }
                    equipmentDropQueue.Clear();
                }
                if (stopwatch < 0)
                {
                    canDrop = true;
                    stopwatch = dropCooldown;
                }
            }

            private void SetEquipmentSlot(byte i)
            {
                if (i > maxAvailableSlot)
                {
                    return;
                }
                Chat.AddMessage("Backpack: Set equipment slot to " + i);
                inventory.SetActiveEquipmentSlot(i);
            }

            private void QueueDrop(byte equipmentSlot)
            {
                if (inventory.equipmentStateSlots.Length < equipmentSlot)
                {
                    _logger.LogWarning("RiskOfBulletstorm: Backpack was told to drop slot " + equipmentSlot + " but it is out of bounds of length " + inventory.equipmentStateSlots.Length);
                    return;
                }
                var equipment = inventory.equipmentStateSlots[equipmentSlot];
                var index = equipment.equipmentIndex;

                if (inventory.equipmentStateSlots.Length > 0 && index != EquipmentIndex.None)
                {
                    _logger.LogMessage("QueueDrop: Check succeeded.");

                    equipmentDropQueue.Add(index);
                    SetEquipmentIndexUnsafe(inventory, EquipmentIndex.None, equipmentSlot);
                    stopwatch = dropCooldown;
                    canDrop = false;
                }
            }

            [Server]
            public void SetEquipmentIndexUnsafe(Inventory inventory, EquipmentIndex newEquipmentIndex, byte equipmentSlot)
            {
                if (!NetworkServer.active) return;

                EquipmentState equipment = inventory.GetEquipment(equipmentSlot);
                byte charges = equipment.charges;
                Run.FixedTimeStamp chargeFinishTime = equipment.chargeFinishTime;
                if (equipment.equipmentIndex == EquipmentIndex.None && chargeFinishTime.isNegativeInfinity)
                {
                    charges = 1;
                    chargeFinishTime = Run.FixedTimeStamp.now;
                }
                EquipmentState equipmentState = new EquipmentState(newEquipmentIndex, chargeFinishTime, charges);
                inventory.SetEquipment(equipmentState, equipmentSlot);
            }

            private void CreateDropletFromEquipmentSlot(byte equipmentSlot)
            {
                var slot = equipmentDropQueue[equipmentSlot];
                var pickupIndex = PickupCatalog.FindPickupIndex(slot);
                PickupDropletController.CreatePickupDroplet(pickupIndex, characterBody.corePosition, Vector3.up * 5);
            }

            private void CycleSlot(bool cycleRight)
            {
                var currentSlot = inventory.activeEquipmentSlot + (cycleRight ? 1 : -1);
                var newValue = (byte)LoopAround(currentSlot, 0, maxAvailableSlot);
                inventory.SetActiveEquipmentSlot(newValue);
                Chat.AddMessage("Backpack: Set equipment slot to " + newValue);
            }

            private float LoopAround(float value, float min, float max)
            {
                if (value < min) value = max;
                else if (value > max) value = min;
                return value;
            }
        }
    }
}
