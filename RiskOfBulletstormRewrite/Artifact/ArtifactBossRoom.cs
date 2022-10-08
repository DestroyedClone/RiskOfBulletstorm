using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactBossRoom : ArtifactBase<ArtifactBossRoom>
    {
        public override string ArtifactName => "Artifact of the Boss Chamber";

        public override string ArtifactLangTokenName => "BOSSCHAMBER";

        public override Sprite ArtifactEnabledIcon => Assets.NullSprite;

        public override Sprite ArtifactDisabledIcon => Assets.NullSprite;

        public override void Init(ConfigFile config)
        {
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
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= ClearSpawns;
            CharacterBody.onBodyStartGlobal -= BuffBoss;
        }


        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            //teleporter interaction
            TeleporterInteraction.onTeleporterBeginChargingGlobal += ClearSpawns;
            CharacterBody.onBodyStartGlobal += BuffBoss;
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