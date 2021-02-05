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

namespace RiskOfBulletstorm
{
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(TILER2Plugin.ModGuid, TILER2Plugin.ModVer)]
    [BepInDependency(EnemyItemDisplays.EnemyItemDisplaysPlugin.MODUID, BepInDependency.DependencyFlags.SoftDependency)] //because chen's mod has it
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(
        nameof(ItemAPI),
        nameof(BuffAPI),
        nameof(LanguageAPI),
        nameof(ResourcesAPI),
        nameof(PlayerAPI),
        nameof(PrefabAPI),
        nameof(SoundAPI),
        nameof(OrbAPI),
        nameof(NetworkingAPI),
        nameof(EffectAPI),
        nameof(EliteAPI),
        nameof(LoadoutAPI),
        nameof(SurvivorAPI)
        )]
    [BepInPlugin(ModGuid, ModName, ModVer)]

    public class BulletstormPlugin : BaseUnityPlugin
    {
        public const string ModVer = "1.1.0";
        public const string ModName = "Risk of Bulletstorm";
        public const string ModGuid = "com.DestroyedClone.RiskOfBulletstorm";

        internal static FilingDictionary<CatalogBoilerplate> masterItemList = new FilingDictionary<CatalogBoilerplate>();

        private static ConfigFile ConfigFile;

        internal static BepInEx.Logging.ManualLogSource _logger;

        private void Awake()
        {
            _logger = Logger;

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RiskOfBulletstorm.riskofgungeonassets"))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                var provider = new AssetBundleResourcesProvider("@RiskOfBulletstorm", bundle);
                ResourcesAPI.AddProvider(provider);
            }

            ConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);

            masterItemList = T2Module.InitAll<CatalogBoilerplate>(new T2Module.ModInfo
            {
                displayName = "Risk of Bulletstorm",
                longIdentifier = "RISKOFBULLETSTORMMOD",
                shortIdentifier = "ROB",
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