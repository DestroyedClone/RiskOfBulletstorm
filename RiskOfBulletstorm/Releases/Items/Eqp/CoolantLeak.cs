using RiskOfBulletstorm.Utils;
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using ThinkInvisible.ClassicItems;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class CoolantLeak : Equipment_V2<CoolantLeak>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage (Default: 100% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float Bomb_DamageDealt { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Cooldown (Default: 14.00 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 14.00f;

        public override string displayName => "Coolant Leak";
        //public override float cooldown { get; protected set; } = Cooldown_config;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Don't Overheat!\nSprays an area with liquid coolant. Useful for putting out fires or electrifying areas!";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public static GameObject BombPrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;

        public CoolantLeak()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Bomb.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/BombIcon.png";
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            GameObject commandoGrenadePrefab = Resources.Load<GameObject>("prefabs/projectiles/CommandoGrenadeProjectile");
            BombPrefab = commandoGrenadePrefab.InstantiateClone("Bulletstorm_CoolantLeak");
            BombPrefab.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            BombPrefab.GetComponent<ProjectileSimple>().velocity = 35; //default 50
            BombPrefab.GetComponent<ProjectileSimple>().lifetime = 5; //default 5
            BombPrefab.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Item;
            BombPrefab.GetComponent<ProjectileImpactExplosion>().falloffModel = BlastAttack.FalloffModel.None;
            Object.Destroy(BombPrefab.GetComponent<ApplyTorqueOnStart>());

            var model = Resources.Load<GameObject>(modelResourcePath);
            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();

            var controller = BombPrefab.GetComponent<ProjectileController>();
            controller.ghostPrefab = model;

            ProjectileCatalog.getAdditionalEntries += list => list.Add(BombPrefab);

            if (BombPrefab) PrefabAPI.RegisterNetworkPrefab(BombPrefab);

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
            GameObject gameObject = slot.gameObject;
            if (!gameObject || !body) return false;

            //int DeployCount = instance.CheckEmbryoProc(body) ? 2 : 1; //Embryo Check

            Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            FireBomb(body, gameObject);
            if (instance.CheckEmbryoProc(body)) FireBomb(body, gameObject, 1f);
            return true;
        }

        public void FireBomb(CharacterBody body, GameObject gameObject, float yOffset = 0)
        {
            Vector3 corePos = Util.GetCorePosition(body);
            Vector3 offset = new Vector3(0f, yOffset, 0f);
            InputBankTest input = body.inputBank;

            if (NetworkServer.active)
            {
                ProjectileManager.instance.FireProjectile(BombPrefab, corePos + offset, Util.QuaternionSafeLookRotation(input.aimDirection),
                                      gameObject, body.damage * Bomb_DamageDealt,
                                      0f, Util.CheckRoll(body.crit, body.master),
                                      DamageColorIndex.Item, null, -1f);
            }
        }
    }
}
