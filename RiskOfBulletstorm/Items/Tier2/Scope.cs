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

        protected override string GetLoreString(string langID = null) => "4:44 - [Kate] Found this wicked sweet scope in a chest." +
            "\n 4:55 - [Kate] Actually trying to fight with this on my face is kinda hard, might ditch." +
            "\n 5:20 - [Kate] Think this stuff really speaks to me, feels like I'm hitting my shots more often." +
            "\n 6:00 - [Kate] Alright, I'm keeping it. I feel so... like... focused... It's pretty great!" +
            "\n 6:02 - [Kate] Think this is like... covered in alien mind control invisible text? No biggie, whatever helps! :)";

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
            int InventoryCount = self.owner.GetComponent<CharacterBody>().inventory.GetItemCount(catalogIndex);
            if (InventoryCount > 0)
            {
                var ResultMult = 1 - (SpreadReduction + SpreadReductionStack * (InventoryCount - 1));
                //self.maxSpread = Mathf.Max(self.maxSpread * ResultMult * 0.75f, 2 * ResultMult);

                var oldMax = self.maxSpread;
                self.maxSpread = Mathf.Max(self.maxSpread * ResultMult, 0);

                var oldMin = self.minSpread;
                self.minSpread = Mathf.Min(0, self.minSpread * ResultMult);

                //self.owner.GetComponent<CharacterBody>().SetSpreadBloom(ResultMult, false);
                Debug.Log("Scope: Max:" + oldMax.ToString() + "=>" + self.maxSpread.ToString());
                Debug.Log("Scope: Min:" + oldMin.ToString() + "=>" + self.minSpread.ToString());
            }
            orig(self);
        }
    }
}
