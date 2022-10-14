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
        public static ConfigEntry<float> cfgTeleportRange;
        public static ConfigEntry<float> cfgTeleportRangePerStack;
        public static ConfigEntry<float> cfgDamageVulnerabilityMultiplier;
        public static ConfigEntry<float> cfgDamageVulnerabilityMultiplierPerStack;
        public static ConfigEntry<float> cfgDamageVulnerabilityDuration;

        public override string ItemName => "Bloodied Scarf";

        public override string ItemLangTokenName => "BLOODIEDSCARF";

        public override bool ItemDescriptionLogbookOverride => true;
        public override string[] ItemLogbookDescriptionParams => teleportSkillDefParams;

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.CannotSteal,
            ItemTag.Cleansable,
            ItemTag.AIBlacklist,
            ItemTag.CannotCopy
        };

        public static SkillDef teleportSkillDef;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSkillDef();
            CreateLang();
            CreateItem();
            Hooks();
        }

        public string[] teleportSkillDefParams => new string[]
        {
            GetChance(cfgTeleportRange),
            GetChance(cfgTeleportRangePerStack),
            GetChance(cfgDamageVulnerabilityMultiplier),
            GetChance(cfgDamageVulnerabilityMultiplierPerStack),
            cfgDamageVulnerabilityDuration.Value.ToString()
        };

        public void CreateSkillDef()
        {
            teleportSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            teleportSkillDef.activationState = new SerializableEntityStateType(typeof(TeleportUtilitySkillState));
            teleportSkillDef.activationStateMachineName = "Body";
            teleportSkillDef.baseMaxStock = 1;
            teleportSkillDef.baseRechargeInterval = 8;
            teleportSkillDef.beginSkillCooldownOnSkillEnd = true;
            teleportSkillDef.canceledFromSprinting = false;
            teleportSkillDef.cancelSprintingOnActivation = false;
            teleportSkillDef.dontAllowPastMaxStocks = false;
            teleportSkillDef.forceSprintDuringState = true;
            teleportSkillDef.fullRestockOnAssign = true;
            teleportSkillDef.icon = Assets.LoadSprite("SKILL_BLOODIEDSCARF");
            teleportSkillDef.interruptPriority = InterruptPriority.Vehicle;
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
            ContentAddition.AddEntityState<TeleportUtilitySkillState>(out bool wasAdded);
        }

        protected override void CreateLang()
        {
            base.CreateLang();
            
            foreach (var lang in RoR2.Language.steamLanguageTable)
            {
                var langName = lang.Value.webApiName;
                DeferToken(teleportSkillDef.skillDescriptionToken, langName, teleportSkillDefParams);
            }
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgTeleportRange = config.Bind(ConfigCategory, "Teleport Range", 5f, "Distance in meters");
            cfgTeleportRangePerStack = config.Bind(ConfigCategory, "Teleport Range Per Stack", 2.5f, "Distance in meters");
            cfgDamageVulnerabilityMultiplier = config.Bind(ConfigCategory, "Damage Vulnerability Multiplier", .2f, "");
            cfgDamageVulnerabilityMultiplierPerStack = config.Bind(ConfigCategory, "Damage Vulnerability Multiplier Per Stack", .1f, "");
            cfgDamageVulnerabilityDuration = config.Bind(ConfigCategory, "Damage Vulnerability Duration", 1f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += onBodyInventoryChangedGlobal;
        }

        private void onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (characterBody.skillLocator)
            {
                characterBody.ReplaceSkillIfItemPresent(characterBody.skillLocator.utility, ItemDef.itemIndex, teleportSkillDef);
            }
        }
    }

    #region oldTeleportDef
    /*
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
            duration = Teleport.baseDuration / attackSpeedStat;
            base.characterBody.SetAimTimer(duration + 2f);
            cachedCrosshairPrefab = base.characterBody.defaultCrosshairPrefab;
            base.PlayAnimation("Gesture, Additive", "PrepWall", "PrepWall.playbackRate", duration);
            Util.PlaySound(Teleport.teleportSoundString, base.gameObject);
            areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(ArrowRain.areaIndicatorPrefab);
            areaIndicatorInstance.transform.localScale = new Vector3(1f, 3f, 1f);
            UpdateAreaIndicator();
        }

        private void UpdateAreaIndicator()
        {
            goodPlacement = false;
            areaIndicatorInstance.SetActive(true);
            bool areaIndicatorExists = areaIndicatorInstance;
            if (areaIndicatorExists)
            {
                float num = Teleport.maxDistance;
                Ray aimRay = base.GetAimRay();
                bool flag2 = Physics.Raycast(CameraRigController.ModifyAimRayIfApplicable(aimRay, base.gameObject, out float num2), out raycastHit, num + num2, LayerIndex.world.mask);
                if (flag2)
                {
                    areaIndicatorInstance.transform.position = raycastHit.point;
                    areaIndicatorInstance.transform.up = raycastHit.normal;
                    areaIndicatorInstance.transform.forward = -aimRay.direction;
                    goodPlacement = (Vector3.Angle(Vector3.up, raycastHit.normal) < Teleport.maxSlopeAngle);
                }
                if (areaIndicatorExists != goodPlacement || crosshairOverrideRequest == null)
                {
                    CrosshairUtils.OverrideRequest overrideRequest = crosshairOverrideRequest;
                    if (overrideRequest != null)
                    {
                        overrideRequest.Dispose();
                    }
                    crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, goodPlacement ? Teleport.goodCrosshairPrefab : Teleport.badCrosshairPrefab, CrosshairUtils.OverridePriority.Skill);
                }
            }
            areaIndicatorInstance.SetActive(goodPlacement);
        }

        public override void Update()
        {
            base.Update();
            UpdateAreaIndicator();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            bool flag = stopwatch >= duration && !base.inputBank.skill3.down && base.isAuthority;
            if (flag)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            bool flag = !outer.destroying;
            if (flag)
            {
                bool flag2 = goodPlacement;
                if (flag2)
                {
                    base.PlayAnimation("Gesture, Additive", "FireWall");
                    Util.PlaySound(Teleport.fireSoundString, base.gameObject);
                    bool flag3 = areaIndicatorInstance && base.isAuthority;
                    if (flag3)
                    {
                        EffectManager.SimpleMuzzleFlash(Teleport.muzzleflashEffect, base.gameObject, "MuzzleLeft", true);
                        EffectManager.SimpleMuzzleFlash(Teleport.muzzleflashEffect, base.gameObject, "MuzzleRight", true);
                        Vector3 forward = areaIndicatorInstance.transform.forward;
                        forward.y = 0f;
                        forward.Normalize();
                        Vector3 vector = Vector3.Cross(Vector3.up, forward);
                        bool flag4 = Util.CheckRoll(critStat, base.characterBody.master);
                        modelTransform = base.GetModelTransform();
                        bool flag5 = modelTransform;
                        if (flag5)
                        {
                            characterModel = modelTransform.GetComponent<CharacterModel>();
                            hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
                        }
                        bool flag6 = characterModel;
                        if (flag6)
                        {
                            characterModel.invisibilityCount++;
                        }
                        bool flag7 = hurtboxGroup;
                        if (flag7)
                        {
                            HurtBoxGroup hurtBoxGroup = hurtboxGroup;
                            int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                            hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
                        }
                        CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                        bool flag8 = base.characterMotor;
                        if (flag8)
                        {
                            base.characterMotor.velocity = Vector3.zero;
                            base.characterMotor.Motor.SetFieldValue("_internalTransientPosition", areaIndicatorInstance.transform.position + raycastHit.normal);
                        }
                        bool flag9 = !outer.destroying;
                        if (flag9)
                        {
                            Util.PlaySound(endSoundString, base.gameObject);
                            CreateBlinkEffect(Util.GetCorePosition(base.gameObject));
                            modelTransform = base.GetModelTransform();
                            bool flag10 = modelTransform;
                            if (flag10)
                            {
                                TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                                temporaryOverlay.duration = 0.6f;
                                temporaryOverlay.animateShaderAlpha = true;
                                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                                temporaryOverlay.destroyComponentOnEnd = true;
                                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                                temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
                                TemporaryOverlay temporaryOverlay2 = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                                temporaryOverlay2.duration = 0.7f;
                                temporaryOverlay2.animateShaderAlpha = true;
                                temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                                temporaryOverlay2.destroyComponentOnEnd = true;
                                temporaryOverlay2.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                                temporaryOverlay2.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
                            }
                        }
                        bool flag11 = characterModel;
                        if (flag11)
                        {
                            characterModel.invisibilityCount--;
                        }
                        bool flag12 = hurtboxGroup;
                        if (flag12)
                        {
                            HurtBoxGroup hurtBoxGroup2 = hurtboxGroup;
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
            EntityState.Destroy(areaIndicatorInstance.gameObject);
            CrosshairUtils.OverrideRequest overrideRequest = crosshairOverrideRequest;
            if (overrideRequest != null)
            {
                overrideRequest.Dispose();
            }
            base.OnExit();
        }

        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(blinkVector);
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
    }*/
    #endregion
}