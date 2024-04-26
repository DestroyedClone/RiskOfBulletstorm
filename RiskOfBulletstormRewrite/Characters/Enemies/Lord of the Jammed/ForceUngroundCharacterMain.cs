using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskOfBulletstormRewrite.Characters.Enemies.Lord_of_the_Jammed
{
    internal class ForceUngroundCharacterMain : GenericCharacterMain
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor.hasEffectiveAuthority)
                characterMotor.Motor.ForceUnground();
        }
    }
}
