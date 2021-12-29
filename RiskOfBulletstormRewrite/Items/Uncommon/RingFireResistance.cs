using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class RingFireResistance : ItemBase<RingFireResistance>
    {
        public static ConfigEntry<float> cfgBaseResist;
        //public static ConfigEntry<float> cfgStackResist;

        public override string ItemName => "Ring of Fire Resistance";

        public override string ItemLangTokenName => "RINGFIRERESISTANCE";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Healing
        };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgBaseResist = config.Bind(ConfigCategory, "Base Fire Resistance", 0.1f, "");
            //cfgStackResist = config.Bind(ConfigCategory, "Hyperbolic Stack Fire Resistance", 0.25f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            On.RoR2.DotController.InflictDot_GameObject_GameObject_DotIndex_float_float += ReduceFireDot;
        }

        private void ReduceFireDot(On.RoR2.DotController.orig_InflictDot_GameObject_GameObject_DotIndex_float_float orig, GameObject victimObject, GameObject attackerObject, DotController.DotIndex dotIndex, float duration, float damageMultiplier)
        {
            if (dotIndex == DotController.DotIndex.PercentBurn || dotIndex == DotController.DotIndex.Burn)
            {
                if (victimObject && victimObject.GetComponent<CharacterBody>())
                {
                    var characterBody = victimObject.GetComponent<CharacterBody>();
                    var itemCount = GetCount(characterBody);
                    if (itemCount > 0)
                    {
                        var multiplier = Utils.ItemHelpers.GetHyperbolicValue(cfgBaseResist.Value, itemCount);
                        _logger.LogMessage($"DoT: dmgMult({damageMultiplier})   reduction({1-multiplier:F2})");
                        damageMultiplier *= (1 - multiplier);
                    }
                }
            }
            orig(victimObject, attackerObject, dotIndex, duration, damageMultiplier);
        }
    }
}
