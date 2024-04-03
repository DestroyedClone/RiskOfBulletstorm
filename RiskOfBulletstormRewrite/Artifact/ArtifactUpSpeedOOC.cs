using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactUpSpeedOOC : ArtifactBase<ArtifactUpSpeedOOC>
    {
        public const float moveSpeedAdditiveMultiplier = 0.45f;

        public override string ArtifactName => "Artifact of the Swift Post-Battle";

        public override string ArtifactLangTokenName => "UPSPEEDOOC";

        public override Sprite ArtifactEnabledIcon => LoadSprite(true);

        public override Sprite ArtifactDisabledIcon => LoadSprite(false);

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (!sender.HasBuff(Utils.Buffs.ArtifactSpeedUpBuff)) return;
            args.moveSpeedMultAdd += moveSpeedAdditiveMultiplier;
        }

        public void OnTeleporterCharged(TeleporterInteraction self)
        {
            if (!NetworkServer.active) return;
            self.gameObject.AddComponent<RiskOfBulletstorm_ArtifactSpeedUpController>();
        }

        public override void OnArtifactEnabled()
        {
            TeleporterInteraction.onTeleporterChargedGlobal += OnTeleporterCharged;
        }

        public override void OnArtifactDisabled()
        {
            TeleporterInteraction.onTeleporterChargedGlobal -= OnTeleporterCharged;
        }

        public class RiskOfBulletstorm_ArtifactSpeedUpController : MonoBehaviour
        {
            public void Awake()
            {
                CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;

                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    if (!player.master) continue;

                    var body = player.master.GetBody();
                    if (body)
                    {
                        body.AddBuff(Utils.Buffs.ArtifactSpeedUpBuff);
                    }
                }
            }

            private void CharacterBody_onBodyStartGlobal(CharacterBody characterBody)
            {
                if (characterBody.isPlayerControlled)
                    characterBody.AddBuff(Utils.Buffs.ArtifactSpeedUpBuff);
            }

            public void OnDestroy()
            {
                CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
            }
        }
    }
}