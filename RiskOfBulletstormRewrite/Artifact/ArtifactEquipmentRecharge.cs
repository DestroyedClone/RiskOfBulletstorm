using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactEquipmentRecharge : ArtifactBase<ArtifactEquipmentRecharge>
    {
        public override string ArtifactName => "Artifact of Linked Equipment";

        public override string ArtifactLangTokenName => "DAMAGEEQUIPMENTRECHARGE";

        public override Sprite ArtifactEnabledIcon => Assets.NullSprite;

        public override Sprite ArtifactDisabledIcon => Assets.NullSprite;

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public override void Hooks()
        {
            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;
        }

        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            On.RoR2.EquipmentSlot.UpdateInventory += EquipmentSlot_UpdateInventory;
            On.RoR2.EquipmentState.
        }

        private void EquipmentSlot_UpdateInventory(On.RoR2.EquipmentSlot.orig_UpdateInventory orig, EquipmentSlot self)
        {
            orig(self);
            if (self.inventory)
            {
                self._rechargeTime = 
            }
        }

        private void RunArtifactManager_onArtifactDisabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            
        }

        public class EquipmentSlotHolder : MonoBehaviour
        {
            public EquipmentSlot equipmentSlot;

            public void FixedUpdate()
            {

            }
        }

    }
}