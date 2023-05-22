using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Items
{
    public class AlphaBullets : ItemBase<AlphaBullets>
    {
        public static float damage = 0.05f;
        public static float damagePerStack = 0.025f;

        public override string ItemName => "Alpha Bullets";

        public override string ItemLangTokenName => "ALPHABULLET";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            ToPct(damage),
            ToPct(damagePerStack)
        };

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => LoadModel();

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Damage
        };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += StatCoefficients;
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster obj)
        {
            if (NetworkServer.active && obj.inventory)
            {
                obj.gameObject.AddComponent<RBSAlphaBulletController>().characterMaster = obj;
            }
        }

        private void StatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender)
            {
                var buffCount = sender.GetBuffCount(Utils.Buffs.AlphaBulletBuff);
                var itemCount = GetCount(sender);
                if (buffCount > 0 && itemCount > 0)
                {
                    var multiplier = GetStack(damage, damagePerStack, itemCount);

                    args.damageMultAdd += buffCount * multiplier;
                }
            }
        }

        public class RBSAlphaBulletController : MonoBehaviour
        {
            public CharacterBody body;
            public CharacterMaster characterMaster;

            public GenericSkill primary = null;
            public GenericSkill secondary = null;
            public GenericSkill utility = null;
            public GenericSkill special = null;

            public BuffIndex AlphaBulletBuffIndex => Utils.Buffs.AlphaBulletBuff.buffIndex;

            public bool hasItem = false;


            public void Start()
            {
                body = characterMaster.GetBody();
                if (body)
                {
                    return;
                }
                characterMaster.onBodyStart += CharacterMaster_onBodyStart;
                characterMaster.onBodyDestroyed += CharacterMaster_onBodyDestroyed;
                characterMaster.inventory.onInventoryChanged += Inventory_onInventoryChanged;
            }

            private void Inventory_onInventoryChanged()
            {
                hasItem = instance.GetCount(characterMaster) > 0;
            }

            private void CharacterMaster_onBodyDestroyed(CharacterBody obj)
            {
                body = null;
            }

            private void CharacterMaster_onBodyStart(CharacterBody obj)
            {
                body = obj;
                if (body.skillLocator)
                {
                    var sk = body.skillLocator;
                    primary = sk.primary;
                    secondary = sk.secondary;
                    utility = sk.utility;
                    special = sk.special;
                }
            }

            public void OnDestroy()
            {
                characterMaster.onBodyStart -= CharacterMaster_onBodyStart;
                characterMaster.onBodyDestroyed -= CharacterMaster_onBodyDestroyed;
                characterMaster.inventory.onInventoryChanged -= Inventory_onInventoryChanged;
            }

            public void OnDisable()
            {
                SetBuffCount(0);
            }

            public void SetBuffCount(int count)
            {
                if (body == null)
                {
                    return;
                }

                var currentBuffCount = body.GetBuffCount(AlphaBulletBuffIndex);
                if (currentBuffCount == count)
                {
                    return;
                }

                if (currentBuffCount > count)
                {
                    for (int i = 0; i < currentBuffCount - count; i++)
                    {
                        body.RemoveBuff(AlphaBulletBuffIndex);
                    }
                }
                else
                {
                    for (int i = 0; i < count - currentBuffCount; i++)
                    {
                        body.AddBuff(AlphaBulletBuffIndex);
                    }
                }
            }


            public void FixedUpdate()
            {
                if (!hasItem)
                {
                    SetBuffCount(0);
                    return;
                }
                var validSkillCount = Inc(primary) + Inc(secondary) + Inc(utility) + Inc(special);
                SetBuffCount(validSkillCount);
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