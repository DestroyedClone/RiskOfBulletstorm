﻿using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using static R2API.Utils.CommandHelper;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstorm
{
    public static class ConsoleCommands
    {
        [ConCommand(commandName = "ROB_set_equipment_slot", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets equipment slot to specified index.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        private static void SetEquipmentSlot(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var inventory = localMaster.inventory;
            if (inventory)
            {
                var value = (byte)args.GetArgInt(0);
                inventory.SetActiveEquipmentSlot(value);
            }
        }

        [ConCommand(commandName = "ROB_list_equipment", flags = ConVarFlags.None, helpText = "Prints the equipment equipped.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Empty Arg required")]
        private static void ListPlayerEquipment(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var inventory = localMaster.inventory;
            if (inventory)
            {
                var equipmentStateSlots = inventory.equipmentStateSlots;
                var length = equipmentStateSlots.Length;
                if (length > 0)
                {
                    for (int i = 0; i <= length; i++)
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
                        Debug.Log(
                            "[" + (i) + "] " +
                            eqpName +
                            (charges == -6 ? "" : " " + charges + "x") +
                            (cooldown == -7 ? "" : " CD:" + cooldown + " ")
                            );
                    }
                }
            }
        }

        [ConCommand(commandName = "ROB_set_equipment_slot", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets equipment slot to specified index.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        private static void GivePingItem(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var inventory = localMaster.inventory;
            if (inventory)
            {
                var value = (byte)args.GetArgInt(0);
                inventory.SetActiveEquipmentSlot(value);
            }
        }


        [ConCommand(commandName = "ROB_target_enable", flags = ConVarFlags.ExecuteOnServer, helpText = "Allow yourself to use the target commands")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void TargetToggle(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var bodyObject = localMaster.GetBodyObject();
            if (bodyObject)
            {
                var component = bodyObject.GetComponent<ROBConsoleCommand>();
                if (!component)
                {
                    bodyObject.AddComponent<ROBConsoleCommand>();
                    Debug.Log("Gave component!");
                    return;
                }
            }
        }

        private static ROBConsoleCommand HasComponent(CharacterMaster characterMaster)
        {
            var bodyObject = characterMaster.GetBodyObject();
            if (bodyObject)
            {
                var component = bodyObject.GetComponent<ROBConsoleCommand>();
                if (!component)
                {
                    Debug.LogError("Player is missing component. Do ROB_target_component, and try again.");
                    return null;
                }
                return component;
            }
            Debug.LogError("Player's body object is missing.");
            return null;
        }

        [ConCommand(commandName = "ROB_target_hit", flags = ConVarFlags.ExecuteOnServer, helpText = "Call the command then attack an enemy to mark them for targeted commands. Run again to stop.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void TargetChoose(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component)
            {
                var state = component.GetState();
                if (state != 1)
                {
                    state = 1;
                    Debug.Log("Attack an enemy to target them. Run the command again to cancel.");
                    On.RoR2.HealthComponent.TakeDamage += TargetEnemyHook;
                    return;
                }
                state = 0;
                On.RoR2.HealthComponent.TakeDamage -= TargetEnemyHook;
                Debug.Log("No longer targeting.");
                return;
            }
        }

        private static void TargetEnemyHook(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component)
            {
                if (damageInfo.attacker.gameObject == localMaster.GetBodyObject())
                {
                    damageInfo.rejected = true;
                    damageInfo.procCoefficient = -1;
                    damageInfo.crit = false;
                    component.SetBody(self.body);
                    Chat.AddMessage("Body assigned to "+ self.body.GetDisplayName());
                }
            }
            orig(self, damageInfo);
        }

        [ConCommand(commandName = "ROB_target_give_item", flags = ConVarFlags.ExecuteOnServer, helpText = "Gives the target item(s). Syntax: [itemindex] [amount]")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        private static void TargetGiveItem(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component && component.HasBody())
            {
                var inventory = component.targetedBody.inventory;
                if (inventory)
                { //https://stackoverflow.com/questions/23563960/how-to-get-enum-value-by-string-or-int
                    ItemIndex itemIndex = (ItemIndex)args.GetArgInt(0);
                    int itemCount = args.GetArgInt(1);

                    inventory.GiveItem(itemIndex, itemCount);
                    Chat.AddMessage("Gave "+itemIndex+" x"+itemCount+" to "+component.targetedBody.GetDisplayName());
                }
            }
        }

        [ConCommand(commandName = "ROB_target_inventory", flags = ConVarFlags.ExecuteOnServer, helpText = "Prints their inventory.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void TargetCheckItems(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component && component.HasBody())
            {
                var inventory = component.targetedBody.inventory;
                if (inventory)
                { //https://stackoverflow.com/questions/23563960/how-to-get-enum-value-by-string-or-int
                    var ChatQueue = component.targetedBody.GetDisplayName() + "'s inventory:\n";
                    foreach (ItemIndex itemIndex in (ItemIndex[])Enum.GetValues(typeof(ItemIndex)))
                    {
                        var itemCount = inventory.GetItemCount(itemIndex);
                        if (itemCount > 0)
                            ChatQueue += itemIndex+" x"+itemCount+"\n";
                    }

                    Debug.Log(ChatQueue);
                }
            }
        }


        public class ROBConsoleCommand : MonoBehaviour
        {
            byte state = 0;
            public CharacterBody targetedBody = null;

            public void SetState(byte value)
            {
                state = value;
            }
            public byte GetState()
            {
                return state;
            }
            public void SetBody(CharacterBody characterBody)
            {
                targetedBody = characterBody;
            }
            public bool HasBody()
            {
                if (targetedBody && targetedBody != null)
                {
                    return true;
                }
                Debug.LogError("Body not found!.");
                return false;
            }
        }
    }
}
