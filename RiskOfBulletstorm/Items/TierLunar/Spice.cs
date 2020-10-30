﻿//using System;
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
    public class Spice : Item_V2<Spice> //Change to equipment that gives cursed.
    {
        public override string displayName => "Spice";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        public string dynamicPickupText = "THE CUBE";
        public float SpiceReplaceChance = 0f;
        protected override string GetPickupString(string langID = null)
        {
            return dynamicPickupText;
        }

        protected override string GetDescString(string langid = null) => $"Provides a bonus to your stats ...so there's no harm in taking more, right?";

        protected override string GetLoreString(string langID = null) => "A potent gun-enhancing drug from the far reaches of the galaxy. It is known to be extremely addictive, and extremely expensive.";

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
            //On.RoR2.HealthComponent.TakeDamage += CalculateSpiceReward;
            GetStatCoefficients += GiveSpiceReward;
            On.RoR2.CharacterBody.OnInventoryChanged += CalculateSpiceReward;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            //On.RoR2.HealthComponent.TakeDamage -= CalculateSpiceReward;
            GetStatCoefficients -= GiveSpiceReward;
        }
        private void CalculateSpiceReward(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) //blessed komrade
        {
            var InventoryCount = GetCount(self);
            switch(InventoryCount)
            {
                case 0:
                    break;
                case 1:
                    SpiceReplaceChance += 0.1f;
                    dynamicPickupText = "A tantalizing cube of power.";
                    break;
                case 2:
                    SpiceReplaceChance += 0.1f;
                    dynamicPickupText = "One more couldn't hurt.";
                    break;
                case 3:
                    SpiceReplaceChance += 0.2f;
                    dynamicPickupText = "Just one more hit...";
                    break;
                case 4:
                    SpiceReplaceChance += 0.3f;
                    dynamicPickupText = "MORE";
                    break;
                default:
                    SpiceReplaceChance = Mathf.Min(SpiceReplaceChance + 0.05f,1.0f);
                    break;
            }
            orig(self);
        }
        private static void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, UnityEngine.Vector3 position, UnityEngine.Vector3 velocity)
        {
            if (pickupIndex == PickupCatalog.FindPickupIndex(ItemIndex.AlienHead))
            {
                pickupIndex = PickupCatalog.FindPickupIndex(ItemIndex.Hoof);
            }
            orig(pickupIndex, position, velocity);
        }
        private void GiveSpiceReward(CharacterBody sender, StatHookEventArgs args)
        {
        }
    }
}