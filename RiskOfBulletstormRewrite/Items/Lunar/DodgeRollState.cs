using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;


namespace RiskOfBulletstormRewrite.Items
{
    //pillaged from CHEFMod
    public class DodgeRollState : BaseSkillState
    {
		public static float baseDuration = 1.5f;
		private float duration;
		public float speedMultiplier = 2f;
        
		private Vector3 idealDirection;
		private ChildLocator childLocator;
        private Animator animator;
        private Transform modelRoot;
        private Vector3 modelRootOldScale = Vector3.one;
        private Quaternion modelRootOldRot = Quaternion.identity;
        private float calculatedRotationValue = 1;

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
        }

        public float GetDuration()
        {
            //eventually cloranthy ring
            return baseDuration;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (modelRoot)
            {
                var addedRot = Vector3.one * calculatedRotationValue;
                addedRot *= this.duration/(base.fixedAge+0.1f);
                var newRotation = modelRootOldRot.eulerAngles + addedRot;
                modelRoot.localRotation = Quaternion.Euler(newRotation);
                modelRoot.localScale = Vector3.one*0.7f;
            }
            
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
                }
                this.UpdateDirection();
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
			return base.characterDirection.forward * base.characterBody.moveSpeed * this.speedMultiplier;// Mathf.Pow((base.characterBody.moveSpeed * base.characterBody.moveSpeed) + 300f, 0.5f); //base.characterBody.moveSpeed * this.speedMultiplier;
		}
    }
}