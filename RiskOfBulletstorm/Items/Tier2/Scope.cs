//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;

namespace RiskOfBulletstorm.Items
{
    public class Scope : Item_V2<Scope> 
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Bullet Spread Reduction (Default: 10%)", AutoConfigFlags.PreventNetMismatch)]
        public float SpreadReduction { get; private set; } = 0.10f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Bullet Spread Reduction Stack (Default: 5%)", AutoConfigFlags.PreventNetMismatch)]
        public float SpreadReductionStack { get; private set; } = 0.05f;
        public override string displayName => "Scope";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Steady Aim\nA standard scope. Increases accuracy!";

        protected override string GetDescString(string langid = null) => $"Decreases bullet spread by {Pct(SpreadReduction)}" +
            $"\n(+{Pct(SpreadReductionStack)} per stack)";

        protected override string GetLoreString(string langID = null) => "4:44 - [Kate] Found this scope in a chest, reporting back." +
            "\n 4:55 - [Kate] Seems to work pretty well whether it's attached or not." +
            "\n 5:20 - [Kate] This really works quite well, my shots are hitting more often." +
            "\n 6:00 - [Kate] Alright, I'm keeping it. Can't explain it, feels like I'm actually tightening the spread." +
            "\n 6:02 - [Kate] Scratch that, this is ACTUALLY tightening the spread of bullets. Well, whatever helps.";

        public Scope()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Scope.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/ScopeIcon.png";
        }
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
            On.RoR2.BulletAttack.Fire += BulletAttack_Fire;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.BulletAttack.Fire -= BulletAttack_Fire;
        }

        private void BulletAttack_Fire(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            //doesn't work on MULT?????
            int InventoryCount = self.owner.GetComponent<CharacterBody>().inventory.GetItemCount(catalogIndex);
            if (InventoryCount > 0)
            {
                var ResultMult = 1 - (SpreadReduction + SpreadReductionStack * (InventoryCount - 1));
                //self.maxSpread = Mathf.Max(self.maxSpread * ResultMult * 0.75f, 2 * ResultMult);

                //var oldMax = self.maxSpread;
                self.maxSpread = Mathf.Max(self.maxSpread * ResultMult, 0);

                //var oldMin = self.minSpread;
                self.minSpread = Mathf.Min(0, self.minSpread * ResultMult);

                //self.owner.GetComponent<CharacterBody>().SetSpreadBloom(ResultMult, false);
                //Debug.Log("Scope: Max:" + oldMax.ToString() + "=>" + self.maxSpread.ToString());
                //Debug.Log("Scope: Min:" + oldMin.ToString() + "=>" + self.minSpread.ToString());
            }
            orig(self);
        }
    }
}
