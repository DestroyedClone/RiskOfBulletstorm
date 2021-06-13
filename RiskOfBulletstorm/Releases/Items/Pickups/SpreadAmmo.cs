using System.Collections.ObjectModel;
using RoR2;
using RoR2.UI;
using UnityEngine;
using TILER2;
using System;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class PickupAmmoSpread : Item<PickupAmmoSpread>
    {
        public override string displayName => "Spread Ammo";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Restores all allies' cooldowns on pickup.";

        protected override string GetDescString(string langid = null) => $"On pickup, <style=cIsUtility>restores cooldowns</style> of <style=cIsUtility>all skills</style> and an <style=cIsDamage>equipment charge</style> for all allies.";

        protected override string GetLoreString(string langID = null) => "";

        public static GameObject pickupEffect = Resources.Load<GameObject>("prefabs/effects/AmmoPackPickupEffect");

        public PickupAmmoSpread()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/SpreadAmmo.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/SpreadAmmo.png");
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.UI.NotificationQueue.OnItemPickup += NotificationQueue_OnItemPickup;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            On.RoR2.UI.NotificationQueue.OnItemPickup -= NotificationQueue_OnItemPickup;
        }

        private void NotificationQueue_OnItemPickup(On.RoR2.UI.NotificationQueue.orig_OnItemPickup orig, NotificationQueue self, CharacterMaster characterMaster, ItemIndex itemIndex)
        {
            if (itemIndex == catalogIndex)
                return;

            orig(self, characterMaster, itemIndex);
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            var inventoryCount = GetCount(self);
            if (inventoryCount == 0) return;
            for (int i = 0; i < inventoryCount; i++)
            {
                ApplyAmmoPackToTeam(self.teamComponent.teamIndex);
                self.inventory.RemoveItem(catalogIndex);
            }
        }

        public void ApplyAmmoPackToTeam(TeamIndex teamIndex = TeamIndex.Player, bool restoreEquipmentCharges = true, bool restoreOffhandEquipmentCharges = true)
        {
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
