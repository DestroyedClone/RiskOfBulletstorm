using RiskOfBulletstormRewrite.Artifact;
using RiskOfBulletstormRewrite.Equipment;
using RiskOfBulletstormRewrite.Items;
using RiskOfBulletstormRewrite.Modules;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using System;
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
                BetterUICompat.StatsDisplay();
                BetterUICompat.ItemStats();
                BetterUICompat.BuffsDisplay();
                BetterUICompat.ItemTags();
            }
        }

        internal static class BetterUICompat
        {

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void StatsDisplay()
            {
                BetterUI.StatsDisplay.AddStatsDisplay("$riskofbulletstorm_accuracy", (BetterUI.StatsDisplay.DisplayCallback)GetAccuracy);
                BetterUI.StatsDisplay.AddStatsDisplay("$riskofbulletstorm_curse", (BetterUI.StatsDisplay.DisplayCallback)GetCurse);
                BetterUI.StatsDisplay.AddStatsDisplay("$riskofbulletstorm_steal", (BetterUI.StatsDisplay.DisplayCallback)GetStealChance);
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void ItemTags()
            {

            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void BetterUICompat_RegisterTag()
            {
                BetterUI.ItemStats.RegisterTag(new BetterUI.ItemStats.ItemStat(), new BetterUI.ItemStats.ItemTag()
                {

                });
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void BuffsDisplay()
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
                GetFloat(AlphaBullets.damage),
                GetFloat(AlphaBullets.damagePerStack)
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
                GetFloat(AlphaBullets.damage),
                GetFloat(AlphaBullets.damagePerStack)});
                RegisterBuffInfo(Buffs.DodgeRollBuff, "DODGEROLL",
                new string[]{
                Utils.ItemHelpers.Pct(DodgeRollUtilityReplacement.cfgDamageVulnerabilityMultiplier),
                Utils.ItemHelpers.Pct(DodgeRollUtilityReplacement.cfgDamageVulnerabilityMultiplierPerStack)
                });
                /*RegisterBuffInfo(Buffs.ArtifactSpeedUpBuff,
                    "ARTIFACTSPEEDUP",
                    new string[] { ArtifactUpSpeedOOC.moveSpeedAdditiveMultiplier.ToString() });*/
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            internal static void ItemStats()
            {
                //todo: capformula, registermodifier
                var prefix = "RISKOFBULLETSTORM_STAT_";

                #region Common

                BetterUI.ItemStats.RegisterStat(Items.Antibody.instance.ItemDef,
                    prefix + "ANTIBODY_HEALCHANCE",
                    Antibody.cfgChance / 100, //divided by 100 because the rollchacne is 25f
                    BetterUI.ItemStats.NoStacking,
                    BetterUI.ItemStats.StatFormatter.Chance);
                BetterUI.ItemStats.RegisterStat(Antibody.instance.ItemDef,
                    prefix + "ANTIBODY_HEALAMOUNT",
                    Antibody.cfgMultiplier,
                    Antibody.cfgMultiplierPerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent,
                    BetterUI.ItemStats.ItemTag.Healing);
                /*BetterUI.ItemStats.RegisterStat(IrradiatedLead.instance.ItemDef,
                    prefix + "IRRADIATEDLEAD_POISONCHANCE",
                    IrradiatedLead.cfgChance,
                    IrradiatedLead.cfgChanceStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent);*/
                BetterUI.ItemStats.RegisterStat(Mustache.instance.ItemDef,
                    prefix + "MUSTACHE_DURATION",
                    Mustache.cfgDuration,
                    BetterUI.ItemStats.NoStacking,
                    BetterUI.ItemStats.StatFormatter.Seconds);
                BetterUI.ItemStats.RegisterStat(Mustache.instance.ItemDef,
                    prefix + "MUSTACHE_REGEN",
                    Mustache.cfgRegenAmount,
                    Mustache.cfgRegenAmountPerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Regen,
                    BetterUI.ItemStats.ItemTag.Healing);
                BetterUI.ItemStats.RegisterStat(Scope.instance.ItemDef,
                    prefix + "SCOPE_SPREADREDUCTION",
                    Scope.cfgSpreadReduction,
                    Scope.cfgSpreadReductionPerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent,
                    null);

                #endregion Common

                #region Uncommon

                BetterUI.ItemStats.RegisterStat(AlphaBullets.instance.ItemDef,
                    prefix + "ALPHABULLETS_DAMAGEBONUS",
                    AlphaBullets.damage,
                    AlphaBullets.damagePerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent,
                    BetterUI.ItemStats.ItemTag.Damage);
                //todo get stat for buff count * damage

                /* BetterUI.ItemStats.RegisterStat(AmmoBelt.instance.ItemDef,
                    prefix + "AMMOBELT_EXTRASTOCKS",
                    AmmoBelt.cfgPercentageStockAdditive,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent); */

                BetterUI.ItemStats.RegisterStat(BattleStandard.instance.ItemDef,
                    prefix + "BATTLESTANDARD_DAMAGEBONUS",
                    BattleStandard.damage,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent,
                    BetterUI.ItemStats.ItemTag.Damage);

                /* BetterUI.ItemStats.RegisterStat(GhostBullets.instance.ItemDef,
                    prefix + "GHOSTBULLETS_DAMAGEREDUCTION",
                    GhostBullets.cfgDamageReduction,
                    BetterUI.ItemStats.NoStacking,
                    BetterUI.ItemStats.StatFormatter.Percent);
                BetterUI.ItemStats.RegisterStat(GhostBullets.instance.ItemDef,
                    prefix + "GHOSTBULLETS_PIERCECOUNT",
                    1,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Charges); */

                /* BetterUI.ItemStats.RegisterStat(RingChestFriendship.instance.ItemDef,
                    prefix + "RINGCHESTFRIENDSHIP_CREDITMULTIPLIER",
                    RingChestFriendship.cfgCreditMultiplier,
                    RingChestFriendship.cfgCreditMultiplierPerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent); */

                /*BetterUI.ItemStats.RegisterStat(RingFireResistance.instance.ItemDef,
                    prefix + "RINGFIRERESISTANCE_REDUCTION",
                    RingFireResistance.cfgBaseResist,
                    BetterUI.ItemStats.HyperbolicStacking,
                    BetterUI.ItemStats.StatFormatter.Percent);*/

                BetterUI.ItemStats.RegisterStat(CoinCrown.instance.ItemDef,
                    prefix + "COINCROWN_MONEYMULTADD",
                    CoinCrown.cashMultiplier,
                    CoinCrown.cashMultiplierPerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent);

                #endregion Uncommon

                #region Legendary

                /*BetterUI.ItemStats.RegisterStat(Backpack.instance.ItemDef,
                    prefix + "BACKPACK_SLOTS",
                    1,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Charges,
                    null);*/
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
                    Clone.cfgItemsToKeep,
                    Clone.cfgItemsToKeepPerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Charges,
                    null);*/

                #endregion Legendary

                #region Lunar

                BetterUI.ItemStats.RegisterStat(BloodiedScarf.instance.ItemDef,
                    prefix + "BLOODIEDSCARF_DISTANCE",
                    BloodiedScarf.cfgTeleportRange,
                    BloodiedScarf.cfgTeleportRangePerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Range);
                BetterUI.ItemStats.RegisterStat(BloodiedScarf.instance.ItemDef,
                    prefix + "BLOODIEDSCARF_DAMAGEVULNERABILITY",
                    BloodiedScarf.cfgDamageVulnerabilityMultiplier,
                    BloodiedScarf.cfgDamageVulnerabilityMultiplierPerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent);
                BetterUI.ItemStats.RegisterStat(BloodiedScarf.instance.ItemDef,
                    prefix + "BLOODIEDSCARF_VULNERABILITYDURATION",
                    BloodiedScarf.cfgDamageVulnerabilityDuration,
                    BetterUI.ItemStats.NoStacking,
                    BetterUI.ItemStats.StatFormatter.Seconds);

                BetterUI.ItemStats.RegisterStat(DodgeRollUtilityReplacement.instance.ItemDef,
                    prefix + "DODGEROLLUTILITYREPLACEMENT_DAMAGEVULNERABILITY",
                    DodgeRollUtilityReplacement.cfgDamageVulnerabilityMultiplier,
                    DodgeRollUtilityReplacement.cfgDamageVulnerabilityMultiplierPerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent);
                BetterUI.ItemStats.RegisterStat(DodgeRollUtilityReplacement.instance.ItemDef,
                    prefix + "DODGEROLLUTILITYREPLACEMENT_VULNERABILITYDURATION",
                    DodgeRollUtilityReplacement.cfgDamageVulnerabilityDuration,
                    DodgeRollUtilityReplacement.cfgDamageVulnerabilityDurationDecreasePerStack,
                    DodgeRollDurationStacking,
                    BetterUI.ItemStats.StatFormatter.Seconds);

                /*BetterUI.ItemStats.RegisterStat(HipHolster.instance.ItemDef,
                    prefix + "HIPHOLSTER_CHANCE",
                    HipHolster.cfgFreeStockChance,
                    HipHolster.cfgFreeStockChancePerStack,
                    CustomHyperbolicStacking,
                    BetterUI.ItemStats.StatFormatter.Chance,
                    BetterUI.ItemStats.ItemTag.SkillCooldown);*/
                /*BetterUI.ItemStats.RegisterStat(HipHolster.instance.ItemDef,
                    prefix + "HIPHOLSTER_CHANCE",
                    HipHolster.cfgChanceHyperbolic,
                    BetterUI.ItemStats.HyperbolicStacking,
                    BetterUI.ItemStats.StatFormatter.Chance,
                    BetterUI.ItemStats.ItemTag.SkillCooldown);*/

                BetterUI.ItemStats.RegisterStat(RingMiserlyProtection.instance.ItemDef,
                    prefix + "RINGMISERLYPROTECTION_HEALTH",
                    RingMiserlyProtection.cfgMaxHealthPctAdded,
                    RingMiserlyProtection.cfgMaxHealthPctAddedStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent,
                    BetterUI.ItemStats.ItemTag.MaxHealth);

                #endregion Lunar

                #region NoTier

                BetterUI.ItemStats.RegisterStat(Items.OrangeConsumed.instance.ItemDef,
                    prefix + "ORANGECONSUMED_MAXHEALTHADD",
                    Orange.cfgMaxHealthIncrease,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent,
                    BetterUI.ItemStats.ItemTag.MaxHealth);
                BetterUI.ItemStats.RegisterStat(Items.OrangeConsumed.instance.ItemDef,
                    prefix + "ORANGECONSUMED_SKILLREDUCTION",
                    Orange.cfgChargeRateReduction,
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
                    Spice2.cfgStatDamage,
                    Spice2.cfgStatDamageStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent);
                BetterUI.ItemStats.RegisterStat(SpiceTally.instance.ItemDef,
                    prefix + "SPICETALLY_ACCURACY",
                    Spice2.cfgStatAccuracy,
                    Spice2.cfgStatAccuracyStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent);
                BetterUI.ItemStats.RegisterStat(SpiceTally.instance.ItemDef,
                    prefix + "SPICETALLY_ROR2CURSE",
                    Spice2.cfgStatRORCurse,
                    Spice2.cfgStatRORCurseStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Charges);
                /*BetterUI.ItemStats.RegisterStat(OrangeConsumed.instance.ItemDef,
                    prefix + "RINGMISERLYPROTECTION_HEALTH",
                    Orange.cfgMaxHealthIncrease,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent,
                    BetterUI.ItemStats.ItemTag.SkillCooldown);*/

                #endregion NoTier

                #region Void Uncommon

                BetterUI.ItemStats.RegisterStat(BattleStandardVoid.instance.ItemDef,
                    prefix + "BATTLESTANDARDVOID_DAMAGEBONUSPERMINION",
                    BattleStandardVoid.damage,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Percent,
                    BetterUI.ItemStats.ItemTag.Damage);
                BetterUI.ItemStats.RegisterStat(CoinCrownVoid.instance.ItemDef,
                    prefix + "EXTRACASHPERKILL",
                    CoinCrownVoid.cfgCashAdder,
                    CoinCrownVoid.cfgCashAdderStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Charges);

                #endregion Void Uncommon

                #region Void Legendary

                /*BetterUI.ItemStats.RegisterStat(CloneVoid.instance.ItemDef,
                    prefix + "CLONEVOID_ITEMSPERSTAGE",
                    CloneVoid.cfgItemsToGet,
                    CloneVoid.cfgItemsToGetPerStack,
                    BetterUI.ItemStats.LinearStacking,
                    BetterUI.ItemStats.StatFormatter.Charges);
                BetterUI.ItemStats.RegisterStat(CloneVoid.instance.ItemDef,
                    prefix + "CLONEVOID_STAGECOUNT",
                    CloneVoid.cfgStageCount,
                    BetterUI.ItemStats.NoStacking,
                    BetterUI.ItemStats.StatFormatter.Charges);*/

                #endregion Void Legendary
            }
        }

        private static string GetFloat(BepInEx.Configuration.ConfigEntry<float> entry)
        {
            return GetFloat(entry);
        }
        private static string GetFloat(float value)
        {
            return value.ToString();
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

        public static float DodgeRollDurationStacking(float value, float extraStac, int stacks)
        {
            return DodgeRollUtilityReplacement.GetDuration(stacks);
        }

        public static float CustomHyperbolicStacking(float value, float extraStac, int stacks)
        {
            return Utils.ItemHelpers.GetHyperbolicValue(value, extraStac, stacks);
        }
    }
}