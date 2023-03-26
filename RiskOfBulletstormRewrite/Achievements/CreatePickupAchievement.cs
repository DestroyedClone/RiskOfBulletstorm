using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskOfBulletstormRewrite.Achievements
{
    public abstract class CreatePickupAchievement : BaseAchievement
    {
        public virtual PickupDef PickupDef { get; } = null;

        public void CreatePickup
    }
}
