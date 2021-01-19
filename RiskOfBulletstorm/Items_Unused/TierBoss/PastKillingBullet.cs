/*
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
using System;
using static RiskOfBulletstorm.Shared.HelperUtil;
using GenericNotification = On.RoR2.UI.GenericNotification;

namespace RiskOfBulletstorm.Items
{
    public class PastKillingBullet : Item_V2<PastKillingBullet>
    {
        public override string displayName => "Bullet That Can Kill The Past";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Don't Miss";

        protected override string GetDescString(string langid = null) => $"A bullet that can kill the past. You're not sure what will happen when you fire it, but you feel exhilarated!";

        protected override string GetLoreString(string langID = null) => "";

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
            On.RoR2.GenericPickupController.OnTriggerStay += PreventAutoPickup;
            On.RoR2.PickupDropletController.CreatePickupDroplet += OnlyOnePickup;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GenericPickupController.OnTriggerStay -= PreventAutoPickup;
            On.RoR2.PickupDropletController.CreatePickupDroplet -= OnlyOnePickup;
        }
        private void PreventAutoPickup(On.RoR2.GenericPickupController.orig_OnTriggerStay orig, GenericPickupController self, Collider other)
        {
            if (self.pickupIndex != PickupCatalog.FindPickupIndex(catalogIndex))
                orig(self, other);
            else
                orig(self, null);
        }

        private void OnlyOnePickup(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            //Prevent multiple copies of the bullet being dropped into the world//

            int TeamInvCount = GetPlayersItemCount(catalogIndex);
            PickupIndex loot = Run.instance.treasureRng.NextElementUniform(Run.instance.availableBossDropList);
            PickupDef def = PickupCatalog.GetPickupDef(loot);

            if (pickupIndex == PickupCatalog.FindPickupIndex(catalogIndex)) //if it's the RealBullet
            {
                if (TeamInvCount > 1) //and someone already has the REAL bullet
                {
                    pickupIndex = PickupCatalog.FindPickupIndex(def.itemIndex); //change it to something else, we only want one in an inv
                }
            }
            orig(pickupIndex, position, velocity);
        }
    }
}
*/