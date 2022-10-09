﻿using BepInEx.Configuration;
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
    public class Clone : ItemBase<Clone>
    {
        public static ConfigEntry<int> cfgItemsToKeep;
        public static ConfigEntry<int> cfgItemsToKeepPerStack;
        public override string ItemName => "Clone";

        public override string ItemLangTokenName => "CLONE";

        public override string[] ItemFullDescriptionParams => new string[]
        {
            cfgItemsToKeep.Value.ToString(),
            cfgItemsToKeepPerStack.Value.ToString()
        };

        public override ItemTier Tier => ItemTier.Tier3;

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

        public static bool isCloneRestarting = false;

        public static GameObject GetPickupModel()
        {
            var bearItemDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/Base/ExtraLife/ExtraLife.asset").WaitForCompletion();
            var bearPMP = bearItemDef.pickupModelPrefab;
            var clonePMP = PrefabAPI.InstantiateClone(bearPMP, "ClonePickupModelPrefab", false);
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
            cfgItemsToKeep = config.Bind(ConfigCategory, "Items To Keep", 5);
            cfgItemsToKeepPerStack = config.Bind(ConfigCategory, "Items To Keep Per Stack", 5);
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
                    //var cloneObject = new GameObject();
                    Stage.instance.gameObject.AddComponent<RBS_CloneController>();
                    //UnityEngine.Object.DontDestroyOnLoad(cloneObject);
                    PseudoRestartRun();
                }
            }
        }

        public class RBS_CloneController : MonoBehaviour
        {
            //public Xoroshiro128Plus rng;
            public Dictionary<NetworkUser, Inventory> playerInventories = new Dictionary<NetworkUser, Inventory>();
            public Dictionary<PlayerCharacterMasterController, List<KeyValuePair<ItemIndex, int>>> itemsToGive = new Dictionary<PlayerCharacterMasterController, List<KeyValuePair<ItemIndex, int>>>();
            public bool hasGivenItems = false;

            public void Awake()
            {
                //rng = new Xoroshiro128Plus(Run.instance.seed);
                AcquireItemsToReturn();
                Stage.onServerStageBegin += OnServerStageBegin;
            }

            public void OnDestroy()
            {
                Stage.onServerStageBegin -= OnServerStageBegin;
            }

            public void OnServerStageBegin(Stage stage)
            {
                if (hasGivenItems) return;
                hasGivenItems = true;

                GiveItemsToPlayers();
            }

            //todo optimize
            public void AcquireItemsToReturn()
            {
                var cloneCount = Util.GetItemCountForTeam(TeamIndex.Player, instance.ItemDef.itemIndex, false);
                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    var inv = player.master.inventory;
                    itemsToGive.Add(player, GetRandomItemsFromInventory(inv, cloneCount));
                    var ownItemCount = inv.GetItemCount(instance.ItemDef);
                    var tempInvGameObject = new GameObject();
                    inv.CopyItemsFrom(tempInvGameObject.AddComponent<Inventory>());
                    Destroy(tempInvGameObject);
                    
                    if (ownItemCount > 0)
                    {
                        //remove isnt needed since 
                        //inv.RemoveItem(instance.ItemDef, ownItemCount);
                        inv.GiveItem(CloneConsumed.instance.ItemDef, ownItemCount);
                        //since the stage changes it might not show up for hte player... todo check
                        CharacterMasterNotificationQueue.PushItemTransformNotification(player.master, instance.ItemDef.itemIndex, CloneConsumed.instance.ItemDef.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                    }
                }
            }

            public void GiveItemsToPlayers()
            {
                foreach (var pair in itemsToGive)
                {
                    foreach (var itemAmt in pair.Value)
                    {
                        pair.Key.master.inventory.GiveItem(itemAmt.Key, itemAmt.Value);
                    }
                }
                Destroy(this);
            }

            //basically an inventory
            public List<KeyValuePair<ItemIndex, int>> GetRandomItemsFromInventory(Inventory inventory, int cloneCount)
            {
                List<KeyValuePair<ItemIndex, int>> list = new List<KeyValuePair<ItemIndex, int>>();

                for (int i = 0; i < inventory.itemAcquisitionOrder.Count; i++)
                {
                    var currentItemIndex = inventory.itemAcquisitionOrder[i];
                    if (currentItemIndex == instance.ItemDef.itemIndex)
                        continue;
                    //no infinite clones, bozo
                    var currentItemDef = ItemCatalog.GetItemDef(currentItemIndex);
                    if (!currentItemDef)
                        continue;
                    //keeping hidden items are important: difficulty modifier items
                    //so are NoTiers, usually
                    if (currentItemDef?.hidden == true || currentItemDef.tier == ItemTier.Tier1)
                    {
                        list.Add(new KeyValuePair<ItemIndex, int>(currentItemIndex, 1));
                        continue;
                    }

                    var itemCount = UnityEngine.Random.RandomRangeInt(0, Mathf.Min(inventory.GetItemCount(currentItemIndex), cloneCount));
                    cloneCount -= itemCount;
                    list.Add(new KeyValuePair<ItemIndex, int>((ItemIndex)i, itemCount));
                    if (cloneCount <= 0)
                    {
                        break;
                    }
                }

                return list;
            }
        }

        private void PseudoRestartRun()//(CharacterMaster masterWithClone)
        {
            //if (!masterWithClone) return;
            isCloneRestarting = true;
            Run.instance.SetRunStopwatch(0);
            Run.instance.NetworkstageClearCount = -1;
            TeamManager.instance.SetTeamLevel(TeamIndex.Player, 0);
            TeamManager.instance.SetTeamLevel(TeamIndex.Monster, 0);
            TeamManager.instance.SetTeamLevel(TeamIndex.Lunar, 0);
            TeamManager.instance.SetTeamLevel(TeamIndex.Void, 0);
            WeightedSelection<SceneDef> weightedSelection = new WeightedSelection<SceneDef>(8);

            string @string = Run.cvRunSceneOverride.GetString();
            if (@string != "")
            {
                weightedSelection.AddChoice(SceneCatalog.GetSceneDefFromSceneName(@string), 1f);
            }
            else if (Run.instance.startingSceneGroup)
            {
                Run.instance.startingSceneGroup.AddToWeightedSelection(weightedSelection, new Func<SceneDef, bool>(Run.instance.CanPickStage));
            }
            else
            {
                for (int j = 0; j < Run.instance.startingScenes.Length; j++)
                {
                    if (Run.instance.CanPickStage(Run.instance.startingScenes[j]))
                    {
                        weightedSelection.AddChoice(Run.instance.startingScenes[j], 1f);
                    }
                }
            }
            Run.instance.PickNextStageScene(weightedSelection);

            //var choices = Run.instance.startingSceneGroup;

            //Run.instance.PickNextStageScene(weightedSelection);
            Run.instance.AdvanceStage(Run.instance.nextStageScene);
        }
    }
}