
using System.Collections.Generic;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;

namespace RiskOfBulletstorm.Items
{
    public class RingTriggers : Item_V2<RingTriggers>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius? (Value: Degrees, Min: 0, Max 360)", AutoConfigFlags.PreventNetMismatch)]
        public float RingTriggers_Radius { get; private set; } = 360f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many times to fire within the radius? (Min: 0)", AutoConfigFlags.PreventNetMismatch)]
        public float RingTriggers_FireAmount { get; private set; } = 360f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many seconds should Ring of Triggers fire?", AutoConfigFlags.PreventNetMismatch)]
        public float RingTriggers_Duration { get; private set; } = 3f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional seconds should Ring of Triggers fire per stack?", AutoConfigFlags.PreventNetMismatch)]
        public float RingTriggers_DurationStack { get; private set; } = 1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What should the cooldown in seconds for the Ring of Triggers' ability??", AutoConfigFlags.PreventNetMismatch)]
        public float RingTriggers_Cooldown { get; private set; } = 10f;
        public override string displayName => "Ring of Triggers";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.EquipmentRelated });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Items == Guns\nSummons bullets on active item use.";

        protected override string GetDescString(string langid = null) => $"Using your <style=cIsDamage>equipment</style> fires your firstmost combat skill " +
            $"{RingTriggers_FireAmount} times within {RingTriggers_Radius} degrees for {RingTriggers_Duration} seconds +{RingTriggers_DurationStack} per stack";

        protected override string GetLoreString(string langID = null) => "This ring bestows upon its bearer the now obvious knowledge that within the walls of the Gungeon all items are actually guns in strange, distorted form. An artifact of the Order's belief in transgunstantiation.";

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.EquipmentSlot.Execute += EquipmentSlot_Execute;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.EquipmentSlot.Execute -= EquipmentSlot_Execute;
        }
        private void EquipmentSlot_Execute(On.RoR2.EquipmentSlot.orig_Execute orig, EquipmentSlot self)
        {
            if (NetworkServer.active)
            {
                Debug.Log("Ring of Triggers: Entered execute");
                var characterBody = self.characterBody;
                if (characterBody)
                {
                    Debug.Log("Ring of Triggers: Entered cb");
                    var inputBank = characterBody.inputBank;
                    if (inputBank)
                    {
                        Debug.Log("Ring of Triggers: Entered bank");
                        var skill1 = inputBank.skill1;
                        var wasPressed = skill1.down;
                        var lastDirection = inputBank.aimDirection;
                        var segment = RingTriggers_Radius / Mathf.Max(RingTriggers_FireAmount,1);
                        for (int i = 0; i < RingTriggers_FireAmount; i++)
                        {
                            Debug.Log("Ring of Triggers: fire");
                            skill1.PushState(true);
                            inputBank.aimDirection = new Vector3(inputBank.aimDirection.x, segment*(i+1), inputBank.aimDirection.y);
                        }
                        skill1.PushState(wasPressed);
                        inputBank.aimDirection = lastDirection;
                    }
                }
            }
            orig(self);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Engine")]
        public class RingTriggersComponent : MonoBehaviour
        {
            public float cooldown = instance.RingTriggers_Cooldown;
            private float lifetime = 0f;
            private bool isReady = true;

            void FixedUpdate()
            {
                if (lifetime >= 0)
                    lifetime -= Time.deltaTime;
                else
                {
                    isReady = true;
                }
            }


        }
    }
}
