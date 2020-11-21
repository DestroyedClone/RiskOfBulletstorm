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
using static RiskOfBulletstorm.Shared.BlankRelated;


namespace RiskOfBulletstorm.Items
{
    public class Armor : Item_V2<Armor>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Activate a blank when armor is depleted? (Default: true)", AutoConfigFlags.PreventNetMismatch)]
        public bool Armor_ActivateBlank { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Health Threshold for blocking damage. ", AutoConfigFlags.PreventNetMismatch)]
        public float Armor_HealthThreshold { get; private set; } = 0.20f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Protects from death?")]
        public bool Armor_ProtectDeath { get; private set; } = false;

        public override string displayName => "Armor";
        public string descText = "Prevents a single hit";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Protect Body\n"+descText+" from heavy hits";

        protected override string GetDescString(string langid = null)
        {
            string descString = $"<style=cIsUtility>{descText}</style> that would have exceeded <style=cIsDamage>{Pct(Armor_HealthThreshold)} health.</style>";
            if (Armor_ProtectDeath) descString += $"\n </style=cIsUtility>Also protects from death.</style>" +
                    $"\nConsumed on use.";
            return descString;
        }

        protected override string GetLoreString(string langID = null) => "The blue of this shield was formed from the shavings of a Blank." +
            "Dents into the weak, aluminium metal from bullets and projectiles trigger the power of the Blank.";


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
                        (Armor_ProtectDeath && endHealth <= 0 ) || 
                        (endHealth / self.fullHealth >= Armor_HealthThreshold) ) && 
                        (!damageInfo.rejected)
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
