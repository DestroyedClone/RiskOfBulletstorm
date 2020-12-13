using EntityStates.Treebot.Weapon;
using R2API;
using RoR2;
using RoR2.UI;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;
using ThinkInvisible.ClassicItems;

namespace RiskOfBulletstorm.Items
{
    public class MultiplayerBuddy : Equipment_V2<MultiplayerBuddy>
    {
        public override string displayName => "MultiplayerBuddy";
        public override float cooldown { get; protected set; } = 0f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            Embryo_V2.instance.Compat_Register(catalogIndex);
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.UI.ConsoleWindow.OnEnable += ConsoleWindow_OnEnable;
            On.RoR2.UI.ConsoleWindow.OnDisable += ConsoleWindow_OnDisable;
            On.RoR2.UI.ChatBox.OnEnable += ChatBox_OnEnable;
        }

        private void ChatBox_OnEnable(On.RoR2.UI.ChatBox.orig_OnEnable orig, ChatBox self)
        {
            throw new System.NotImplementedException();
        }

        private void ConsoleWindow_OnDisable(On.RoR2.UI.ConsoleWindow.orig_OnDisable orig, ConsoleWindow self)
        {
            throw new System.NotImplementedException();
        }

        private void ConsoleWindow_OnEnable(On.RoR2.UI.ConsoleWindow.orig_OnEnable orig, ConsoleWindow self)
        {
            orig.
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            return false;
        }
    }
}
