using BepInEx;
using BepInEx.Configuration;
//using R2API;
//using R2API.Utils;
//using System.Reflection;
//using UnityEngine;
using Path = System.IO.Path;
using TILER2;
using static TILER2.MiscUtil;
//using RoR2;
//using R2API.AssetPlus;

namespace DestroyedClone
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin(ModGuid, ModName, ModVer)]
    public class RiskofBulletstorm : BaseUnityPlugin
    {
        public const string ModVer = "0.0.1";
        public const string ModName = "Risk of Bulletstorm";
        public const string ModGuid = "com.DestroyedClone.RiskOfBulletstorm";

        //private static ConfigFile ConfigFile;
        internal static FilingDictionary<ItemBoilerplate> masterItemList = new FilingDictionary<ItemBoilerplate>();
        private void Awake()
        {
            //ConfigFile = new ConfigFile(Path.Combine(Paths.ConfigPath, ModGuid + ".cfg"), true);
            masterItemList = ItemBoilerplate.InitAll("RiskofBulletstorm");
            /*
            foreach (ItemBoilerplate x in masterItemList)
            {
                x.SetupConfig(ConfigFile);
            } */

            foreach (ItemBoilerplate x in masterItemList)
            {
                x.SetupBehavior();
            }
        }
    }
}