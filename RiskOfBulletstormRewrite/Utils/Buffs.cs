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

        public static BuffDef MetronomeTrackerBuff;

        public static void CreateBuffs()
        {
            //Buffs
            MustacheBuff = AddBuff("Power of Commerce", LegacyResourcesAPI.Load<Sprite>(""), Color.yellow, false, true);

            //Trackers
            MetronomeTrackerBuff = AddBuff("Metronome Stacks (Display)",
                Main.MainAssets.LoadAsset<Sprite>("Assets/Textures/Icons/Buffs/Metronome.png"),
                Color.blue,
                false,
                true);
        }

        public static BuffDef AddBuff(string name, Sprite iconSprite, Color buffColor, bool isDebuff = false, bool canStack = false)
        {
            var customBuff = ScriptableObject.CreateInstance<BuffDef>();

            customBuff.name = name;
            customBuff.buffColor = buffColor;
            customBuff.canStack = canStack;
            customBuff.isDebuff = isDebuff;
            customBuff.iconSprite = iconSprite;

            ContentAddition.AddBuffDef(customBuff);
            return customBuff;
        }
    }
}
