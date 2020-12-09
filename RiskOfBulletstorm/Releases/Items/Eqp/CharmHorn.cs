
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class CharmHorn : Equipment_V2<CharmHorn>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius of charmed enemies? (Default: 100m)", AutoConfigFlags.PreventNetMismatch)]
        public float CharmHorn_Radius { get; private set; } = 100f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the duration of charmed enemies? (Default: 10 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public float CharmHorn_Duration { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds? (Default: 85.00 seconds)", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 85.00f;

        public override string displayName => "Charm Horn";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "The Call Of Duty\nWhen blown, this horn will call those nearby to aid you.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Blows the horn to <style=cIsUtility>charm</style> enemies within <style=cIsUtility>{CharmHorn_Radius} meters</style> for {CharmHorn_Duration} seconds.";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "There are strange inconsistencies in the behavior of the Gundead. Originally thought to be heartless killing machines, they have been known to capture certain invaders for unknown purposes. Furthermore, evidence of a crude religion has been discovered. Perhaps, one day, they could be reasoned with?";

        public static GameObject CharmWardPrefab { get; private set; }
        public readonly BuffIndex charmIndex = GungeonBuffController.Charm;

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            GameObject warbannerPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard");
            CharmWardPrefab = warbannerPrefab.InstantiateClone("Bulletstorm_CharmHornWard");

            BuffWard buffWard = CharmWardPrefab.GetComponent<BuffWard>();
            //buffWard.Networkradius = CharmHorn_Radius;
            buffWard.expires = true;
            buffWard.expireDuration = 3f;
            buffWard.buffType = charmIndex;
            buffWard.floorWard = false;
            buffWard.invertTeamFilter = true;

            if (ClassicItemsCompat.enabled)
                ClassicItemsCompat.RegisterEmbryo(catalogIndex);
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
        public override void SetupLate()
        {
            base.SetupLate();

            BuffWard buffWard = CharmWardPrefab.GetComponent<BuffWard>();
            buffWard.buffType = charmIndex;

            if (CharmWardPrefab) PrefabAPI.RegisterNetworkPrefab(CharmWardPrefab);
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            GameObject gameObject = slot.gameObject;
            if (!gameObject || !body) return false;
            float multiplier = 1.0f;

            Util.PlaySound(FireMines.throwMineSoundString, gameObject);
            if (ClassicItemsCompat.enabled && ClassicItemsCompat.CheckEmbryoProc(instance, body))
                multiplier += 0.5f;
            CharmNearby(body, CharmHorn_Radius * multiplier);
            return true;
        }

        public void CharmNearby(CharacterBody body, float radius)
        {
            if (NetworkServer.active)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(CharmWardPrefab, body.transform.position, Quaternion.identity);
                gameObject.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                //gameObject.GetComponent<BuffWard>().Networkradius *= radius;
                NetworkServer.Spawn(gameObject);

            }
        }
    }
}
