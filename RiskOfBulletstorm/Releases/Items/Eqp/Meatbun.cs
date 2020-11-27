using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;
using ThinkInvisible.ClassicItems;

namespace RiskOfBulletstorm.Items
{
    public class Meatbun : Equipment_V2<Meatbun>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Heal%?" +
            "\nDefault: 0.33 = 33% health", AutoConfigFlags.PreventNetMismatch)]
        public float Meatbun_HealAmount { get; private set; } = 0.33f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage Bonus %? (Default: 0.45 = +45% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float Meatbun_DamageBonus { get; private set; } = 0.45f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Max amount of buffs from Meatbun. (Default: 5)", AutoConfigFlags.PreventNetMismatch)]
        public int Meatbun_BuffLimit { get; private set; } = 5;

        public override string displayName => "Meatbun";
        public override float cooldown { get; protected set; } = 90f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "On A Roll\nHeals for a small amount. Increases damage dealt until injured again.";

        protected override string GetDescString(string langid = null) => $"<style=cIsHealing>Heals for {Pct(Meatbun_HealAmount)} health</style>, and increases <style=cIsDamage>damage by +{Pct(Meatbun_DamageBonus)}</style> until damaged.";

        protected override string GetLoreString(string langID = null) => "A delicious, freshly baked roll! Sometimes, things just work out.";

        public BuffIndex MeatbunBoost { get; private set; }

        public Meatbun()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Meatbun.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/MeatbunIcon.png";
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();
            Embryo_V2.instance.Compat_Register(catalogIndex);
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            var dmgBuff = new CustomBuff(
            new BuffDef
            {
                buffColor = Color.white,
                canStack = true,
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
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            var body = self.body;
            if (body)
            {
                int MeatBunBoostCount = body.GetBuffCount(MeatbunBoost);
                for (int i = 0; i < MeatBunBoostCount; i++) body.RemoveBuff(MeatbunBoost);
            }
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (damageInfo.attacker)
            {
                CharacterBody body = damageInfo.attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    int MeatBunBoostCount = body.GetBuffCount(MeatbunBoost);
                    if (MeatBunBoostCount > 0)
                    {
                        damageInfo.damage *= MeatBunBoostCount * Meatbun_DamageBonus;
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

            int MeatBunBoostCount = body.GetBuffCount(MeatbunBoost);

            if (MeatBunBoostCount < Meatbun_BuffLimit)
                body.AddBuff(MeatbunBoost);

            health.HealFraction(Meatbun_HealAmount, default);
            if (instance.CheckEmbryoProc(body)) health.HealFraction(Meatbun_HealAmount, default);
            return true;
        }
    }
}
