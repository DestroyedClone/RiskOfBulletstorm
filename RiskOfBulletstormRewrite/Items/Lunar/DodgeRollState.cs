using RoR2;
using RoR2.Skills;
using EntityStates;
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
        private GameObject modelRoot;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = baseDuration;
			childLocator = base.GetModelChildLocator();
            animator = base.GetModelAnimator();

            if (base.isAuthority)
            {
				base.gameObject.layer = LayerIndex.fakeActor.intVal;
				base.characterMotor.Motor.RebuildCollidableLayers();
            }
			if (!base.characterMotor.isGrounded) base.characterMotor.velocity.y = 15f; 
            if (animator)
                animator.enabled = false;
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
            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2Content.Buffs.Intangible);
            }
        }

        public override void OnExit()
        {
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			base.characterMotor.Motor.RebuildCollidableLayers();
            animator.enabled = true;
            if (NetworkServer.active)
            {
                characterBody.RemoveBuff(RoR2Content.Buffs.Intangible);
            }
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