//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using static EntityStates.BaseState;


namespace RiskOfBulletstorm.Items
{
    public class Ticket : Equipment_V2<Ticket>
    {
        //TODO: USE CHEN's HEALTH LOSS CODE FOR FLOATS
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Additional Health Scaling? (Default: 1 = +100% health)", AutoConfigFlags.PreventNetMismatch)]
        public float HealthBonus { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Additional Damage Scaling? (Default: 0.5 = +50% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageBonus { get; private set; } = 0.5f;

        public override string displayName => "Ticket";
        public override float cooldown { get; protected set; } = 45f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Do You Have Yours?";

        protected override string GetDescString(string langid = null) => $"Summons Gatling Gull as an ally.";

        protected override string GetLoreString(string langID = null) => "Gatling Gull respects martial prowess in Gungeoneers. Spend this ticket to bring in the big guns.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject ItemBodyModelPrefab;

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

            //GameObject gameObject = body.gameObject;
            //Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            // SpawnGull(body);
            //CharacterMaster characterMaster = 
            new MasterSummon
            {
                masterPrefab = MasterCatalog.FindMasterPrefab("ClayBruiserMaster"),
                position = body.transform.position,
                rotation = body.transform.rotation,
                summonerBodyObject = body.gameObject,
                ignoreTeamMemberLimit = true,
                teamIndexOverride = new TeamIndex?(TeamIndex.Player)
            }.Perform();
            return true;
        }
    }
}
