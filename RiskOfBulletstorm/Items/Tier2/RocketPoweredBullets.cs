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
using RoR2.Projectile;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;

namespace RiskOfBulletstorm.Items
{
    public class RocketPoweredBullets : Item_V2<RocketPoweredBullets> //Change to equipment that gives cursed.
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Projectile Speed Multiplier? (Default: 0.5 = +50% projectile speed)", AutoConfigFlags.PreventNetMismatch)]
        public float ProjSpeedMult { get; private set; } = 0.5f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Projectile Speed Multiplier per stack? (Default: 0.05 = +5% projectile speed)", AutoConfigFlags.PreventNetMismatch)]
        public float ProjSpeedMultStack { get; private set; } = 0.05f;
        public override string displayName => "Rocket-Powered Bullets";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Faster Projectiles\nIncreased projectile speed and power.";

        protected override string GetDescString(string langid = null) => $"Increases projectile speed by {Pct(ProjSpeedMult)}" +
            $"\n(+{Pct(ProjSpeedMultStack)} per stack)";

        protected override string GetLoreString(string langID = null) => "Known for her impatience, Cadence grew tired of waiting for her bullets to reach their target. She developed these tiny rockets to give each shot extra speed.";

        public int InventoryCount = 0;

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
            On.RoR2.CharacterBody.OnInventoryChanged += UpdateInvCount;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileManager_FireProjectile_FireProjectileInfo;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= UpdateInvCount;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= ProjectileManager_FireProjectile_FireProjectileInfo;
        }
        private void UpdateInvCount(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            InventoryCount = GetCount(self);
            orig(self);
        }
        private void ProjectileManager_FireProjectile_FireProjectileInfo(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo fireProjectileInfo)
        {
            Chat.AddMessage("RocketPoweredBullets: Entered Hook");
            if (InventoryCount > 0)
            {
                var ProjMultFinal = 1 + ProjSpeedMult + ProjSpeedMultStack * (InventoryCount - 1);
                //RocketBulletComponent RocketBulletComponent = self.GetComponent<RocketBulletComponent>();
                //if (!RocketBulletComponent) { Chat.AddMessage("No bullet component found?"); }
                Chat.AddMessage("RocketPoweredBullets: Current Speed Override: " + fireProjectileInfo.speedOverride.ToString() + " x (" + ProjMultFinal.ToString() + ") = (" + (fireProjectileInfo.speedOverride * ProjMultFinal).ToString() + ")");
                fireProjectileInfo.speedOverride *= ProjMultFinal;
                fireProjectileInfo.useSpeedOverride = true;
            }
            orig(self, fireProjectileInfo);
        }
        public class RocketBulletComponent : MonoBehaviour
        {
            public float RocketBulletMultiplier;
        }

    }
}
