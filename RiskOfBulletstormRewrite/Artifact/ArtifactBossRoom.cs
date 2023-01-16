using BepInEx.Configuration;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactBossRoom : ArtifactBase<ArtifactBossRoom>
    {
        public override string ArtifactName => "Artifact of the Boss Chamber";

        public override string ArtifactLangTokenName => "BOSSCHAMBER";

        public override Sprite ArtifactEnabledIcon => LoadSprite(true);

        public override Sprite ArtifactDisabledIcon => LoadSprite(false);

        public override void Init(ConfigFile config)
        {
            return;
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public override void Hooks()
        {
            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;
        }

        private void RunArtifactManager_onArtifactDisabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            CharacterBody.onBodyStartGlobal -= BuffBoss;
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= ClearSpawns;
            On.RoR2.TeleporterInteraction.ChargingState.FixedUpdate -= ChargeTeleporterOnBossKill;
        }


        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            CharacterBody.onBodyStartGlobal += BuffBoss;
            TeleporterInteraction.onTeleporterBeginChargingGlobal += ClearSpawns;
            On.RoR2.TeleporterInteraction.ChargingState.FixedUpdate += ChargeTeleporterOnBossKill;
        }

        private void ChargeTeleporterOnBossKill(On.RoR2.TeleporterInteraction.ChargingState.orig_FixedUpdate orig, EntityStates.BaseState self)
        {
            orig(self);
            var sub = (TeleporterInteraction.ChargingState)self;
            if (NetworkServer.active)
            {
                if (sub.teleporterInteraction.monstersCleared)
                {
                    var itemCount = Util.GetItemCountForTeam(TeamIndex.Player, RoR2Content.Items.FocusConvergence.itemIndex, true);
                    var durationToAdd = 1f/itemCount;
                    sub.teleporterInteraction.holdoutZoneController.charge += durationToAdd;
                    ClearBossResiduals(sub.teleporterInteraction);
                }
            }
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
            
            /* foreach (var minion in CharacterBody.instancesList)
            {
                if (minion && minion.isBoss)
                {
                    
                }
            } */
        }

        public void ClearSpawns(TeleporterInteraction self)
        {
            if (NetworkServer.active)
            {
                foreach (var enemy in CharacterMaster.readOnlyInstancesList)
                {
                    if (!enemy.isBoss && enemy.teamIndex == TeamIndex.Monster)
                    {
                        var body = enemy.GetBody();
                        if (body && body.healthComponent && !body.isBoss)
                        {
                            body.healthComponent.Suicide();
                        }
                    }
                }
                self.bonusDirector.enabled = false;
                //self.bossDirector.enabled = false;
            }
        }

        public void BuffBoss(CharacterBody characterBody)
        {
            if (NetworkServer.active && characterBody.isBoss && characterBody.inventory)
            {
                characterBody.inventory.GiveItem(RoR2Content.Items.Hoof); //14% spd
                characterBody.inventory.GiveItem(RoR2Content.Items.Syringe); //15%atk
                characterBody.inventory.GiveItem(RoR2Content.Items.Pearl, 5); //+50% health
            }
        }
    }
}