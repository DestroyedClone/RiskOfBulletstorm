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
        public override string displayName => "Unity";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Our Powers Combined\nIncreased combat effectiveness per item.";

        protected override string GetDescString(string langid = null) => $"Clears all stacks of Fire on the user upon taking damage.";

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
            On.RoR2.HealthComponent.TakeDamage += ClearFire;
            On.RoR2.CharacterBody.OnInventoryChanged += GiveRandomRed;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= ClearFire;
            On.RoR2.CharacterBody.OnInventoryChanged -= GiveRandomRed;
        }
        private void ClearFire(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            var InventoryCount = GetCount(self.body);

            if (InventoryCount < 1)
                return;

            if (InventoryCount > 0 && self.body.HasBuff(BuffIndex.OnFire))
            {
                self.body.RemoveBuff(BuffIndex.OnFire);
            }
            orig(self, damageInfo);
        }
    }
}
