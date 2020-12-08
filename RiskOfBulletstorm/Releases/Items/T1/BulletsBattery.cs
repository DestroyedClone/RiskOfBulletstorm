//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;

namespace RiskOfBulletstorm.Items
{
    public class BulletsBattery : Item_V2<BulletsBattery>
    {
        public override string displayName => "Battery Bullets";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Zap!\nElectrifies all bullets fired. Increases accuracy.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Electrifies bulletstorm-specific water for 3 seconds (+1s per stack)";
            desc += "Decreases shot spread by 5%";

            return desc;
        }

        protected override string GetLoreString(string langID = null) => "The shock troops of the Hegemony of Man consist entirely of heartless machines, shielded from voltage attacks. Their victims however, have learned to fear the inevitable sting of their electrified shells.";

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
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileManager_FireProjectile_FireProjectileInfo;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= ProjectileManager_FireProjectile_FireProjectileInfo;
        }
        private void ProjectileManager_FireProjectile_FireProjectileInfo(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, RoR2.Projectile.ProjectileManager self, RoR2.Projectile.FireProjectileInfo fireProjectileInfo)
        {
            if (fireProjectileInfo.owner && fireProjectileInfo.owner.GetComponent<CharacterBody>() && fireProjectileInfo.owner.GetComponent<CharacterBody>().inventory)
            {
                var electric = fireProjectileInfo.projectilePrefab.AddComponent<Bulletstorm_Electrify>();
                var proj = fireProjectileInfo.projectilePrefab;
                var projdmg = proj.gameObject.GetComponent<ProjectileDamage>();
                var invcount = fireProjectileInfo.owner.GetComponent<CharacterBody>().inventory.GetItemCount(catalogIndex);
                if (projdmg && invcount > 0)
                {
                    if (Util.CheckRoll(5f * invcount))
                        projdmg.damageType = DamageType
                }
            }
            orig(self, fireProjectileInfo);
        }
        private class Bulletstorm_Electrify : MonoBehaviour
        {

        }
    }
}
