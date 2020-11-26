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

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Barrier%? (Default: 0.5 = 50%% barrier)", AutoConfigFlags.PreventNetMismatch)]
        public float Medkit_BarrierAmount { get; private set; } = 0.5f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        //public float Cooldown_config { get; private set; } = 8f;

        public override string displayName => "Portable Barrel Device";
        public override float cooldown { get; protected set; } = 90f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Know When To Fold 'Em\nAdvanced polymers reinforce this state-of-the-art ballistic bin.";

        protected override string GetDescString(string langid = null) => $"Places a barrel nearby.";

        protected override string GetLoreString(string langID = null) => "";
        public PortableBarrelDevice()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Barrel.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/PortableBarrelIcon.png";
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            Embryo_V2.instance.Compat_Register(catalogIndex);
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
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;

            var BarrierAmt = health.fullBarrier * Medkit_BarrierAmount;

            
            if (instance.CheckEmbryoProc(body))
            {
                
            }
            return true;
        }

        private void PlaceTable()
        {
            isc.DoSpawn(args.senderBody.transform.position, new Quaternion(), new DirectorSpawnRequest(
                isc,
                new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                    maxDistance = 100f,
                    minDistance = 20f,
                    position = args.senderBody.transform.position,
                    preventOverhead = true
                },
                RoR2Application.rng)
            );
        }
    }
}
