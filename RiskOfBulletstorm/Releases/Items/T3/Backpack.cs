
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
using  RiskOfBulletstorm.Utils;
using static RiskOfBulletstorm.Utils.HelperUtil;
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

        public static GameObject ItemBodyModelPrefab;
        public static int ToolbotBodyIndex;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

            //GameObject ScavBackpackPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/ScavBackpack.prefab");
            //ScavBackpackPrefab.GetComponent<MeshRenderer>().

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
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.Start -= CharacterBody_Start;
        }

        public override void SetupLate()
        {
            base.SetupLate();
            ToolbotBodyIndex = SurvivorCatalog.GetBodyIndexFromSurvivorIndex(SurvivorIndex.Toolbot);
        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active && self.isPlayerControlled)
            {
                var masterObj = self.masterObject;
                BackpackComponent backpackComponent = masterObj.GetComponent<BackpackComponent>();
                if (!backpackComponent) { backpackComponent = masterObj.AddComponent<BackpackComponent>(); }
                backpackComponent.ToolbotBodyIndex = ToolbotBodyIndex;
                backpackComponent.characterBody = self;
                backpackComponent.localUser = LocalUserManager.readOnlyLocalUsersList[0];
                backpackComponent.inventory = self.inventory;
                backpackComponent.Subscribe();
                backpackComponent.UpdateToolbot(self);
                backpackComponent.CharacterBody_onInventoryChanged();
            }
        }

        public class BackpackComponent : MonoBehaviour
        {
            public LocalUser localUser;
            public CharacterBody characterBody;
            public Inventory inventory;
            public byte inventoryCount = 0;
            public byte isToolbot = 0;
            public byte selectableSlot = 0;
            public int ToolbotBodyIndex;

            public void Start()
            {
                Chat.AddMessage("cum");
            }
            public void OnDisable()
            {
                if (characterBody)
                    characterBody.onInventoryChanged -= CharacterBody_onInventoryChanged;
            }
            public void Subscribe()
            {
                if (characterBody)
                    characterBody.onInventoryChanged += CharacterBody_onInventoryChanged;
            }
            public void UpdateToolbot(CharacterBody characterBody)
            {
                isToolbot = (byte)(characterBody.bodyIndex == ToolbotBodyIndex ? 1 : 0);
            }
            public void UpdateCount()
            {
                inventoryCount = (byte)inventory.GetItemCount(instance.catalogIndex);
                selectableSlot = (byte)(inventoryCount + isToolbot);
            }
            public void CharacterBody_onInventoryChanged()
            {
                UpdateCount();
                if (inventory.activeEquipmentSlot > inventoryCount)
                    inventory.SetActiveEquipmentSlot(inventoryCount);
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
                            if (Input.GetKeyDown(KeyCode.Alpha0)) { SetEquipmentSlot(9); }

                            if (Input.GetKeyDown(KeyCode.Equals))
                            {
                                var equipmentStateSlots = inventory.equipmentStateSlots;
                                if (equipmentStateSlots.Length > 0)
                                {
                                    for (int i = 0; i <= inventoryCount; i++)
                                    {
                                        var eqpName = "None";
                                        var charges = -6;
                                        var cooldown = -7;
                                        if (i < equipmentStateSlots.Length) //prevents out of bounds error from unset slots
                                        {
                                            var eqp = equipmentStateSlots[i];
                                            if (eqp.equipmentIndex != EquipmentIndex.None)
                                            {
                                                eqpName = Language.GetString(eqp.equipmentDef.nameToken);
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
            private void SetEquipmentSlot(byte i)
            {
                if (i > selectableSlot)
                {
                    return;
                }
                Chat.AddMessage("Backpack: Set equipment slot to " + i);
                inventory.SetActiveEquipmentSlot(i);
            }
            private void CycleSlot(bool cycleRight)
            {
                var currentSlot = inventory.activeEquipmentSlot + (cycleRight ? 1 : -1);
                var newValue = (byte)LoopAround(currentSlot, 0, selectableSlot);
                inventory.SetActiveEquipmentSlot(newValue);
                Chat.AddMessage("Backpack: Set equipment slot to " + newValue);
            }
        }
    }
}
