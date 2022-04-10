using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class Metronome : ItemBase<Metronome>
    {
        public static ConfigEntry<int> cfgMaxKills;
        public static ConfigEntry<int> cfgMaxKillsStack;
        public static ConfigEntry<int> cfgKillsLostOnOtherSkillUse;
        public static ConfigEntry<float> cfgDamageCoefficientPerStack;

        public override string ItemName => "Metronome";

        public override string ItemLangTokenName => "METRONOME";

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("Assets/Models/Prefabs/Metronome.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("Assets/Textures/Icons/Metronome.png");

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Damage
        };

        public BuffDef MetronomeBuffTally;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgMaxKills = config.Bind(ConfigCategory, "Max Kills", 75, "What are the maximum amount of kills that can be counted by the Metronome?");
            cfgMaxKillsStack = config.Bind(ConfigCategory, "Max Kills Per Stack", 25, "How many additional max kills can be counted by the Metronome per stack?");
            cfgKillsLostOnOtherSkillUse = config.Bind(ConfigCategory, "Kills Lost On Other Skill Use", 75, "How many kills are lost upon using a different ability?");
            cfgDamageCoefficientPerStack = config.Bind(ConfigCategory, "Damage Multiplier per kill", 0.02f, "What is the damage multiplier per kill for the metronome?");

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            var InventoryCount = GetCount(self);
            MetronomeTrackKills metronomeTrackKills = self.gameObject.GetComponent<MetronomeTrackKills>();
            if (!metronomeTrackKills) { metronomeTrackKills = self.gameObject.AddComponent<MetronomeTrackKills>(); }
            metronomeTrackKills.characterBody = self;
            if (InventoryCount > 0)
            {
                metronomeTrackKills.enabled = true;
                metronomeTrackKills.maxkills = MaxKills + MaxKillsPerStack * InventoryCount;

                metronomeTrackKills.UpdateKills();
            }
            else
            {
                metronomeTrackKills.kills = 0;
                metronomeTrackKills.enabled = false;
            }

        }

        public class MetronomeTrackKills : MonoBehaviour
        {
            public int kills = 0;
            public int maxkills = 16;
            public int LastSkillSlotUsed = 0;
            public CharacterBody characterBody;
            public BuffDef tallyBuff = Metronome.instance.MetronomeBuffTally;

            public void OnDisable()
            {
                if (characterBody)
                {
                    HelperUtil.ClearBuffStacks(characterBody, tallyBuff.buffIndex);
                }
            }

            public void UpdateKills()
            {
                var InventoryCount = characterBody.inventory.GetItemCount(instance.catalogIndex);
                maxkills = MaxKills + MaxKillsPerStack * (InventoryCount - 1);
                kills = Mathf.Min(kills, maxkills);//this resets it if you have less metronomes from like cleansing
                UpdateBuffStack();
            }

            public void UpdateBuffStack()
            {
                characterBody.SetBuffCount(tallyBuff.buffIndex, kills);
            }
            public void SetLastSkillSlot(int SlotNumber)
            {
                if (LastSkillSlotUsed != SlotNumber)
                {
                    LastSkillSlotUsed = SlotNumber;
                    kills = Math.Max(0, kills - KillsLostOnOtherUse);
                    UpdateKills();
                }
            }
            public void IncrementKills()
            {
                if (kills < maxkills)
                {
                    kills++;
                    UpdateKills();
                }
            }
        }
    }
}
