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
using static R2API.DirectorAPI;

namespace RiskOfBulletstorm.Items
{
    public class BulletstormPickupsController : Item_V2<BulletstormPickupsController>
    {
        public override string displayName => "BulletstormPickupsController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        private int globalDeaths = 0;

        //private readonly int StageMultiplier = 2;
        //private readonly int DifficultyMultiplier = 1;
        private readonly int KillRequirement = 10;

        private readonly PickupIndex KeyIndex = Key.instance.pickupIndex;
        private readonly PickupIndex BlankIndex = Blank.instance.pickupIndex;
        private readonly PickupIndex ArmorIndex = Armor.instance.pickupIndex;

        private readonly float PickupRollChance = 0.20f;

        //WeightedSelection<PickupIndex> weightedSelection = new WeightedSelection<PickupIndex>();

        private WeightedSelection<PickupIndex> weightedSelection;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            weightedSelection = new WeightedSelection<PickupIndex>();
            weightedSelection.AddChoice(KeyIndex, 0.5f);
            weightedSelection.AddChoice(BlankIndex, 0.3f);
            weightedSelection.AddChoice(ArmorIndex, 0.2f);

        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.Stage.Start += Stage_Start;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GlobalEventManager.OnCharacterDeath -= GlobalEventManager_OnCharacterDeath;
            On.RoR2.Stage.Start -= Stage_Start;
        }
        private void Stage_Start(On.RoR2.Stage.orig_Start orig, RoR2.Stage self)
        {
            orig(self);
            globalDeaths = 0;
        }

        private void CreatePickup(PickupIndex pickupIndex, Vector3 position)
        {
            PickupDropletController.CreatePickupDroplet(pickupIndex, position, Vector3.up * 5);
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            CharacterBody VictimBody = damageReport.victimBody;
            if (VictimBody)
            {
                Vector3 PickupPosition = VictimBody.transform.position;
                //int StageMultAdd = StageMultiplier * Run.instance.stageClearCount;
                //int DiffMultAdd = Run.instance.selectedDifficulty;
                
                globalDeaths++;
                if (globalDeaths % KillRequirement == 0)
                {
                    if (Util.CheckRoll(PickupRollChance)) //Roll to spawn pickups
                    {
                        Chat.AddMessage("Pickups: Rolled success.");

                        var randfloat = UnityEngine.Random.Range(0f, 1f);
                        PickupIndex dropList = weightedSelection.Evaluate(randfloat);
                        CreatePickup(dropList, PickupPosition);

                    }
                }
            }
        }
    }
}
