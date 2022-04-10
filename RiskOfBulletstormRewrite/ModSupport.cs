using BepInEx;
using R2API;
using R2API.Utils;
using RiskOfBulletstormRewrite.Artifact;
using RiskOfBulletstormRewrite.Controllers;
using RiskOfBulletstormRewrite.Equipment;
using RiskOfBulletstormRewrite.Equipment.EliteEquipment;
using RiskOfBulletstormRewrite.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Path = System.IO.Path;
using RiskOfBulletstormRewrite.Utils;

namespace RiskOfBulletstormRewrite
{
    internal class ModSupport
    {
        internal static bool betterUILoaded = false;
        internal static bool itemStatsLoaded = false;
        internal static void CheckForModSupport()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.xoxfaby.BetterUI"))
            {
                betterUILoaded = true;
            }
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.ontrigger.itemstats"))
            {
                itemStatsLoaded = true;
            }
        }

        internal static void BetterUICompat()
        {
            var prefix = "RISKOFBULLETSTORM_BUFF_";
            void RegisterBuffInfo(RoR2.BuffDef buffDef, string nameToken = null, string descToken = null)
            {
                BetterUI.Buffs.RegisterBuffInfo(buffDef, prefix+nameToken, prefix+descToken);
            }
            RegisterBuffInfo(Buffs.MustacheBuff, "MUSTACHE_NAME", "MUSTACHE_DESC");
            RegisterBuffInfo(Buffs.MetronomeTrackerBuff, "METRONOME_NAME", "METRONOME_DESC");
        }
    }
}
