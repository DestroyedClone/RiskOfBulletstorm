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
        public static BuffDef MustacheBuff;

        public static void CreateBuffs()
        {
            MustacheBuff = AddBuff("Power of Commerce", Resources.Load<Sprite>(""), Color.yellow, false, true);
        }

        public static BuffDef AddBuff(string name, Sprite iconSprite, Color buffColor, bool isDebuff = false, bool canStack = false)
        {
            var customBuff = ScriptableObject.CreateInstance<BuffDef>();

            customBuff.name = name;
            customBuff.buffColor = buffColor;
            customBuff.canStack = canStack;
            customBuff.isDebuff = isDebuff;
            customBuff.iconSprite = iconSprite;

            BuffAPI.Add(new CustomBuff(customBuff));
            return customBuff;
        }
    }
}
