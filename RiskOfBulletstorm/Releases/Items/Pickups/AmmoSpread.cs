﻿//using System;
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
    public class PickupAmmoSpread : Item_V2<PickupAmmoSpread>
    {
        public override string displayName => "Spread Ammo";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Restores everyone's cooldowns";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public static GameObject Pickup_AmmoSpread { get; private set; }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            GameObject ammoPickupPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/AmmoPack");
            Pickup_AmmoSpread = ammoPickupPrefab.InstantiateClone("AmmoSpread");
            Pickup_AmmoSpread.GetComponent<DestroyOnTimer>().duration = 30f;
            Pickup_AmmoSpread.GetComponent<BeginRapidlyActivatingAndDeactivating>().delayBeforeBeginningBlinking = 25f;
            Pickup_AmmoSpread.AddComponent<GiveAmmoToTeam>();

            UnityEngine.Object.Destroy(Pickup_AmmoSpread.GetComponent<VelocityRandomOnStart>());
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
            On.RoR2.AmmoPickup.OnTriggerStay += AmmoPickup_OnTriggerStay;
            On.RoR2.PickupDropletController.CreatePickupDroplet += CreatePickup;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.AmmoPickup.OnTriggerStay -= AmmoPickup_OnTriggerStay;
            On.RoR2.PickupDropletController.CreatePickupDroplet -= CreatePickup;
        }

        private void AmmoPickup_OnTriggerStay(On.RoR2.AmmoPickup.orig_OnTriggerStay orig, AmmoPickup self, Collider other)
        {
            GiveAmmoToTeam ammoToTeam = self.GetComponent<GiveAmmoToTeam>();
            if (ammoToTeam)
            {
                Chat.AddMessage("AmmoSpread: Player walked into");
                int AppliedPlayers = 0;
                TeamComponent[] array2 = UnityEngine.Object.FindObjectsOfType<TeamComponent>(); //gorag opus yoink
                //TeamIndex teamIndex = other.gameObject.GetComponent<TeamComponent>().teamIndex;
                //SkillLocator skillLocator = other.GetComponent<SkillLocator>();
                for (int i = 0; i < array2.Length; i++)
                {
                    if (array2[i].teamIndex == TeamIndex.Player)
                    {
                        //array2[i].GetComponent<CharacterBody>().AddTimedBuff(BuffIndex.TeamWarCry, 7f);
                        array2[i].GetComponent<CharacterBody>().GetComponent<SkillLocator>().ApplyAmmoPack();
                        Chat.AddMessage("AmmoSpread: "+ array2[i]+" applied!");
                        EffectManager.SimpleEffect(self.pickupEffect, self.transform.position, Quaternion.identity, true);
                    }
                }
                SkillLocator skillLocator = other.GetComponent<SkillLocator>();
                if (skillLocator)
                {
                    AppliedPlayers++;
                    skillLocator.ApplyAmmoPack();
                    Chat.AddMessage("AmmoSpread: Given to "+other);
                }
                if (AppliedPlayers > 0)
                {
                    UnityEngine.Object.Destroy(self.baseObject);
                }
            }
            orig(self, other);
        }
        private void CreatePickup(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            var body = PlayerCharacterMasterController.instances[0].master.GetBody();
            
            if (pickupIndex == PickupCatalog.FindPickupIndex(catalogIndex)) //safety to prevent softlocks
            {
                pickupIndex = PickupCatalog.FindPickupIndex(ItemIndex.Syringe);
                SpawnAmmoPickup(body.gameObject.transform.position);
            }

            orig(pickupIndex, position, velocity);
        }

        private void SpawnAmmoPickup(Vector3 sapPosition)
        {
            Pickup_AmmoSpread.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
            GameObject gameObject7 = UnityEngine.Object.Instantiate(Pickup_AmmoSpread, sapPosition, new Quaternion(0f, 0f, 0f, 0f));
            NetworkServer.Spawn(gameObject7);
        }

        public class GiveAmmoToTeam : MonoBehaviour
        {

        }
    }
}
