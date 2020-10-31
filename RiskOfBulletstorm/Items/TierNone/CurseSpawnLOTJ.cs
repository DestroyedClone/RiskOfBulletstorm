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
using EntityStates.Engi.EngiWeapon;
//using Aetherium.Utils;
using RoR2.CharacterAI;
using RoR2.Skills;
using System.Net;

namespace RiskOfBulletstorm.Items
{
    public class CurseSpawnLOTJ : Item_V2<CurseSpawnLOTJ>
    {
        public override string displayName => "";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject ItemBodyModelPrefab;
        public int InventoryCount_LOTJItem;
        public static ItemIndex CurseSpawnLOTJItemIndex;
        public static GameObject characterPrefab;

        public override void SetupBehavior()
        {
            CreatePrefab();


            base.SetupBehavior();
        }
        internal static void CreatePrefab()
        {
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/BrotherGlassBody"), "LordJammedBody", true);

            CharacterBody bodyComponent = characterPrefab.GetComponent<CharacterBody>();
            bodyComponent.bodyIndex = -1; //def: 19
            bodyComponent.baseNameToken = "LORDJAMMED_NAME"; // name token
            bodyComponent.subtitleNameToken = "LORDJAMMED_SUBTITLE"; // subtitle token- used for umbras
            bodyComponent.bodyFlags = CharacterBody.BodyFlags.Mechanical;
            //bodyComponent.mainRootSpeed = 0;
            bodyComponent.baseMaxHealth = 10000;
            bodyComponent.levelMaxHealth = 10000;
            bodyComponent.baseRegen = 1000f;
            bodyComponent.levelRegen = 1000f;
            bodyComponent.baseMaxShield = 0;
            bodyComponent.levelMaxShield = 0;
            //bodyComponent.baseMoveSpeed = 12;
            bodyComponent.levelMoveSpeed = 0;
            //bodyComponent.baseAcceleration = 80;
            //bodyComponent.baseJumpPower = 0;
            bodyComponent.levelJumpPower = 0;
            //bodyComponent.baseDamage = 15;
            //bodyComponent.levelDamage = 1.5f;
            bodyComponent.baseAttackSpeed = 1;
            bodyComponent.levelAttackSpeed = 0;
            bodyComponent.baseCrit = 0;
            //bodyComponent.levelCrit = 0;
            bodyComponent.baseArmor = 10000; // 0.0099 damage multiplier
            bodyComponent.levelArmor = 0;
            bodyComponent.baseJumpCount = 0;
            bodyComponent.sprintingSpeedMultiplier = 1.45f;

            HealthComponent healthComponent = characterPrefab.GetComponent<HealthComponent>();
            healthComponent.dontShowHealthbar = true;

        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

            var itemDisplayRules = new ItemDisplayRule[1];
            var curseSpawnLOTJ = new CustomItem(CurseSpawnLOTJ.instance.itemDef, itemDisplayRules);

            CurseSpawnLOTJItemIndex = ItemAPI.Add(curseSpawnLOTJ); // ItemAPI sends back the ItemIndex of your item


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
            orig(self);
        }
    }
}
*/