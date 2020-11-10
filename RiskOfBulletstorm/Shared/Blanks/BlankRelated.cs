using System.Collections.Generic;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using System.Net;

namespace RiskOfBulletstorm.Shared
{
    public static class BlankRelated
    {
        public static bool FireBlank(CharacterBody attacker, Vector3 corePosition, float blankRadius, float damageMult, float projectileClearRadius, bool consumeBlank = false)
        {
            //var body = attacker.GetComponent<CharacterBody>();
            var blankAmount = attacker.inventory.GetItemCount(Items.Blank.instance.catalogIndex);
            if (blankAmount == 0 && consumeBlank) //if needs blank and have no blank
            {
                Debug.LogError("[RiskOfBulletstorm]: Blank was required, but player had no blank!");
                return false;
            }
            new BlastAttack
            {
                attacker = attacker.gameObject, //who
                inflictor = blankObject, //how
                position = corePosition,
                procCoefficient = 0f,
                losType = BlastAttack.LoSType.None,
                falloffModel = BlastAttack.FalloffModel.None,
                baseDamage = attacker.damage * damageMult,
                damageType = DamageType.Stun1s,
                //crit = self.RollCrit(),
                radius = blankRadius,
                teamIndex = TeamIndex.Player,
                baseForce = 900,
                //baseForce = 2000f,
                //bonusForce = new Vector3(0, 1600, 0)
            }.Fire();

            if (projectileClearRadius != 0)
            { 
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
                        Object.Destroy(projectileController2.gameObject);
                    }
                    j++;
                }
            }
            if (consumeBlank)
            {
                attacker.inventory.RemoveItem(Items.Blank.instance.catalogIndex);
            }
            return true;
        }

        public static GameObject blankObject;
    }
}
