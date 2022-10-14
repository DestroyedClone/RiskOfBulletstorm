using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Main;
using System.Collections.Generic;

namespace RiskOfBulletstormRewrite.Items
{
    public class BabyGoodMimic : ItemBase<BabyGoodMimic>
    {
        public override string ItemName => "Baby Good Mimic";

        public override string ItemLangTokenName => "BABYGOODMIMIC";

        public override ItemTier Tier => ItemTier.Tier3;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
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
            Inventory.onInventoryChangedGlobal += OnInventoryChangedGlobal;
        }

        public void OnInventoryChangedGlobal(Inventory inventory)
        {
            if (NetworkServer.active)
            {
                var comp = inventory.GetComponent<RBS_BabyMimicBehaviour>();
                if (inventory.GetItemCount(ItemDef) > 0)
                {
                    if (!comp)
                    {
                        //Chat.AddMessage("Mimic giving component");
                        comp = inventory.gameObject.AddComponent<RBS_BabyMimicBehaviour>();
                        //comp.ownerMaster = inventory.GetComponent<CharacterMaster>();
                        //comp.ownerBody = comp.ownerMaster.GetBody();
                        //comp.inventory = inventory;
                    }
                } else {
                    if (comp)
                    {
                        //Chat.AddMessage("Mimic removing component");
                        UnityEngine.Object.Destroy(comp);
                    }
                }
            }
        }

        //down, right, down, up, down, up
        //died to FUCKING rat

        public class RBS_BabyMimicBehaviour : MonoBehaviour
        {
            public CharacterMaster ownerMaster;
            public CharacterBody ownerBody;
            public Inventory inventory;
            public MinionOwnership.MinionGroup minionGroup;
            public float stopwatch = 0;
            float duration = 30f;
            int itemCount = 0;

            public List<CharacterMaster> mimics = new List<CharacterMaster>();

            public void OnEnable()
            {
                ownerMaster = gameObject.GetComponent<CharacterMaster>();
                OnOwnerBodyStart(ownerMaster.GetBody());
                inventory = ownerMaster.inventory;
                
                inventory.onInventoryChanged += UpdateItemCount;
                ownerMaster.onBodyStart += OnOwnerBodyStart;
                minionGroup = MinionOwnership.MinionGroup.FindGroup(ownerBody.master.netId);
                if (minionGroup == null)
                    enabled = false;
            }

            public void OnDisable()
            {
                inventory.onInventoryChanged -= UpdateItemCount;
                ownerMaster.onBodyStart -= OnOwnerBodyStart;
            }

            public void OnOwnerBodyStart(CharacterBody body)
            {
                ownerBody = body;
            }

            public void UpdateItemCount()
            {
                itemCount = inventory.GetItemCount(instance.ItemDef);
            }

            public void FixedUpdate()
            {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= duration)
                {
                    stopwatch = 0;
                    if (ownerBody)
                        UpdateMinions();
                }
            }

            public void UpdateMinions()
            {
                int minionsToCopy = itemCount - mimics.Count;

                if (minionsToCopy > 0)
                {
                    foreach (var minion in minionGroup.members)
                    {
                        var cb = minion.GetComponent<CharacterBody>();
                        if (cb && cb.healthComponent && cb.healthComponent.alive
                        && cb.inventory && cb.master 
                        && cb.inventory.GetItemCount(RoR2Content.Items.Ghost) == 0
                        && !cb.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical))
                        {
                            AssignMinion(cb);
                            minionsToCopy--;
                        }
                        if (minionsToCopy <= 0)
                            break;
                    }
                } else {
                    stopwatch += 5;
                }
            }

            public void AssignMinion(CharacterBody targetMinion)
            {
                var copy = Util.TryToCreateGhost(targetMinion, ownerBody, 10);
                copy.inventory.RemoveItem(RoR2Content.Items.Ghost);
                copy.inventory.RemoveItem(RoR2Content.Items.HealthDecay, 10);
                copy.baseNameToken = "RISKOFBULLETSTORM_BABYGOODMIMIC_BODY_NAME";
                copy.subtitleNameToken = "RISKOFBULLETSTORM_BABYGOODMIMIC_BODY_SUBTITLE";
                //copy.portraitIcon = 
                TeleportHelper.TeleportBody(copy, ownerBody.footPosition);

                var comp = copy.gameObject.AddComponent<RBS_BabyMimicPairing>();
                //comp.myBody = copy;
                comp.ownerBody = ownerBody;
                //comp.targetBody = targetMinion;
                comp.targetMaster = targetMinion.master;
                comp.ownerComponent = this;
                mimics.Add(copy.master);
            }
        }

        public class RBS_BabyMimicPairing : MonoBehaviour
        {
            public CharacterMaster myMaster;
            public CharacterBody ownerBody;
            //public CharacterBody myBody;
            public CharacterMaster targetMaster;
            public RBS_BabyMimicBehaviour ownerComponent;

            public void OnEnable()
            {
                myMaster = gameObject.GetComponent<CharacterMaster>();
                //myBody = myMaster.GetBody();
                targetMaster.onBodyDestroyed += OnBodyDestroyed;
            }

            public void OnDisable()
            {
                targetMaster.onBodyDestroyed -= OnBodyDestroyed;
            }

            public void OnBodyDestroyed(CharacterBody characterBody)
            {
                if (ownerComponent)
                {
                    ownerComponent.mimics.Remove(myMaster);
                }
                myMaster.TrueKill();
            }
        }

    }
}
