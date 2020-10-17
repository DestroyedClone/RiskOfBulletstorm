using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;


namespace RiskOfBulletstorm.Items
{
    public class EnragingPhoto : Item<EnragingPhoto>
    {
        public override string displayName => "Enraging Photo";
        public override ItemTier itemTier => RoR2.ItemTier.Tier2;
        protected override string NewLangName(string langID = null) => displayName;

        protected override string NewLangPickup(string langID = null) => "Deal extra damage for a short time after getting hit.";

        protected override string NewLangDesc(string langid = null) => $"Gain a temporary <style=cIsDamage>{Pct(DmgBoost)} damage bonus</style> upon taking <style=cIsDamage>{Pct(HealthThreshold)} </style> of your health that lasts {BuffBaseDuration} seconds. <style=cStack>(+{BuffAddDuration} second duration per additional Enraging Photo.)</style>";
        protected override string NewLangLore(string langID = null) => "\"Don't Believe His Lies\"\n\nA photo that the Convict brought with her to the Gungeon.\nDeal extra damage for a short time after getting hit.\n\nOn the journey to the Breach, the Pilot once asked her why she always stared at this photo. Later, she was released from the brig.";

        [AutoUpdateEventInfo(AutoUpdateEventFlags.InvalidateDescToken)]
        [AutoItemConfig("How many seconds should Enraging Photo's buff last with a single stack? (Default: 5 (seconds))", AutoItemConfigFlags.PreventNetMismatch)]
        public float BuffBaseDuration { get; private set; } = 5f;
        [AutoUpdateEventInfo(AutoUpdateEventFlags.InvalidateDescToken)]
        [AutoItemConfig("How many additional seconds of buff should each Enraging Photo after the first give? (Default: 0.5 (seconds))", AutoItemConfigFlags.PreventNetMismatch)]
        public float BuffAddDuration { get; private set; } = 0.5f;

        [AutoUpdateEventInfo(AutoUpdateEventFlags.InvalidateDescToken)]
        [AutoItemConfig("What percent of health lost should activate the Enraging Photo's damage bonus? (Default: 0.12 (12%))", AutoItemConfigFlags.PreventNetMismatch)]
        public float HealthThreshold { get; private set; } = 0.12f;

        [AutoUpdateEventInfo(AutoUpdateEventFlags.InvalidateDescToken)]
        [AutoItemConfig("How much should your damage be increased when Enraging Photo activates? (Default: 2.00 (200%))", AutoItemConfigFlags.PreventNetMismatch)]
        public float DmgBoost { get; private set; } = 2.00f;

        [AutoItemConfig("If true, damage to shield and barrier (from e.g. Personal Shield Generator, Topaz Brooch) will not count towards triggering Enraging Photo")]
        public bool RequireHealth { get; private set; } = true;


        private static BuffIndex EnragedBuff;

        public EnragingPhoto()
        {
            CustomBuff EnragedBuffDef = new CustomBuff(new BuffDef
            {
                buffColor = new Color(255, 25, 25),
                canStack = true,
                isDebuff = true,
                name = "ROBEnragedBuff",
            });
            EnragedBuff = BuffAPI.Add(EnragedBuffDef);
        }
        protected override void LoadBehavior()
        {
            Chat.AddMessage("Item Get!");
        }

        protected override void UnloadBehavior()
        {
            Chat.AddMessage("Item Lost?");
        }
    }
}
