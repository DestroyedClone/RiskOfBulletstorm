﻿using System.Collections.ObjectModel;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using System;
using GenericNotification = On.RoR2.UI.GenericNotification;

namespace RiskOfBulletstorm.Items
{
    public class PickupAmmoSpread : Item_V2<PickupAmmoSpread>
    {
        public override string displayName => "Spread Ammo";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Restores all allies' cooldowns on pickup.";

        protected override string GetDescString(string langid = null) => $"On pickup, restores all skills cooldowns and one equipment charge for all equipment for all allies.";

        protected override string GetLoreString(string langID = null) => "";

        public PickupAmmoSpread()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/SpreadAmmo.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/SpreadAmmo.png"; //For evolution somehow
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupLate()
        {
            base.SetupLate();
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
            On.RoR2.UI.NotificationQueue.OnItemPickup += NotificationQueue_OnItemPickup;
        }

        private void NotificationQueue_OnItemPickup(On.RoR2.UI.NotificationQueue.orig_OnItemPickup orig, NotificationQueue self, CharacterMaster characterMaster, ItemIndex itemIndex)
        {
            if (itemIndex == catalogIndex)
                return;

            orig(self, characterMaster, itemIndex);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            On.RoR2.UI.NotificationQueue.OnItemPickup -= NotificationQueue_OnItemPickup;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            var inventoryCount = GetCount(self);
            for (int i = 0; i < inventoryCount; i++)
            {
                ApplyAmmoPackToTeam(self.teamComponent.teamIndex);
                self.inventory.RemoveItem(catalogIndex);
            }
        }

        public void ApplyAmmoPackToTeam(TeamIndex teamIndex = TeamIndex.Player, bool restoreEquipmentCharges = true, bool restoreOffhandEquipmentCharges = true)
        {
            var pickupEffect = (GameObject)Resources.Load("prefabs/effects/AmmoPackPickupEffect");
            ReadOnlyCollection<TeamComponent> teamComponents = TeamComponent.GetTeamMembers(teamIndex);
            foreach (TeamComponent teamComponent in teamComponents)
            {
                CharacterBody body = teamComponent.body;
                if (body)
                {
                    body.GetComponent<SkillLocator>()?.ApplyAmmoPack();

                    var inventory = body.inventory;
                    if (inventory)
                    {
                        if (restoreEquipmentCharges) inventory.RestockEquipmentCharges(0, 1);

                        if (restoreOffhandEquipmentCharges)
                        {
                            for (int i = 0; i < inventory.GetEquipmentSlotCount()-1; i++)
                            {
                                inventory.RestockEquipmentCharges((byte)Math.Min(i, 255), 1);
                            }
                        }
                    }
                    EffectManager.SimpleEffect(pickupEffect, body.transform.position, Quaternion.identity, true);
                }
            }
            
        }
    }
}