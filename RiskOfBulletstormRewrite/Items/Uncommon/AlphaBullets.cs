using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Items;
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

        public override string WikiLink => "https://thunderstore.io/package/DestroyedClone/RiskOfBulletstorm/wiki/177-item-alpha-bullet/";

        public override void Hooks()
        {
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
                    var multiplier = GetStack(damage, damagePerStack, itemCount);

                    args.damageMultAdd += buffCount * multiplier;
                }
            }
        }

        public class RBSAlphaBulletBehaviour : BaseItemBodyBehavior
        {
            private SkillLocator skillLocator;

            private InputBankTest inputBank;
            public CharacterMaster characterMaster;

            public GenericSkill primary = null;
            public GenericSkill secondary = null;
            public GenericSkill utility = null;
            public GenericSkill special = null;

            public BuffIndex AlphaBulletBuffIndex => Utils.Buffs.AlphaBulletBuff.buffIndex;

            private void Awake()
            {
                base.enabled = false;
            }

            private void OnEnable()
            {
                if (this.body)
                {
                    this.skillLocator = this.body.GetComponent<SkillLocator>();
                    this.inputBank = this.body.GetComponent<InputBankTest>();
                    if (body.skillLocator)
                    {
                        var sk = body.skillLocator;
                        primary = sk.primary;
                        secondary = sk.secondary;
                        utility = sk.utility;
                        special = sk.special;
                    }
                }
            }

            private void OnDisable()
            {
                if (this.body)
                {
                    while (this.body.HasBuff(AlphaBulletBuffIndex))
                    {
                        this.body.RemoveBuff(AlphaBulletBuffIndex);
                    }
                }
                this.inputBank = null;
                this.skillLocator = null;
            }

            public void FixedUpdate()
            {
                var validSkillCount = Inc(primary) + Inc(secondary) + Inc(utility) + Inc(special);
                SetBuffCount(validSkillCount);
            }

            public int Inc(GenericSkill gs)
            {
                if (!gs)
                    return 0;
                return gs.stock >= gs.maxStock ? 1 : 0;
            }

            public void SetBuffCount(int count)
            {
                if (body == null)
                    return;

                var currentBuffCount = body.GetBuffCount(AlphaBulletBuffIndex);
                int difference = count - currentBuffCount;

                if (difference > 0)
                {
                    for (int i = 0; i < difference; i++)
                    {
                        body.AddBuff(AlphaBulletBuffIndex);
                    }
                }
                else if (difference < 0)
                {
                    for (int i = 0; i < -difference; i++)
                    {
                        body.RemoveBuff(AlphaBulletBuffIndex);
                    }
                }
            }
        }

        //based off PrimarySkillShurikenBehavior
        public class RBSAlphaBulletBehavior : CharacterBody.ItemBehavior
        {
            private SkillLocator skillLocator;

            private InputBankTest inputBank;
            public CharacterMaster characterMaster;

            public GenericSkill primary = null;
            public GenericSkill secondary = null;
            public GenericSkill utility = null;
            public GenericSkill special = null;

            public BuffIndex AlphaBulletBuffIndex => Utils.Buffs.AlphaBulletBuff.buffIndex;

            private void Awake()
            {
                base.enabled = false;
            }

            private void OnEnable()
            {
                if (this.body)
                {
                    this.skillLocator = this.body.GetComponent<SkillLocator>();
                    this.inputBank = this.body.GetComponent<InputBankTest>();
                    if (body.skillLocator)
                    {
                        var sk = body.skillLocator;
                        primary = sk.primary;
                        secondary = sk.secondary;
                        utility = sk.utility;
                        special = sk.special;
                    }
                }
            }

            private void OnDisable()
            {
                if (this.body)
                {
                    while (this.body.HasBuff(AlphaBulletBuffIndex))
                    {
                        this.body.RemoveBuff(AlphaBulletBuffIndex);
                    }
                }
                this.inputBank = null;
                this.skillLocator = null;
            }

            public void FixedUpdate()
            {
                var validSkillCount = Inc(primary) + Inc(secondary) + Inc(utility) + Inc(special);
                SetBuffCount(validSkillCount);
            }

            public int Inc(GenericSkill gs)
            {
                if (!gs)
                    return 0;
                return gs.stock >= gs.maxStock ? 1 : 0;
            }

            public void SetBuffCount(int count)
            {
                if (body == null)
                    return;

                var currentBuffCount = body.GetBuffCount(AlphaBulletBuffIndex);
                int difference = count - currentBuffCount;

                if (difference > 0)
                {
                    for (int i = 0; i < difference; i++)
                    {
                        body.AddBuff(AlphaBulletBuffIndex);
                    }
                }
                else if (difference < 0)
                {
                    for (int i = 0; i < -difference; i++)
                    {
                        body.RemoveBuff(AlphaBulletBuffIndex);
                    }
                }
            }
        }
    }

    public class AlphaBulletsBodyBehavior : BaseItemBodyBehavior
    {
        public BuffIndex AlphaBulletBuffIndex => Utils.Buffs.AlphaBulletBuff.buffIndex;
        [BaseItemBodyBehavior.ItemDefAssociationAttribute(useOnServer = true, useOnClient = true)]
        private static ItemDef GetItemDef()
        {
            return AlphaBullets.instance.ItemDef;
        }

        private void OnEnable()
        {
            this.headstompersControllerObject = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/HeadstompersController"));
            this.headstompersControllerObject.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.body.gameObject, null);
        }

        private void OnDisable()
        {
            if (this.headstompersControllerObject)
            {
                UnityEngine.Object.Destroy(this.headstompersControllerObject);
                this.headstompersControllerObject = null;
            }
        }

        private GameObject headstompersControllerObject;
    }
}