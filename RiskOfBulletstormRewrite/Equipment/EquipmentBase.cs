using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rewired;

namespace RiskOfBulletstormRewrite.Equipment
{
    public abstract class EquipmentBase<T> : EquipmentBase where T : EquipmentBase<T>
    {
        public static T Instance { get; private set; }

        public EquipmentBase()
        {
            if (Instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting EquipmentBoilerplate/Equipment was instantiated twice");
            Instance = this as T;
        }
    }

    public abstract class EquipmentBase
    {
        public const string CooldownDescription = "What is the cooldown in seconds of this equipment?";
        public const string CooldownName = "Cooldown";

        /// <summary>
        /// Name of the Equipment, for the Config and Internals
        /// </summary>
        public abstract string EquipmentName { get; }

        /// <summary>
        /// Primary Token for language.
        /// <para>Ex: "GAWK" => used in RBS_GAWK_NAME, RBS_GAWK_DESC, ETC</para>
        /// </summary>
        public abstract string EquipmentLangTokenName { get; }

        /// <summary>
        /// Optional parameters for the Equipment Pickup Token
        /// </summary>
        public virtual string[] EquipmentPickupDescParams { get; }

        /// <summary>
        /// Optional parameters for the Equipment Description Token
        /// </summary>
        public virtual string[] EquipmentFullDescriptionParams { get; }

        public abstract GameObject EquipmentModel { get; }
        public abstract Sprite EquipmentIcon { get; }

        public virtual bool AppearsInSinglePlayer { get; } = true;

        public virtual bool AppearsInMultiPlayer { get; } = true;

        public virtual bool CanDrop { get; } = true;

        public virtual float Cooldown { get; } = 60f;

        public virtual bool EnigmaCompatible { get; } = true;

        public virtual bool IsBoss { get; } = false;

        public virtual bool IsLunar { get; } = false;

        public virtual bool CanBeDroppedByPlayer { get; } = true;

        /// <summary>
        /// Can be randomly triggered by things such as Bottled Chaos
        /// </summary>
        public virtual bool CanBeRandomlyTriggered { get; } = true;

        public virtual Equipment.EquipmentBase DependentEquipment { get; } = null;

        /// <summary>
        /// The internal name of its parent equipment, so that when its Parent is disabled, so too will it as a child.
        /// <para>Ex: AppleConsumed has its ParentEquipmentName as "Apple". AppleConsumed loses its ability to be disabled and requires
        /// Apple to be disabled.</para>
        /// </summary>
        public virtual string ParentEquipmentName { get; } = null;

        public EquipmentDef EquipmentDef;
        public GameObject ItemBodyModelPrefab;

        public string ConfigCategory
        {
            get
            {
                return "Equipment: " + EquipmentName;
            }
        }

        public virtual string EquipmentPickupToken
        {
            get
            {
                return "RISKOFBULLETSTORM_EQUIPMENT_" + EquipmentLangTokenName + "_PICKUP";
            }
        }

        public virtual string EquipmentDescriptionToken
        {
            get
            {
                return "RISKOFBULLETSTORM_EQUIPMENT_" + EquipmentLangTokenName + "_DESCRIPTION";
            }
        }

        public string GetChance(ConfigEntry<float> configEntry)
        {
            return (configEntry.Value * 100).ToString();
        }

        public abstract ItemDisplayRuleDict CreateItemDisplayRules();

        /// <summary>
        /// This method structures your code execution of this class. An example implementation inside of it would be:
        /// <para>CreateConfig(config);</para>
        /// <para>CreateLang();</para>
        /// <para>CreateEquipment();</para>
        /// <para>Hooks();</para>
        /// <para>This ensures that these execute in this order, one after another, and is useful for having things available to be used in later methods.</para>
        /// <para>P.S. CreateItemDisplayRules(); does not have to be called in this, as it already gets called in CreateEquipment();</para>
        /// </summary>
        /// <param name="config">The config file that will be passed into this from the main class.</param>
        public abstract void Init(ConfigFile config);

        protected virtual void CreateConfig(ConfigFile config)
        { }

        public virtual UnlockableDef UnlockableDef { get; set; } = null;

        public virtual UnlockableDef CreateUnlockableDef()
        {
            UnlockableDef unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            unlockableDef.achievementIcon = LoadSprite();
            unlockableDef.cachedName = $"RiskOfBulletstorm.Equipment.{EquipmentLangTokenName}";
            return unlockableDef;
        }

        /// <summary>
        /// Take care to call base.CreateLang()!
        /// </summary>
        protected virtual void CreateLang()
        {
            Main._logger.LogMessage($"{EquipmentName} CreateLang()");
            bool formatPickup = EquipmentPickupDescParams?.Length > 0;
            //Main._logger.LogMessage("pickupCheck");
            bool formatDescription = EquipmentFullDescriptionParams?.Length > 0; //https://stackoverflow.com/a/41596301
            //Main._logger.LogMessage("descCheck");
            if (formatDescription && formatPickup)
            {
                //Main._logger.LogMessage("Nothing to format.");
                return;
            }

            if (formatPickup)
            {
                LanguageOverrides.DeferToken(EquipmentPickupToken, EquipmentPickupDescParams);
            }

            if (formatDescription)
            {
                LanguageOverrides.DeferToken(EquipmentDescriptionToken, EquipmentFullDescriptionParams);
            }
        }

        protected void CreateEquipment()
        {
            var prefix = "RISKOFBULLETSTORM_EQUIPMENT_";
            EquipmentDef = ScriptableObject.CreateInstance<EquipmentDef>();
            EquipmentDef.name = prefix + EquipmentLangTokenName;
            EquipmentDef.nameToken = prefix + EquipmentLangTokenName + "_NAME";
            EquipmentDef.pickupToken = prefix + EquipmentLangTokenName + "_PICKUP";
            EquipmentDef.descriptionToken = prefix + EquipmentLangTokenName + "_DESCRIPTION";
            EquipmentDef.loreToken = prefix + EquipmentLangTokenName + "_LORE";
            EquipmentDef.pickupModelPrefab = EquipmentModel;
            EquipmentDef.pickupIconSprite = EquipmentIcon;
            EquipmentDef.appearsInSinglePlayer = AppearsInSinglePlayer;
            EquipmentDef.appearsInMultiPlayer = AppearsInMultiPlayer;
            EquipmentDef.canDrop = CanDrop;
            EquipmentDef.cooldown = Cooldown;
            EquipmentDef.enigmaCompatible = EnigmaCompatible;
            EquipmentDef.isBoss = IsBoss;
            EquipmentDef.isLunar = IsLunar;
            EquipmentDef.canBeRandomlyTriggered = CanBeRandomlyTriggered;

            UnlockableDef = CreateUnlockableDef();
            if (UnlockableDef != null)
            {
                ContentAddition.AddUnlockableDef(UnlockableDef);
                EquipmentDef.unlockableDef = UnlockableDef;
            }
            if (!CanBeDroppedByPlayer)
            {

            }

            //EquipmentDef.colorIndex

            ItemAPI.Add(new CustomEquipment(EquipmentDef, CreateItemDisplayRules()));
            On.RoR2.EquipmentSlot.PerformEquipmentAction += PerformEquipmentAction;
        }

        //runs on server
        private bool PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, RoR2.EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (Tweaks.EquipmentCanBeDropped(equipmentDef)
                )
            /*    && Tweaks.PlayerCharacterMasterCanSendBodyInput(self, out LocalUser localUser, out Player player, out CameraRigController cameraRigController))
            {
                if (player.GetButtonDown(5)) //interact
                {
                    Tweaks.DropEquipment(self, equipmentDef);
                    return true;
                }
            }*/
            {
                bool inputCheck = self.inputBank && self.inputBank.interact.down;
                if (inputCheck)
                {
                    Tweaks.DropEquipment(self, equipmentDef);
                    return true;
                }
            }
            if (equipmentDef == EquipmentDef)
            {
                return ActivateEquipment(self);
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }

        //runs on server
        protected abstract bool ActivateEquipment(EquipmentSlot slot);

        public virtual void Hooks()
        { }

        public Sprite LoadSprite(string equipmentNameToken = "")
        {
            var token = equipmentNameToken == "" ? EquipmentLangTokenName : equipmentNameToken;
            return Assets.LoadSprite($"EQUIPMENT_{token}");
        }

        public GameObject LoadModel(string equipmentNameToken = "")
        {
            var token = equipmentNameToken == "" ? EquipmentLangTokenName : equipmentNameToken;
            return Assets.LoadObject($"{token}.prefab");
        }

        public static implicit operator EquipmentBase(Type v)
        {
            return (EquipmentBase)System.Activator.CreateInstance(v);
        }

        #region Targeting Setup

        //Targeting Support
        public virtual bool UseTargeting { get; } = false;

        public GameObject TargetingIndicatorPrefabBase = null;

        public enum TargetingType
        {
            Enemies,
            Friendlies,
        }

        public virtual TargetingType TargetingTypeEnum { get; } = TargetingType.Enemies;

        //Based on MysticItem's targeting code.
        protected void UpdateTargeting(On.RoR2.EquipmentSlot.orig_Update orig, EquipmentSlot self)
        {
            orig(self);

            if (self.equipmentIndex == EquipmentDef.equipmentIndex)
            {
                var targetingComponent = self.GetComponent<TargetingControllerComponent>();
                if (!targetingComponent)
                {
                    targetingComponent = self.gameObject.AddComponent<TargetingControllerComponent>();
                    targetingComponent.VisualizerPrefab = TargetingIndicatorPrefabBase;
                }

                if (self.stock > 0)
                {
                    switch (TargetingTypeEnum)
                    {
                        case (TargetingType.Enemies):
                            targetingComponent.ConfigureTargetFinderForEnemies(self);
                            break;

                        case (TargetingType.Friendlies):
                            targetingComponent.ConfigureTargetFinderForFriendlies(self);
                            break;
                    }
                }
                else
                {
                    targetingComponent.Invalidate();
                    targetingComponent.Indicator.active = false;
                }
            }
        }

        public class TargetingControllerComponent : MonoBehaviour
        {
            public GameObject TargetObject;
            public GameObject VisualizerPrefab;
            public Indicator Indicator;
            public BullseyeSearch TargetFinder;
            public Action<BullseyeSearch> AdditionalBullseyeFunctionality = (search) => { };

            public void Awake()
            {
                Indicator = new Indicator(gameObject, null);
            }

            public void OnDestroy()
            {
                Invalidate();
            }

            public void Invalidate()
            {
                TargetObject = null;
                Indicator.targetTransform = null;
            }

            public void ConfigureTargetFinderBase(EquipmentSlot self)
            {
                if (TargetFinder == null) TargetFinder = new BullseyeSearch();
                TargetFinder.teamMaskFilter = TeamMask.allButNeutral;
                TargetFinder.teamMaskFilter.RemoveTeam(self.characterBody.teamComponent.teamIndex);
                TargetFinder.sortMode = BullseyeSearch.SortMode.Angle;
                TargetFinder.filterByLoS = true;
                float num;
                Ray ray = CameraRigController.ModifyAimRayIfApplicable(self.GetAimRay(), self.gameObject, out num);
                TargetFinder.searchOrigin = ray.origin;
                TargetFinder.searchDirection = ray.direction;
                TargetFinder.maxAngleFilter = 10f;
                TargetFinder.viewer = self.characterBody;
            }

            public void ConfigureTargetFinderForEnemies(EquipmentSlot self)
            {
                ConfigureTargetFinderBase(self);
                TargetFinder.teamMaskFilter = TeamMask.GetUnprotectedTeams(self.characterBody.teamComponent.teamIndex);
                TargetFinder.RefreshCandidates();
                TargetFinder.FilterOutGameObject(self.gameObject);
                AdditionalBullseyeFunctionality(TargetFinder);
                PlaceTargetingIndicator(TargetFinder.GetResults());
            }

            public void ConfigureTargetFinderForFriendlies(EquipmentSlot self)
            {
                ConfigureTargetFinderBase(self);
                TargetFinder.teamMaskFilter = TeamMask.none;
                TargetFinder.teamMaskFilter.AddTeam(self.characterBody.teamComponent.teamIndex);
                TargetFinder.RefreshCandidates();
                TargetFinder.FilterOutGameObject(self.gameObject);
                AdditionalBullseyeFunctionality(TargetFinder);
                PlaceTargetingIndicator(TargetFinder.GetResults());
            }

            public void PlaceTargetingIndicator(IEnumerable<HurtBox> TargetFinderResults)
            {
                HurtBox hurtbox = TargetFinderResults.Any() ? TargetFinderResults.First() : null;

                if (hurtbox)
                {
                    TargetObject = hurtbox.healthComponent.gameObject;
                    Indicator.visualizerPrefab = VisualizerPrefab;
                    Indicator.targetTransform = hurtbox.transform;
                }
                else
                {
                    Invalidate();
                }
                Indicator.active = hurtbox;
            }
        }

        #endregion Targeting Setup
    }
}