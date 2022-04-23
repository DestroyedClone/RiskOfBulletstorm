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
    public class Clone : ItemBase<Clone>
    {
        public override string ItemName => "Clone";

        public override string ItemLangTokenName => "CLONE";

        public override ItemTier Tier => ItemTier.Tier3;

        public override GameObject ItemModel => GetPickupModel();

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist
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
                    PseudoRestartRun(peopleWithClones[0]);
                }
            }
        }

        private void PseudoRestartRun(CharacterMaster masterWithClone)
        {
            if (!masterWithClone) return;
            masterWithClone.inventory.RemoveItem(ItemDef);
            isCloneRestarting = true;
            Run.instance.SetRunStopwatch(0);
            Run.instance.NetworkstageClearCount = -1;
            TeamManager.instance.SetTeamLevel(TeamIndex.Player, 1);
            TeamManager.instance.SetTeamLevel(TeamIndex.Monster, 1);
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

            Run.instance.PickNextStageScene(weightedSelection);
            Run.instance.AdvanceStage(Run.instance.nextStageScene);
        }
    }
}