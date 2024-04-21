using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class LordOfTheJammedTargetingItem : ItemBase<LordOfTheJammedTargetingItem>
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

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }

        public override void Hooks()
        {
            Inventory.onServerItemGiven += Inventory_onServerItemGiven;
        }

        private void Inventory_onServerItemGiven(Inventory inventory, ItemIndex itemIndex, int stackCount)
        {
            if (itemIndex == ItemDef.itemIndex)
            {
                if (!inventory.gameObject.GetComponent<LOTJTracker>())
                    inventory.gameObject.AddComponent<LOTJTracker>();
            }
        }

        public class LOTJTracker : MonoBehaviour
        {
            public CharacterMaster ownerMaster = null;
            public CharacterBody ownerBody = null;

            public CharacterMaster bossMaster = null;
            public CharacterBody bossBody = null;
            public RoR2.CharacterAI.BaseAI baseAI;

            public float stopwatch = 0f;
            public float duration = 10f;

            public void Start()
            {
                ownerMaster = GetComponent<CharacterMaster>();
                ownerMaster.onBodyStart += OwnerMaster_onBodyStart;
                ownerMaster.onBodyDestroyed += OwnerMaster_onBodyDestroyed;

                ownerBody = ownerMaster.GetBody();

                AttemptSpawn();
            }

            private void OwnerMaster_onBodyDestroyed(CharacterBody obj)
            {
                if (ownerMaster.IsDeadAndOutOfLivesServer())
                {
                    if (bossMaster)
                        bossMaster.TrueKill();
                }
            }

            private void AttemptSpawn()
            {
                if (bossMaster || !ownerBody)
                {
                    return;
                }
                var masterSummon = new MasterSummon()
                {
                    ignoreTeamMemberLimit = true,
                    //masterPrefab = Enemies.LordofTheJammedMonster.masterPrefab,
                    masterPrefab = MasterCatalog.FindMasterPrefab("BrotherGlassMaster"),
                    position = TeleporterInteraction.instance
                    ? TeleporterInteraction.instance.transform.position
                    : transform.position,
                    teamIndexOverride = TeamIndex.Monster
                };
                masterSummon.preSpawnSetupCallback += (charMaster) =>
                {
                    charMaster.inventory.GiveItem(RoR2Content.Items.Ghost);
                    charMaster.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
                    charMaster.inventory.GiveItem(LordOfTheJammedIdentifierItem.instance.ItemDef);
                    charMaster.inventory.GiveItem(RoR2Content.Items.Syringe, 30);
                };

                bossMaster = masterSummon.Perform();
                if (!bossMaster) return;

                bossBody = bossMaster.GetBody();

                foreach (EntityStateMachine entityStateMachine in bossBody.GetComponents<EntityStateMachine>())
                {
                    if (entityStateMachine.customName == "Body")
                    {
                        entityStateMachine.SetState(new EntityStates.Heretic.SpawnState());
                        return;
                    }
                }

                if (bossMaster.TryGetComponent(out BaseAI baseAI))
                {
                    baseAI.currentEnemy = new BaseAI.Target(baseAI)
                    {
                        characterBody = ownerBody
                    };
                    baseAI.neverRetaliateFriendlies = true;
                    baseAI.enemyAttentionDuration = Mathf.Infinity;
                    /*foreach (var skillDriver in baseAI.skillDrivers)
                    {
                        skillDriver.moveTargetType = AISkillDriver.TargetType.Custom;
                    }*/
                }
            }

            private void OwnerMaster_onBodyStart(CharacterBody body)
            {
                ownerBody = body;

                if (bossMaster && bossMaster.TryGetComponent<BaseAI>(out BaseAI baseAI))
                {
                    baseAI.currentEnemy = new BaseAI.Target(baseAI)
                    {
                        characterBody = ownerBody
                    };
                }
            }

            public void FixedUpdate()
            {
                RedirectAttention();
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch <= duration)
                {
                    stopwatch = 0;
                    AttemptSpawn();
                }
            }

            public void RedirectAttention()
            {
                if (ownerBody && baseAI)
                    baseAI.currentEnemy = new BaseAI.Target(baseAI)
                    {
                        characterBody = ownerBody
                    };
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