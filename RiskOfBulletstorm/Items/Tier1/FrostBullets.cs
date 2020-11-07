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
using static RiskOfBulletstorm.Shared.SharedLead;
using RiskOfBulletstorm.Shared;

namespace RiskOfBulletstorm.Items
{
    public class FrostBullets : Item_V2<FrostBullets>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance to freeze per stack (Default: 5%)", AutoConfigFlags.PreventNetMismatch)]
        public float ProcChance { get; private set; } = 0.05f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Freezing duration. Frozen is double that amount. (Default: 3 s)", AutoConfigFlags.PreventNetMismatch)]
        public float Duration { get; private set; } = 3f;
        public override string displayName => "Irradiated Lead";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Icy Fire\nAll bullets can freeze enemies.";

        protected override string GetDescString(string langid = null) => $"{Pct(ProcChance)} chance per stack to freeze enemy for {Duration} seconds";

        protected override string GetLoreString(string langID = null) => "This bullet upgrade attaches small condensers to each bullet fired, lowering their temperature substantially. Accidentally invented by the Gungeon Acquisitions team when Cadence told Ox to \"Cool it with all the bullets.\"";


        public static BuffIndex GungeonFreezeStackDebuff;
        public static BuffIndex GungeonFrozenDebuff;

        const int stacksToFrozen = 12;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

            var freezeStackDebuff = new CustomBuff(
            new BuffDef
            {
                name = "Freezing",
                buffColor = new Color(10, 125, 125),
                canStack = true,
                isDebuff = true,
            });
            GungeonFreezeStackDebuff = BuffAPI.Add(freezeStackDebuff);

            var frozenDebuff = new CustomBuff(
            new BuffDef
            {
                name = "Frozen!",
                buffColor = new Color(10, 255, 255),
                canStack = false,
                isDebuff = true,
            });
            GungeonFrozenDebuff = BuffAPI.Add(frozenDebuff);

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
            var body = victim.gameObject.GetComponent<CharacterBody>();
            int BuffCount = body.GetBuffCount(GungeonFreezeStackDebuff);

            switch(BuffCount)
            {
                case stacksToFrozen:
                    body.RemoveBuff(GungeonFreezeStackDebuff);
                    body.AddTimedBuffAuthority(GungeonFrozenDebuff, Duration * 2);
                    //Add damage equal to 33% health
                    break;
                default:
                    GiveLeadEffect(damageInfo, victim, catalogIndex, GungeonFreezeStackDebuff, DotController.DotIndex.None, Duration);
                    break;
            }
        }
    }
}
