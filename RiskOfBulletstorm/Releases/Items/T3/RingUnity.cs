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

//TY Harb

namespace RiskOfBulletstorm.Items
{
    public class Unity : Item_V2<Unity>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage increase on single stack? (Default: 0.1)" +
            "\nKeep in mind that this number is MULTIPLIED by the amount of TOTAL items.",
            AutoConfigFlags.PreventNetMismatch)]
        public float DamageBonus { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage increase on subsequent stacks (Default: 0.01)" +
            "\nKeep in mind that this number is MULTIPLIED by the amount of TOTAL items.", AutoConfigFlags.PreventNetMismatch)]
        public float DamageBonusStack { get; private set; } = 0.01f;
        public override string displayName => "Unity";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Our Powers Combined\nIncreased combat effectiveness per item.";

        protected override string GetDescString(string langid = null) => $"+{DamageBonus} ({DamageBonusStack} per stack) damage per unique item in inventory";

        protected override string GetLoreString(string langID = null) => "This ring takes a small amount of power from each gun carried and adds it to the currently equipped gun.";

        //FieldInfo itemStacksField;

        public Unity()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/RingUnity.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/RingUnityIcon.png";
        }

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
            GetStatCoefficients += BoostDamage;
            On.RoR2.Run.Start += Run_Start;
        }

        private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            //itemStacksField = typeof(Inventory).GetField("itemStacks", BindingFlags.Instance | BindingFlags.NonPublic);

        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients -= BoostDamage;
            On.RoR2.Run.Start -= Run_Start;
        }

        static int GetTotalItemCountOfTier(ItemTier itemTier, CharacterBody self) //borrowed method
        {
            int num = 0;
            ItemIndex itemIndex = ItemIndex.Syringe;
            ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
            while (itemIndex < itemCount)
            {
                if (ItemCatalog.GetItemDef(itemIndex).tier == itemTier)
                {
                    num += self.inventory.GetItemCount(itemIndex);
                }
                itemIndex++;
            }
            return num;
        }
        private void BoostDamage(CharacterBody sender, StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (!inventory) return;
            var UnityInventoryCount = inventory.GetItemCount(catalogIndex);
            //var inv = self.inventory;
            //int tier1Items = GetItemCount(ItemTier.Tier1, self);
            //int tier2Items = GetItemCount(ItemTier.Tier2, self);
            int TotalItemCount = 0;

            if (UnityInventoryCount > 0)
            {
                foreach (ItemTier tier in (ItemTier[])Enum.GetValues(typeof(ItemTier))) //https://stackoverflow.com/questions/105372/how-to-enumerate-an-enum
                {
                    TotalItemCount += GetTotalItemCountOfTier(tier, sender);
                }
                args.baseDamageAdd += TotalItemCount * (DamageBonus + (DamageBonusStack * (UnityInventoryCount - 1)));
            }
        }
    }
}
