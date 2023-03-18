using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using EntityStates;

namespace RiskOfBulletstormRewrite.Enemies.WeirdEgg
{
    public class WeirdEggMain : GenericCharacterMain, IOnTakeDamageServerReceiver
    {
        public int stagesCleared = 0;

        void IOnTakeDamageServerReceiver.OnTakeDamageServer(DamageReport damageReport)
        {
            if (damageReport.damageInfo.damageType.HasFlag(DamageType.IgniteOnHit)
                || damageReport.damageInfo.damageType.HasFlag(DamageType.PercentIgniteOnHit))
            {
                SpawnEel();
            } else
            {
                if (CheckStageClears())
                {
                    SpawnItem();
                } else
                {
                    SpawnSlug();
                }
            }
            return;
        }

        public void SpawnSlug() { }
        public void SpawnEel() { }
        public void SpawnItem() { }

        public bool CheckStageClears()
        {
            if (Run.instance)
            {
                return Run.instance.stageClearCount > 4;
            }
            return true;
        }

    }
}
