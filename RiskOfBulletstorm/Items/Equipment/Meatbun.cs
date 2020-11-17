using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Meatbun : Equipment_V2<Meatbun>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Heal%? (Default: 1.0 = 100% heal)", AutoConfigFlags.PreventNetMismatch)]
        public float HealAmount { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage Bonus %? (Default: 2.0 = 200% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageBonus { get; private set; } = 2f;

        public override string displayName => "Medkit";
        public override float cooldown { get; protected set; } = 90f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "On A Roll\nHeals for a small amount. Increases damage dealt until injured.";

        protected override string GetDescString(string langid = null) => $"Heals for {Pct(HealAmount)} health, and increases damage by +{Pct(DamageBonus)} until you take damage.";

        protected override string GetLoreString(string langID = null) => "A delicious, freshly baked roll! Sometimes, things just work out.";

        public BuffIndex MeatbunBoost { get; private set; }

        public override void SetupBehavior()
        {
            base.SetupBehavior();

        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            var dmgBuff = new CustomBuff(
            new BuffDef
            {
                buffColor = Color.white,
                canStack = false,
                isDebuff = false,
                name = "Meatbun Bonus",
            });
            MeatbunBoost = BuffAPI.Add(dmgBuff);
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (damageInfo.attacker)
            {
                CharacterBody body = damageInfo.attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    if (body.HasBuff(MeatbunBoost))
                    {
                        damageInfo.damage *= DamageBonus;
                    }
                }
            }
            orig(self, damageInfo, victim);
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;

            health.HealFraction(0.33f, default);
            return true;
        }
    }
}
