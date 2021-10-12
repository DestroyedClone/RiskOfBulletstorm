using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using static RiskOfBulletstormRewrite.Utils.ItemHelpers;

namespace RiskOfBulletstormRewrite.Items
{
    public class SpiceTally : ItemBase<SpiceTally>
    {

        public override string ItemName => "Spice (Consumed)";

        public override string ItemLangTokenName => "SPICETALLY";

        public override string ItemPickupDesc => "It's delicious!";

        public override string ItemFullDescription => $"Grants various stat changes. Most notably, increased damage and reduced accuracy and health.";

        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("Assets/Models/Prefabs/Scope.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("Assets/Textures/Icons/Scope.png");

        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {

        }
    }
}