
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using static RoR2.GenericSkill;
using EntityStates;


namespace RiskOfBulletstorm.Items
{
    public class DodgeRollLunar : Item_V2<DodgeRollLunar>
    {
        public override string displayName => "Teaching of the Dodge Roll";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.Cleansable });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Replaces your utility with a dodge roll.";

        protected override string GetDescString(string langid = null) => $"Roll a distance forward." +
            $"\nReplaced with a random lunar beyond one stack.";

        protected override string GetLoreString(string langID = null) => "";

        public static GameObject ItemBodyModelPrefab;
        public static SkillDef teleportSkillDef;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

            LanguageAPI.Add("BULLETSTORM_UTILITY_REPLACEMENT_NAME", "Dodge Roll");
            LanguageAPI.Add("BULLETSTORM_UTILITY_REPLACEMENT_DESCRIPTION", "Teaching of the Dodge Roll:\n" +
                "Roll forward, gaining brief invulnerability.");

            LoadoutAPI.AddSkill(typeof(Lunar.TeleportUtilitySkillState));
            teleportSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            teleportSkillDef.activationState = new SerializableEntityStateType(typeof(Lunar.TeleportUtilitySkillState));
            teleportSkillDef.activationStateMachineName = "Weapon";
            teleportSkillDef.baseMaxStock = 1;
            teleportSkillDef.baseRechargeInterval = 8f;
            teleportSkillDef.beginSkillCooldownOnSkillEnd = true;
            teleportSkillDef.canceledFromSprinting = false;
            teleportSkillDef.fullRestockOnAssign = true;
            teleportSkillDef.interruptPriority = InterruptPriority.Skill;
            teleportSkillDef.isBullets = false;
            teleportSkillDef.isCombatSkill = true;
            teleportSkillDef.mustKeyPress = false;
            teleportSkillDef.noSprint = true;
            teleportSkillDef.rechargeStock = 1;
            teleportSkillDef.requiredStock = 1;
            teleportSkillDef.shootDelay = 0f;
            teleportSkillDef.stockToConsume = 1;
            teleportSkillDef.icon = Resources.Load<Sprite>("textures/difficultyicons/texDifficultyEclipse1Icon");
            teleportSkillDef.skillDescriptionToken = "BULLETSTORM_UTILITY_REPLACEMENT_DESCRIPTION";
            teleportSkillDef.skillName = "BULLETSTORM_UTILITY_REPLACEMENT_NAME";
            teleportSkillDef.skillNameToken = "BULLETSTORM_UTILITY_REPLACEMENT_NAME";

            LoadoutAPI.AddSkillDef(teleportSkillDef);

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
        }


        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            CharacterBody body = self.body;
            if (body)
            {
                int debuffStacks = body.GetBuffCount(ScarfVuln);
                if (debuffStacks > 0)
                {
                    damageInfo.damage *= 1 + (debuffStacks * BloodiedScarf_DamageIncrease);
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
        }

        private void Inventory_GiveItem(On.RoR2.Inventory.orig_GiveItem orig, Inventory self, ItemIndex itemIndex, int count)
        {
            var scarfCount = GetCount(self);
            var stridesCount = self.GetItemCount(ItemIndex.LunarUtilityReplacement);
            var body = self.gameObject.GetComponent<PlayerCharacterMasterController>()?.body;

            // TODO: Replace with a switch statement or something holy shit man

            // Picking up a scarf while you have strides
            if (stridesCount > 0 && itemIndex == catalogIndex)
            {
                if (BloodiedScarf_DropCoins)
                    DropLunarCoin(body.corePosition, stridesCount);
                else
                    DropItemIndex(body.corePosition, ItemIndex.LunarUtilityReplacement, stridesCount);
                self.RemoveItem(ItemIndex.LunarUtilityReplacement, stridesCount);
            }
            // Picking up a Strides while you have Scarves
            if (scarfCount > 0 && itemIndex == ItemIndex.LunarUtilityReplacement)
            {
                if (BloodiedScarf_DropCoins)
                    DropLunarCoin(body.corePosition, scarfCount);
                else
                    DropItemIndex(body.corePosition, instance.catalogIndex, scarfCount);
                self.RemoveItem(catalogIndex, scarfCount);
            }
            orig(self, itemIndex, count);
        }

        void DropLunarCoin(Vector3 position, int count)
        {
            for (uint i = 0; i < count; i++)
            {
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex("LunarCoin.Coin0"), position, Vector3.up * 2f);
            }
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
            if (skillLocator)
            {
                if (skillLocator.utility)
                {
                    var scarfCount = GetCount(self);
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
}
