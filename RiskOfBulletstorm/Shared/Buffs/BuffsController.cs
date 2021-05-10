using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using System;
using RiskOfBulletstorm.Items;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Shared.Buffs
{
    public static class BuffsController
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("CHARM: Should Bosses (including Umbrae) get charmed?", AutoConfigFlags.PreventNetMismatch)]
        public static bool Config_Charm_Boss { get; private set; } = false;

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

        public static void RegisterBuff(BuffDef buffDef, Color buffColor, bool canStack, bool isDebuff, string iconPath, string name)
        {
            buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.iconSprite = assetBundle.LoadAsset<Sprite>(iconPath);
            buffDef.name = name;
            BuffAPI.Add(new CustomBuff(buffDef));
        }

        public static void RegisterBuffs()
        {
            RegisterBuff(Anger,
                Color.red,
                false,
                false,
                "@RiskOfBulletstorm:Assets/Textures/Icons/Buffs/Enraged.png",
                "Enraged");

            RegisterBuff(Charm,
                new Color32(201, 42, 193, 255),
                false,
                true,
                "@RiskOfBulletstorm:Assets/Textures/Icons/Buffs/Charmed.png",
                "Charmed");

            RegisterBuff(Jammed,
                new Color32(150, 10, 10, 255),
                false,
                false,
                "@RiskOfBulletstorm:Assets/Textures/Icons/Buffs/Jammed.png",
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
            if (sender.HasBuff(Anger)) { args.damageMultAdd += EnragingPhoto.instance.EnragingPhoto_DmgBoost; }
            if (sender.HasBuff(Jammed))
            {
                args.damageMultAdd += curse.Curse_DamageBoost;
                args.critAdd += curse.Curse_CritBoost;
                args.attackSpeedMultAdd += curse.Curse_AttackSpeedBoost;
                args.moveSpeedMultAdd += curse.Curse_MoveSpeedBoost;
                args.baseHealthAdd += curse.Curse_HealthBoost;
            }
        }
    }
}
