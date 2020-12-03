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
using System.Linq;
using JetBrains.Annotations;
using static RiskOfBulletstorm.Utils.HelperUtil;

namespace RiskOfBulletstorm.Items
{
    public class CartographerRing : Item_V2<CartographerRing>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance for stage to scan? (Default: 20%)", AutoConfigFlags.PreventNetMismatch)]
        public float CartographerRing_ScanChance { get; private set; } = 20f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Stack chance for stage to scan? (Default: 10%)", AutoConfigFlags.PreventNetMismatch)]
        public float CartographerRing_ScanChanceStack { get; private set; } = 10f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Duration of scanner in seconds (Default: 0-Infinite.)", AutoConfigFlags.PreventNetMismatch)]
        public float CartographerRing_ScanDuration { get; private set; } = 0f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Continue to pulse scans after the stage starts? (Default: false)", AutoConfigFlags.PreventNetMismatch)]
        public bool CartographerRing_KeepScanningPastStart { get; private set; } = false;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Destroy the scans on teleporter start? (Default: false)", AutoConfigFlags.PreventNetMismatch)]
        public bool CartographerRing_DestroyOnTeleporterStart { get; private set; } = true;

        public override string displayName => "Cartographer's Ring";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Some Floors Are Familiar\nSometimes reveals the floor.";
        protected override string GetDescString(string langid = null) => $"<style=cIsUtility>{CartographerRing_ScanChance}% chance of revealing all interactables upon stage start.</style>" +
            $"\n <style=cStack>(+{CartographerRing_ScanChanceStack}% per stack)</style>." +
            $"\n <style=cSub>Chance is shared amongst players.</style>";

        protected override string GetLoreString(string langID = null) => "The Gungeon is unmappable, but it was not always so. It is said that in his youth, the great cartographer Woban has created four great maps, one for each floor of the Gungeon. While working on the fifth and final map, the walls suddenly began to shift strangely; they continue to do so to this day.";

        public static GameObject PermanentScannerPrefab;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            PermanentScannerPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/NetworkedObjects/ChestScanner"), "Bulletstorm_ChestScanner");
            ChestRevealer chestRevealer = PermanentScannerPrefab.GetComponent<ChestRevealer>();
            chestRevealer.radius = 1000;
            chestRevealer.pulseTravelSpeed = 1000;
            chestRevealer.revealDuration = 1;
            chestRevealer.pulseInterval = 10;
            chestRevealer.pulseEffectScale = 0;
            chestRevealer.pulseEffectPrefab = null; //light mode users

            if (CartographerRing_ScanDuration < 0)
            {
                chestRevealer.revealDuration = 99999; //~27 hours
            }
            else
            {
                chestRevealer.revealDuration = Mathf.Max(1, CartographerRing_ScanDuration);
            }

            DestroyOnTimer destroyOnTimer = PermanentScannerPrefab.GetComponent<DestroyOnTimer>();

            if (CartographerRing_KeepScanningPastStart)
            {
                UnityEngine.Object.Destroy(destroyOnTimer);
                //destroyOnTimer.duration = 99999;
            }
            else
            {
                destroyOnTimer.duration = CartographerRing_ScanDuration;
            }

            if (PermanentScannerPrefab) PrefabAPI.RegisterNetworkPrefab(PermanentScannerPrefab);
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
            Stage.onServerStageComplete += StageEnd_DestroyComponent;
            if (CartographerRing_DestroyOnTeleporterStart)
                TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterCharged_DestroyComponent;
        }


        public override void Uninstall()
        {
            base.Uninstall();
            Stage.onStageStartGlobal -= Stage_onStageStartGlobal;
            Stage.onServerStageComplete -= StageEnd_DestroyComponent;
            if (CartographerRing_DestroyOnTeleporterStart)
                TeleporterInteraction.onTeleporterBeginChargingGlobal -= TeleporterCharged_DestroyComponent;
        }

        private void TeleporterCharged_DestroyComponent(TeleporterInteraction obj)
        {
            DestroyIndicators();
        }
        private void StageEnd_DestroyComponent(Stage obj)
        {
            DestroyIndicators();
        }

        private void DestroyIndicators()
        {
            if (PermanentScannerPrefab)
            {
                //https://stackoverflow.com/questions/604831/collection-was-modified-enumeration-operation-may-not-execute
                foreach (var comp in ChestRevealer.RevealedObject.currentlyRevealedObjects.ToList())
                {
                    comp.Value.enabled = false;
                }
                UnityEngine.Object.Destroy(PermanentScannerPrefab);
            }
        }

        //ty Ghor for the hook
        private void Stage_onStageStartGlobal(Stage obj)
        {
            int InventoryCount = GetPlayersItemCount(catalogIndex);

            if (InventoryCount > 0)
            {
                var ResultChance = CartographerRing_ScanChance + CartographerRing_ScanChanceStack * (InventoryCount - 1);
                if (Util.CheckRoll(ResultChance))
                {
                    var clone = UnityEngine.Object.Instantiate(PermanentScannerPrefab);
                    //NetworkServer.Spawn(UnityEngine.Object.Instantiate(PermanentScannerPrefab));
                    NetworkServer.Spawn(clone);
                }
            }
        }

    }
}
