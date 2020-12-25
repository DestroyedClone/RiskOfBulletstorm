
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

namespace RiskOfBulletstorm.Items
{
    public class Backpack : Item_V2<Backpack>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What button should be held down to choose which slot to select?", AutoConfigFlags.None)]
        public KeyCode Backpack_ModifierButton { get; private set; } = KeyCode.LeftShift;
        //https://docs.unity3d.com/ScriptReference/KeyCode.html
        public override string displayName => "Backpack";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.EquipmentRelated, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Item Capacity Up!\nThe Backpack grants you the use of another Active Item. Useful, but cumbersome.";

        protected override string GetDescString(string langid = null) => $"Grants one extra equipment slot." +
            $"\nUse {Backpack_ModifierButton} and number keys to swap between slots." +
            $"\n<link=\"youtube.com/destroyedclone\">Test</link>";

        protected override string GetLoreString(string langID = null) => "";

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
        }


        public override void Install()
        {
            base.Install();
            On.RoR2.CharacterBody.Start += CharacterBody_Start;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.Start -= CharacterBody_Start;
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
                backpackComponent.Subscribe();
            }
        }


        public class BackpackComponent : MonoBehaviour
        {
            public LocalUser localUser;
            public CharacterBody characterBody;
            public Inventory inventory;
            public byte maxAvailableSlot = 0;
            //private bool isToolbot = false;
            //private byte defaultMax = 0;

            public void UpdateDefault()
            {
                //defaultMax = (byte)(characterBody.baseNameToken.ToUpper().Contains("TOOLBOT") ? 1 : 0);
            }

            public void Subscribe()
            {
                characterBody.onInventoryChanged += CharacterBody_onInventoryChanged;
            }

            private void CharacterBody_onInventoryChanged()
            {
                var invcount = (byte)inventory.GetItemCount(instance.catalogIndex);
                
                if (maxAvailableSlot > invcount)
                {
                    var difference = maxAvailableSlot - invcount;
                    Chat.AddMessage("Backpack: Difference is "+difference);
                    for (int i = 1; i <= difference; i++)
                    {
                        var slot = (byte)(maxAvailableSlot+i);
                        Chat.AddMessage("Backpack: Dropping Slot " + (slot));
                        DropEquipSlot(slot);
                    }

                    // Check here so if they're under it doesn't force them, only if they're above
                    if (inventory.activeEquipmentSlot > invcount)
                        inventory.SetActiveEquipmentSlot(invcount);
                }
                if (maxAvailableSlot != invcount)
                {
                    maxAvailableSlot = invcount;
                    Chat.AddMessage("Backpack: Updated allowed slots to " + maxAvailableSlot);
                }
            }

            public void OnDisable()
            {
                if (characterBody)
                    characterBody.onInventoryChanged -= CharacterBody_onInventoryChanged;
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
                            if (Input.GetKeyDown(KeyCode.Alpha1)) SetEquipmentSlot(0);
                            else if (Input.GetKeyDown(KeyCode.Alpha2)) SetEquipmentSlot(1);
                            else if (Input.GetKeyDown(KeyCode.Alpha3)) SetEquipmentSlot(2);
                            else if (Input.GetKeyDown(KeyCode.Alpha4)) SetEquipmentSlot(3);
                            else if (Input.GetKeyDown(KeyCode.Alpha5)) SetEquipmentSlot(4);
                            else if (Input.GetKeyDown(KeyCode.Alpha6)) SetEquipmentSlot(5);
                            else if (Input.GetKeyDown(KeyCode.Alpha7)) SetEquipmentSlot(6);
                            else if (Input.GetKeyDown(KeyCode.Alpha8)) SetEquipmentSlot(7);
                            else if (Input.GetKeyDown(KeyCode.Alpha9)) SetEquipmentSlot(8);
                            else if (Input.GetKeyDown(KeyCode.Alpha0)) SetEquipmentSlot(9);
                            else if (Input.GetKeyDown(KeyCode.Equals))
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
                                                eqpName = eqp.equipmentDef.nameToken;
                                            }
                                            charges = eqp.charges;
                                            cooldown = eqp.isPerfomingRecharge ? Mathf.Max((int)eqp.chargeFinishTime.timeUntil,0) : cooldown;
                                        }
                                        // Slot 0: "[1] Bomb 5x CD:10"
                                        Chat.AddMessage(
                                            "[" + (i) + "] " +
                                            eqpName +
                                            (charges == -6 ? "" : " "+ charges+"x") +
                                            (cooldown == -7 ? "" : " CD:"+ cooldown + " ")
                                            );
                                    }
                                }
                            }
                        }
                    }
                }
            }

            private void SetEquipmentSlot(byte i)
            {
                //var equipmentCount = inventory.GetEquipmentSlotCount() + defaultMax;
                //var value = (byte)Mathf.Min(i, equipmentCount);
                if (i > maxAvailableSlot)
                {
                    return;
                }
                Chat.AddMessage("Backpack: Set equipment slot to " + i);
                inventory.SetActiveEquipmentSlot(i);
            }

            private void DropEquipSlot(byte equipmentSlot)
            {
                var equipment = inventory.equipmentStateSlots[equipmentSlot];
                var index = equipment.equipmentIndex;
                if (index != EquipmentIndex.None)
                {
                    var pickupIndex = PickupCatalog.FindPickupIndex(index);
                    PickupDropletController.CreatePickupDroplet(pickupIndex, characterBody.corePosition, Vector3.up * 5);
                    equipment.equipmentIndex = EquipmentIndex.None;
                }
            }
        }
    }
}
