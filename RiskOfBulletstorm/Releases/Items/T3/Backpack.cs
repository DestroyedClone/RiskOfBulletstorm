
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

            public void OnEnable()
            {
                //isToolbot = characterBody.baseNameToken == "TOOLBOT_BODY_NAME";
                //if (isToolbot) defaultMax++;
                //inventory = characterBody.inventory;
                characterBody.onInventoryChanged += CharacterBody_onInventoryChanged;
            }

            private void CharacterBody_onInventoryChanged()
            {
                var invcount = Math.Min(inventory.GetItemCount(instance.catalogIndex), 0); ;
                
                if (maxAvailableSlot < invcount)
                {
                    var difference = invcount - maxAvailableSlot;
                    for (int i = 0; i < difference; i++)
                    {
                        DropEquipSlot((byte)(maxAvailableSlot + i));
                    }
                }
                maxAvailableSlot = (byte)invcount;

            }

            public void OnDisable()
            {
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
                            if (Input.GetKeyDown(KeyCode.Alpha1)) SetEquipmentSlot(inventory, 0);
                            else if (Input.GetKeyDown(KeyCode.Alpha2)) SetEquipmentSlot(inventory, 1);
                            else if (Input.GetKeyDown(KeyCode.Alpha3)) SetEquipmentSlot(inventory, 2);
                            else if (Input.GetKeyDown(KeyCode.Alpha4)) SetEquipmentSlot(inventory, 3);
                            else if (Input.GetKeyDown(KeyCode.Alpha5)) SetEquipmentSlot(inventory, 4);
                            else if (Input.GetKeyDown(KeyCode.Alpha6)) SetEquipmentSlot(inventory, 5);
                            else if (Input.GetKeyDown(KeyCode.Alpha7)) SetEquipmentSlot(inventory, 6);
                            else if (Input.GetKeyDown(KeyCode.Alpha8)) SetEquipmentSlot(inventory, 7);
                            else if (Input.GetKeyDown(KeyCode.Alpha9)) SetEquipmentSlot(inventory, 8);
                            else if (Input.GetKeyDown(KeyCode.Alpha0)) SetEquipmentSlot(inventory, 9);
                            else if (Input.GetKeyDown(KeyCode.Underscore))
                            {
                                var equipmentStateSlots = inventory.equipmentStateSlots;
                                for (int i = 0; i < equipmentStateSlots.Length - 1; i++)
                                {
                                    var eqp = equipmentStateSlots[i];
                                    Chat.AddMessage("[" + i+1 + "] : "+eqp.equipmentDef.name);
                                }
                            }
                        }
                    }
                }
            }

            private void SetEquipmentSlot(Inventory inventory, byte i)
            {
                var equipmentCount = inventory.GetEquipmentSlotCount();
                //var value = (byte)Mathf.Min(i, equipmentCount);
                if (i > equipmentCount || i > maxAvailableSlot)
                {
                    Chat.AddMessage("Backpack: Selected slot "+i+" is greater than "+ equipmentCount);
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
