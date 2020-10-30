using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class RingFireResistance : Item_V2<RingFireResistance>
    {
        public override string displayName => "Ring of Fire Resistance";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "No Burns!\nPrevents damage from fire.";

        protected override string GetDescString(string langid = null) => $"Clears all stacks of Fire on the user upon taking damage. Prevents fire damage from inflicting.";

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
            else
            {
                if (self.body.HasBuff(BuffIndex.OnFire))
                {
                    self.body.RemoveBuff(BuffIndex.OnFire);
                }
                switch (damageInfo.damageType)
                {
                    case DamageType.IgniteOnHit:
                    case DamageType.PercentIgniteOnHit:
                        damageInfo.damageType = DamageType.Generic;
                        break;
                    default:
                        break;
                }
            }
            orig(self, damageInfo);
        }
            private void GiveRandomRed(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) //ripped from harbcrate, i did credit though.
        {
            orig(self);
            var amount = GetCount(self);
            if (amount > 1)
            {
                self.inventory.RemoveItem(catalogIndex);
                PickupIndex loot = Run.instance.treasureRng.NextElementUniform(Run.instance.availableTier3DropList);
                if (self.isPlayerControlled)
                    PickupDropletController.CreatePickupDroplet(loot, self.corePosition, Vector3.up * 5);
                else
                {
                    PickupDef def = PickupCatalog.GetPickupDef(loot);
                    self.inventory.GiveItem(def.itemIndex);
                    var lootCount = self.inventory.GetItemCount(def.itemIndex);
                    Chat.AddPickupMessage(self, def.nameToken, ColorCatalog.GetColor(ItemCatalog.GetItemDef(def.itemIndex).colorIndex), (uint)lootCount);
                }
            }
        }
    }
}
