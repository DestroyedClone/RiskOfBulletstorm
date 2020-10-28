using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using EntityStates;
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using static EntityStates.BaseState;

namespace RiskOfBulletstorm.Items
{
    public class Bomb : Equipment_V2<Bomb>
    {
        //TODO: USE CHEN's HEALTH LOSS CODE FOR FLOATS
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage? (Default: 0.6 = 60% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageDealt { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public float Cooldown_config { get; private set; } = 0.5f;

        public override string displayName => "Ticket";
        public override float cooldown { get; protected set; } = 2f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Use For Boom";

        protected override string GetDescString(string langid = null) => $"Throws a bomb that explodes after a short delay, dealing {Pct(DamageDealt)} damage.";

        protected override string GetLoreString(string langID = null) => "Countless experienced adventurers have brought Bombs to the Gungeon seeking secret doors, only to be foiled by the existence of Blanks. Still, explosives have their place.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject BombPrefab { get; private set; }

        public override void SetupBehavior()
        {
            GameObject engiMinePrefab = Resources.Load<GameObject>("prefabs/projectiles/EngiGrenadeProjectile");
            BombPrefab = engiMinePrefab.InstantiateClone("RollBomb");
            //BombPrefab.GetComponent<ProjectileSimple>().velocity = 0; //default 50
            BombPrefab.GetComponent<ProjectileSimple>().lifetime = 6; //default 5
            BombPrefab.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Item;
            BombPrefab.GetComponent<ProjectileImpactExplosion>().lifetime = 6;
            //BombPrefab.GetComponent<ProjectileImpactExplosion>().destroyOnEnemy = false; //default True
            BombPrefab.GetComponent<ProjectileImpactExplosion>().timerAfterImpact = false;
            UnityEngine.Object.Destroy(BombPrefab.GetComponent<ApplyTorqueOnStart>());
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

            Vector3 corePos = Util.GetCorePosition(body);
            GameObject GameObject = slot.gameObject;
            var input = body.inputBank;

            Util.PlaySound(FireMines.throwMineSoundString, GameObject);
            if (NetworkServer.active)
            {
                ProjectileManager.instance.FireProjectile(BombPrefab, corePos, RoR2.Util.QuaternionSafeLookRotation(input.aimDirection),
                                      GameObject, body.damage * DamageDealt,
                                      0f, Util.CheckRoll(body.crit, body.master),
                                      DamageColorIndex.Item, null, -1f);
            }
            return true;
        }
    }
}
