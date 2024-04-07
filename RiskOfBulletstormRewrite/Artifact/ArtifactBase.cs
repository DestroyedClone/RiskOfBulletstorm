using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using RoR2.ExpansionManagement;
using System;
using UnityEngine;
using RiskOfBulletstormRewrite.Modules;

namespace RiskOfBulletstormRewrite.Artifact
{
    public abstract class ArtifactBase<T> : ArtifactBase where T : ArtifactBase<T>
    {
        public static T instance { get; private set; }

        public ArtifactBase()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ArtifactBase was instantiated twice");
            instance = this as T;
        }
    }

    public abstract class ArtifactBase
    {
        ///<summary>
        ///English name of the artifact.
        ///</summary>
        public abstract string ArtifactName { get; }

        ///<summary>
        ///Language Token responsible for the localization.
        ///</summary>
        public abstract string ArtifactLangTokenName { get; }

        ///<summary>
        ///The auto-generated token for the description.
        ///</summary>
        public string ArtifactDescriptionToken
        {
            get
            {
                return LanguageOverrides.LanguageTokenPrefixArtifact + ArtifactLangTokenName + "_DESCRIPTION";
            }
        }

        ///<summary>
        ///Parameters for formatting the description language token.
        ///</summary>
        public virtual object[] ArtifactFullDescriptionParams { get; }

        ///<summary>
        ///The required ExpansionDef for this artifact.
        ///</summary>
        public virtual ExpansionDef ArtifactExpansionDef { get; }

        ///<summary>
        ///The icon to use for the artifact when it's enabled.
        ///</summary>
        public abstract Sprite ArtifactEnabledIcon { get; }

        ///<summary>
        ///The icon to use for the artifact when it's disabled.
        ///</summary>
        public abstract Sprite ArtifactDisabledIcon { get; }

        ///<summary>
        ///The ArtifactDef for the Artifact.
        ///</summary>
        public ArtifactDef ArtifactDef;

        ///<summary>
        ///The auto-generated category for the config.
        ///</summary>
        public string ConfigCategory
        {
            get
            {
                return "Artifact: " + ArtifactName;
            }
        }

        public virtual string WikiLink { get; }

        //For use only after the run has started.
        public bool ArtifactEnabled => RunArtifactManager.instance.IsArtifactEnabled(ArtifactDef);

        public virtual void Init(ConfigFile config)
        {
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        ///<summary>
        ///Method responsible for creating and deferring the language tokens.
        ///</summary>
        protected void CreateLang()
        {
            //Main._logger.LogMessage($"{ArtifactName} CreateLang()");
            bool formatDescription = ArtifactFullDescriptionParams?.Length > 0; //https://stackoverflow.com/a/41596301
            //Main._logger.LogMessage("descCheck");
            if (formatDescription)
            {
                LanguageOverrides.DeferToken(ArtifactDescriptionToken, ArtifactFullDescriptionParams);
                return;
            }
        }

        ///<summary>
        ///Method to create the artifact.
        ///</summary>
        protected void CreateArtifact()
        {
            var prefix = LanguageOverrides.LanguageTokenPrefixArtifact;
            ArtifactDef = ScriptableObject.CreateInstance<ArtifactDef>();
            ArtifactDef.cachedName = prefix + ArtifactLangTokenName;
            ArtifactDef.nameToken = prefix + ArtifactLangTokenName + "_NAME";
            ArtifactDef.descriptionToken = prefix + ArtifactLangTokenName + "_DESCRIPTION";
            ArtifactDef.smallIconSelectedSprite = ArtifactEnabledIcon;
            ArtifactDef.smallIconDeselectedSprite = ArtifactDisabledIcon;
            if (ArtifactExpansionDef)
            {
                ArtifactDef.requiredExpansion = ArtifactExpansionDef;
            }

            ContentAddition.AddArtifactDef(ArtifactDef);
            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;
        }

        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            OnArtifactEnabled();
        }

        private void RunArtifactManager_onArtifactDisabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            OnArtifactDisabled();
        }

        public abstract void OnArtifactEnabled();

        public abstract void OnArtifactDisabled();

        public virtual void Hooks()
        { }

        public Sprite LoadSprite(bool enabled)
        {
            return Assets.LoadSprite(
            "Assets/Icons/ARTIFACT_" + ArtifactLangTokenName
            + (enabled ? "_ENABLED" : "_DISABLED") + ".png"
            );
        }
    }
}