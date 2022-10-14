using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

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

    ///<summary> 
    ///Abstract class responsible for Artifacts.
    ///</summary>
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
        ///Parameters for formatting the description language token.
        ///</summary>
        public virtual string[] ArtifactFullDescriptionParams { get; }

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

        ///<summary>
        ///The auto-generated token for the description.
        ///</summary>
        public string ArtifactDescriptionToken
        {
            get
            {
                return "RISKOFBULLETSTORM_ARTIFACT_" + ArtifactLangTokenName + "_DESCRIPTION";
            }
        }

        //For use only after the run has started.
        
        ///<summary>
        ///Boolean for checking whether the artifact is enabled. Run only.
        ///</summary>
        public bool ArtifactEnabled => RunArtifactManager.instance.IsArtifactEnabled(ArtifactDef);

        ///<summary>
        ///The main method executed for setting up the class.
        ///</summary>
        public abstract void Init(ConfigFile config);
        public virtual void CreateConfig(ConfigFile config) { }

        ///<summary>
        ///Method responsible for creating and deferring the language tokens.
        ///</summary>
        protected void CreateLang()
        {
            Main._logger.LogMessage($"{ArtifactName} CreateLang()");
            bool formatDescription = ArtifactFullDescriptionParams?.Length > 0; //https://stackoverflow.com/a/41596301
            //Main._logger.LogMessage("descCheck");
            if (formatDescription)
            {
                //Main._logger.LogMessage("Nothing to format.");
                return;
            }

            foreach (var lang in RoR2.Language.steamLanguageTable)
            {
                var langName = lang.Value.webApiName;
                // Main._logger.LogMessage($"[{langName}]Modifying {ItemLangTokenName}");

                if (formatDescription)
                {
                    DeferToken(ArtifactDescriptionToken, langName, ArtifactFullDescriptionParams);
                }
            }
        }

        ///<summary>
        ///Helper method to defer language tokens.
        ///</summary>
        private void DeferToken(string token, string lang, params string[] args)
        {
            //Main._logger.LogMessage($"Deferring {token} w/ lang {lang}");
            RiskOfBulletstormRewrite.Language.langTokenValues.Add(new Language.LangTokenValue() { token = token, lang = lang, strings = args });
        }

        ///<summary>
        ///Method to create the artifact.
        ///</summary>
        protected void CreateArtifact()
        {
            var prefix = "RISKOFBULLETSTORM_ARTIFACT_";
            ArtifactDef = ScriptableObject.CreateInstance<ArtifactDef>();
            ArtifactDef.cachedName = prefix + ArtifactLangTokenName;
            ArtifactDef.nameToken = prefix + ArtifactLangTokenName + "_NAME";
            ArtifactDef.descriptionToken = prefix + ArtifactLangTokenName + "_DESCRIPTION";
            ArtifactDef.smallIconSelectedSprite = ArtifactEnabledIcon;
            ArtifactDef.smallIconDeselectedSprite = ArtifactDisabledIcon;

            ContentAddition.AddArtifactDef(ArtifactDef);
        }

        ///<summary>
        ///Method for calling hooks.
        ///</summary>
        public abstract void Hooks();

        public Sprite LoadSprite(bool enabled)
        {
            return Assets.LoadNewSprite(
            Assets.assemblyDir + "\\Assets\\ARTIFACT_"+ArtifactLangTokenName
            + (enabled ? "_ENABLED" : "_DISABLED") + ".png"
            );
        }
    }
}
