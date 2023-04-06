using BepInEx.Configuration;
using EntityStates;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class BloodiedScarf : ItemBase<BloodiedScarf>
    {
        public static ConfigEntry<float> cfgTeleportRange;
        public static ConfigEntry<float> cfgTeleportRangePerStack;
        public static ConfigEntry<float> cfgDamageVulnerabilityMultiplier;
        public static ConfigEntry<float> cfgDamageVulnerabilityMultiplierPerStack;
        public static ConfigEntry<float> cfgDamageVulnerabilityDuration;

        public override string ItemName => "Bloodied Scarf";

        public override string ItemLangTokenName => "BLOODIEDSCARF";

        public override bool ItemDescriptionLogbookOverride => true;

        public override ItemTier Tier => ItemTier.Lunar;

        public override bool IsSkillReplacement => true;

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.CannotSteal,
            ItemTag.Cleansable,
            ItemTag.AIBlacklist,
            ItemTag.CannotCopy
        };

        public static SkillDef teleportSkillDef;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateSkillDef();
            CreateLang();
            CreateItem();
            Hooks();
        }

        public string[] TeleportSkillDefParams => new string[]
        {
            cfgTeleportRange.Value.ToString(),
            cfgTeleportRangePerStack.Value.ToString(),
            GetChance(cfgDamageVulnerabilityMultiplier),
            GetChance(cfgDamageVulnerabilityMultiplierPerStack),
            cfgDamageVulnerabilityDuration.Value.ToString()
        };

        public void CreateSkillDef()
        {
            teleportSkillDef = ScriptableObject.CreateInstance<SkillDef>();
            teleportSkillDef.activationState = new SerializableEntityStateType(typeof(TeleportUtilitySkillState));
            teleportSkillDef.activationStateMachineName = "Body";
            teleportSkillDef.baseMaxStock = 1;
            teleportSkillDef.baseRechargeInterval = 8;
            teleportSkillDef.beginSkillCooldownOnSkillEnd = true;
            teleportSkillDef.canceledFromSprinting = false;
            teleportSkillDef.cancelSprintingOnActivation = false;
            teleportSkillDef.dontAllowPastMaxStocks = false;
            teleportSkillDef.forceSprintDuringState = true;
            teleportSkillDef.fullRestockOnAssign = true;
            teleportSkillDef.icon = Assets.LoadSprite("SKILL_BLOODIEDSCARF");
            teleportSkillDef.interruptPriority = InterruptPriority.Vehicle;
            teleportSkillDef.isCombatSkill = false;
            teleportSkillDef.keywordTokens = new string[]
            {
                "KEYWORD_AGILE"
            };
            teleportSkillDef.mustKeyPress = true;
            teleportSkillDef.rechargeStock = 1;
            teleportSkillDef.requiredStock = 1;
            teleportSkillDef.resetCooldownTimerOnUse = true;
            teleportSkillDef.skillDescriptionToken = "RISKOFBULLETSTORM_SKILL_TELEPORT_DESCRIPTION";
            teleportSkillDef.skillName = "RiskOfBulletstormTeleport";
            teleportSkillDef.skillNameToken = "RISKOFBULLETSTORM_SKILL_TELEPORT_NAME";
            (teleportSkillDef as ScriptableObject).name = teleportSkillDef.skillName;
            teleportSkillDef.stockToConsume = 1;

            ContentAddition.AddSkillDef(teleportSkillDef);
            ContentAddition.AddEntityState<TeleportUtilitySkillState>(out bool wasAdded);
        }

        protected override void CreateLang()
        {
            base.CreateLang();

            LanguageOverrides.DeferToken(teleportSkillDef.skillDescriptionToken, TeleportSkillDefParams);

            LanguageOverrides.DeferLateTokens(ItemDescriptionToken, new string[] { teleportSkillDef.skillDescriptionToken });
            LanguageOverrides.DeferLateTokens(ItemDescriptionLogbookToken, new string[]{
                ItemPickupToken,
                teleportSkillDef.skillNameToken,
                teleportSkillDef.skillDescriptionToken});
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgTeleportRange = config.Bind(ConfigCategory, "Teleport Range", 50f, "Distance in meters");
            cfgTeleportRangePerStack = config.Bind(ConfigCategory, "Teleport Range Per Stack", 5f, "Distance in meters");
            cfgDamageVulnerabilityMultiplier = config.Bind(ConfigCategory, "Damage Vulnerability Multiplier", .2f, "");
            cfgDamageVulnerabilityMultiplierPerStack = config.Bind(ConfigCategory, "Damage Vulnerability Multiplier Per Stack", .1f, "");
            cfgDamageVulnerabilityDuration = config.Bind(ConfigCategory, "Damage Vulnerability Duration", 1f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += OnBodyInventoryChangedGlobal;
            On.RoR2.HealthComponent.TakeDamage += TakeDamage;
        }

        private void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body && self.body.HasBuff(Utils.Buffs.DodgeRollBuff))
            {
                damageInfo.damage *= 1 +
                GetStack(cfgDamageVulnerabilityMultiplier, cfgDamageVulnerabilityMultiplierPerStack, GetCount(self.body));
            }
            orig(self, damageInfo);
        }

        private void OnBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            if (characterBody.skillLocator)
            {
                characterBody.ReplaceSkillIfItemPresent(characterBody.skillLocator.utility, ItemDef.itemIndex, teleportSkillDef);
            }
        }
    }
}