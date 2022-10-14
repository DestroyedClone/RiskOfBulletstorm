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

        public override Sprite ArtifactEnabledIcon => LoadSprite(true);

        public override Sprite ArtifactDisabledIcon => LoadSprite(false);

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
            TeleporterInteraction.onTeleporterChargedGlobal -= OnTeleporterCharged;
        }


        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            TeleporterInteraction.onTeleporterChargedGlobal += OnTeleporterCharged;
        }

        public void OnTeleporterCharged(TeleporterInteraction self)
        {
            if (NetworkServer.active)
                {
                    //Chat.AddMessage("speedup 1");
                    foreach (var player in PlayerCharacterMasterController.instances)
                    {
                    //Chat.AddMessage($"speedup: {player.GetDisplayName()}");
                        if (player.master)
                        {
                            //Chat.AddMessage("speedup: has master");
                            var body = player.master.GetBody();
                            if (body)
                            {
                                //Chat.AddMessage("speedup: has body");
                                body.AddBuff(RoR2Content.Buffs.CloakSpeed);
                            }
                        }
                    }
                }
        }
    }
}