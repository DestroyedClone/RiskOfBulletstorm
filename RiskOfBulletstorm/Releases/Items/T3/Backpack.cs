using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static RiskOfBulletstorm.Utils.HelperUtil;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class Backpack : Item<Backpack>
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

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the scrollwheel be used in addition to the cycle buttons?", AutoConfigFlags.None)]
        public static bool Backpack_UseScrollWheel { get; private set; } = true;
        //https://docs.unity3d.com/ScriptReference/KeyCode.html
        public override string displayName => "Backpack";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.EquipmentRelated, ItemTag.AIBlacklist });
        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "<b>Item Capacity Up!</b>\nThe Backpack grants you the use of another Active Item. Useful, but cumbersome.";
        protected override string GetLoreString(string langID = null) => "A backpack (also called rucksack, knapsack, packsack, pack, Haversack, or Bergen) is a bag put on somebody's back. It usually has two straps that go over the shoulders. It is used to carry things in it, and it often has many compartments to carry things. People often use backpacks on camping trips, hikes, or any form of outdoor activity where people need to carry many things. Backpacks are also be used in the military by soldiers. It can be also used in school, or in this case, it also called a bookbag or school bag.\n\nLarge backpacks, used to carry loads over 10 kg (22 lbs), and smaller sports backpacks, usually offload the biggest part of their weight onto padded hip belts. This leaves the shoulder straps mainly for keeping the load in place. This makes it easier to carry heavy loads, because the hips are stronger than the shoulders. It also improves agility and balance, because the load lies closer to the center of mass of the person wearing it.\n\nIn the very distant past, backpacks were to carry hunters' larger catches. If the hunts were even larger, the hunters would cut their prey into pieces and hand out the pieces to other hunters. The hunters would then carry smaller pieces separately. The bag would be made up of animal skin and sewn together by animal intestines. They would then be woven together tightly to make a firm material.\n\nSome backpacks are specialized to carry a particular thing, such as fluids or a laptop computer.";
        public static KeyCode ModifierKey_KB = KeyCode.None;
        public static KeyCode CycleLeftKey_KB = KeyCode.None;
        public static KeyCode CycleRightKey_KB = KeyCode.None;

        public KeyCode CycleLeftKey_GP = KeyCode.None;
        public KeyCode CycleRightKey_GP = KeyCode.None;

        public static GameObject ItemBodyModelPrefab;
        public static int ToolbotBodyIndex;

        public Backpack()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/Backpack.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/Backpack.png");
        }

        public override void SetupConfig()
        {
            base.SetupConfig();
            ModifierKey_KB = Backpack_ModifierButton;
                CycleLeftKey_KB = Backpack_CycleLeftButton;
                CycleRightKey_KB = Backpack_CycleRightButton;
            
        }
        protected override string GetDescString(string langid = null)
        {
            var desc = $"Grants access to an <style=cIsUtility>extra equipment slot</style> <style=cStack>(+1 per stack)</style>" +
            $"\nHold {ModifierKey_KB} and press 1-0 to switch equipment slots." +
            $"\nPress {CycleLeftKey_KB} or {CycleRightKey_KB} to cycle to the previous/next slot.";
            desc += Backpack_UseScrollWheel ? $" Additionally, the scrollwheel can be used." : $"";
            return desc;
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();
            if (Compat_ItemStats.enabled)
            {
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return count; },
                    (value, inv, master) => { return $"Additional Backpack Slots: {value}"; }
                ));
            }
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

        public override void SetupLate()
        {
            base.SetupLate();
            ToolbotBodyIndex = (int)SurvivorCatalog.GetBodyIndexFromSurvivorIndex(SurvivorCatalog.FindSurvivorIndex("Toolbot"));
            if (ToolbotBodyIndex < 0)
            {
                Debug.Log("SEARCHME: Failed finding toolbot");
            }
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
                backpackComponent.UpdateToolbot(self);
                backpackComponent.Subscribe();
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
            readonly string equipmentPrefix = "Selected slot ";
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
                isToolbot = (byte)((int)characterBody.bodyIndex == ToolbotBodyIndex ? 1 : 0);
            }
            public void UpdateCount()
            {
                inventoryCount = (byte)inventory.GetItemCount(instance.catalogIndex);
                selectableSlot = (byte)(inventoryCount + isToolbot);
            }
            public void CharacterBody_onInventoryChanged()
            {
                UpdateCount();
                if (inventory.activeEquipmentSlot > selectableSlot)
                    inventory.SetActiveEquipmentSlot(selectableSlot);
            }
            public void Update()
            {
                if (!localUser.isUIFocused) //TY KingEnderBrine for the help
                {
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

                        if (Backpack_UseScrollWheel)
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
                Chat.AddMessage(equipmentPrefix + i);
                inventory.SetActiveEquipmentSlot(i);
            }
            private void CycleSlot(bool cycleRight)
            {
                var currentSlot = inventory.activeEquipmentSlot + (cycleRight ? 1 : -1);
                var newValue = (byte)LoopAround(currentSlot, 0, selectableSlot);
                inventory.SetActiveEquipmentSlot(newValue);
                Chat.AddMessage(equipmentPrefix + newValue);
            }
        }
    }
}
