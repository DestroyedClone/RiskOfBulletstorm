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
        [AutoConfig("What is the radius of the Magazine Rack's zone? (Default: 10.0 meters)", AutoConfigFlags.PreventNetMismatch)]
        public float MagazineRack_Radius { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the duration of the Magazine Rack's zone? (Default: 7.00 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public float MagazineRack_Duration { get; private set; } = 7.00f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the max amount of Magazine Racks that can be spawned per person? (Default: 1 per person)", AutoConfigFlags.PreventNetMismatch)]
        public int MagazineRack_MaxObjectsPerPerson { get; private set; } = 1;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds? (Default: 90 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 90.00f;

        public override string displayName => "Magazine Rack";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Instant Mail Order\nPlace to create a zone of no cooldowns.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Place to create a zone of infinite ammo within a radius of {MagazineRack_Radius} meters";
            desc += $"that lasts {MagazineRack_Duration} seconds. Up to {MagazineRack_MaxObjectsPerPerson} racks out per person.";
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
            if (MagazineRack_Duration > 0)
            {
                buffWard.expires = true;
                buffWard.expireDuration = MagazineRack_Duration;
            }
            buffWard.buffType = BuffIndex.NoCooldowns;

            MagazinePrefab.AddComponent<Bulletstorm_MagazineKiller>();

            if (MagazinePrefab) PrefabAPI.RegisterNetworkPrefab(MagazinePrefab);

            if (ClassicItemsCompat.enabled)
                ClassicItemsCompat.RegisterEmbryo(catalogIndex);
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
            CharacterBody.onBodyStartGlobal += GiveTracker;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            CharacterBody.onBodyStartGlobal -= GiveTracker;
        }
        private void GiveTracker(CharacterBody obj)
        {
            if (NetworkServer.active)
            {
                var gameObject = obj.gameObject;
                if (gameObject)
                {
                    gameObject.AddComponent<Bulletstorm_MagazineTracker>();
                    Debug.Log("gave tracker");
                }
            }
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            GameObject gameObject = slot.gameObject;
            if (!gameObject || !body) return false;

            bool EmbryoProc = ClassicItemsCompat.enabled && ClassicItemsCompat.CheckEmbryoProc(instance, body);
            return PlaceWard(body, MagazinePrefab, EmbryoProc);
        }

        public bool PlaceWard(CharacterBody body, GameObject wardObject, bool embryoProc)
        {
            if (NetworkServer.active)
            {
                var MagazineTracker = body.GetComponent<Bulletstorm_MagazineTracker>();
                if (MagazineTracker && MagazineTracker.instances.Count < MagazineRack_MaxObjectsPerPerson)
                {
                    Debug.Log("1");
                    GameObject gameObject = UnityEngine.Object.Instantiate(wardObject, body.transform.position, Quaternion.identity);
                    Debug.Log("2");
                    gameObject.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                    Debug.Log("3");
                    gameObject.GetComponent<BuffWard>().Networkradius *= embryoProc ? 1.5f : 1f;
                    Debug.Log("4");
                    gameObject.GetComponent<BuffWard>().expireDuration *= embryoProc ? 1.2f: 1f;
                    Debug.Log("5");
                    body.GetComponent<Bulletstorm_MagazineTracker>().instances.Add(gameObject);
                    Debug.Log("6");
                    gameObject.GetComponent<Bulletstorm_MagazineKiller>().characterBody = body;
                    Debug.Log("7");
                    NetworkServer.Spawn(gameObject);
                    return true;
                }
            }
            return false;
        }

        public class Bulletstorm_MagazineTracker : MonoBehaviour
        {
            public List<GameObject> instances;
        }

        public class Bulletstorm_MagazineKiller : MonoBehaviour
        {
            public CharacterBody characterBody;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnDisable()
            {
                if (characterBody && characterBody.GetComponent<Bulletstorm_MagazineTracker>())
                    characterBody.GetComponent<Bulletstorm_MagazineTracker>().instances.Remove(gameObject);
            }
        }
    }
}
