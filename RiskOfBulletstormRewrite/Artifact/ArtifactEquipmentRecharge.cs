using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using UnityEngine.Networking;
using System.Collections.Generic;

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

        public const float alternateEquipmentDecreaseMultiplier = 0.5f;

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
            //On.RoR2.EquipmentSlot.UpdateInventory += EquipmentSlot_UpdateInventory;
            GlobalEventManager.onServerDamageDealt += ReduceWaitOnDamage;
            CharacterMaster.onStartGlobal += GiveComponent;
            On.RoR2.Inventory.DeductActiveEquipmentCooldown += DeductFromComponent;
        }

        private void RunArtifactManager_onArtifactDisabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            //On.RoR2.EquipmentSlot.UpdateInventory -= EquipmentSlot_UpdateInventory;
            GlobalEventManager.onServerDamageDealt -= ReduceWaitOnDamage;
            CharacterMaster.onStartGlobal -= GiveComponent;
            On.RoR2.Inventory.DeductActiveEquipmentCooldown -= DeductFromComponent;
        }
        
        public void DeductFromComponent(On.RoR2.Inventory.orig_DeductActiveEquipmentCooldown orig, Inventory self, float seconds)
        {
            self.GetComponent<RBS_ArtifactEquipmentRechargeComponent>()?.DeductCooldown(seconds);
            orig(self, seconds);
        }

        public void GiveComponent(CharacterMaster characterMaster)
        {
            if (characterMaster.inventory)
            {
                var comp = characterMaster.gameObject.AddComponent<RBS_ArtifactEquipmentRechargeComponent>();
                comp.inventory = characterMaster.inventory;
            }
                
        }

        public void ReduceWaitOnDamage(DamageReport damageReport)
        {
            if (damageReport.attackerMaster && damageReport.attackerMaster.inventory)
            {
                var damageDealt = damageReport.damageDealt;
                //test: 100 damage = 1s of charge
                var resultingCooldownReduction = damageDealt / 100f;
                damageReport.attackerMaster.GetComponent<RBS_ArtifactEquipmentRechargeComponent>()?.DeductCooldown(resultingCooldownReduction);
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

        public class RBS_ArtifactEquipmentRechargeComponent : MonoBehaviour
        {
            public Inventory inventory = null;
            //public EquipmentState primaryEquipment = default;
            //public float remainingDuration = 0;
            List<float> remainingDurations = new List<float>();


            public void DeductCooldown(float duration)
            {
                for (int i = 0; i < remainingDurations.Count; i++)
                {
                    if (i == inventory.activeEquipmentSlot)
                        remainingDurations[i] -= duration;
                    else
                        remainingDurations[i] -=  duration * alternateEquipmentDecreaseMultiplier;
                }
                inventory.HandleInventoryChanged();
            }

            public void Start()
            {
                //primaryEquipment = inventory.GetEquipment((uint)inventory.activeEquipmentSlot);
            }

            public void FixedUpdate()
            {
                float normalCooldownMax = 1;
                for (int i = 0; i < remainingDurations.Count; i++)
                {
                    EquipmentState equipment = inventory.GetEquipment((uint)i);
                    if (remainingDurations[i] <= normalCooldownMax)
                    {
                        remainingDurations[i] = equipment.chargeFinishTime.timeUntil;
                    }
                    else {
                        inventory.SetEquipment(
                            new EquipmentState(equipment.equipmentIndex,
                            Run.FixedTimeStamp.now + remainingDurations[i],
                            equipment.charges), (uint)i
                        );
                    }
                }
            }
        }

    }
}