﻿using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class SharedComponents
    {
        public class BulletstormChestInteractorComponent : MonoBehaviour
        {
            public bool hasUsedLockpicks = false;
        }

    }
}