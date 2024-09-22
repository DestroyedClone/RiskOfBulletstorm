using System;
using System.Collections.Generic;
using System.Text;

namespace RiskOfBulletstormRewrite.Utils
{
    internal class AvailabilityManager
    {
        public static void Initialize()
        {
            On.RoR2.EntityStateCatalog.Init += EntityStateCatalog_Init1;
        }

        private static System.Collections.IEnumerator EntityStateCatalog_Init1(On.RoR2.EntityStateCatalog.orig_Init orig)
        {
            AvailabilityManager.EntityStateMachine.availability.MakeAvailable();
            return orig();
        }

        public static class EntityStateMachine
        {
            public static ResourceAvailability availability = new ResourceAvailability();
        }
    }
}
