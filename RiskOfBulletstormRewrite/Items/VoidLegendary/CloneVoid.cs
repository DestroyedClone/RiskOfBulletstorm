using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace RiskOfBulletstormRewrite.Items
{
    public class CloneVoid : ItemBase<CloneVoid>
    {
        public static ConfigEntry<int> cfgStageCount;
        public static ConfigEntry<int> cfgItemsToGet;
        public static ConfigEntry<int> cfgItemsToGetPerStack;
        public override string ItemName => "Prototype";

        public override string ItemLangTokenName => "CLONEVOID";
        public override string[] ItemFullDescriptionParams => new string[]
        {
            cfgStageCount.Value.ToString(),
            cfgItemsToGet.Value.ToString(),
            cfgItemsToGetPerStack.Value.ToString()
        };

        public override ItemTier Tier => ItemTier.VoidTier3;

        public override GameObject ItemModel => GetPickupModel();

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.BrotherBlacklist,
            ItemTag.CannotCopy,
            ItemTag.CannotDuplicate,
            ItemTag.CannotSteal
        };

        public override ItemDef ContagiousOwnerItemDef => Clone.instance.ItemDef;

        public static bool isCloneRestarting = false;

        public static GameObject GetPickupModel()
        {
            var bearItemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ExtraLife/ExtraLife.asset").WaitForCompletion();
            var bearPMP = bearItemDef.pickupModelPrefab;
            var clonePMP = PrefabAPI.InstantiateClone(bearPMP, "PrototypePickupModelPrefab", false);
            var cloneMeshRenderer = clonePMP.GetComponent<MeshRenderer>();
            if (cloneMeshRenderer)
            {
                var mushroomMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Mushroom/matMushroom.mat").WaitForCompletion();
                for (int i = 0; i < cloneMeshRenderer.materials.Length; i++)
                {
                    cloneMeshRenderer.materials[i] = mushroomMat;
                }
            }
            return clonePMP;
        }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgStageCount = config.Bind(ConfigCategory, "Stage Count", 5, "The amount of stages this item gives items for.");
            cfgItemsToGet = config.Bind(ConfigCategory, "Items To Get", 5);
            cfgItemsToGetPerStack = config.Bind(ConfigCategory, "Items To Get Per Stack", 5);
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            On.RoR2.PlayerCharacterMasterController.OnBodyDeath += PlayerCharacterMasterController_OnBodyDeath;
            On.RoR2.CharacterMaster.SpawnBody += CharacterMaster_SpawnBody;
            On.RoR2.Run.BeginGameOver += PreventGameOverIfClone;
        }

        private CharacterBody CharacterMaster_SpawnBody(On.RoR2.CharacterMaster.orig_SpawnBody orig, CharacterMaster self, Vector3 position, Quaternion rotation)
        {
            if (self.playerCharacterMasterController)
                isCloneRestarting = false;
            return orig(self, position, rotation);
        }

        private void PreventGameOverIfClone(On.RoR2.Run.orig_BeginGameOver orig, Run self, GameEndingDef gameEndingDef)
        {
            if (!isCloneRestarting)
                orig(self, gameEndingDef);
        }

        private void PlayerCharacterMasterController_OnBodyDeath(On.RoR2.PlayerCharacterMasterController.orig_OnBodyDeath orig, PlayerCharacterMasterController self)
        {
            orig(self);
            if (!NetworkServer.active) return;
            var stage = RoR2.Stage.instance;
            if (stage && stage.spawnedAnyPlayer && stage.stageAdvanceTime.isInfinity && !Run.instance.isGameOverServer)
            {
                ReadOnlyCollection<PlayerCharacterMasterController> instances = PlayerCharacterMasterController.instances;
                bool allDead = true;
                List<CharacterMaster> peopleWithClones = new List<CharacterMaster>();

                //int playerItemCount = 0;
                for (int i = 0; i < instances.Count; i++)
                {
                    PlayerCharacterMasterController playerCharacterMasterController = instances[i];
                    if (playerCharacterMasterController.isConnected)
                    {
                        if (!playerCharacterMasterController.master.IsDeadAndOutOfLivesServer())
                        {
                            // If there's a player alive, end the loop early.
                            allDead = false;
                            break;
                        }
                        else
                        {
                            if (GetCount(playerCharacterMasterController.master) > 0)
                            {
                                peopleWithClones.Add(playerCharacterMasterController.master);
                            }
                        }
                    }
                }
                if (allDead && peopleWithClones.Count > 0)
                {
                    isCloneRestarting = true;
                    Stage.instance.gameObject.AddComponent<RBS_CloneVoidController>();
                    Clone.instance.PseudoRestartRun();
                }
            }
        }

        public class RBS_CloneVoidController : MonoBehaviour
        {
            public int itemInstancesLeftToGive = cfgStageCount.Value;
            public int itemCountPerInstance = -1;

            public void Awake()
            {
                if (itemCountPerInstance < 0)
                {
                    itemCountPerInstance = Util.GetItemCountForTeam(TeamIndex.Player, instance.ItemDef.itemIndex, false);
                }
            }

            public void OnEnable()
            {
                Stage.onServerStageBegin += OnServerStageBegin;
            }

            public void OnDisable()
            {
                Stage.onServerStageBegin -= OnServerStageBegin;
                Destroy(this);
            }

            public void OnServerStageBegin(Stage stage)
            {
                if (itemInstancesLeftToGive < 0)
                {
                    enabled = false;
                    return;
                }
                var itemGift = GetRandomItems(itemCountPerInstance, false, false);
                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    foreach (var item in itemGift)
                    {
                        player.master.inventory.GiveItem(item);
                    }
                }
                itemInstancesLeftToGive--;
            }

            public List<ItemIndex> GetRandomItems(int count, bool lunarEnabled, bool voidEnabled)
            {
                if (!NetworkServer.active)
                {
                    Debug.LogWarning("[Server] function 'CloneVoid::GiveRandomItems(System.Int32,System.Boolean,System.Boolean)' called on client");
                    return null;
                }
                List<ItemIndex> valueToReturn = new List<ItemIndex>();
                try
                {
                    if (count > 0)
                    {
                        
                        WeightedSelection<List<PickupIndex>> weightedSelection = new WeightedSelection<List<PickupIndex>>(8);
                        weightedSelection.AddChoice(Run.instance.availableTier1DropList, 100f);
                        weightedSelection.AddChoice(Run.instance.availableTier2DropList, 60f);
                        weightedSelection.AddChoice(Run.instance.availableTier3DropList, 4f);
                        if (lunarEnabled)
                        {
                            weightedSelection.AddChoice(Run.instance.availableLunarItemDropList, 4f);
                        }
                        if (voidEnabled)
                        {
                            weightedSelection.AddChoice(Run.instance.availableVoidTier1DropList, 4f);
                            weightedSelection.AddChoice(Run.instance.availableVoidTier1DropList, 2.3999999f);
                            weightedSelection.AddChoice(Run.instance.availableVoidTier1DropList, 0.16f);
                        }
                        for (int i = 0; i < count; i++)
                        {
                            List<PickupIndex> list = weightedSelection.Evaluate(UnityEngine.Random.value);
                            PickupDef pickupDef = PickupCatalog.GetPickupDef(list[UnityEngine.Random.Range(0, list.Count)]);
                            var resolvedItemIndex = (pickupDef != null) ? pickupDef.itemIndex : ItemIndex.None;
                            valueToReturn.Add(resolvedItemIndex);
                        }
                    }
                }
                catch (ArgumentException)
                {
                }
                return valueToReturn;
            }
        }
    }
}