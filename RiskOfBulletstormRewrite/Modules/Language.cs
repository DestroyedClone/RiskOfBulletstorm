using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Modules
{
    public class LanguageOverrides
    {
        public static ConfigFile config;

        public const string LanguageTokenPrefix = "RISKOFBULLETSTORM_";
        public const string LanguageTokenPrefixArtifact = LanguageTokenPrefix + "ARTIFACT_";

        //public static event Action<ConfigFile> onConfigLoaded;

        public struct ReplacementToken
        {
            public string assignedToken;
            public string formatToken;
            public object[] args;
        }

        ///<summary>
        ///Helper method to defer language tokens.
        ///</summary>
        public static void DeferToken(string token, params object[] args)
        {
            //Main._logger.LogMessage($"Deferring {token} w/ lang {lang}");
            replacementTokens.Add(new ReplacementToken() { assignedToken = token, formatToken = token, args = args });
        }

        ///<summary>
        ///Helper method to defer language tokens. All passed args must be existing tokens.
        ///</summary>
        public static void DeferLateTokens(string token, params object[] args)
        {
            postReplacementTokens.Add(new ReplacementToken() { assignedToken = token, formatToken = token, args = args });
        }

        public static void DeferUniqueToken(string assignedToken, string formatToken, params object[] args)
        {
            replacementTokens.Add(new ReplacementToken() { assignedToken = assignedToken, formatToken = formatToken, args = args });
        }

        ///<summary>
        ///Helper method to defer language tokens. All passed args must be existing tokens.
        ///</summary>
        public static void DeferLateUniqueTokens(string assignedToken, string formatToken, params object[] args)
        {
            postReplacementTokens.Add(new ReplacementToken() { assignedToken = assignedToken, formatToken = formatToken, args = args });
        }

        public static List<ReplacementToken> replacementTokens = new List<ReplacementToken>();
        public static List<ReplacementToken> postReplacementTokens = new List<ReplacementToken>();
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

        private static string FormatToken(ReplacementToken replacementToken, string langName)
        {
            //GetStringFormatted uses currentLanguage, while we have to use other languages
            var oldString = RoR2.Language.GetString(replacementToken.formatToken, langName);
            var newString = string.Format(oldString, replacementToken.args);
            return newString;
        }

        private static string FormatToken(string formatToken, string[] args, string langName)
        {
            //GetStringFormatted uses currentLanguage, while we have to use other languages
            var oldString = RoR2.Language.GetString(formatToken, langName);
            var newString = string.Format(oldString, args);
            return newString;
        }

        private static void AssignToken(ReplacementToken token, string langName)
        {
            var newString = FormatToken(token, langName);
            var assignedToken = token.formatToken;
            if (token.assignedToken == null || token.assignedToken != token.formatToken)
            {
                assignedToken = token.assignedToken;
            }
            LanguageAPI.Add(assignedToken, newString, langName);
        }

        private static void SetupLanguage()
        {
            Main._logger.LogMessage($"Setting up language with {replacementTokens.Count + postReplacementTokens.Count} tokens.");
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
                        AssignToken(replacementToken, langName);

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
                catch (Exception ex)
                {
                    Main._logger.LogError($"Failed to load replacement token {replacementToken.assignedToken}"
                    + $"Params: {replacementToken.args.ToString()}");
                    Main._logger.LogError(ex.ToString());
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
                        foreach (var tokenArg in postReplacementToken.args)
                        {
                            resolvedReplacementTokens.Add(RoR2.Language.GetString(tokenArg.ToString(), langName));
                        }

                        var assignedToken = postReplacementToken.formatToken;
                        if (postReplacementToken.assignedToken == null || postReplacementToken.assignedToken != postReplacementToken.formatToken)
                        {
                            assignedToken = postReplacementToken.assignedToken;
                        }

                        var newString = FormatToken(postReplacementToken.formatToken, resolvedReplacementTokens.ToArray(), langName);
                        LanguageAPI.Add(assignedToken, newString, langName);
                    }
                }
                catch
                {
                    Main._logger.LogError($"Failed to load post replacement token {postReplacementToken.assignedToken}"
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