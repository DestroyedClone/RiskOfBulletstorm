using System;
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
    }
}
