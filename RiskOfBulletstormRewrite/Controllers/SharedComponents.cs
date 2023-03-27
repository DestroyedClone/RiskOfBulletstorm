using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class SharedComponents
    {
        /// <summary>
        /// Component for the purpose of storing information regarding chests.
        /// <para><b>Trusty Lockpicks</b>: Affects hasUsedLockpicks </para>
        /// <para><b>Drill</b>: Can't interact with Lockpicked Chests</para>
        /// </summary>
        public class BulletstormChestInteractorComponent : MonoBehaviour
        {
            public bool hasUsedLockpicks = false;
            public string nameModifier = "";
            public string contextModifier = "";
            public PurchaseInteraction purchaseInteraction;
            public ChestBehavior chestBehavior;

            private readonly string attemptContextToken = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT";
            private readonly string failContextToken = "RISKOFBULLETSTORM_LOCKPICKS_CONTEXT_ATTEMPT_LOSE";

            public bool InteractorHasValidEquipment(Interactor interactor)
            {
                if (interactor.TryGetComponent<CharacterBody>(out CharacterBody characterBody)
                    && characterBody.equipmentSlot)
                {
                    if (characterBody.equipmentSlot.equipmentIndex == Equipment.TrustyLockpicks.Instance.EquipmentDef.equipmentIndex
                        || characterBody.equipmentSlot.equipmentIndex == Equipment.Drill.Instance.EquipmentDef.equipmentIndex)
                    {
                        return true;
                    }
                }
                return false;
            }

            public string GetContextualString(string original)
            {
                string formattingToken = hasUsedLockpicks ? failContextToken : attemptContextToken;
                return Language.GetStringFormatted(formattingToken, original);
            }

            public void UpdateTokens()
            {
            }
        }
    }
}