using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using EntityStates;
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using EntityStates.Engi.Mine;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using static EntityStates.BaseState;

namespace RiskOfBulletstorm.Items
{
    public class ProximityMine : Equipment_V2<ProximityMine>
    {
        //TODO: USE CHEN's HEALTH LOSS CODE FOR FLOATS
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage? (Default: 1 = 100% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageDealt { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public float Cooldown_config { get; private set; } = 0.5f;

        public override string displayName => "Proximity Mine";
        public override float cooldown { get; protected set; } = 2f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Use To Place";

        protected override string GetDescString(string langid = null) => $"This mine activates when an enemy gets close for {Pct(DamageDealt)} damage.";

        protected override string GetLoreString(string langID = null) => "Appears to be a homage to similar proximity-based explosives from the 64th Tenthdo console game in which 7 men fight for the Platinum Eye.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject BombPrefab { get; private set; }

        public override void SetupBehavior()
        {
            GameObject engiMinePrefab = Resources.Load<GameObject>("prefabs/projectiles/EngiMine");
            BombPrefab = engiMinePrefab.InstantiateClone("ProximityMine");
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
            On.EntityStates.Engi.Mine.MineArmingWeak.FixedUpdate += On_ESMineArmingWeak;
            On.EntityStates.Engi.Mine.BaseMineArmingState.OnEnter += On_ESBaseMineArmingState;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.EntityStates.Engi.Mine.MineArmingWeak.FixedUpdate -= On_ESMineArmingWeak;
            On.EntityStates.Engi.Mine.BaseMineArmingState.OnEnter -= On_ESBaseMineArmingState;
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;

            GameObject gameObject = body.gameObject;
            Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            DropMine(body, gameObject);

            return true;
        }
        private Quaternion MineDropDirection()
        {
            return Util.QuaternionSafeLookRotation(
                new Vector3(0f, 0f, 0f)
            );
        }
        private void DropMine(CharacterBody userBody, GameObject userGameObject)
        {
            Vector3 corePos = Util.GetCorePosition(userBody);
            {
                ProjectileManager.instance.FireProjectile(BombPrefab, corePos, MineDropDirection(),
                                      userGameObject, userBody.damage * DamageDealt,
                                      0f, Util.CheckRoll(userBody.crit, userBody.master),
                                      DamageColorIndex.Item, null, -1f);
            }
        }
        private void On_ESMineArmingWeak(On.EntityStates.Engi.Mine.MineArmingWeak.orig_FixedUpdate orig, MineArmingWeak self)
        {
            if (self.outer.name != "InstantMine(Clone)") orig(self);
            else self.outer.SetNextState(new MineArmingFull());
        }
        private void On_ESBaseMineArmingState(On.EntityStates.Engi.Mine.BaseMineArmingState.orig_OnEnter orig, BaseMineArmingState self)
        {
            orig(self);
            if (self.outer.name == "InstantMine(Clone)")
            {
                if (self.forceScale != 1f) self.forceScale = 1f;
                if (self.damageScale != 1f) self.damageScale = 1f;
            }
        }
    }
}
