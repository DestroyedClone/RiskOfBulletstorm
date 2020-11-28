using System;
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
        [AutoConfig("Minimum damage required before counting as a hit (Default: 5)", AutoConfigFlags.PreventNetMismatch)]
        public int MasterRound_MinimumDamage { get; private set; } = 5;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow the player's own damage to count as a hit? Default: false", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_AllowSelfDamage { get; private set; } = false;

        public override string displayName => "Master Round";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Nth Chamber";

        protected override string GetDescString(string langid = null) => $"<style=cIsHealth>+150 health</style>";

        protected override string GetLoreString(string langID = null) => "Apocryphal texts recovered from cultists of the Order indicate that the Gun and the Bullet are linked somehow." +
            "\nAny who enter the Gungeon are doomed to remain, living countless lives in an effort to break the cycle." +
            "\nFew return from the deadly route that leads to the Forge. Yet fewer survive that venture into less-explored territory." +
            "\nA monument to the legendary hero greets all who challenge the Gungeon, though their identity has been lost to the ages." +
            "\nThe legendary hero felled the beast at the heart of the Gungeon with five rounds. According to the myth, the sixth remains unfired.";
        readonly string[] adjustedPickup =
        {
            "first",
            "second",
            "third",
            "fourth",
            "fifth",
            "endless"
        };

        readonly string[] adjustedDesc =
        {
            "rare",
            "potent",
            "exceptional",
            "extraordinary",
            "unfathomable",
            "unprecedented"
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
            component.currentHits++;
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
            foreach (var player in playerList)
            {
                var gameObject = player.body.gameObject;
                var MasterRoundComponent = gameObject.GetComponent<MasterRoundComponent>();
                if (!MasterRoundComponent) MasterRoundComponent = gameObject.AddComponent<MasterRoundComponent>();
                MasterRoundComponent.allowedHits = MasterRound_AllowedHits + StageCount;
                //SAFETY
                MasterRoundComponent.teleporterCharging = true;
            }
        }
        private void GenericNotification_SetItem(On.RoR2.UI.GenericNotification.orig_SetItem orig, GenericNotification self, ItemDef itemDef)
        {
            orig(self, itemDef);
            if (itemDef.itemIndex != catalogIndex) return;
            var StageCount = Run.instance.stageClearCount;
            if (StageCount > adjustedPickup.Length) StageCount = adjustedPickup.Length-1;
            string numberString = adjustedPickup[StageCount];
            string descString = adjustedDesc[StageCount];

            //https://www.dotnetperls.com/uppercase-first-letter
            string numberCapitalized = char.ToUpper(numberString[0]) + numberString.Substring(1);

            string output = numberCapitalized + " Chamber" +
                "\nThis " + descString + " artifact indicates mastery of the " + numberString + " chamber.";

            self.descriptionText.token = output;
        }

        private void Check(TeleporterInteraction teleporterInteraction)
        {
            bool success = true;

            var comps = UnityEngine.Object.FindObjectsOfType<MasterRoundComponent>();
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
                for (int i = 0; i < comps.Length; i++)
                {
                    PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(catalogIndex);
                    Vector3 pickupVelocity = new Vector3(360 / i, 100, 360 / i);
                    PickupDropletController.CreatePickupDroplet(pickupIndex, teleporterInteraction.transform.position, pickupVelocity);
                }
            }
            else
            {
                Chat.AddMessage("Players didn't survive the hits");
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
