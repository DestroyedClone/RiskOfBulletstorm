using RoR2;
using BepInEx;
using UnityEngine;


namespace RiskOfBulletstormRewrite
{
    public abstract class PastInfoBase
    {
        public abstract GameObject BodyPrefab { get; }
        public abstract BodyIndex BodyIndex { get; set; }

        public struct PastInfo
        {
            
        }
    }
}