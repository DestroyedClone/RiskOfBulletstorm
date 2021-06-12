using System.Collections.ObjectModel;
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
        public int RequiredKills { get; private set; } = 25;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What value should the required kills be multiplied by per stage?", AutoConfigFlags.PreventNetMismatch)]
        public float StageMultiplier { get; private set; } = 1.1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the chance to create a pickup after reaching the required amount of kills? (Value: Direct Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float RollChance { get; private set; } = 40f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the weighted chance to select a Blank?", AutoConfigFlags.PreventNetMismatch)]
        public float ChanceBlank { get; private set; } = 0.45f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the weighted chance to select an Armor?", AutoConfigFlags.PreventNetMismatch)]
        public float ChanceArmor { get; private set; } = 0.15f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the weighted chance to select an Ammo Spread?", AutoConfigFlags.PreventNetMismatch)]
        public float ChanceAmmo { get; private set; } = 0.7f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Debugging: Enable to show in console when a Forgive Me, Please was detected with its damageinfo. Use it to test for any false positives.", AutoConfigFlags.PreventNetMismatch)]
        public bool DebugShowDollProc { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Debugging: Enable to show in console the info about the kills, and the info about the final result.", AutoConfigFlags.PreventNetMismatch)]
        public bool ShowProgress { get; private set; } = false;
        public override string displayName => "BulletstormPickupsController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public WeightedSelection<PickupDef> weightedSelection = new WeightedSelection<PickupDef>(3)
        {

        }; //change to 4 when key is fixed

        private readonly GameObject SpawnedPickupEffect = Resources.Load<GameObject>("prefabs/effects/LevelUpEffect");
        public override void Install()
        {
            base.Install();
            RoR2.Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            On.RoR2.MapZone.TryZoneStart += MapZone_TryZoneStart;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.PickupCatalog.Init += PickupCatalog_Init;
        }
        public override void Uninstall()
        {
            base.Uninstall();
            RoR2.Stage.onStageStartGlobal -= Stage_onStageStartGlobal;
            On.RoR2.MapZone.TryZoneStart -= MapZone_TryZoneStart;
            GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.PickupCatalog.Init -= PickupCatalog_Init;
        }

        private void PickupCatalog_Init(On.RoR2.PickupCatalog.orig_Init orig)
        {
            orig();
            //needs to setup late so the indicies can be setup
            //weightedSelection.AddChoice(Key.instance.pickupIndex, 0.15f); currently unused while i rework it
            weightedSelection.AddChoice(Blank.instance.pickupDef, ChanceBlank);
            weightedSelection.AddChoice(Armor.instance.pickupDef, ChanceArmor);
            weightedSelection.AddChoice(PickupAmmoSpread.instance.pickupDef, ChanceAmmo);
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (!NetworkServer.active || !Stage.instance)
            {
                return;
            }
            BulletstormPickupsComponent pickupsComponent = Stage.instance.GetComponent<BulletstormPickupsComponent>();
            if (!pickupsComponent) return;
            if (CheckIfDoll(damageReport.damageInfo) || damageReport.victimTeamIndex == TeamIndex.Player || damageReport.victimTeamIndex == TeamIndex.None) return;
            var requiredKills = pickupsComponent.requiredKills;
            CharacterBody VictimBody = damageReport.victimBody;
            if (VictimBody)
            {
                //int DiffMultAdd = Run.instance.selectedDifficulty; //TODO: Add difficulty scaling?
                pickupsComponent.globalDeaths++;
                if (ShowProgress) _logger.LogMessage(string.Format("[Bulletstorm] Kills/StageRequired: {0}/{1}", pickupsComponent.globalDeaths, requiredKills));

                if (pickupsComponent.globalDeaths % requiredKills == 0)
                {
                    Vector3 pickupPosition = EvaluatePickupPosition(pickupsComponent, VictimBody);
                    pickupPosition += Vector3.up * 2f;
                    if (ShowProgress)
                        _logger.LogMessage(string.Format("Pickups Controller: Resulting Kill Info (setup for roll):" +
                            "\nwasMapDeath: {0}" +
                            "\nlastHitAttacker: {1}" +
                            "\nposition: {2}", pickupsComponent.wasMapDeath, pickupsComponent.lastHitAttacker, pickupPosition));

                    var teamLuck = RiskOfBulletstorm.Utils.HelperUtil.GetPlayersLuck();
                    if (Util.CheckRoll(RollChance, teamLuck)) //Roll to spawn pickups
                    {
                        //Chat.AddMessage("Pickups: Rolled success.");

                        SpawnPickup(pickupPosition);
                    }
                    else
                    {
                        if (ShowProgress)
                            _logger.LogMessage("[Bulletstorm] Pickups Controller: Roll failed!");
                    }
                    pickupsComponent.wasMapDeath = false;
                    pickupsComponent.lastHitAttacker = null;
                    pickupsComponent.globalDeaths = 0;
                }
            }
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
            if (!gameObj.GetComponent<BulletstormPickupsComponent>())
                gameObj.AddComponent<BulletstormPickupsComponent>();
        }

        private Vector3 EvaluatePickupPosition(BulletstormPickupsComponent pickupsComponent, CharacterBody victimBody)
        {
            Vector3 PickupPosition = new Vector3();
            if (pickupsComponent.wasMapDeath) //If they die from falling off the map
            {
                if (pickupsComponent.lastHitAttacker) // get the last attacker
                {
                    PickupPosition = pickupsComponent.lastHitAttacker.transform.position; //set the position of the pickup to be on the player
                }
                else
                {
                    var playerSearch = new BullseyeSearch() //let's just get the nearest player
                    {
                        viewer = victimBody,
                        sortMode = BullseyeSearch.SortMode.Distance,
                        teamMaskFilter = TeamMask.allButNeutral
                    };
                    playerSearch.teamMaskFilter.RemoveTeam(TeamIndex.Monster);
                    playerSearch.RefreshCandidates();
                    playerSearch.FilterOutGameObject(victimBody.gameObject);
                    var list = playerSearch.GetResults().ToList();
                    bool success = false;
                    if (list.Count > 0)
                    {
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
                    }
                    if (!success) //
                    {
                        PickupPosition = list.FirstOrDefault().transform.position;
                    }
                }
            }
            else // If it wasn't a map death
            {
                PickupPosition = victimBody.transform.position;
            }
            return PickupPosition;
        }

        private void SpawnPickup(Vector3 pickupPosition)
        {
            var randfloat = UnityEngine.Random.Range(0f, 1f);
            var dropDef = weightedSelection.Evaluate(randfloat);

            if (ShowProgress)
            {
                _logger.LogMessage(string.Format("Pickups Controller: Roll success! Chosen item {0} {1} {2}",
                    dropDef.pickupIndex, dropDef.internalName, Language.GetString(dropDef.nameToken)));
            }
            if (dropDef != null)
            {
                PickupDropletController.CreatePickupDroplet(dropDef.pickupIndex, pickupPosition, Vector3.up * 5);

                EffectManager.SimpleEffect(SpawnedPickupEffect, pickupPosition + Vector3.up * 4f, Quaternion.identity, true);
                EffectManager.SimpleEffect(SpawnedPickupEffect, pickupPosition, Quaternion.identity, true);
                EffectManager.SimpleEffect(SpawnedPickupEffect, pickupPosition + Vector3.down * 4f, Quaternion.identity, true);
            } 
        }

        private bool CheckIfDoll(DamageInfo dmginfo)
        {
            if (!dmginfo.inflictor && dmginfo.procCoefficient == 1 && dmginfo.damageColorIndex == DamageColorIndex.Item && dmginfo.force == Vector3.zero && dmginfo.damageType == DamageType.Generic)
            {
                if (DebugShowDollProc)
                {
                    _logger.LogMessage("[RiskOfBulletstorm]Pickups Controller: Forgive Me, Please usage was detected.");
                    _logger.LogMessage("[RiskOfBulletstorm]Pickups Controller: " + dmginfo);
                }
                return true;
            }
            else
                return false;
        }


        public class BulletstormPickupsComponent : MonoBehaviour
        {
            public int globalDeaths = 0;
            public int requiredKills = 10;
            public bool wasMapDeath = false;
            public GameObject lastHitAttacker;

            public int stageCount = 0;
            public float StageMultiplier => BulletstormPickupsController.instance.StageMultiplier;
            public int ConfigRequiredKills => instance.RequiredKills;


            public void Awake()
            {
                var StageMult = (int)(StageMultiplier * stageCount);
                if (stageCount < 1) StageMult = 1;
                requiredKills = ConfigRequiredKills * StageMult;
            }
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
