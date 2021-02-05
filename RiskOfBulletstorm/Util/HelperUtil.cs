using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using RoR2.UI;
using UnityEngine;
using RiskOfBulletstorm;
using static RiskOfBulletstorm.Shared.Buffs.BuffsController;
using static RiskOfBulletstorm.BulletstormPlugin;
using static RiskOfBulletstorm.Items.CurseController;

namespace RiskOfBulletstorm.Utils
{
    public static class HelperUtil
    {
        public static void ClearBuffStacks(CharacterBody characterBody, BuffIndex buffIndex, int stacks = -1)
        {
            var buffcount = characterBody.GetBuffCount(buffIndex);
            int iterate = buffcount;
            if (stacks > 0) iterate = stacks;

            for (int i = 0; i < iterate; i++)
            {
                characterBody.RemoveBuff(buffIndex);
            }
        }
        public static void AddBuffStacks(CharacterBody characterBody, BuffIndex buffIndex, int stacks = 1)
        {
            for (int i = 0; i < stacks; i++)
                characterBody.AddBuff(buffIndex);
        }
        public static int GetPlayerCount()
        {
            return PlayerCharacterMasterController.instances.Count;
        }

        public static int GetUniqueItemCount(CharacterBody characterBody)
        {
            int num = 0;
            ItemIndex itemIndex = ItemIndex.Syringe;
            ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
            while (itemIndex < itemCount)
            {
                if (characterBody.inventory.GetItemCount(itemIndex) > 0)
                    num++;
                itemIndex++;
            }
            return num;
        }

        public static CharacterBody GetPlayerWithMostItemIndex(ItemIndex itemIndex)
        {
            var instances = PlayerCharacterMasterController.instances;
            var largestStack = 0;
            CharacterBody chosenBody = null;
            foreach (PlayerCharacterMasterController playerCharacterMaster in instances)
            {
                var master = playerCharacterMaster.master;
                var body = master.GetBody();
                if (body)
                {
                    var invcount = body.inventory.GetItemCount(itemIndex);
                    if (invcount > largestStack)
                    {
                        largestStack = invcount;
                        chosenBody = body;
                    }
                }
            }
            return chosenBody;
        }

        public static bool HasItem(CharacterBody characterBody, ItemIndex itemIndex)
        {
            if (characterBody == null || characterBody.inventory == null || itemIndex == ItemIndex.None)
                return false;
            if (characterBody.inventory.GetItemCount(itemIndex) > 0)
                return true;
            else return false;
        }

        public static void GiveItemToPlayers(ItemIndex itemIndex, bool showInChat = true, int amount = 1, int max = 1)
        {
            var instances = PlayerCharacterMasterController.instances;
            foreach (PlayerCharacterMasterController playerCharacterMaster in instances)
            {
                var master = playerCharacterMaster.master;
                if (master)
                {
                    var body = playerCharacterMaster.body;
                    if (body)
                    {
                        var inventory = master.inventory;

                        if (inventory)
                        {
                            GiveItemIfLess(master, itemIndex, showInChat, body, amount, max);
                        }
                    }
                }
            }
        }


        public static void GiveItemToPlayers(ItemIndex itemIndex, bool showInChat = true, int amount = 1)
        {
            var instances = PlayerCharacterMasterController.instances;
            foreach (PlayerCharacterMasterController playerCharacterMaster in instances)
            {
                var master = playerCharacterMaster.master;
                if (master)
                {
                    var body = playerCharacterMaster.body;
                    if (body)
                    {
                        var inventory = master.inventory;

                        if (inventory)
                        {
                            if (showInChat) SimulatePickup(master, itemIndex, amount);
                            else inventory.GiveItem(itemIndex, amount);
                        }
                    }
                }
            }
        }
        public static float GetPlayersLuck()
        {
            var instances = PlayerCharacterMasterController.instances;
            float luck = 0f;
            foreach (PlayerCharacterMasterController playerCharacterMaster in instances)
            {
                luck += playerCharacterMaster.master ? playerCharacterMaster.master.luck : 0f;
            }
            return luck;
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used when showInChat is not false")]
        public static int GiveItemIfLess(CharacterMaster characterMaster, ItemIndex itemIndex, bool showInChat = true, CharacterBody characterBody = null, int amount = 1, int max = 1)
        {
            var self = characterMaster.inventory;
            var InventoryCount = self.GetItemCount(itemIndex);
            //var pickupindex = PickupCatalog.FindPickupIndex(itemIndex);
            //var pickupDef = PickupCatalog.GetPickupDef(pickupindex);
            if (InventoryCount < max)
            {
                if (InventoryCount + amount > max) amount = max - InventoryCount;

                if (!showInChat)
                {
                    _logger.LogDebug("HelperUtil: GiveItemIfLess: ShowInChat was false, so we gave the item.");
                    self.GiveItem(itemIndex, amount);
                }
                else
                {
                    _logger.LogDebug("HelperUtil: GiveItemIfLess: ShowInChat was true, so we called SimulatePickup()");
                    SimulatePickup(characterMaster, itemIndex, amount);
                }
            }
            return amount;
        }

        public static void SimulatePickup(CharacterMaster characterMaster, ItemIndex itemIndex, int amount = 1)
        {
            var self = characterMaster.inventory;
            //var list = NotificationQueue.instancesList;
            var pickupIndex = PickupCatalog.FindPickupIndex(itemIndex);
            //var pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            //var nameToken = pickupDef.nameToken;
            //var color = pickupDef.baseColor;
            //var body = characterMaster.GetBody();

            GenericPickupController.SendPickupMessage(characterMaster, pickupIndex);

            //Chat.AddPickupMessage(body, nameToken, color, (uint)amount);
            Util.PlaySound("Play_UI_item_pickup", characterMaster.GetBodyObject());

            self.GiveItem(itemIndex, amount);
            /*
            for (int i = 0; i < list.Count; i++)
            {
                list[i].OnPickup(characterMaster, pickupIndex);
            }*/

        }

        public static float LoopAround(float value, float min, float max)
        {
            if (value < min) value = max;
            else if (value > max) value = min;
            return value;
        }


        public static string NumbertoOrdinal(int number)
        {
            string words = NumberToWords(number);
            words = words.TrimEnd('\\');
            if (words.EndsWith("one"))
            {
                words = words.Remove(words.LastIndexOf("one") + 0).Trim();
                words += "first";
            }
            else if (words.EndsWith("two"))
            {
                words = words.Remove(words.LastIndexOf("two") + 0).Trim();
                words += "second";
            }
            else if (words.EndsWith("three"))
            {
                words = words.Remove(words.LastIndexOf("three") + 0).Trim();
                words += "third";
            }
            else if (words.EndsWith("five"))
            {
                words = words.Remove(words.LastIndexOf("five") + 0).Trim();
                words += "fifth";
            }
            else if (words.EndsWith("eight"))
            {
                words = words.Remove(words.LastIndexOf("eight") + 0).Trim();
                words += "eighth";
            }
            else if (words.EndsWith("nine"))
            {
                words = words.Remove(words.LastIndexOf("nine") + 0).Trim();
                words += "ninth";
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
        /*
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
            var curseIndex = Items.CurseController.curseTally;
            if (!characterBody)
                return HelperUtil.GetPlayersItemCount(curseIndex);
            return characterBody.inventory.GetItemCount(curseIndex);
        }
        */
        public static void JamEnemy(CharacterBody body, float curseChance = 100f)
        {
            var curseEffect = Resources.Load<GameObject>("prefabs/effects/ImpSwipeEffect");
            if (Util.CheckRoll(curseChance))
            {
                EffectManager.SpawnEffect(curseEffect, new EffectData
                {
                    origin = body.transform.position + Vector3.up * 2,
                    //rotation = Util.QuaternionSafeLookRotation(Vector3.up),
                    scale = 2f
                }, false);
                //body.AddBuff(Items.GungeonBuffController.Jammed);
                IsJammed jammed = body.gameObject.GetComponent<IsJammed>();
                if (!jammed) jammed = body.gameObject.AddComponent<IsJammed>();
                jammed.characterBody = body;
            }
        }

        public static bool CheckJammedStatus(CharacterBody body)
        {
            if (body.inventory)
            {
                return body.inventory.GetItemCount(isJammedItem) > 0;
            }
            return body.GetComponent<IsJammed>();
        }
    }
    /*
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
    }*/ //Currently unimplemented
}
