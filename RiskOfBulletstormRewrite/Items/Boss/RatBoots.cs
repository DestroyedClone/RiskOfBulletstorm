using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class RatBoots : ItemBase<RatBoots>
    {
        public override string ItemName => "Rat Boots";

        public override string ItemLangTokenName => "RATBOOTS";

        public override ItemTier Tier => ItemTier.Boss;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

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

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMotor.Start += CharacterMotor_Start;
        }

        private void CharacterMotor_Start(On.RoR2.CharacterMotor.orig_Start orig, CharacterMotor self)
        {
            orig(self);
            if (UnityEngine.Networking.NetworkServer.active)
            {
                var comp = self.gameObject.AddComponent<RatBootsComponent>();
                comp.characterMotor = self;
            }
        }

        public class RatBootsComponent : MonoBehaviour
        {
            public CharacterMotor characterMotor;
            public float age;
            public float

            public void Start()
            {
                if (!characterMotor)
                {
                    characterMotor = gameObject.GetComponent<CharacterMotor>();
                }
                if (!characterMotor)
                {
                    enabled = false;
                }
            }

            public void Update()
            {
                if (characterMotor.isGrounded)
                {
                    characterMotor.isGrounded = true;
                }
            }
        }
    }
}
