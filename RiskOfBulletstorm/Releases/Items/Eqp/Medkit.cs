﻿using EntityStates.Treebot.Weapon;
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
    public class Medkit : Equipment_V2<Medkit>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Heal%? (Default: 1.0 = 100% heal)", AutoConfigFlags.PreventNetMismatch)]
        public float Medkit_HealAmount { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Barrier%? (Default: 0.5 = 50%% barrier)", AutoConfigFlags.PreventNetMismatch)]
        public float Medkit_BarrierAmount { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Cooldown (Default: 100 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 100f;

        public override string displayName => "Medkit";
        public string descText = "Heals";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => descText + "\nMedkits provides substantial healing when used.";

        protected override string GetDescString(string langid = null) => $"{descText} for <style=cIsHealing>{Pct(Medkit_HealAmount)} health</style> and gives a <style=cIsHealing>temporary barrier for {Pct(Medkit_BarrierAmount)} of your health</style>.";

        protected override string GetLoreString(string langID = null) => "Contains a small piece of fairy." +
            "\nSeeking a place that would provide a near constant flow of the desperate and injured, Médecins Sans Diplôme recognized the Gungeon as the perfect place to found their practice.";

        public Medkit()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Medkit.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Medkit.png";
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();
            Embryo_V2.instance.Compat_Register(catalogIndex);
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
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;

            var BarrierAmt = health.fullBarrier * Medkit_BarrierAmount;

            health.HealFraction(Medkit_HealAmount, default);
            health.AddBarrier(BarrierAmt);
            if (instance.CheckEmbryoProc(body))
            {
                health.HealFraction(Medkit_HealAmount, default);
                health.AddBarrier(BarrierAmt);
            }
            return true;
        }
    }
}