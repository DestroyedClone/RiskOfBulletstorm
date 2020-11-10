using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Shared
{
    public static class SharedMethods
    {
        public static void GiveLeadEffect(DamageInfo damageInfo, GameObject victim, ItemIndex itemIndex, BuffIndex buffIndex, DotController.DotIndex dotIndex, float duration, float procChance = 0.05f, float damageMultiplier = 0.25f)
        {
            CharacterBody body = damageInfo.attacker.GetComponent<CharacterBody>();
            var InventoryCount = body.inventory.GetItemCount(itemIndex);
            var procChanceFinal = damageInfo.procCoefficient * procChance * InventoryCount;

            if (!body || InventoryCount < 1 || !Util.CheckRoll(procChanceFinal, body.master) || !damageInfo.rejected) return;

            victim.gameObject.GetComponent<CharacterBody>()?.AddTimedBuff(buffIndex, duration);
            //AddTimedBuffAuthority vs AddTimedBuff?
            DotController.InflictDot(victim.gameObject, body.gameObject, dotIndex, duration, damageMultiplier); //thanks komrade
        }
    }
}
