using System;
using System.Collections.Generic;
using System.Text;

namespace RiskOfBulletstormRewrite.Utils
{
    internal class AvailabilityManager
    {
        public static void Initialize()
        {
            On.RoR2.EntityStateCatalog.Init += EntityStateCatalog_Init;
        }

        private static void EntityStateCatalog_Init(On.RoR2.EntityStateCatalog.orig_Init orig)
        {
            orig();
            AvailabilityManager.EntityStateMachine.availability.MakeAvailable();
        }

        public static class EntityStateMachine
        {
            public static ResourceAvailability availability = new ResourceAvailability();
        }
    }
}
