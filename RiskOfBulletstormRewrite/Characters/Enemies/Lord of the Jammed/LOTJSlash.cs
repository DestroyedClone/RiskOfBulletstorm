using EntityStates;

namespace RiskOfBulletstormRewrite.Characters.Enemies.Lord_of_the_Jammed
{
    public class LOTJSlash : BaseSkillState
    {
        public void FireBulletInCardinalDirections()
        {

        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}