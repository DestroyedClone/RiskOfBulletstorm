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
using JetBrains.Annotations;
using static RiskOfBulletstorm.Shared.HelperUtil;

namespace RiskOfBulletstorm.Items
{
    public class CartographerRing : Item_V2<CartographerRing>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance for stage to scan? (Default: 20%)", AutoConfigFlags.PreventNetMismatch)]
        public float CartographerRing_ScanChance { get; private set; } = 0.2f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Stack chance for stage to scan? (Default: 10%)", AutoConfigFlags.PreventNetMismatch)]
        public float CartographerRing_ScanChanceStack { get; private set; } = 0.1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Duration of scanner in seconds (Default: 0-Infinite.)", AutoConfigFlags.PreventNetMismatch)]
        public float CartographerRing_ScanDuration { get; private set; } = 0f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Destroy the scans on teleporter start? (Default: false)", AutoConfigFlags.PreventNetMismatch)]
        public bool CartographerRing_DestroyOnTeleporterStart { get; private set; } = true;
        public override string displayName => "Cartographer's Ring";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Some Floors Are Familiar\nSometimes reveals the floor.";
        protected override string GetDescString(string langid = null) => $"<style=cIsUtility>{Pct(CartographerRing_ScanChance)} chance of activating a Scanner upon stage start.</style>" +
            $"\n <style=cStack>(+{Pct(CartographerRing_ScanChanceStack)} per stack)</style>." +
            $"\n <style=cSub>Chance is shared amongst players.</style>";

        protected override string GetLoreString(string langID = null) => "The Gungeon is unmappable, but it was not always so. It is said that in his youth, the great cartographer Woban has created four great maps, one for each floor of the Gungeon. While working on the fifth and final map, the walls suddenly began to shift strangely; they continue to do so to this day.";

        public static GameObject PermanentScannerPrefab; // the survivor body prefab

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            PermanentScannerPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/NetworkedObjects/ChestScanner"), "ChestScannerPermanent");
            ChestRevealer chestRevealer = PermanentScannerPrefab.GetComponent<ChestRevealer>();
            chestRevealer.radius = 1000;
            chestRevealer.pulseTravelSpeed = 1000;
            chestRevealer.revealDuration = 99999;
            chestRevealer.pulseEffectScale = 0;

            DestroyOnTimer destroyOnTimer = PermanentScannerPrefab.GetComponent<DestroyOnTimer>();
            if (CartographerRing_ScanDuration <= 0)
            {
                //UnityEngine.Object.Destroy(PermanentScannerPrefab.GetComponent<DestroyOnTimer>());
                destroyOnTimer.duration = 9999999;
            }
            else
            {
                destroyOnTimer.duration = CartographerRing_ScanDuration;
            }

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
            Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            TeleporterInteraction.onTeleporterBeginChargingGlobal += DestroyScanner;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            Stage.onStageStartGlobal -= Stage_onStageStartGlobal;
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= DestroyScanner;
        }
        private void DestroyScanner(TeleporterInteraction obj)
        {
            if (CartographerRing_DestroyOnTeleporterStart)
            {
                UnityEngine.Object.Destroy(PermanentScannerPrefab);
            }
        }

        //suggested by Ghor
        private void Stage_onStageStartGlobal(Stage obj)
        {
            int InventoryCount = GetPlayersItemCount(catalogIndex);

            if (InventoryCount > 0)
            {
                var ResultChance = CartographerRing_ScanChance + CartographerRing_ScanChanceStack * (InventoryCount - 1);
                Debug.Log("CartRing: Scan Chance: " + ResultChance.ToString());
                if (Util.CheckRoll(ResultChance))
                {
                    Debug.Log("CartRing: Scan Success");
                    //NetworkServer.Spawn(UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/NetworkedObjects/ChestScanner"), Vector3.zero, Quaternion.identity));
                    NetworkServer.Spawn(PermanentScannerPrefab);
                }
            }
        }
    }
}
