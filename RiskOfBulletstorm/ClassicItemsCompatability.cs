using ThinkInvisible.ClassicItems;
using TILER2;
using RoR2;

//credit to chen for the base stuff
namespace RiskOfBulletstorm
{
    public static class ClassicItemsCompat
    {
        internal static bool hasSetup = false;

        private static bool? _enabled;

        public static bool CheckEmbryoProc(Equipment_V2 instance, CharacterBody characterBody)
        {
            return instance.CheckEmbryoProc(characterBody);
        }

        public static void RegisterEmbryo(EquipmentIndex equipmentIndex)
        {
            Embryo_V2.instance.Compat_Register(equipmentIndex);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "dont want to")]
        public static bool enabled
        {
            get
            {
                if (_enabled == null) _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(ClassicItemsPlugin.ModGuid);
                return (bool)_enabled;
            }
        }
    }
}