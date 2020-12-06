using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;
using ThinkInvisible.ClassicItems;

namespace RiskOfBulletstorm.Items
{
    public class Meatbun : Equipment_V2<Meatbun>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of maximum health should the Meatbun heal? (Default: 33% of max health)", AutoConfigFlags.PreventNetMismatch)]
        public float Meatbun_HealAmount { get; private set; } = 0.33f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much should the damage be increased by after using the Meatbun? (Default: 0.1 = +10% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float Meatbun_DamageBonus { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the max amount of buffs that Meatbun can give? (Default: 5 stack)", AutoConfigFlags.PreventNetMismatch)]
        public int Meatbun_BuffLimit { get; private set; } = 5;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the minimum percentage of health lost from a single hit to remove the buffs? (Default: 0.05 = 5% health loss)", AutoConfigFlags.PreventNetMismatch)]
        public float Meatbun_HealthThreshold { get; private set; } = 0.05f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds? (Default: 90 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 90f;

        public override string displayName => "Meatbun";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            var desc = "On A Roll\n";
            var doHeal = Meatbun_HealAmount > 0;
            var canBuff = Meatbun_BuffLimit > 0 && Meatbun_DamageBonus > 0;
            if (!doHeal && !canBuff)
                return desc + "Does nothing.";
            if (doHeal) desc += "Heals for a small amount. ";
            if (canBuff) desc += "Increases damage dealt until injured again.";
            return desc;
        }

        protected override string GetDescString(string langid = null)
        {
            if (Meatbun_HealAmount <= 0 && Meatbun_BuffLimit <= 0)
                return $"Does nothing.";

            var desc = $"";
            // heal amount //
            if (Meatbun_HealAmount > 0) desc += $"<style=cIsHealing>Heals for {Pct(Meatbun_HealAmount)} health</style>, and i";
            else desc += $"I";

            //damage bonus //
            if (Meatbun_BuffLimit > 0)
            {
                desc += $"ncreases <style=cIsDamage>damage by +{Pct(Meatbun_DamageBonus)}</style> until damaged";

                // health threshold
                if (Meatbun_HealthThreshold > 0)
                    desc += $"by at least {Pct(Meatbun_HealthThreshold)} health";
            }

            desc += $".";

            desc += $"<style=cStack>Buff stacks up to {Meatbun_BuffLimit} time{(Meatbun_BuffLimit > 1 ? "s" : "")}</style>";
            if (Meatbun_BuffLimit > 1)
                desc += $"for a max of <style=cIsDamage>+{Pct(Meatbun_BuffLimit * Meatbun_DamageBonus)} damage.</style>";
            return desc;
        }

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
            On.RoR2.HealthComponent.TakeDamage += RemoveBuffs;
            On.RoR2.HealthComponent.TakeDamage += IncreaseDmg;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= RemoveBuffs;
            On.RoR2.HealthComponent.TakeDamage -= IncreaseDmg;
        }

        private void IncreaseDmg(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                CharacterBody body = attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    int MeatBunBoostCount = body.GetBuffCount(MeatbunBoost);
                    if (MeatBunBoostCount > 0)
                    {
                        //var olddmg = (float)damageInfo.damage;
                        damageInfo.damage *= 1 + (MeatBunBoostCount * Meatbun_DamageBonus);
                        //Debug.Log("Meatbun: Increased damage from " + olddmg + " to " + damageInfo.damage + " with " + MeatBunBoostCount + " stacks");
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void RemoveBuffs(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var oldHealth = self.health;
            orig(self, damageInfo);
            var healthCompare = (oldHealth - self.health) / self.fullHealth;
            var body = self.body;
            if (body)
            {
                if (healthCompare >= Meatbun_HealthThreshold)
                {
                    int MeatBunBoostCount = body.GetBuffCount(MeatbunBoost);
                    for (int i = 0; i < MeatBunBoostCount; i++) body.RemoveBuff(MeatbunBoost);
                }
            }
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

            if (Meatbun_HealAmount > 0)
            {
                health.HealFraction(Meatbun_HealAmount, default);
                if (instance.CheckEmbryoProc(body)) health.HealFraction(Meatbun_HealAmount, default);
            }
            return true;
        }
    }
}
