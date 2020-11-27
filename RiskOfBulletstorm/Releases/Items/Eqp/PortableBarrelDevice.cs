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
        [AutoConfig("Heal%? (Default: 1.0 = 100% heal)", AutoConfigFlags.PreventNetMismatch)]
        public float PortableTableDevice_HealAmount { get; private set; } = 1f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        //public float Cooldown_config { get; private set; } = 8f;

        public override string displayName => "Portable Barrel Device";
        public override float cooldown { get; protected set; } = 90f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Know When To Fold 'Em\nAdvanced polymers reinforce this state-of-the-art ballistic bin.";

        protected override string GetDescString(string langid = null) => $"Places a barrel nearby.";

        protected override string GetLoreString(string langID = null) => "";

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
            iscBarrelNew.prefab = BarrelPrefab;
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
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
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;

            PlaceTable(body);
            if (instance.CheckEmbryoProc(body))
            {
                PlaceTable(body);
            }
            return true;
        }

        private void PlaceTable(CharacterBody characterBody)
        {
            iscBarrelNew.DoSpawn(characterBody.transform.position, characterBody.transform.rotation, new DirectorSpawnRequest(
                iscBarrelNew, placementRule, RoR2Application.rng)
            );
        }
    }
}
