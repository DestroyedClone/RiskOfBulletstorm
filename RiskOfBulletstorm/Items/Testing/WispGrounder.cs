//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using TILER2;
using System;
using static RiskOfBulletstorm.Shared.HelperUtil;
using RoR2.Projectile;
using UnityEngine;
using EntityStates.GolemMonster;

namespace RiskOfBulletstorm.Items
{
    public class WispFiresProjectiles : Item_V2<WispFiresProjectiles>
    {
        public override string displayName => "Wisp Speculum";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Forces wisps to fire projectiles instead of bullets";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public static GameObject BombPrefab { get; private set; }

        public override void SetupBehavior()
        {
            GameObject engiMinePrefab = Resources.Load<GameObject>("prefabs/projectiles/EngiGrenadeProjectile");
            BombPrefab = engiMinePrefab.InstantiateClone("RollBomb");
            BombPrefab.GetComponent<ProjectileSimple>().velocity = 1; //default 50
            BombPrefab.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Item;
            BombPrefab.GetComponent<ProjectileImpactExplosion>().destroyOnEnemy = false; //default True
            UnityEngine.Object.Destroy(BombPrefab.GetComponent<ApplyTorqueOnStart>());
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
            On.EntityStates.Wisp1Monster.FireEmbers.OnEnter += FireEmbers_OnEnter;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.EntityStates.Wisp1Monster.FireEmbers.OnEnter -= FireEmbers_OnEnter;
        }
        private void FireEmbers_OnEnter(On.EntityStates.Wisp1Monster.FireEmbers.orig_OnEnter orig, EntityStates.Wisp1Monster.FireEmbers self)
        {
            int InvCount = GetPlayersItemCount(catalogIndex);
            if (InvCount == 0)
            {
                orig(self);
                return;
            }
            var health = self.outer.gameObject.GetComponent<HealthComponent>();
            self.outer.enabled = false;
            orig(self);
            if (true == false)
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    damage = health.fullCombinedHealth,
                    damageType = DamageType.BypassOneShotProtection
                };

                health.TakeDamage(damageInfo);
                Chat.AddMessage("Fuck you, Wisp!");
                orig(self);
            }
        }
    }
}
