//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;


namespace RiskOfBulletstorm.Items
{
    public class EnragingPhoto : Item_V2<EnragingPhoto>
    {
        //TODO: USE CHEN's HEALTH LOSS CODE FOR FLOATS
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many seconds should Enraging Photo's buff last with a single stack? (Default: 3 (seconds))", AutoConfigFlags.PreventNetMismatch)]
        public float BaseDurationOfBuffInSeconds { get; private set; } = 3f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional seconds of buff should each Enraging Photo after the first give? (Default: 0.25 (seconds))", AutoConfigFlags.PreventNetMismatch)]
        public float AdditionalDurationOfBuffInSeconds { get; private set; } = 0.25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of health lost should activate the Enraging Photo's damage bonus? (Default: 0.33 (33%))", AutoConfigFlags.PreventNetMismatch)]
        public float HealthThreshold { get; private set; } = 0.33f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much should your damage be increased when Enraging Photo activates? (Default: 1.00 (+100% damage))", AutoConfigFlags.PreventNetMismatch)]
        public float DmgBoost { get; private set; } = 1.00f;
        
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("If true, damage to shield and barrier (from e.g. Personal Shield Generator, Topaz Brooch) will not count towards triggering Enraging Photo")]
        public bool RequireHealth { get; private set; } = true;

        public override string displayName => "Enraging Photo";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Deal extra damage for a short time after receivig a heavy hit.";

        protected override string GetDescString(string langid = null) => $"Gain a temporary <style=cIsDamage>{Pct(DmgBoost)} damage bonus</style> upon taking <style=cIsDamage>{Pct(HealthThreshold)} </style> of your health that lasts {BaseDurationOfBuffInSeconds} seconds. <style=cStack>(+{AdditionalDurationOfBuffInSeconds} second duration per additional Enraging Photo.)</style>";

        protected override string GetLoreString(string langID = null) => "\"Don't Believe His Lies\"\n\nA photo that the Convict brought with her to the Gungeon.\nDeal extra damage for a short time after getting hit.\n\nOn the journey to the Breach, the Pilot once asked her why she always stared at this photo. Later, she was released from the brig.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject ItemBodyModelPrefab;

        public BuffIndex ROBEnraged { get; private set; }

        public override void SetupBehavior()
        {

        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            var dmgBuff = new CustomBuff(
            new BuffDef
            {
                buffColor = Color.red,
                canStack = false,
                isDebuff = false,
                name = "Enraged",
            });
            ROBEnraged = BuffAPI.Add(dmgBuff);
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.HealthComponent.TakeDamage += CalculateDamageReward;
            GetStatCoefficients += AddDamageReward;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= CalculateDamageReward;
            GetStatCoefficients -= AddDamageReward;
        }
        private void CalculateDamageReward(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            var InventoryCount = GetCount(self.body);

            /*var oldHealth = self.health;
            var oldCH = self.combinedHealth;

            if (InventoryCount < 1
                || (RequireHealth && (oldHealth - self.health) / self.fullHealth < HealthThreshold)
                || (!RequireHealth && (oldCH - self.combinedHealth) / self.fullCombinedHealth < HealthThreshold))
                return;*/
            var oldHealth = self.health;
            orig(self, damageInfo);

            if (InventoryCount < 1
                || (oldHealth - self.health) / self.fullHealth < HealthThreshold)
                return;

            if (InventoryCount > 0 && self.body.GetBuffCount(ROBEnraged) < InventoryCount)
            {
                self.body.AddTimedBuffAuthority(ROBEnraged, (BaseDurationOfBuffInSeconds + (AdditionalDurationOfBuffInSeconds * InventoryCount - 1)));
            }
            orig(self, damageInfo);
        }

        private void AddDamageReward(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.HasBuff(ROBEnraged)) { args.damageMultAdd += DmgBoost; }
        }
    }
}
