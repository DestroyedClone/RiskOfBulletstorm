//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using RiskOfBulletstorm.Shared;

namespace RiskOfBulletstorm.Items
{
    public class Curse : Item_V2<Curse>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many curse is needed before Lord of the Jammed spawns? Set it to -1 to disable. (Default: 10)", AutoConfigFlags.PreventNetMismatch)]
        public float Curse_LOTJAmount { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Enable Jammed enemies?", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_Enable { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional damage should a Jammed enemy deal? (Default: 1 (+100%))", AutoConfigFlags.PreventNetMismatch)]
        public float Curse_DamageBoost { get; private set; } = 1.00f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional crit should a Jammed enemy have? (Default: 1 (+100%))", AutoConfigFlags.PreventNetMismatch)]
        public float Curse_CritBoost { get; private set; } = 1f;

        public override string displayName => "CurseMasterItem";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public override void SetupBehavior()
        {

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
            CharacterBody.onBodyStartGlobal += JamEnemy;
            On.RoR2.CharacterBody.OnInventoryChanged += UpdateCurseCount;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            CharacterBody.onBodyStartGlobal -= JamEnemy;
            On.RoR2.CharacterBody.OnInventoryChanged -= UpdateCurseCount;
        }
        private void JamEnemy(CharacterBody obj)
        {
            int PlayerItemCount = HelperUtil.GetPlayersItemCount(catalogIndex);
            float RollValue = 0f;

            switch (PlayerItemCount)
            {
                case 0:
                    RollValue = 0f;
                    break;
                case 1:
                case 2:
                    RollValue = 0.01f;
                    break;
                case 3:
                case 4:
                    RollValue = 0.02f;
                    break;
                case 5:
                case 6:
                    RollValue = 0.05f;
                    break;
                case 7:
                case 8:
                    RollValue = 0.10f;
                    break;
                case 9:
                    RollValue = 0.25f;
                    break;
                case 10:
                    RollValue = 0.50f;
                    break;
                default:
                    RollValue = 0.50f;
                    break;
            } //adjusts jammed chance

            if (obj.teamComponent.teamIndex != TeamIndex.Player)
            {
                Chat.AddMessage("Curse: Rolled with " + RollValue + " with item count: " + PlayerItemCount);
                if (Util.CheckRoll(RollValue))
                {
                    Chat.AddMessage("Jammed Enemy with chance:"+RollValue);
                    obj.AddBuff(GungeonBuffController.Jammed);
                }
            }
        }
        private void UpdateCurseCount(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
        }
    }
}
