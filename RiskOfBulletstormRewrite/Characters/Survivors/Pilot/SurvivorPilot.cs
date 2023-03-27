using System;
using System.Collections.Generic;
using System.Text;
using RiskOfBulletstormRewrite.Modules;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Characters.Survivors.Pilot
{
    internal class SurvivorPilot : SurvivorBase<SurvivorPilot>
    {
        public override string CharacterName => "Pilot";

        public override string CharacterLangTokenName => "PILOT";

        public override Sprite CharacterIcon => Assets.NullSprite;

        public override Color SurvivorColor => Color.blue;


    }
}
