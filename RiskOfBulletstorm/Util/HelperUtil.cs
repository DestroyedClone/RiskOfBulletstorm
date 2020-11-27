using RoR2;
using UnityEngine;

namespace RiskOfBulletstorm.Shared
{
    public static class HelperUtil
    {
        public static int GetPlayersItemCount(ItemIndex itemIndex)
        {
            var instances = PlayerCharacterMasterController.instances;
            int InventoryCount = 0;
            foreach (PlayerCharacterMasterController playerCharacterMaster in instances)
            {
                var master = playerCharacterMaster.master;
                var body = master.GetBody();
                if (body) InventoryCount += body.inventory.GetItemCount(itemIndex);
            }
            return InventoryCount;
        }
        public static GameObject GetFirstPlayerWithItem(ItemIndex itemIndex)
        {
            int InventoryCount = 0;
            GameObject playerObject = null;

            for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count; i++)
            { //CharacterMaster.readOnlyInstancesList[i] is the player. 
                var player = CharacterMaster.readOnlyInstancesList[i];
                InventoryCount += player.inventory.GetItemCount(itemIndex);
                if (InventoryCount > 0)
                {
                    playerObject = player.gameObject;
                    break;
                }
            }
            return playerObject;
        }
        public static void GiveItemIfLess(CharacterBody self, ItemIndex itemindex, int amount = 1, int max = 1)
        {
            var InventoryCount = self.inventory.GetItemCount(itemindex);
            if (InventoryCount < max)
            {
                if (InventoryCount + amount > max) amount = max - InventoryCount;

                self.inventory.GiveItem(itemindex, amount);
            }
        }
    }
}
