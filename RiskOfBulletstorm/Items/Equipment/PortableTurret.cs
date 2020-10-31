using RoR2;
using UnityEngine;
using TILER2;


namespace RiskOfBulletstorm.Items
{
    public class PortableTurret : Equipment_V2<PortableTurret>
    {

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Additional Health Scaling? (Default: 1 = +100% health)", AutoConfigFlags.PreventNetMismatch)]
        //public float HealthBonus { get; private set; } = 1f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Additional Damage Scaling? (Default: 0.5 = +50% damage)", AutoConfigFlags.PreventNetMismatch)]
        //public float DamageBonus { get; private set; } = 0.5f;

        public override string displayName => "Portable Turret";
        public string descText = "Shoots at enemies for a short time";
        public override float cooldown { get; protected set; } = 75f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Some Assembly Required\n" + descText;

        protected override string GetDescString(string langid = null) => $"{descText}";

        protected override string GetLoreString(string langID = null) => "This portable turret is actually a doll commonly given to young Gundead. It shoots at anything nearby, and is thought to encourage good habits.";

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
                masterPrefab = MasterCatalog.FindMasterPrefab("EngiTurretMaster"),
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
