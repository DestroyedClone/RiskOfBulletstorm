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


        public static bool CanCharacterBodyTakeFallDamage(CharacterBody body)
        {
            return body.bodyFlags.HasFlag(CharacterBody.BodyFlags.IgnoreFallDamage);
        }

        private void AllNoFallDamage(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            CharacterBody body = other.GetComponent<CharacterBody>();
            if (body)
            {
                bool canTakeFallDamage = CanCharacterBodyTakeFallDamage(body);
                if (canTakeFallDamage) body.bodyFlags &= CharacterBody.BodyFlags.IgnoreFallDamage;

                orig(self, other);

                if (canTakeFallDamage) body.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                /* if (body.master && body.master.TryGetComponent<RoR2.CharacterAI.BaseAI>(out RoR2.CharacterAI.BaseAI baseAI))
                {
                    if (baseAI.currentEnemy == null)
                        baseAI.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                } */
                return;
            }
            orig(self, other);
        }
    }
}