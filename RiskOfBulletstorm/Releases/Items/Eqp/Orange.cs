using EntityStates.Treebot.Weapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;
using ThinkInvisible.ClassicItems;
using RiskOfBulletstorm;

namespace RiskOfBulletstorm.Items
{
    public class Orange : Equipment_V2<Orange>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Rarity? (Default: 0.8 = 80% chance to spawn)", AutoConfigFlags.PreventNetMismatch)]
        public float Orange_Rarity { get; private set; } = 0.8f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        //public float Cooldown_config { get; private set; } = 8f;

        public override string displayName => "Orange";
        public override float cooldown { get; protected set; } = 1f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "You're Not Alexander\nWith this orange, your style... it's impetuous. Your defense, impregnable.";

        protected override string GetDescString(string langid = null) => $"<style=cIsHealing>100% heal.</style> <style=cIsUtility>Permanently increases health by 10% and reduces equipment recharge rate by 10%</style>" +
            $"\n<style=cDeath>One-time Use.</style> <style=cWorldEvent>{Pct(Orange_Rarity)} rarer.</style> ";

        protected override string GetLoreString(string langID = null) => "";

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            Embryo_V2.instance.Compat_Register(catalogIndex);
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
                    Chat.AddMessage("Orange was re-rolled!");
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

            int DeployCount = instance.CheckEmbryoProc(body) ? 2 : 1; //Embryo Check

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
