
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
	public class MachoBrace : Item_V2<MachoBrace> //Add Glow (good effect on the invuln from the dio revive buff)
	{
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage Bonus? (Default: 0.3 = +30% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float MachoBraceDamage { get; private set; } = 0.3f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage Bonus per stack? (Default: 0.1 = +10% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float MachoBraceDamageStack { get; private set; } = 0.1f;

        public override string displayName => "Macho Brace";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Value for Effort\nShots are more powerful when coming out of your utility. Does not affect speed.";

        protected override string GetDescString(string langid = null) => $"Using your utility increases the damage of your text attack by <style=cIsDamage>{Pct(MachoBraceDamage)} damage (+{Pct(MachoBraceDamageStack)} per stack) </style>.";

        protected override string GetLoreString(string langID = null) => "Training to be the very best takes commitment, and hard work.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        //public static GameObject ItemBodyModelPrefab;
        public static GameObject BombPrefab { get; private set; }
        public BuffIndex MachoBrace_Boost { get; private set; }

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
                buffColor = Color.yellow,
                canStack = false,
                isDebuff = false,
                name = "MachoBrace_Boost",
            });
            MachoBrace_Boost = BuffAPI.Add(dmgBuff);
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
            GetStatCoefficients += AddDamageReward;
        }
        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GenericSkill.OnExecute -= GenericSkill_OnExecute;
            GetStatCoefficients -= AddDamageReward;
        }
        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            var invCount = GetCount(self.characterBody);
            CharacterBody vBody = self.characterBody;

            if (vBody.skillLocator.FindSkill(self.skillName))
            {
                if (invCount > 0)
                {
                    if (vBody.skillLocator.utility.Equals(self)) //is this [redacted] redundant?
                    {
                        if (vBody.HasBuff(MachoBrace_Boost)) //should prevent weird stacking
                        {
                            vBody.RemoveBuff(MachoBrace_Boost);
                        }
                        vBody.AddBuff(MachoBrace_Boost);
                    }
                    else
                    {
                        if (vBody.HasBuff(MachoBrace_Boost))
                        {
                            vBody.RemoveBuff(MachoBrace_Boost);
                        }
                    }
                }
            }
            orig(self);
        }
        private void AddDamageReward(CharacterBody sender, StatHookEventArgs args)
        {
            var InventoryCount = GetCount(sender);
            if (sender.HasBuff(MachoBrace_Boost))
            {
                args.damageMultAdd += MachoBraceDamage + MachoBraceDamageStack * (InventoryCount - 1);
            }
        }
    }
}
