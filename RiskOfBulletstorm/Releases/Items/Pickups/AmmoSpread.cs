using System.Collections.ObjectModel;
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
        protected override string GetPickupString(string langID = null) => "Restores all players' cooldowns on pickup.";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

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
            GenericNotification.SetItem += GenericNotification_SetItem;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            GenericNotification.SetItem -= GenericNotification_SetItem;
        }
        private void GenericNotification_SetItem(GenericNotification.orig_SetItem orig, RoR2.UI.GenericNotification self, ItemDef itemDef)
        {
            if (itemDef.itemIndex == catalogIndex)
            {
                orig(self, null);
                return;
            }

            orig(self, itemDef);
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

                        //in case some maniac uses more than two equipment slots
                        if (restoreOffhandEquipmentCharges)
                        {
                            for (int i = 0; i < inventory.GetEquipmentSlotCount(); i++)
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
