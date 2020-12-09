using RoR2;
using UnityEngine;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class Orange : Equipment_V2<Orange>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the chance for the Orange to spawn? (Default: 80.00% chance to spawn)" +
            "\n((When it is chosen by the game, it does a roll. By default, it has a 80% not to get rerolled.))", AutoConfigFlags.PreventNetMismatch)]
        public float Orange_Rarity { get; private set; } = 80.00f;

        public override string displayName => "Orange";
        public override float cooldown { get; protected set; } = 0f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "You're Not Alexander\nPermanently increases stats upon consumption.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"<style=cIsHealing>Heals for 100% health.</style> <style=cIsHealth>Permanently increases max health by 10%</style> and <style=cIsUtility>reduces equipment recharge rate by 10%</style>"+
              $"\n<style=cDeath>One-time Use.</style>";
            if (Orange_Rarity < 100f)
                desc += $"<style=cWorldEvent>{Orange_Rarity}% chance to spawn.</style>";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "With this orange, your style... it's impetuous. Your defense, impregnable.";
        public Orange()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Orange.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/OrangeIcon.png";
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();

            if (ClassicItemsCompat.enabled)
                ClassicItemsCompat.RegisterEmbryo(catalogIndex);
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
                if (!Util.CheckRoll(Orange_Rarity, body.master)) //rarity roll
                {
                    PickupIndex loot = Run.instance.treasureRng.NextElementUniform(Run.instance.availableEquipmentDropList);
                    PickupDef def = PickupCatalog.GetPickupDef(loot);
                    pickupIndex = PickupCatalog.FindPickupIndex(def.equipmentIndex);
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

            int DeployCount = ClassicItemsCompat.enabled && ClassicItemsCompat.CheckEmbryoProc(instance, body) ? 2 : 1; //Embryo Check

            for (int i = 0; i < DeployCount; i++)
            {
                inventory.GiveItem(ItemIndex.BoostHp);
                inventory.GiveItem(ItemIndex.BoostEquipmentRecharge);
                health.HealFraction(1, default);
            }
            inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico

            return false;
        }
    }
}
