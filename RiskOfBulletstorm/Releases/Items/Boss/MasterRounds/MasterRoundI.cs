using System.Collections.ObjectModel;
using RoR2;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class MasterRoundI : Item<MasterRoundI>
    {
        public override string displayName => "Master Round I";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "This rare artifact indicates mastery of the first chamber." +
            "\nApocryphal texts recovered from cultists of the Order indicate that the Gun and the Bullet are linked somehow.";

    }
}
