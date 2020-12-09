using RoR2;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Medkit : Equipment_V2<Medkit>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of maximum health should the Medkit heal? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Medkit_HealAmount { get; private set; } = 0.75f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of barrier should the Meatbun give? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Medkit_BarrierAmount { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 100f;

        public override string displayName => "Medkit";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null)
        {
            if (Medkit_HealAmount > 0 && Medkit_BarrierAmount > 0) return "Heals\nMedkits provides substantial healing when used.";
            else return "There's nothing inside?";
        }

        protected override string GetDescString(string langid = null)
        {
            var canHeal = Medkit_HealAmount > 0;
            var canBarrier = Medkit_BarrierAmount > 0;
            if (!canHeal && !canBarrier) return $"Everything inside was emptied, it does nothing.";
            var desc = $"";
            if (canHeal) desc += $"Heals for <style=cIsHealing>{Pct(Medkit_HealAmount)} health</style>. ";
            if (canBarrier) desc += $"Gives a <style=cIsHealing>temporary barrier for {Pct(Medkit_BarrierAmount)} of your max health.</style>.";
            return desc;
        }

        protected override string GetLoreString(string langID = null)
        {
            var desc = "";
            if (Medkit_HealAmount > 0)
                desc += "Contains";
            else desc += "Used to contain";
            desc += " a small piece of fairy." +
                "\nSeeking a place that would provide a near constant flow of the desperate and injured, Médecins Sans Diplôme recognized the Gungeon as the perfect place to found their practice.";
            return desc;
        }

        public Medkit()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Medkit.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Medkit.png";
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();

            if (ClassicItemsCompat.enabled)
                ClassicItemsCompat.RegisterEmbryo(catalogIndex);
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
            if (ClassicItemsCompat.enabled && ClassicItemsCompat.CheckEmbryoProc(instance, body))
            {
                health.HealFraction(Medkit_HealAmount, default);
                health.AddBarrier(BarrierAmt);
            }
            return true;
        }
    }
}
