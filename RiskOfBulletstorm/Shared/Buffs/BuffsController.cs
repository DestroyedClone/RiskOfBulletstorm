using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using System;
using RiskOfBulletstorm.Items;

namespace RiskOfBulletstorm.Shared.Buffs
{
    public static class BuffsController
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("CHARM: Should Bosses (including Umbrae) get charmed?", AutoConfigFlags.PreventNetMismatch)]
        public static bool Config_Charm_Boss { get; private set; } = false;

        //public static BuffIndex Burn { get; private set; } 
        //public static BuffIndex Poison { get; private set; }
        //public static BuffIndex Curse { get; private set; }
        //public static BuffIndex Stealth { get; private set; }
        //public static BuffIndex Petrification { get; private set; }
        public static BuffIndex Anger { get; private set; } // done
        //public static BuffIndex Buffed { get; private set; }
        //public static BuffIndex BurnEnemy { get; private set; } //burn without heal negation
        //public static BuffIndex PoisonEnemy { get; private set; } //blight
        public static BuffIndex Charm { get; private set; } // done
        //public static BuffIndex Encheesed { get; private set; }
        //public static BuffIndex Fear { get; private set; } //classic items' fear
        public static BuffIndex Jammed { get; private set; } // done
        //public static BuffIndex Slow { get; private set; } // slow
        //public static BuffIndex Freeze { get; private set; } 
        //public static BuffIndex Stun { get; private set; } // existing status from damagetype
        //public static BuffIndex Weakened { get; private set; }
        //public static BuffIndex Tangled { get; private set; }
        //public static BuffIndex Encircled { get; private set; }
        //public static BuffIndex Glittered { get; private set; }
        //public static BuffIndex Bloody { get; private set; }


        private static readonly CurseController curse = CurseController.instance;
        public static void Init()
        {
            RegisterBuffs();
            Hooks();
        }

        public static void RegisterBuffs()
        {
            var angerBuff = new CustomBuff(
            new BuffDef
            {
                buffColor = Color.red,
                canStack = false,
                isDebuff = false,
                iconPath = "@RiskOfBulletstorm:Assets/Textures/Icons/Buffs/Enraged.png",
                name = "Enraged",
            });
            Anger = BuffAPI.Add(angerBuff);

            var charmedBuff = new CustomBuff(
            new BuffDef
            {
                name = "Charmed",
                buffColor = new Color32(201, 42, 193, 255),
                canStack = false,
                isDebuff = true,
                iconPath = "@RiskOfBulletstorm:Assets/Textures/Icons/Buffs/Charmed.png",
            });
            Charm = BuffAPI.Add(charmedBuff);

            var jammedBuff = new CustomBuff(
            new BuffDef
            {
                name = "Affix_Jammed",
                buffColor = new Color32(150, 10, 10, 255),
                canStack = false,
                isDebuff = false,
                iconPath = "@RiskOfBulletstorm:Assets/Textures/Icons/Buffs/Jammed.png",
                //eliteIndex = JammedEliteIndex,
            });
            Jammed = BuffAPI.Add(jammedBuff);
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
