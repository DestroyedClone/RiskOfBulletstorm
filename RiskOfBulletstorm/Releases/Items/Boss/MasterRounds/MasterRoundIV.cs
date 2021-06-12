using System.Collections.ObjectModel;
using RoR2;
using TILER2;
using static RiskOfBulletstorm.Items.MasterRoundController;

namespace RiskOfBulletstorm.Items
{
    public class MasterRoundIV : Item<MasterRoundIV>
    {
        public override string displayName => "Master Round IV";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => GetItemPickupStr();

        protected override string GetDescString(string langid = null) => GetItemDescStr();

        protected override string GetLoreString(string langID = null) => "This extraordinary artifact indicates mastery of the fourth chamber." +
            "\nA monument to the legendary hero greets all who challenge the Gungeon, though their identity has been lost to the ages.";
        public MasterRoundIV()
        {
            iconResource = staticIcon;
        }
    }
}
