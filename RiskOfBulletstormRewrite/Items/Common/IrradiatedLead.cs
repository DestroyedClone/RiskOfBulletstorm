using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class IrradiatedLead : ItemBase<IrradiatedLead>
    {
        public static ConfigEntry<float> cfgChance;
        public static ConfigEntry<float> cfgChanceStack;

        public override string ItemName => "Irradiated Lead";

        public override string ItemLangTokenName => "IRRADIATEDLEAD";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Damage
        };

        public override void Init(ConfigFile config)
        {
            return;
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgChance = config.Bind(ConfigCategory, "Chance", 0.05f, "");
            cfgChanceStack = config.Bind(ConfigCategory, "Chance Per Stack", 0.02f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = ItemModel;
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.05f, 0.05f, 0.05f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0, 0, 0)
                }
            });
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += ModifyDamage;
        }

        public void ModifyDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.attacker)
            {
                var body = damageInfo.attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    var itemCount = GetCount(body);
                    if (itemCount > 0)
                    {
                        if (Util.CheckRoll(GetStack(cfgChance, cfgChanceStack, itemCount), body.master ?? null))
                        {
                            damageInfo.damageType &= DamageType.BleedOnHit;
                        }
                    }
                }
            }
            orig(self, damageInfo);
        }
    }
}