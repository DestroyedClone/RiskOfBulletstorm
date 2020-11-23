//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;

namespace RiskOfBulletstorm.Items
{
    public class GuonStoneGlass : Item_V2<GuonStoneGlass>
    {
        public override string displayName => "Glass Guon Stone";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Fleeting Defense\nBlocks projectiles, but shatters if its owner is wounded.";

        protected override string GetDescString(string langid = null) => $"Each guon stone <style=cIsUtility>blocks a projectile or bullet</style>" +
            $"\nIf you get hit, <style=cDeath>all stacks shatter</style>.";

        protected override string GetLoreString(string langID = null) => "A gift from the Lady of Pane.\n\nGungeoneers who say a prayer to this silly goddess receive her blessing of three Glass Guon Stones.";

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
            On.RoR2.HealthComponent.TakeDamage += OnHurt;
        }

        private void OnHurt(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            throw new NotImplementedException();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}
