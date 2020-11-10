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
        [AutoConfig("Max kills?", AutoConfigFlags.PreventNetMismatch)]
        public int MaxKills { get; private set; } = 75;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Additional max kills per stack?", AutoConfigFlags.PreventNetMismatch)]
        public int MaxKillsStack { get; private set; } = 25;

        public float dmgCoeff = 0.02f;

        public override string displayName => "Metronome";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Better And Better\nImproves your damage with sequential kills, but loses it upon another skill.";

        protected override string GetDescString(string langid = null) => $"Improves your damage by {Pct(dmgCoeff)} per kill with the same skill." +
            $"\n Max Kills: {MaxKills} + {MaxKillsStack} per stack." +
            $"\n Using a different skill will reset your bonus.";

        protected override string GetLoreString(string langID = null) => "Tick, tick, tick, tick...." +
            "\n The metronome struck back and forth, and with every strike, a bullet ended a life." +
            "\n The sharpshooter grew stronger and stronger with each kill, his sniper shredding through skin, clothing, armor, even bullet-proof scales." +
            "\n He always had his machete at his hip, incase anyone got in too close. He knew if he struck, he would strike with the force of a thousand bullets." +
            "\n But he was wrong." +
            "\n An enemy came in close. Too close for his sniper. So he struck, not with the force of many, but with the force of one." +
            "\n And it only took one strike to end him." +
            "\n Tick, Tick, tick, tick";

        int LastSkillSlotUsed = 0;

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
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            On.RoR2.GlobalEventManager.OnCharacterDeath -= GlobalEventManager_OnCharacterDeath;
            GetStatCoefficients -= Metronome_GetStatCoefficients;
            On.RoR2.GenericSkill.OnExecute -= GenericSkill_OnExecute;
        }
        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            var invCount = GetCount(self.characterBody);
            CharacterBody vBody = self.characterBody;
            SkillLocator skillLocation = vBody.skillLocator;
            MetronomeTrackKills MetronomeTrackKills = self.gameObject.GetComponent<MetronomeTrackKills>();

            void SetLastSkillSlot(int SlotNumber)
            {
                if (LastSkillSlotUsed != SlotNumber)
                {
                    LastSkillSlotUsed = SlotNumber;
                    MetronomeTrackKills.kills = 0;
                    Debug.Log("Metronome: Kills reset due to change to slot " + LastSkillSlotUsed.ToString(), self);
                }
            }

            if (skillLocation.FindSkill(self.skillName))
            {
                if (invCount > 0)
                {
                    if (skillLocation.primary.Equals(self))         { SetLastSkillSlot(0); } 
                    else if (skillLocation.secondary.Equals(self))  { SetLastSkillSlot(1); }
                    else if (skillLocation.utility.Equals(self))    { SetLastSkillSlot(2); }
                    else if (skillLocation.special.Equals(self))    { SetLastSkillSlot(3); } 
                    else { Debug.LogError("Metronome: Invalid Skill Slot accessed!", self); }
                }
            }
            orig(self);
        }
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            var InventoryCount = GetCount(self);
            MetronomeTrackKills MetronomeTrackKills = self.gameObject.GetComponent<MetronomeTrackKills>();
            if (InventoryCount > 0)
            {
                if (!MetronomeTrackKills) { MetronomeTrackKills = self.gameObject.AddComponent<MetronomeTrackKills>(); }
                MetronomeTrackKills.maxkills = MaxKills + MaxKillsStack * InventoryCount;

                var compMaxKills = MetronomeTrackKills.maxkills;
                if (MetronomeTrackKills.kills > compMaxKills) MetronomeTrackKills.kills = compMaxKills;
            } else
            {
                if (MetronomeTrackKills) { UnityEngine.Object.Destroy(MetronomeTrackKills); }
            }
            orig(self);
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            if (self.gameObject)
            {
                var attackerBody = damageReport?.attackerBody;
                int inventoryCount = GetCount(attackerBody);
                //var InventoryCount = attackerBody.inventory.GetItemCount(catalogIndex);
                if (attackerBody)
                {
                    if (inventoryCount > 0)
                    {
                        var componentExists = attackerBody.GetComponent<MetronomeTrackKills>();
                        if (componentExists?.kills < componentExists?.maxkills)
                        {
                            componentExists.kills += 1;
                            Debug.Log("Metronome: Kill Added to slot "+ LastSkillSlotUsed.ToString(), self);
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
                var DamageMultiplier = dmgCoeff * MetronomeTrackKills.kills;
                args.damageMultAdd += DamageMultiplier;
            }
        }

        public class MetronomeTrackKills : MonoBehaviour
        {
            public int kills;
            public int maxkills;
        }
    }
}
