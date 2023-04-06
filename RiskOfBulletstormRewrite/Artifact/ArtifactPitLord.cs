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
            CharacterBody.onBodyStartGlobal -= GiveOobTeleport;
            On.RoR2.MapZone.TryZoneStart -= AllNoFallDamage;
        }

        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            CharacterBody.onBodyStartGlobal += GiveOobTeleport;
            On.RoR2.MapZone.TryZoneStart += AllNoFallDamage;
        }

        public void GiveOobTeleport(CharacterBody body)
        {
            if (NetworkServer.active)
                body.inventory?.GiveItem(RoR2Content.Items.TeleportWhenOob);
        }

        public static bool CharacterBodyCanTakeFallDamage(CharacterBody body)
        {
            return (body.bodyFlags & CharacterBody.BodyFlags.IgnoreFallDamage) == CharacterBody.BodyFlags.None;
        }

        //https://github.com/KomradeSpectre/AetheriumMod/blob/rewrite-master/Aetherium/Items/UnstableDesign.cs#L505-L520
        private void AllNoFallDamage(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            CharacterBody body = other.GetComponent<CharacterBody>();
            if (body)
            {
                bool canTakeFallDamage = CharacterBodyCanTakeFallDamage(body);
                //var teamComponent = body.teamComponent;
                //var oldTeamIndex = teamComponent.teamIndex;

                //teamComponent.teamIndex = TeamIndex.Player; //Set the team of it to player to avoid it dying when it falls into a hellzone.
                if (canTakeFallDamage) body.bodyFlags &= CharacterBody.BodyFlags.IgnoreFallDamage;

                orig(self, other); //Run the effect of whatever zone it is in on it. Since it is of the Player team, it obviously gets teleported back into the zone.

                //teamComponent.teamIndex = oldTeamIndex; //Now make it hostile again. Thanks Obama.
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