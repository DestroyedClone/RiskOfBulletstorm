using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;

namespace RiskOfBulletstorm.ExtraSkillSlotsOverrides
{
    internal static class UIHooks
    {
        #region SettingsPanelController
        internal static void SettingsPanelControllerStart(On.RoR2.UI.SettingsPanelController.orig_Start orig, RoR2.UI.SettingsPanelController self)
        {
            orig(self);

            if (self.name == "SettingsSubPanel, Controls (M&KB)" || self.name == "SettingsSubPanel, Controls (Gamepad)")
            {
                var jumpBindingTransform = self.transform.Find("Scroll View").Find("Viewport").Find("VerticalLayout").Find("SettingsEntryButton, Binding (Jump)");

                AddActionBindingToSettings(RewiredActions.ActivateBlankName, jumpBindingTransform);
                AddActionBindingToSettings(RewiredActions.BackpackModifierName, jumpBindingTransform);
                AddActionBindingToSettings(RewiredActions.BackpackCycleLeftName, jumpBindingTransform);
                AddActionBindingToSettings(RewiredActions.BackpackCycleRightName, jumpBindingTransform);
            }
        }
        internal static void AddActionBindingToSettings(string actionName, Transform buttonToCopy)
        {
            var inputBindingObject = GameObject.Instantiate(buttonToCopy, buttonToCopy.parent);
            var inputBindingControl = inputBindingObject.GetComponent<InputBindingControl>();
            inputBindingControl.actionName = actionName;
            //Usualy calling awake is bad as it's supposed to be called only by unity.
            //But in this case it is necessary to apply "actionName" change.
            inputBindingControl.Awake();
        }
        #endregion
    }
}
