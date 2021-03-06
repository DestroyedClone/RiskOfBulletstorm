﻿
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;


namespace RiskOfBulletstorm.Items
{
    public class GreenGuonStone : Item_V2<GreenGuonStone>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance to heal? (Default: 10%)", AutoConfigFlags.PreventNetMismatch)]
        public float HealChance { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Base Heal Amount? Default 50", AutoConfigFlags.PreventNetMismatch)]
        public float HealAmount { get; private set; } = 50f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Stack Heal Amount? Default 25", AutoConfigFlags.PreventNetMismatch)]
        public float HealAmountStack { get; private set; } = 25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Increase chance if damage is lethal?", AutoConfigFlags.PreventNetMismatch)]
        public bool LethalSave { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Lethal Save Chance. Default: 25%", AutoConfigFlags.PreventNetMismatch)]
        public float LethalSaveChance { get; private set; } = 25f;
        
        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("If true, damage to shield and barrier (from e.g. Personal Shield Generator, Topaz Brooch) will not count towards triggering Enraging Photo")]
        //public bool RequireHealth { get; private set; } = true;
        public override string displayName => "Green Guon Stone";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing });

        protected override string GetNameString(string langID = null) => displayName;
        public string descText = "Chance to heal upon taking ";

        protected override string GetPickupString(string langID = null) => "Chance To Heal\n"+descText+"damage";

        protected override string GetDescString(string langid = null) => $"{HealChance}% {descText} damage. Raises to {LethalSaveChance} if lethal." +
            $"\n Heals for {HealAmount} + {HealAmountStack} per stack.";

        protected override string GetLoreString(string langID = null) => "The Green Guon stone abhors pain, and has a small chance to heal its bearer upon being wounded. It seems to grow more desperate as the risk of death rises.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject ItemBodyModelPrefab;

        public BuffIndex ROBEnraged { get; private set; }

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
            On.RoR2.HealthComponent.TakeDamage += TankHit;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= TankHit;
        }
        private void TankHit(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var InventoryCount = GetCount(self.body);
            //var actualHealAmount = self.fullHealth * (HealAmount + (HealAmountStack * (InventoryCount - 1)));
            var actualHealAmount = HealAmount + HealAmountStack * (InventoryCount - 1);

            if (InventoryCount > 0)
            {
                if (!damageInfo.rejected)
                {
                    if (self.health - damageInfo.damage > 0) //Is your expected health loss greater than zero?
                    { //nonfatal
                        if (Util.CheckRoll(HealChance))
                        {//success
                            Chat.AddMessage("GreenGuon: NONLETHAL SUCCESS");
                            damageInfo.rejected = true;
                            self.Heal(actualHealAmount, default, true);
                            Chat.AddMessage("GreenGuon: Damage Blocked: " + damageInfo.damage.ToString());
                            Chat.AddMessage("GreenGuon: Healed for " + actualHealAmount.ToString());
                        }
                        else
                        {//fail
                         //Chat.AddMessage("NONLETHAL FAILURE");
                        }
                    }
                    else
                    {//fatal
                        if (Util.CheckRoll(LethalSaveChance))
                        {//success
                            Chat.AddMessage("GreenGuon: LETHAL SUCCESS!!");
                            damageInfo.rejected = true;
                            self.Heal(actualHealAmount, default, true);
                            Chat.AddMessage("GreenGuon: Damage Blocked: " + damageInfo.damage.ToString());
                            Chat.AddMessage("GreenGuon: Healed for " + actualHealAmount.ToString());
                        }
                        else
                        {//fail
                         //Chat.AddMessage("LETHAL FAILURE");
                        }
                    }
                }
            }
            orig(self, damageInfo);
        }
    }
}
