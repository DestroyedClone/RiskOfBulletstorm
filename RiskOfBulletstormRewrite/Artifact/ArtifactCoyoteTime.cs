using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactCoyoteTime : ArtifactBase<ArtifactCoyoteTime>
    {
        public override string ArtifactName => "Artifact of the Canis Duration";

        public override string ArtifactLangTokenName => "COYOTETIME";

        public override Sprite ArtifactEnabledIcon => Assets.NullSprite;

        public override Sprite ArtifactDisabledIcon => Assets.NullSprite;

        //public static ConfigEntry<float> cfgWindowOfTime;
        public const float windowOfTime = 0.3f;

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
            RoR2.CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.RoR2.CharacterMotor.OnLeaveStableGround += OnLeaveStableGround;
        }

        private void OnLeaveStableGround(On.RoR2.CharacterMotor.orig_OnLeaveStableGround orig, CharacterMotor self)
        {
            var comp = self.gameObject.GetComponent<RBS_CoyoteTime>();
            if (!comp)
            {
                comp = self.gameObject.AddComponent<RBS_CoyoteTime>();
                comp.characterMotor = self;
                comp.jumpCountOnStart = self.jumpCount;
            }
        }

        private class RBS_CoyoteTime : MonoBehaviour
        {
            public CharacterMotor characterMotor;
            public int jumpCountOnStart = 0;
            private float age = 0;
            private float duration => windowOfTime;

            public void FixedUpdate()
            {
                age += Time.fixedDeltaTime;
                if (characterMotor.isGrounded)
                {
                    Destroy(this);
                }
                if (age >= duration)
                {
                    ConsumeJump();
                    Destroy(this);
                }
            }

            public void ConsumeJump()
            {
                if (!characterMotor.isGrounded)
                {
                    if (characterMotor.jumpCount == jumpCountOnStart)
                    {
                        characterMotor.jumpCount += 1;
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