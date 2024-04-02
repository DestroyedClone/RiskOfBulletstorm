using BepInEx.Configuration;
using RiskOfBulletstormRewrite.Items;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Artifact.ArtifactBossRoom;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactBossRoom : ArtifactBase<ArtifactBossRoom>
    {
        public override string ArtifactName => "Artifact of the Boss Chamber";

        public override string ArtifactLangTokenName => "BOSSCHAMBER";

        public override Sprite ArtifactEnabledIcon => Assets.NullSprite;//Assets.LoadSprite("Assets/Icons/ARTIFACT_ADAPTIVEARMORBOSSES_ENABLED");

        public override Sprite ArtifactDisabledIcon => Assets.NullSprite;//Assets.LoadSprite("Assets/Icons/ARTIFACT_ADAPTIVEARMORBOSSES_DISABLED");

        //public static ItemDef RewardItemDef => MasterRound.instance.ItemDef;
        public static float MaxHealthAdditiveMultiplier { get; private set; } = 0.10f;
        public static int MaxAllowedHits { get; private set; } = 3;
        public static int AdditionalAllowedHitsPerStage { get; private set; } = 1;
        public static int MinimumDamageBeforeInvalidation { get; private set; } = 5;
        public static bool AllowSelfDamage { get; private set; } = false;
        public static bool EnableAnnounceOnStart { get; private set; } = true;
        public bool EnableShowHitInChat { get; private set; } = true;
        public static bool OnlyAllowTeleBoss { get; private set; } = true;
        public static bool EnableLunarWisps { get; private set; } = false;
        public static bool EnableLunarGolems { get; private set; } = false;
        public static bool EnableLunarExploders { get; private set; } = false;
        public BodyIndex BodyIndexLunarGolem;
        public BodyIndex BodyIndexLunarWisp;
        public BodyIndex BodyIndexLunarExploder;

        public override void Init(ConfigFile config)
        {
            base.Init(config);
            BodyCatalog.availability.CallWhenAvailable(() => {
                BodyIndexLunarGolem = BodyCatalog.FindBodyIndex("LUNARGOLEM");
                BodyIndexLunarWisp = BodyCatalog.FindBodyIndex("LUNARWISP");
                BodyIndexLunarExploder = BodyCatalog.FindBodyIndex("EXPLODER");
            });
        }

        public void ClearBossResiduals(TeleporterInteraction self)
        {
            foreach (var projectile in InstanceTracker.GetInstancesList<ProjectileController>())
            {
                if (projectile?.teamFilter?.teamIndex != TeamIndex.Player)
                {
                    UnityEngine.Object.Destroy(projectile);
                }
            }
        }

        public override void OnArtifactEnabled()
        {
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal += TeleporterInteraction_onTeleporterChargedGlobal;
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction interaction)
        {
            StartMasterRoundEvent();
        }
        private void TeleporterInteraction_onTeleporterChargedGlobal(TeleporterInteraction obj)
        {
            if (NetworkServer.active)
            {
                var comps = UnityEngine.Object.FindObjectsOfType<MasterRoundMasterComponent>();
                foreach (var component in comps)
                {
                    component.teleporterCharging = false;
                    component.enabled = false;
                }
                CheckMasterRoundEventResult();
            }
        }
        public void StartMasterRoundEvent()
        {
            var playerList = PlayerCharacterMasterController.instances;
            var StageCount = Run.instance.stageClearCount;
            var maxHits = MaxAllowedHits + StageCount * AdditionalAllowedHitsPerStage;
            if (NetworkServer.active)
            {
                foreach (var player in playerList)
                {
                    var body = player.master.GetBody();
                    if (body && body.healthComponent && body.healthComponent.alive)
                    {
                        var masterRoundComponent = player.master.gameObject.GetComponent<MasterRoundMasterComponent>();
                        if (!masterRoundComponent) masterRoundComponent = player.master.gameObject.AddComponent<MasterRoundMasterComponent>();
                        masterRoundComponent.allowedHits = maxHits + 1;
                        masterRoundComponent.teleporterCharging = true;
                        masterRoundComponent.enabled = true;
                        InstanceTracker.Add<MasterRoundMasterComponent>(masterRoundComponent);
                    }
                }
            }
            if (EnableAnnounceOnStart)
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage { baseToken = "RISKOFBULLETSTORM_ARTIFACT_BOSSCHAMBER_START", paramTokens = new[] { maxHits.ToString() } });
        }

        /*public static int GetTrueMasterRoundNumber(int stageCount)
        {
            int trueStageCont = (stageCount) % MasterRoundDefs.Length;
            return trueStageCont;
        }*/

        public void CheckMasterRoundEventResult()
        {
            bool success = true;

            foreach (var component in InstanceTracker.GetInstancesList<MasterRoundMasterComponent>())
            {
                Chat.AddMessage(component.currentHits + " / " + component.allowedHits);
                if (component.currentHits > component.allowedHits)
                {
                    success = false;
                    break;
                }
            }
            if (success)
            {
                //var currentStage = Run.instance.stageClearCount;
                //var MasterRoundToGive = MasterRoundDefs[GetTrueMasterRoundNumber(currentStage)];
                //HelperUtil.GiveItemToPlayers(MasterRoundToGive.itemIndex, true, 1);
                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    player.master.inventory.GiveItem(RoR2Content.Items.Pearl);
                }
            }
        }
        public class MasterRoundMasterComponent : MonoBehaviour
        {
            public bool teleporterCharging = false;
            public int currentHits = 0;
            public int allowedHits = 7;

            public const float maxDistance = 100f;
            public Vector3 teleporterPositionCached;
            public CharacterMaster charMaster;

            public CharacterBody charBody
            {
                get
                {
                    if (!_charBody)
                    {
                        _charBody = charMaster.GetBody();
                    }
                    return _charBody;
                }
                set
                {
                    _charBody = value;
                }
            }
            private CharacterBody _charBody;

            public string charName;

            public void Awake()
            {
                teleporterPositionCached = TeleporterInteraction.instance.transform.position;
                charMaster = gameObject.GetComponent<CharacterMaster>();
            }

            public void OnEnable()
            {
                var body = charMaster.GetBody();
                if (body && !body.GetComponent<MasterRoundBodyComponent>())
                {
                    CharMaster_onBodyStart(body);
                }
                charMaster.onBodyStart += CharMaster_onBodyStart;
            }

            private void CharMaster_onBodyStart(CharacterBody body)
            {
                if (!body.GetComponent<MasterRoundBodyComponent>())
                    body.gameObject.AddComponent<MasterRoundBodyComponent>().masterComponent = this;
            }

            public void OnDisable()
            {
                charMaster.onBodyStart -= CharMaster_onBodyStart;
            }

            public void IncrementHit(DamageReport damageReport)
            {
                if (damageReport.victim == null) return;
                if (!teleporterCharging) return; // no component aka not a player
                if (damageReport.damageInfo.rejected || damageReport.damageInfo.damage < MinimumDamageBeforeInvalidation) return; //minimum damage check
                var attacker = damageReport.damageInfo.attacker;
                if (!attacker) return; // TODO: Figure out if this donkey conflicts, because I feel like it might
                if (attacker == damageReport.victim)
                {
                    if (!AllowSelfDamage) return;
                }
                else
                {
                    var characterBody = attacker.gameObject.GetComponent<CharacterBody>();
                    if (!characterBody) return;
                    if (OnlyAllowTeleBoss)
                    {
                        if (!characterBody.isBoss) return;
                        if (!EnableLunarWisps && characterBody.bodyIndex == ArtifactBossRoom.instance.BodyIndexLunarWisp) return;
                        if (!EnableLunarGolems && characterBody.bodyIndex == ArtifactBossRoom.instance.BodyIndexLunarGolem) return;
                        if (!EnableLunarExploders && characterBody.bodyIndex == ArtifactBossRoom.instance.BodyIndexLunarExploder) return;
                    }
                }
                currentHits++;
                if (instance.EnableShowHitInChat)
                {
                    if (currentHits <= allowedHits)
                    {
                        var characterBody = damageReport.victim.GetComponent<CharacterBody>();
                        string victimName = characterBody ? characterBody.GetUserName() : characterBody.GetDisplayName();
                        string token = currentHits < allowedHits ? "RISKOFBULLETSTORM_ARTIFACT_BOSSCHAMBER_HIT" : "RISKOFBULLETSTORM_ARTIFACT_BOSSCHAMBER_FAIL";
                        string attackerName = damageReport.damageInfo.attacker.GetComponent<CharacterBody>().GetDisplayName();
                        Chat.SendBroadcastChat(
                        new Chat.SimpleChatMessage
                        {
                            baseToken = token,
                            paramTokens = new[] { victimName, currentHits.ToString(), allowedHits.ToString(), attackerName
                        }
                        });
                    }
                }
            }


            /*public void FixedUpdate()
            {
                if (!charBody) return;
                var corePosition = charBody.corePosition;
                var distanceFromTeleporter = Vector3.Distance(teleporterPositionCached, corePosition);
                if (distanceFromTeleporter < maxDistance)
                    return;

                var difference = Mathf.Abs(distanceFromTeleporter - maxDistance);
                var fraction = difference / distanceFromTeleporter;
                var newPosition = Vector3.Lerp(corePosition, teleporterPositionCached, fraction);

                TeleportHelper.TeleportBody(charBody, newPosition);
            }*/

            public class MasterRoundBodyComponent : MonoBehaviour, IOnTakeDamageServerReceiver
            {
                public MasterRoundMasterComponent masterComponent;

                public void OnTakeDamageServer(DamageReport damageReport)
                {
                    masterComponent.IncrementHit(damageReport);
                }
            }
        }

        public override void OnArtifactDisabled()
        {
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal -= TeleporterInteraction_onTeleporterChargedGlobal;
        }
    }
}