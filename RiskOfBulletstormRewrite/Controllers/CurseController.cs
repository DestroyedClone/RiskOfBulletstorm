using BepInEx.Configuration;
using RiskOfBulletstormRewrite.Items;
using RoR2;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class CurseController : ControllerBase<CurseController>
    {
        public override string ConfigCategory => "Controller: Curse";
        public ConfigEntry<int> cfgMaxCurse;
        public ConfigEntry<float> cfgCurseMonsterCreditMultiplier;

        public ItemDef LOTJItemDef => LordOfTheJammedItem.instance.ItemDef;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgMaxCurse = config.Bind(ConfigCategory, "Lord of the Jammed Curse Requirement", 20, "The amount of curse required for the Lord of the Jammed to spawn.");
            cfgCurseMonsterCreditMultiplier = config.Bind(ConfigCategory, "Additive Monster Credit Multiplier", 0.1f, "The additive multiplier to monster credits for the director per 1 curse. 0.1 = +10% credits");
        }

        public override void Hooks()
        {
            Inventory.onInventoryChangedGlobal += GiveLordOfTheJammedItemIfInventoryHasMaxCurse;
            On.RoR2.CombatDirector.OnEnable += CombatDirector_OnEnable;
        }

        private void CombatDirector_OnEnable(On.RoR2.CombatDirector.orig_OnEnable orig, CombatDirector self)
        {
            orig(self);
            self.creditMultiplier *= 1
                + cfgCurseMonsterCreditMultiplier.Value
                * Util.GetItemCountForTeam(TeamIndex.Player, CurseTally.instance.ItemDef.itemIndex, false, true);
        }

        private void GiveLordOfTheJammedItemIfInventoryHasMaxCurse(Inventory inventory)
        {
            if (!NetworkServer.active) return;
            var curseCount = inventory.GetItemCount(CurseTally.instance.ItemDef);
            if (curseCount >= cfgMaxCurse.Value)
            {
                var lotjcount = inventory.GetItemCount(LordOfTheJammedItem.instance.ItemDef);
                if (lotjcount <= 0 && inventory.TryGetComponent(out CharacterMaster master))
                {
                    inventory.GiveItem(LOTJItemDef, 1);
                    CharacterMasterNotificationQueue.PushPickupNotification(master, PickupCatalog.FindPickupIndex(LOTJItemDef.itemIndex));
                }
            }
        }
    }
}