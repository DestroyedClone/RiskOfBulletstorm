using System;
using RoR2;
using UnityEngine;
using EntityStates;
using static RiskOfBulletstorm.Items.NuBulletstormExtraStatsController;

namespace RiskOfBulletstorm.Bandit2
{
    public class FireShotgunRBS : EntityStates.Bandit2.Weapon.FireShotgun2
    {
		RBSExtraStatsController statsController;

        public override void OnEnter()
        {
            base.OnEnter();
			statsController = base.characterBody.masterObject.GetComponent<RBSExtraStatsController>();
			if (!statsController)
				BulletstormPlugin._logger.LogError("Missing ExtraStatsController!");
		}

        public override void FireBullet(Ray aimRay)
		{
			base.StartAimMode(aimRay, 3f, false);
			this.DoFireEffects();
			this.PlayFireAnimation();
			base.AddRecoil(-1f * this.recoilAmplitudeY, -1.5f * this.recoilAmplitudeY, -1f * this.recoilAmplitudeX, 1f * this.recoilAmplitudeX);
			if (base.isAuthority)
			{
				Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
				Vector3 axis = Vector3.Cross(aimRay.direction, rhs);
				float num = 0f;
				if (base.characterBody)
				{
					num = base.characterBody.spreadBloomAngle;
				}
				float angle = 0f;
				float num2 = 0f;
				if (this.bulletCount > 1)
				{
					var accuracy = statsController.bulletAccuracy;
					num2 = UnityEngine.Random.Range(SimpleSpread(accuracy, this.minFixedSpreadYaw + num),
						SimpleSpread(accuracy, this.maxFixedSpreadYaw + num)) * 2f;
					angle = num2 / (float)(this.bulletCount - 1);
				}
				Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
				Quaternion rotation = Quaternion.AngleAxis(angle, axis);
				Ray aimRay2 = new Ray(aimRay.origin, direction);
				for (int i = 0; i < this.bulletCount; i++)
				{
					BulletAttack bulletAttack = base.GenerateBulletAttack(aimRay2);
					this.ModifyBullet(bulletAttack);
					bulletAttack.Fire();
					aimRay2.direction = rotation * aimRay2.direction;
				}
			}
		}
	}
}
