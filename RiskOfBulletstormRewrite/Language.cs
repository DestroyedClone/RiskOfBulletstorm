using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;

namespace RiskOfBulletstormRewrite
{
    public class Language
    {
        public static ConfigFile config;

        //public static event Action<ConfigFile> onConfigLoaded;

        public struct LangTokenValue
        {
            public string lang;
            public string token;
            public string[] strings;
        }

        public static List<LangTokenValue> langTokenValues = new List<LangTokenValue>();

        public static List<Type> configEntries = new List<Type>();

        public static void Initialize()
        {
            On.RoR2.UI.MainMenu.MainMenuController.Start += FinalizeLanguage;
        }

        private static void FinalizeLanguage(On.RoR2.UI.MainMenu.MainMenuController.orig_Start orig, RoR2.UI.MainMenu.MainMenuController self)
        {
            orig(self);
            SetupLanguage();
            //SetupConfigLanguage(config);
            On.RoR2.UI.MainMenu.MainMenuController.Start -= FinalizeLanguage;
        }

        private static string FormatToken(string token, string lang, params string[] objects)
        {
            var oldString = RoR2.Language.GetString(token, lang);
            var newString = string.Format(oldString, objects);
            return newString;
        }

        private static void SetupLanguage()
        {
            Main._logger.LogMessage($"Setting up language with {langTokenValues.Count} tokens.");
            foreach (var langTokenValue in langTokenValues)
            {
                var newString = FormatToken(langTokenValue.token, langTokenValue.lang, langTokenValue.strings);
                LanguageAPI.Add(langTokenValue.token, newString, langTokenValue.lang);
/*
                try
                {

                }catch
                {
                    Main._logger.LogError($"Failed setting up token \"{langTokenValue.token}\" for language {langTokenValue.lang}." +
                        $"\nFollowing parameters where given:");
                    foreach (var str in langTokenValue.strings)
                    {
                        Main._logger.LogMessage(str);
                    }
                }*/
                /*if (langTokenValue.lang == "en")
                {
                    Main._logger.LogMessage($"{langTokenValue.token} {newString}");
                }*/
            }
        }

        public static string GetToken(string token)
        {
            return RoR2.Language.GetString(token, RoR2.Language.currentLanguageName);
        }
        /*
        private static void SetupConfigLanguage(ConfigFile config)
        {
            Action<ConfigFile> action = Language.onConfigLoaded;
            if (action == null)
            {
                return;
            }
            action(config);
        }*/
    }
}