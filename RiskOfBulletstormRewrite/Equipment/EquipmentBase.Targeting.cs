using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public abstract partial class EquipmentBase
    {
        public GameObject TargetingIndicatorPrefabBase = null;

        public virtual void FilterTargetFinderHurtbox(EquipmentSlot slot, BullseyeSearch targetFinder) { }

        private void EquipmentSlot_UpdateTargets(On.RoR2.EquipmentSlot.orig_UpdateTargets orig, EquipmentSlot self, EquipmentIndex targetingEquipmentIndex, bool userShouldAnticipateTarget)
        {
            //https://github.com/ThinkInvis/RoR2-TinkersSatchel/blob/35a9445e2cacfac2d577590b378a45b4239689bd/Items/LunarEqp/MonkeysPaw.cs#L215-L217
            if (targetingEquipmentIndex != EquipmentDef.equipmentIndex)
            {
                orig(self, targetingEquipmentIndex, userShouldAnticipateTarget);
                return;
            }

            ConfigureTargetFinder(self);
            bool hasTargetTransform = self.currentTarget.transformToIndicateAt;
            if (hasTargetTransform)
            {
                GenericPickupController pickupController = self.currentTarget.pickupController;
                ConfigureTargetIndicator(self, targetingEquipmentIndex, pickupController, ref hasTargetTransform);
            }
            self.targetIndicator.active = hasTargetTransform && self.stock > 0;
            self.targetIndicator.targetTransform = (hasTargetTransform ? self.currentTarget.transformToIndicateAt : null);
        }

        public enum TargetFinderType
        {
            Enemies,
            BossesWithRewards,
            Friendlies,
            Pickups,
            Custom,
            None
        }

        public virtual TargetFinderType EquipmentTargetFinderType { get; } = TargetFinderType.None;

        protected void ConfigureTargetFinder(EquipmentSlot slot)
        {
            if (EquipmentTargetFinderType == TargetFinderType.Enemies
                || EquipmentTargetFinderType == TargetFinderType.BossesWithRewards
                || EquipmentTargetFinderType == TargetFinderType.Friendlies)
            {
                if (EquipmentTargetFinderType == TargetFinderType.Enemies || EquipmentTargetFinderType == TargetFinderType.BossesWithRewards)
                {
                    slot.ConfigureTargetFinderForEnemies();
                }
                if (EquipmentTargetFinderType == TargetFinderType.Friendlies)
                {
                    slot.ConfigureTargetFinderForFriendlies();
                }
                HurtBox source = null;
                if (EquipmentTargetFinderType == TargetFinderType.BossesWithRewards)
                {
                    using (IEnumerator<HurtBox> enumerator = slot.targetFinder.GetResults().GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            HurtBox hurtBox = enumerator.Current;
                            if (hurtBox && hurtBox.healthComponent && hurtBox.healthComponent.body)
                            {
                                DeathRewards component = hurtBox.healthComponent.body.gameObject.GetComponent<DeathRewards>();
                                if (component && component.bossDropTable && !hurtBox.healthComponent.body.HasBuff(RoR2Content.Buffs.Immune))
                                {
                                    source = hurtBox;
                                    break;
                                }
                            }
                        }
                        goto IL_134;
                    }
                }
                FilterTargetFinderHurtbox(slot, slot.targetFinder);
                source = slot.targetFinder.GetResults().FirstOrDefault<HurtBox>();
            IL_134:
                slot.currentTarget = new EquipmentSlot.UserTargetInfo(source);
            }
            else if (EquipmentTargetFinderType == TargetFinderType.Pickups)
            {
                slot.currentTarget = new EquipmentSlot.UserTargetInfo(slot.FindPickupController(slot.GetAimRay(), 10f, 30f, true, slot.equipmentIndex == EquipmentDef.equipmentIndex));
            }
            else if (EquipmentTargetFinderType == TargetFinderType.Custom)
            {
                ConfigureTargetFinderCustom(slot);
                SetCurrentTargetCustom(slot);
            }
            else
            {
                slot.currentTarget = default;
            }
        }

        protected virtual void ConfigureTargetFinderCustom(EquipmentSlot equipmentSlot)
        {
            RiskOfBulletstormRewrite.Main._logger.LogWarning($"EquipmentBase.Targeting ConfigureTargetFinderCustom isn't set!");
        }

        protected virtual void SetCurrentTargetCustom(EquipmentSlot equipmentSlot)
        {
            RiskOfBulletstormRewrite.Main._logger.LogWarning($"EquipmentBase.Targeting SetCurrentTargetCustom isn't set!");
            equipmentSlot.currentTarget = new EquipmentSlot.UserTargetInfo(equipmentSlot.targetFinder.GetResults().FirstOrDefault<HurtBox>());
        }

        protected virtual void ConfigureTargetIndicator(EquipmentSlot equipmentSlot, EquipmentIndex targetingEquipmentIndex, GenericPickupController genericPickupController, ref bool shouldShowOverride)
        {
            equipmentSlot.targetIndicator.visualizerPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/BossHunterIndicator");
        }

        public virtual bool ShouldAnticipateTarget(EquipmentSlot equipmentSlot)
        {
            if (EquipmentTargetFinderType == TargetFinderType.None || EquipmentTargetFinderType == TargetFinderType.Pickups)
            {
                return false;
            }
            return equipmentSlot.stock > 0;
        }
    }
}