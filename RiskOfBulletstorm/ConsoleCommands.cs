using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using static R2API.Utils.CommandHelper;
using R2API.Utils;
using RoR2;

namespace RiskOfBulletstorm
{
    public static class ConsoleCommands
    {
        [ConCommand(commandName = "set_equipment_slot", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets equipment slot to specified index")]
        private static void SetEquipmentSlot(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var inventory = localMaster.inventory;
            if (inventory)
            {

            }
        }

        [ConCommand(commandName = "list_player_equipment", flags = ConVarFlags.None, helpText = "Prints the equipment equipped by a player. args[0]=(int)value")]
        private static void ListPlayerEquipment(ConCommandArgs args)
        {
            //Actual code here 
        }
    }
}
