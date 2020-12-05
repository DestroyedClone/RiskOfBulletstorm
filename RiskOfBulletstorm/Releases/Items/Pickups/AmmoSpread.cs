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
        public float AmmoSpread_LifetimeBlinking { get; private set; } = 25f;
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
            Pickup_AmmoSpread.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;

            AmmoPickupSpread ammoPickupSpread = Pickup_AmmoSpread.AddComponent<AmmoPickupSpread>();
            ammoPickupSpread.teamIndex = TeamIndex.Player;

            UnityEngine.Object.Destroy(Pickup_AmmoSpread.GetComponent<VelocityRandomOnStart>());
            UnityEngine.Object.Destroy(Pickup_AmmoSpread.GetComponent<AmmoPickup>());


            //ProjectileCatalog.getAdditionalEntries += list => list.Add(ammoPickupPrefab);
            if (Pickup_AmmoSpread) PrefabAPI.RegisterNetworkPrefab(Pickup_AmmoSpread);
        }
        public override void SetupLate()
        {
            base.SetupLate();
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
            On.RoR2.PickupDropletController.CreatePickupDroplet += CreatePickup;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PickupDropletController.CreatePickupDroplet -= CreatePickup;
        }

        private void CreatePickup(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            if (pickupIndex == PickupCatalog.FindPickupIndex(catalogIndex)) //safety to prevent softlocks
            {
                SpawnAmmoPickup(position);
            }
            else
            {
                orig(pickupIndex, position, velocity);
            }
        }

        private void SpawnAmmoPickup(Vector3 sapPosition)
        {
            GameObject gameObject7 = UnityEngine.Object.Instantiate(Pickup_AmmoSpread, sapPosition, new Quaternion(0f, 0f, 0f, 0f));
            NetworkServer.Spawn(gameObject7);
        }

        public class AmmoPickupSpread : MonoBehaviour
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
            private void OnTriggerStay(Collider other)
            {
                if (NetworkServer.active && alive && TeamComponent.GetObjectTeam(other.gameObject) == teamIndex)
                {

                    SkillLocator skillLocatorOther = other.GetComponent<SkillLocator>();
                    if (skillLocatorOther)
                    {
                        ReadOnlyCollection<TeamComponent> teamComponents = TeamComponent.GetTeamMembers(teamIndex);

                        alive = false;

                        foreach (TeamComponent teamComponent in teamComponents)
                        {
                            CharacterBody body = teamComponent.body;
                            if (body)
                            {
                                var skillLocator = body.GetComponent<SkillLocator>();
                                if (skillLocator)
                                    skillLocator.ApplyAmmoPack();
                                var inventory = body.inventory;
                                if (inventory)
                                {
                                    inventory.RestockEquipmentCharges(0, 1);
                                    if (inventory.GetEquipmentSlotCount() > 1) inventory.RestockEquipmentCharges(1, 1); //MULT
                                }
                                EffectManager.SimpleEffect(pickupEffect, transform.position, Quaternion.identity, true);
                            }
                        }
                        Destroy(gameObject);
                    }
                }
            }

            public TeamIndex teamIndex;

            public GameObject pickupEffect = (GameObject)Resources.Load("prefabs/effects/AmmoPackPickupEffect");

            private bool alive = true;
        }
    }
}
