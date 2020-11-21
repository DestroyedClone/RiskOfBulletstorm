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
                TeamComponent[] array2 = UnityEngine.Object.FindObjectsOfType<TeamComponent>();
                TeamIndex teamIndex = other.gameObject.GetComponent<TeamComponent>().teamIndex;
                //SkillLocator skillLocator = other.GetComponent<SkillLocator>();
                for (int i = 0; i < array2.Length; i++)
                {
                    if (array2[i].teamIndex == teamIndex)
                    {
                        //array2[i].GetComponent<CharacterBody>().AddTimedBuff(BuffIndex.TeamWarCry, 7f);
                        array2[i].GetComponent<SkillLocator>().ApplyAmmoPack();
                    }
                }
            }
            orig(self, other);
        }
        private void CreatePickup(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            var body = PlayerCharacterMasterController.instances[0].master.GetBody();
            
            if (pickupIndex == PickupCatalog.FindPickupIndex(catalogIndex)) //safety to prevent softlocks
            {
                pickupIndex = PickupCatalog.FindPickupIndex(ItemIndex.None);
                SpawnAmmoPickup(body.gameObject.transform.position);
                position = new Vector3(0, -9999, 0); //lol idk how to remove it
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
