﻿using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static R2API.RecalculateStatsAPI;
using RiskOfBulletstorm.Items;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Shared.Buffs
{
    public static class BuffsController
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("CHARM: Should Bosses (including Umbrae) get charmed?", AutoConfigFlags.PreventNetMismatch)]
        public static bool Config_Charm_Boss { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("ENRAGE: How much extra damage should you deal?", AutoConfigFlags.PreventNetMismatch)]
        public static float Config_Enrage_Damage { get; private set; } = 1f;

        //public static BuffDef Burn { get; private set; } 
        //public static BuffDef Poison { get; private set; }
        //public static BuffDef Curse { get; private set; }
        //public static BuffDef Stealth { get; private set; }
        //public static BuffDef Petrification { get; private set; }
        public static BuffDef Anger { get; private set; } // done
        //public static BuffDef Buffed { get; private set; }
        //public static BuffDef BurnEnemy { get; private set; } //burn without heal negation
        //public static BuffDef PoisonEnemy { get; private set; } //blight
        public static BuffDef Charm { get; private set; } // done
        //public static BuffDef Encheesed { get; private set; }
        //public static BuffDef Fear { get; private set; } //classic items' fear
        public static BuffDef Jammed { get; private set; } // done
        //public static BuffDef Slow { get; private set; } // slow
        //public static BuffDef Freeze { get; private set; } 
        //public static BuffDef Stun { get; private set; } // existing status from damagetype
        //public static BuffDef Weakened { get; private set; }
        //public static BuffDef Tangled { get; private set; }
        //public static BuffDef Encircled { get; private set; }
        //public static BuffDef Glittered { get; private set; }
        //public static BuffDef Bloody { get; private set; }


        private static readonly CurseController curse = CurseController.instance;
        public static void Init()
        {
            RegisterBuffs();
            Hooks();
        }

        public static BuffDef RegisterBuff(Color buffColor, bool canStack, bool isDebuff, string iconPath, string name)
        {
            var buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.iconSprite = assetBundle.LoadAsset<Sprite>(iconPath);
            buffDef.name = BulletstormPlugin.displayName + ":" + name;
            BuffAPI.Add(new CustomBuff(buffDef));
            return buffDef;
        }

        public static void RegisterBuffs()
        {
            Anger = RegisterBuff(Color.red,
                false,
                false,
                "Assets/Textures/Icons/Buffs/Enraged.png",
                "Enraged");

            Charm = RegisterBuff(new Color32(201, 42, 193, 255),
                false,
                true,
                "Assets/Textures/Icons/Buffs/Charmed.png",
                "Charmed");

            Jammed = RegisterBuff(new Color32(150, 10, 10, 255),
                false,
                false,
                "Assets/Textures/Icons/Buffs/Jammed.png",
                "Affix_Jammed");
        }

        public static void Hooks()
        {
            // Buffs //
            CharmBuff.Install();
            GetStatCoefficients += StatHooks_GetStatCoefficients;
        }

        private static void StatHooks_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.HasBuff(Anger)) { args.damageMultAdd += Config_Enrage_Damage; }
            if (sender.HasBuff(Jammed))
            {
                args.damageMultAdd += curse.JammedDamageBoost;
                args.critAdd += curse.JammedCritBoost;
                args.attackSpeedMultAdd += curse.JammedAttackSpeedBoost;
                args.moveSpeedMultAdd += curse.JammedMoveSpeedBoost;
                args.baseHealthAdd += curse.JammedHealthBoost;
            }
        }
    }
}
