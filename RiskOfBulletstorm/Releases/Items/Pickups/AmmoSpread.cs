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
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Lifetime of Ammo Spread. Default: 30 seconds", AutoConfigFlags.PreventNetMismatch)]
        public float AmmoSpread_Lifetime { get; private set; } = 30f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("At how many seconds should it start blinking? Default: 25 seconds", AutoConfigFlags.PreventNetMismatch)]
        public float AmmoSpread_LifetimeBlinking { get; private set; } = 30f;
        public override string displayName => "Spread Ammo";
        public override ItemTier itemTier => ItemTier.NoTier;
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
            Pickup_AmmoSpread = ammoPickupPrefab.InstantiateClone("Bulletstorm_AmmoSpread");
            Pickup_AmmoSpread.GetComponent<DestroyOnTimer>().duration = 30f;
            Pickup_AmmoSpread.GetComponent<BeginRapidlyActivatingAndDeactivating>().delayBeforeBeginningBlinking = Math.Min(AmmoSpread_LifetimeBlinking, AmmoSpread_Lifetime);
            Pickup_AmmoSpread.AddComponent<GiveAmmoToTeam>();

            UnityEngine.Object.Destroy(Pickup_AmmoSpread.GetComponent<VelocityRandomOnStart>());

            ProjectileCatalog.getAdditionalEntries += list => list.Add(ammoPickupPrefab);
            if (ammoPickupPrefab) PrefabAPI.RegisterNetworkPrefab(ammoPickupPrefab);
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
            GiveAmmoToTeam ammoToTeam = self.gameObject.GetComponent<GiveAmmoToTeam>();
            if (ammoToTeam)
            {
                Chat.AddMessage("AmmoSpread: Player walked into");
                int AppliedPlayers = 0;
                //TeamComponent[] array2 = UnityEngine.Object.FindObjectsOfType<TeamComponent>(); //gorag opus yoink
                ReadOnlyCollection<TeamComponent> array2 = TeamComponent.GetTeamMembers(TeamIndex.Player);

                foreach (TeamComponent teamComponent in array2)
                {
                    CharacterBody body = teamComponent.body;
                    if (body)
                    {
                        body.GetComponent<SkillLocator>()?.ApplyAmmoPack();
                        body.inventory?.RestockEquipmentCharges(0, 1);
                        if (body.inventory?.GetEquipmentSlotCount() > 1) body.inventory?.RestockEquipmentCharges(1, 1); //MULT
                        Chat.AddMessage("AmmoSpread: " + body.GetUserName() + " applied!");
                        EffectManager.SimpleEffect(self.pickupEffect, self.transform.position, Quaternion.identity, true);
                        AppliedPlayers++;
                    }
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
                SpawnAmmoPickup(body.gameObject.transform.position);
            }
            else
            {
                orig(pickupIndex, position, velocity);
            }
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
