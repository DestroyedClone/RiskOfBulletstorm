using System.Collections.Generic;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class MagazineRack : Equipment_V2<MagazineRack>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius in meters of the Magazine Rack's zone?", AutoConfigFlags.PreventNetMismatch)]
        public float MagazineRack_Radius { get; private set; } = 4f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the duration of the Magazine Rack's zone?", AutoConfigFlags.PreventNetMismatch)]
        public float MagazineRack_Duration { get; private set; } = 2.50f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 90.00f;

        public override string displayName => "Magazine Rack";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Instant Mail Order\nPlace to create a zone of no cooldowns.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Place to create a zone of infinite ammo within a radius of {MagazineRack_Radius} meters";
            desc += $"that lasts {MagazineRack_Duration} seconds.";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "Often found in gungeon doctors´ offices, this rack displays magazines of all sorts. The clips contained within should prove useful, and plentiful, for even the most inaccurate of Gungeoneers.";

        public static GameObject MagazinePrefab { get; private set; }

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            GameObject warbannerPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard");
            MagazinePrefab = warbannerPrefab.InstantiateClone("Bulletstorm_MagazineRackObject");

            BuffWard buffWard = MagazinePrefab.GetComponent<BuffWard>();
            buffWard.Networkradius = MagazineRack_Radius;
            buffWard.radius = MagazineRack_Radius;
            buffWard.expires = true;
            buffWard.expireDuration = MagazineRack_Duration;
            buffWard.buffType = BuffIndex.NoCooldowns;


            if (MagazinePrefab) PrefabAPI.RegisterNetworkPrefab(MagazinePrefab);

            if (ClassicItemsCompat.enabled)
            {
                Debug.Log("MagazineRack: Classicitemscompat added");
                ClassicItemsCompat.RegisterEmbryo(catalogIndex);
            }
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
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            GameObject gameObject = slot.gameObject;
            if (!gameObject || !body) return false;

            bool EmbryoProc = ClassicItemsCompat.enabled && ClassicItemsCompat.CheckEmbryoProc(instance, body);
            Debug.Log("results: "+ ClassicItemsCompat.enabled+" and "+ ClassicItemsCompat.CheckEmbryoProc(instance, body));
            return PlaceWard(body, MagazinePrefab, EmbryoProc);
        }

        public bool PlaceWard(CharacterBody body, GameObject wardObject, bool embryoProc)
        {
            if (NetworkServer.active)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(wardObject, body.transform.position, Quaternion.identity);
                gameObject.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                gameObject.GetComponent<BuffWard>().Networkradius *= embryoProc ? 1.5f : 1f;
                gameObject.GetComponent<BuffWard>().expireDuration *= embryoProc ? 1.2f : 1f;
                NetworkServer.Spawn(gameObject);
                return true;
            }
            return false;
        }
    }
}
