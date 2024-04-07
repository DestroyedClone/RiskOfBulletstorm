using RoR2;
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
            var sortedList = Main.Artifacts
                .Where(equip => equip.ArtifactDef.nameToken.StartsWith("RISKOFBULLETSTORM_"))
                .ToList();
            sb.AppendLine("## Artifacts");
            sb.AppendLine("|Icon| Artifact | Desc |");
            sb.AppendLine("|:--:|:--:|--|");
            foreach (var artifactBase in sortedList)
            {
                var artifact = artifactBase.ArtifactDef;
                string descriptionToken = ReplaceStyleTagsWithBackticks(Language.GetString(artifact.descriptionToken));

                sb.AppendLine(string.Format("| ![](https://github.com/DestroyedClone/RiskOfBulletstorm/raw/master/RiskOfBulletstorm_Unity/Assets/Icons/ARTIFACT_{0}_ENABLED.png) | [**{1}**]({2}) | {3}",
                    GetLangTokenFromToken(artifact.nameToken),
                    Language.GetString(artifact.nameToken),
                    artifactBase.WikiLink,
                    descriptionToken));
            }
        }

        public static void CreateHeaderSection()
        {
            sb.AppendLine("# Risk of Bulletstorm");
            sb.AppendLine("");
            sb.AppendLine("| [![github issues/request link](https://raw.githubusercontent.com/DestroyedClone/PoseHelper/master/PoseHelper/github_link.webp)](https://github.com/DestroyedClone/RiskOfBulletstorm/issues) | [![discord invite](https://raw.githubusercontent.com/DestroyedClone/PoseHelper/master/PoseHelper/discord_link.webp)](https://discord.gg/DpHu3qXMHK) |");
            sb.AppendLine("|--|--|");
            sb.AppendLine("");
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
        public static Dictionary<ItemTier, string> itemTierTokens = new Dictionary<ItemTier, string>()
        {
            { ItemTier.Tier1, "Common" },
            { ItemTier.Tier2, "Uncommon" },
            { ItemTier.Tier3, "Legendary" },
            { ItemTier.Boss, "Boss" },
            { ItemTier.VoidTier1, "Void Common" },
            { ItemTier.VoidTier2, "Void Uncommon" },
            { ItemTier.VoidTier3, "Void Legendary" },
            { ItemTier.VoidBoss, "Void Boss" },
        };

        public static void CreateItemSection()
        {
            var sortedList = Main.Items
                .Where(item => item.ItemDef.nameToken.StartsWith("RISKOFBULLETSTORM_"))
                .Where(item => desiredTierArrangement.Contains(item.ItemDef.tier))
                .OrderBy(item => Array.IndexOf(desiredTierArrangement, item.ItemDef.tier))
                .ToList();
            sb.AppendLine("## Items");
            sb.AppendLine("|Icon| Item | Pickup |");
            sb.AppendLine("|:--:|:--:|--|");
            ItemTier previousItemTier = ItemTier.NoTier;
            foreach (var itemBase in sortedList)
            {
                var item = itemBase.ItemDef;
                if (previousItemTier != itemBase.Tier)
                {
                    previousItemTier = itemBase.Tier;
                    itemTierTokens.TryGetValue(itemBase.Tier, out string tierName);
                    sb.AppendLine($"| {tierName} |");
                }

                string pickupToken = ReplaceStyleTagsWithBackticks(Language.GetString(item.pickupToken));
                string descriptionToken = ReplaceStyleTagsWithBackticks(Language.GetString(item.descriptionToken));

                sb.AppendLine(string.Format("| ![](https://github.com/DestroyedClone/RiskOfBulletstorm/raw/master/RiskOfBulletstorm_Unity/Assets/Icons/ITEM_{0}.png) | [**{1}**]({2}) | {3}",
                    GetLangTokenFromToken(item.nameToken),
                    Language.GetString(item.nameToken),
                    itemBase.WikiLink,
                    pickupToken));
            }
        }

        public static bool[] desiredEquipmentArrangement = new bool[] { false, true };
        public static void CreateEquipmentSection()
        {
            var sortedList = Main.Equipments
                .Where(equip => equip.EquipmentDef.nameToken.StartsWith("RISKOFBULLETSTORM_"))
                .Where(equip => equip.EquipmentDef.canDrop)
                .OrderBy(equip => Array.IndexOf(desiredEquipmentArrangement, equip.IsLunar))
                .ToList();
            sb.AppendLine("## Equipment");
            sb.AppendLine("|Icon| Equipment | Pickup | CD |");
            sb.AppendLine("|:--:|:--:|--|--|");
            var notLunar = false;
            sb.AppendLine($"| Equipment |");
            foreach (var equipBase in sortedList)
            {
                if (notLunar != equipBase.IsLunar)
                {
                    sb.AppendLine($"| Lunar Equipment |");
                    notLunar = equipBase.IsLunar;
                }
                var equip = equipBase.EquipmentDef;

                string pickupToken = ReplaceStyleTagsWithBackticks(Language.GetString(equip.pickupToken));
                string descriptionToken = ReplaceStyleTagsWithBackticks(Language.GetString(equip.descriptionToken));
                sb.AppendLine(string.Format("| ![](https://github.com/DestroyedClone/RiskOfBulletstorm/raw/master/RiskOfBulletstorm_Unity/Assets/Icons/EQUIPMENT_{0}.png) | [**{1}**]({2}) | {3} | {4}",
                    GetLangTokenFromToken(equip.nameToken),
                    Language.GetString(equip.nameToken),
                    equipBase.WikiLink,
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
