using EntityStates.Treebot.Weapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;
using ThinkInvisible.ClassicItems;

namespace RiskOfBulletstorm.Items
{
    public class PortableBarrelDevice : Equipment_V2<PortableBarrelDevice>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How long should the barrel stick around after being spawned? Default: 16 seconds", AutoConfigFlags.PreventNetMismatch)]
        public static float PortableTableDevice_Lifetime { get; private set; } = 16;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How long should the barrel stick around after being interacted with? Default: 4 seconds", AutoConfigFlags.PreventNetMismatch)]
        public static float PortableTableDevice_UseLifetime { get; private set; } = 4f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many barrels should be allowed to spawn within the world? Default: 30, Set to -1 for infinite", AutoConfigFlags.PreventNetMismatch)]
        public static int PortableTableDevice_MaxBarrels { get; private set; } = 30;

        public override string displayName => "Portable Barrel Device";

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Cooldown (Default: 30 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 30f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Know When To Fold 'Em\nPlaces a barrel.";

        protected override string GetDescString(string langid = null) => $"Places a <style=cIsUtility>barrel</style> nearby.";

        protected override string GetLoreString(string langID = null) => "Advanced polymers reinforce this state-of-the-art ballistic bin.";

        private static InteractableSpawnCard iscBarrelNew;
        private static InteractableSpawnCard iscBarrel;
        private static GameObject BarrelPrefab;
        private static DirectorPlacementRule placementRule;

        public PortableBarrelDevice()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Barrel.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/PortableBarrelIcon.png";
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            Embryo_V2.instance.Compat_Register(catalogIndex);
            iscBarrel = (InteractableSpawnCard)Resources.Load<SpawnCard>("SpawnCards/InteractableSpawnCard/iscBarrel1");
            iscBarrelNew = Object.Instantiate(iscBarrel);
            BarrelPrefab = iscBarrelNew.prefab;
            BarrelPrefab = BarrelPrefab.InstantiateClone($"Bulletstorm_Barrel");
            BarrelInteraction barrelInteraction = BarrelPrefab.GetComponent<BarrelInteraction>();
            barrelInteraction.expReward = 0;
            barrelInteraction.goldReward = 0;
            BarrelDestroyOnInteraction barrelDestroyOnInteraction = BarrelPrefab.AddComponent<BarrelDestroyOnInteraction>();
            barrelDestroyOnInteraction.lifetime = PortableTableDevice_Lifetime;
            barrelDestroyOnInteraction.uselifetime = PortableTableDevice_UseLifetime;
            iscBarrelNew.prefab = BarrelPrefab;
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                maxDistance = 100f,
                minDistance = 20f,
                preventOverhead = true
            };
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.BarrelInteraction.OnInteractionBegin += DestroyBarrel;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.BarrelInteraction.OnInteractionBegin -= DestroyBarrel;
        }

        private void DestroyBarrel(On.RoR2.BarrelInteraction.orig_OnInteractionBegin orig, BarrelInteraction self, Interactor activator)
        {
            orig(self, activator);
            var component = self.gameObject.GetComponent<BarrelDestroyOnInteraction>();
            if (component)
            {
                component.used = true;
            }
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;

            if (PlaceTable(body))
            {
                if (instance.CheckEmbryoProc(body))
                {
                    PlaceTable(body);
                }
                return true;
            } else
            {
                return false;
            }
        }

        private bool PlaceTable(CharacterBody characterBody)
        {
            var maxBarrels = PortableTableDevice_MaxBarrels;
            var barrelAmt = Object.FindObjectsOfType<BarrelDestroyOnInteraction>().Length;

            if (barrelAmt < maxBarrels || maxBarrels == -1)
            {
                iscBarrelNew.DoSpawn(characterBody.transform.position, characterBody.transform.rotation, new DirectorSpawnRequest(
                    iscBarrelNew, placementRule, RoR2Application.rng));
                return true;
            }
            return false;
        }

        private class BarrelDestroyOnInteraction : MonoBehaviour
        {
            public float lifetime = 16;
            public float uselifetime = 4;
            public bool used = false;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
            private void FixedUpdate()
            {
                if (used) //shorten the lifetime to the use time.
                {
                    used = false;
                    if (lifetime > uselifetime)
                        lifetime = uselifetime;
                }

                lifetime -= Time.fixedDeltaTime;
                if (lifetime <= 0f)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
