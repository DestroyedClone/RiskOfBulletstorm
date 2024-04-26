using System;
using EntityStates;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Characters.Enemies.Lord_of_the_Jammed
{
    //Heretic.SpawnState
    //BrotherMonster.ThrownSpawnState
    //BrotherMonster.SkySpawnState
    //EntityStates.FlyingVermin.Mode.GrantFlight
    //EntityStates.Vulture.Fly
    public class LOTJSpawnState : BaseState
    {
        public LOTJSpawnState()
        {
            AvailabilityManager.EntityStateMachine.availability.CallWhenAvailable(() =>
            {
            });
        }

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Body", "ExitSkyLeap", "SkyLeap.playbackRate", EntityStates.BrotherMonster.SkySpawnState.duration);
            this.PlayAnimation("FullBody Override", "BufferEmpty");
            Util.PlaySound(EntityStates.BrotherMonster.SkySpawnState.soundString, base.gameObject);

            Util.PlaySound(EntityStates.Heretic.SpawnState.spawnSoundString, base.gameObject);
            EffectManager.SimpleEffect(EntityStates.Heretic.SpawnState.effectPrefab, base.characterBody.corePosition, Quaternion.identity, false);
            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                CharacterModel component = modelTransform.GetComponent<CharacterModel>();
                if (component && EntityStates.Heretic.SpawnState.overlayMaterial)
                {
                    TemporaryOverlay temporaryOverlay = component.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = EntityStates.Heretic.SpawnState.overlayDuration;
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = EntityStates.Heretic.SpawnState.overlayMaterial;
                    temporaryOverlay.inspectorCharacterModel = component;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.animateShaderAlpha = true;
                }
            }
            this.characterGravityParameterProvider = base.gameObject.GetComponent<ICharacterGravityParameterProvider>();
            this.characterFlightParameterProvider = base.gameObject.GetComponent<ICharacterFlightParameterProvider>();
            if (this.characterGravityParameterProvider != null)
            {
                CharacterGravityParameters gravityParameters = this.characterGravityParameterProvider.gravityParameters;
                gravityParameters.channeledAntiGravityGranterCount++;
                this.characterGravityParameterProvider.gravityParameters = gravityParameters;
            }
            if (this.characterFlightParameterProvider != null)
            {
                CharacterFlightParameters flightParameters = this.characterFlightParameterProvider.flightParameters;
                flightParameters.channeledFlightGranterCount++;
                this.characterFlightParameterProvider.flightParameters = flightParameters;
            }
            if (base.characterMotor)
            {
                //base.characterMotor.velocity.y = Fly.launchSpeed;
                base.characterMotor.Motor.ForceUnground();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge > EntityStates.BrotherMonster.SkySpawnState.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        protected ICharacterGravityParameterProvider characterGravityParameterProvider;

        protected ICharacterFlightParameterProvider characterFlightParameterProvider;
    }
}
