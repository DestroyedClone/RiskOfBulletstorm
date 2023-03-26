using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Achievements
{
    public abstract class RoBBaseAchievement : BaseAchievement
    {
        public abstract string BaseToken { get; }

        public virtual Sprite Icon => Assets.LoadSprite($"ACHIEVEMENT_{BaseToken}");

    }
}
