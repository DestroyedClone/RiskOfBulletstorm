using EntityStates;
using EntityStates.Huntress;
using EntityStates.Mage.Weapon;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class TeleportUtilitySkillState : BaseSkillState
    {
        //Blink
        //private Transform modelTransform;
        public static GameObject blinkPrefab;

        private float stopwatch;
        private Vector3 blinkVector = Vector3.zero;

        [SerializeField]
        public float duration = 0.1f;

        [SerializeField]
        public string beginSoundString;

        [SerializeField]
        public string endSoundString;

        //private CharacterModel characterModel;
        //private HurtBoxGroup hurtboxGroup;

        //ArrowRain
        public static GameObject areaIndicatorPrefab;

        public static GameObject muzzleFlashEffect;
        private GameObject areaIndicatorInstance;
        //private bool shouldFireArrowRain;

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

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

        private Transform modelRoot;

        public override void OnEnter()
        {
            base.OnEnter();
            modelRoot = base.GetModelTransform().GetComponentInChildren<SkinnedMeshRenderer>()?.rootBone;

            //areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(modelRoot?.parent.parent.gameObject ?? ArrowRain.areaIndicatorPrefab);//(ArrowRain.areaIndicatorPrefab);

            base.GetModelAnimator().enabled = false;
            areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(GetModelTransform().gameObject ?? ArrowRain.areaIndicatorPrefab);
            base.GetModelAnimator().enabled = true;
            //areaIndicatorInstance.transform.localScale = new Vector3(ArrowRain.arrowRainRadius / 3, ArrowRain.arrowRainRadius / 1.5f, ArrowRain.arrowRainRadius / 3);

            //Icewall
            baseDuration = PrepWall.baseDuration / 2;
            muzzleflashEffect = PrepWall.muzzleflashEffect;
            goodCrosshairPrefab = PrepWall.goodCrosshairPrefab;
            badCrosshairPrefab = PrepWall.badCrosshairPrefab;
            prepWallSoundString = PrepWall.prepWallSoundString;
            fireSoundString = PrepWall.fireSoundString;
            maxSlopeAngle = PrepWall.maxSlopeAngle;

            this.duration = baseDuration / this.attackSpeedStat;
            base.characterBody.SetAimTimer(this.duration + 2f);
            cachedCrosshairPrefab = base.characterBody.defaultCrosshairPrefab;
            this.UpdateAreaIndicator();
        }

        private float GetMaxDistance() //1000f
        {
            if (characterBody && characterBody.inventory)
            {
                return BloodiedScarf.instance.GetStack(BloodiedScarf.cfgTeleportRange, BloodiedScarf.cfgTeleportRangePerStack, this.characterBody.inventory.GetItemCount(Items.BloodiedScarf.instance.ItemDef));
            }
            return BloodiedScarf.cfgTeleportRange;
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
                if (areaIndicatorInstance != goodPlacement || crosshairOverrideRequest == null)
                {
                    CrosshairUtils.OverrideRequest overrideRequest = crosshairOverrideRequest;
                    overrideRequest?.Dispose();
                    crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, goodPlacement ? TeleportUtilitySkillState.goodCrosshairPrefab : TeleportUtilitySkillState.badCrosshairPrefab, CrosshairUtils.OverridePriority.Skill);
                }
            }
            //this.areaIndicatorInstance.SetActive(this.goodPlacement);
            areaIndicatorInstance.transform.localScale = Vector3.one * (goodPlacement ? 1f : -1f);
        }

        protected virtual Vector3 GetBlinkVector()
        {
            return base.inputBank.aimDirection;
        }

        private void CreateBlinkEffect(Vector3 origin)
        {
            return;
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
                CreateBlinkEffect(outer.commonComponents.characterBody.corePosition);
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

                        if (UnityEngine.Networking.NetworkServer.active && characterBody)
                        {
                            characterBody.AddTimedBuff(Utils.Buffs.BloodiedScarfBuff, BloodiedScarf.cfgDamageVulnerabilityDuration, 1);
                            /* //MasterBlankItem.FireBlank(characterBody, characterBody.corePosition, 0f, 0f, 6f, false, false, false);
							var inventory = characterBody.inventory;
							if (inventory)
							{
								int scarfCount = inventory.GetItemCount(BloodiedScarf.instance.ItemDef);
								int debuffStacks = characterBody.GetBuffCount(Utils.Buffs.BloodiedScarfBuff);
								if (debuffStacks > 0)
								{
									characterBody.ClearTimedBuffs(Utils.Buffs.BloodiedScarfBuff);
								}
								for (uint i = 0; i < scarfCount; i++)
									characterBody.AddTimedBuff(Utils.Buffs.BloodiedScarfBuff, 1f);
							} */
                        }
                    }
                }
                else
                {
                    base.skillLocator.utility.AddOneStock();
                }
            }
            EntityState.Destroy(this.areaIndicatorInstance.gameObject);
            CrosshairUtils.OverrideRequest overrideRequest = crosshairOverrideRequest;
            if (overrideRequest != null)
            {
                overrideRequest.Dispose();
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}