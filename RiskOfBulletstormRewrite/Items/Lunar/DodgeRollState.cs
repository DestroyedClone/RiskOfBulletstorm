using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Items.DodgeRollUtilityReplacement;


namespace RiskOfBulletstormRewrite.Items
{
    //pillaged from CHEFMod
    //( | | | | | | |) <---- baguette as tribute
    public class DodgeRollState : BaseSkillState
    {
		public static float baseDuration = 0.75f;
		private float duration;
		public float speedMultiplier = 2f;
        
		private Vector3 idealDirection;
		private ChildLocator childLocator;
        private Animator animator;
        //this is really gay (derogatory) and doesnt work (fake)
        private Transform modelRoot;
        private Vector3 modelRootOldScale = Vector3.one;
        private Quaternion modelRootOldRot = Quaternion.identity;
        private float calculatedRotationValue = 1;

        private float calcPercentageToApplyVulnerability = 0.5f;

        private bool hasGivenBuff = false;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = GetDuration();
            calculatedRotationValue = 360/duration;
			childLocator = base.GetModelChildLocator();
            animator = base.GetModelAnimator();
            modelRoot = base.GetModelTransform().GetComponentInChildren<SkinnedMeshRenderer>()?.rootBone;
            if (modelRoot)
            {
                modelRootOldScale = modelRoot.localScale;
                modelRootOldRot = modelRoot.localRotation;
            }
            if (base.isAuthority)
            {
				base.gameObject.layer = LayerIndex.fakeActor.intVal;
				base.characterMotor.Motor.RebuildCollidableLayers();
            }
            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2Content.Buffs.Intangible);
            }
			if (!base.characterMotor.isGrounded) base.characterMotor.velocity.y = 15f; 
            if (animator)
                animator.enabled = false;
            if (characterMotor)
                characterMotor.useGravity = false;
            UpdateDirection();
            calcPercentageToApplyVulnerability = 
                Utils.ItemHelpers.GetHyperbolicValue(DodgeRollUtilityReplacement.cfgVulnDuration.Value, DodgeRollUtilityReplacement.instance.GetCount(base.characterBody));
            /* (Utils.ItemHelpers.GetHyperbolicValue(
                DodgeRollUtilityReplacement.cfgVulnDuration.Value,
                DodgeRollUtilityReplacement.cfgVulnDurationReduction.Value,
                DodgeRollUtilityReplacement.instance.GetCount(base.characterBody))); */
            
        }

        public float GetDuration()
        {
            //eventually cloranthy ring
            return baseDuration;
        }

        public override void Update()
        {
            base.Update();

            if (modelRoot)
            {
                var addedRot = new Vector3(calculatedRotationValue, 0,0);
                addedRot *= this.duration/(base.fixedAge+0.1f);
                var newRotation = modelRootOldRot.eulerAngles + addedRot;
                modelRoot.localRotation = Quaternion.Euler(newRotation);
                modelRoot.localScale = Vector3.one*0.7f;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            

            if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
				return;
			}

            if (base.isAuthority)
            {
                if (base.characterBody)
                {
                    base.characterBody.isSprinting = true;
                    if (!hasGivenBuff)
                    {
                        if (base.fixedAge <= calcPercentageToApplyVulnerability
                        || base.fixedAge >= duration)
                        {
                            base.characterBody.AddBuff(Utils.Buffs.DodgeRollBuff);
                        } else {
                            base.characterBody.RemoveBuff(Utils.Buffs.DodgeRollBuff);
                        }
                    }
                }
                //this.UpdateDirection();
                if (base.characterDirection)
                {
					base.characterDirection.moveVector = this.idealDirection;
					if (base.characterMotor && !base.characterMotor.disableAirControlUntilCollision)
					{
						base.characterMotor.rootMotion += this.GetIdealVelocity() * Time.fixedDeltaTime;
					}
                }
            }
        }

        public override void OnExit()
        {
            if (modelRoot)
            {
                modelRoot.localScale = modelRootOldScale;
                modelRoot.localRotation = modelRootOldRot;
            }
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			base.characterMotor.Motor.RebuildCollidableLayers();
            if (base.isAuthority)
                base.characterBody.RemoveBuff(Utils.Buffs.DodgeRollBuff);
            if (animator)
                animator.enabled = true;
            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
            }
            if (characterMotor)
                characterMotor.useGravity = characterMotor.gravityParameters.CheckShouldUseGravity();
            base.OnExit();
        }
        private void UpdateDirection()
		{
			if (base.inputBank)
			{
				Vector2 vector = Util.Vector3XZToVector2XY(base.inputBank.moveVector);
				if (vector != Vector2.zero)
				{
					vector.Normalize();
					this.idealDirection = new Vector3(vector.x, 0f, vector.y).normalized;
				}
			}
		}

		private Vector3 GetIdealVelocity()
		{
			var baseSpeed = base.characterDirection.forward * base.characterBody.moveSpeed * this.speedMultiplier;
            //full speed 85% then taper down rapidly
            var modifiedSpeed = baseSpeed;

            if (fixedAge >= baseDuration * 0.75f)
            {
                modifiedSpeed = base.characterDirection.forward * 
                Mathf.Lerp(base.characterBody.moveSpeed * this.speedMultiplier,
                 characterBody.moveSpeed * 0.25f, 
                 (fixedAge/baseDuration));
            }

            return modifiedSpeed;
		}
    }
}