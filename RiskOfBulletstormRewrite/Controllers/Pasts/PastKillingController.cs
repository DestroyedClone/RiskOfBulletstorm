using BepInEx.Configuration;
using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Controllers.Pasts
{
    public class PastKillingController : ControllerBase<PastKillingController>
    {
        public static GameEndingDef pastKilledEndingDef;

        public override void Init(ConfigFile config)
        {
            Hooks();
        }

        public override void Hooks()
        {
            On.EntityStates.Interactables.MSObelisk.EndingGame.DoFinalAction += HijackToPast;
            On.RoR2.GameEndingCatalog.Init += GameEndingCatalog_Init;
        }

        private void GameEndingCatalog_Init(On.RoR2.GameEndingCatalog.orig_Init orig)
        {
            orig();

            CreateEndingDef();
        }

        public void CreateEndingDef()
        {
            pastKilledEndingDef = ScriptableObject.CreateInstance<GameEndingDef>();
            pastKilledEndingDef.backgroundColor = Color.red;
            pastKilledEndingDef.cachedName = "RiskOfBulletstorm_PastKilledEnding";
            //pastKilledEndingDef.defaultKillerOverride;
            pastKilledEndingDef.endingTextToken = "GAME_RESULT_PASTKILL_WIN";
            pastKilledEndingDef.foregroundColor = Color.blue;
            pastKilledEndingDef.gameEndingIndex = (GameEndingIndex)GameEndingCatalog.endingCount;
            pastKilledEndingDef.gameOverControllerState = LegacyResourcesAPI.Load<GameEndingDef>("RoR2/Base/ClassicRun/LimboEnding").gameOverControllerState;
            pastKilledEndingDef.icon = Assets.NullSprite;
            pastKilledEndingDef.isWin = true;
            pastKilledEndingDef.lunarCoinReward = 100;
            pastKilledEndingDef.material = LegacyResourcesAPI.Load<Material>("RoR2/Base/AltarSkeleton/matAltarSkeleton");
            pastKilledEndingDef.showCredits = false;

            LanguageAPI.Add("GAME_RESULT_PASTKILL_WIN", "You have killed your past!");

            R2API.ContentAddition.AddGameEndingDef(pastKilledEndingDef);

            //GameEndingDef[] newDefs = new GameEndingDef[] { };
            //newDefs = GameEndingCatalog.gameEndingDefs;
            //ArrayUtils.ArrayAppend(ref newDefs, in pastKilledEndingDef);
            //GameEndingCatalog.SetGameEndingDefs(newDefs);
        }

        public struct PastInfo
        {
            public string sceneName;
            public bool clearInventory;
            public Dictionary<ItemDef, int> itemInfo;
            public EquipmentDef equipInfo;
        }

        private PastInfo GetPastInfoForCharacter()
        {
            return new PastInfo();
        }

        private void HijackToPast(On.EntityStates.Interactables.MSObelisk.EndingGame.orig_DoFinalAction orig, EntityStates.Interactables.MSObelisk.EndingGame self)
        {
            bool flag = false;
            var purchaseInteraction = self.GetComponent<PurchaseInteraction>();
            if (purchaseInteraction.lastActivator)
            {
                if (purchaseInteraction.lastActivator.GetComponent<CharacterBody>()?.inventory?.GetItemCount(Items.PastKillingBullet.instance.ItemDef) > 0)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                /*var nextScene = new TransitionToPastEntityState
                {
                    sceneNameForSceneDef = "golemplains"
                };
                self.outer.SetNextState(nextScene);*/
                Run.instance.BeginGameOver(pastKilledEndingDef);
                return;
            }
            orig(self);
        }
    }
}