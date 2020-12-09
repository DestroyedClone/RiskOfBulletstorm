using System.Collections.ObjectModel;
using RoR2;
using TILER2;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.Shared.Blanks.MasterBlankItem;


namespace RiskOfBulletstorm.Items
{
    public class Armor : Item_V2<Armor>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should Armor activate a blank when broken?", AutoConfigFlags.PreventNetMismatch)]
        public bool Armor_ActivateBlank { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the minimum amount of damage that Armor should be consumed for? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Armor_HealthThreshold { get; private set; } = 0.20f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should Armor protect against fatal damage? This takes priority over the required minimum amount of damage.")]
        public bool Armor_ProtectDeath { get; private set; } = false;

        public override string displayName => "Armor";
        public string descText = "Prevents a single hit";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Protect Body\n"+descText+" from heavy hits";

        protected override string GetDescString(string langid = null)
        {
            string descString = $"<style=cIsUtility>{descText}</style> that would have exceeded <style=cIsDamage>{Pct(Armor_HealthThreshold)} health</style>";
            if (Armor_ActivateBlank) descString += $"\nand <style=cIsUtility>activates a Blank</style>";
            if (Armor_ProtectDeath) descString += $"\n </style=cIsUtility>Also protects from death.</style>" +
                    $"\nConsumed on use.";
            return descString;
        }

        protected override string GetLoreString(string langID = null) => "The blue of this shield was formed from the shavings of a Blank." +
            "Dents into the weak, aluminium metal from bullets and projectiles trigger the power of the Blank.";

        public Armor()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Armor.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/ArmorIcon.png";
        }

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
            On.RoR2.HealthComponent.TakeDamage += TankHit;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= TankHit;
        }
        private void TankHit(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var InventoryCount = GetCount(self.body);
            var health = self.combinedHealth;
            var endHealth = health - damageInfo.damage;

            if (InventoryCount > 0)
            {
                if (
                    (
                        (Armor_ProtectDeath && endHealth <= 0 ) || // If it protects from death and you would have died, *OR*
                        (endHealth / self.fullHealth >= Armor_HealthThreshold) ) &&  // the damage dealt exceeds armor threshold
                        (!damageInfo.rejected) //and its not rejected
                    )
                {
                    damageInfo.rejected = true;
                    self.body.inventory.RemoveItem(catalogIndex);

                    if (Armor_ActivateBlank) FireBlank(self.body, self.body.corePosition, 6f, 1f, -1);
                }
            }
            orig(self, damageInfo);
        }
    }
}
