using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using EntityStates;
using EntityStates.Commando;

namespace RiskOfBulletstorm.Releases.Items.Lunar
{
    public class DodgeRollSkillState : DodgeState
    {
		public static float durationToFire;
		public static GameObject muzzleEffectPrefab;
		public static GameObject tracerEffectPrefab;
		public static GameObject hitEffectPrefab;
		public static float damageCoefficient;
		public static float force;

		public override void OnEnter()
		{
			base.OnEnter();
			characterBody.AddBuff(BuffIndex.HiddenInvincibility);
		}
		public override void FixedUpdate()
		{
			base.FixedUpdate();
		}
		public void FakeRollModel(uint axis)
		{
			this.characterBody;
		}


        public override void OnExit()
        {
            base.OnExit();
			characterBody.RemoveBuff(BuffIndex.HiddenInvincibility);
        }
    }
}
