using RoR2;
using UnityEngine;
using RiskOfBulletstorm;

namespace RiskOfBulletstorm.Utils
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

        public static string NumbertoOrdinal(int number)
        {
            string words = NumberToWords(number);
            words = words.TrimEnd('\\');
            if (words.EndsWith("One"))
            {
                words = words.Remove(words.LastIndexOf("One") + 0).Trim();
                words += "First";
            }
            else if (words.EndsWith("Two"))
            {
                words = words.Remove(words.LastIndexOf("Two") + 0).Trim();
                words += "Second";
            }
            else if (words.EndsWith("Three"))
            {
                words = words.Remove(words.LastIndexOf("Three") + 0).Trim();
                words += "Third";
            }
            else if (words.EndsWith("Five"))
            {
                words = words.Remove(words.LastIndexOf("Five") + 0).Trim();
                words += "Fifth";
            }
            else if (words.EndsWith("Eight"))
            {
                words = words.Remove(words.LastIndexOf("Eight") + 0).Trim();
                words += "Eighth";
            }
            else if (words.EndsWith("Nine"))
            {
                words = words.Remove(words.LastIndexOf("Nine") + 0).Trim();
                words += "Ninth";
            }
            else
            {
                words += "th";
            }
            return words;
        } //https://stackoverflow.com/questions/48966570/integer-to-long-ordinal-string-in-c-net

        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Mathf.Abs(number));

            string words = "";
            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        } //https://stackoverflow.com/questions/2729752/converting-numbers-in-to-words-c-sharp

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

        public static void JamEnemy(CharacterBody body, float curseChance = 100f)
        {
            var curseEffect = (GameObject)Resources.Load("prefabs/effects/DeathMarkAfflictionEffect.prefab");
            if (Util.CheckRoll(curseChance))
            {
                EffectManager.SpawnEffect(curseEffect, new EffectData
                {
                    origin = body.transform.position + Vector3.up * 2,
                    //rotation = Util.QuaternionSafeLookRotation(Vector3.up),
                    scale = 2f
                }, false);
                body.AddBuff(Items.GungeonBuffController.Jammed);
            }
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
