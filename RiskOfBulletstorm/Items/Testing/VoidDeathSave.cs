
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class VoidDeathSave : Item_V2<VoidDeathSave>
    {
        public override string displayName => "Void Altar";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Prevents being sentenced to Gay Baby Jail, destroys upon use.";

        protected override string GetDescString(string langid = null) => $"Getting caught in a Void Reaver nullification field " +
            $"\n Replaced with a random red if an extra is picked up.";

        protected override string GetLoreString(string langID = null) => "A crystallized Void Fragment in the shape of a small human. Confuses Void Reavers to the point they can't distinguish between this and flesh..";

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
            On.RoR2.HealthComponent.TakeDamage += GetInvCount;
            On.RoR2.HealthComponent.Suicide += PreventSuicide;
            //On.RoR2.CharacterBody.OnInventoryChanged += GiveRandomRed;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= GetInvCount;
            On.RoR2.HealthComponent.Suicide -= PreventSuicide;
            //On.RoR2.CharacterBody.OnInventoryChanged -= GiveRandomRed;
        }
        private void GetInvCount(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            //var InventoryCount = GetCount(self.body);

            /*
            if (InventoryCount > 0)
            {
                if (damageInfo.damageType == DamageType.VoidDeath)
                {
                    damageInfo.damageType = DamageType.Generic;
                    self.body.inventory.GiveItem(ItemIndex.ExtraLife);
                    self.body.inventory.RemoveItem(catalogIndex);
                }
            }*/
            orig(self, damageInfo);
        }
        private void PreventSuicide(On.RoR2.HealthComponent.orig_Suicide orig, HealthComponent self, GameObject killerOverride, GameObject inflictorOverride, DamageType damageType)
        {
            var InventoryCount = GetCount(self.body);

            if (InventoryCount < 1)
            {
                orig(self, killerOverride, inflictorOverride, damageType);
                Chat.AddMessage("No void save, followed suicide.");
            } else
            {
                Chat.AddMessage("Void Death saved");
                return;
            }
        }
        /*private void GiveRandomRed(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) //ripped from harbcrate, i did credit though.
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
        }*/
    }
}
