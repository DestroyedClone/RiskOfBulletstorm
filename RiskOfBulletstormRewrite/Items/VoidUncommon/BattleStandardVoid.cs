using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;
using System.Linq;

namespace RiskOfBulletstormRewrite.Items
{
    public class BattleStandardVoid : ItemBase<BattleStandardVoid>
    {
        public static ConfigEntry<float> cfgDamage;

        public override string ItemName => "Leaders Standard";

        public override string ItemLangTokenName => "BATTLESTANDARDVOID";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgDamage)
        };

        public override ItemTier Tier => ItemTier.VoidTier2;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemDef ContagiousOwnerItemDef => Items.BattleStandard.instance.ItemDef;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.Damage,
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
            cfgDamage = config.Bind(ConfigCategory, "Damage Percentage Per Ally", 0.1f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.master)
            { //kingednerbrine
                var count = GetCount(sender);
                if (count > 0)
                {
                    //var minions = CharacterMaster.readOnlyInstancesList.Where(el => el.minionOwnership.ownerMaster == sender.master);
                    var minionGroup = MinionOwnership.MinionGroup.FindGroup(sender.master.netId);
                    if (minionGroup != null)
                        args.damageMultAdd += count * cfgDamage.Value * minionGroup.memberCount;
                }
            }
        }
    }
}
