using RoR2;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Ration : Equipment_V2<Ration>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of maximum health should the Ration heal? ? (Default: 0.4 = 40% of max health)", AutoConfigFlags.PreventNetMismatch)]
        public float Ration_HealAmount { get; private set; } = 0.4f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the Ration be consumed to save the holder from death? (Default: true)", AutoConfigFlags.PreventNetMismatch)]
        public bool Ration_SaveFromDeath { get; private set; } = true;

        public override float cooldown { get; protected set; } = 0f;

        public override string displayName => "Ration";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            var desc = "Calories, Mate\n";
            if (Ration_HealAmount > 0)
            {
                desc += "Provides healing on use. ";
                if (Ration_SaveFromDeath) desc += "If equipped, will be used automatically upon fatal damage.";
            }
            else return "Someone ate this before you did.";
            return desc;
        }

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Throws away this empty Ration.";
            if (Ration_HealAmount > 0)
            {
                desc = $"Heals for <style=cIsHealing>{Pct(Ration_HealAmount)} health.</style>";
                if (Ration_SaveFromDeath)
                    desc += $"\n<style=cIsUtility>Automatically used upon fatal damage. " +
                            $"\n</style><style=cDeath>One-Time Use.</style>";
            }
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "This MRE comes in the form of a dry and dense cookie. It doesn't taste great, but it delivers the calories the body needs.";
        public Ration()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Ration.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/RationIcon.png";
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
            On.RoR2.HealthComponent.TakeDamage += TankHit;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= TankHit;
        }

        private void TankHit(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (Ration_SaveFromDeath)
            {
                var body = self.body;
                if (body)
                {
                    var inventory = body.inventory;
                    if (inventory)
                    {
                        if (inventory.GetEquipmentIndex() == catalogIndex)
                        {
                            var endHealth = self.combinedHealth - damageInfo.damage;
                            if ((endHealth <= 0) && (!damageInfo.rejected))
                            {
                                damageInfo.rejected = true;
                                RationUse(self, inventory);
                            }
                        }
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void RationUse(HealthComponent health, Inventory inventory)
        {
            if (Ration_HealAmount > 0)
            {
                health.HealFraction(Ration_HealAmount, default);
                if (ClassicItemsCompat.enabled && ClassicItemsCompat.CheckEmbryoProc(instance, health.body)) health.HealFraction(Ration_HealAmount, default);
            }
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
