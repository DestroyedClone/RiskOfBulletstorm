//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
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
    public class GungeonBuffController : Item_V2<GungeonBuffController>
    {
        public override string displayName => "GungeonBuffController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";
        public BuffIndex Burn { get; private set; }
        public BuffIndex Poison { get; private set; }
        public BuffIndex Curse { get; private set; }
        public BuffIndex Stealth { get; private set; }
        public BuffIndex Petrification { get; private set; }
        public BuffIndex Anger { get; private set; }
        public BuffIndex Buffed { get; private set; }
        public BuffIndex BurnEnemy { get; private set; }
        public BuffIndex PoisonEnemy { get; private set; }
        public BuffIndex Charm { get; private set; }
        public BuffIndex Encheesed { get; private set; }
        public BuffIndex Fear { get; private set; }
        public BuffIndex Jammed { get; private set; }
        public BuffIndex Slow { get; private set; }
        public BuffIndex Freeze { get; private set; }
        public BuffIndex Stun { get; private set; }
        public BuffIndex Weakened { get; private set; }
        public BuffIndex Tangled { get; private set; }
        public BuffIndex Encircled { get; private set; }
        public BuffIndex Glittered { get; private set; }
        public BuffIndex Bloody { get; private set; }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        //https://enterthegungeon.gamepedia.com/Status_Effects
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            var angerBuff = new CustomBuff(
            new BuffDef
            {
                buffColor = Color.red,
                canStack = false,
                isDebuff = false,
                name = "Enraged",
            });
            Anger = BuffAPI.Add(angerBuff);
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}
