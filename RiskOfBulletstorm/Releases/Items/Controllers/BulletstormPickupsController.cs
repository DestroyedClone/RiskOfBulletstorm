﻿using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static RiskOfBulletstorm.BulletstormPlugin;
using System.Linq;

namespace RiskOfBulletstorm.Items
{
    public class BulletstormPickupsController : Item<BulletstormPickupsController>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the base amount of kills required to roll a pickup spawn?", AutoConfigFlags.PreventNetMismatch)]
        public int BUP_RequiredKills { get; private set; } = 25;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What value should the required kills be multiplied by per stage?", AutoConfigFlags.PreventNetMismatch)]
        public float BUP_StageMultiplier { get; private set; } = 1.1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the chance to create a pickup after reaching the required amount of kills? (Value: Direct Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float BUP_RollChance { get; private set; } = 40f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the weighted chance to select a Blank?", AutoConfigFlags.PreventNetMismatch)]
        public float BUP_chanceBlank { get; private set; } = 0.45f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the weighted chance to select an Armor?", AutoConfigFlags.PreventNetMismatch)]
        public float BUP_chanceArmor { get; private set; } = 0.15f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the weighted chance to select an Ammo Spread?", AutoConfigFlags.PreventNetMismatch)]
        public float BUP_chanceAmmo { get; private set; } = 0.7f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Debugging: Enable to show in console when a Forgive Me, Please was detected with its damageinfo. Use it to test for any false positives.", AutoConfigFlags.PreventNetMismatch)]
        public bool BUP_DebugShowDollProc { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Debugging: Enable to show in console the info about the kills, and the info about the final result.", AutoConfigFlags.PreventNetMismatch)]
        public bool BUP_ShowProgress { get; private set; } = false;
        public override string displayName => "BulletstormPickupsController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public WeightedSelection<PickupDef> weightedSelection = new WeightedSelection<PickupDef>(3); //change to 4 when key is fixed

        private readonly GameObject SpawnedPickupEffect = Resources.Load<GameObject>("prefabs/effects/LevelUpEffect");

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }
        public override void SetupLate()
        {
            base.SetupLate();
            //needs to setup late so the indicies can be setup
            //weightedSelection.AddChoice(Key.instance.pickupIndex, 0.15f); currently unused while i rework it
            weightedSelection.AddChoice(Blank.instance.pickupDef, BUP_chanceBlank);
            weightedSelection.AddChoice(Armor.instance.pickupDef, BUP_chanceArmor);
            weightedSelection.AddChoice(PickupAmmoSpread.instance.pickupDef, BUP_chanceAmmo);
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
            On.RoR2.MapZone.TryZoneStart += MapZone_TryZoneStart;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GlobalEventManager.OnCharacterDeath -= GlobalEventManager_OnCharacterDeath;
            RoR2.Stage.onStageStartGlobal -= Stage_onStageStartGlobal;
            On.RoR2.MapZone.TryZoneStart -= MapZone_TryZoneStart;
        }

        private void MapZone_TryZoneStart(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            orig(self, other);
            if (!Stage.instance) return;
            BulletstormPickupsComponent pickupsComponent = Stage.instance.GetComponent<BulletstormPickupsComponent>();
            if (!pickupsComponent) return;
            CharacterBody characterBody = other.GetComponent<CharacterBody>();
            if (!characterBody) return;
            TeamComponent teamComponent = characterBody.teamComponent;
            if (!teamComponent) return;
            if (teamComponent.teamIndex == TeamIndex.Player) return;

            if (self.zoneType == MapZone.ZoneType.OutOfBounds)
            {
                HealthComponent healthComponent = characterBody.healthComponent;
                if (healthComponent)
                {
                    pickupsComponent.wasMapDeath = true;
                    pickupsComponent.lastHitAttacker = healthComponent.lastHitAttacker;
                }
            }
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
        }

        private bool CheckIfDoll(DamageInfo dmginfo)
        {
            if (!dmginfo.inflictor && dmginfo.procCoefficient == 1 && dmginfo.damageColorIndex == DamageColorIndex.Item && dmginfo.force == Vector3.zero && dmginfo.damageType == DamageType.Generic)
            {
                if (BUP_DebugShowDollProc)
                {
                    _logger.LogMessage("[RiskOfBulletstorm]Pickups Controller: Forgive Me, Please usage was detected.");
                    _logger.LogMessage("[RiskOfBulletstorm]Pickups Controller: " + dmginfo);
                }
                return true;
            }
            else
                return false;
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            if (!NetworkServer.active || !Stage.instance)
            {
                return;
            }
            var dmginfo = damageReport.damageInfo;
            BulletstormPickupsComponent pickupsComponent = Stage.instance.GetComponent<BulletstormPickupsComponent>();
            if (!Stage.instance || CheckIfDoll(dmginfo) || damageReport.victimTeamIndex == TeamIndex.Player || damageReport.victimTeamIndex == TeamIndex.None || !pickupsComponent)
            {
                //Debug.Log("current stage: "+Stage.instance+"| Doll? "+CheckIfDoll(dmginfo)+"| teamindex: "+damageReport.victimTeamIndex+" pickups component: "+pickupsComponent);
                return;
            }
            var requiredKills = pickupsComponent.requiredKills;
            CharacterBody VictimBody = damageReport.victimBody;
            if (VictimBody)
            {
                //int DiffMultAdd = Run.instance.selectedDifficulty; //TODO: Add difficulty scaling?
                pickupsComponent.globalDeaths++;
                if (BUP_ShowProgress)
                    _logger.LogMessage(string.Format("[Bulletstorm] Kills/StageRequired: {0}/{1}", pickupsComponent.globalDeaths, requiredKills));
                if (pickupsComponent.globalDeaths % requiredKills == 0)
                {
                    Vector3 PickupPosition = new Vector3();
                    if (pickupsComponent.wasMapDeath) //If they die off the map
                    {
                        if (pickupsComponent.lastHitAttacker) //If it's killed by a player
                        {
                            PickupPosition = pickupsComponent.lastHitAttacker.transform.position; //set the position of the pickup to be on the player
                        }
                        else
                        {
                            var playerSearch = new BullseyeSearch() //let's just get the nearest player
                            {
                                viewer = VictimBody,
                                sortMode = BullseyeSearch.SortMode.Distance,
                                teamMaskFilter = TeamMask.allButNeutral
                            };
                            playerSearch.teamMaskFilter.RemoveTeam(TeamIndex.Monster);
                            playerSearch.RefreshCandidates();
                            playerSearch.FilterOutGameObject(VictimBody.gameObject);
                            var list = playerSearch.GetResults().ToList();
                            bool success = false;
                            foreach (var player in list)
                            {
                                var body = player.gameObject.GetComponent<CharacterBody>();
                                if (body)
                                {
                                    if (body.isPlayerControlled)
                                    {
                                        PickupPosition = body.corePosition;
                                        success = true;
                                        break;
                                    }
                                }
                            }
                            if (!success) //fine just the nearest friendly character, ye bastard.
                            {
                                PickupPosition = list.FirstOrDefault().transform.position;
                            }
                            // another way of doing it, but this looks unclean so im not sure if i really want it.
                            //var nearestPlayer = new SphereSearch { origin = VictimBody.transform.position, mask = LayerIndex.entityPrecise.mask }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(VictimBody.teamComponent.teamIndex)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes().FirstOrDefault<HurtBox>();
                        }
                    } else
                    {
                        PickupPosition = VictimBody.transform.position;
                    }
                    PickupPosition += Vector3.up * 2f;
                    if (BUP_ShowProgress)
                        _logger.LogMessage(string.Format("[Bulletstorm] Pickups Controller: Resulting Kill Info (setup for roll):" +
                            "\nwasMapDeath: {0}" +
                            "\nlastHitAttacker: {1}" +
                            "\nposition: {2}", pickupsComponent.wasMapDeath, pickupsComponent.lastHitAttacker, PickupPosition));

                    var teamLuck = RiskOfBulletstorm.Utils.HelperUtil.GetPlayersLuck();
                    if (Util.CheckRoll(BUP_RollChance, teamLuck)) //Roll to spawn pickups
                    {
                        //Chat.AddMessage("Pickups: Rolled success.");

                        var randfloat = UnityEngine.Random.Range(0f, 1f);
                        var dropDef = weightedSelection.Evaluate(randfloat);

                        if (BUP_ShowProgress)
                        {
                            _logger.LogMessage(string.Format("[Bulletstorm] Pickups Controller: Roll success! Chosen item {0} {1} {2}",
                                dropDef.pickupIndex, pickupDef.internalName, Language.GetString(pickupDef.nameToken)));
                        }
                        PickupDropletController.CreatePickupDroplet(dropDef.pickupIndex, PickupPosition, Vector3.up * 5);


                        EffectManager.SimpleEffect(SpawnedPickupEffect, PickupPosition+Vector3.up*8f, Quaternion.identity, true);
                        EffectManager.SimpleEffect(SpawnedPickupEffect, PickupPosition, Quaternion.identity, true);
                        EffectManager.SimpleEffect(SpawnedPickupEffect, PickupPosition+Vector3.down*8f, Quaternion.identity, true);
                    } else
                    {
                        if (BUP_ShowProgress)
                            _logger.LogMessage("[Bulletstorm] Pickups Controller: Roll failed!");
                    }
                    pickupsComponent.wasMapDeath = false;
                    pickupsComponent.lastHitAttacker = null;
                    pickupsComponent.globalDeaths = 0;
                }
            }
        }


        public class BulletstormPickupsComponent : MonoBehaviour
        {
            public int globalDeaths = 0;
            public int requiredKills = 10;
            public bool wasMapDeath = false;
            public GameObject lastHitAttacker;
        }

        public class BulletstormPickupIndicatorTerminator : MonoBehaviour
        {
            public ParticleSystem particleSystem;
            public GameObject spawnedItem;
            public void Awake()
            {
                if (!particleSystem)
                    this.enabled = false;
            }
            public void FixedUpdate()
            {
                if (particleSystem.time >= 43)
                {
                    particleSystem.time = 0;
                }
            }
        }
    }
}
