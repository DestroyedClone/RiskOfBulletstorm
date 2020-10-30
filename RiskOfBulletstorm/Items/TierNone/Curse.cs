using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;


namespace RiskOfBulletstorm.Items
{
    public class Curse : Item_V2<Curse>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many curse is needed before Lord of the Jammed spawns? Set it to 1.11 to disable. (Default: 10)", AutoConfigFlags.PreventNetMismatch)]
        public float CurseMax { get; private set; } = 10f; //THIS IS WHERE I LEFT OFF!!!!!

        public override string displayName => "Curse";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject ItemBodyModelPrefab;

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
            GetStatCoefficients += AddDamage;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients -= AddDamage;
        }
        private void AddDamage(CharacterBody sender, StatHookEventArgs args)
        {
            var invCount = GetCount(sender);
            if (invCount > 0)
            { args.damageMultAdd += DamageBonus + DamageBonusStack * (invCount - 1); }
        }
    }
}
