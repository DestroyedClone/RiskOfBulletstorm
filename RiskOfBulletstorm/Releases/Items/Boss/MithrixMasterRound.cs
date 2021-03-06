﻿using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static RiskOfBulletstorm.Items.MasterRoundNth;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class MithrixMasterRound : Item<MithrixMasterRound>
    {
        //public static Color32 lunarColor32 = ColorCatalog.GetColor(ColorCatalog.ColorIndex.LunarItem);
        //public static string lunarColorString = ColorCatalog.GetColorHexString(ColorCatalog.ColorIndex.LunarItem);
        //string formattedDisplayName = string.Format("Celestial Master Round", lunarColorString);
        public override string displayName => "Master Round (Lunar)";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        private readonly string desc = $"Lunar Chamber\nThis celestial artifact indicates mastery of the planet.";
        protected override string GetPickupString(string langID = null) => desc;

        protected override string GetDescString(string langid = null) => desc;

        protected override string GetLoreString(string langID = null) => "The last bullet that delivered the hero to redemption.";

        public MithrixMasterRound()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Textures/Icons/MasterRoundXVII.png");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Models/Prefabs/SpreadAmmo.prefab");
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }
        public override void Install()
        {
            base.Install();
            On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += Phase1_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter += EncounterFinished_OnEnter;
            GetStatCoefficients += MithrixMasterRound_GetStatCoefficients;

            On.RoR2.UI.GenericNotification.SetItem += GenericNotification_SetItem;

        }

        private void GenericNotification_SetItem(On.RoR2.UI.GenericNotification.orig_SetItem orig, RoR2.UI.GenericNotification self, ItemDef itemDef)
        {
            orig(self, itemDef);
            //if (itemDef == ItemCatalog.GetItemDef(catalogIndex))
                //self.titleTMP.color = lunarColor32;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter -= Phase1_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.EncounterFinished.OnEnter -= EncounterFinished_OnEnter;
            GetStatCoefficients -= MithrixMasterRound_GetStatCoefficients;

            On.RoR2.UI.GenericNotification.SetItem -= GenericNotification_SetItem;
        }

        private void Phase1_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
        {
            orig(self);
            MasterRoundNth.instance.MasterRound_Start();
        }

        private void EncounterFinished_OnEnter(On.EntityStates.Missions.BrotherEncounter.EncounterFinished.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.EncounterFinished self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                var comps = UnityEngine.Object.FindObjectsOfType<MasterRoundComponent>();
                foreach (var component in comps)
                {
                    component.teleporterCharging = false;
                }
                MasterRoundNth.instance.MasterRound_CheckResult(catalogIndex);
            }
        }
        private void MithrixMasterRound_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            args.healthMultAdd += MasterRoundNth.instance.MasterRound_MaxHealthMult * GetCount(sender);
        }
    }
}
