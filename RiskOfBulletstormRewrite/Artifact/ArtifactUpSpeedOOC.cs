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
        public static ConfigEntry<float> cfgMoveSpeedAdditiveMultiplier;

        public override string ArtifactName => "Artifact of the Swift Post-Battle";

        public override string ArtifactLangTokenName => "UPSPEEDOOC";

        public override Sprite ArtifactEnabledIcon => LoadSprite(true);

        public override Sprite ArtifactDisabledIcon => LoadSprite(false);

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            base.CreateConfig(config);
            cfgMoveSpeedAdditiveMultiplier = config.Bind(ConfigCategory, Assets.cfgMoveSpeedAdditiveMultiplierKey, 0.45f, Assets.cfgMoveSpeedAdditiveMultiplierDesc);
        }

        public override void Hooks()
        {
            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Utils.Buffs.ArtifactSpeedUpBuff))
            {
                args.moveSpeedMultAdd += cfgMoveSpeedAdditiveMultiplier.Value;
            }
        }

        private void RunArtifactManager_onArtifactDisabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            TeleporterInteraction.onTeleporterChargedGlobal -= OnTeleporterCharged;
        }

        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            TeleporterInteraction.onTeleporterChargedGlobal += OnTeleporterCharged;
        }

        public void OnTeleporterCharged(TeleporterInteraction self)
        {
            if (NetworkServer.active)
            {
                self.gameObject.AddComponent<RiskOfBulletstorm_ArtifactSpeedUpController>();
            }
        }

        public class RiskOfBulletstorm_ArtifactSpeedUpController : MonoBehaviour
        {
            public void Awake()
            {
                CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;

                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    if (player.master)
                    {
                        var body = player.master.GetBody();
                        if (body)
                        {
                            body.AddBuff(Utils.Buffs.ArtifactSpeedUpBuff);
                        }
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