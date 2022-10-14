using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using RoR2.CharacterAI;

namespace RiskOfBulletstormRewrite.Items
{
    public class LordOfTheJammedItem : ItemBase<LordOfTheJammedItem>
    {
        public override string ItemName => "Lord of the Jammed";

        public override string ItemLangTokenName => "LORDOFTHEJAMMED";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("ExampleItemPrefab.prefab");

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {

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
            public CharacterBody ownerBody = null;
            public CharacterBody bossBody = null;
            public CharacterMaster bossMaster = null;
            public RoR2.CharacterAI.BaseAI baseAI;

            public void Start()
            {
                ownerBody = gameObject.GetComponent<CharacterBody>();
                if (!bossMaster)
                {
                    var masterSummon = new MasterSummon()
                    {
                        ignoreTeamMemberLimit = true,
                        //masterPrefab = Enemies.LordofTheJammedMonster.masterPrefab,
                        masterPrefab = MasterCatalog.FindMasterPrefab("BrotherBody"),
                        position = TeleporterInteraction.instance 
                        ? TeleporterInteraction.instance.transform.position
                        : transform.position,
                        teamIndexOverride = TeamIndex.Monster
                    };

                    bossMaster = masterSummon.Perform();
                    bossMaster.inventory.GiveItem(RoR2Content.Items.Ghost.itemIndex);
                    bossMaster.inventory.GiveItem(RoR2Content.Items.SummonedEcho);
                    bossBody = bossMaster.GetBody();
                    baseAI = bossMaster.GetComponent<BaseAI>();
                }
            }

            public void FixedUpdate()
            {
                RedirectAttention();
            }

            public void RedirectAttention()
            {
                baseAI.customTarget.characterBody = ownerBody;
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
