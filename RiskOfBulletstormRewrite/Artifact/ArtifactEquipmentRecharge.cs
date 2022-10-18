using BepInEx.Configuration;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactEquipmentRecharge : ArtifactBase<ArtifactEquipmentRecharge>
    {
        public override string ArtifactName => "Artifact of Equipment Battery";

        public override string ArtifactLangTokenName => "DAMAGEEQUIPMENTRECHARGE";

        public override Sprite ArtifactEnabledIcon => LoadSprite(true);

        public override Sprite ArtifactDisabledIcon => LoadSprite(false);

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public const float damagePerSecond = 50f;
        public const float alternateEquipmentDecreaseMultiplier = 0.5f;
        public float stageDamagePerSecond
        {
            get
            {
                if (ArtifactGungeonSpoof.instance.ArtifactEnabled)
                {
                    var spoofDamage = damagePerSecond * 0.5f;
                    return Mathf.Max(spoofDamage *
                Run.instance.stageClearCount, spoofDamage);
                }
                return Mathf.Max(damagePerSecond *
                Run.instance.stageClearCount, damagePerSecond);
            }
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
                var resultingCooldownReduction = damageDealt / stageDamagePerSecond;
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
            public List<float> remainingDurations = new List<float>();


            public void DeductCooldown(float duration)
            {
                for (int i = 0; i < remainingDurations.Count; i++)
                {
                    if (i == inventory.activeEquipmentSlot)
                        remainingDurations[i] = Mathf.Max(remainingDurations[i] - duration, 0);
                    else
                        remainingDurations[i] = Mathf.Max(remainingDurations[i] - duration * alternateEquipmentDecreaseMultiplier, 0);
                }
                inventory.HandleInventoryChanged();
            }

            public void Start()
            {
                //primaryEquipment = inventory.GetEquipment((uint)inventory.activeEquipmentSlot);
                inventory.onInventoryChanged += UpdateInventory;
                UpdateInventory();
            }

            public void UpdateInventory()
            {
            }

            public void OnDestroy()
            {
                inventory.onInventoryChanged -= UpdateInventory;
            }

            public void FixedUpdate()
            {
                if (inventory.equipmentStateSlots == null)
                {
                    return;
                }
                
                var invlength = inventory.equipmentStateSlots.Length;
                
                //for matching the equipment list
                var durlength = remainingDurations.Count;
                while (durlength < invlength)
                {
                    //var timeLeft = inventory.equipmentStateSlots[durlength++].chargeFinishTime.timeUntil;
                    durlength++;
                    remainingDurations.Add(0);
                }

                float normalCooldownMax = 1f;
                for (int i = 0; i < durlength; i++)
                {
                    EquipmentState equipment = inventory.GetEquipment((uint)i);
                    if (remainingDurations[i] <= 0)
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