using RoR2;
using UnityEngine;

namespace RiskOfBulletstorm.Shared
{
    public static class HelperUtil
    {
        public static int GetPlayersItemCount(ItemIndex itemIndex)
        {
            int InventoryCount = 0;
            for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count; i++)
            { //CharacterMaster.readOnlyInstancesList[i] is the player. 
                InventoryCount += CharacterMaster.readOnlyInstancesList[i].inventory.GetItemCount(itemIndex);
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
    }
    /*private void GiveItemVsMax(CharacterBody self, ItemIndex itemindex, int amount = 1, int max = 1)
    {
        var InventoryCount = GetCount(self);
        if (InventoryCount < max)
        {
            if (InventoryCount + amount > max)
            {
                self.inventory.GiveItem(itemindex, (max - InventoryCount));
            }
            else
            {
                self.inventory.GiveItem(itemindex, amount);
            }
        }
    }*/
}
