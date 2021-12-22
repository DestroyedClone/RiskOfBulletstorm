using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Artifact
{
    class ArtifactPitLord : ArtifactBase<ArtifactPitLord>
    {
        public static ConfigEntry<bool> cfgAffectMonsters;

        public override string ArtifactName => "Artifact of the Pit Lord";

        public override string ArtifactLangTokenName => "ARTIFACT_OF_PITLORD";

        public override string ArtifactDescription => $"When enabled, players {(cfgAffectMonsters.Value ? "and monsters" : "")} don't take damage from falling off stage.";

        public override Sprite ArtifactEnabledIcon => RoR2Content.Items.FallBoots.pickupIconSprite;

        public override Sprite ArtifactDisabledIcon => RoR2Content.Items.FallBoots.pickupIconSprite;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        private void CreateConfig(ConfigFile config)
        {
            cfgAffectMonsters = config.Bind("Artifact: " + ArtifactName, "Monsters Included", false, "If true, then monsters will be teleported back on stage like players do.");
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
            if (!cfgAffectMonsters.Value)
                On.RoR2.MapZone.TryZoneStart -= PlayerNoFallDamage;
            else
                On.RoR2.MapZone.TryZoneStart -= AllNoFallDamage;
        }

        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                _logger.LogMessage($"Artifact {artifactDef} (ID:{artifactDef.artifactIndex}) does not equal {ArtifactDef} (ID:{ArtifactDef.artifactIndex})");
                return;
            }
            if (!cfgAffectMonsters.Value)
                On.RoR2.MapZone.TryZoneStart += PlayerNoFallDamage;
            else
                On.RoR2.MapZone.TryZoneStart += AllNoFallDamage;
        }

        private bool CharacterBodyCanTakeFallDamage(CharacterBody body)
        {
            return (body.bodyFlags & CharacterBody.BodyFlags.IgnoreFallDamage) == CharacterBody.BodyFlags.None;
        }

        private void AllNoFallDamage(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            CharacterBody body = other.GetComponent<CharacterBody>();
            if (body)
            {
                bool canTakeFallDamage = CharacterBodyCanTakeFallDamage(body);
                var teamComponent = body.teamComponent;
                var oldTeamIndex = teamComponent.teamIndex;

                teamComponent.teamIndex = TeamIndex.Player; //Set the team of it to player to avoid it dying when it falls into a hellzone.
                if (canTakeFallDamage) body.bodyFlags &= CharacterBody.BodyFlags.IgnoreFallDamage;

                orig(self, other); //Run the effect of whatever zone it is in on it. Since it is of the Player team, it obviously gets teleported back into the zone.
                
                teamComponent.teamIndex = oldTeamIndex; //Now make it hostile again. Thanks Obama.
                if (canTakeFallDamage) body.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                return;
            }
            orig(self, other);
        }

        //https://github.com/KomradeSpectre/AetheriumMod/blob/rewrite-master/Aetherium/Items/UnstableDesign.cs#L505-L520
        private void PlayerNoFallDamage(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            CharacterBody body = other.GetComponent<CharacterBody>();
            if (body && body.teamComponent && body.teamComponent.teamIndex == TeamIndex.Player)
            {
                //GEM.Oncharacterhitground
                if (CharacterBodyCanTakeFallDamage(body))
                {
                    body.bodyFlags &= CharacterBody.BodyFlags.IgnoreFallDamage;
                    orig(self, other);
                    body.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                    return;
                }
            }
            orig(self, other);
        }
    }
}
