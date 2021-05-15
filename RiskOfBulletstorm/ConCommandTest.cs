using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using static R2API.Utils.CommandHelper;
using R2API.Utils;
using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.UI;
using System.Collections.ObjectModel;
using System.Linq;

namespace RiskOfBulletstorm
{
    public static class ConsoleCommands
    {
        [ConCommand(commandName = "viewall", flags = ConVarFlags.ExecuteOnServer, helpText = "Views all tags visible.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Empty Arg required")]
        private static void ViewAllUnviewed(ConCommandArgs args)
        {
            Debug.Log("testing");
            UserProfile userProfile = args.GetSenderLocalUser().userProfile;
            var viewableNames = (from node in ViewablesCatalog.rootNode.Descendants()
                                         where node.shouldShowUnviewed(userProfile)
                                         select node.fullName);
            foreach (var viewableName in viewableNames.ToList())
            {
                Debug.Log(viewableName);
                if (string.IsNullOrEmpty(viewableName))
                {
                    Debug.Log("Null, cancelling");
                    continue;
                }
                foreach (LocalUser localUser in LocalUserManager.readOnlyLocalUsersList)
                {
                    Debug.Log("marking as viewed");
                    localUser.userProfile.MarkViewableAsViewed(viewableName);
                }
            }
        }

        [ConCommand(commandName = "viewall_normal", flags = ConVarFlags.ExecuteOnServer, helpText = "Views all tags visible.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Empty Arg required")]
        private static void ViewAllUnviewedaaa(ConCommandArgs args)
        {
            UserProfile userProfile = args.GetSenderLocalUser().userProfile;
            var viewableNames = (from node in ViewablesCatalog.rootNode.Descendants()
                                 where node.shouldShowUnviewed(userProfile)
                                 select node.fullName);
            foreach (var viewableName in viewableNames.ToList())
            {
                Debug.Log(viewableName);
                if (string.IsNullOrEmpty(viewableName))
                {
                    Debug.Log("Null, cancelling");
                    continue;
                }
                foreach (LocalUser localUser in LocalUserManager.readOnlyLocalUsersList)
                {
                    Debug.Log("marking as viewed");
                    localUser.userProfile.MarkViewableAsViewed(viewableName);
                }
            }
        }

        [ConCommand(commandName = "viewall2", flags = ConVarFlags.ExecuteOnServer, helpText = "Views all tags visible.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Empty Arg required")]
        private static void ViewAllUnviewed2(ConCommandArgs args)
        {
            UserProfile userProfile = args.GetSenderLocalUser().userProfile;
            var listViewables = (from node in ViewablesCatalog.rootNode.Descendants()
                                         select node.fullName).ToArray<string>();
            foreach (var viewableName in listViewables)
            {
                if (userProfile.HasViewedViewable(viewableName))
                {
                    return;
                }
                userProfile.viewedViewables.Add(viewableName);
                userProfile.RequestSave(false);
            }
        }
        [ConCommand(commandName = "viewallonscreen", flags = ConVarFlags.ExecuteOnServer, helpText = "Views all tags visible.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Empty Arg required")]
        private static void ViewAllUnviewedScreen(ConCommandArgs args)
        {
            var tags = GameObject.FindObjectsOfType<RoR2.UI.ViewableTag>();
            foreach (var tag in tags)
            {
                tag.InvokeMethod("TriggerView");
            }
        }
    }
}
