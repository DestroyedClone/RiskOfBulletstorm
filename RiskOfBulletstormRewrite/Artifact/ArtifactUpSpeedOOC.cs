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
            RoR2.CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
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
                foreach (var cm in CharacterMaster._readOnlyInstancesList)
                {
                    var body = cm.GetBody();
                    if (body && cm.teamIndex == TeamIndex.Player)
                    {
                        body.AddBuff(RoR2Content.Buffs.CloakSpeed);
                    }
                }
            }
        }



        private void CharacterBody_onBodyStartGlobal(CharacterBody characterBody)
        {
            if (NetworkServer.active && characterBody.isBoss && characterBody.inventory && characterBody.inventory.GetItemCount(RoR2Content.Items.AdaptiveArmor) <= 0)
            {
                characterBody.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
            }
        }
    }
}