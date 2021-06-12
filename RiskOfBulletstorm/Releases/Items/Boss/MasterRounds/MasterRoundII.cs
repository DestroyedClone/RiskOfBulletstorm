using System.Collections.ObjectModel;
using RoR2;
using TILER2;
using static RiskOfBulletstorm.Items.MasterRoundController;

namespace RiskOfBulletstorm.Items
{
    public class MasterRoundII : Item<MasterRoundII>
    {
        public override string displayName => "Master Round II";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => GetItemPickupStr();

        protected override string GetDescString(string langid = null) => GetItemDescStr();

        protected override string GetLoreString(string langID = null) => "This potent artifact indicates mastery of the second chamber." +
            "\nAny who enter the Gungeon are doomed to remain, living countless lives in an effort to break the cycle.";
        public MasterRoundII()
        {
            iconResource = staticIcon;
        }
    }
}
