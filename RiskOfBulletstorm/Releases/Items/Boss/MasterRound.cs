﻿using System.Collections.ObjectModel;
using System.Linq;
using RoR2;
using RoR2.UI;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using static RiskOfBulletstorm.Utils.HelperUtil;

namespace RiskOfBulletstorm.Items
{
    public class MasterRoundNth : Item_V2<MasterRoundNth>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Max Health Increase? (Default: 150)", AutoConfigFlags.PreventNetMismatch)]
        public int MasterRound_MaxHealthAdd { get; private set; } = 150;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Permitted Hits: 3", AutoConfigFlags.PreventNetMismatch)]
        public static int MasterRound_AllowedHits { get; private set; } = 3;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Permitted Hits added to Max per stage: 1", AutoConfigFlags.PreventNetMismatch)]
        public static int MasterRound_AllowedHitsPerStage { get; private set; } = 1;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Minimum damage required before counting as a hit (Default: 5)", AutoConfigFlags.PreventNetMismatch)]
        public int MasterRound_MinimumDamage { get; private set; } = 5;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow the player's own damage to count as a hit? Default: false", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_AllowSelfDamage { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Show in chat the amount of hits allowed? Default: true", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_AnnounceMax { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Show who gets hit in chat? Default: false", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_ShowHitInChat { get; private set; } = false;

        public override string displayName => "Master Round";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Given to those who survive the boss fight without exceeding a certain amount of hits";

        protected override string GetDescString(string langid = null) => $"Increases maximum health by <style=cIsHealing>{MasterRound_MaxHealthAdd} health</style> <style=cStack>(+{MasterRound_MaxHealthAdd} per stack)</style>";

        protected override string GetLoreString(string langID = null) => "Apocryphal texts recovered from cultists of the Order indicate that the Gun and the Bullet are linked somehow." +
            "\nAny who enter the Gungeon are doomed to remain, living countless lives in an effort to break the cycle." +
            "\nFew return from the deadly route that leads to the Forge. Yet fewer survive that venture into less-explored territory." +
            "\nA monument to the legendary hero greets all who challenge the Gungeon, though their identity has been lost to the ages." +
            "\nThe legendary hero felled the beast at the heart of the Gungeon with five rounds. According to the myth, the sixth remains unfired.";

        readonly string[] adjustedDesc =
        {
            "questionable",
            "rare",
            "potent",
            "exceptional",
            "extraordinary",
            "unfathomable",
            "unprecedented"
        };

        readonly string[] bannedStages =
        {
            "mysteryspace",
            "limbo",
            "bazaar",
            "artifactworld",
            "goldshores",
            "arena"
        };

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
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal += TeleporterInteraction_onTeleporterChargedGlobal;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            GetStatCoefficients += MasterRoundNth_GetStatCoefficients;
            On.RoR2.UI.GenericNotification.SetItem += GenericNotification_SetItem;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal -= TeleporterInteraction_onTeleporterChargedGlobal;
            On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
            GetStatCoefficients -= MasterRoundNth_GetStatCoefficients;
            On.RoR2.UI.GenericNotification.SetItem -= GenericNotification_SetItem;
        }

        private void MasterRoundNth_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            var InventoryCount = GetCount(sender);
            if (InventoryCount > 0)
            {
                args.baseHealthAdd += MasterRound_MaxHealthAdd;
            }
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (!victim) return;
            var component = victim.gameObject.GetComponent<MasterRoundComponent>();
            if (!component) return;
            if (!MasterRound_AllowSelfDamage && damageInfo.attacker == victim) return;
            if (damageInfo.rejected || damageInfo.damage < MasterRound_MinimumDamage) return;
            if (!component.teleporterCharging) return;
            component.currentHits++;
            if (MasterRound_ShowHitInChat)
            {
                Chat.AddMessage("MasterRound: " + victim.name + " has " + component.currentHits + "/" + component.allowedHits);
            }
        }

        private void TeleporterInteraction_onTeleporterChargedGlobal(TeleporterInteraction obj)
        {
            var comps = UnityEngine.Object.FindObjectsOfType<MasterRoundComponent>();
            foreach (var component in comps)
            {
                component.teleporterCharging = false;
            }
            Check(obj);
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            var playerList = PlayerCharacterMasterController.instances;
            var StageCount = Run.instance.stageClearCount;
            var maxHits = MasterRound_AllowedHits + StageCount * MasterRound_AllowedHitsPerStage;
            foreach (var player in playerList)
            {
                var body = player.master.GetBody();
                if (body)
                {
                    var MasterRoundComponent = body.gameObject.GetComponent<MasterRoundComponent>();
                    if (!MasterRoundComponent) MasterRoundComponent = body.gameObject.AddComponent<MasterRoundComponent>();
                    MasterRoundComponent.allowedHits = maxHits;
                    MasterRoundComponent.teleporterCharging = true;
                }
            }
            if (MasterRound_AnnounceMax)
                Chat.AddMessage("MASTER ROUND: Max Hits: "+ maxHits);
        }
        private void GenericNotification_SetItem(On.RoR2.UI.GenericNotification.orig_SetItem orig, GenericNotification self, ItemDef itemDef)
        {
            orig(self, itemDef);
            if (itemDef.itemIndex != catalogIndex) return;
            var StageCount = Mathf.Max(Run.instance.stageClearCount + 1, 1);

            string numberString = NumbertoOrdinal(StageCount);
            string numberCapitalized = char.ToUpper(numberString[0]) + numberString.Substring(1);
            string descString = adjustedDesc[Mathf.Clamp(StageCount, 0, adjustedDesc.Length)];

            //https://www.dotnetperls.com/uppercase-first-letter

            string output = numberCapitalized + " Chamber" +
                "\nThis " + descString + " artifact indicates mastery of the " + numberString + " chamber.";
            if (bannedStages.Contains(SceneCatalog.mostRecentSceneDef.baseSceneName))
            {
                output = "huh? how did you...";
            }
            self.descriptionText.token = output;
        }

        private void Check(TeleporterInteraction teleporterInteraction)
        {
            bool success = true;

            var comps = UnityEngine.Object.FindObjectsOfType<MasterRoundComponent>();
            var rotvalue = 360 / comps.Length;
            foreach (var component in comps)
            {
                if (component.currentHits > component.allowedHits)
                {
                    success = false;
                    break;
                }
            }
            if (success)
            {
                for (int i = 1; i <= comps.Length; i++)
                {
                    var rotvaluemult = rotvalue * i;

                    PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(catalogIndex);
                    Vector3 pickupVelocity = new Vector3(rotvaluemult, 45, rotvaluemult);
                    var offset = Vector3.up * 2f;
                    PickupDropletController.CreatePickupDroplet(pickupIndex, teleporterInteraction.transform.position + offset, pickupVelocity);
                }
            }
        }

        public class MasterRoundComponent : MonoBehaviour
        {
            public bool teleporterCharging = false;
            public int currentHits = 0;
            public int allowedHits = 7;
        }
    }
}
