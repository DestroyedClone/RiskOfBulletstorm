using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;
using static RiskOfBulletstormRewrite.Artifact.ArtifactPitLord;

namespace RiskOfBulletstormRewrite.Artifact
{
    class ArtifactPitLordPlayer : ArtifactBase<ArtifactPitLordPlayer>
    {
        public override string ArtifactName => "Artifact of the Pit Lord (Players)";

        public override string ArtifactLangTokenName => "ARTIFACT_OF_PITLORD_PLAYER";

        public override string ArtifactDescription => $"When enabled, players don't take damage from falling off stage.";

        public override Sprite ArtifactEnabledIcon => RoR2Content.Items.FallBoots.pickupIconSprite;

        public override Sprite ArtifactDisabledIcon => RoR2Content.Items.FallBoots.pickupIconSprite;

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
            On.RoR2.MapZone.TryZoneStart -= PlayerNoFallDamage;
        }

        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                _logger.LogMessage($"Artifact {artifactDef} (ID:{artifactDef.artifactIndex}) does not equal {ArtifactDef} (ID:{ArtifactDef.artifactIndex})");
                return;
            }
            On.RoR2.MapZone.TryZoneStart += PlayerNoFallDamage;
        }
    }
}
