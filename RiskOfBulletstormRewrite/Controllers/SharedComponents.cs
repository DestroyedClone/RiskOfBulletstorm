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
        }
    }
}