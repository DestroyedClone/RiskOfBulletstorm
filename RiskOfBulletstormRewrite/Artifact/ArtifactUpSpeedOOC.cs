using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactUpSpeedOOC : ArtifactBase<ArtifactUpSpeedOOC>
    {
        public override string ArtifactName => "Artifact of the Swift Post-Battle";

        public override string ArtifactLangTokenName => "UPSPEEDOOC";

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
            TeleporterInteraction.onTeleporterFinishGlobal -= TrackTeleporterDone;
        }


        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            TeleporterInteraction.onTeleporterFinishGlobal += TrackTeleporterDone;
        }

        public void TrackTeleporterDone(TeleporterInteraction self)
        {
            if (NetworkServer.active)
            {
                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    if (player.master)
                    {
                        var body = player.master.GetBody();
                        if (body)
                        {
                            body.AddBuff(RoR2Content.Buffs.CloakSpeed);
                        }
                    }
                }
            }
        }
    }
}