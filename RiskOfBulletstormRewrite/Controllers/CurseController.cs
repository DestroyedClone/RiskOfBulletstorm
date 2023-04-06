﻿using BepInEx.Configuration;
using RiskOfBulletstormRewrite.Items;
using RoR2;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class CurseController : ControllerBase<CurseController>
    {
        public ConfigEntry<int> cfgMaxCurse;
        public ConfigEntry<float> cfgCurseMonsterCreditMultiplier;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgMaxCurse = config.Bind(ConfigCategory, "Lord of the Jammed Curse Requirement", 10, "The amount of curse required for the Lord of the Jammed to spawn.");
            cfgCurseMonsterCreditMultiplier = config.Bind(ConfigCategory, "Additive Monster Credit Multiplier", 0.1f, "The additive multiplier to monster credits for the director per 1 curse. 0.1 = +10% credits");
        }

        public override void Hooks()
        {
            Inventory.onInventoryChangedGlobal += Inventory_onInventoryChangedGlobal;
            On.RoR2.CombatDirector.OnEnable += CombatDirector_OnEnable;
        }

        private void CombatDirector_OnEnable(On.RoR2.CombatDirector.orig_OnEnable orig, CombatDirector self)
        {
            orig(self);
            self.creditMultiplier *= 1 
                + cfgCurseMonsterCreditMultiplier.Value 
                * Util.GetItemCountForTeam(TeamIndex.Player, CurseTally.instance.ItemDef.itemIndex, false, true);
        }

        private void Inventory_onInventoryChangedGlobal(Inventory inventory)
        {
            if (NetworkServer.active) return;
            var curseCount = inventory.GetItemCount(Items.CurseTally.instance.ItemDef);
            if (curseCount >= cfgMaxCurse.Value)
            {
                var lotjcount = inventory.GetItemCount(Items.LordOfTheJammedItem.instance.ItemDef);
                if (lotjcount <= 0)
                {
                    inventory.GiveItem(Items.LordOfTheJammedItem.instance.ItemDef, 1);
                }
            }
        }
    }
}
