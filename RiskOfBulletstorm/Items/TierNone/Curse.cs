/*
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using EntityStates.ScavMonster;

namespace RiskOfBulletstorm.Items
{
    public class Curse : Item_V2<Curse>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many curse is needed before Lord of the Jammed spawns? Set it to -1 to disable. (Default: 10)", AutoConfigFlags.PreventNetMismatch)]
        public float CurseMax { get; private set; } = 10f; //THIS IS WHERE I LEFT OFF!!!!!

        public override string displayName => "";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject ItemBodyModelPrefab;
        private int InventoryCount;
        private bool isMaxCurse = false;

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
            On.RoR2.CharacterBody.OnInventoryChanged += CalculateCurse;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CalculateCurse;

        }
        private void CalculateCurse(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self) //blessed komrade
        {
            InventoryCount = GetCount(self);
            //int InventoryCountLOTJ = GetCount(CurseSpawnLOTJ.instance.InventoryCount_LOTJItem);
            if (InventoryCount >= CurseMax) //If you're at max curse, set isMaxCurse to true
            {
                isMaxCurse = true;
            }
            if (isMaxCurse) //if you're at max curse, continue
            {

            }
            orig(self);
        }
    }
}
*/