using System.Collections.ObjectModel;
using RoR2;
using TILER2;
using static RiskOfBulletstorm.Items.MasterRoundController;

namespace RiskOfBulletstorm.Items
{
    public class MasterRoundIII : Item<MasterRoundIII>
    {
        public override string displayName => "Master Round III";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => GetItemPickupStr();

        protected override string GetDescString(string langid = null) => GetItemDescStr();

        protected override string GetLoreString(string langID = null) => "This exceptional artifact indicates mastery of the third chamber." +
            "\nFew return from the deadly route that leads to the Forge. Yet fewer survive that venture into less-explored territory.";
    }
}
