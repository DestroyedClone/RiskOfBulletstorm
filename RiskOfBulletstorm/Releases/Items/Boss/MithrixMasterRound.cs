//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using RiskOfBulletstorm.Utils;

namespace RiskOfBulletstorm.Items
{
    public class MithrixMasterRound : Item_V2<MithrixMasterRound>
    {
        public override string displayName => "Master Round";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        private readonly string desc = $"Lunar Chamber\nThis celestial artifact indicates mastery of the planet.";
        protected override string GetPickupString(string langID = null) => desc;

        protected override string GetDescString(string langid = null) => desc;

        protected override string GetLoreString(string langID = null) => "The last bullet that delivered the hero to redemption.";

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

        }
        public override void Install()
        {
            base.Install();
            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += EncounterFinished_OnEnter;
            GetStatCoefficients += MithrixMasterRound_GetStatCoefficients;

        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter -= EncounterFinished_OnEnter;
            GetStatCoefficients -= MithrixMasterRound_GetStatCoefficients;
        }

        private void EncounterFinished_OnEnter(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                HelperUtil.GiveItemToPlayers(catalogIndex);
            }
        }
        private void MithrixMasterRound_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            args.baseHealthAdd += MasterRoundNth.instance.MasterRound_MaxHealthAdd * GetCount(sender);
        }
    }
}
