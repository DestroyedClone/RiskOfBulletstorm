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

        public override string WikiLink => "https://thunderstore.io/package/DestroyedClone/RiskOfBulletstorm/wiki/219-artifact-adaptive-armor/";

        private void GiveAdaptiveArmorToBoss(CharacterMaster characterMaster)
        {
            if (NetworkServer.active && characterMaster.isBoss && characterMaster.inventory?.GetItemCount(RoR2Content.Items.AdaptiveArmor) <= 0)
            {
                characterMaster.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
            }
        }

        public override void OnArtifactEnabled()
        {
            CharacterMaster.onStartGlobal += GiveAdaptiveArmorToBoss;
        }

        public override void OnArtifactDisabled()
        {
            CharacterMaster.onStartGlobal -= GiveAdaptiveArmorToBoss;
        }
    }
}