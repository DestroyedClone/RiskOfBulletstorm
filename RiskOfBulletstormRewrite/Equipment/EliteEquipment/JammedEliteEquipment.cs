﻿using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Controllers;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment.EliteEquipment
{
    public class JammedEliteEquipment : EliteEquipmentBase<JammedEliteEquipment>
    {
        public override string EliteEquipmentName => "Jammed Elite";

        public override string EliteEquipmentLangTokenName => "JAMMEDELITE";

        public override GameObject EliteEquipmentModel => Assets.NullModel;

        public override Sprite EliteEquipmentIcon => Assets.NullSprite;

        public override Sprite EliteBuffIcon => Assets.NullSprite;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            CreateEliteTiers();
            CreateElite();
            Hooks();
        }

        private void CreateConfig(ConfigFile config)
        {

        }

        private void CreateEliteTiers()
        {
            //For this, if you want to create your own elite tier def to place your elite, you can do it here.
            //Otherwise, don't set CanAppearInEliteTiers and it will appear in the first vanilla tier.

            //In this we create our own tier which we'll put our elites in. It has:
            //- 6 times the base elite cost.
            //- 3 times the base elite damage boost.
            //- 4.5 times the base elite health boost.
            //- It can only become available to spawn after the player has looped at least once.

            //Additional note: since this accepts an array, it supports multiple elite tier defs, but do not put a cost of 0 on the cost multiplier.

            CanAppearInEliteTiers = new CombatDirector.EliteTierDef[]
            {
                new CombatDirector.EliteTierDef()
                {
                    costMultiplier = CombatDirector.baseEliteCostMultiplier * 6,
                    eliteTypes = Array.Empty<EliteDef>(),
                    isAvailable = SetAvailability
                }
            };
        }

        private bool SetAvailability(SpawnCard.EliteRules arg)
        {
            //return Run.instance.loopClearCount > 0 && arg == SpawnCard.EliteRules.Default;
            return CurseController.instance.teamwideCurseCount > 0 && arg == SpawnCard.EliteRules.Default;
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(EliteBuffDef))
            {
                args.critAdd += 100f;
            }
        }

        //If you want an on use effect, implement it here as you would with a normal equipment.
        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            return false;
        }
    }
}
