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
using static RiskOfBulletstorm.Shared.HelperUtil;

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
        [AutoConfig("Destroy the scans on teleporter start? (Default: false)", AutoConfigFlags.PreventNetMismatch)]
        public bool CartographerRing_DestroyOnTeleporterStart { get; private set; } = true;
        public override string displayName => "Cartographer's Ring";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Some Floors Are Familiar\nSometimes reveals the floor.";
        protected override string GetDescString(string langid = null) => $"<style=cIsUtility>{Pct(CartographerRing_ScanChance)} chance of revealing all interactables upon stage start.</style>" +
            $"\n <style=cStack>(+{Pct(CartographerRing_ScanChanceStack)} per stack)</style>." +
            $"\n <style=cSub>Chance is shared amongst players.</style>";

        protected override string GetLoreString(string langID = null) => "The Gungeon is unmappable, but it was not always so. It is said that in his youth, the great cartographer Woban has created four great maps, one for each floor of the Gungeon. While working on the fifth and final map, the walls suddenly began to shift strangely; they continue to do so to this day.";

        //public static GameObject PermanentScannerPrefab; // the survivor body prefab
        public static GameObject PermanentPoiPrefab;
        public static Stage currentStage;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            /*PermanentScannerPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/NetworkedObjects/ChestScanner"), "Bulletstorm_ChestScanner");
            ChestRevealer chestRevealer = PermanentScannerPrefab.GetComponent<ChestRevealer>();
            chestRevealer.radius = 1000;
            chestRevealer.pulseTravelSpeed = 1000;
            chestRevealer.revealDuration = 99999;
            chestRevealer.pulseEffectScale = 0;*/

            PermanentPoiPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/PositionIndicators/PoiPositionIndicator"), "Bulletstorm_PoiPositionIndicator");
            //var RevealedObject = PermanentPoiPrefab.Comop

            /*DestroyOnTimer destroyOnTimer = PermanentScannerPrefab.GetComponent<DestroyOnTimer>();
            if (CartographerRing_ScanDuration <= 0)
            {
                //UnityEngine.Object.Destroy(PermanentScannerPrefab.GetComponent<DestroyOnTimer>());
                destroyOnTimer.duration = 9999999;
            }
            else
            {
                destroyOnTimer.duration = CartographerRing_ScanDuration;
            }*/
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
            On.RoR2.PurchaseInteraction.Awake += RevealChest;
            Stage.onServerStageComplete += StageEnd_DestroyComponent;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            if (CartographerRing_DestroyOnTeleporterStart)
                TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterCharged_DestroyComponent;
        }

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            orig(self, activator);
            if (!currentStage) return;

            var component = currentStage.GetComponent<BulletstormRevealChests>();
            if (component)
            {
                var purchaseInteractions = component.purchaseInteractions;
                var positionIndicators = component.positionIndicators;
                for (int i = 0; i < purchaseInteractions.Count; i++)
                {
                    if (purchaseInteractions[i] == self)
                    {
                        purchaseInteractions.Remove(self);
                        positionIndicators.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            Stage.onStageStartGlobal -= Stage_onStageStartGlobal;
            On.RoR2.PurchaseInteraction.Awake -= RevealChest;
            Stage.onServerStageComplete -= StageEnd_DestroyComponent;
            On.RoR2.PurchaseInteraction.OnInteractionBegin -= PurchaseInteraction_OnInteractionBegin;
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
            if (currentStage)
            {
                BulletstormRevealChests component = currentStage.GetComponent<BulletstormRevealChests>();
                if (component)
                {
                    var positionIndicators = component.positionIndicators;
                    foreach (PositionIndicator obj in positionIndicators)
                    {
                        UnityEngine.Object.Destroy(obj);
                    }
                    UnityEngine.Object.Destroy(component);
                }
            }
        }

        private void RevealChest(On.RoR2.PurchaseInteraction.orig_Awake orig, PurchaseInteraction self)
        {
            orig(self);
            var gameObject = self.gameObject;
            if (!gameObject) return;
            var transform = gameObject.transform;

            if (self.available)
            {
                GameObject prefab = UnityEngine.Object.Instantiate(PermanentPoiPrefab);
                PositionIndicator positionIndicator = prefab.GetComponent<PositionIndicator>();
                positionIndicator.targetTransform = transform;
                currentStage.GetComponent<BulletstormRevealChests>()?.positionIndicators.Add(positionIndicator);
                currentStage.GetComponent<BulletstormRevealChests>()?.purchaseInteractions.Add(self);
            }
        }

        //ty Ghor for the hook
        private void Stage_onStageStartGlobal(Stage obj)
        {
            int InventoryCount = GetPlayersItemCount(catalogIndex);
            var gameObject = obj.gameObject;
            currentStage = obj;

            if (InventoryCount > 0)
            {
                var ResultChance = CartographerRing_ScanChance + CartographerRing_ScanChanceStack * (InventoryCount - 1);
                if (Util.CheckRoll(ResultChance))
                {
                    if (!gameObject.GetComponent<BulletstormRevealChests>()) gameObject.AddComponent<BulletstormRevealChests>();
                    //NetworkServer.Spawn(UnityEngine.Object.Instantiate(PermanentScannerPrefab));
                }
            }
        }

        public class BulletstormRevealChests : MonoBehaviour
        {
            public List<PositionIndicator> positionIndicators;
            public List<PurchaseInteraction> purchaseInteractions;
        }
        public class BulletstormChestRevealed : MonoBehaviour
        {

        }
    }
}
