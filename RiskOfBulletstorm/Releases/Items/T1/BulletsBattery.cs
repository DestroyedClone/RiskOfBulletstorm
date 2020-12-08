using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using TILER2;

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
            var desc = $"5% chance per stack to add Shock5s to your attacks.";

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
            On.RoR2.BulletAttack.Fire += BulletAttack_Fire;
            On.RoR2.OverlapAttack.Fire += OverlapAttack_Fire;
            On.RoR2.BlastAttack.Fire += BlastAttack_Fire;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= ProjectileManager_FireProjectile_FireProjectileInfo;
            On.RoR2.BulletAttack.Fire -= BulletAttack_Fire;
            On.RoR2.OverlapAttack.Fire -= OverlapAttack_Fire;
            On.RoR2.BlastAttack.Fire -= BlastAttack_Fire;
        }

        private BlastAttack.Result BlastAttack_Fire(On.RoR2.BlastAttack.orig_Fire orig, BlastAttack self)
        {
            self.damageType = ShockRoll(self.attacker, self.damageType);
            return orig(self);
        }

        private DamageType ShockRoll(GameObject owner, DamageType baseDamageType)
        {
            if (owner && owner.GetComponent<CharacterBody>() && owner.GetComponent<CharacterBody>().inventory)
            {
                var invcount = owner.GetComponent<CharacterBody>().inventory.GetItemCount(catalogIndex);
                if (invcount > 0)
                    if (Util.CheckRoll(5f * invcount))
                        return baseDamageType |= DamageType.Shock5s;
            }
            return baseDamageType;
        }

        private bool OverlapAttack_Fire(On.RoR2.OverlapAttack.orig_Fire orig, OverlapAttack self, List<HealthComponent> hitResults)
        {
            self.damageType = ShockRoll(self.attacker, self.damageType);
            return orig(self, hitResults);
        }

        private void BulletAttack_Fire(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            self.damageType = ShockRoll(self.owner, self.damageType);
            orig(self);
        }

        private void ProjectileManager_FireProjectile_FireProjectileInfo(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, RoR2.Projectile.ProjectileManager self, RoR2.Projectile.FireProjectileInfo fireProjectileInfo)
        {
            var component = fireProjectileInfo.projectilePrefab.GetComponent<ProjectileDamage>();
            component.damageType = ShockRoll(fireProjectileInfo.owner, component.damageType);
            orig(self, fireProjectileInfo);
        }
        private class BulletstormShockEffect : MonoBehaviour
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnDisable()
            {
                EffectManager.SimpleEffect(Resources.Load<GameObject>("Prefabs/TemporaryVisualEffects/TeslaFieldBuffEffect"), gameObject.transform.position, gameObject.transform.rotation, true);
            }
        }
    }
}
