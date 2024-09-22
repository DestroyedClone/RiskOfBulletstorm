﻿using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Utils
{
    public static class Buffs
    {
        public static BuffDef MustacheBuff;
        public static BuffDef MetronomeTrackerBuff;
        public static BuffDef BloodiedScarfBuff;
        public static BuffDef AlphaBulletBuff;
        public static BuffDef DodgeRollBuff;
        public static BuffDef ArtifactSpeedUpBuff;

        public static void CreateBuffs()
        {
            //Buffs
            MustacheBuff = AddBuff("Power of Commerce",
            Modules.Assets.LoadSprite("BUFF_MUSTACHE"),
            Color.yellow, false, true);

            BloodiedScarfBuff = AddBuff("Vulnerable",
            Modules.Assets.LoadSprite("BUFF_BLOODIEDSCARF"),
            Color.red, true, false);
            AlphaBulletBuff = AddBuff("Alpha Bullet Damage",
            Modules.Assets.LoadSprite("BUFF_ALPHABULLET"),
            Color.yellow, false, true);
            DodgeRollBuff = AddBuff("Dodgeroll Buff",
            Modules.Assets.LoadSprite("BUFF_DODGEROLL"),
            Color.yellow,
            true, false);

            ArtifactSpeedUpBuff = AddBuff("Speed Up",
                Modules.Assets.LoadSprite("BUFF_SPEEDUPOOC"),
                Color.blue,
                false,
                false);

            //Trackers
            MetronomeTrackerBuff = AddBuff("Metronome Stacks (Display)",
                Modules.Assets.LoadSprite("BUFF_METRONOME"),
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