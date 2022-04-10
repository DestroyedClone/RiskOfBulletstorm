using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class BrickOfCash : EquipmentBase<BrickOfCash>
    {
        public static ConfigEntry<float> cfg;

        public override string EquipmentName => "Brick of Cash";

        public override string EquipmentLangTokenName => "BRICKOFCASH";

        public override GameObject EquipmentModel => Assets.NullModel;

        public override Sprite EquipmentIcon => Assets.NullSprite;

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

        }

        private void PressurePlateController_Start(On.RoR2.PressurePlateController.orig_Start orig, PressurePlateController self)
        {
            orig(self);
            if (Utils.ItemHelpers.GetAtLeastOneEquipmentForTeam(TeamIndex.Player, BrickOfCash.instance.EquipmentDef.equipmentIndex, false))
                CreateIndicator(self.transform);
        }

        private void ChestBehavior_Start(On.RoR2.ChestBehavior.orig_Start orig, ChestBehavior self)
        {
            orig(self);
            var comp = self.GetComponent<PurchaseInteraction>();
            if (comp)
            {
                if (comp.displayNameToken == "CHEST1_STEALTHED_NAME")
                {
                    if (Utils.ItemHelpers.GetAtLeastOneEquipmentForTeam(TeamIndex.Player, BrickOfCash.instance.EquipmentDef.equipmentIndex, false))
                        CreateIndicator(comp.transform);
                }
            }
        }

        private void CreateIndicator(Transform parent)
        {
            if (!NetworkServer.active) return;
            var prefab = LegacyResourcesAPI.Load<GameObject>("RoR2/Base/Common/PoiPositionIndicator");
            var copy = Object.Instantiate(prefab, parent.position, Quaternion.identity, parent);
            copy.GetComponent<PositionIndicator>().targetTransform = parent;
            NetworkServer.Spawn(copy);
            UnityEngine.Object.Destroy(copy);
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
            return false;
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
