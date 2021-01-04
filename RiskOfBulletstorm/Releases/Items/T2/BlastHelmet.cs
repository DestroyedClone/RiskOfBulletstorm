
using System.Collections.Generic;
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
using static RiskOfBulletstorm.Utils.HelperUtil;
using static RiskOfBulletstorm.RiskofBulletstorm;

namespace RiskOfBulletstorm.Items
{
    public class BlastHelmet : Item_V2<BlastHelmet>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What button should be held down to choose which slot to select with number keys?", AutoConfigFlags.None)]
        public KeyCode BlastHelmet_ { get; private set; } = KeyCode.LeftShift;
        public override string displayName => "Blast Helmet";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Duck And Cover\nReduces hazard radius of explosions.";

        protected override string GetDescString(string langid = null) => $"Reduces enemy explosives by 1% per stack up -99%";

        protected override string GetLoreString(string langID = null) => "This hardy helmet provides protection when too close to an explosion. Be wary though, it only works at a distance.";

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
            On.RoR2.BlastAttack.Fire += BlastAttack_Fire;
        }

        private BlastAttack.Result BlastAttack_Fire(On.RoR2.BlastAttack.orig_Fire orig, BlastAttack self)
        {
            var attackerTeamIndex = self.teamIndex;

            var enemyTeamIndex = TeamIndex.None;
            switch (attackerTeamIndex)
            {
                case TeamIndex.Player:
                    enemyTeamIndex = TeamIndex.Monster;
                    break;
                case TeamIndex.Monster:
                case TeamIndex.Neutral:
                    enemyTeamIndex = TeamIndex.Player;
                    break;
            }

            var enemyItemCount = Util.GetItemCountForTeam(enemyTeamIndex, catalogIndex, true);
            if (enemyItemCount > 0)
            {
                float radiusCoefficient = Mathf.Max(100 - enemyItemCount,1)/100;
                self.radius *= radiusCoefficient;
                _logger.LogMessage("BlastHelmet: Adjusted blast radius by coefficient of "+ radiusCoefficient);
            }
            return orig(self);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.BlastAttack.Fire -= BlastAttack_Fire;
        }
    }
}
