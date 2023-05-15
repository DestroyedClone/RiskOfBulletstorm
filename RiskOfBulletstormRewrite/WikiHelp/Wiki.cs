﻿using BepInEx;
using HarmonyLib;
using R2API;
using R2API.Utils;
using RiskOfBulletstormRewrite.Artifact;
using RiskOfBulletstormRewrite.Controllers;
using RiskOfBulletstormRewrite.Equipment;
using RiskOfBulletstormRewrite.Equipment.EliteEquipment;
using RiskOfBulletstormRewrite.Items;
using RiskOfBulletstormRewrite.Modules;
using RiskOfBulletstormRewrite.WikiHelp;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using Path = System.IO.Path;

namespace RiskOfBulletstormRewrite.WikiHelp
{
    public class Wiki
    {
        public static bool isEnabled = false;

        public static void Initialize()
        {
            //On.RoR2.UI.MainMenu.MainMenuController.Start += MainMenuController_Start;
            //On.RoR2.UI.MainMenu.MainMenuController.Start += OutputEquipmentForWiki;
        }

        private void OutputEquipmentForWiki(On.RoR2.UI.MainMenu.MainMenuController.orig_Start orig, RoR2.UI.MainMenu.MainMenuController self)
        {
            orig(self);
            foreach (var equipment in EquipmentCatalog.equipmentDefs)
            {
                if (equipment
                    && equipment.name.StartsWith("RISKOFBULLETSTORM_"))
                {
                    var isLunar = equipment.isLunar;

                    string pattern = @"(?<=_)\w+(?=_)";
                    string desiredText = Regex.Match(equipment.nameToken, pattern).Value;

                    string pattern2 = "<style=.*?>(.*?)</style>";
                    string replacement = "`$1` ";
                    string pickupResult = Regex.Replace(Language.GetString(equipment.pickupToken), pattern2, replacement);
                    string descriptionResult = Regex.Replace(Language.GetString(equipment.descriptionToken), pattern2, replacement);

                    var newString = string.Format(equiptempstring,
                        new string[]
                        {
                            Language.GetString(equipment.nameToken),
                            desiredText,
                                pickupResult.Replace("\n", "<br>").ToString(),
                                descriptionResult.Replace("\n", "<br>").ToString(),
                                isLunar.ToString(),
                                equipment.cooldown.ToString()
                        });

                    UnityEngine.Debug.Log("\n# " + newString);
                }
            }
        }

        private void MainMenuController_Start(On.RoR2.UI.MainMenu.MainMenuController.orig_Start orig, RoR2.UI.MainMenu.MainMenuController self)
        {
            orig(self);

            foreach (var item in ItemCatalog.allItemDefs)
            {
                if (item
                && item.name.StartsWith("RISKOFBULLETSTORM_ITEM_"))
                {
                    var category = "";
                    if (item.tags.Length > 0)
                    {
                        if (item.tags.Length == 1)
                        {
                            category = item.tags[0].ToString();
                        }
                        else
                        {
                            for (int i = 0; i < item.tags.Length; i++)
                            {
                                ItemTag tag = item.tags[i];
                                category += tag.ToString();
                                if (i < item.tags.Length)
                                {
                                    category += ", ";
                                }
                            }
                        }
                    }

                    string pattern = "<style=.*?>(.*?)</style>";
                    string replacement = "`$1` ";
                    string pickupResult = Regex.Replace(Language.GetString(item.pickupToken), pattern, replacement);
                    string descriptionResult = Regex.Replace(Language.GetString(item.descriptionToken), pattern, replacement);

                    var newString = string.Format(tempstring,
                        new string[]
                        {
                                Language.GetString(item.nameToken.ToString()),
                                pickupResult.Replace("\n", "<br>").ToString(),
                                descriptionResult.Replace("\n", "<br>").ToString(),
                                Language.GetString(item.tier.ToString()),
                                category
                        });
                    UnityEngine.Debug.Log("\n# " + newString);
                }
            }
        }

        private readonly string tempstring = "{0}\r\n|  | {0} |  |\r\n|--|:--:|--|\r\n| The **{0}** is an [item](https://riskofrain2.fandom.com/wiki/Items \"Items\").<br><br>PUT EFFECT HERE<img src=\"https://i.imgur.com/G5NGtxV.png\" title=\"Antibody\" width=\"1000\" height=\"1\" align=\"center\"/> | <img src=\"image\" width=\"50%\" align=\"center\"/></img><br><small>{1}<br></small>{2}<p align=\"left\">Rarity: {3}</p></br><p align=\"left\">Category: {4}</p> |\r\n";
        private readonly string equiptempstring = "# {0}\r\n|  | {0} |  |\r\n|--|:--:|--|\r\n| The **{0}** is an equipment.<br><img src=\"https://i.imgur.com/G5NGtxV.png\" width=\"1000\"/>| <img src=\"https://raw.githubusercontent.com/DestroyedClone/RiskOfBulletstorm/master/RiskOfBulletstorm_Unity/Assets/Icons/EQUIPMENT_{1}.png\" width=\"50%\" align=\"center\"/></img><br><small>{2}<br></small>{3}<p align=\"left\">Lunar: {4}<br>Cooldown: {5}</p>  |";

    }
}
