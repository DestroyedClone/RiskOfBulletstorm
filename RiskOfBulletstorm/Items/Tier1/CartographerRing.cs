//using System;
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
using System;

namespace RiskOfBulletstorm.Items
{
    public class CartographerRing : Item_V2<CartographerRing>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance for stage to scan? (Default: 20%)", AutoConfigFlags.PreventNetMismatch)]
        public float ScanChance { get; private set; } = 0.2f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Stack chance for stage to scan? (Default: 10%)", AutoConfigFlags.PreventNetMismatch)]
        public float ScanChanceStack { get; private set; } = 0.1f;
        public override string displayName => "Cartographer's Ring";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Some Floors Are Familiar\nSometimes reveals the floor.";

        protected override string GetDescString(string langid = null) => $"{Pct(ScanChance)} chance of activating a Scanner upon stage start." +
            $"\n (+{Pct(ScanChanceStack)} per stack)";

        protected override string GetLoreString(string langID = null) => "The Gungeon is unmappable, but it was not always so. It is said that in his youth, the great cartographer Woban has created four great maps, one for each floor of the Gungeon. While working on the fifth and final map, the walls suddenly began to shift strangely; they continue to do so to this day.";

        private int InventoryCount = 0;

        public override void SetupBehavior()
        {

        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.SceneDirector.PopulateScene += ScanStage;
            On.RoR2.CharacterBody.OnInventoryChanged += UpdateInvCount;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.SceneDirector.PopulateScene -= ScanStage;
            On.RoR2.CharacterBody.OnInventoryChanged -= UpdateInvCount;
        }

        //use Stage.onStageStartGlobal? (suggested by Ghor)
        private void ScanStage(On.RoR2.SceneDirector.orig_PopulateScene orig, SceneDirector self)
        {
            orig(self);
            Chat.AddMessage("CartRing: Entered Hook");
            if (InventoryCount > 0)
            {
                var ResultChance = ScanChance*100 + ScanChanceStack*100 * (InventoryCount - 1);
                Chat.AddMessage("CartRing: Scan Chance: "+ResultChance.ToString());
                if (Util.CheckRoll(ResultChance))
                {
                    Chat.AddMessage("CartRing: Scan Success?");
                    NetworkServer.Spawn(UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/ChestScanner"), Vector3.zero, Quaternion.identity));
                }
            }
        }
        private void UpdateInvCount(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            InventoryCount = GetCount(self);
            orig(self);
        }
    }
}
