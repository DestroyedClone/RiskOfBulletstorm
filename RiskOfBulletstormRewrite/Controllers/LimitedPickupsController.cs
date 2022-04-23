using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using RoR2;
using RoR2.Items;
using RiskOfBulletstormRewrite;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Generic;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class LimitedPickupsController : ControllerBase<LimitedPickupsController>
	{
        public static List<PickupDef> limitedPickupDefs = new List<PickupDef>();
        public static BasicPickupDropTable dtDuplicator;

        public override void Init(ConfigFile config)
        {
            SetupConfig(config);
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            On.RoR2.PickupDropTable.RegenerateAll += PickupDropTable_RegenerateAll;
        }

        private void PickupDropTable_RegenerateAll(On.RoR2.PickupDropTable.orig_RegenerateAll orig, Run run)
        {
            orig(run);
            BanDuplicators();
        }

        public void BanDuplicators()
        {
            dtDuplicator = Utils.ItemHelpers.Load<BasicPickupDropTable>("RoR2/Base/DuplicatorMilitary/dtDuplicatorTier3.asset");
            foreach (var choice in limitedPickupDefs)
            {
                for (int i = 0; i < dtDuplicator.selector.choices.Length; i++)
                {
                    var choiceInfo = dtDuplicator.selector.choices[i];
                    if (choiceInfo.value == choice.pickupIndex)
                    {
                        choiceInfo.weight = 0;
                        break;
                    }
                }
            }
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody obj)
        {
            foreach (var pickupDef in limitedPickupDefs)
            {
                if (pickupDef.itemIndex != ItemIndex.None && obj.inventory.GetItemCount(pickupDef.itemIndex) > 0)
                {
                    Run.instance.DisableItemDrop(pickupDef.itemIndex);
                }
            }
        }

        private void Run_onRunStartGlobal(Run run)
        {

        }

        public static void SetupConfig(ConfigFile config)
        {

        }


    }
}
