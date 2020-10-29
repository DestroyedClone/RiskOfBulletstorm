/*
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
    public class Drill : Equipment_V2<Drill>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage? (Default: 0.6 = 60% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageDealt { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Cooldown? (Default: 8 = 8 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public float Cooldown_config { get; private set; } = 0.5f;

        public override string displayName => "Drill";
        public string descText = "Opens locked chests. Loud.";
        public override bool isLunar => true;
        public override float cooldown { get; protected set; } = 2f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Sawgeant\n"+descText;

        protected override string GetDescString(string langid = null) => $"Unlocks a chest <style=cIsDeath>...but spawns enemies depending on the chest's value.</style>";

        protected override string GetLoreString(string langID = null) => "A device once used to access the innermost chambers of ancient currency reliquaries. Cheaply constructed and prone to breaking. In ancient texts, the word \"drill\" is commonly preceeded [sic] or followed by expletives.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

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
*/