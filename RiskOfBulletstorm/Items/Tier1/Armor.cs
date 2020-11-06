﻿
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
        public bool ActivateBlank { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Health Threshold for blocking damage. ", AutoConfigFlags.PreventNetMismatch)]
        public float HealthThreshold { get; private set; } = 0.25f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Protects from death?")]
        public bool RequireHealth { get; private set; } = false;

        public override string displayName => "Armor";
        public string descText = "Prevents a single hit";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Protect Body\n"+descText+" from heavy hits";

        protected override string GetDescString(string langid = null) => $"{descText} that would have exceeded {Pct(HealthThreshold)} health.\n If the shot would have killed you, spends itself.";

        protected override string GetLoreString(string langID = null) => "This sheet of metal is so fragile that it explodes upon the slightest scratch. Yet we still use it as armor. Go figure.";


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

            var oldHealth = self.health;
            orig(self, damageInfo);
            if (InventoryCount > 0)
            {
                if (((oldHealth - self.health) / self.fullHealth < HealthThreshold) || (RequireHealth && (oldHealth - damageInfo.damage <= 0)) && (!damageInfo.rejected) )
                {
                    damageInfo.damage = 0; //Remove if bloodshrines dont work
                    damageInfo.rejected = true;
                    self.body.inventory.RemoveItem(catalogIndex);

                    if (ActivateBlank)
                    {
                        Chat.AddMessage("Armor: Armor Broke, Firing blank!");
                        FireBlank(self.body.gameObject, self.body.corePosition, 6f, 0.3f, -1);
                    }
                }
            }
            //orig(self, damageInfo);
        }
    }
}
