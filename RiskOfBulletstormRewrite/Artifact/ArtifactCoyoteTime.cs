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

        public static ConfigEntry<float> cfgWindowOfTimeForActivation;

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
            cfgWindowOfTimeForActivation = config.Bind(ConfigCategory, "Time Window", 0.2f, "The amount of time in seconds that the character can walk off a platform before they can no longer jump.");
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
                if (!self.gameObject.TryGetComponent(out RiskOfBulletstorm_CoyoteTimeController _))
                {
                    if (EntityStateMachine.FindByCustomName(self.gameObject, "Body")?.IsInMainState() == true)
                    {
                        RiskOfBulletstorm_CoyoteTimeController comp = self.gameObject.AddComponent<RiskOfBulletstorm_CoyoteTimeController>();
                        comp.characterMotor = self;
                        comp.jumpCountOnStart = initJumpCount;
                    }
                }
            }
        }

        private class RiskOfBulletstorm_CoyoteTimeController : MonoBehaviour
        {
            public CharacterMotor characterMotor;
            public int jumpCountOnStart = 0;
            private float age = 0;
            private float Duration => cfgWindowOfTimeForActivation.Value;

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

                if (characterMotor.isGrounded || age >= Duration)
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