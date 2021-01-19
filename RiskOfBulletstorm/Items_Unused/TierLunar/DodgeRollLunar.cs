
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
        public static SkillDef dodgeRollSkillDef;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

            LanguageAPI.Add("BULLETSTORM_DODGEUTILITY_REPLACEMENT_NAME", "Dodge Roll");
            LanguageAPI.Add("BULLETSTORM_DODGEUTILITY_REPLACEMENT_DESCRIPTION", "Teaching of the Dodge Roll:\n" +
                "Roll forward, gaining brief invulnerability.");

            LoadoutAPI.AddSkill(typeof(Lunar.TeleportUtilitySkillState));
            dodgeRollSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            dodgeRollSkillDef.activationState = new SerializableEntityStateType(typeof(Lunar.TeleportUtilitySkillState));
            dodgeRollSkillDef.activationStateMachineName = "Weapon";
            dodgeRollSkillDef.baseMaxStock = 1;
            dodgeRollSkillDef.baseRechargeInterval = 8f;
            dodgeRollSkillDef.beginSkillCooldownOnSkillEnd = true;
            dodgeRollSkillDef.canceledFromSprinting = false;
            dodgeRollSkillDef.fullRestockOnAssign = true;
            dodgeRollSkillDef.interruptPriority = InterruptPriority.Skill;
            dodgeRollSkillDef.isBullets = false;
            dodgeRollSkillDef.isCombatSkill = true;
            dodgeRollSkillDef.mustKeyPress = false;
            dodgeRollSkillDef.noSprint = true;
            dodgeRollSkillDef.rechargeStock = 1;
            dodgeRollSkillDef.requiredStock = 1;
            dodgeRollSkillDef.shootDelay = 0f;
            dodgeRollSkillDef.stockToConsume = 1;
            dodgeRollSkillDef.icon = Resources.Load<Sprite>("textures/difficultyicons/texDifficultyEclipse8Icon");
            dodgeRollSkillDef.skillDescriptionToken = "BULLETSTORM_DODGEUTILITY_REPLACEMENT_DESCRIPTION";
            dodgeRollSkillDef.skillName = "BULLETSTORM_DODGEUTILITY_REPLACEMENT_NAME";
            dodgeRollSkillDef.skillNameToken = "BULLETSTORM_DODGEUTILITY_REPLACEMENT_NAME";

            LoadoutAPI.AddSkillDef(dodgeRollSkillDef);

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
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);

            var skillLocator = self.skillLocator;
            if (skillLocator)
            {
                if (skillLocator.utility)
                {
                    var bookCount = GetCount(self);
                    if (bookCount > 0)
                    {
                        skillLocator.utility.SetSkillOverride(this, dodgeRollSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                    }
                    else
                    {
                        skillLocator.utility.UnsetSkillOverride(this, dodgeRollSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                    }
                }
            }
        }

    }
}
