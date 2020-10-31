/*
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;


namespace RiskOfBulletstorm.Items
{
    public class LaserSight : Item_V2<LaserSight>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many damage should [+1 Bullets] provide with a single stack? (Default: 0.1 = 10% dmg)", AutoConfigFlags.PreventNetMismatch)]
        public float ShotSpreadReduce { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional damage should each [+1 Bullets] after the first give? (Default: 0.01 = 1% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float ShotSpreadReducesStack { get; private set; } = 0.01f;

        public override string displayName => "Laser Scope";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "+1 To Bullet.\nMasterwork bullets deal more damage.";

        protected override string GetDescString(string langid = null) => $"I.";

        protected override string GetLoreString(string langID = null) => "Masterwork bullets.\n\nPeer-reviewed studies have shown that these bullets are precisely 1 better than normal bullets.";

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
            { }
        }
    }
}
*/