using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class BulletstormPickupsController : Item_V2<BulletstormPickupsController>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the base amount of kills required to roll a pickup spawn?", AutoConfigFlags.PreventNetMismatch)]
        public int BUP_RequiredKills { get; private set; } = 30;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What value should the required kills be multiplied by per stage?", AutoConfigFlags.PreventNetMismatch)]
        public float BUP_StageMultiplier { get; private set; } = 1.5f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the chance to create a pickup after reaching the required amount? (Value: Direct Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float BUP_RollChance { get; private set; } = 30f;
        public override string displayName => "BulletstormPickupsController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

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
            weightedSelection.AddChoice(Key.instance.pickupIndex, 0.15f);
            weightedSelection.AddChoice(Blank.instance.pickupIndex, 0.25f);
            weightedSelection.AddChoice(Armor.instance.pickupIndex, 0.1f);
            weightedSelection.AddChoice(PickupAmmoSpread.instance.pickupIndex, 0.3f);
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
            BulletstormPickupsComponent pickupsComponent = gameObj.GetComponent<BulletstormPickupsComponent>();
            if (!pickupsComponent) pickupsComponent = gameObj.AddComponent<BulletstormPickupsComponent>();


            var stageCount = Run.instance.stageClearCount;
            var StageMult = (int)(BUP_StageMultiplier * stageCount);
            if (stageCount < 1) StageMult = 1;
            var requiredKills = BUP_RequiredKills * StageMult;

            pickupsComponent.requiredKills = (int)requiredKills;

            currentStage = gameObj;
        }

        private bool CheckIfDoll(DamageInfo dmginfo)
        {
            if (!dmginfo.inflictor && dmginfo.procCoefficient == 1 && dmginfo.damageColorIndex == DamageColorIndex.Item && dmginfo.force == Vector3.zero && dmginfo.damageType == DamageType.Generic)
            {
                Debug.Log("Doll Detected: ");
                Debug.Log(dmginfo);
                return true;
            }
            else
                return false;
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            if (!NetworkServer.active) return;
            var dmginfo = damageReport.damageInfo;
            BulletstormPickupsComponent pickupsComponent = currentStage?.GetComponent<BulletstormPickupsComponent>();
            if (!currentStage || CheckIfDoll(dmginfo) || damageReport.victimTeamIndex == TeamIndex.Player || !pickupsComponent)
            {
                Debug.Log("current stage: "+currentStage+"| Doll? "+CheckIfDoll(dmginfo)+"| teamindex: "+damageReport.victimTeamIndex+" pickups component: "+pickupsComponent);
                orig(self, damageReport);
                return;
            }
            var requiredKills = pickupsComponent.requiredKills;
            CharacterBody VictimBody = damageReport.victimBody;

            if (VictimBody)
            {
                Vector3 PickupPosition = VictimBody.transform.position + Vector3.up *2f;

                //int DiffMultAdd = Run.instance.selectedDifficulty;

                pickupsComponent.globalDeaths++;
                //Chat.AddMessage("kills: "+ pickupsComponent.globalDeaths + " / "+ requiredKills);
                if (pickupsComponent.globalDeaths % requiredKills == 0)
                {
                    if (Util.CheckRoll(BUP_RollChance)) //Roll to spawn pickups
                    {
                        //Chat.AddMessage("Pickups: Rolled success.");

                        var randfloat = UnityEngine.Random.Range(0f, 1f);
                        PickupIndex dropList = weightedSelection.Evaluate(randfloat);
                        PickupDropletController.CreatePickupDroplet(dropList, PickupPosition, Vector3.up * 5);
                    } else
                    {
                        //Chat.AddMessage("Roll failed");
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
