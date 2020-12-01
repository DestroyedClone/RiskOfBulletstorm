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
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            //On.EntityStates.BrotherHaunt.FireRandomProjectiles.OnEnter += FireRandomProjectiles_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += EncounterFinished_OnEnter;
        }

        private void EncounterFinished_OnEnter(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            orig(self);
            //var currentScene = SceneCatalog.currentSceneName;
            Debug.Log("Entered FireProjectiles");
            //if (currentScene == "moon")
            if (true)
            {
                Debug.Log("On the moon");
                if (NetworkServer.active)
                {
                    Debug.Log("Here");
                    HelperUtil.GiveItemToPlayers(catalogIndex);
                }
                else
                {

                    Debug.Log("Network server not active");
                }
            }
        }

        private void FireRandomProjectiles_OnEnter(On.EntityStates.BrotherHaunt.FireRandomProjectiles.orig_OnEnter orig, EntityStates.BrotherHaunt.FireRandomProjectiles self)
        {
            orig(self);
            var currentScene = SceneCatalog.currentSceneName;
            Debug.Log("Entered FireProjectiles");
            if (currentScene == "moon")
            {
                Debug.Log("On the moon");
                if (NetworkServer.active)
                {
                    Debug.Log("Here");
                    HelperUtil.GiveItemToPlayers(catalogIndex);
                } else
                {

                    Debug.Log("Network server not active");
                }
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            //On.EntityStates.BrotherHaunt.FireRandomProjectiles.OnEnter -= FireRandomProjectiles_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter -= EncounterFinished_OnEnter;
        }
    }
}
