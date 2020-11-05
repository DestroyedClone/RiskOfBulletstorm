//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;

namespace RiskOfBulletstorm.Items
{
    public class Metronome : Item_V2<Metronome>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Additional max kills per stack?", AutoConfigFlags.PreventNetMismatch)]
        public int MaxKillsStack { get; private set; } = 50;

        public override string displayName => "Metronome";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Better And Better";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "Tick, tick, tick, tick...." +
            "\n The metronome struck back and forth, and with every strike, a bullet ended a life." +
            "\n The sharpshooter grew stronger and stronger with each kill, his sniper shredding through skin, clothing, armor, even bullet-proof scales." +
            "\n He always had his machete at his hip, incase anyone got in too close. He knew if he struck, he would strike with the force of a thousand bullets." +
            "\n But he was wrong." +
            "\n An enemy came in close. Too close for his sniper. So he struck, not with the force of many, but with the force of one." +
            "\n And it only took one strike to end him." +
            "\n Tick, Tick, tick, tick";

        private int InventoryCount = 0;

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
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            GetStatCoefficients += Metronome_GetStatCoefficients;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            On.RoR2.GlobalEventManager.OnCharacterDeath -= GlobalEventManager_OnCharacterDeath;
            GetStatCoefficients -= Metronome_GetStatCoefficients;
        }
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            MetronomeTrackKills MetronomeTrackKills = self.gameObject.GetComponent<MetronomeTrackKills>();
            if (!MetronomeTrackKills) { MetronomeTrackKills = self.gameObject.AddComponent<MetronomeTrackKills>(); }
            InventoryCount = GetCount(self);
            MetronomeTrackKills.maxkills = 150 + MaxKillsStack * InventoryCount;
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            if (NetworkServer.active && self.gameObject)
            {
                int inventoryCount = GetCount(damageReport.attackerBody);
                if (damageReport?.attackerBody)
                {
                    if (inventoryCount > 0)
                    {
                        var componentExists = damageReport.attackerBody.GetComponent<MetronomeTrackKills>();
                        if (componentExists?.kills < componentExists?.maxkills)
                        {
                            componentExists.kills += 1;
                            Chat.AddMessage("Kill Added!");
                        }
                    }
                }
            }
            orig(self, damageReport);
        }

        private void Metronome_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            MetronomeTrackKills MetronomeTrackKills = sender.gameObject.GetComponent<MetronomeTrackKills>();
            if (MetronomeTrackKills)
            {
                var DamageMultiplier = 0.02 * MetronomeTrackKills.kills;
                args.damageMultAdd += (float)DamageMultiplier;
            }
        }

        public class MetronomeTrackKills : MonoBehaviour
        {
            public int kills;
            public int maxkills;
        }
    }
}
