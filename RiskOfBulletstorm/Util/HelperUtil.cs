using RoR2;
using UnityEngine;

namespace RiskOfBulletstorm.Shared
{
    public static class HelperUtil
    {
        public static int GetPlayerCount()
        {
            return PlayerCharacterMasterController.instances.Count;
        }

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
    public static class CurseUtil
    {
        public static string GetCleanseShrineText(float CurseCount)
        {
            string[] cleanseShrineText =
            {
            "The spectres don't disturb you.",
            "You are touched by darkness.",
            "You are wreathed in darkness.",
            "Tarry not. They come for you.",
            "No one can help you."
            };
            switch (CurseCount)
            {
                case 0:
                case 1:
                case 2:
                    return cleanseShrineText[0];
                case 3:
                case 4:
                    return cleanseShrineText[1];
                case 5:
                case 6:
                    return cleanseShrineText[2];
                case 7:
                case 8:
                case 9:
                    return cleanseShrineText[3];
                default:
                    return cleanseShrineText[4];
            }
        }

        public static float GetCurseCount(CharacterBody characterBody = null)
        {
            var curseIndex = Items.Curse.instance.catalogIndex;
            if (characterBody)
                return HelperUtil.GetPlayersItemCount(curseIndex);
            return characterBody.inventory.GetItemCount(curseIndex);
        }
    }
    public static class SpawnUtil
    {
        public static float GetMimicSpawnChance()
        {
            var mimicChance = 2.25f;
            var curseCount = CurseUtil.GetCurseCount();
            //var mimicToothNecklaceCount = HelperUtil.GetPlayersItemCount(mimicnecklace.instance.catalogIndex);
            //var ringMimicFriendshipCount = HelperUtil.GetPlayersItemCount(mimicnecklace.instance.catalogIndex);

            mimicChance += curseCount * 2.1f;
            return Mathf.Clamp(mimicChance,0,100);
        }
    }
}
