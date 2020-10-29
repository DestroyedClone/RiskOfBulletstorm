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
    public class RingFireResistance : Item_V2<RingFireResistance>
    {
        public override string displayName => "Ring of Fire Resistance";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "No Burns!\nPrevents damage from fire.";

        protected override string GetDescString(string langid = null) => $"Clears all stacks of Fire on the user upon taking damage.";

        protected override string GetLoreString(string langID = null) => "A ring originally worn by the legendary gunsmith himself. Later in life, Edwin no longer needed it, but the ring proved indispensable during his early years in the Forge. It eventually passed to his eldest daughter.";

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
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= ClearFire;
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
