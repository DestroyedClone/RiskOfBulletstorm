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
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
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

            //ExtraAwake();
        }

        /*public void ExtraAwake()
        {
            //Add actions to RoR2.InputCatalog
            ExtraInputs.AddActionsToInputCatalog();

            //Hook to method with some rewired initialization (or not? Anyway it works) to add custom actions
            var userDataInit = typeof(UserData).GetMethod("KFIfLMJhIpfzcbhqEXHpaKpGsgeZ", BindingFlags.NonPublic | BindingFlags.Instance);
            new Hook(userDataInit, (Action<Action<UserData>, UserData>)ExtraInputs.AddCustomActions);

            //Adding custom actions to Settings
            On.RoR2.UI.SettingsPanelController.Start += UIHooks.SettingsPanelControllerStart;

            //Adding custom InputBankTest
            On.RoR2.InputBankTest.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<ExtraInputBankTest>();
            };
            On.RoR2.InputBankTest.CheckAnyButtonDown += ExtraInputBankTest.CheckAnyButtonDownOverrideHook;

            //Applying override to BaseSkillState
            IL.EntityStates.BaseSkillState.IsKeyDownAuthority += ExtraBaseSkillState.IsKeyDownAuthorityILHook;

            var baseSkillStateOnEnter = typeof(BaseSkillState).GetMethod("OnEnter", BindingFlags.Public | BindingFlags.Instance);
            new Hook(baseSkillStateOnEnter, new Action<Action<BaseSkillState>, BaseSkillState>((orig, self) =>
            {
                ExtraBaseSkillState.Add(self);
                orig(self);
            }));

            var baseSkillStateOnExit = typeof(BaseSkillState).GetMethod("OnExit", BindingFlags.Public | BindingFlags.Instance);
            new Hook(baseSkillStateOnExit, new Action<Action<BaseSkillState>, BaseSkillState>((orig, self) =>
            {
                orig(self);
                ExtraBaseSkillState.Remove(self);
            }));

            //Adding custom PlayerCharacterMasterController
            On.RoR2.PlayerCharacterMasterController.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<ExtraPlayerCharacterMasterController>();
            };
            On.RoR2.PlayerCharacterMasterController.SetBody += ExtraPlayerCharacterMasterController.SetBodyOverrideHook;
        }*/
        

        private void Start()
        {
            T2Module.SetupAll_PluginStart(masterItemList);
            CatalogBoilerplate.ConsoleDump(Logger, masterItemList);
        }
    }
}