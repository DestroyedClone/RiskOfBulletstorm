using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactPitLord : ArtifactBase<ArtifactPitLord>
    {
        public override string ArtifactName => "Artifact of the Pit Lord";

        public override string ArtifactLangTokenName => "PITLORD";

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
            CharacterMaster.onStartGlobal -= GiveOobTeleportToCharacter;
            On.RoR2.MapZone.TryZoneStart -= AllNoFallDamage;
        }

        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            CharacterMaster.onStartGlobal += GiveOobTeleportToCharacter;
            On.RoR2.MapZone.TryZoneStart += AllNoFallDamage;
        }

        private void GiveOobTeleportToCharacter(CharacterMaster characterMaster)
        {
            if (NetworkServer.active)
                characterMaster.inventory?.GiveItem(RoR2Content.Items.TeleportWhenOob);
        }

        private void AllNoFallDamage(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            if (other.TryGetComponent(out CharacterBody body))
            {
                if (!body.bodyFlags.HasFlag(CharacterBody.BodyFlags.IgnoreFallDamage))
                {
                    body.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                    orig(self, other);
                    body.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
                    return;
                }
            }

            orig(self, other);
        }

    }
}