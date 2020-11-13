//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using GenericNotification = On.RoR2.UI.GenericNotification;
using System;

namespace RiskOfBulletstorm.Items
{
    public class Spice : Item_V2<Spice> //Change to equipment that gives cursed?
    {
        public override string displayName => "Spice";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;

        private string dynamicPickupText = "THE CUBE";
        private float SpiceReplaceChance = 0f;
        protected override string GetPickupString(string langID = null)
        {
            return dynamicPickupText;
        }

        protected override string GetDescString(string langid = null) => $"Provides a bonus to your stats ...so there's no harm in taking more, right?";

        protected override string GetLoreString(string langID = null) => "A potent gun-enhancing drug from the far reaches of the galaxy. It is known to be extremely addictive, and extremely expensive.";
        public Spice()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Spice.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Spice.png";
        }
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
            On.RoR2.PickupDropletController.CreatePickupDroplet += PickupDropletController_CreatePickupDroplet;
            GenericNotification.SetItem += SetNotificationItemHook;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            //On.RoR2.HealthComponent.TakeDamage -= CalculateSpiceReward;
            GetStatCoefficients -= GiveSpiceReward;
            On.RoR2.CharacterBody.OnInventoryChanged -= CalculateSpiceReward;
            On.RoR2.PickupDropletController.CreatePickupDroplet -= PickupDropletController_CreatePickupDroplet;
        }
        private void SetNotificationItemHook(GenericNotification.orig_SetItem orig, RoR2.UI.GenericNotification self, ItemDef itemDef)
        {
            //self.descriptionText.token = itemDef.descriptionToken;
            orig(self, itemDef);
            if (itemDef.itemIndex == catalogIndex)
            { self.descriptionText.token = dynamicPickupText; }
        }
        private void CalculateSpiceReward(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) //blessed komrade
        {
            int InventoryCount = GetCount(self);
            SpiceReplaceChance = Math.Min(InventoryCount * 5, 100);
            switch (InventoryCount)
            {
                case 0:
                    break;
                case 1:
                    dynamicPickupText = "A tantalizing cube of power.";
                    //self.inventory.GiveItem(Spice_RewardA.instance.catalogIndex);
                    //self.inventory.GiveItem(ItemIndex.Hoof);
                    //GiveItemVsMax(self, Spice_RewardA.instance.catalogIndex);
                    break;
                case 2:
                    dynamicPickupText = "One more couldn't hurt.";
                    //GiveItemVsMax(self, ItemIndex.Feather);
                    break;
                case 3:
                    dynamicPickupText = "Just one more hit...";
                    //GiveItemVsMax(self, ItemIndex.AlienHead);
                    break;
                case 4:
                    dynamicPickupText = "MORE";
                    //GiveItemVsMax(self, ItemIndex.LunarBadLuck);
                    break;
                default:
                    dynamicPickupText = "MORE";
                    break;
            }
            //protected override string GetPickupString = dynamicPickupText;
            orig(self);
        }
        private void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            var body = PlayerCharacterMasterController.instances[0].master.GetBody();
            if (Util.CheckRoll(SpiceReplaceChance, body.master))
            {
                if (pickupIndex != PickupCatalog.FindPickupIndex(ItemIndex.ArtifactKey)) //safety to prevent softlocks
                    pickupIndex = PickupCatalog.FindPickupIndex(catalogIndex);
            }
            orig(pickupIndex, position, velocity);
        }
        private void GiveSpiceReward(CharacterBody sender, StatHookEventArgs args)
        {
            int InventoryCount = GetCount(sender);
            args.baseDamageAdd += InventoryCount;
            switch (InventoryCount)
            {
                case 0:
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }
        /*private static readonly Dictionary<ItemIndex, int> SpiceBonusItems = new Dictionary<ItemIndex, int>
        {
            { ItemIndex.AACannon, 0 },
            { ItemIndex.AACannon, 1 },
            { ItemIndex.AACannon, 2 },
            { ItemIndex.AACannon, 3 },
        };*/
        private void SpiceGiveItem(CharacterBody sender, ItemIndex itemIndex, int requiredAmount)
        {
            var InventoryCount = sender.inventory.GetItemCount(itemIndex) ;
            if (InventoryCount == requiredAmount)
            {

            } else
            {

            }
        }
    }
}
