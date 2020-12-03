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
using static RiskOfBulletstorm.Utils.HelperUtil;

namespace RiskOfBulletstorm.Items
{
    public class BulletstormPickupsController : Item_V2<BulletstormPickupsController>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Enable Pickups?", AutoConfigFlags.PreventNetMismatch)]
        public bool BUP_Enable { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("RequiredKills = 35", AutoConfigFlags.PreventNetMismatch)]
        public int BUP_RequiredKills { get; private set; } = 35;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Multiplier per stage count = 2.00x", AutoConfigFlags.PreventNetMismatch)]
        public float BUP_StageMultiplier { get; private set; } = 2f;
        public override string displayName => "BulletstormPickupsController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        //private readonly int StageMultiplier = 2;
        //private readonly int DifficultyMultiplier = 1;


        private readonly float PickupRollChance = 20f;

        //WeightedSelection<PickupIndex> weightedSelection = new WeightedSelection<PickupIndex>();

        public WeightedSelection<PickupIndex> weightedSelection = new WeightedSelection<PickupIndex>(4);

        private GameObject currentStage; 

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            //weightedSelection = new WeightedSelection<PickupIndex>();
        }
        public override void SetupLate()
        {
            base.SetupLate();
            //needs to setup late so the indicies can be setup
            weightedSelection.AddChoice(Key.instance.pickupIndex, 0.5f);
            weightedSelection.AddChoice(Blank.instance.pickupIndex, 0.3f);
            weightedSelection.AddChoice(Armor.instance.pickupIndex, 0.1f);
            weightedSelection.AddChoice(PickupAmmoSpread.instance.pickupIndex, 0.1f);
        }

        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            RoR2.Stage.onStageStartGlobal += Stage_onStageStartGlobal;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GlobalEventManager.OnCharacterDeath -= GlobalEventManager_OnCharacterDeath;
            RoR2.Stage.onStageStartGlobal -= Stage_onStageStartGlobal;
        }

        private void Stage_onStageStartGlobal(RoR2.Stage obj)
        {
            var gameObj = obj.gameObject;
            Debug.Log("on stage start entered");
            BulletstormPickupsComponent pickupsComponent = gameObj.GetComponent<BulletstormPickupsComponent>();
            if (!pickupsComponent) pickupsComponent = gameObj.AddComponent<BulletstormPickupsComponent>();


            var stageCount = Run.instance.stageClearCount;
            var StageMult = BUP_StageMultiplier * stageCount;
            if (stageCount < 1) StageMult = 1;
            var requiredKills = BUP_RequiredKills * StageMult;

            pickupsComponent.requiredKills = (int)requiredKills;

            currentStage = gameObj;
        }

        private bool CheckIfDoll(DamageInfo dmginfo)
        {
            if (!dmginfo.inflictor && dmginfo.procCoefficient == 1 && dmginfo.damageColorIndex == DamageColorIndex.Item && dmginfo.force == Vector3.zero && dmginfo.damageType == DamageType.Generic)
            {
                Debug.Log("Doll Detected!");
                return true;
            }
            else
                return false;
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            var dmginfo = damageReport.damageInfo;
            BulletstormPickupsComponent pickupsComponent = currentStage?.GetComponent<BulletstormPickupsComponent>();
            if (!currentStage || CheckIfDoll(dmginfo) || damageReport.victimTeamIndex == TeamIndex.Player || !pickupsComponent)
            {
                Debug.Log("current stage"+currentStage+"| Doll?"+CheckIfDoll(dmginfo)+"| teamindex"+damageReport.victimTeamIndex+" pickups component"+pickupsComponent);
                orig(self, damageReport);
                return;
            }
            Debug.Log("initial pickups checks passed");
            //var kills = pickupsComponent.globalDeaths;
            var requiredKills = pickupsComponent.requiredKills;
            CharacterBody VictimBody = damageReport.victimBody;

            if (VictimBody)
            {
                Chat.AddMessage("Body found"+VictimBody);
                Vector3 PickupPosition = VictimBody.transform.position + Vector3.up *2f;

                //int DiffMultAdd = Run.instance.selectedDifficulty;

                pickupsComponent.globalDeaths++;
                Chat.AddMessage("kills: "+ pickupsComponent.globalDeaths + " / "+ requiredKills);
                if (pickupsComponent.globalDeaths % requiredKills == 0)
                {
                    if (Util.CheckRoll(PickupRollChance)) //Roll to spawn pickups
                    {
                        Chat.AddMessage("Pickups: Rolled success.");

                        var randfloat = UnityEngine.Random.Range(0f, 1f);
                        PickupIndex dropList = weightedSelection.Evaluate(randfloat);
                        PickupDropletController.CreatePickupDroplet(dropList, PickupPosition, Vector3.up * 5);
                    } else
                    {
                        Chat.AddMessage("Roll failed");
                    }
                    //Okay, there's no way we'll hit the integer limit, so no need to set it to zero again.
                }
            }

        }

        public class BulletstormPickupsComponent : MonoBehaviour
        {
            public int globalDeaths = 0;
            public int requiredKills = 10;
        }
    }
}
