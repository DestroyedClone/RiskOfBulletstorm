
//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using static RoR2.GenericSkill;


namespace RiskOfBulletstorm.Items
{
    public class BloodiedScarf : Item_V2<BloodiedScarf>
    {
        const float dmgIncrease = 0.25f;

        public override string displayName => "Bloodied Scarf";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.Cleansable });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>Blink Away</b>\nDodge roll is replaced with a blink.";

        protected override string GetDescString(string langid = null) => $"<style=cIsUtility>Teleport</style> up to <style=cIsUtility>5m</style> " +
            $"<style=cStack>(+5m per stack)</stack>.\n" +
            $"After teleporting, <style=cDeath>take 25% more damage (per stack) for 1 seconds after teleporting</style>.";

        protected override string GetLoreString(string langID = null) => "This simple scarf was once worn by a skilled assassin. Betrayed by his brothers and assumed dead...";

        public static GameObject ItemBodyModelPrefab;
        //public static SkillDef TeleportUtilitySkillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("LunarPrimaryReplacement"));
        public static BuffIndex ScarfVuln { get; private set; }
        public static SkillDef teleportSkillDef;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();


            //teleportSkillDef
            var scarfDebuff = new CustomBuff(
            new BuffDef
            {
                buffColor = Color.blue,
                canStack = true,
                isDebuff = true,
                name = "Bloodied Scarf: Vulnerable!",
            });
            ScarfVuln = BuffAPI.Add(scarfDebuff);

        }
        public override void SetupLate()
        {
            base.SetupLate();

            SkillCatalog.skillsDefined.CallWhenAvailable(delegate
            {
                teleportSkillDef = SkillCatalog.GetSkillDef(SkillCatalog.FindSkillIndexByName("Blink"));
            });
        }


        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.Inventory.GiveItem += Inventory_GiveItem;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.EntityStates.Huntress.BlinkState.OnExit += BlinkState_OnExit;
        }

        private void BlinkState_OnExit(On.EntityStates.Huntress.BlinkState.orig_OnExit orig, EntityStates.Huntress.BlinkState self)
        {
            orig(self);
            var body = self.characterBody;
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    int scarfCount = inventory.GetItemCount(catalogIndex);
                    int debuffStacks = body.GetBuffCount(ScarfVuln);
                    if (debuffStacks > 0)
                    {
                        body.ClearTimedBuffs(ScarfVuln);
                    }
                    for (uint i = 0; i < scarfCount; i++)
                        body.AddTimedBuff(ScarfVuln, 1f);
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            CharacterBody body = self.body;
            if (body)
            {
                int debuffStacks = body.GetBuffCount(ScarfVuln);
                if (debuffStacks > 0)
                {
                    damageInfo.damage *= 1 + (debuffStacks * dmgIncrease);
                }
            }
            
            orig(self, damageInfo);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
            On.RoR2.Inventory.GiveItem -= Inventory_GiveItem;
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
            On.EntityStates.Huntress.BlinkState.OnExit -= BlinkState_OnExit;
        }

        private void Inventory_GiveItem(On.RoR2.Inventory.orig_GiveItem orig, Inventory self, ItemIndex itemIndex, int count)
        {
            var scarfCount = GetCount(self);
            var stridesCount = self.GetItemCount(ItemIndex.LunarUtilityReplacement);
            var body = self.gameObject.GetComponent<PlayerCharacterMasterController>()?.body;

            // Picking up a scarf while you have strides
            if (stridesCount > 0 && itemIndex == ItemIndex.LunarUtilityReplacement)
            {
                Chat.AddMessage("Picking up a scarf while you have strides");
                DropItemIndex(body.corePosition, ItemIndex.LunarUtilityReplacement, stridesCount);
                self.RemoveItem(ItemIndex.LunarUtilityReplacement, stridesCount);
            }
            // Picking up a Strides while you have Scarves
            if (scarfCount > 0 && itemIndex == catalogIndex)
            {
                Chat.AddMessage("Picking up a Strides while you have Scarves");
                DropItemIndex(body.corePosition, catalogIndex, stridesCount);
                self.RemoveItem(catalogIndex, scarfCount);
            }
            orig(self, itemIndex, count);
        }

        void DropItemIndex(Vector3 position, ItemIndex itemIndex, int count)
        {
            for (uint i = 0; i < count; i++)
            {
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(itemIndex), position, Vector3.up * 5f);
            }
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);

            var skillLocator = self.skillLocator;
            var scarfCount = GetCount(self);
            if (skillLocator)
            {
                if (scarfCount > 0)
                {
                    //skillLocator.utility.SetSkillOverride(this, CharacterBody.CommonAssets.lunarPrimaryReplacementSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                    skillLocator.utility.SetSkillOverride(this, teleportSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                }
                else
                {
                    //skillLocator.utility.UnsetSkillOverride(this, CharacterBody.CommonAssets.lunarPrimaryReplacementSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                    skillLocator.utility.UnsetSkillOverride(this, teleportSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                }
            }
        }
    }
}
