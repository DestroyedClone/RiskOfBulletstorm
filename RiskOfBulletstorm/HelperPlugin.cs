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

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches recommended by modwiki")]
            public static bool enabled
            {
                get
                {
                    if (_enabled == null) _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
                    return (bool)_enabled;
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches methodname from original")]
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

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches methodname from original")]
            public static void setPanelTitle(string panelTitle)
            {
                ModSettingsManager.setPanelTitle(panelTitle);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches methodname from original")]
            public static void setPanelDescription(string panelDescription)
            {
                ModSettingsManager.setPanelDescription(panelDescription);
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches methodname from original")]
            public static void addStartupListener(UnityEngine.Events.UnityAction unityAction)
            {
                ModSettingsManager.addStartupListener(unityAction);
               // ModSettingsManager.addStartupListener(new UnityEngine.Events.UnityAction(loadModels));
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches methodname from original")]
            public static string getOptionValue(string keybindingName)
            {
                return ModSettingsManager.getOptionValue(keybindingName);
            }

            //[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Matches methodname from original")]
            /*public static void addListener(ModOption modOption, UnityEngine.Events.UnityAction<float> unityAction)
            {
                ModSettingsManager.addListener(ModSettingsManager.getOption("Test Slider"), new UnityEngine.Events.UnityAction<float>(floatEvent));
            }*/
        }
    }
}