using EntityStates;

namespace RiskOfBulletstormRewrite.Enemies
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