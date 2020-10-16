using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text; //?
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using DestroyedClone.RiskOfBulletstorm;

namespace RiskOfBulletstorm.Items
{
    public class EnragingPhoto : Item<EnragingPhoto>
    {
        [AutoUpdateEventInfo(AutoUpdateEventFlags.InvalidateDescToken)]
        [AutoItemConfig("How many seconds should Enraging Photo's buff last with a single stack? (Default: 5 (seconds))", AutoItemConfigFlags.PreventNetMismatch)]
        public float baseDurationOfBuffInSeconds { get; private set; } = 5f;
        [AutoUpdateEventInfo(AutoUpdateEventFlags.InvalidateDescToken)]
        [AutoItemConfig("How many additional seconds of buff should each Enraging Photo after the first give? (Default: 0.5 (seconds))", AutoItemConfigFlags.PreventNetMismatch)]
        public float additionalDurationOfBuffInSeconds { get; private set; } = 0.5f;

        [AutoUpdateEventInfo(AutoUpdateEventFlags.InvalidateDescToken)]
        [AutoItemConfig("What percent of health lost should activate the Enraging Photo's damage bonus? (Default: 0.12 (12%))", AutoItemConfigFlags.PreventNetMismatch)]
        public float healthThreshold { get; private set; } = 0.12f;

        [AutoUpdateEventInfo(AutoUpdateEventFlags.InvalidateDescToken)]
        [AutoItemConfig("How much should your damage be increased when Enraging Photo activates? (Default: 2.00 (200%))", AutoItemConfigFlags.PreventNetMismatch)]
        public float dmgBoost { get; private set; } = 2.00f;

        [AutoItemConfig("If true, damage to shield and barrier (from e.g. Personal Shield Generator, Topaz Brooch) will not count towards triggering Enraging Photo")]
        public bool requireHealth { get; private set; } = true;

        public override string displayName => "Enraging Photo";

        public override ItemTier itemTier => RoR2.ItemTier.Tier2;

        //public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage }); //giving error
        protected override string NewLangName(string langID = null) => displayName;

        protected override string NewLangPickup(string langID = null) => "Deal extra damage for a short time after getting hit.";

        protected override string NewLangDesc(string langid = null) => $"Gain a temporary <style=cIsDamage>{Pct(dmgBoost)} damage bonus</style> upon taking <style=cIsDamage>{Pct(healthThreshold)} </style> of your health that lasts {baseDurationOfBuffInSeconds} seconds. <style=cStack>(+{additionalDurationOfBuffInSeconds} second duration per additional Enraging Photo.)</style>";

        protected override string NewLangLore(string langID = null) => "\"Don't Believe His Lies\"\n\nA photo that the Convict brought with her to the Gungeon.\nDeal extra damage for a short time after getting hit.\n\nOn the journey to the Breach, the Pilot once asked her why she always stared at this photo. Later, she was released from the brig.";

        public BuffIndex ROBEnraged { get; private set; }
        public EnragingPhoto()
        {
            onAttrib += (tokenIdent, namePrefix) =>
            {
                var robEnraged = new R2API.CustomBuff(
                    new BuffDef
                    {
                        buffColor = Color.red,
                        canStack = false,
                        isDebuff = false,
                        name = namePrefix + "EnragedPhotoDamage",
                    });
                ROBEnraged = R2API.BuffAPI.Add(robEnraged);
            };
        }

        protected override void LoadBehavior()
        {
            GetStatCoefficients += AddDamageReward;
            On.RoR2.HealthComponent.TakeDamage += CalculateDamageReward;
        }

        protected override void UnloadBehavior()
        {
            GetStatCoefficients -= AddDamageReward;
            On.RoR2.HealthComponent.TakeDamage -= CalculateDamageReward;
        }
        private void CalculateDamageReward(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var oldHealth = self.health;
            var oldCH = self.combinedHealth;
            CharacterBody vBody = self.body;
            GameObject vGameObject = self.gameObject;

            orig(self, damageInfo);

            int icnt = GetCount(vBody);
            if (icnt < 1
                || (requireHealth && (oldHealth - self.health) / self.fullHealth < healthThreshold)
                || (!requireHealth && (oldCH - self.combinedHealth) / self.fullCombinedHealth < healthThreshold))
                return;

            var InventoryCount = GetCount(self.body);
            if (InventoryCount > 0)
            {
                self.body.AddTimedBuffAuthority(ROBEnraged, (baseDurationOfBuffInSeconds + (additionalDurationOfBuffInSeconds * InventoryCount - 1)));
            }
            orig(self, damageInfo);
        }
        private void AddDamageReward(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.HasBuff(ROBEnraged)) { args.baseDamageAdd += dmgBoost; }
        }

    }
}
