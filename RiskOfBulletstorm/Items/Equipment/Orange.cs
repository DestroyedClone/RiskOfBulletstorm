using EntityStates.Treebot.Weapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Orange : Equipment_V2<Orange>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Rarity? (Default: 0.45 = 45% chance to spawn)", AutoConfigFlags.PreventNetMismatch)]
        public float Rarity { get; private set; } = 0.45f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        //public float Cooldown_config { get; private set; } = 8f;

        public override string displayName => "Orange";
        public override float cooldown { get; protected set; } = 1f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "You're Not Alexander\nWith this orange, your style... it's impetuous. Your defense, impregnable.";

        protected override string GetDescString(string langid = null) => $"100% heal. One-time Use. 50% rarer. Permanently reduces cooldown and recharge rate.";

        protected override string GetLoreString(string langID = null) => "God Hand reference";

        public override void SetupBehavior()
        {
            base.SetupBehavior();

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
            On.RoR2.PickupDropletController.CreatePickupDroplet += PickupDropletController_CreatePickupDroplet;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PickupDropletController.CreatePickupDroplet -= PickupDropletController_CreatePickupDroplet;
        }
        private void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            var body = PlayerCharacterMasterController.instances[0].master.GetBody();
            if (pickupIndex == PickupCatalog.FindPickupIndex(catalogIndex)) //if it's the orange
            {
                if (!Util.CheckRoll(Rarity, body.master)) //rarity roll
                {
                    PickupIndex loot = Run.instance.treasureRng.NextElementUniform(Run.instance.availableEquipmentDropList);
                    PickupDef def = PickupCatalog.GetPickupDef(loot);
                    pickupIndex = PickupCatalog.FindPickupIndex(def.itemIndex);
                }
            }
            orig(pickupIndex, position, velocity);
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;
            Inventory inventory = body.inventory;
            if (!inventory) return false;

            inventory.GiveItem(ItemIndex.Infusion);
            inventory.GiveItem(ItemIndex.AlienHead);
            health.HealFraction(1, default);
            inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico

            return false;
        }
    }
}
