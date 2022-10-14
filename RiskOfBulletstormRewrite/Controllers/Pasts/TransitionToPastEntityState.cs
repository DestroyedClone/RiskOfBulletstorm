using EntityStates;
using RoR2;
using System;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Controllers.Pasts
{
    public class TransitionToPastEntityState : BaseState
	{
		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (NetworkServer.active && fixedAge >= EntityStates.Interactables.MSObelisk.TransitionToNextStage.duration)
			{
				Stage.instance.BeginAdvanceStage(SceneCatalog.GetSceneDefFromSceneName(sceneNameForSceneDef));
				outer.SetNextState(new Idle());
			}
		}

		public static float duration;

		public string sceneNameForSceneDef = "golemplains";
	}
}
