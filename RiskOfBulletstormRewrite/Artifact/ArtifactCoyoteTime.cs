using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactCoyoteTime : ArtifactBase<ArtifactCoyoteTime>
    {
        public override string ArtifactName => "Artifact of the Canis Duration";

        public override string ArtifactLangTokenName => "COYOTETIME";

        public override Sprite ArtifactEnabledIcon => LoadSprite(true);

        public override Sprite ArtifactDisabledIcon => LoadSprite(false);

        //public static ConfigEntry<float> cfgWindowOfTime;
        public const float windowOfTime = 0.2f;

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
            On.RoR2.CharacterMotor.OnLeaveStableGround -= OnLeaveStableGround;
        }


        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            On.RoR2.CharacterMotor.OnLeaveStableGround += OnLeaveStableGround;
        }

        private void OnLeaveStableGround(On.RoR2.CharacterMotor.orig_OnLeaveStableGround orig, CharacterMotor self)
        {
            int initJumpCount = self.jumpCount;
            orig(self);
            if (self.jumpCount != initJumpCount)
            {
                self.jumpCount = initJumpCount;
                var comp = self.gameObject.GetComponent<RBS_CoyoteTime>();
                if (!comp)
                {
                    var bodyESM = EntityStateMachine.FindByCustomName(self.gameObject, "Body");

                    if (bodyESM && bodyESM.IsInMainState())
                    {
                        comp = self.gameObject.AddComponent<RBS_CoyoteTime>();
                        comp.characterMotor = self;
                        comp.jumpCountOnStart = initJumpCount;
                    }
                }
            }
        }

        private class RBS_CoyoteTime : MonoBehaviour
        {
            public CharacterMotor characterMotor;
            public int jumpCountOnStart = 0;
            private float age = 0;
            private float duration => windowOfTime;

            public void Awake()
            {
                if (!characterMotor)
                {
                    characterMotor = gameObject.GetComponent<CharacterMotor>();
                }
                characterMotor.useGravity = false;
            }

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

            public void OnDestroy()
            {
                characterMotor.useGravity = characterMotor.gravityParameters.CheckShouldUseGravity();
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
    }
}