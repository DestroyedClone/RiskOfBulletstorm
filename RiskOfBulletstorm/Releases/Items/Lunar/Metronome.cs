using RiskOfBulletstorm.Utils;
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
        [AutoConfig("Max kills? Default: 75", AutoConfigFlags.PreventNetMismatch)]
        public static int Metronome_MaxKills { get; private set; } = 75;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Additional max kills per stack? Default: 25", AutoConfigFlags.PreventNetMismatch)]
        public static int Metronome_MaxKillsStack { get; private set; } = 25;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Kills lost upon using a different ability, Default: 25", AutoConfigFlags.PreventNetMismatch)]
        public static int Metronome_KillsLost { get; private set; } = 25;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage Multiplier. Default: 2%", AutoConfigFlags.PreventNetMismatch)]
        public static float Metronome_DmgCoeff { get; private set; } = 0.02f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
       // [AutoConfig("Show stacks of metronome as a buff on screen? Default: true", AutoConfigFlags.PreventNetMismatch)]
        //public static bool Metronome_ShowAsBuff { get; private set; } = true;

        public override string displayName => "Metronome";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Better And Better\nImproves your damage with sequential kills, <style=cDeath>but loses some upon another skill.</style>";

        protected override string GetDescString(string langid = null) => $"Improves your damage by <style=cIsDamage>{Pct(Metronome_DmgCoeff)} per kill</style> with the same skill." +
            $"\n <style=cStack>Max Kills: {Metronome_MaxKills} + {Metronome_MaxKillsStack} per stack.</style>" +
            $"\n <style=cDeath>Using a different skill will reset your bonus by {Metronome_MaxKillsStack}</style>";

        protected override string GetLoreString(string langID = null) => "Tick, tick, tick, tick...." +
            "\n The metronome struck back and forth, and with every strike, a bullet ended a life." +
            "\n The sharpshooter grew stronger and stronger with each kill, his rifle shredding through skin, clothing, armor, even bullet-proof scales." +
            "\n He always had his machete at his hip, incase anyone got in too close. He knew if he struck, he would strike with the force of a thousand bullets." +
            "\n But he was wrong." +
            "\n An enemy came in close. Too close for his sniper. So he struck, not with the force of many, but with the force of one." +
            "\n And it only took one strike to end him." +
            "\n Tick, Tick, tick, tick";

        public static BuffIndex MetronomeBuffTally { get; private set; }

        public Metronome()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Metronome.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/MetronomeIcon.png";
        }

        public override void SetupBehavior()
        {

        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            var metronomeBuffTallyDef = new CustomBuff(
            new BuffDef
            {
                buffColor = Color.cyan,
                canStack = true,
                isDebuff = false,
                name = "Metronome Stacks",
            });
            MetronomeBuffTally = BuffAPI.Add(metronomeBuffTallyDef);

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
            CharacterBody vBody = self.characterBody;
            if (vBody)
            {
                int invCount = vBody.inventory.GetItemCount(catalogIndex);
                SkillLocator skillLocation = vBody.skillLocator;
                if (skillLocation)
                {
                    MetronomeTrackKills MetronomeTrackKills = self.gameObject.GetComponent<MetronomeTrackKills>();
                    if (MetronomeTrackKills)
                    {
                        if (skillLocation.FindSkill(self.skillName)) //Updates last skill slot used
                        {
                            if (invCount > 0)
                            {
                                if (skillLocation.primary.Equals(self)) { MetronomeTrackKills.SetLastSkillSlot(0); }
                                else if (skillLocation.secondary.Equals(self)) { MetronomeTrackKills.SetLastSkillSlot(1); }
                                else if (skillLocation.utility.Equals(self)) { MetronomeTrackKills.SetLastSkillSlot(2); }
                                else if (skillLocation.special.Equals(self)) { MetronomeTrackKills.SetLastSkillSlot(3); }
                                //else { Debug.LogError("Metronome: Invalid Skill Slot accessed!", self); }
                            }
                        }
                    }
                }
            }
            orig(self);
        }
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) //Update Max Kills
        {
            var InventoryCount = GetCount(self);
            MetronomeTrackKills MetronomeTrackKills = self.gameObject.GetComponent<MetronomeTrackKills>();
            if (InventoryCount > 0)
            {
                if (!MetronomeTrackKills) { MetronomeTrackKills = self.gameObject.AddComponent<MetronomeTrackKills>(); }
                MetronomeTrackKills.maxkills = Metronome_MaxKills + Metronome_MaxKillsStack * InventoryCount;

                MetronomeTrackKills.UpdateKills();
            } else
            {
                if (MetronomeTrackKills) { UnityEngine.Object.Destroy(MetronomeTrackKills); }
            }
            orig(self);
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            var attackerBody = damageReport?.attackerBody;
            int inventoryCount = GetCount(attackerBody);
            if (attackerBody)
            {
                if (inventoryCount > 0)
                {
                    var componentExists = attackerBody.GetComponent<MetronomeTrackKills>();
                    if (componentExists.IsKillsLessThanMax())
                    {
                        componentExists.IncrementKills();
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
                args.damageMultAdd += Metronome_DmgCoeff * MetronomeTrackKills.kills;
            }
        }

        public class MetronomeTrackKills : MonoBehaviour
        {
            public int kills = 0;
            public int maxkills = 16;
            public int LastSkillSlotUsed = 0;
            public CharacterBody characterBody;

            public void OnEnable()
            {
                var cb = gameObject.GetComponent<CharacterBody>();
                if (cb)
                    characterBody = cb;
                else
                    Destroy(gameObject.GetComponent<MetronomeTrackKills>());
            }

            public void UpdateKills()
            {
                var InventoryCount = characterBody.inventory.GetItemCount(Metronome.instance.catalogIndex);
                maxkills = Metronome_MaxKills + Metronome_MaxKillsStack * InventoryCount;
                if (kills > maxkills) kills = maxkills; //this resets it if you have less metronomes from like cleansing
                UpdateBuffStack();
            }
            public bool IsKillsLessThanMax()
            {
                return kills < maxkills;
            }
            public void UpdateBuffStack()
            {
                HelperUtil.ClearBuffStacks(characterBody, MetronomeBuffTally);
                HelperUtil.AddBuffStacks(characterBody, MetronomeBuffTally, kills);
            }
            public void SetLastSkillSlot(int SlotNumber)
            {
                if (LastSkillSlotUsed != SlotNumber)
                {
                    LastSkillSlotUsed = SlotNumber;
                    ReduceStacks();
                }
            }
            public void ReduceStacks()
            {
                kills = Math.Max(0, kills - Metronome_KillsLost);
            }
            public void IncrementKills()
            {
                kills++;
            }

        }
    }
}
