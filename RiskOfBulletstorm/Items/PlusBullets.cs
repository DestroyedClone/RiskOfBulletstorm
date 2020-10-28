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
    public class PlusBullets : Item_V2<PlusBullets>
    {
        //TODO: USE CHEN's HEALTH LOSS CODE FOR FLOATS
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many damage should [+1 Bullets] provide with a single stack? (Default: 0.25 = 25% dmg)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageBonus { get; private set; } = 0.25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional damage should each [+1 Bullets] after the first give? (Default: 0.05 = 5% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageBonusStack { get; private set; } = 0.05f;

        public float damageReward;
        public override string displayName => "+1 Bullets";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "+1 To Bullet.";

        protected override string GetDescString(string langid = null) => $"Increases damage by <style=cIsDamage>{Pct(DamageBonus)}% damage</style>. <style=cIsDamage>+{Pct(DamageBonusStack)}% damage</style> per stack.";

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
            GetStatCoefficients -= AddDamage; //
        }
        private void AddDamage(CharacterBody sender, StatHookEventArgs args)
        {
            var invCount = GetCount(sender);
            if (invCount > 0)
            { args.damageMultAdd += DamageBonus + DamageBonusStack * invCount - 1; }
        }
    }
}
