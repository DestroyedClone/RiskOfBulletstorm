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

    public abstract class ArtifactBase
    {
        public abstract string ArtifactName { get; }

        public abstract string ArtifactLangTokenName { get; }
        public virtual string[] ArtifactFullDescriptionParams { get; }

        public abstract Sprite ArtifactEnabledIcon { get; }

        public abstract Sprite ArtifactDisabledIcon { get; }

        public ArtifactDef ArtifactDef;
        public string ConfigCategory
        {
            get
            {
                return "Artifact: " + ArtifactName;
            }
        }
        public string ArtifactDescriptionToken
        {
            get
            {
                return "RISKOFBULLETSTORM_ARTIFACT_" + ArtifactLangTokenName + "_DESCRIPTION";
            }
        }

        //For use only after the run has started.
        public bool ArtifactEnabled => RunArtifactManager.instance.IsArtifactEnabled(ArtifactDef);

        public abstract void Init(ConfigFile config);

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

        private void DeferToken(string token, string lang, params string[] args)
        {
            //Main._logger.LogMessage($"Deferring {token} w/ lang {lang}");
            RiskOfBulletstormRewrite.Language.langTokenValues.Add(new Language.LangTokenValue() { token = token, lang = lang, strings = args });
        }

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

        public abstract void Hooks();
    }
}
