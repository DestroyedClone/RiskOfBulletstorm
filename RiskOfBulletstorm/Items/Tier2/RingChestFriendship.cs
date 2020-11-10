using System.Collections.ObjectModel;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using static RiskOfBulletstorm.Shared.HelperUtil;
using static R2API.DirectorAPI;

namespace RiskOfBulletstorm.Items
{
    public class RingChestFriendship : Item_V2<RingChestFriendship> //Change to equipment that gives cursed.
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Director Credit Multiplier (Default: 0.1 = +10%)", AutoConfigFlags.PreventNetMismatch)]
        public float DirectorCreditMult { get; private set; } = 0.1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Director Credit Multiplier (Default: 0.02 = +2%)", AutoConfigFlags.PreventNetMismatch)]
        public float DirectorCreditMultStack { get; private set; } = 0.02f;

        public override string displayName => "Ring of Chest Friendship";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        private readonly string descText = "Increases the chance of finding chests";
        protected override string GetPickupString(string langID = null) => "Chest Friends Forever\n" + descText;

        protected override string GetDescString(string langid = null) => $"{descText} by {Pct(DirectorCreditMult)} + {Pct(DirectorCreditMult)} per stack.";

        protected override string GetLoreString(string langID = null) => "This ring was first given to Winchester, largely due to a naming mix-up. With little use for treasure, Winchester eventually gave it away as a prize in one of his strange games.";

        public override void SetupBehavior()
        {
            base.SetupBehavior();
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
            //On.RoR2.SceneDirector.Awake += SceneDirector_Awake;
            //On.RoR2.SceneDirector.Start += SceneDirector_Start;
            StageSettingsActions += MultiplyCredits;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            //On.RoR2.SceneDirector.Awake -= SceneDirector_Awake;
            //On.RoR2.SceneDirector.Start -= SceneDirector_Start;
            StageSettingsActions -= MultiplyCredits;
        }

        /*private void SceneDirector_Awake(On.RoR2.SceneDirector.orig_Awake orig, SceneDirector self)
        {
            var InventoryCount = GetPlayersItemCount(catalogIndex);
            Debug.Log("ChestFriend: Entered Hook", self);
            if (InventoryCount > 0)
            {
                var ResultMult = 1 + DirectorCreditMult + DirectorCreditMultStack * (InventoryCount - 1);
                Debug.Log("ChestFriend: Credits "+self.interactableCredit.ToString()+" multiplied by "+ ((int)ResultMult).ToString(), self);
                self.interactableCredit *= (int)ResultMult;
            }
            orig(self);
        }*/

        /*
        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            var InventoryCount = GetPlayersItemCount(catalogIndex);
            Debug.Log("ChestFriend: Entered Hook", self);
            if (InventoryCount > 0)
            {
                var ResultMult = 1 + DirectorCreditMult + DirectorCreditMultStack * (InventoryCount - 1);
                Debug.Log("ChestFriend: Credits " + self.interactableCredit.ToString() + " multiplied by " + ((int)ResultMult).ToString(), self);
                self.interactableCredit *= (int)ResultMult;
            }
            orig(self);
        } */
        private void MultiplyCredits(StageSettings stageSettings, StageInfo stageInfo )
        {
            var InventoryCount = GetPlayersItemCount(catalogIndex);
            var ResultMult = 1 + DirectorCreditMult + DirectorCreditMultStack * (InventoryCount - 1);
            stageSettings.SceneDirectorInteractableCredits *= (int)ResultMult;
        }
    }
}
