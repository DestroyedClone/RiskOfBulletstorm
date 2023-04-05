using BepInEx;
using HarmonyLib;
using R2API;
using R2API.Utils;
using RiskOfBulletstormRewrite.Artifact;
using RiskOfBulletstormRewrite.Controllers;
using RiskOfBulletstormRewrite.Equipment;
using RiskOfBulletstormRewrite.Equipment.EliteEquipment;
using RiskOfBulletstormRewrite.Items;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Networking;
using Path = System.IO.Path;

[assembly: HG.Reflection.SearchableAttribute.OptIn]

////dotnet build --configuration Release
namespace RiskOfBulletstormRewrite
{
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [BepInDependency("com.xoxfaby.BetterUI", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [R2APISubmoduleDependency(nameof(ItemAPI),
    nameof(LanguageAPI),
    nameof(ContentAddition),
    nameof(EliteAPI),
    nameof(RecalculateStatsAPI),
    nameof(DirectorAPI))]
    public class Main : BaseUnityPlugin
    {
        public const string ModGuid = "com.DestroyedClone.RiskOfBulletstorm";
        public const string ModName = "Risk of Bulletstorm";
        public const string ModVer = "1.2.2";

        internal static BepInEx.Logging.ManualLogSource _logger;

        public List<ArtifactBase> Artifacts = new List<ArtifactBase>();
        public List<ItemBase> Items = new List<ItemBase>();
        public List<EquipmentBase> Equipments = new List<EquipmentBase>();
        public List<EliteEquipmentBase> EliteEquipments = new List<EliteEquipmentBase>();

        public static List<ItemDef> itemDefsThatCantBeAutoPickedUp = new List<ItemDef>();
        public static List<PickupIndex> pickupIndicesThatCantBePickedUp = new List<PickupIndex>();
        public static R2API.ScriptableObjects.R2APISerializableContentPack ContentPack { get; private set; }
        public static string LocationOfProgram;

        public static PluginInfo pluginInfo;

        private void Awake()
        {
            ContentPack = R2API.ContentManagement.R2APIContentManager.ReserveSerializableContentPack();
        }

        private void Start()
        {
            _logger = Logger;
            pluginInfo = Info;
            LanguageOverrides.config = Config;

            LocationOfProgram = Path.GetDirectoryName(Info.Location);

            // Don't know how to create/use an asset bundle, or don't have a unity project set up?
            // Look here for info on how to set these up: https://github.com/KomradeSpectre/AetheriumMod/blob/rewrite-master/Tutorials/Item%20Mod%20Creation.md#unity-project

            Assets.Init();

            Utils.Buffs.CreateBuffs();
            //On.RoR2.Language.LoadTokensFromFile += Language_LoadTokensFromFile;

            AddToAssembly();
            SetupVoid();
            RoR2Application.onLoad += () =>
            {
                foreach (var item in itemDefsThatCantBeAutoPickedUp)
                {
                    var pickupIndex = PickupCatalog.FindPickupIndex(item.itemIndex);
                    if (pickupIndex.isValid)
                    {
                        pickupIndicesThatCantBePickedUp.Add(pickupIndex);
                    }
                }
            };

            ModSupport.CheckForModSupport();

            //todo enemy essembly thing
            //Enemies.LordofTheJammedMonster.CreatePrefab();
            Commands.Initialize();
            Tweaks.Init(Config);
            /* stupidlanguageshit(); */
            LanguageOverrides.Initialize();
            On.RoR2.GenericPickupController.OnTriggerStay += GenericPickupController_OnTriggerStay;

            //debug
            Run.onRunStartGlobal += Run_onRunStartGlobal;
        }

        private void GenericPickupController_OnTriggerStay(On.RoR2.GenericPickupController.orig_OnTriggerStay orig, GenericPickupController self, UnityEngine.Collider other)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            var cache = self.pickupIndex;
            bool isPickupManual = pickupIndicesThatCantBePickedUp.Contains(self.pickupIndex);
            if (isPickupManual)
                self.pickupIndex = PickupIndex.none;
            orig(self, other);
            self.pickupIndex = cache;
        }

        private void Run_onRunStartGlobal(Run obj)
        {
            RoR2.Console.instance.SubmitCmd(LocalUserManager.GetFirstLocalUser().currentNetworkUser, "no_enemies", true);
            RoR2.Console.instance.SubmitCmd(LocalUserManager.GetFirstLocalUser().currentNetworkUser, "stage1_pod 0", true);
            RoR2.UI.ConsoleWindow.cvConsoleEnabled.SetBool(true);
        }

        public static Dictionary<ItemDef, ItemDef> voidConversions = new Dictionary<ItemDef, ItemDef>();

        public void SetupVoid()
        {
            On.RoR2.Items.ContagiousItemManager.Init += MethodNameHere;
        }

        private void MethodNameHere(On.RoR2.Items.ContagiousItemManager.orig_Init orig)
        {
            foreach (var itemPair in voidConversions)
            {
                _logger.LogMessage($"Adding conversion: {itemPair.Key.nameToken} to {itemPair.Value.nameToken}");
                RoR2.ItemDef.Pair transformation = new RoR2.ItemDef.Pair()
                {
                    itemDef1 = itemPair.Key,
                    itemDef2 = itemPair.Value
                };
                RoR2.ItemCatalog.itemRelationships[RoR2.DLC1Content.ItemRelationshipTypes.ContagiousItem] = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].AddToArray(transformation);
            }

            orig();
        }

        public void AddToAssembly()
        {
            //This section automatically scans the project for all artifacts
            var ArtifactTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ArtifactBase)));

            foreach (var artifactType in ArtifactTypes)
            {
                ArtifactBase artifact = (ArtifactBase)Activator.CreateInstance(artifactType);
                if (ValidateArtifact(artifact, Artifacts))
                {
                    artifact.Init(Config);
                }
            }

            //this section automatically scans the project for all equipment
            var EquipmentTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EquipmentBase)));
            var loadedEquipmentNames = new List<string>();
            var childEquipmentTypes = new List<EquipmentBase>();

            foreach (var equipmentType in EquipmentTypes)
            {
                EquipmentBase equipment = (EquipmentBase)System.Activator.CreateInstance(equipmentType);
                if (equipment.ParentEquipmentName != null)
                {
                    childEquipmentTypes.Add(equipment);
                    continue;
                }

                if (ValidateEquipment(equipment, Equipments))
                {
                    equipment.Init(Config);
                    loadedEquipmentNames.Add(equipment.EquipmentName);
                }
            }

            foreach (var childEquip in childEquipmentTypes)
            {
                if (loadedEquipmentNames.Contains(childEquip.ParentEquipmentName))
                    childEquip.Init(Config);
            }

            //This section automatically scans the project for all items
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));
            var loadedItemNames = new List<string>();
            var childItemTypes = new List<ItemBase>();

            foreach (var itemType in ItemTypes)
            {
                ItemBase item = (ItemBase)System.Activator.CreateInstance(itemType);
                if (
                    item.ParentEquipmentName != null
                    || item.ParentItemName != null
                )
                {
                    childItemTypes.Add(item);
                    continue;
                }
                if (ValidateItem(item, Items))
                {
                    item.Init(Config);
                    loadedItemNames.Add(item.ItemName);
                }
            }

            foreach (var childItem in childItemTypes)
            {
                if (loadedItemNames.Contains(childItem.ParentItemName)
                || loadedEquipmentNames.Contains(childItem.ParentEquipmentName))
                {
                    //dependent children dont have rights, no validation.
                    //if (ValidateItem(childItem, Items))
                    {
                        childItem.Init(Config);
                    }
                }
            }

            //this section automatically scans the project for all elite equipment
            var EliteEquipmentTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EliteEquipmentBase)));

            foreach (var eliteEquipmentType in EliteEquipmentTypes)
            {
                EliteEquipmentBase eliteEquipment = (EliteEquipmentBase)System.Activator.CreateInstance(eliteEquipmentType);
                if (ValidateEliteEquipment(eliteEquipment, EliteEquipments))
                {
                    eliteEquipment.Init(Config);
                }
            }

            //this section automatically scans the project for all controllers
            var ControllerTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ControllerBase)));

            foreach (var controllerType in ControllerTypes)
            {
                ControllerBase controllerBase = (ControllerBase)System.Activator.CreateInstance(controllerType);
                controllerBase.Init(Config);
            }
            /*
            //this section automatically scans the project for all monsters
            var MonsterTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(MonsterBase)));

            foreach (var monsterType in MonsterTypes)
            {
                ControllerBase monsterBase = (ControllerBase)System.Activator.CreateInstance(monsterType);
                //monsterBase.Init(Config);
            }*/
        }

        #region Validators

        /// <summary>
        /// A helper to easily set up and initialize an artifact from your artifact classes if the user has it enabled in their configuration files.
        /// </summary>
        /// <param name="artifact">A new instance of an ArtifactBase class."</param>
        /// <param name="artifactList">The list you would like to add this to if it passes the config check.</param>
        public bool ValidateArtifact(ArtifactBase artifact, List<ArtifactBase> artifactList)
        {
            var enabled = Config.Bind<bool>("Artifact: " + artifact.ArtifactName, "Enable Artifact?", true, "Should this artifact appear for selection?").Value;

            if (enabled)
            {
                artifactList.Add(artifact);
            }
            return enabled;
        }

        /// <summary>
        /// A helper to easily set up and initialize an item from your item classes if the user has it enabled in their configuration files.
        /// <para>Additionally, it generates a configuration for each item to allow blacklisting it from AI.</para>
        /// </summary>
        /// <param name="item">A new instance of an ItemBase class."</param>
        /// <param name="itemList">The list you would like to add this to if it passes the config check.</param>
        public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
        {
            /*if (item.Tier == ItemTier.NoTier)
            {
                return true;
            }*/
            if (item.IsSkillReplacement)
            {
            }
            var enabled = Config.Bind<bool>(item.ConfigCategory, "Enable Item?", true, "Should this item appear in runs?").Value;
            var aiBlacklist = Config.Bind<bool>(item.ConfigCategory, "Blacklist Item from AI Use?", false, "Should the AI not be able to obtain this item?").Value;
            if (enabled)
            {
                itemList.Add(item);
                if (aiBlacklist)
                {
                    item.AIBlacklisted = true;
                }
            }
            return enabled;
        }

        /// <summary>
        /// A helper to easily set up and initialize an equipment from your equipment classes if the user has it enabled in their configuration files.
        /// </summary>
        /// <param name="equipment">A new instance of an EquipmentBase class."</param>
        /// <param name="equipmentList">The list you would like to add this to if it passes the config check.</param>
        public bool ValidateEquipment(EquipmentBase equipment, List<EquipmentBase> equipmentList)
        {
            if (Config.Bind<bool>(equipment.ConfigCategory, "Enable Equipment?", true, "Should this equipment appear in runs?").Value)
            {
                equipmentList.Add(equipment);
                return true;
            }
            return false;
        }

        /// <summary>
        /// A helper to easily set up and initialize an elite equipment from your elite equipment classes if the user has it enabled in their configuration files.
        /// </summary>
        /// <param name="eliteEquipment">A new instance of an EliteEquipmentBase class.</param>
        /// <param name="eliteEquipmentList">The list you would like to add this to if it passes the config check.</param>
        /// <returns></returns>
        public bool ValidateEliteEquipment(EliteEquipmentBase eliteEquipment, List<EliteEquipmentBase> eliteEquipmentList)
        {
            var enabled = Config.Bind<bool>("Equipment: " + eliteEquipment.EliteEquipmentName, "Enable Elite Equipment?", true, "Should this elite equipment appear in runs? If disabled, the associated elite will not appear in runs either.").Value;

            if (enabled)
            {
                eliteEquipmentList.Add(eliteEquipment);
                return true;
            }
            return false;
        }

        #endregion Validators
    }
}