using BepInEx.Configuration;
using EntityStates;
using EntityStates.Huntress;
using EntityStates.Mage.Weapon;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class BloodiedScarf : ItemBase<BloodiedScarf>
    {
        public override string ItemName => "Bloodied Scarf";

        public override string ItemLangTokenName => "BLOODIEDSCARF";

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.CannotSteal,
            ItemTag.Cleansable
        };

        public static SkillDef teleportSkillDef;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
            CreateSkillDef();
        }

        public void CreateSkillDef()
        {
            teleportSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            teleportSkillDef.activationState = new SerializableEntityStateType(typeof(Teleport));
            teleportSkillDef.activationStateMachineName = "Weapon";
            teleportSkillDef.baseMaxStock = 1;
            teleportSkillDef.baseRechargeInterval = 12;
            teleportSkillDef.beginSkillCooldownOnSkillEnd = true;
            teleportSkillDef.canceledFromSprinting = false;
            teleportSkillDef.cancelSprintingOnActivation = false;
            teleportSkillDef.dontAllowPastMaxStocks = false;
            teleportSkillDef.forceSprintDuringState = true;
            teleportSkillDef.fullRestockOnAssign = true;
            teleportSkillDef.icon = Assets.NullSprite;
            teleportSkillDef.interruptPriority = InterruptPriority.Pain;
            teleportSkillDef.isCombatSkill = false;
            teleportSkillDef.keywordTokens = new string[]
            {
                "KEYWORD_AGILE"
            };
            teleportSkillDef.mustKeyPress = true;
            teleportSkillDef.rechargeStock = 1;
            teleportSkillDef.requiredStock = 1;
            teleportSkillDef.resetCooldownTimerOnUse = true;
            teleportSkillDef.skillDescriptionToken = "RISKOFBULLETSTORM_SKILL_TELEPORT_DESCRIPTION";
            teleportSkillDef.skillName = "RiskOfBulletstormTeleport";
            teleportSkillDef.skillNameToken = "RISKOFBULLETSTORM_SKILL_TELEPORT_NAME";
            (teleportSkillDef as ScriptableObject).name = "RiskOfBulletstormTeleport";
            teleportSkillDef.stockToConsume = 1;

            ContentAddition.AddSkillDef(teleportSkillDef);
        }

        public override void CreateConfig(ConfigFile config)
        {
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (characterBody.skillLocator)
            {
                characterBody.ReplaceSkillIfItemPresent(characterBody.skillLocator.utility, ItemDef.itemIndex, teleportSkillDef);
            }
        }
    }

    //https://thunderstore.io/package/bongopd/ArtificerRangeTeleport/
    public class Teleport : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Teleport.baseDuration = PrepWall.baseDuration;
            Teleport.teleportSoundString = PrepWall.prepWallSoundString;
            Teleport.maxDistance = PrepWall.maxDistance;
            Teleport.maxSlopeAngle = PrepWall.maxSlopeAngle;
            Teleport.goodCrosshairPrefab = PrepWall.goodCrosshairPrefab;
            Teleport.badCrosshairPrefab = PrepWall.badCrosshairPrefab;
            Teleport.fireSoundString = PrepWall.fireSoundString;
            Teleport.muzzleflashEffect = PrepWall.muzzleflashEffect;
            Teleport.projectilePrefab = PrepWall.projectilePrefab;
            Teleport.damageCoefficient = PrepWall.damageCoefficient;
            Teleport.blinkPrefab = BlinkState.blinkPrefab;
            characterBody.duration = Teleport.baseDuration / characterBody.attackSpeedStat;
            base.characterBody.SetAimTimer(characterBody.duration + 2f);
            characterBody.cachedCrosshairPrefab = base.characterBody.defaultCrosshairPrefab;
            base.PlayAnimation("Gesture, Additive", "PrepWall", "PrepWall.playbackRate", characterBody.duration);
            Util.PlaySound(Teleport.teleportSoundString, base.gameObject);
            characterBody.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
            characterBody.areaIndicatorInstance.transform.localScale = new Vector3(1f, 3f, 1f);
            characterBody.UpdateAreaIndicator();
        }

        private void UpdateAreaIndicator()
        {
            characterBody.goodPlacement = false;
            characterBody.areaIndicatorInstance.SetActive(true);
            bool areaIndicatorExists = characterBody.areaIndicatorInstance;
            if (areaIndicatorExists)
            {
                float num = Teleport.maxDistance;
                Ray aimRay = base.GetAimRay();
                bool flag2 = Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(aimRay, base.gameObject, out float num2), out characterBody.raycastHit, num + num2, LayerIndex.world.mask);
                if (flag2)
                {
                    characterBody.areaIndicatorInstance.transform.position = characterBody.raycastHit.point;
                    characterBody.areaIndicatorInstance.transform.up = characterBody.raycastHit.normal;
                    characterBody.areaIndicatorInstance.transform.forward = -aimRay.direction;
                    characterBody.goodPlacement = (Vector3.Angle(Vector3.up, characterBody.raycastHit.normal) < Teleport.maxSlopeAngle);
                }
                if (areaIndicatorExists != characterBody.goodPlacement || characterBody.crosshairOverrideRequest == null)
                {
                    CrosshairUtils.OverrideRequest overrideRequest = characterBody.crosshairOverrideRequest;
                    if (overrideRequest != null)
                    {
                        overrideRequest.Dispose();
                    }
                    characterBody.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, characterBody.goodPlacement ? Teleport.goodCrosshairPrefab : Teleport.badCrosshairPrefab, CrosshairUtils.OverridePriority.Skill);
                }
            }
            characterBody.areaIndicatorInstance.SetActive(characterBody.goodPlacement);
        }

        public override void Update()
        {
            base.Update();
            characterBody.UpdateAreaIndicator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            characterBody.stopwatch += Time.fixedDeltaTime;
            bool flag = characterBody.stopwatch >= characterBody.duration && !base.inputBank.skill3.down && base.isAuthority;
            if (flag)
            {
                characterBody.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            bool flag = !characterBody.outer.destroying;
            if (flag)
            {
                bool flag2 = characterBody.goodPlacement;
                if (flag2)
                {
                    base.PlayAnimation("Gesture, Additive", "FireWall");
                    Util.PlaySound(Teleport.fireSoundString, base.gameObject);
                    bool flag3 = characterBody.areaIndicatorInstance && base.isAuthority;
                    if (flag3)
                    {
                        EffectManager.SimpleMuzzleFlash(Teleport.muzzleflashEffect, base.gameObject, "MuzzleLeft", true);
                        EffectManager.SimpleMuzzleFlash(Teleport.muzzleflashEffect, base.gameObject, "MuzzleRight", true);
                        Vector3 forward = characterBody.areaIndicatorInstance.transform.forward;
                        forward.y = 0f;
                        forward.Normalize();
                        Vector3 vector = Vector3.Cross(Vector3.up, forward);
                        bool flag4 = Util.CheckRoll(characterBody.critStat, base.characterBody.master);
                        characterBody.modelTransform = base.GetModelTransform();
                        bool flag5 = characterBody.modelTransform;
                        if (flag5)
                        {
                            characterBody.characterModel = characterBody.modelTransform.GetComponent<CharacterModel>();
                            characterBody.hurtboxGroup = characterBody.modelTransform.GetComponent<HurtBoxGroup>();
                        }
                        bool flag6 = characterBody.characterModel;
                        if (flag6)
                        {
                            characterBody.characterModel.invisibilityCount++;
                        }
                        bool flag7 = characterBody.hurtboxGroup;
                        if (flag7)
                        {
                            HurtBoxGroup hurtBoxGroup = characterBody.hurtboxGroup;
                            int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                            hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                        }
                        characterBody.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                        bool flag8 = base.characterMotor;
                        if (flag8)
                        {
                            base.characterMotor.velocity = Vector3.zero;
                            base.characterMotor.Motor.SetFieldValue("_internalTransientPosition", characterBody.areaIndicatorInstance.transform.position + characterBody.raycastHit.normal);
                        }
                        bool flag9 = !characterBody.outer.destroying;
                        if (flag9)
                        {
                            Util.PlaySound(characterBody.endSoundString, base.gameObject);
                            characterBody.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                            characterBody.modelTransform = base.GetModelTransform();
                            bool flag10 = characterBody.modelTransform;
                            if (flag10)
                            {
                                TemporaryOverlay temporaryOverlay = characterBody.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                                temporaryOverlay.duration = 0.6f;
                                temporaryOverlay.animateShaderAlpha = true;
                                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                                temporaryOverlay.destroyComponentOnEnd = true;
                                temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashBright");
                                temporaryOverlay.AddToCharacerModel(characterBody.modelTransform.GetComponent<CharacterModel>());
                                TemporaryOverlay temporaryOverlay2 = characterBody.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                                temporaryOverlay2.duration = 0.7f;
                                temporaryOverlay2.animateShaderAlpha = true;
                                temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                                temporaryOverlay2.destroyComponentOnEnd = true;
                                temporaryOverlay2.originalMaterial = Resources.Load<Material>("Materials/matHuntressFlashExpanded");
                                temporaryOverlay2.AddToCharacerModel(characterBody.modelTransform.GetComponent<CharacterModel>());
                            }
                        }
                        bool flag11 = characterBody.characterModel;
                        if (flag11)
                        {
                            characterBody.characterModel.invisibilityCount--;
                        }
                        bool flag12 = characterBody.hurtboxGroup;
                        if (flag12)
                        {
                            HurtBoxGroup hurtBoxGroup2 = characterBody.hurtboxGroup;
                            int hurtBoxesDeactivatorCounter2 = hurtBoxGroup2.hurtBoxesDeactivatorCounter - 1;
                            hurtBoxGroup2.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter2;
                        }
                    }
                }
                else
                {
                    base.skillLocator.utility.AddOneStock();
                    base.PlayCrossfade("Gesture, Additive", "BufferEmpty", 0.2f);
                }
            }
            EntityState.Destroy(characterBody.areaIndicatorInstance.gameObject);
            CrosshairUtils.OverrideRequest overrideRequest = characterBody.crosshairOverrideRequest;
            if (overrideRequest != null)
            {
                overrideRequest.Dispose();
            }
            base.OnExit();
        }

        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(characterBody.blinkVector);
            effectData.origin = origin;
            EffectManager.SpawnEffect(Teleport.blinkPrefab, effectData, false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

        public RaycastHit raycastHit;

        public static float baseDuration;

        public static GameObject areaIndicatorPrefab;

        public static GameObject projectilePrefab;

        public static float damageCoefficient;

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
    }
}