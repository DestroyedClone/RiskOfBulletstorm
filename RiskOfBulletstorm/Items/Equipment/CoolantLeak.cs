using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using ThinkInvisible.ClassicItems;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class CoolantLeak : Equipment_V2<CoolantLeak>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius that Coolant Leak covers? (Default: 10m)", AutoConfigFlags.PreventNetMismatch)]
        public float CoolantLeak_Radius { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the duration that Coolant Leak's water pools last for? (Default: 10 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public float CoolantLeak_Duration { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Cooldown (Default: 14.00 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 14.00f;

        public override string displayName => "Coolant Leak";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Don't Overheat!\nSprays an area with liquid coolant. Useful for putting out fires or electrifying areas!";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public static GameObject WaterPoolPlacerPrefab { get; private set; }
        public static GameObject WaterPoolPrefab { get; private set; }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            GameObject sporeGrenadeDotZonePrefab = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectileDotZone");
            WaterPoolPrefab = sporeGrenadeDotZonePrefab.InstantiateClone("Bulletstorm_WaterPool");
            
            ProjectileDotZone projectileDotZone = WaterPoolPrefab.GetComponent<ProjectileDotZone>();
            projectileDotZone.attackerFiltering = AttackerFiltering.Default;
            projectileDotZone.lifetime = CoolantLeak_Duration;
            
            ProjectileDamage projectileDamage = WaterPoolPrefab.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Shock5s;
            projectileDamage.enabled = false;

            GameObject sporeGrenadePrefab = Resources.Load<GameObject>("prefabs/projectiles/SporeGrenadeProjectile");
            WaterPoolPlacerPrefab = sporeGrenadePrefab.InstantiateClone("Bulletstorm_WaterPoolPlacer");
            WaterPoolPlacerPrefab.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Item;
            ProjectileImpactExplosion projectileImpactExplosion = WaterPoolPlacerPrefab.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.childrenProjectilePrefab = WaterPoolPrefab;

            ProjectileCatalog.getAdditionalEntries += list => list.Add(WaterPoolPlacerPrefab);
            ProjectileCatalog.getAdditionalEntries += list => list.Add(WaterPoolPrefab);

            if (WaterPoolPlacerPrefab) PrefabAPI.RegisterNetworkPrefab(WaterPoolPlacerPrefab);
            if (WaterPoolPrefab) PrefabAPI.RegisterNetworkPrefab(WaterPoolPrefab);
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

            Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            FireBomb(body, gameObject);
            return true;
        }

        public void FireBomb(CharacterBody body, GameObject gameObject)
        {
            Vector3 corePos = Util.GetCorePosition(body);
            InputBankTest input = body.inputBank;

            if (NetworkServer.active)
            {
                ProjectileManager.instance.FireProjectile(WaterPoolPlacerPrefab, corePos, Util.QuaternionSafeLookRotation(input.aimDirection),
                                      gameObject, 0f,
                                      0f, false,
                                      DamageColorIndex.Item, null, -1f);
            }
        }


    }
}
