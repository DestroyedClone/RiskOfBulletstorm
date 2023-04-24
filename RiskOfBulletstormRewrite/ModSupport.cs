using RiskOfBulletstormRewrite.Artifact;
using RiskOfBulletstormRewrite.Equipment;
using RiskOfBulletstormRewrite.Items;
using RiskOfBulletstormRewrite.Modules;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using System.Runtime.CompilerServices;
using UnityEngine;

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
                BetterUICompat_ItemStats();
                BetterUICompat_Buffs();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void BetterUICompat_RegisterTag()
        {
            BetterUI.ItemStats.RegisterTag(null, new BetterUI.ItemStats.ItemTag()
            {
            });
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void BetterUICompat_Buffs()
        {
            var prefix = "RISKOFBULLETSTORM_BUFF_";
            void RegisterBuffInfo(RoR2.BuffDef buffDef, string baseToken, string[] descTokenParams = null)
            {
                if (descTokenParams != null && descTokenParams.Length > 0)
                {
                    LanguageOverrides.DeferToken(prefix + baseToken + "_DESC", descTokenParams);
                }
                BetterUI.Buffs.RegisterBuffInfo(buffDef, prefix + baseToken + "_NAME", prefix + baseToken + "_DESC");
            }
            RegisterBuffInfo(Buffs.AlphaBulletBuff, "ALPHABULLET", new string[]
            {
                GetFloat(AlphaBullets.cfgDamage),
                GetFloat(AlphaBullets.cfgDamageStack)
            });
            RegisterBuffInfo(Buffs.MustacheBuff, "MUSTACHE", new string[]{
                GetFloat(Mustache.cfgRegenAmount),
                GetFloat(Mustache.cfgRegenAmountPerStack)});
            /*(Buffs.MetronomeTrackerBuff, "METRONOME", new string[]{});*/
            RegisterBuffInfo(Buffs.BloodiedScarfBuff, "BLOODIEDSCARF", new string[]{
                GetFloat(BloodiedScarf.cfgDamageVulnerabilityMultiplier),
                GetFloat(BloodiedScarf.cfgDamageVulnerabilityMultiplierPerStack)
            });
            RegisterBuffInfo(Buffs.BloodiedScarfBuff, "ALPHABULLET", new string[]{
                GetFloat(AlphaBullets.cfgDamage),
                GetFloat(AlphaBullets.cfgDamageStack)});
            RegisterBuffInfo(Buffs.DodgeRollBuff, "DODGEROLL",
            new string[]{
                Utils.ItemHelpers.Pct(DodgeRollUtilityReplacement.cfgDamageVulnerabilityMultiplier.Value),
                Utils.ItemHelpers.Pct(DodgeRollUtilityReplacement.cfgDamageVulnerabilityMultiplierPerStack.Value)
            });
            RegisterBuffInfo(Buffs.ArtifactSpeedUpBuff,
                "ARTIFACTSPEEDUP",
                new string[] { GetFloat(ArtifactUpSpeedOOC.cfgMoveSpeedAdditiveMultiplier) });
        }

        private static string GetFloat(BepInEx.Configuration.ConfigEntry<float> entry)
        {
            return entry.Value.ToString();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void BetterUICompat_StatsDisplay()
        {
            BetterUI.StatsDisplay.AddStatsDisplay("$riskofbulletstorm_accuracy", (BetterUI.StatsDisplay.DisplayCallback)GetAccuracy);
            BetterUI.StatsDisplay.AddStatsDisplay("$riskofbulletstorm_curse", (BetterUI.StatsDisplay.DisplayCallback)GetCurse);
            BetterUI.StatsDisplay.AddStatsDisplay("$riskofbulletstorm_steal", (BetterUI.StatsDisplay.DisplayCallback)GetStealChance);
        }

        private static string GetAccuracy(CharacterBody body)
        {
            string value = "N/A";
            var master = body.master;
            if (master)
            {
                var extraStatsController = master.GetComponent<Controllers.ExtraStatsController.RBSExtraStatsController>();
                if (extraStatsController)
                {
                    return $"{(extraStatsController.idealizedAccuracyStat * 100f):0.##}%";
                }
            }

            return value;
        }

        //this duplicate is kinda goofy, is this unoptimized?
        private static string GetCurse(CharacterBody body)
        {
            string value = "N/A";
            var master = body.master;
            if (master)
            {
                var extraStatsController = master.GetComponent<Controllers.ExtraStatsController.RBSExtraStatsController>();
                if (extraStatsController)
                {
                    return $"{extraStatsController.curse:0.##}";
                }
            }

            return value;
        }

        private static string GetStealChance(CharacterBody body)
        {
            var stolenItemCount = Items.StolenItemTally.instance.GetCount(body);
            var rollchance = (100 / (stolenItemCount + 1)).ToString("0.##");
            string value = rollchance + "%";
            return value;
        }

        public static float ScopeDamageStacking(float value, float extraStackValue, int stacks)
        {
            return Mathf.Max(0, (stacks + 1) - Scope.expectedMaxStacks) * value;
        }

        public static float DodgeRollDurationStacking(float value, float extraStackValue, int stacks)
        {
            return DodgeRollUtilityReplacement.GetDuration(stacks);
        }

        public static float CustomHyperbolicStacking(float value, float extraStackValue, int stacks)
        {
            return Utils.ItemHelpers.GetHyperbolicValue(value, extraStackValue, stacks);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        internal static void BetterUICompat_ItemStats()
        {
            //todo: capformula, registermodifier
            var prefix = "RISKOFBULLETSTORM_STAT_";

            #region Common

            BetterUI.ItemStats.RegisterStat(Items.Antibody.instance.ItemDef,
                prefix + "ANTIBODY_HEALCHANCE",
                Antibody.cfgChance.Value / 100, //divided by 100 because the rollchacne is 25f
                BetterUI.ItemStats.NoStacking,
                BetterUI.ItemStats.StatFormatter.Chance);
            BetterUI.ItemStats.RegisterStat(Antibody.instance.ItemDef,
                prefix + "ANTIBODY_HEALAMOUNT",
                Antibody.cfgMultiplier.Value,
                Antibody.cfgMultiplierPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                BetterUI.ItemStats.ItemTag.Healing);
            /*BetterUI.ItemStats.RegisterStat(IrradiatedLead.instance.ItemDef,
                prefix + "IRRADIATEDLEAD_POISONCHANCE",
                IrradiatedLead.cfgChance.Value,
                IrradiatedLead.cfgChanceStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent);*/
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

            #endregion Common

            #region Uncommon

            BetterUI.ItemStats.RegisterStat(AlphaBullets.instance.ItemDef,
                prefix + "ALPHABULLETS_DAMAGEBONUS",
                AlphaBullets.cfgDamage.Value,
                AlphaBullets.cfgDamageStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                BetterUI.ItemStats.ItemTag.Damage);
            //todo get stat for buff count * damage

            /* BetterUI.ItemStats.RegisterStat(AmmoBelt.instance.ItemDef,
                prefix + "AMMOBELT_EXTRASTOCKS",
                AmmoBelt.cfgPercentageStockAdditive.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent); */

            BetterUI.ItemStats.RegisterStat(BattleStandard.instance.ItemDef,
                prefix + "BATTLESTANDARD_DAMAGEBONUS",
                BattleStandard.cfgDamage.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                BetterUI.ItemStats.ItemTag.Damage);

            /* BetterUI.ItemStats.RegisterStat(GhostBullets.instance.ItemDef,
                prefix + "GHOSTBULLETS_DAMAGEREDUCTION",
                GhostBullets.cfgDamageReduction.Value,
                BetterUI.ItemStats.NoStacking,
                BetterUI.ItemStats.StatFormatter.Percent);
            BetterUI.ItemStats.RegisterStat(GhostBullets.instance.ItemDef,
                prefix + "GHOSTBULLETS_PIERCECOUNT",
                1,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Charges); */

            /* BetterUI.ItemStats.RegisterStat(RingChestFriendship.instance.ItemDef,
                prefix + "RINGCHESTFRIENDSHIP_CREDITMULTIPLIER",
                RingChestFriendship.cfgCreditMultiplier.Value,
                RingChestFriendship.cfgCreditMultiplierPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent); */

            /*BetterUI.ItemStats.RegisterStat(RingFireResistance.instance.ItemDef,
                prefix + "RINGFIRERESISTANCE_REDUCTION",
                RingFireResistance.cfgBaseResist.Value,
                BetterUI.ItemStats.HyperbolicStacking,
                BetterUI.ItemStats.StatFormatter.Percent);*/

            BetterUI.ItemStats.RegisterStat(CoinCrown.instance.ItemDef,
                prefix + "COINCROWN_MONEYMULTADD",
                CoinCrown.cfgCashMultiplier.Value,
                CoinCrown.cfgCashMultiplierPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent);

            #endregion Uncommon

            #region Legendary

            BetterUI.ItemStats.RegisterStat(Backpack.instance.ItemDef,
                prefix + "BACKPACK_SLOTS",
                1,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Charges,
                null);
            //BabyGoodMimic
            /* BetterUI.ItemStats.RegisterStat(BabyGoodMimic.instance.ItemDef,
                 prefix + "BABYGOODMIMIC_MINIONCOUNT",
                 1,
                 BetterUI.ItemStats.LinearStacking,
                 BetterUI.ItemStats.StatFormatter.Charges,
                 null);
             BetterUI.ItemStats.RegisterStat(BabyGoodMimic.instance.ItemDef,
                 prefix + "BABYGOODMIMIC_COOLDOWN",
                 30,
                 BetterUI.ItemStats.NoStacking,
                 BetterUI.ItemStats.StatFormatter.Seconds,
                 null);*/

            /*BetterUI.ItemStats.RegisterStat(Clone.instance.ItemDef,
                prefix + "CLONE_ITEMCOUNT",
                Clone.cfgItemsToKeep.Value,
                Clone.cfgItemsToKeepPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Charges,
                null);*/

            #endregion Legendary

            #region Lunar

            BetterUI.ItemStats.RegisterStat(BloodiedScarf.instance.ItemDef,
                prefix + "BLOODIEDSCARF_DISTANCE",
                BloodiedScarf.cfgTeleportRange.Value,
                BloodiedScarf.cfgTeleportRangePerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Range);
            BetterUI.ItemStats.RegisterStat(BloodiedScarf.instance.ItemDef,
                prefix + "BLOODIEDSCARF_DAMAGEVULNERABILITY",
                BloodiedScarf.cfgDamageVulnerabilityMultiplier.Value,
                BloodiedScarf.cfgDamageVulnerabilityMultiplierPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent);
            BetterUI.ItemStats.RegisterStat(BloodiedScarf.instance.ItemDef,
                prefix + "BLOODIEDSCARF_VULNERABILITYDURATION",
                BloodiedScarf.cfgDamageVulnerabilityDuration.Value,
                BetterUI.ItemStats.NoStacking,
                BetterUI.ItemStats.StatFormatter.Seconds);

            BetterUI.ItemStats.RegisterStat(DodgeRollUtilityReplacement.instance.ItemDef,
                prefix + "DODGEROLLUTILITYREPLACEMENT_DAMAGEVULNERABILITY",
                DodgeRollUtilityReplacement.cfgDamageVulnerabilityMultiplier.Value,
                DodgeRollUtilityReplacement.cfgDamageVulnerabilityMultiplierPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent);
            BetterUI.ItemStats.RegisterStat(DodgeRollUtilityReplacement.instance.ItemDef,
                prefix + "DODGEROLLUTILITYREPLACEMENT_VULNERABILITYDURATION",
                DodgeRollUtilityReplacement.cfgDamageVulnerabilityDuration.Value,
                DodgeRollUtilityReplacement.cfgDamageVulnerabilityDurationDecreasePerStack.Value,
                DodgeRollDurationStacking,
                BetterUI.ItemStats.StatFormatter.Seconds);

            BetterUI.ItemStats.RegisterStat(HipHolster.instance.ItemDef,
                prefix + "HIPHOLSTER_CHANCE",
                HipHolster.cfgFreeStockChance.Value,
                HipHolster.cfgFreeStockChancePerStack.Value,
                CustomHyperbolicStacking,
                BetterUI.ItemStats.StatFormatter.Chance,
                BetterUI.ItemStats.ItemTag.SkillCooldown);

            BetterUI.ItemStats.RegisterStat(RingMiserlyProtection.instance.ItemDef,
                prefix + "RINGMISERLYPROTECTION_HEALTH",
                RingMiserlyProtection.cfgMaxHealthPctAdded.Value,
                RingMiserlyProtection.cfgMaxHealthPctAddedStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                BetterUI.ItemStats.ItemTag.MaxHealth);

            #endregion Lunar

            #region NoTier

            BetterUI.ItemStats.RegisterStat(Items.OrangeConsumed.instance.ItemDef,
                prefix + "ORANGECONSUMED_MAXHEALTHADD",
                Orange.cfgMaxHealthIncrease.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                BetterUI.ItemStats.ItemTag.MaxHealth);
            BetterUI.ItemStats.RegisterStat(Items.OrangeConsumed.instance.ItemDef,
                prefix + "ORANGECONSUMED_SKILLREDUCTION",
                Orange.cfgChargeRateReduction.Value,
                BetterUI.ItemStats.HyperbolicStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                BetterUI.ItemStats.ItemTag.SkillCooldown);
            BetterUI.ItemStats.RegisterStat(CurseTally.instance.ItemDef,
                prefix + "CURSETALLY_CURSE",
                1,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Charges);
            BetterUI.ItemStats.RegisterStat(SpiceTally.instance.ItemDef,
                prefix + "SPICETALLY_DAMAGE",
                Spice2.cfgStatDamage.Value,
                Spice2.cfgStatDamageStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent);
            BetterUI.ItemStats.RegisterStat(SpiceTally.instance.ItemDef,
                prefix + "SPICETALLY_ACCURACY",
                Spice2.cfgStatAccuracy.Value,
                Spice2.cfgStatAccuracyStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent);
            BetterUI.ItemStats.RegisterStat(SpiceTally.instance.ItemDef,
                prefix + "SPICETALLY_ROR2CURSE",
                Spice2.cfgStatRORCurse.Value,
                Spice2.cfgStatRORCurseStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Charges);
            /*BetterUI.ItemStats.RegisterStat(OrangeConsumed.instance.ItemDef,
                prefix + "RINGMISERLYPROTECTION_HEALTH",
                Orange.cfgMaxHealthIncrease.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                BetterUI.ItemStats.ItemTag.SkillCooldown);*/

            #endregion NoTier

            #region Void Uncommon

            BetterUI.ItemStats.RegisterStat(BattleStandardVoid.instance.ItemDef,
                prefix + "BATTLESTANDARDVOID_DAMAGEBONUSPERMINION",
                BattleStandardVoid.cfgDamage.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Percent,
                BetterUI.ItemStats.ItemTag.Damage);
            BetterUI.ItemStats.RegisterStat(CoinCrownVoid.instance.ItemDef,
                prefix + "EXTRACASHPERKILL",
                CoinCrownVoid.cfgCashAdder.Value,
                CoinCrownVoid.cfgCashAdderStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Charges);

            #endregion Void Uncommon

            #region Void Legendary

            /*BetterUI.ItemStats.RegisterStat(CloneVoid.instance.ItemDef,
                prefix + "CLONEVOID_ITEMSPERSTAGE",
                CloneVoid.cfgItemsToGet.Value,
                CloneVoid.cfgItemsToGetPerStack.Value,
                BetterUI.ItemStats.LinearStacking,
                BetterUI.ItemStats.StatFormatter.Charges);
            BetterUI.ItemStats.RegisterStat(CloneVoid.instance.ItemDef,
                prefix + "CLONEVOID_STAGECOUNT",
                CloneVoid.cfgStageCount.Value,
                BetterUI.ItemStats.NoStacking,
                BetterUI.ItemStats.StatFormatter.Charges);*/

            #endregion Void Legendary
        }
    }
}