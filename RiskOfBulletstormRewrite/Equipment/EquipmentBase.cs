using BepInEx;
using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RoR2.ExpansionManagement;

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

    public abstract partial class EquipmentBase
    {
        public const string CooldownDescription = "What is the cooldown in seconds of this equipment?";
        public const string CooldownName = "Cooldown";
        ///<summary>
        ///Name of the equipment.
        ///</summary>
        public abstract string EquipmentName { get; }

        ///<summary>
        ///Language Token Name responsible for the internals.
        /// <para>"ITEMNAME" => RBS_ITEMNAME_NAME</para>
        ///</summary>
        public abstract string EquipmentLangTokenName { get; }

        ///<summary>
        ///The auto-generated token for the pickup.
        ///</summary>
        public virtual string EquipmentPickupToken
        {
            get
            {
                return LanguageOverrides.LanguageTokenPrefixEquipment + EquipmentLangTokenName + "_PICKUP";
            }
        }

        /// <summary>
        /// Optional parameters for the Equipment Pickup Token
        /// </summary>
        public virtual object[] EquipmentPickupDescParams { get; }

        ///<summary>
        ///The auto-generated token for the description.
        ///</summary>
        public virtual string EquipmentDescriptionToken
        {
            get
            {
                return LanguageOverrides.LanguageTokenPrefixEquipment + EquipmentLangTokenName + "_DESCRIPTION";
            }
        }

        /// <summary>
        /// Optional parameters for the Equipment Description Token
        /// </summary>
        public virtual object[] EquipmentFullDescriptionParams { get; }

        /// <summary>
        /// Primary Token for language.
        /// <para>Ex: "GAWK" => used in RBS_GAWK_NAME, RBS_GAWK_DESC, ETC</para>
        /// </summary>
        public virtual string EquipmentUniquePickupToken { get; }

        /// <summary>
        /// Primary Token for language.
        /// <para>Ex: "GAWK" => used in RBS_GAWK_NAME, RBS_GAWK_DESC, ETC</para>
        /// </summary>
        public virtual string EquipmentUniqueDescriptionToken { get; }

        ///<summary>
        ///The equipment's pickup model.
        ///</summary>
        public abstract GameObject EquipmentModel { get; }

        ///<summary>
        ///The equipment's icon sprite.
        ///</summary>
        public abstract Sprite EquipmentIcon { get; }

        public virtual bool AppearsInSinglePlayer { get; } = true;

        public virtual bool AppearsInMultiPlayer { get; } = true;

        /// <summary>
        /// Whether or not this equipment is accessible in the droplist. Consider enabling for consumed items.
        /// </summary>
        public virtual bool CanDrop { get; } = true;

        public virtual float Cooldown { get; } = 60f;

        public virtual bool EnigmaCompatible { get; } = true;

        /// <summary>
        /// Enables its pickupdisplay to use boss particles.
        /// </summary>
        public virtual bool IsBoss { get; } = false;

        public virtual bool IsLunar { get; } = false;

        public virtual bool CanBeDroppedByPlayer { get; } = true;

        /// <summary>
        /// Can be randomly triggered by things such as Bottled Chaos
        /// </summary>
        public virtual bool CanBeRandomlyTriggered { get; } = true;

        public virtual Equipment.EquipmentBase DependentEquipment { get; } = null;
        public virtual string WikiLink { get; }

        /// <summary>
        /// The internal name of its parent equipment, so that when its Parent is disabled, so too will it as a child.
        /// <para>Ex: AppleConsumed has its ParentEquipmentName as "Apple". AppleConsumed loses its ability to be disabled and requires Apple to be disabled.</para>
        /// </summary>
        public virtual string ParentEquipmentName { get; } = null;

        public EquipmentDef EquipmentDef;
        public GameObject ItemBodyModelPrefab;

        public abstract ItemDisplayRuleDict CreateItemDisplayRules();

        public virtual UnlockableDef UnlockableDef { get; set; } = null;
        public virtual bool UnlockableDefAutoSetup { get; } = false;

        ///<summary>
        ///The required ExpansionDef for this artifact.
        ///</summary>
        public virtual ExpansionDef ExpansionDef { get; }
        public string GetChance(ConfigEntry<float> configEntry)
        {
            return ToPct(configEntry.Value);
        }

        public string ToPct(float value)
        {
            return (value * 100).ToString();
        }

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
        public virtual void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        public string ConfigCategory
        {
            get
            {
                return "Equipment: " + EquipmentName;
            }
        }

        protected virtual void CreateConfig(ConfigFile config)
        { }

        /// <summary>
        /// Take care to call base.CreateLang()!
        /// </summary>
        protected virtual void CreateLang()
        {
            //Main._logger.LogMessage($"{EquipmentName} CreateLang()");
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
                if (EquipmentUniquePickupToken.IsNullOrWhiteSpace())
                {
                    LanguageOverrides.DeferToken(EquipmentPickupToken, EquipmentPickupDescParams);
                }
                else
                {
                    LanguageOverrides.DeferUniqueToken(EquipmentUniquePickupToken, EquipmentPickupToken, EquipmentPickupDescParams);
                }
            }

            if (formatDescription)
            {
                if (EquipmentUniqueDescriptionToken.IsNullOrWhiteSpace())
                {
                    LanguageOverrides.DeferToken(EquipmentDescriptionToken, EquipmentFullDescriptionParams);
                }
                else
                {
                    LanguageOverrides.DeferUniqueToken(EquipmentUniqueDescriptionToken, EquipmentDescriptionToken, EquipmentFullDescriptionParams);
                }
            }
        }

        protected void CreateEquipment()
        {
            var prefix = LanguageOverrides.LanguageTokenPrefixEquipment;
            EquipmentDef = ScriptableObject.CreateInstance<EquipmentDef>();
            EquipmentDef.name = prefix + EquipmentLangTokenName;
            EquipmentDef.nameToken = prefix + EquipmentLangTokenName + "_NAME";
            EquipmentDef.pickupToken = EquipmentPickupToken;
            EquipmentDef.descriptionToken = EquipmentDescriptionToken;
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
            if (ExpansionDef)
            {
                EquipmentDef.requiredExpansion = ExpansionDef;
            }

            if (IsLunar)
            {
                EquipmentDef.colorIndex = ColorCatalog.ColorIndex.LunarItem;
            }

            /*if (UnlockableDefAutoSetup)
                //UnlockableDef = Assets.CreateUnlockableDef("Equipment." + EquipmentLangTokenName, EquipmentDef.pickupIconSprite);
            if (UnlockableDef != null)
            {
                ContentAddition.AddUnlockableDef(UnlockableDef);
                EquipmentDef.unlockableDef = UnlockableDef;
            }*/

            ItemAPI.Add(new CustomEquipment(EquipmentDef, CreateItemDisplayRules()));
            On.RoR2.EquipmentSlot.PerformEquipmentAction += PerformEquipmentAction;
            if (EquipmentTargetFinderType != TargetFinderType.None)// && TargetingIndicatorPrefabBase)
            {
                On.RoR2.EquipmentSlot.UpdateTargets += EquipmentSlot_UpdateTargets;
            }
        }

        private bool PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, RoR2.EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == EquipmentDef)
            {
                if (EquipmentTargetFinderType != TargetFinderType.None)
                {
                    self.UpdateTargets(equipmentDef.equipmentIndex, ShouldAnticipateTarget(self));
                }
                return ActivateEquipment(self);
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }

        protected abstract bool ActivateEquipment(EquipmentSlot slot);

        /*public Sprite LoadEquipmentIcon(string equipmentNameToken = "")
        {
            var token = equipmentNameToken == "" ? EquipmentLangTokenName : equipmentNameToken;
            return Assets.LoadSprite($"tex{token}Icon");
        }

        public GameObject LoadPickupModel(string equipmentNameToken = "")
        {
            var token = equipmentNameToken == "" ? EquipmentLangTokenName : equipmentNameToken;
            return Assets.LoadObject($"Pickup{token}.prefab");
        }*/

        /*public Sprite LoadSprite(string spriteName)
        {
            return Assets.LoadSprite(spriteName);
        }

        public GameObject LoadModel(string modelName)
        {
            return Assets.LoadObject(modelName);
        }*/
        public Sprite LoadSprite(string equipmentNameToken = "")
        {
            var token = equipmentNameToken == "" ? EquipmentLangTokenName : equipmentNameToken;
            return Modules.Assets.LoadSprite($"EQUIPMENT_{token}");
        }

        public GameObject LoadModel(string equipmentNameToken = "")
        {
            var token = equipmentNameToken == "" ? EquipmentLangTokenName : equipmentNameToken;
            return Modules.Assets.LoadObject($"{token}.prefab");
        }

        public virtual void Hooks()
        { }
    }
}