using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactPitLord : ArtifactBase<ArtifactPitLord>
    {
        public override string ArtifactName => "Artifact of the Pit Lord";

        public override string ArtifactLangTokenName => "PITLORD";

        public override Sprite ArtifactEnabledIcon => LoadSprite(true);

        public override Sprite ArtifactDisabledIcon => LoadSprite(false);

        public override string WikiLink => "https://thunderstore.io/package/DestroyedClone/RiskOfBulletstorm/wiki/218-artifact-pit-lord/";

        public static List<CharacterBody> recentlyTeleported = new List<CharacterBody>();

        private void GiveOobTeleportToCharacter(CharacterMaster characterMaster)
        {
            if (NetworkServer.active)
                characterMaster.inventory?.GiveItem(RoR2Content.Items.TeleportWhenOob);
        }

        private void AllNoFallDamage(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            if (other.TryGetComponent(out CharacterBody body)
                && !body.bodyFlags.HasFlag(CharacterBody.BodyFlags.IgnoreFallDamage))
            {
                body.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                orig(self, other);
                body.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
                return;
            }

            orig(self, other);
        }

        public override void OnArtifactEnabled()
        {
            CharacterMaster.onStartGlobal += GiveOobTeleportToCharacter;
            //On.RoR2.MapZone.TryZoneStart += AllNoFallDamage;
            Stage.onServerStageBegin += Stage_onServerStageBegin;
            On.RoR2.TeleportHelper.OnTeleport += TeleportHelper_OnTeleport;
            On.RoR2.GlobalEventManager.OnCharacterHitGroundServer += GlobalEventManager_OnCharacterHitGroundServer;
        }

        private void GlobalEventManager_OnCharacterHitGroundServer(On.RoR2.GlobalEventManager.orig_OnCharacterHitGroundServer orig, GlobalEventManager self, CharacterBody characterBody, Vector3 impactVelocity)
        {
            orig(self, characterBody, impactVelocity);
            if (!recentlyTeleported.Contains(characterBody)) return;
            characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            recentlyTeleported.Remove(characterBody);
        }

        private void TeleportHelper_OnTeleport(On.RoR2.TeleportHelper.orig_OnTeleport orig, GameObject gameObject, Vector3 newPosition, Vector3 delta)
        {
            orig(gameObject, newPosition, delta);
            if (gameObject.TryGetComponent(out CharacterBody characterBody)
                && !characterBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.IgnoreFallDamage)
                && !recentlyTeleported.Contains(characterBody))
            {
                recentlyTeleported.Add(characterBody);
                characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
        }

        private void Stage_onServerStageBegin(Stage obj)
        {
            recentlyTeleported.Clear();
        }

        public override void OnArtifactDisabled()
        {
            CharacterMaster.onStartGlobal -= GiveOobTeleportToCharacter;
            //On.RoR2.MapZone.TryZoneStart += AllNoFallDamage;
            Stage.onServerStageBegin -= Stage_onServerStageBegin;
            On.RoR2.TeleportHelper.OnTeleport -= TeleportHelper_OnTeleport;
            On.RoR2.GlobalEventManager.OnCharacterHitGroundServer -= GlobalEventManager_OnCharacterHitGroundServer;
        }
    }
}