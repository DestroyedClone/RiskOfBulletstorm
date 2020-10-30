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

namespace RiskOfBulletstorm.Items
{
    public class SynergyEnabler : Item_V2<SynergyEnabler>
    {
        public override string displayName => "SynergyEnabler";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        /*bool[] synergies =
        {
            false, // 
            false  // 
        };*/
        //bool synergy_1 = false; //Soldier's Syringe + Energy Drink = OG Cola: +20% faster attack speed, movement speed, and health regen.
        bool synergy_2 = false; //Harvester's Scythe + Old Guiollotine = Die! Die! Die!: +25 base damage.
                                //ItemIndex synergy_2_a = ItemIndex.CritHeal;
                                //ItemIndex synergy_2_b = ItemIndex.ExecuteLowHealthElite;

        /*jaggedArray[,] synergies = { 
            { 1, 2, 3 },
            { 4, 5, 6 }
        };*/

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
            On.RoR2.CharacterBody.OnInventoryChanged += TestSynergy;
            GetStatCoefficients += GiveSynergy;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= TestSynergy;
            GetStatCoefficients -= GiveSynergy;
        }
        private void TestSynergy(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
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
        /*static bool CheckInvForItem(ItemIndex itemIndex, Inventory inv)
        {
            if (inv.GetItemCount(itemIndex) < 1)
            {return false;}
            return true;
            
        }*/
        private void GiveSynergy(CharacterBody sender, StatHookEventArgs args)
        {
            if (synergy_2 == true)
            {
                args.baseDamageAdd += 25;
            }
        }
    }
}
