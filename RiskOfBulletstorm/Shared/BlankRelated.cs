using System.Collections.Generic;
using RoR2;
using UnityEngine;
using RoR2.Projectile;

namespace RiskOfBulletstorm.Shared
{
    public static class BlankRelated
    {
        public static void FireBlank(GameObject attacker, Vector3 corePosition, float blankRadius, float damageMult, float projectileClearRadius)
        {
            var body = attacker.GetComponent<CharacterBody>();
            new BlastAttack
            {
                attacker = attacker,
                position = corePosition,
                procCoefficient = 0f,
                losType = BlastAttack.LoSType.None,
                falloffModel = BlastAttack.FalloffModel.None,
                baseDamage = body.damage * damageMult,
                damageType = DamageType.Stun1s,
                //crit = self.RollCrit(),
                radius = blankRadius,
                teamIndex = TeamIndex.Player,
                baseForce = 1000f,
                bonusForce = new Vector3(0, 1000, 0)
            }.Fire();

            if (projectileClearRadius != 0)
            { //Remove all Projectiles
                if (projectileClearRadius == -1) { projectileClearRadius = 999; }
                float blankRadiusSquared = projectileClearRadius * projectileClearRadius;
                List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
                List<ProjectileController> list = new List<ProjectileController>();
                int i = 0;
                int count = instancesList.Count;
                while (i < count)
                {
                    ProjectileController projectileController = instancesList[i];
                    if (projectileController.teamFilter.teamIndex != TeamIndex.Player && (projectileController.transform.position - corePosition).sqrMagnitude < blankRadiusSquared)
                    {
                        list.Add(projectileController);
                        Chat.AddMessage("Projectile Added: " + projectileController.ToString());
                    }
                    i++;
                }
                int j = 0;
                int count2 = list.Count;
                while (j < count2)
                {
                    ProjectileController projectileController2 = list[j];
                    if (projectileController2)
                    {
                        UnityEngine.Object.Destroy(projectileController2.gameObject);
                    }
                    j++;
                }
            }
        }
    }
}
