using RiskOfBulletstormRewrite.Utils.Components;
using RoR2;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskOfBulletstormRewrite.Utils
{
    internal class ItemHelpers
    {
        /// <summary>
        /// A helper that will set up the RendererInfos of a GameObject that you pass in.
        /// <para>This allows it to go invisible when your character is not visible, as well as letting overlays affect it.</para>
        /// </summary>
        /// <param name="obj">The GameObject/Prefab that you wish to set up RendererInfos for.</param>
        /// <param name="debugmode">Do we attempt to attach a material shader controller instance to meshes in this?</param>
        /// <returns>Returns an array full of RendererInfos for GameObject.</returns>
        public static CharacterModel.RendererInfo[] ItemDisplaySetup(GameObject obj, bool debugmode = false)
        {
            List<Renderer> AllRenderers = new List<Renderer>();

            var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers.Length > 0) { AllRenderers.AddRange(meshRenderers); }

            var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderers.Length > 0) { AllRenderers.AddRange(skinnedMeshRenderers); }

            CharacterModel.RendererInfo[] renderInfos = new CharacterModel.RendererInfo[AllRenderers.Count];

            for (int i = 0; i < AllRenderers.Count; i++)
            {
                if (debugmode)
                {
                    var controller = AllRenderers[i].gameObject.AddComponent<MaterialControllerComponents.HGControllerFinder>();
                    controller.Renderer = AllRenderers[i];
                }

                renderInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = AllRenderers[i] is SkinnedMeshRenderer ? AllRenderers[i].sharedMaterial : AllRenderers[i].material,
                    renderer = AllRenderers[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false //We allow the mesh to be affected by overlays like OnFire or PredatoryInstinctsCritOverlay.
                };
            }

            return renderInfos;
        }

        public static string Pct(float value)
        {
            return $"{100 * value}%";
        }

        public static void GiveItemToPlayers(ItemDef itemDef, bool showInChat = true, int amount = 1)
        {
            var instances = PlayerCharacterMasterController.instances;
            foreach (PlayerCharacterMasterController playerCharacterMaster in instances)
            {
                var master = playerCharacterMaster.master;
                if (master)
                {
                    var body = playerCharacterMaster.body;
                    if (body)
                    {
                        var inventory = master.inventory;

                        if (inventory)
                        {
                            if (showInChat) SimulatePickup(master, itemDef, amount);
                            else inventory.GiveItem(itemDef, amount);
                        }
                    }
                }
            }
        }

        public static void SimulatePickup(CharacterMaster characterMaster, ItemDef itemDef, int amount = 1, bool showNotif = true)
        {
            var self = characterMaster.inventory;
            var pickupIndex = PickupCatalog.FindPickupIndex(itemDef.itemIndex);
            //var pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            //var nameToken = pickupDef.nameToken;
            //var color = pickupDef.baseColor;
            //var body = characterMaster.GetBody();

            if (pickupIndex > PickupIndex.none)
            {
                //Chat.AddPickupMessage(body, nameToken, color, (uint)amount);
                Util.PlaySound("Play_UI_item_pickup", characterMaster.GetBodyObject());

                self.GiveItem(itemDef, amount);
                GenericPickupController.SendPickupMessage(characterMaster, pickupIndex);

                if (showNotif)
                {
                    //TODO: Re-add
                    //var list = NotificationQueue.instancesList;
                    //list[0].OnPickup(characterMaster, pickupIndex);
                    /*for (int i = 0; i < list.Count; i++)
                    {
                        list[i].OnPickup(characterMaster, pickupIndex);
                    }*/
                }
            }
        }

        public static float GetHyperbolicValue(float baseAmount, float stackAmount)
        {
            return 1 - 1 / (1 + baseAmount * stackAmount);
        }

        public static float GetHyperbolicValue(float baseAmount, float stackAmount, int itemCount)
        {
            if (itemCount == 0)
                return 0;

            return 1 - 1 / (1 + baseAmount * (stackAmount * (itemCount - 1)));
        }

        public static int GetEquipmentCountForTeam(TeamIndex teamIndex, EquipmentIndex equipmentIndex, bool requiresAlive, bool requiresConnected = true)
        {
            int num = 0;
            ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
            int i = 0;
            int count = readOnlyInstancesList.Count;
            while (i < count)
            {
                CharacterMaster characterMaster = readOnlyInstancesList[i];
                if (characterMaster.teamIndex == teamIndex && (!requiresAlive || characterMaster.hasBody) && (!requiresConnected || !characterMaster.playerCharacterMasterController || characterMaster.playerCharacterMasterController.isConnected))
                {
                    //if (characterMaster.inventory.eq
                }
                i++;
            }
            return num;
        }

        public static bool GetAtLeastOneEquipmentForTeam(TeamIndex teamIndex, EquipmentIndex equipmentIndex, bool requiresAlive, bool requiresConnected = true)
        {
            ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
            int i = 0;
            int count = readOnlyInstancesList.Count;
            while (i < count)
            {
                CharacterMaster characterMaster = readOnlyInstancesList[i];
                if (characterMaster.teamIndex == teamIndex && (!requiresAlive || characterMaster.hasBody) && (!requiresConnected || !characterMaster.playerCharacterMasterController || characterMaster.playerCharacterMasterController.isConnected))
                {
                    if (characterMaster.inventory && characterMaster.inventory.currentEquipmentIndex == equipmentIndex)
                        return true;
                }
                i++;
            }
            return false;
        }

        //Squid.FireSpine
        public static void FireSquidShot(CharacterBody ownerBody, Vector3 origin, float damageCoefficient, float procCoefficient = 1)
        {
            BullseyeSearch enemyFinder = new BullseyeSearch();
            enemyFinder.viewer = ownerBody;
            enemyFinder.maxDistanceFilter = float.PositiveInfinity;
            enemyFinder.searchOrigin = origin;
            enemyFinder.searchDirection = Vector3.up;
            enemyFinder.sortMode = BullseyeSearch.SortMode.Distance;
            enemyFinder.teamMaskFilter = TeamMask.allButNeutral;
            enemyFinder.minDistanceFilter = 0f;
            enemyFinder.maxAngleFilter = 180;
            enemyFinder.filterByLoS = true;
            if (FriendlyFireManager.friendlyFireMode != FriendlyFireManager.FriendlyFireMode.Off)
            {
                enemyFinder.teamMaskFilter.RemoveTeam(ownerBody.teamComponent.teamIndex);
            }
            enemyFinder.RefreshCandidates();
            HurtBox hurtBox = enemyFinder.GetResults().FirstOrDefault<HurtBox>();
            if (hurtBox)
            {
                SquidOrb squidOrb = new SquidOrb();
                squidOrb.damageValue = ownerBody.damage * damageCoefficient;
                squidOrb.isCrit = Util.CheckRoll(ownerBody.crit, ownerBody.master);
                squidOrb.teamIndex = TeamComponent.GetObjectTeam(ownerBody.gameObject);
                squidOrb.attacker = ownerBody.gameObject;
                squidOrb.procCoefficient = procCoefficient;
                HurtBox hurtBox2 = hurtBox;
                if (hurtBox2)
                {
                    squidOrb.origin = origin;
                    squidOrb.target = hurtBox2;
                    OrbManager.instance.AddOrb(squidOrb);
                }
            }
        }

        public static T Load<T>(string assetPath)
        {
            var loadedAsset = Addressables.LoadAssetAsync<T>(assetPath).WaitForCompletion();
            return loadedAsset;
        }
    }
}