/*
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
    public class GreenGuonStone : Item_V2<GreenGuonStone>
    {
        //TODO: USE CHEN's HEALTH LOSS CODE FOR FLOATS
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance to heal? (Default: 0.2)", AutoConfigFlags.PreventNetMismatch)]
        public float HealChance { get; private set; } = 0.2f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Base Heal Percent? 0.33", AutoConfigFlags.PreventNetMismatch)]
        public float HealAmount { get; private set; } = 0.33f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Stack Heal Percent? 0.11", AutoConfigFlags.PreventNetMismatch)]
        public float HealAmountStack { get; private set; } = 0.11f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Increase chance if damage is lethal?", AutoConfigFlags.PreventNetMismatch)]
        public bool LethalSave { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Lethal Save Chance. Default: 0.5f", AutoConfigFlags.PreventNetMismatch)]
        public float LethalSaveChance { get; private set; } = 0.5f;
        
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("If true, damage to shield and barrier (from e.g. Personal Shield Generator, Topaz Brooch) will not count towards triggering Enraging Photo")]
        public bool requireHealth { get; private set; } = true;
        public override string displayName => "Green Guon Stone";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Chance To Heal";

        protected override string GetDescString(string langid = null) => $"TODO";

        protected override string GetLoreString(string langID = null) => "The Green Guon stone abhors pain, and has a small chance to heal its bearer upon being wounded. It seems to grow more desperate as the risk of death rises.";

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
        private void On_HCTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo di)
        {
            var oldHealth = self.health;
            var oldCH = self.combinedHealth;

            orig(self, di);

            int icnt = GetCount(self.body);
            if (icnt < 1
                || (requireHealth && (oldHealth - self.health) / self.fullHealth < healthThreshold)
                || (!requireHealth && (oldCH - self.combinedHealth) / self.fullCombinedHealth < healthThreshold))
                return;

        }

        private void AddDamageReward(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.HasBuff(ROBEnraged)) { args.damageMultAdd += DmgBoost; }
        }
    }
}
*/