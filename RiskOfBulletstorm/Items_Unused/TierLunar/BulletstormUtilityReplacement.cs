using System;
using System.Collections.Generic;
using System.Text;
using EntityStates;
using EntityStates.Huntress;
using EntityStates.Mage.Weapon;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstorm.Items.Lunar
{
    public class BulletstormUtilityReplacement : BaseState
    {
        public RaycastHit raycastHit;
        public static float baseDuration;
        public static GameObject areaIndicatorPrefab;
        public static GameObject muzzleflashEffect;
        public static GameObject goodCrosshairPrefab;
        public static GameObject badCrosshairPrefab;
        public static string teleportSoundString;
        public static float maxDistance;
        public static string fireSoundString;
        public static float maxSlopeAngle;

        private float duration;
        private float stopwatch;
        private bool goodPlacement;
        private GameObject areaIndicatorInstance;
        private GameObject cachedCrosshairPrefab;
        private Transform modelTransform;
        public static GameObject blinkPrefab;
        private Vector3 blinkVector = Vector3.zero;

        [SerializeField]
        public float speedCoefficient = 25f;
        [SerializeField]
        public string beginSoundString;
        [SerializeField]
        public string endSoundString;

        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;

		public override void OnEnter()
		{
			base.OnEnter();
			baseDuration = PrepWall.baseDuration;
			teleportSoundString = PrepWall.prepWallSoundString;
			maxDistance = PrepWall.maxDistance;
            maxSlopeAngle = PrepWall.maxSlopeAngle;
			goodCrosshairPrefab = PrepWall.goodCrosshairPrefab;
			badCrosshairPrefab = PrepWall.badCrosshairPrefab;
			fireSoundString = PrepWall.fireSoundString;
			muzzleflashEffect = PrepWall.muzzleflashEffect;
			blinkPrefab = BlinkState.blinkPrefab;
			this.duration = baseDuration / this.attackSpeedStat;
			base.characterBody.SetAimTimer(this.duration + 2f);
			this.cachedCrosshairPrefab = base.characterBody.crosshairPrefab;
			Util.PlaySound(teleportSoundString, base.gameObject);
			this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
			this.areaIndicatorInstance.transform.localScale = new Vector3(1f, 3f, 1f);
			this.UpdateAreaIndicator();
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002178 File Offset: 0x00000378
		private void UpdateAreaIndicator()
		{
			this.goodPlacement = false;
			this.areaIndicatorInstance.SetActive(true);
			bool flag = this.areaIndicatorInstance;
			if (flag)
			{
				float num = maxDistance;
                Ray aimRay = base.GetAimRay();
                bool flag2 = Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(aimRay, base.gameObject, out float num2), out this.raycastHit, num + num2, LayerIndex.world.mask);
				if (flag2)
				{
					this.areaIndicatorInstance.transform.position = this.raycastHit.point;
					this.areaIndicatorInstance.transform.up = this.raycastHit.normal;
					this.areaIndicatorInstance.transform.forward = -aimRay.direction;
					this.goodPlacement = (Vector3.Angle(Vector3.up, this.raycastHit.normal) < maxSlopeAngle);
				}
				base.characterBody.crosshairPrefab = (this.goodPlacement ? goodCrosshairPrefab : badCrosshairPrefab);
			}
			this.areaIndicatorInstance.SetActive(this.goodPlacement);
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000022A0 File Offset: 0x000004A0
		public override void Update()
		{
			base.Update();
			this.UpdateAreaIndicator();
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000022B4 File Offset: 0x000004B4
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			bool flag = this.stopwatch >= this.duration && !base.inputBank.skill3.down && base.isAuthority;
			if (flag)
			{
				this.outer.SetNextStateToMain();
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002318 File Offset: 0x00000518
		public override void OnExit()
		{
			bool flag = !this.outer.destroying;
			if (flag)
			{
				bool flag2 = this.goodPlacement;
				if (flag2)
				{
					base.PlayAnimation("Gesture, Additive", "FireWall");
					Util.PlaySound(fireSoundString, base.gameObject);
					bool flag3 = this.areaIndicatorInstance && base.isAuthority;
					if (flag3)
					{
						Vector3 forward = this.areaIndicatorInstance.transform.forward;
						forward.y = 0f;
						forward.Normalize();
						//Vector3 vector = Vector3.Cross(Vector3.up, forward);
						this.modelTransform = base.GetModelTransform();
						bool flag5 = this.modelTransform;
						if (flag5)
						{
							this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
							this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
						}
						bool flag6 = this.characterModel;
						if (flag6)
						{
							this.characterModel.invisibilityCount++;
						}
						bool flag7 = this.hurtboxGroup;
						if (flag7)
						{
							HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
							int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
							hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
						}
						this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
						bool flag8 = base.characterMotor;
						if (flag8)
						{
							base.characterMotor.velocity = Vector3.zero;
							Reflection.SetFieldValue<Vector3>(base.characterMotor.Motor, "_internalTransientPosition", this.areaIndicatorInstance.transform.position + this.raycastHit.normal);
						}
						bool flag9 = !this.outer.destroying;
						if (flag9)
						{
							Util.PlaySound(this.endSoundString, base.gameObject);
							this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
							this.modelTransform = base.GetModelTransform();
							bool flag10 = this.modelTransform;
							if (flag10)
							{
								TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
								temporaryOverlay.duration = 0.6f;
								temporaryOverlay.animateShaderAlpha = true;
								temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
								temporaryOverlay.destroyComponentOnEnd = true;
								temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashBright");
								temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
								TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
								temporaryOverlay2.duration = 0.7f;
								temporaryOverlay2.animateShaderAlpha = true;
								temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
								temporaryOverlay2.destroyComponentOnEnd = true;
								temporaryOverlay2.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashExpanded");
								temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
							}
						}
						bool flag11 = this.characterModel;
						if (flag11)
						{
							this.characterModel.invisibilityCount--;
						}
						bool flag12 = this.hurtboxGroup;
						if (flag12)
						{
							HurtBoxGroup hurtBoxGroup2 = this.hurtboxGroup;
							int hurtBoxesDeactivatorCounter2 = hurtBoxGroup2.hurtBoxesDeactivatorCounter - 1;
							hurtBoxGroup2.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter2;
						}
						var characterBody = this.characterBody;
						if (characterBody)
                        {
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
					base.PlayCrossfade("Gesture, Additive", "BufferEmpty", 0.2f);
				}
			}
			EntityState.Destroy(this.areaIndicatorInstance.gameObject);
			base.characterBody.crosshairPrefab = this.cachedCrosshairPrefab;
			base.OnExit();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x000026F4 File Offset: 0x000008F4
		private void CreateBlinkEffect(Vector3 origin)
		{
            EffectData effectData = new EffectData
            {
                rotation = Util.QuaternionSafeLookRotation(this.blinkVector),
                origin = origin
            };
            EffectManager.SpawnEffect(blinkPrefab, effectData, false);
		}

		public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }


    }
}
