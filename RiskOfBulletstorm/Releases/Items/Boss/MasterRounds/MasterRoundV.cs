using System.Collections.ObjectModel;
using RoR2;
using TILER2;
using static RiskOfBulletstorm.Items.MasterRoundController;

namespace RiskOfBulletstorm.Items
{
    public class MasterRoundV : Item<MasterRoundV>
    {
        public override string displayName => "Master Round V";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => GetItemPickupStr();

        protected override string GetDescString(string langid = null) => GetItemDescStr();

        protected override string GetLoreString(string langID = null) => "This unfathomable artifact indicates mastery of the penultimate chamber." +
            "\nThe legendary hero felled the beast at the heart of the Gungeon with five rounds. According to the myth, the sixth remains unfired.";
    }
}
