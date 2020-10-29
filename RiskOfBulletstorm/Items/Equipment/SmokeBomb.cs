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
    public class SmokeBomb : Equipment_V2<SmokeBomb>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Duration Default: 6 seconds", AutoConfigFlags.PreventNetMismatch)]
        public float CloakDuration { get; private set; } = 6f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        //public float Cooldown_config { get; private set; } = 8f;

        public override string displayName => "Smoke Bomb";
        public override float cooldown { get; protected set; } = 36f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Vanish!";

        protected override string GetDescString(string langid = null) => $"Temporary Invisible for {CloakDuration} seconds.";

        protected override string GetLoreString(string langID = null) => "REEEEEEEEEEEE I DONT HAVE A LORE";

        public static GameObject BombPrefab { get; private set; }

        public override void SetupBehavior()
        {

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

            GameObject GameObject = slot.gameObject;

            Util.PlaySound(FireMines.throwMineSoundString, GameObject);

            body.AddTimedBuffAuthority(BuffIndex.Cloak, CloakDuration);

            return true;
        }
    }
}
