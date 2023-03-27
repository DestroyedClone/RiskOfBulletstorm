using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using UnityEngine.Networking;
using System.Collections.Generic;
using RiskOfBulletstormRewrite.Modules;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class BrickOfCash : EquipmentBase<BrickOfCash>
    {
        //public static ConfigEntry<float> cfg;

        public override string EquipmentName => "Brick of Cash";

        public override string EquipmentLangTokenName => "BRICKOFCASH";

        public override GameObject EquipmentModel => Assets.NullModel;

        public override Sprite EquipmentIcon => LoadSprite();

        public static GameObject CashProjectile = null;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
            CreateAssets();
        }

        public void CreateAssets()
        {
        }

        public override void Hooks()
        {
            On.RoR2.ChestBehavior.Start += ChestBehavior_Start;
            On.RoR2.PressurePlateController.Start += PressurePlateController_Start;
            On.RoR2.Inventory.SetEquipmentInternal += Inventory_SetEquipmentInternal;

            //instance tracking
            On.RoR2.ChestBehavior.Start += AddChestBehavior;
            On.RoR2.PressurePlateController.Start += AddPressurePlateController;
        }

        private void AddPressurePlateController(On.RoR2.PressurePlateController.orig_Start orig, PressurePlateController self)
        {
            orig(self);
            InstanceTracker.Add(self);
            if (!self.GetComponent<BulletstormInstanceTrackerRemover>())
                self.gameObject.AddComponent<BulletstormInstanceTrackerRemover>().pressurePlateController = self;
        }

        private void AddChestBehavior(On.RoR2.ChestBehavior.orig_Start orig, ChestBehavior self)
        {
            orig(self);
            InstanceTracker.Add(self);
            if (!self.GetComponent<BulletstormInstanceTrackerRemover>())
                self.gameObject.AddComponent<BulletstormInstanceTrackerRemover>().chestBehavior = self;
        }

        public class BulletstormInstanceTrackerRemover : MonoBehaviour
        {
            public ChestBehavior chestBehavior;
            public PressurePlateController pressurePlateController;

            public void OnDestroy()
            {
                if (chestBehavior)
                    InstanceTracker.Remove(chestBehavior);
                if (pressurePlateController)
                    InstanceTracker.Remove(pressurePlateController);
            }
        }

        private bool Inventory_SetEquipmentInternal(On.RoR2.Inventory.orig_SetEquipmentInternal orig, Inventory self, EquipmentState equipmentState, uint slot)
        {
            bool isCash = equipmentState.equipmentDef == EquipmentDef;
            var original = orig(self, equipmentState, slot);
            if (isCash && original)
            {
                RatOut();
            }
            return original;
        }

        private void RatOut()
        {
            foreach (var chestBehavior in InstanceTracker.GetInstancesList<ChestBehavior>())
            {
                if (ChestCheck(chestBehavior))
                {
                    CreateIndicator(chestBehavior.transform);
                }
            }
            foreach (var pressurePlateController in InstanceTracker.GetInstancesList<PressurePlateController>())
            {
                CreateIndicator(pressurePlateController.transform);
            }
        }

        private void PressurePlateController_Start(On.RoR2.PressurePlateController.orig_Start orig, PressurePlateController self)
        {
            orig(self);
            if (PlayersHaveBrickOfCash())
                CreateIndicator(self.transform);
        }

        private void ChestBehavior_Start(On.RoR2.ChestBehavior.orig_Start orig, ChestBehavior self)
        {
            orig(self);
            if (ChestCheck(self) && PlayersHaveBrickOfCash())
            {
                CreateIndicator(self.transform);
            }
        }

        private bool ChestCheck(ChestBehavior chestBehavior)
        {
            var comp = chestBehavior.GetComponent<PurchaseInteraction>();
            if (comp)
            {
                if (comp.displayNameToken == "CHEST1_STEALTHED_NAME"
                    || comp.displayNameToken == "GOLDCHEST_NAME")
                {
                    return true;
                }
            }
            return false;
        }

        private bool PlayersHaveBrickOfCash()
        {
            return Utils.ItemHelpers.GetAtLeastOneEquipmentForTeam(TeamIndex.Player, BrickOfCash.Instance.EquipmentDef.equipmentIndex, true);
        }

        private void CreateIndicator(Transform parent)
        {
            if (!NetworkServer.active) return;
            ChestRevealer.PendingReveal item = new ChestRevealer.PendingReveal
            {
                gameObject = parent.gameObject,
                time = Run.FixedTimeStamp.now,
                duration = Mathf.Infinity
            };
            ChestRevealer.pendingReveals.Add(item);
        }

        protected override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            var aimRay = slot.GetAimRay();
            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(EquipmentDef.equipmentIndex),
                aimRay.origin, aimRay.direction * 20f);
            slot.characterBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
            CharacterMasterNotificationQueue.PushEquipmentTransformNotification(slot.characterBody.master, slot.characterBody.inventory.currentEquipmentIndex, EquipmentIndex.None, CharacterMasterNotificationQueue.TransformationType.Default);
            return true;
        }

        private void ThrowBrick(EquipmentSlot slot)
        {
            if (slot && slot.equipmentIndex != EquipmentIndex.None)
            {
                slot.equipmentIndex = EquipmentIndex.None;
            }
        }
    }
}
