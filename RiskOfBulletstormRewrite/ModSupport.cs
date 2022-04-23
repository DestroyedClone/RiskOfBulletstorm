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
using RoR2;

namespace RiskOfBulletstormRewrite
{
    internal class ModSupport
    {
        internal static bool betterUILoaded = false;
        internal static void CheckForModSupport()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.xoxfaby.BetterUI"))
            {
                betterUILoaded = true;
                BetterUICompat_StatsDisplay();
            }
        }

        // Called by Buffs.cs
        internal static void BetterUICompat_Buffs()
        {
            var prefix = "RISKOFBULLETSTORM_BUFF_";
            void RegisterBuffInfo(RoR2.BuffDef buffDef, string nameToken = null, string descToken = null)
            {
                BetterUI.Buffs.RegisterBuffInfo(buffDef, prefix+nameToken, prefix+descToken);
            }
            RegisterBuffInfo(Buffs.MustacheBuff, "MUSTACHE_NAME", "MUSTACHE_DESC");
            RegisterBuffInfo(Buffs.MetronomeTrackerBuff, "METRONOME_NAME", "METRONOME_DESC");
            RegisterBuffInfo(Buffs.BloodiedScarfBuff, "BLOODIEDSCARF_NAME", "BLOODIEDSCARF_DESC");
        }

        internal static void BetterUICompat_StatsDisplay()
        {
            BetterUI.StatsDisplay.AddStatsDisplay("$accuracy", (BetterUI.StatsDisplay.DisplayCallback)GetAccuracy);
        }

        private static string GetAccuracy(CharacterBody body)
        {
            string value = null;
            var extraStatsController = body.GetComponent<Controllers.ExtraStatsController.RBSExtraStatsController>();
            if (extraStatsController)
            {
                return $"{extraStatsController.idealizedAccuracyStat * 100f}%";
            }

            return value;
        }
        public static float ScopeDamageStacking(float value, float extraStackValue, int stacks)
        {
            return Mathf.Max(0, (stacks+1) - Scope.expectedMaxStacks) * value;
        }

        //Called in Main, after itemdefs are registed.
        internal static void BetterUICompat_ItemStats()
        {
            var prefix = "RISKOFBULLETSTORM_STAT_";
            // Common
            BetterUI.ItemStats.RegisterStat(Items.Antibody.instance.ItemDef,
                prefix + "ANTIBODY_HEALCHANCE",
                Antibody.cfgChance.Value,
                BetterUI.ItemStats.NoStacking,
                BetterUI.ItemStats.StatFormatter.Chance);
            BetterUI.ItemStats.RegisterStat(Antibody.instance.ItemDef,
                prefix + "ANTIBODY_HEALAMOUNT",
                Antibody.cfgMultiplier.Value,
                Antibody.cfgMultiplierPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent);
            BetterUI.ItemStats.RegisterStat(Mustache.instance.ItemDef,
                prefix + "MUSTACHE_DURATION",
                Mustache.cfgDuration.Value,
                BetterUI.ItemStats.NoStacking,
                BetterUI.ItemStats.StatFormatter.Seconds);
            BetterUI.ItemStats.RegisterStat(Mustache.instance.ItemDef,
                prefix + "MUSTACHE_REGEN",
                Mustache.cfgRegenAmount.Value,
                Mustache.cfgRegenAmountPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Regen,
                BetterUI.ItemStats.ItemTag.Healing);
            BetterUI.ItemStats.RegisterStat(Scope.instance.ItemDef,
                prefix + "SCOPE_SPREADREDUCTION",
                Scope.cfgSpreadReduction.Value,
                Scope.cfgSpreadReductionPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                null);

            BetterUI.ItemStats.RegisterStat(Scope.instance.ItemDef,
                prefix + "SCOPE_DAMAGEMULTIPLIER",
                Scope.cfgDamageMultiplierPerStack.Value,
                0,
                ScopeDamageStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                BetterUI.ItemStats.ItemTag.Damage);

            //Uncommon
            BetterUI.ItemStats.RegisterStat(RingChestFriendship.instance.ItemDef,
                prefix + "RINGCHESTFRIENDSHIP_CREDITMULTIPLIER",
                RingChestFriendship.cfgCreditMultiplier.Value,
                RingChestFriendship.cfgCreditMultiplierPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent);

            //Legendary
            BetterUI.ItemStats.RegisterStat(Backpack.instance.ItemDef,
                prefix + "BACKPACK_SLOTS",
                1,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Charges,
                null);
            //Clone

            // Lunar
            BetterUI.ItemStats.RegisterStat(HipHolster.instance.ItemDef,
                prefix + "HIPHOLSTER_CHANCE",
                HipHolster.cfgFreeStockChance.Value,
                HipHolster.cfgFreeStockChancePerStack.Value,
                BetterUI.ItemStats.HyperbolicStacking,
                BetterUI.ItemStats.StatFormatter.Chance,
                BetterUI.ItemStats.ItemTag.SkillCooldown);

            BetterUI.ItemStats.RegisterStat(RingMiserlyProtection.instance.ItemDef,
                prefix + "RINGMISERLYPROTECTION_HEALTH",
                RingMiserlyProtection.cfgMaxHealthPctAdded.Value,
                RingMiserlyProtection.cfgMaxHealthPctAddedStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                BetterUI.ItemStats.ItemTag.MaxHealth);


        }
    }
}
