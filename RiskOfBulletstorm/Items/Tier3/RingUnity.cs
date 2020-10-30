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
using UnityEngine.Networking.NetworkSystem;

namespace RiskOfBulletstorm.Items
{
    public class Unity : Item_V2<Unity>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage increase on single stack? (Default: 0.1)" +
            "\nKeep in mind that this number is MULTIPLIED by the amount of TOTAL items.",
            AutoConfigFlags.PreventNetMismatch)]
        public float DamageBonus { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage increase on subsequent stacks (Default: 0.01)" +
            "\nKeep in mind that this number is MULTIPLIED by the amount of TOTAL items.", AutoConfigFlags.PreventNetMismatch)]
        public float DamageBonusStack { get; private set; } = 0.05f;
        public override string displayName => "Unity";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Our Powers Combined\nIncreased combat effectiveness per item.";

        protected override string GetDescString(string langid = null) => $"+{DamageBonus} ({DamageBonusStack} per stack) damage per unique item in inventory";

        protected override string GetLoreString(string langID = null) => "This ring takes a small amount of power from each gun carried and adds it to the currently equipped gun.";

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
            On.RoR2.CharacterBody.OnInventoryChanged += GiveUnityBonus;
            GetStatCoefficients += BoostDamage;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= GiveUnityBonus;
            GetStatCoefficients -= BoostDamage;
        }
        private void GiveUnityBonus(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) 
        {
            orig(self);
            var amount = GetCount(self);
            var inv = self.inventory;
            var tier1Items = RoR2.Inventory;
            Inventory.Get
            if (amount < 1) { return; }
            else
            {
                
            }
        }
        private void BoostHealth(CharacterBody sender, StatHookEventArgs args)
        {
            args.baseDamageAdd += TotalItemCount;
        }
    }
}
