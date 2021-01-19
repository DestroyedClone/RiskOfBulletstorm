using System;
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
using static RiskOfBulletstorm.Shared.SharedMethods;
using RiskOfBulletstorm.Shared;

namespace RiskOfBulletstorm.Items
{
    public class IrradiatedLead : Item_V2<IrradiatedLead>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance to poison per stack (Default: 5%)", AutoConfigFlags.PreventNetMismatch)]
        public float ProcChance { get; private set; } = 0.05f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Poison duration (Default: 3 s)", AutoConfigFlags.PreventNetMismatch)]
        public float Duration { get; private set; } = 3f;
        public override string displayName => "Irradiated Lead";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Poison Shot\nThese irradiated slugs have a chance to poison any target they touch.";

        protected override string GetDescString(string langid = null) => $"{Pct(ProcChance)} chance per stack to poison enemy for {Duration} seconds";

        protected override string GetLoreString(string langID = null) => "A favorite of the Resourceful Rat.";


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
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);

            GiveLeadEffect(damageInfo, victim, catalogIndex, BuffIndex.Blight, DotController.DotIndex.Blight, Duration);
        }
    }
}
