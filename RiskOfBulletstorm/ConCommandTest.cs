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
        private static void DeathStateClear(ConCommandArgs args)
        {
            UserProfile userProfile = args.GetSenderLocalUser().userProfile;
            var viewableNames = (from node in ViewablesCatalog.rootNode.Descendants()
                                         where node.shouldShowUnviewed(userProfile)
                                         select node.fullName).ToArray();
            foreach (var viewableName in viewableNames)
            {
                if (string.IsNullOrEmpty(viewableName))
                {
                    continue;
                }
                foreach (LocalUser localUser in LocalUserManager.readOnlyLocalUsersList)
                {
                    localUser.userProfile.MarkViewableAsViewed(viewableName);
                }
            }
        }
    }
}
