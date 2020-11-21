using EntityStates.Treebot.Weapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;
using ThinkInvisible.ClassicItems;

namespace RiskOfBulletstorm.Items
{
    public class Ration : Equipment_V2<Ration>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Heal%? (Default: 0.4 = 40% heal)", AutoConfigFlags.PreventNetMismatch)]
        public float Ration_HealAmount { get; private set; } = 0.4f;

        public override string displayName => "Ration";
        public override float cooldown { get; protected set; } = 1f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Calories, Mate\nProvides healing on use. If equipped, will be used automatically on death.";

        protected override string GetDescString(string langid = null) => $"<style=cIsHealing>{Pct(Ration_HealAmount)} heal.</style>" +
            $"\n<style=cDeath>One-Time Use.</style> <style=cIsUtility>Automatically used upon fatal damage.</style>";

        protected override string GetLoreString(string langID = null) => "This MRE comes in the form of a dry and dense cookie. It doesn't taste great, but it delivers the calories the body needs.";

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
            On.RoR2.HealthComponent.TakeDamage += TankHit;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= TankHit;
        }

        private void TankHit(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var combinedHealth = self.combinedHealth;
            var endHealth = combinedHealth - damageInfo.damage;
            var body = self.body;
            var inventory = body.inventory;

            if (inventory.GetEquipmentIndex() == catalogIndex)
            {
                if ((endHealth <= 0) && (!damageInfo.rejected))
                {
                    damageInfo.rejected = true;

                    RationUse(self, inventory);
                }
            }
            orig(self, damageInfo);
        }

        private void RationUse(HealthComponent health, Inventory inventory)
        {
            health.HealFraction(Ration_HealAmount, default);
            if (instance.CheckEmbryoProc(health.body)) health.HealFraction(Ration_HealAmount, default);
            inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;
            Inventory inventory = body.inventory;
            if (!inventory) return false;

            RationUse(health, inventory);

            return false;
        }
    }
}
