﻿using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;
using RiskOfBulletstormRewrite;
using BepInEx;

namespace RiskOfBulletstormRewrite.MarkdownCreation
{
    public class ReadmeCreator
    {
        public static bool isEnabled = true;
        public static bool showWarning = true;

        static StringBuilder sb;
        public string readmeString =
            "";
        public static void Initialization()
        {
            On.RoR2.UI.MainMenu.MainMenuController.Start += MainMenuController_Start;
        }

        private static void MainMenuController_Start(On.RoR2.UI.MainMenu.MainMenuController.orig_Start orig, RoR2.UI.MainMenu.MainMenuController self)
        {
            orig(self);
            sb = HG.StringBuilderPool.RentStringBuilder();
            CreateHeaderSection();
            CreateItemSection();
            CreateEquipmentSection();
            CreateArtifactSection();
            Debug.Log(sb.ToString());
        }

        private static void CreateArtifactSection()
        {
            var sortedList = ArtifactCatalog.artifactDefs
                .Where(equip => equip.nameToken.StartsWith("RISKOFBULLETSTORM_"))
                .ToList();
            sb.AppendLine("## Artifacts");
            sb.AppendLine("|Icon| Artifact | Desc |");
            sb.AppendLine("|:--:|:--:|--|");
            foreach (var artifact in sortedList)
            {
                string descriptionToken = ReplaceStyleTagsWithBackticks(Language.GetString(artifact.descriptionToken));
                sb.AppendLine(string.Format("| ![](https://github.com/DestroyedClone/RiskOfBulletstorm/raw/master/RiskOfBulletstorm_Unity/Assets/Icons/ARTIFACT_{0}_ENABLED.png)<br>{1}</br> | {1} | {2}",
                    GetLangTokenFromToken(artifact.nameToken),
                    Language.GetString(artifact.nameToken),
                    descriptionToken));
            }
        }

        public static void CreateHeaderSection()
        {
            sb.Append("# Risk of Bulletstorm");
            sb.AppendLine("");
            sb.AppendLine("| [![github issues/request link](https://raw.githubusercontent.com/DestroyedClone/PoseHelper/master/PoseHelper/github_link.webp)](https://github.com/DestroyedClone/RiskOfBulletstorm/issues) | [![discord invite](https://raw.githubusercontent.com/DestroyedClone/PoseHelper/master/PoseHelper/discord_link.webp)](https://discord.gg/DpHu3qXMHK) |");
            sb.AppendLine("|--|--|");
            sb.AppendLine("![Risk of Bulletstorm logo](https://raw.githubusercontent.com/DestroyedClone/RiskOfBulletstorm/master/RiskOfBulletstormRewrite/Readme/Banner.png)");
            sb.AppendLine("");
            sb.AppendLine("**Risk of Bulletstorm** <sup>(**Risk** of a Rain**storm** of **Bullets**, not Bulletstorm the game!)</sup>, is a content mod that adds items, equipment, and mechanics from the [**Enter The Gungeon**](https://www.dodgeroll.com/gungeon/) property by Dodge Roll Games.");
            sb.AppendLine("");
            if (showWarning)
            {
                sb.AppendLine("## Warning");
                sb.AppendLine("* Mod's not done so there's missing models/textures/effects/item displays");
                sb.AppendLine("* Not tested or setup for multiplayer");
                sb.AppendLine("* Not tested for any mod incompats");
            }
            sb.AppendLine("");
            var showWiki = true;
            if (showWiki)
            {
                sb.AppendLine("## Wiki");
                sb.AppendLine("There is a rudimentary wiki for this with additional information.");
                sb.AppendLine("![wiki indicator](https://raw.githubusercontent.com/DestroyedClone/RiskOfBulletstorm/master/RiskOfBulletstormRewrite/Readme/wikihint.png)");
                sb.AppendLine("");
            }
        }

        public static ItemTier[] desiredTierArrangement = new ItemTier[]
        {
            ItemTier.Tier1,
            ItemTier.Tier2,
            ItemTier.Tier3,
            ItemTier.Boss,
            ItemTier.VoidTier1,
            ItemTier.VoidTier2,
            ItemTier.VoidTier3,
            ItemTier.VoidBoss
        };

        public static void CreateItemSection()
        {
            var sortedList = ItemCatalog.allItemDefs
                .Where(item => item.nameToken.StartsWith("RISKOFBULLETSTORM_"))
                .Where(item => desiredTierArrangement.Contains(item.tier))
                .OrderBy(item => Array.IndexOf(desiredTierArrangement, item.tier))
                .ToList();
            sb.AppendLine("## Items");
            sb.AppendLine("|Icon| Item | Pickup |");
            sb.AppendLine("|:--:|:--:|--|");
            foreach (var item in sortedList)
            {
                string pickupToken = ReplaceStyleTagsWithBackticks(Language.GetString(item.pickupToken));
                string descriptionToken = ReplaceStyleTagsWithBackticks(Language.GetString(item.descriptionToken));

                sb.AppendLine(string.Format("| ![](https://github.com/DestroyedClone/RiskOfBulletstorm/raw/master/RiskOfBulletstorm_Unity/Assets/Icons/ITEM_{0}.png)<br>{1}</br> | {1} | {2}",
                    GetLangTokenFromToken(item.nameToken),
                    Language.GetString(item.nameToken),
                    pickupToken));
            }
        }

        public static void CreateEquipmentSection()
        {
            var sortedList = EquipmentCatalog.equipmentDefs
                .Where(equip => equip.nameToken.StartsWith("RISKOFBULLETSTORM_"))
                .Where(equip => equip.canDrop)
                .ToList();
            sb.AppendLine("## Equipment");
            sb.AppendLine("|Icon| Equipment | Pickup | CD |");
            sb.AppendLine("|:--:|:--:|--|--|");
            foreach (var equip in sortedList)
            {
                string pickupToken = ReplaceStyleTagsWithBackticks(Language.GetString(equip.pickupToken));
                string descriptionToken = ReplaceStyleTagsWithBackticks(Language.GetString(equip.descriptionToken));
                sb.AppendLine(string.Format("| ![](https://github.com/DestroyedClone/RiskOfBulletstorm/raw/master/RiskOfBulletstorm_Unity/Assets/Icons/EQUIPMENT_{0}.png)<br>{1}</br> | {1} | {2} | {3}",
                    GetLangTokenFromToken(equip.nameToken),
                    Language.GetString(equip.nameToken),
                    pickupToken,
                    equip.cooldown));
            }
        }

        public static string ReplaceStyleTagsWithBackticks(string input)
        {
            string pattern = @"<style=[^>]+>(.*?)<\/style>";
            string replacement = "`$1`";

            string output = Regex.Replace(input, pattern, replacement);
            output = output.Replace("\n","<br>");

            return output;
        }
        public static string GetLangTokenFromToken(string input)
        {
            foreach (var text in new string[] { "_NAME", "RISKOFBULLETSTORM_",
            "ITEM_", "EQUIPMENT_", "ARTIFACT_"} )
            {
                input = input.Replace(text, "");
            }
            return input;
        }
    }
}
