using System.Collections.ObjectModel;
using RoR2;
using TILER2;
using static RiskOfBulletstorm.Items.MasterRoundController;
using static RiskOfBulletstorm.BulletstormPlugin;
using UnityEngine;

namespace RiskOfBulletstorm.Items
{
    public class MasterRoundI : Item<MasterRoundI>
    {
        public override string displayName => "Master Round I";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => GetItemPickupStr();

        protected override string GetDescString(string langid = null) => GetItemDescStr();

        protected override string GetLoreString(string langID = null) => "This rare artifact indicates mastery of the first chamber." +
            "\nApocryphal texts recovered from cultists of the Order indicate that the Gun and the Bullet are linked somehow.";

        public MasterRoundI()
        {
            iconResource = staticIcon;
        }
    }
}
