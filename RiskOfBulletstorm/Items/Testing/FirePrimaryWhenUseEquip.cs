
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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RiskOfBulletstorm.Items
{
    public class FirePrimaryOnEquip : Item_V2<FirePrimaryOnEquip>
    {
        public override string displayName => "UsePrimaryOnEquipUse";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

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
            var characterBody = self.characterBody;
            if (characterBody)
            {
                var inventory = characterBody.inventory;
                if (inventory && inventory.GetItemCount(catalogIndex) > 0)
                {
                    var instances = PlayerCharacterMasterController.instances;
                    foreach (PlayerCharacterMasterController playerCharacterMaster in instances)
                    {
                        //var master = playerCharacterMaster.master;
                        //var body = master.GetBody();
                        var bodyInputs = playerCharacterMaster.bodyInputs;
                        bodyInputs.skill1.PushState(true);
                        Debug.Log("Set "+ playerCharacterMaster.GetDisplayName() + "'s skill1 ("+ bodyInputs.skill1 +") to "+ bodyInputs.skill1.down);
                    }
                }
            }
            orig(self);
        }
    }
}
