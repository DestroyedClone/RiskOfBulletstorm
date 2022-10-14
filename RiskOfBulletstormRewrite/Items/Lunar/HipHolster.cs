using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;
using RoR2.Skills;
using System.Collections.Generic;

namespace RiskOfBulletstormRewrite.Items
{
    public class HipHolster : ItemBase<HipHolster>
    {
        public static ConfigEntry<float> cfgFreeStockChance;
        public static ConfigEntry<float> cfgFreeStockChancePerStack;

        public override string ItemName => "Hip Holster";

        public override string ItemLangTokenName => "HIPHOLSTER";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgFreeStockChance),
            GetChance(cfgFreeStockChancePerStack)
        };

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Cleansable
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
            cfgFreeStockChance = config.Bind(ConfigCategory, "Chance of Free Shot", 0.1f, "");
            cfgFreeStockChancePerStack = config.Bind(ConfigCategory, "Chance of Free Shot Per Stack", 0.1f, "Hyperbolic");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody obj)
        {
            if (NetworkServer.active)
            {
                var comp = obj.gameObject.GetComponent<HipHolsterController>();
                bool flag = GetCount(obj) > 0;
                if (flag != comp)
                {
                    if (flag)
                    {
                        if (obj.skillLocator)
                        {
                            comp = obj.gameObject.AddComponent<HipHolsterController>();
                            comp.skillLocator = obj.skillLocator;
                            comp.inventory = obj.inventory;
                            comp.characterBody = obj;
                        }
                        return;
                    }
                    UnityEngine.Object.Destroy(comp);
                }
            }
        }

        public class HipHolsterController : MonoBehaviour
        {
            public CharacterBody characterBody;
            public Inventory inventory;
            public SkillLocator skillLocator;
            public GenericSkill[] genericSkills;
            public int[] lastStocks;

            public void Start()
            {
                List<GenericSkill> genericSkillsList = new List<GenericSkill>();
                List<int> lastStockList = new List<int>();
                if (skillLocator.primary)
                {
                    genericSkillsList.Add(skillLocator.primary);
                    lastStockList.Add(skillLocator.primary.stock);
                }
                if (skillLocator.secondary)
                {
                    genericSkillsList.Add(skillLocator.secondary);
                    lastStockList.Add(skillLocator.secondary.stock);
                }
                if (skillLocator.utility)
                {
                    genericSkillsList.Add(skillLocator.utility);
                    lastStockList.Add(skillLocator.utility.stock);
                }
                if (skillLocator.special)
                {
                    genericSkillsList.Add(skillLocator.special);
                    lastStockList.Add(skillLocator.special.stock);
                }
                genericSkills = genericSkillsList.ToArray();
                lastStocks = lastStockList.ToArray();
            }

            public void FixedUpdate()
            {
                int i = 0;
                while (i < genericSkills.Length)
                {
                    var gs = genericSkills[i];
                    if (gs)
                    {
                        if (lastStocks[i] < gs.stock)
                        {
                            if (RoR2.Util.CheckRoll(100 * Utils.ItemHelpers.GetHyperbolicValue(cfgFreeStockChance.Value, cfgFreeStockChancePerStack.Value, HipHolster.instance.GetCount(characterBody))))
                            {
                                gs.stock += gs.skillDef.requiredStock;
                                gs.ExecuteIfReady();
                            }
                        }
                        lastStocks[i] = gs.stock;
                    }
                    i++;
                }
            }
        }
    }
}
