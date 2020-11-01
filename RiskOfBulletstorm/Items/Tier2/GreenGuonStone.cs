/*
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;


namespace RiskOfBulletstorm.Items
{
    public class GreenGuonStone : Item_V2<GreenGuonStone>
    {
        //TODO: USE CHEN's HEALTH LOSS CODE FOR FLOATS!!!!
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance to heal? (Default: 2%)", AutoConfigFlags.PreventNetMismatch)]
        public float HealChance { get; private set; } = 2f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Base Heal Percent? 0.33", AutoConfigFlags.PreventNetMismatch)]
        public float HealAmount { get; private set; } = 0.33f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Stack Heal Percent? 0.11", AutoConfigFlags.PreventNetMismatch)]
        public float HealAmountStack { get; private set; } = 0.11f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Increase chance if damage is lethal?", AutoConfigFlags.PreventNetMismatch)]
        public bool LethalSave { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Lethal Save Chance. Default: 50%", AutoConfigFlags.PreventNetMismatch)]
        public float LethalSaveChance { get; private set; } = 50f;
        
        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("If true, damage to shield and barrier (from e.g. Personal Shield Generator, Topaz Brooch) will not count towards triggering Enraging Photo")]
        //public bool RequireHealth { get; private set; } = true;
        public override string displayName => "Green Guon Stone";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing });

        protected override string GetNameString(string langID = null) => displayName;
        public string descText = "Chance to heal upon taking ";

        protected override string GetPickupString(string langID = null) => "Chance To Heal\n"+descText+"damage";

        protected override string GetDescString(string langid = null) => $"{HealChance}% {descText} damage. Raises to {LethalSaveChance} if lethal.";

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

            if (InventoryCount > 0)
            {
                if (self.health - damageInfo.damage > 0) //Is your expected health greater than zero?
                { //nonfatal
                    if (Util.CheckRoll(HealChance))
                    {//success
                        Chat.AddMessage("NONLETHAL SUCCESS");
                        //damageInfo.damage = 0;
                        self.health *= 1 + HealAmount + (HealAmountStack * (InventoryCount - 1));
                    }
                    else
                    {//fail
                        Chat.AddMessage("NONLETHAL FAILURE");
                    }
                }
                else 
                {//fatal
                    if (Util.CheckRoll(LethalSaveChance))
                    {//success
                        Chat.AddMessage("LETHAL SUCCESS");
                        //damageInfo.damage = 0;
                        self.health *= 1 + HealAmount + (HealAmountStack * (InventoryCount - 1));
                    }
                    else
                    {//fail
                        Chat.AddMessage("LETHAL FAILURE");
                    }
                }

                Chat.AddMessage("Worked!");
            }
            orig(self, damageInfo);
        }
    }
}
*/