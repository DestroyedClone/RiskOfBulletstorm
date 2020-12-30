using R2API.Utils;
using Rewired;
using Rewired.Data;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RiskOfBulletstorm.ExtraSkillSlotsOverrides
{
    internal static class ExtraInputs
    {
        internal static void AddActionsToInputCatalog()
        {
            var extraActionAxisPair_Blank = new InputCatalog.ActionAxisPair(RewiredActions.ActivateBlankName, AxisRange.Full);
            var extraActionAxisPair_BackpackModifier = new InputCatalog.ActionAxisPair(RewiredActions.BackpackModifierName, AxisRange.Full);
            var extraActionAxisPair_BackpackCycleLeft = new InputCatalog.ActionAxisPair(RewiredActions.BackpackCycleLeftName, AxisRange.Full);
            var extraActionAxisPair_BackpackCycleRight = new InputCatalog.ActionAxisPair(RewiredActions.BackpackCycleRightName, AxisRange.Full);

            InputCatalog.actionToToken.Add(extraActionAxisPair_Blank, "Activate Blank");
            InputCatalog.actionToToken.Add(extraActionAxisPair_BackpackModifier, "Hold to modify backpack slot");
            InputCatalog.actionToToken.Add(extraActionAxisPair_BackpackCycleLeft, "Cycle Backpack slot left");
            InputCatalog.actionToToken.Add(extraActionAxisPair_BackpackCycleRight, "Cycle Backpack slot right");
        }

        internal static void AddCustomActions(Action<UserData> orig, UserData self)
        {
            var firstAction = CreateInputAction(RewiredActions.ActivateBlank, RewiredActions.ActivateBlankName);
            var secondAction = CreateInputAction(RewiredActions.BackpackModifier, RewiredActions.BackpackModifierName);
            var thirdAction = CreateInputAction(RewiredActions.BackpackCycleLeft, RewiredActions.BackpackCycleLeftName);
            var fourthAction = CreateInputAction(RewiredActions.BackpackCycleRight, RewiredActions.BackpackCycleRightName);

            var actions = self.GetFieldValue<List<InputAction>>("actions");

            actions?.Add(firstAction);
            actions?.Add(secondAction);
            actions?.Add(thirdAction);
            actions?.Add(fourthAction);

            orig(self);
        }

        internal static InputAction CreateInputAction(int id, string name, InputActionType type = InputActionType.Button)
        {
            var action = new InputAction();

            action.SetFieldValue("_id", id);
            action.SetFieldValue("_name", name);
            action.SetFieldValue("_type", type);
            action.SetFieldValue("_descriptiveName", name);
            action.SetFieldValue("_behaviorId", 0);
            action.SetFieldValue("_userAssignable", true);
            action.SetFieldValue("_categoryId", 0);

            return action;
        }
    }
}
