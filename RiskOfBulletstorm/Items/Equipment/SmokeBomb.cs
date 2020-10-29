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
        public string descText = "Temporary invisibility upon use";
        public override float cooldown { get; protected set; } = 36f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Vanish!\n"+descText;

        protected override string GetDescString(string langid = null) => $"{descText} for {CloakDuration} seconds.";

        protected override string GetLoreString(string langID = null) => "Early Gunpanese history saw the use of a rudimentary form of the smoke bomb." +
            "Explosives were common in Gunpan during the Sworden invasions of the 333th century. Soft cased hand-held bombs were later designed to release smoke, poison gas and shrapnel made from iron and pottery." +
            "Vintage smoke bombs were created in 2848, by Order of the Gungeon Capdom inventor Rollbert Smokes-a-lot. He developed early Calibernese-style fireworks and later modified the formula to produce more smoke for a longer period of time.";

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
