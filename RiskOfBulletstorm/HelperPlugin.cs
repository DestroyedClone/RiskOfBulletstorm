using TILER2;
using RoR2;
using UnityEngine;
using BepInEx;
using RiskOfOptions;

//credit to chen for the base stuff
namespace RiskOfBulletstorm
{
    using ThinkInvisible.ClassicItems;
    public class HelperPlugin : BaseUnityPlugin
    {
        public static class ClassicItemsCompat
        {
            private static bool? _enabled;

            internal static bool hasSetup = false;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "dont want to")]
            public static bool enabled
            {
                get
                {
                    if (_enabled == null) _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ClassicItemsPlugin.ModGuid);
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
                Debug.Log("Embryo_V2Compat_: Registed equipment " + equipmentIndex);
            }
        }

        public static class RiskOfOptionsCompat
        {
            private static bool? _enabled;

            internal static bool hasSetup = false;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "dont want to")]
            public static bool enabled
            {
                get
                {
                    if (_enabled == null) _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
                    return (bool)_enabled;
                }
            }

            public static void addOption(byte OptionType, string title, string desc, string defaultValue)
            {
                ModOption.OptionType peepee;
                switch (OptionType)
                {
                    case 0:
                        peepee = ModOption.OptionType.Bool;
                        break;
                    case 1:
                        peepee = ModOption.OptionType.Slider;
                        break;
                    default:
                        peepee = ModOption.OptionType.Keybinding;
                        break;
                }
                ModSettingsManager.addOption(new ModOption(peepee, title, desc, defaultValue));
            }

            public static void setPanelTitle(string panelTitle)
            {
                ModSettingsManager.setPanelTitle(panelTitle);
            }

            public static void setPanelDescription(string panelDescription)
            {
                ModSettingsManager.setPanelDescription(panelDescription);
            }


        }
    }
}