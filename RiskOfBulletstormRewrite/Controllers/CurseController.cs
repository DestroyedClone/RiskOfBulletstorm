using RiskOfBulletstormRewrite.Characters.Enemies;
using RiskOfBulletstormRewrite.Items;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class CurseController : ControllerBase<CurseController>
    {
        public override string ConfigCategory => "Controller: Curse";
        public int cfgMaxCurse = 20;
        public float cfgCurseMonsterCreditMultiplier = 0.1f;

        public ItemDef LOTJItemDef => LordOfTheJammedTargetingItem.instance.ItemDef;
        public int teamwideCurseCount = 0;
        //https://github.com/Moffein/BazaarLimit/blob/21b1342309b51cdf560460fd0d11a6ee79ca4a7f/BazaarLimit/Class1.cs#L26
        private static readonly SceneDef bazaarSceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/bazaar/bazaar.asset").WaitForCompletion();

        public override void Hooks()
        {
            Inventory.onInventoryChangedGlobal += GiveLordOfTheJammedItemIfInventoryHasMaxCurse;
            Inventory.onInventoryChangedGlobal += UpdateTeamCurseCount;
            On.RoR2.CombatDirector.OnEnable += CombatDirector_OnEnable;
            On.EntityStates.NewtMonster.SpawnState.OnEnter += SpawnState_OnEnter;
        }

        private void UpdateTeamCurseCount(Inventory _)
        {
            teamwideCurseCount = Util.GetItemCountForTeam(TeamIndex.Player, CurseTally.instance.ItemDef.itemIndex, false, true);
        }

        private void CombatDirector_OnEnable(On.RoR2.CombatDirector.orig_OnEnable orig, CombatDirector self)
        {
            orig(self);
            self.creditMultiplier *= 1
                + cfgCurseMonsterCreditMultiplier
                * teamwideCurseCount;
        }

        private void GiveLordOfTheJammedItemIfInventoryHasMaxCurse(Inventory inventory)
        {
            if (!NetworkServer.active) return;
            var curseCount = inventory.GetItemCount(CurseTally.instance.ItemDef);
            if (curseCount < cfgMaxCurse) return;

            var lotjcount = inventory.GetItemCount(LordOfTheJammedTargetingItem.instance.ItemDef);
            if (lotjcount <= 0 && inventory.TryGetComponent(out CharacterMaster master))
            {
                inventory.GiveItem(LOTJItemDef, 1);
                GenericPickupController.SendPickupMessage(master, PickupCatalog.FindPickupIndex(LOTJItemDef.itemIndex));
            }
        }

        private void SpawnState_OnEnter(On.EntityStates.NewtMonster.SpawnState.orig_OnEnter orig, EntityStates.NewtMonster.SpawnState self)
        {
            if (NetworkServer.active && SceneCatalog.GetSceneDefForCurrentScene() == bazaarSceneDef)
            {
                if (!self.outer.GetComponent<LordofTheJammedMonster.NewtKickFromShopIfLOTJBehaviour>())
                {
                    self.outer.gameObject.AddComponent<LordofTheJammedMonster.NewtKickFromShopIfLOTJBehaviour>();
                }
            }
            orig(self);
        }
    }
}