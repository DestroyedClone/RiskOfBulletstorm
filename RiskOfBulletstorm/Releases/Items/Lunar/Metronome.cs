using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;
using System;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class Metronome : Item<Metronome>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What are the maximum amount of kills that can be counted by the Metronome?", AutoConfigFlags.PreventNetMismatch)]
        public static int MaxKills { get; private set; } = 75;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional max kills can be counted by the Metronome per stack?", AutoConfigFlags.PreventNetMismatch)]
        public static int MaxKillsPerStack { get; private set; } = 25;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many kills are lost upon using a different ability?", AutoConfigFlags.PreventNetMismatch)]
        public static int KillsLostOnOtherUse { get; private set; } = 75;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the damage multiplier per kill for the metronome? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public static float DamageCoefficientPerStack { get; private set; } = 0.02f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Use styled formatting or descriptive formatting for the description? (Value: Bool)", AutoConfigFlags.PreventNetMismatch)]
        public static bool EnableUseStyleFormat { get; private set; } = true;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        // [AutoConfig("Show stacks of metronome as a buff on screen? Default: true", AutoConfigFlags.PreventNetMismatch)]
        //public static bool Metronome_ShowAsBuff { get; private set; } = true;

        public override string displayName => "Metronome";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null)
        {
            var desc = "<b>Better And Better</b>\n";
            if (DamageCoefficientPerStack == 0 && MaxKillsPerStack <= 0)
                return desc + "Does nothing.";

            if (DamageCoefficientPerStack != 0)
            {
                if (DamageCoefficientPerStack > 0) desc += "<style=cIsDamage>Improves";
                else if (DamageCoefficientPerStack < 0) desc += "<style=cDeath>Worsens";
                desc += " your damage with ";

                // I'm losing my mind IDK WHATS GOING ON AAAAAAAAAAAAAAAAAAAAAAAAAAAA
                if (MaxKills > 1 || MaxKillsPerStack > 0)
                    desc += "sequential kills";
                else desc += "one kill";
                desc += "</style>";

                if (KillsLostOnOtherUse > 0)
                    desc += ", <style=cDeath>but loses "+ KillsLostOnOtherUse +" upon another skill.</style>";
            }
            return desc;
        }

        protected override string GetDescString(string langid = null)
        {
            if (EnableUseStyleFormat)
            {
                return $"Increases your damage to enemies by <style=cIsDamage>{Pct(DamageCoefficientPerStack)} per kill</style> with the same skill up to " +
                    $"({MaxKills} times (<style=cStack>{(MaxKillsPerStack > 0 ? MaxKillsPerStack + "per stack.<style>" : "")}" +
                    $" {(KillsLostOnOtherUse > 0 ? "Upon using a different skill, your bonus <style=cHealth>reduces by " + KillsLostOnOtherUse + "</style>" : "")}";
            } else
            {
                var desc = $"";
                // Nothing Check //
                if (DamageCoefficientPerStack == 0 && MaxKillsPerStack <= 0)
                    return desc + "Does nothing.";

                desc += $"Multiplies your damage by <style=cIsDamage>{Pct(DamageCoefficientPerStack)} per kill</style> with the same skill." +
                $"\n <style=cStack>Max Kills: {MaxKills} {(MaxKillsPerStack > 0 ? MaxKillsPerStack + "per stack." : "")}</style>" +
                $"\n {(KillsLostOnOtherUse > 0 ? "<style=cDeath>Using a different skill will reset your bonus by " + KillsLostOnOtherUse + "</style>" : "")}";

                return desc;
            }
        }

        protected override string GetLoreString(string langID = null) => "Tick, tick, tick, tick...." +
            "\n The metronome struck back and forth, and with every strike, a bullet ended a life." +
            "\n The sharpshooter grew stronger and stronger with each kill, his rifle shredding through skin, clothing, armor, even bullet-proof scales." +
            "\n He always had his machete at his hip, incase anyone got in too close. He knew if he struck, he would strike with the force of a thousand bullets." +
            "\n But he was wrong." +
            "\n An enemy came in close. Too close for his sniper. So he struck, not with the force of many, but with the force of one." +
            "\n And it only took one strike to end him." +
            "\n Tick, Tick, tick, tick";

        public BuffDef MetronomeBuffTally;
        public static GameObject ItemBodyModelPrefab;

        public Metronome()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/Metronome.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/Metronome.png");
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            if (Compat_ItemStats.enabled)
            {
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return MaxKills + MaxKillsPerStack * (count - 1); },
                    (value, inv, master) => { return $"Max Stacks: {value}"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return KillsLostOnOtherUse; },
                    (value, inv, master) => { return $"Kills Lost On Other Skill Use: {value}"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef, //TODO? HOW?
                    ((count, inv, master) => {
                        return DamageCoefficientPerStack; 
                    },
                    (value, inv, master) => { return $"Damage Multiplier Per Kill: +{Pct(value)}"; }
                ));
            }
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = modelResource;
                displayRules = GenerateItemDisplayRules();
            }

            base.SetupAttributes();

            MetronomeBuffTally = Shared.Buffs.BuffsController.RegisterBuff(Color.blue,
                true,
                false,
                "Assets/Textures/Icons/Buffs/Metronome.png",
                "Metronome Stacks (Display)");
        }

        public static ItemDisplayRuleDict GenerateItemDisplayRules()
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Base",
                    localPos = new Vector3(0.0961F, 0.3304F, 0.0824F),
                    localAngles = new Vector3(325.1511F, 20.5061F, 124.823F),
                    localScale = new Vector3(0.0331F, 0.0331F, 0.0331F)
                }
            });
            return rules;
        }

        public override void Install()
        {
            base.Install();
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            On.RoR2.GlobalEventManager.OnCharacterDeath -= GlobalEventManager_OnCharacterDeath;
            On.RoR2.GenericSkill.OnExecute -= GenericSkill_OnExecute;
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
        }

        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            CharacterBody vBody = self.characterBody;
            if (vBody && vBody.inventory)
            {
                int invCount = vBody.inventory.GetItemCount(catalogIndex);

                if (invCount > 0)
                {
                    SkillLocator skillLocation = vBody.skillLocator;
                    if (skillLocation)
                    {
                        MetronomeTrackKills MetronomeTrackKills = self.gameObject.GetComponent<MetronomeTrackKills>();
                        if (MetronomeTrackKills && MetronomeTrackKills.enabled)
                        {
                            if (skillLocation.FindSkill(self.skillName)) //Updates last skill slot used
                            {
                                if (skillLocation.primary.Equals(self)) { MetronomeTrackKills.SetLastSkillSlot(0); }
                                else if (skillLocation.secondary.Equals(self)) { MetronomeTrackKills.SetLastSkillSlot(1); }
                                else if (skillLocation.utility.Equals(self)) { MetronomeTrackKills.SetLastSkillSlot(2); }
                                else if (skillLocation.special.Equals(self)) { MetronomeTrackKills.SetLastSkillSlot(3); }
                                //else { _logger.LogError("Metronome: Invalid Skill Slot ["+self+"] accessed!"); }
                            }
                        }
                    }
                }
            }
            orig(self);
        }
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) //Update Max Kills
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
            } else
            {
                metronomeTrackKills.kills = 0;
                metronomeTrackKills.enabled = false;
            }
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            var attackerBody = damageReport?.attackerBody;
            if (attackerBody) //if not world
            {
                int inventoryCount = GetCount(attackerBody);
                if (inventoryCount > 0)
                {
                    var componentExists = attackerBody.GetComponent<MetronomeTrackKills>();
                    if (componentExists && componentExists.enabled)
                    {
                        componentExists.IncrementKills();
                    }
                }
            }
            orig(self, damageReport);
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                MetronomeTrackKills MetronomeTrackKills = damageInfo.attacker.gameObject.GetComponent<MetronomeTrackKills>();
                if (MetronomeTrackKills && MetronomeTrackKills.enabled)
                {
                    damageInfo.damage *= 1 + (DamageCoefficientPerStack * MetronomeTrackKills.kills);
                }
            }
            orig(self, damageInfo);
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
