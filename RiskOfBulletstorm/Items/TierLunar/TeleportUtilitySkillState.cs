using System;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using EntityStates;
using RiskOfBulletstorm.Shared.Blanks;
using EntityStates.Huntress;
using EntityStates.Mage.Weapon;

namespace RiskOfBulletstorm.Items.Lunar
{
    public class TeleportUtilitySkillState : BaseSkillState
	{
		//Blink
		private Transform modelTransform;
		public static GameObject blinkPrefab;
		private float stopwatch;
		private Vector3 blinkVector = Vector3.zero;
		[SerializeField]
		public float duration = 0.3f;
		[SerializeField]
		public float speedCoefficient = 25f;
		[SerializeField]
		public string beginSoundString;
		[SerializeField]
		public string endSoundString;
		private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;

		//ArrowRain
		public static GameObject areaIndicatorPrefab;
		public static GameObject muzzleFlashEffect;
		private GameObject areaIndicatorInstance;
		private bool shouldFireArrowRain;

		//IceWall
		public static float baseDuration;
		//public static GameObject areaIndicatorPrefab;
		//public static GameObject projectilePrefab;
		public static GameObject muzzleflashEffect;
		public static GameObject goodCrosshairPrefab;
		public static GameObject badCrosshairPrefab;
		public static string prepWallSoundString;
		public static string fireSoundString;
		public static float maxSlopeAngle;
		//private float duration;
		//private float stopwatch;
		private bool goodPlacement;
		//private GameObject areaIndicatorInstance;
		private GameObject cachedCrosshairPrefab;

		public override void OnEnter()
		{
			base.OnEnter();
			areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
			areaIndicatorInstance.transform.localScale = new Vector3(ArrowRain.arrowRainRadius/3, ArrowRain.arrowRainRadius/1.5f, ArrowRain.arrowRainRadius/3);

			//Icewall
			baseDuration = PrepWall.baseDuration;
			muzzleflashEffect = PrepWall.muzzleflashEffect;
			goodCrosshairPrefab = PrepWall.goodCrosshairPrefab;
			badCrosshairPrefab = PrepWall.badCrosshairPrefab;
			prepWallSoundString = PrepWall.prepWallSoundString;
			fireSoundString = PrepWall.fireSoundString;
			maxSlopeAngle = PrepWall.maxSlopeAngle;


			this.duration = baseDuration / this.attackSpeedStat;
			base.characterBody.SetAimTimer(this.duration + 2f);
			this.cachedCrosshairPrefab = base.characterBody.crosshairPrefab;
			this.areaIndicatorInstance = UnityEngine.Object.Instantiate(areaIndicatorPrefab);
			this.UpdateAreaIndicator();
		}

		private float GetMaxDistance() //1000f
        {
			if (this.characterBody?.inventory)
            {
				var itemCount = this.characterBody.inventory.GetItemCount(Items.BloodiedScarf.instance.catalogIndex);
				return BloodiedScarf.instance.BloodiedScarf_RangeBase + BloodiedScarf.instance.BloodiedScarf_RangeIncrease * itemCount;
			}
			return BloodiedScarf.instance.BloodiedScarf_RangeBase;
		}

		private void UpdateAreaIndicator() //check
		{
			this.goodPlacement = false;
			this.areaIndicatorInstance.SetActive(true);
			if (this.areaIndicatorInstance)
			{
				float num = GetMaxDistance();
                Ray aimRay = base.GetAimRay();
                if (Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(aimRay, base.gameObject, out float num2), out RaycastHit raycastHit, num + num2, LayerIndex.world.mask))
                {
                    this.areaIndicatorInstance.transform.position = raycastHit.point;
                    this.areaIndicatorInstance.transform.up = raycastHit.normal;
                    this.areaIndicatorInstance.transform.forward = -aimRay.direction;
                    this.goodPlacement = (Vector3.Angle(Vector3.up, raycastHit.normal) < maxSlopeAngle);
                }
                base.characterBody.crosshairPrefab = (this.goodPlacement ? goodCrosshairPrefab : badCrosshairPrefab);
			}
			this.areaIndicatorInstance.SetActive(this.goodPlacement);
		}

		protected virtual Vector3 GetBlinkVector()
		{
			return base.inputBank.aimDirection;
		}

		private void CreateBlinkEffect(Vector3 origin)
		{
            EffectData effectData = new EffectData
            {
                rotation = Util.QuaternionSafeLookRotation(this.blinkVector),
                origin = origin
            };
            EffectManager.SpawnEffect(blinkPrefab, effectData, false);
		}
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.stopwatch >= this.duration && !base.inputBank.skill3.down && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		private void SetPosition(Vector3 newPosition)
		{
			if (base.characterMotor)
			{
				base.characterMotor.Motor.SetPositionAndRotation(newPosition, Quaternion.identity, true);
			}
		}

		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				if (this.goodPlacement)
				{
					Util.PlaySound(fireSoundString, base.gameObject);
					if (this.areaIndicatorInstance && base.isAuthority)
					{
						Vector3 forward = this.areaIndicatorInstance.transform.forward;
						forward.y = 0f;
						forward.Normalize();
						SetPosition(areaIndicatorInstance.transform.position + Vector3.up);

						var characterBody = this.characterBody;
						if (characterBody)
						{
							MasterBlankItem.FireBlank(characterBody, characterBody.corePosition, 0f, 0f, 6f, false, false, false);
							var inventory = characterBody.inventory;
							if (inventory)
							{
								int scarfCount = inventory.GetItemCount(BloodiedScarf.instance.catalogIndex);
								int debuffStacks = characterBody.GetBuffCount(BloodiedScarf.ScarfVuln);
								if (debuffStacks > 0)
								{
									characterBody.ClearTimedBuffs(BloodiedScarf.ScarfVuln);
								}
								for (uint i = 0; i < scarfCount; i++)
									characterBody.AddTimedBuff(BloodiedScarf.ScarfVuln, 1f);
							}
						}
					}
				}
				else
				{
					base.skillLocator.utility.AddOneStock();
				}
			}
			EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			base.characterBody.crosshairPrefab = this.cachedCrosshairPrefab;

			base.OnExit();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}
	}
}
