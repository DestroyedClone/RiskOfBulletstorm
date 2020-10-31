using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using UnityEngine.Networking.NetworkSystem;
using EntityStates.BrotherMonster;
using JetBrains.Annotations;

namespace RiskOfBulletstorm.Items
{
    public class SynergyEnabler : Item_V2<SynergyEnabler>
    {
        public override string displayName => "SynergyEnabler";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "why did you give yourself this";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        //synergy = [string synergyName, string synergyDesc, ItemIndex itemIndexA, ItemIndex itemIndexB, component componentName, method Methodname]
        //bool synergy_1 = false; //Soldier's Syringe + Energy Drink = OG Cola: +20% faster attack speed, movement speed, and health regen.
        //bool synergy_2 = false; //Harvester's Scythe + Old Guiollotine = Die! Die! Die!: +25 base damage.
                                //ItemIndex synergy_2_a = ItemIndex.CritHeal;
                                //ItemIndex synergy_2_b = ItemIndex.ExecuteLowHealthElite;
        public object[,] synergies =
        {
            {"Die! Die! Die!", "Grants you the damage of the Reaper", ItemIndex.HealOnCrit, ItemIndex.ExecuteLowHealthElite, false},
            //+25 dmg
            {"Heretic", "Burn at the Stake", ItemIndex.LunarPrimaryReplacement, ItemIndex.LunarUtilityReplacement, false},
            //All damage becomes the fire damage type
            {"Cleansed","You have been absolved",ItemIndex.LunarBadLuck, ItemIndex.LunarTrinket, false},
            // Immunity to Tonic Afflictions
            {"Gay Baby Escapee","Doggo Needed This",ItemIndex.ShieldOnly, ItemIndex.FocusConvergence, false }
            // Gives you a dio's immediately upon getting reaver'd
            
            //{"Cleansed","Did that feel good?",ItemIndex.LunarBadLuck, ItemIndex.Clover, false},
            // Immunity to Tonic Afflictions
        };
        public bool synergy1Enabled = false;
        public bool synergy2Enabled = false;
        public bool synergy3Enabled = false;
        public bool synergy4Enabled = false;
        private void ReaperSynergy(CharacterBody sender, StatHookEventArgs args)
        {
            if (synergy1Enabled)
            {
                args.baseDamageAdd += 25;
            }
        }
        private void HereticSynergy(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (synergy2Enabled)
            {
                var InventoryCount = GetCount(self.body);

                if (InventoryCount < 1)
                    return;
                else
                {
                    damageInfo.damageType = DamageType.IgniteOnHit;
                }
            }
            orig(self, damageInfo);
        }
        private void CloverSynergy(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            var inv = self.inventory;
            ItemIndex itemIndex = ItemIndex.TonicAffliction;
            if (synergy3Enabled)
            {
                if (CheckInvForItem(itemIndex, inv))//TODO: can also be done with getcomponent but i'm tired rn
                {
                    inv.RemoveItem(itemIndex);
                }
            }
            orig(self);
        }
        private void VoidSynergy(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var InventoryCount = GetCount(self.body);

            if (InventoryCount < 1)
                return;
            else
            {
                switch (damageInfo.damageType)
                {
                    case DamageType.VoidDeath:
                        damageInfo.damageType = DamageType.Generic;
                        damageInfo.damage = 0;
                        break;
                    default:
                        break;
                }
            }
            orig(self, damageInfo);
        }

        public override void Install()
        {
            base.Install();
            On.RoR2.CharacterBody.OnInventoryChanged += SynergyV2;
            GetStatCoefficients += ReaperSynergy;
            On.RoR2.HealthComponent.TakeDamage += HereticSynergy;
            On.RoR2.HealthComponent.TakeDamage += VoidSynergy;
            On.RoR2.CharacterBody.OnInventoryChanged += CloverSynergy;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= SynergyV2;
            GetStatCoefficients -= ReaperSynergy;
            On.RoR2.HealthComponent.TakeDamage -= HereticSynergy;
            On.RoR2.HealthComponent.TakeDamage -= VoidSynergy;
            On.RoR2.CharacterBody.OnInventoryChanged -= CloverSynergy;
        }
        public override void SetupBehavior() { }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        private void SynergyV2(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            var inv = self.inventory;

            for (int x = 0; x < synergies.GetLength(0); x += 1)
            {
                var synergyName = synergies[x, 0];
                var synergyDesc = synergies[x, 1];
                ItemIndex itemFirst = (ItemIndex)synergies[x, 2]; //explicit conversion?
                ItemIndex itemSecond = (ItemIndex)synergies[x, 3];
                bool enabled = (bool)synergies[x, 4];

                bool firstExist = CheckInvForItem(itemFirst, inv);
                bool secondExist = CheckInvForItem(itemSecond, inv);

                if (!firstExist || !secondExist) { synergies[x, 4] = false; }
                if ((firstExist && secondExist) && enabled == false)
                {
                    synergies[x, 4] = true;
                    Chat.AddPickupMessage(self, "Synergy!", new Color32(0, 76, 191, 255), 1);
                    Chat.AddMessage("\n"+synergyName+"\n"+synergyDesc+"\n");
                }
            }
            orig(self);
        }
        static bool CheckInvForItem(ItemIndex itemIndex, Inventory inv)
        {
            if (inv.GetItemCount(itemIndex) < 1)
            { return true; }
            else
            { return false; }
            
        }

        /*private void TestSynergy(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            var inv = self.inventory;
            // Harvester's Scythe + Old Guiollotine = Die! Die! Die!: +25 base damage.
            var amountScythe = inv.GetItemCount(ItemIndex.CritHeal);
            var amountOldG = inv.GetItemCount(ItemIndex.ExecuteLowHealthElite);

            if (amountScythe < 1 || amountOldG < 1)
            {
                synergy_2 = false;
            }
            if ((amountScythe > 0 && amountOldG > 0) && synergy_2 == false)
            {
                synergy_2 = true;
                Chat.AddPickupMessage(self, self.subtitleNameToken, new Color32(0, 76, 191, 255), 1);
                Chat.AddMessage("\nDie! Die! Die!\nGrants you the damage of the Reaper.\n");
            }
            orig(self);
        }
        private void GiveSynergy(CharacterBody sender, StatHookEventArgs args)
        {
            if (synergy_2 == true)
            {
                args.baseDamageAdd += 25;
            }
        }*/
    }
}
