using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class LordOfTheJammedItem : ItemBase<LordOfTheJammedItem>
    {
        public override string ItemName => "Lord of the Jammed";

        public override string ItemLangTokenName => "LORDOFTHEJAMMED";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

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
                tracker.ownerMaster = inventory.gameObject.GetComponent<CharacterMaster>();
            }
        }

        public class LOTJTracker : MonoBehaviour
        {
            public CharacterMaster ownerMaster = null;
            public CharacterBody ownerBody = null;

            public CharacterMaster bossMaster = null;
            public CharacterBody bossBody = null;
            public RoR2.CharacterAI.BaseAI baseAI;

            public void Start()
            {
                ownerMaster.onBodyStart += OwnerMaster_onBodyStart;

                ownerBody = gameObject.GetComponent<CharacterBody>();

                AttemptSpawn();
            }

            private void AttemptSpawn()
            {
                if (!bossMaster)
                {
                    var masterSummon = new MasterSummon()
                    {
                        ignoreTeamMemberLimit = true,
                        //masterPrefab = Enemies.LordofTheJammedMonster.masterPrefab,
                        masterPrefab = MasterCatalog.FindMasterPrefab("BrotherGlassBody"),
                        position = TeleporterInteraction.instance
                        ? TeleporterInteraction.instance.transform.position
                        : transform.position,
                        teamIndexOverride = TeamIndex.Monster
                    };

                    bossMaster = masterSummon.Perform();
                    bossMaster.inventory.GiveItem(RoR2Content.Items.Ghost);
                    bossMaster.inventory.GiveItem(RoR2Content.Items.SummonedEcho);
                    bossBody = bossMaster.GetBody();
                    baseAI = bossMaster.GetComponent<BaseAI>();
                }
            }

            private void OwnerMaster_onBodyStart(CharacterBody body)
            {
                ownerBody = body;
            }

            public void FixedUpdate()
            {
                RedirectAttention();
            }

            public void RedirectAttention()
            {
                if (ownerBody && baseAI)
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