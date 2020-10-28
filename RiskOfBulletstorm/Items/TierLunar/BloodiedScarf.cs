/*
//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using static RoR2.GenericSkill;


namespace RiskOfBulletstorm.Items
{
    public class BloodiedScarf : Item_V2<BloodiedScarf>
    {

        public float damageReward;
        public override string displayName => "Bloodied Scarf";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>Blink Away</b>\nDodge roll is replaced with a blink.";

        protected override string GetDescString(string langid = null) => $"Replaces the dodge roll with a teleport.";

        protected override string GetLoreString(string langID = null) => "This simple scarf was once worn by a skilled assassin. Betrayed by his brothers and assumed dead...";

        public static GameObject ItemBodyModelPrefab;

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
            GetStatCoefficients += GiveSkill ;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients -= GiveSkill;
        }
        private void GiveSkill(CharacterBody sender, StatHookEventArgs args)
        {
            var invCount = GetCount(sender);
            if (invCount > 0)
            { GenericSkill.(SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarPrimaryReplacement"))); }
        }
    }
}
*/