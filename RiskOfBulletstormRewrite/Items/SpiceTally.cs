using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class SpiceTally : ItemBase<SpiceTally>
    {
        public override string ItemName => "Spice (Consumed)";

        public override string ItemLangTokenName => "SPICETALLY";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("Assets/Models/Prefabs/Scope.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("Assets/Textures/Icons/Spice.png");

        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }
    }
}