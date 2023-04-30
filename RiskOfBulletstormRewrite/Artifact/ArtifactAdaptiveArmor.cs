using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactAdaptiveArmor : ArtifactBase<ArtifactAdaptiveArmor>
    {
        public override string ArtifactName => "Artifact of the Adaptive Armor";

        public override string ArtifactLangTokenName => "ADAPTIVEARMORBOSSES";

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
            CharacterMaster.onStartGlobal -= GiveAdaptiveArmorToBoss;
        }

        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            CharacterMaster.onStartGlobal += GiveAdaptiveArmorToBoss;
        }

        private void GiveAdaptiveArmorToBoss(CharacterMaster characterMaster)
        {
            if (NetworkServer.active && characterMaster.isBoss && characterMaster.inventory?.GetItemCount(RoR2Content.Items.AdaptiveArmor) <= 0)
            {
                characterMaster.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
            }
        }
    }
}