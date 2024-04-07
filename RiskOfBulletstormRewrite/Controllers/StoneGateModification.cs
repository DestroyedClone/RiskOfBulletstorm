using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RiskOfBulletstormRewrite.Controllers.SharedComponents;
using RoR2;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Controllers
{
    internal class StoneGateModification
    {
        public static bool srv_isGoolake;
        public static GameObject DoorUnlockable;
        public static GameObject srv_goolakeLockInstance;

        public static void Init()
        {
            CreateDoorLock();
            Stage.onServerStageBegin += UpdateGooLakePerm;
            On.RoR2.BarrelInteraction.OnInteractionBegin += BarrelInteraction_OnInteractionBegin;
            On.RoR2.PressurePlateController.Start += PressurePlateController_Start;
            On.EntityStates.Interactables.StoneGate.Opening.OnEnter += Opening_OnEnter;
        }

        private static void Opening_OnEnter(On.EntityStates.Interactables.StoneGate.Opening.orig_OnEnter orig, EntityStates.Interactables.StoneGate.Opening self)
        {
            orig(self);
            if (RBSStoneGateLockInteraction.Instance)
                UnityEngine.Object.Destroy(RBSStoneGateLockInteraction.Instance.gameObject);
        }

        private static void PressurePlateController_Start(On.RoR2.PressurePlateController.orig_Start orig, PressurePlateController self)
        {
            orig(self);
            self.gameObject.AddComponent<RBSPressurePlateController>().pressurePlateController = self;
        }

        private static void CreateDoorLock()
        {
            var barrel1 = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Barrel1/Barrel1.prefab").WaitForCompletion();
            DoorUnlockable = PrefabAPI.InstantiateClone(barrel1, "RBS_GOOLAKEDOORLOCK");
            var bar = DoorUnlockable.GetComponent<BarrelInteraction>();
            bar.displayNameToken = "RISKOFBULLETSTORM_STONEGATE_NAME";
            bar.contextToken = "RISKOFBULLETSTORM_STONEGATE_CONTEXT";
            bar.goldReward = 0;
            DoorUnlockable.AddComponent<RBSStoneGateLockInteraction>();
            var lockRef = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/MetalLock.prefab");
            var lockBody = UnityEngine.Object.Instantiate(lockRef, DoorUnlockable.transform.Find("mdlBarrel1"));
            lockBody.transform.localPosition = Vector3.zero;
            lockBody.transform.rotation = Quaternion.Euler(270, 0 ,0);
            //DoorUnlockable.transform.Find("mdlBarrel1/BarrelMesh").gameObject.SetActive(false);
            DoorUnlockable.GetComponent<Highlight>().targetRenderer = lockBody.GetComponent<MeshRenderer>();

            //var smr = DoorUnlockable.transform.Find("mdlBarrel1/BarrelMesh").GetComponent<SkinnedMeshRenderer>();
            //smr.SetMaterial(Modules.Assets.mainAssetBundle.LoadAsset<Material>("Assets/Materials/MetalMat.mat"));
            //smr.sharedMesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("Assets/Models/LockModel.fbx");
            //var comp = DoorUnlockable.AddComponent<RBSChestLockInteraction>();
            //comp.StoreHighlightColor(DoorUnlockable.GetComponent<Highlight>());
        }

        private static void UpdateGooLakePerm(Stage obj)
        {
            srv_isGoolake = obj && obj.sceneDef && obj.sceneDef.cachedName == "goolake";
            if (srv_goolakeLockInstance) return;
            if (!NetworkServer.active) return;
            var copy = UnityEngine.Object.Instantiate(DoorUnlockable);

            copy.transform.position = new Vector3(145.1675f, -97.3314f, -340.2334f);
            copy.transform.rotation = Quaternion.identity;

            NetworkServer.Spawn(copy);
            srv_goolakeLockInstance = copy;
        }

        private static void BarrelInteraction_OnInteractionBegin(On.RoR2.BarrelInteraction.orig_OnInteractionBegin orig, BarrelInteraction self, Interactor activator)
        {
            if (srv_goolakeLockInstance)
            {
                if (!self.TryGetComponent(out RBSStoneGateLockInteraction gateLock)) 
                    goto EarlyReturn;
                return;
            }
        EarlyReturn:
            orig(self, activator);
            //onBarrelInteraction?.Invoke(self, activator);
        }

        public class RBSStoneGateLockInteraction : RBSLockInteraction
        {
            public static RBSStoneGateLockInteraction Instance { get; set; }
            public void Awake()
            {
                Instance = this;
            }
            public void OnDestroy()
            {
                Instance = null;
            }
            public void OpenStoneGate()
            {
                if (isLockBroken) return;
                foreach (var plate in InstanceTracker.GetInstancesList<RBSPressurePlateController>())
                {
                    plate.PushPlate();
                }
                OpenBarrel();
                isLockBroken = true;
            }
            void OpenBarrel()
            {
                var barrelInteraction = this.GetComponent<BarrelInteraction>();
                barrelInteraction.Networkopened = true;
                EntityStateMachine component = base.GetComponent<EntityStateMachine>();
                if (component)
                {
                    component.SetNextState(new EntityStates.Barrel.Opening());
                }
            }
        }

        public class RBSPressurePlateController : MonoBehaviour
        {
            public PressurePlateController pressurePlateController;

            public void PushPlate()
            {
                pressurePlateController.SetSwitch(true);
            }

            public void Start()
            {
                InstanceTracker.Add(this);
            }

            public void OnDestroy()
            {
                InstanceTracker.Remove(this);
            }
        }
    }
}
