using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using R2API;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Utils
{
    public static class Buffs
    {
        public static CustomBuff MustacheBuff;

        public static void CreateBuffs()
        {
            MustacheBuff = new CustomBuff("Power of Commerce", Resources.Load<Sprite>(""), Color.yellow, false, true);
        }
    }
}
