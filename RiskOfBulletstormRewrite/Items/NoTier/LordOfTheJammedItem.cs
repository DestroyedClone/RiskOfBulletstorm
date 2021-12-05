using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class LordOfTheJammedItem : ItemBase<LordOfTheJammedItem>
    {
        public override string ItemName => "Lord of the Jammed";

        public override string ItemLangTokenName => "LORDOFTHEJAMMEDITEM";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("ExampleItemPrefab.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("ExampleItemIcon.png");

        public override ItemTag[] ItemTags => new ItemTag[]
        {

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
            Inventory.onServerItemGiven += Inventory_onServerItemGiven;
        }

        private void Inventory_onServerItemGiven(Inventory inventory, ItemIndex arg2, int arg3)
        {
            if (arg2 == ItemDef.itemIndex)
            {
                var tracker = inventory.gameObject.GetComponent<LOTJTracker>();
                if (!tracker)
                    tracker = inventory.gameObject.AddComponent<LOTJTracker>();

            }
        }

        public class LOTJTracker : MonoBehaviour
        {
            public CharacterBody bossBody = null;
            public CharacterMaster bossMaster = null;
            public RoR2.CharacterAI.BaseAI baseAI;

            public void Start()
            {
                if (!bossMaster)
                {
                    var masterSummon = new MasterSummon()
                    {
                        ignoreTeamMemberLimit = true,
                        masterPrefab = Enemies.LordofTheJammedMonster.masterPrefab,
                        position = transform.position,
                        teamIndexOverride = TeamIndex.Monster
                    };

                    bossMaster = masterSummon.Perform();
                    bossBody = bossMaster.GetBody();
                }
            }

            public void OnDestroy()
            {
                if (bossMaster)
                {
                    bossMaster.TrueKill();
                }
            }
        }
    }
}
