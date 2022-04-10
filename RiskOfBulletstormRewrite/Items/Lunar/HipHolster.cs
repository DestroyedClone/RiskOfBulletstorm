using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;
using RoR2.Skills;

namespace RiskOfBulletstormRewrite.Items
{
    public class HipHolster : ItemBase<HipHolster>
    {
        public override string ItemName => "Hip Holster";

        public override string ItemLangTokenName => "HIPHOLSTER";

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

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
                        comp = obj.gameObject.AddComponent<HipHolsterController>();
                        if (obj.skillLocator)
                        {
                            if (obj.skillLocator.special)
                            {
                                comp.specialSkill = obj.skillLocator.special;
                            }
                        }
                        return;
                    }
                    UnityEngine.Object.Destroy(comp);
                }
            }
        }

        public class HipHolsterController : MonoBehaviour
        {
            //public GenericSkill[] genericSkills;
            //public int[] lastStocks;
            public GenericSkill specialSkill;
            public int lastStock = -1;

            public void FixedUpdate()
            {

                if (lastStock != specialSkill.stock)
                {
                    if (lastStock < specialSkill.stock)
                    {
                        specialSkill.stock += specialSkill.skillDef.requiredStock;
                        specialSkill.ExecuteIfReady();
                    }
                    lastStock = specialSkill.stock;
                }
            }
        }
    }
}
