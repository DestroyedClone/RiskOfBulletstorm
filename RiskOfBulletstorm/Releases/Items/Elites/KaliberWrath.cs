
using EliteSpawningOverhaul;
using R2API;
using RoR2;
using TILER2;
using UnityEngine;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm //TODO modify weight of card depending on curse level.
{
    public class KaliberWrath : Equipment_V2<KaliberWrath>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Enable Jammed enemies?", AutoConfigFlags.PreventNetMismatch)]
        public bool KaliberWrath_Enable { get; private set; } = false;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional damage should a Jammed enemy deal?(Default: 2 (+200%))", AutoConfigFlags.PreventNetMismatch)]
        public float KaliberWrath_DamageBoost { get; private set; } = 2.00f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional crit should a Jammed enemy have?(Default: 1 (+100%))", AutoConfigFlags.PreventNetMismatch)]
        public float KaliberWrath_CritBoost { get; private set; } = 1f;
        public override string displayName => "Wrath of Kaliber";

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Become an aspect of defiance";

        protected override string GetDescString(string langid = null)
        {
            string desc = $"+{Pct(KaliberWrath_DamageBoost)} damage dealt. +{Pct(KaliberWrath_CritBoost)} crit bonus.";
            if (KaliberWrath_Enable) desc = "DISABLED";
            return desc;
        }

        //protected override string GetDescString(string langid = null) => $"+{Pct(KaliberWrath_DamageBoost)} damage dealt. +{KaliberWrath_CritBoost} crit bonus.";
        protected override string GetLoreString(string langID = null) => "Several quotes from what seems to be a form of holy scripture were found around the Gungeon." +
            $"They reference the Gungeon's past, and made prophecies about its future." +
            $"\nDue to the references to Kaliber that can be found in the texts, this book is most likely hers, however, it is incomplete." +
            $"\nGunesis 1:1 In the beginning, the Gungeon was formless and void, and bullets moved over the face of the deep." +
            $"\nRevolvations 2:1 These are the words of Kaliber, she who grips the seven sidearms in her hands and walks among the six loaded chambers." +
            $"\n\nWhen the Gun is drawn to the heart of the pacifist, and when the chambers are empty and the shells are spent, the Last of the Jammed will ascend to seal The Breach forever.";

        public static EliteAffixCard JammedEliteCard { get; set; }
        public static EliteIndex JammedEliteIndex;
        public static BuffIndex JammedBuffIndex;

        public override void SetupAttributes()
        {
            base.SetupAttributes();

            if (KaliberWrath_Enable)
            {
                equipmentDef.canDrop = true;
                equipmentDef.enigmaCompatible = false;

                var jammedEliteDef = new CustomElite(
                new EliteDef
                {
                    name = "Jammed",
                    modifierToken = "ROB_ELITE_MODIFIER_JAMMED",
                    color = new Color32(150, 10, 10, 255),
                    eliteEquipmentIndex = equipmentDef.equipmentIndex
                }, 1);
                JammedEliteIndex = EliteAPI.Add(jammedEliteDef);
                LanguageAPI.Add(jammedEliteDef.EliteDef.modifierToken, "Jammed {0}");

                var jammedBuffDef = new CustomBuff(
                new BuffDef
                {
                    name = "Affix_Jammed",
                    buffColor = new Color32(150, 10, 10, 255),
                    iconPath = "",
                    eliteIndex = JammedEliteIndex,
                    canStack = false
                });
                JammedBuffIndex = BuffAPI.Add(jammedBuffDef);
                equipmentDef.passiveBuff = JammedBuffIndex;

                JammedEliteCard = new EliteAffixCard
                {
                    spawnWeight = 0.8f,
                    costMultiplier = 15.0f,
                    damageBoostCoeff = 1.0f,
                    healthBoostCoeff = 4.0f,
                    eliteOnlyScaling = 0.5f,
                    eliteType = JammedEliteIndex,
                };

                EsoLib.Cards.Add(JammedEliteCard);
            }
        }
        public override void Install()
        {
            base.Install();
            if (KaliberWrath_Enable)
                GetStatCoefficients += KaliberReward;

        }
        public override void Uninstall()
        {
            base.Uninstall();
            if (KaliberWrath_Enable)
                GetStatCoefficients -= KaliberReward;
        }
        private void KaliberReward(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.HasBuff(JammedBuffIndex))
            {
                args.damageMultAdd += KaliberWrath_DamageBoost;
                args.critAdd += KaliberWrath_CritBoost;
            }
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            return false;
        }
    }
}
