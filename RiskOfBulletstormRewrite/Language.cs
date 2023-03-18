using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfBulletstormRewrite
{
    public class LanguageOverrides
    {
        public static ConfigFile config;

        //public static event Action<ConfigFile> onConfigLoaded;

        public struct ReplacementToken
        {
            public string token;
            public string[] args;
        }

        public struct PostReplacementToken
        {
            public string baseToken;
            public string[] replacementTokens;
        }

        ///<summary>
        ///Helper method to defer language tokens.
        ///</summary>
        public static void DeferToken(string token, params string[] args)
        {
            //Main._logger.LogMessage($"Deferring {token} w/ lang {lang}");
            LanguageOverrides.replacementTokens.Add(new LanguageOverrides.ReplacementToken() { token = token, args = args });
        }

        public static void DeferLateTokens(string baseToken, params string[] args)
        {
            RiskOfBulletstormRewrite.LanguageOverrides.postReplacementTokens.Add(new PostReplacementToken() { baseToken = baseToken, replacementTokens = args });
        }

        public static List<ReplacementToken> replacementTokens = new List<ReplacementToken>();
        public static List<PostReplacementToken> postReplacementTokens = new List<PostReplacementToken>();
        public static List<Type> configEntries = new List<Type>();

        public static Dictionary<string, string> logbookTokenOverrideDict = new Dictionary<string, string>();

        public static void Initialize()
        {
            On.RoR2.UI.MainMenu.MainMenuController.Start += FinalizeLanguage;
            On.RoR2.UI.LogBook.LogBookController.Start += LogBookController_Start;
        }

        private static void LogBookController_Start(On.RoR2.UI.LogBook.LogBookController.orig_Start orig, RoR2.UI.LogBook.LogBookController self)
        {
            orig(self);
            self.gameObject.AddComponent<RBS_DestroyLogbookHookOnDestroy>();
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
            //todo: vs GetStringFormatted??
            var oldString = RoR2.Language.GetString(token, lang);
            var newString = string.Format(oldString, objects);
            return newString;
        }

        private static void SetupLanguage()
        {
            Main._logger.LogMessage($"Setting up language with {replacementTokens.Count} tokens.");
            DeferTokenFinalize();
            PostDeferTokenFinalize();
        }

        private static void DeferTokenFinalize()
        {
            foreach (var replacementToken in replacementTokens)
            {
                try
                {
                    foreach (var lang in RoR2.Language.steamLanguageTable)
                    {
                        var langName = lang.Value.webApiName;
                        var newString = FormatToken(replacementToken.token, langName, replacementToken.args);
                        LanguageAPI.Add(replacementToken.token, newString, langName);

                        /* try
                        {
                        }catch
                        {
                            Main._logger.LogError($"Failed setting up token \"{langTokenValue.token}\" for language {langTokenValue.lang}." +
                                $"\nFollowing parameters where given:");
                            foreach (var str in langTokenValue.strings)
                            {
                                Main._logger.LogMessage(str);
                            }
                        }
                        if (langTokenValue.lang == "en")
                        {
                            Main._logger.LogMessage($"{langTokenValue.token} {newString}");
                        } */
                    }
                }
                catch
                {
                    Main._logger.LogError($"Failed to load replacement token {replacementToken.token}"
                    + $"Params: {replacementToken.args.ToString()}");
                }
            }
        }

        private static void PostDeferTokenFinalize()
        {
            foreach (var postReplacementToken in postReplacementTokens)
            {
                try
                {
                    foreach (var lang in RoR2.Language.steamLanguageTable)
                    {
                        var langName = lang.Value.webApiName;

                        List<string> resolvedReplacementTokens = new List<string>();
                        foreach (var tokenArg in postReplacementToken.replacementTokens)
                        {
                            resolvedReplacementTokens.Add(RoR2.Language.GetString(tokenArg, langName));
                        }

                        var newString = FormatToken(postReplacementToken.baseToken, langName, resolvedReplacementTokens.ToArray());
                        LanguageAPI.Add(postReplacementToken.baseToken, newString, langName);
                    }
                }
                catch
                {
                    Main._logger.LogError($"Failed to load post replacement token {postReplacementToken.baseToken}"
                    + $"Params are wrong?");
                }
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

        public class RBS_DestroyLogbookHookOnDestroy : MonoBehaviour
        {
            public void Start()
            {
                On.RoR2.Language.GetString_string += OverrideGetStringLogbook;
            }

            private string OverrideGetStringLogbook(On.RoR2.Language.orig_GetString_string orig, string token)
            {
                if (logbookTokenOverrideDict.TryGetValue(token, out string overrideToken))
                {
                    token = overrideToken;
                }
                return orig(token);
            }

            public void OnDestroy()
            {
                On.RoR2.Language.GetString_string -= OverrideGetStringLogbook;
            }
        }
    }
}