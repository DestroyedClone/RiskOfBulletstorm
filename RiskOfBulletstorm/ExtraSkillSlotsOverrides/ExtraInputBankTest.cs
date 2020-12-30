using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfBulletstorm.ExtraSkillSlotsOverrides
{
    [RequireComponent(typeof(InputBankTest))]
    public class ExtraInputBankTest : MonoBehaviour
    {
        public InputBankTest.ButtonState extraSkill1;
        public InputBankTest.ButtonState extraSkill2;
        public InputBankTest.ButtonState extraSkill3;
        public InputBankTest.ButtonState extraSkill4;

        internal static bool CheckAnyButtonDownOverrideHook(On.RoR2.InputBankTest.orig_CheckAnyButtonDown orig, InputBankTest self)
        {
            if (orig(self))
            {
                return true;
            }
            var extraInputBankTest = self.GetComponent<ExtraInputBankTest>();

            return
                extraInputBankTest.extraSkill1.down ||
                extraInputBankTest.extraSkill2.down ||
                extraInputBankTest.extraSkill3.down ||
                extraInputBankTest.extraSkill4.down;
        }
    }
}
