using TILER2;
using RoR2;
using UnityEngine;
using BepInEx;
using ThinkInvisible.ClassicItems;

//credit to chen for the base stuff
namespace RiskOfBulletstorm
{
    public class HelperPlugin : BaseUnityPlugin
    {
        public static class ClassicItemsCompat
        {
            private static bool? _enabled;

            internal static bool hasSetup = false;
            internal static void Setup()
            {
                if (hasSetup)
                {
                    RiskofBulletstorm._logger.LogWarning("[Bulletstorm] ClassicItemsCompat.Setup: Already performed. Skipping.");
                    return;
                }
                hasSetup = true;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "dont want to")]
            public static bool enabled
            {
                get
                {
                    if (_enabled == null) _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");
                    return (bool)_enabled;
                }
            }

            public static bool CheckEmbryoProc(Equipment_V2 instance, CharacterBody characterBody)
            {
                return instance.CheckEmbryoProc(characterBody);
            }
            public static void RegisterEmbryo(EquipmentIndex equipmentIndex)
            {
                Embryo_V2.instance.Compat_Register(equipmentIndex);
                RiskofBulletstorm._logger.LogInfo("[Bulletstorm] Embryo_V2Compat_: Registed equipment ID " + equipmentIndex + EquipmentCatalog.GetEquipmentDef(equipmentIndex).name);
            }
        }

    }
}