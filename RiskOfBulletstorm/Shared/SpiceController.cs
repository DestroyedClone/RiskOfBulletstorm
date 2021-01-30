using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using System;

namespace RiskOfBulletstorm.Items
{
    public class SpiceController : Item_V2<SpiceController>
    {
        public override string displayName => "SpiceController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        //private static readonly float HeartValue = 50f;

        private ItemIndex SpiceTally;

        private float[] SpiceBonusesAdditive;
        private float[,] SpiceBonuses;
        private float[] SpiceBonusesConstantMaxed;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        //https://enterthegungeon.gamepedia.com/Status_Effects
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }

        public override void SetupLate()
        {
            base.SetupLate();
            //so we dont crash
            {
                SpiceTally = Spice.SpiceTally;
                SpiceBonusesAdditive = Spice.SpiceBonusesAdditive;
                SpiceBonuses = Spice.SpiceBonuses;
                SpiceBonusesConstantMaxed = Spice.SpiceBonusesConstantMaxed;
            } //Spice Setup

        }
        public override void Install()
        {
            base.Install();
            GetStatCoefficients += AddSpiceRewards;
            On.RoR2.HealthComponent.TakeDamage += SPICE_HealthComponent_TakeDamage;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients += AddSpiceRewards;
            On.RoR2.HealthComponent.TakeDamage += SPICE_HealthComponent_TakeDamage;
        }
        private void AddSpiceRewards(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var SpiceTallyCount = sender.inventory.GetItemCount(SpiceTally);
                switch (SpiceTallyCount)
                {
                    case 0:
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        //health, attack speed, shot accuracy, enemy bullet speed, damage
                        //args.baseHealthAdd += HeartValue * SpiceBonuses[SpiceTallyCount, 0];
                        args.healthMultAdd += 1 + SpiceBonuses[SpiceTallyCount, 0];
                        args.attackSpeedMultAdd += SpiceBonuses[SpiceTallyCount, 1];
                        //accuracy 
                        //enemy bullet speed
                        //damage
                        break;
                    default:
                        //var baseHealthAdd = HeartValue * SpiceBonusesAdditive[0] * (SpiceTallyCount - 4);
                        //args.baseHealthAdd += baseHealthAdd;
                        args.healthMultAdd += Math.Min(0.1f, 1 + SpiceBonusesAdditive[0] * (SpiceTallyCount - 4));
                        //health, attack speed, shot accuracy, enemy bullet speed, damage
                        args.attackSpeedMultAdd += SpiceBonusesConstantMaxed[1];
                        //accuracy
                        //enemy
                        //damage
                        break;
                }
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Used Values")]
        private void SPICE_HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                CharacterBody body = attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    var inventory = body.inventory;
                    if (inventory)
                    {
                        var SpiceTallyCount = inventory.GetItemCount(SpiceTally);
                        //var DamageMult = 0f;
                        var SpiceMult = 0f;
                        switch (SpiceTallyCount)
                        {
                            case 0: //
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                SpiceMult = SpiceBonuses[SpiceTallyCount, 4];
                                break;
                            default: //also 5
                                SpiceMult = SpiceBonuses[4, 4] + SpiceBonusesAdditive[4] * (SpiceTallyCount - 4);
                                break;
                        }
                        //DamageMult = SpiceMult;
                        damageInfo.damage *= 1 + SpiceMult;
                    }
                }
            }
            orig(self, damageInfo);
        }



    }
}
