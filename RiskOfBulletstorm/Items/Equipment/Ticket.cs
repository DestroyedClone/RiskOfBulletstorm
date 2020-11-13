using RoR2;
using UnityEngine;
using TILER2;


namespace RiskOfBulletstorm.Items
{
    public class Ticket : Equipment_V2<Ticket>
    {

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Additional Health Scaling? (Default: 1 = +100% health)", AutoConfigFlags.PreventNetMismatch)]
        //public float HealthBonus { get; private set; } = 1f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Additional Damage Scaling? (Default: 0.5 = +50% damage)", AutoConfigFlags.PreventNetMismatch)]
        //public float DamageBonus { get; private set; } = 0.5f;

        public override string displayName => "Ticket";
        public string descText = "Summons Gatling Gull (Clay Templar) as an ally.";
        public override float cooldown { get; protected set; } = 120f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Do You Have Yours?\n"+descText;

        protected override string GetDescString(string langid = null) => $"{descText}";

        protected override string GetLoreString(string langID = null) => "Gatling Gull respects martial prowess in Gungeoneers. Spend this ticket to bring in the big guns.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject ItemBodyModelPrefab;
        public Ticket()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Ticket.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/TicketIcon.png";
        }

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
