using EntityStates;
using KinematicCharacterController;
using System;
using System.Collections.Generic;
using System.Text;
using static RoR2.Console;
using UnityEngine;
using RoR2;

namespace RiskOfBulletstormRewrite.Characters.Enemies.Lord_of_the_Jammed
{
    internal class NoclipCharacterMain : BaseCharacterMain
    {
        public KinematicCharacterMotor kcm;
        public CharacterMotor motor; 

        public override void OnEnter()
        {
            base.OnEnter();
            kcm = outer.GetComponent<KinematicCharacterMotor>();
            motor = outer.commonComponents.characterBody.characterMotor;

            if (kcm)
            {
                kcm.CollidableLayers = 0; //doesnt work
            }
            if (motor)
            {
                motor.useGravity = false;
            }
        }

        public bool IsActivated = false;
        internal void InternalToggle()
        {
            if (IsActivated)
            {
                if (kcm)
                {
                    kcm.RebuildCollidableLayers();
                }
                if (motor)
                {
                    motor.useGravity = motor.gravityParameters.CheckShouldUseGravity();
                }
            }
            else
            {
                if (kcm)
                {
                    kcm.CollidableLayers = 0;
                }
                if (motor)
                {
                    motor.useGravity = false;
                }
            }

            IsActivated = !IsActivated;
        }

        public override void Update()
        {
            base.Update(); 
            /*if (kcm) // when respawning or things like that, call the toggle to set the variables correctly again
            {
                if (kcm.CollidableLayers != 0)
                {
                    InternalToggle();
                    InternalToggle();
                }
            }*/

            var inputBank = outer.commonComponents.characterBody.GetComponent<InputBankTest>();
            var motor = outer.commonComponents.characterBody.characterMotor;
            if (inputBank && motor)
            {
                var forwardDirection = inputBank.moveVector.normalized;
                var aimDirection = inputBank.aimDirection.normalized;
                var isForward = Vector3.Dot(forwardDirection, aimDirection) > 0f;

                var isSprinting = outer.commonComponents.characterBody.inputBank.sprint.down;
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                //var isStrafing = outer.commonComponents.characterBody.inputBank..GetAxis("MoveVertical") != 0f;
                var scalar = isSprinting ? 100f : 50f;

                var velocity = forwardDirection * scalar;
                /*if (true)
                {
                    velocity.y = aimDirection.y * (isForward ? scalar : -scalar);
                }*/
                /*if (inputBank.jump.down)
                {
                    velocity.y = 50f;
                }*/

                if (motor)
                {
                    motor.velocity = velocity;
                }
            }
        }
    }
}
