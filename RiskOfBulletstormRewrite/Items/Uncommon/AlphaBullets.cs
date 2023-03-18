using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Items
{
    public class AlphaBullets : ItemBase<AlphaBullets>
    {
        public static ConfigEntry<float> cfgDamage;
        public static ConfigEntry<float> cfgDamageStack;

        public override string ItemName => "Alpha Bullets";

        public override string ItemLangTokenName => "ALPHABULLET";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgDamage),
            GetChance(cfgDamageStack)
        };

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Utility
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
            cfgDamage = config.Bind(ConfigCategory, "Damage", 0.05f, "");
            cfgDamageStack = config.Bind(ConfigCategory, "Damage Per Stack", 0.025f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyStartGlobal += AddComp;
            RecalculateStatsAPI.GetStatCoefficients += StatCoefficients;
        }

        private void StatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                var buffCount = sender.GetBuffCount(Utils.Buffs.AlphaBulletBuff);
                var itemCount = GetCount(sender);
                if (buffCount > 0 && itemCount > 0)
                {
                    var multiplier = GetStack(cfgDamage, cfgDamageStack, itemCount);

                    args.damageMultAdd += buffCount * multiplier;
                }
            }
        }

        public void AddComp(CharacterBody body)
        {
            if (NetworkServer.active && body.inventory)
            {
                body.gameObject.AddComponent<RBS_AlphaBullet>();
            }
        }

        public class RBS_AlphaBullet : MonoBehaviour
        {
            public CharacterBody body;

            public GenericSkill primary = null;
            public GenericSkill secondary = null;
            public GenericSkill utility = null;
            public GenericSkill special = null;

            public bool hasItem = false;

            public void Start()
            {
                if (!body)
                {
                    body = gameObject.GetComponent<CharacterBody>();
                }
                if (!body.skillLocator)
                {
                    enabled = false;
                    return;
                }
                else
                {
                    var sk = body.skillLocator;
                    primary = sk.primary;
                    secondary = sk.secondary;
                    utility = sk.utility;
                    special = sk.special;
                }
                body.onInventoryChanged += OnInventoryChanged;
                OnInventoryChanged();
            }

            public void OnDestroy()
            {
                body.onInventoryChanged -= OnInventoryChanged;
            }

            public void OnInventoryChanged()
            {
                hasItem = body.inventory.GetItemCount(instance.ItemDef) > 0;
                if (hasItem)
                {
                    enabled = true;
                }
                else
                {
                    enabled = false;
                }
            }

            public void OnDisable()
            {
                body.SetBuffCount(Utils.Buffs.AlphaBulletBuff.buffIndex, 0);
            }

            public void FixedUpdate()
            {
                if (!hasItem)
                    return;
                var validSkills = Inc(primary) + Inc(secondary) + Inc(utility) + Inc(special);
                body.SetBuffCount(Utils.Buffs.AlphaBulletBuff.buffIndex, validSkills);
            }

            public int Inc(GenericSkill gs)
            {
                if (!gs)
                    return 0;
                return gs.stock >= gs.maxStock ? 1 : 0;
            }
        }
    }
}