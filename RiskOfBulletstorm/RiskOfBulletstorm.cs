using RiskOfBulletstorm.Utils;
using static RiskOfBulletstorm.HelperPlugin;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using UnityEngine;
using Path = System.IO.Path;
using TILER2;
using static TILER2.MiscUtil;
using RoR2;
using R2API.AssetPlus;
using R2API.Networking;
using ThinkInvisible.ClassicItems;
//using ExtraSkillSlots;


using EntityStates;
using MonoMod.RuntimeDetour;

using Rewired.Data;
using System;

using System.Security;
using System.Security.Permissions;
//using RiskOfBulletstorm.ExtraSkillSlotsOverrides;

namespace RiskOfBulletstorm
{
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency(TILER2Plugin.ModGuid, TILER2Plugin.ModVer)]
    //[BepInDependency(EliteSpawningOverhaul.EsoPlugin.PluginGuid)]
    //[BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    //[BepInDependency("com.KingEnderBrine.ExtraSkillSlots")]
    [BepInDependency(EnemyItemDisplays.EnemyItemDisplaysPlugin.MODUID, BepInDependency.DependencyFlags.SoftDependency)] //because chen's mod has it
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(
        nameof(ItemAPI),
        nameof(LanguageAPI),
        nameof(ResourcesAPI),
        nameof(PlayerAPI),
        nameof(PrefabAPI),
        nameof(SoundAPI),
        nameof(OrbAPI),
        nameof(NetworkingAPI),
        nameof(EffectAPI),
        nameof(EliteAPI),
        nameof(BuffAPI), //?
        nameof(CommandHelper), //?
        nameof(LoadoutAPI), //Artifacts
        nameof(SurvivorAPI))] //?
    [BepInPlugin(ModGuid, ModName, ModVer)]

    // will be used to prevent dropping the upcoming bullet that can kill the past
    //[BepInDependency("com.ThinkInvisible.Yeet", BepInDependency.DependencyFlags.SoftDependency)]
    //[BepInDependency("KookehsDropItemMod", BepInDependency.DependencyFlags.SoftDependency)]
    public class RiskofBulletstorm : BaseUnityPlugin
    {
        public const string ModVer = "1.1.0";
        public const string ModName = "Risk of Bulletstorm";
        public const string ModGuid = "com.DestroyedClone.RiskOfBulletstorm";

        //private static ConfigFile ConfigFile;
        internal static FilingDictionary<CatalogBoilerplate> masterItemList = new FilingDictionary<CatalogBoilerplate>();

        private static ConfigFile ConfigFile;

        internal static BepInEx.Logging.ManualLogSource _logger;

        private void Awake()
        {
            _logger = Logger;
            //JokeMod.Init();
            R2API.Utils.CommandHelper.AddToConsoleWhenReady();

            /*
            if (RiskOfOptionsCompat.enabled)
            {
                RiskOfOptionsCompat.setPanelTitle("Risk of Bulletstorm");
                RiskOfOptionsCompat.setPanelDescription("Configure keybindings.");

                RiskOfOptionsCompat.addOption(2, "BLANK: Activate Blank (Keyboard)", "Keyboard button to activate a Blank", "T");
                RiskOfOptionsCompat.addOption(2, "BLANK: Activate Blank (Gamepad)", "Gamepad button to activate a Blank", null);

                //RiskOfOptionsCompat.addOption(2, "BACKPACK: Modifier (Keyboard)", "Key to hold down to switch between slots", null);
                RiskOfOptionsCompat.addOption(2, "BACKPACK: Modifier (Gamepad)", "Button to hold down to switch between slots", null);

                RiskOfOptionsCompat.addOption(2, "BACKPACK: Cycle Right (Keyboard)", "Key to cycle to the next equipment", "F");
                RiskOfOptionsCompat.addOption(2, "BACKPACK: Cycle Left (Keyboard)", "Key to cycle to the previous equipment", "G");
                RiskOfOptionsCompat.addOption(2, "BACKPACK: Cycle Right (Gamepad)", "Key to cycle to the next equipment", null);
                RiskOfOptionsCompat.addOption(2, "BACKPACK: Cycle Left (Gamepad)", "Key to cycle to the previous equipment", null);
            }*/


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
        }

        private void Start()
        {
            T2Module.SetupAll_PluginStart(masterItemList);
            CatalogBoilerplate.ConsoleDump(Logger, masterItemList);
        }
    }
}