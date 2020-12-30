using R2API.Utils;
using Rewired;
using RoR2;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstorm.ExtraSkillSlotsOverrides
{
    internal class ExtraPlayerCharacterMasterController : NetworkBehaviour
    {
        private PlayerCharacterMasterController playerCharacterMasterController;
        private ExtraInputBankTest extraInputBankTest;

        public void Awake()
        {
            playerCharacterMasterController = GetComponent<PlayerCharacterMasterController>();
        }

        public void FixedUpdate()
        {
            if (!extraInputBankTest || !playerCharacterMasterController.hasEffectiveAuthority || !extraInputBankTest)
            {
                return;
            }

            var blankButtonState = false;
            var backpackModState = false;
            var backpackCycleLeftState = false;
            var backpackCycleRightState = false;

            if (PlayerCharacterMasterController.CanSendBodyInput(playerCharacterMasterController.networkUser, out _, out var inputPlayer, out _))
            {
                blankButtonState = inputPlayer.GetButton(RewiredActions.ActivateBlank);
                backpackModState = inputPlayer.GetButton(RewiredActions.BackpackModifier);
                backpackCycleLeftState = inputPlayer.GetButton(RewiredActions.BackpackCycleLeft);
                backpackCycleRightState = inputPlayer.GetButton(RewiredActions.BackpackCycleRight);
            }

            extraInputBankTest.extraSkill1.PushState(blankButtonState);
            extraInputBankTest.extraSkill2.PushState(backpackModState);
            extraInputBankTest.extraSkill3.PushState(backpackCycleLeftState);
            extraInputBankTest.extraSkill4.PushState(backpackCycleRightState);
        }

        internal static void SetBodyOverrideHook(On.RoR2.PlayerCharacterMasterController.orig_SetBody orig, PlayerCharacterMasterController self, GameObject newBody)
        {
            orig(self, newBody);

            var extraMaster = self.GetComponent<ExtraPlayerCharacterMasterController>();
            if (!extraMaster)
            {
                return;
            }

            extraMaster.extraInputBankTest = self.body ? self.body.GetComponent<ExtraInputBankTest>() : null;
        }
    }
}
