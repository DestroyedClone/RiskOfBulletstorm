using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactEquipmentRecharge : ArtifactBase<ArtifactEquipmentRecharge>
    {
        public override string ArtifactName => "Artifact of Equipment Battery";

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
            GlobalEventManager.onServerDamageDealt += ReduceWaitOnDamage;
        }

        public void ReduceWaitOnDamage(DamageReport damageReport)
        {
            if (damageReport.attackerBody && damageReport.attackerBody.inventory && damageReport.attackerBody.equipmentSlot)
            {
                var body = damageReport.attackerBody;
                //var activeEqp = body.inventory.GetEquipment((uint)body.inventory.activeEquipmentSlot);
                var damageDealt = damageReport.damageDealt;
                //test: 100 damage = 1s of charge
                var resultingCooldownReduction = damageDealt / 100f;
                body.inventory.DeductActiveEquipmentCooldown(resultingCooldownReduction);
            }
        }

        private void EquipmentSlot_UpdateInventory(On.RoR2.EquipmentSlot.orig_UpdateInventory orig, EquipmentSlot self)
        {
            if (self.inventory)
            {
                //self._rechargeTime = Run.FixedTimeStamp.now;
                var eqp = self.inventory.GetEquipment((uint)self.inventory.activeEquipmentSlot);
                //IDK how to make it unchanging
                //so I'll just add the time back every time.
                eqp.chargeFinishTime += Time.fixedDeltaTime;
            }
            orig(self);
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