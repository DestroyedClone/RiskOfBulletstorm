using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using UnityEngine;
using Path = System.IO.Path;
using TILER2;
using static TILER2.MiscUtil;
using R2API.Networking;
using Chen.Helpers;
using Chen.Helpers.GeneralHelpers;
using Chen.Helpers.LogHelpers;
using Chen.Helpers.LogHelpers.Collections;
using static Chen.Helpers.GeneralHelpers.AssetsManager;

using RiskOfBulletstorm;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System.Collections.Generic;
using System;
using EntityStates;
using RoR2.Skills;

namespace RiskOfBulletstorm
{
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(TILER2Plugin.ModGuid, TILER2Plugin.ModVer)]
    [BepInDependency(EnemyItemDisplays.EnemyItemDisplaysPlugin.MODUID, BepInDependency.DependencyFlags.SoftDependency)] //because chen's mod has it
    [BepInDependency("dev.ontrigger.itemstats", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(Chen.Helpers.HelperPlugin.ModGuid, Chen.Helpers.HelperPlugin.ModVer)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(
        nameof(ItemAPI),
        nameof(BuffAPI),
        nameof(LanguageAPI),
        nameof(ResourcesAPI),
        nameof(PrefabAPI),
        nameof(SoundAPI),
        nameof(OrbAPI),
        nameof(NetworkingAPI),
        nameof(EffectAPI),
        nameof(EliteAPI),
        nameof(LoadoutAPI),
        nameof(SurvivorAPI),
        nameof(ProjectileAPI)
        )]
    [BepInPlugin(ModGuid, ModName, ModVer)]

    public class BulletstormPlugin : BaseUnityPlugin
    {
        public const string ModVer = "1.2.0";
        public const string ModName = "Risk of Bulletstorm";
        public const string ModGuid = "com.DestroyedClone.RiskOfBulletstorm";

        internal static FilingDictionary<CatalogBoilerplate> masterItemList = new FilingDictionary<CatalogBoilerplate>();

        private static ConfigFile ConfigFile;

        internal static BepInEx.Logging.ManualLogSource _logger;
        internal static AssetBundle assetBundle;
        internal static string displayName = "Risk of Bulletstorm";

        private void Awake()
        {
            _logger = Logger;

            Logger.LogDebug("Loading assets...");

            BundleInfo bundleInfo = new BundleInfo("@RiskOfBulletstorm", "RiskOfBulletstorm.riskofgungeonassets", BundleType.UnityAssetBundle);
            assetBundle = new AssetsManager(bundleInfo).Register() as AssetBundle;

            ConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);

            masterItemList = T2Module.InitAll<CatalogBoilerplate>(new T2Module.ModInfo
            {
                displayName = displayName,
                longIdentifier = "RISKOFBULLETSTORMMOD",
                shortIdentifier = "RBS",
                mainConfigFile = ConfigFile
            });
            T2Module.SetupAll_PluginAwake(masterItemList);

            Shared.Buffs.BuffsController.Init();

            R2API.Utils.CommandHelper.AddToConsoleWhenReady();
        }

        private void Start()
        {
            T2Module.SetupAll_PluginStart(masterItemList);
            CatalogBoilerplate.ConsoleDump(Logger, masterItemList);
        }
    }
}