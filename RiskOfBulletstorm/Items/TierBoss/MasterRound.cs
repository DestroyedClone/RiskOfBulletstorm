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

        private bool hasBeenHit = false;
        private bool teleporterCharging = false;
        private int currentHits = 0;
        private readonly int allowedHits = 3;
        //private bool rebound = false;

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
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            GetStatCoefficients += MasterRound_GetStatCoefficients;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= OnTeleporterBeginCharging;
            TeleporterInteraction.onTeleporterChargedGlobal -= OnTeleporterCharged;
            On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
            GetStatCoefficients -= MasterRound_GetStatCoefficients;
        }

        private void OnTeleporterBeginCharging(TeleporterInteraction teleporterInteraction)
        {
            //get stage number and multiply the allowed hits

            hasBeenHit = false;
            teleporterCharging = true;
            Chat.AddMessage("MasterRound: Teleporter Started Charging");
        }

        private void OnTeleporterCharged(TeleporterInteraction teleporterInteraction)
        {
            Check(teleporterInteraction);
            teleporterCharging = false;
        }
        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (teleporterCharging)
            {
                if (!hasBeenHit)
                {
                    for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count; i++)
                    { //CharacterMaster.readOnlyInstancesList[i] is the player. }
                        var player = CharacterMaster.readOnlyInstancesList[i];
                        if (hasBeenHit) Chat.AddMessage("MasterRound OnDamageNotified: " + hasBeenHit.ToString() + "=Player was Hit!");
                        if (!victim) Chat.AddMessage("MasterRound OnDamageNotified: " + victim.ToString() + "=victim doesn't exist");
                        if (victim != player.gameObject) Chat.AddMessage("MasterRound OnDamageNotified: " + victim.ToString() + "=victim did not equal " + player.gameObject.ToString());
                        if (!hasBeenHit && victim && victim == player.gameObject && !damageInfo.rejected)
                        {
                            if (currentHits < allowedHits)
                            {
                                currentHits++;
                                Chat.AddMessage("MasterRound: " + victim.name + " got hit! Remaining Hits: "+ (allowedHits - currentHits).ToString());
                            }
                            else
                            {
                                hasBeenHit = true;
                                Chat.AddMessage("MasterRound: dequalified!");
                            }
                        }
                    }
                }
            }
        }

        private void Check(TeleporterInteraction teleporterInteraction)
        {
            if (!hasBeenHit)
            {
                for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count; i++)
                {

                    //body.GetComponent<CharacterBody>()?.inventory.GiveItem(catalogIndex);

                    var player = CharacterMaster.readOnlyInstancesList[i];
                    if (player)
                    {
                        var body = player.GetComponent<CharacterBody>();
                        if (body)
                        {
                            Chat.AddMessage(i.ToString() + "= Gave Master Round to " + player.name);
                            PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(catalogIndex);
                            Vector3 pickupVelocity = new Vector3(100 * i, 100, 100 * i);
                            PickupDropletController.CreatePickupDroplet(pickupIndex, teleporterInteraction.transform.position, pickupVelocity);
                        }
                    }
                }
            } else
            {
                Chat.AddMessage("Players didn't survive the hits");
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
