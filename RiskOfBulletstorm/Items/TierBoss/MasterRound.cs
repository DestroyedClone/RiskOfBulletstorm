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
    public class MasterRound : Item_V2<MasterRound> //Change to equipment that gives cursed.
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Max Health Increase? (Default: 150)", AutoConfigFlags.PreventNetMismatch)]
        public float MaxHealthAdd { get; private set; } = 150f;

        public override string displayName => "Master Round";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Nth Chamber";

        protected override string GetDescString(string langid = null) => $"This rare artifact indicates mastery of the Nth chamber.";

        protected override string GetLoreString(string langID = null) => "";

        private bool hasBeenHit;
        private LocalUser LocalUser { get; set; }

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
            TeleporterInteraction.onTeleporterBeginChargingGlobal += OnTeleporterBeginCharging;
            TeleporterInteraction.onTeleporterChargedGlobal += OnTeleporterCharged;
            GlobalEventManager.onClientDamageNotified += OnClientDamageNotified;
            GetStatCoefficients += MasterRound_GetStatCoefficients;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= OnTeleporterBeginCharging;
            TeleporterInteraction.onTeleporterChargedGlobal -= OnTeleporterCharged;
            GlobalEventManager.onClientDamageNotified -= OnClientDamageNotified;
            GetStatCoefficients -= MasterRound_GetStatCoefficients;
        }

        private void OnTeleporterBeginCharging(TeleporterInteraction teleporterInteraction)
        {
            hasBeenHit = false;
        }

        private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
        {
            Check();
        }

        private void OnClientDamageNotified(DamageDealtMessage damageDealtMessage)
        {
            if (!hasBeenHit && damageDealtMessage.victim && damageDealtMessage.victim == LocalUser.cachedBodyObject)
            {
                hasBeenHit = true;
            }
        }

        private void Check()
        {
            if (LocalUser.cachedBody && LocalUser.cachedBody.healthComponent && LocalUser.cachedBody.healthComponent.alive && !hasBeenHit)
            {
                Chat.AddMessage("Player survived with no hits!");
                for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count; i++)
                {
                    CharacterMaster.readOnlyInstancesList[i].GetComponent<CharacterBody>()?.inventory.GiveItem(catalogIndex);
                    Chat.AddMessage("Gave item");
                }
            }
        }
        private void MasterRound_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            var InventoryCount = GetCount(sender);
            if (InventoryCount > 0)
            {
                args.baseHealthAdd = MaxHealthAdd * (InventoryCount - 1);
            }
        }
    }
}
