using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.BulletstormPlugin;
using static RiskOfBulletstorm.Utils.HelperUtil;
using RoR2.Projectile;
using RiskOfBulletstorm.Shared.Buffs;

namespace RiskOfBulletstorm.Releases.Items.T1
{
    public class TableTechRage : Item<TableTechRage>
    {
        readonly float duration = 2f;
        readonly float stackDuration = 1f;

        public override string displayName => "Table Tech Rage";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Flips Of Fury";

        protected override string GetDescString(string langid = null) => $"Opening a barrel (table) <style=cIsDamage>enrages</style> you, multiplying your damage by +{Pct(BuffsController.Config_Enrage_Damage)} for {duration} seconds <style=cStack>(+{stackDuration} seconds per stack)</style> ";

        protected override string GetLoreString(string langID = null) => "This ancient technique briefly increases damage whenever a table is flipped." +
"\nChapter two of the \"Tabla Sutra.\" When a table is flipped, does it not feel anger? Does it not feel rage? Understand that feeling, and when you flip the table, be yourself flipped." +

"\n\nMade famous in a duel between the third Flipjutsu Master and a nameless table flipper.";

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }

        public override void Install()
        {
            base.Install();
            On.RoR2.BarrelInteraction.OnInteractionBegin += BarrelInteraction_OnInteractionBegin;
        }

        private void BarrelInteraction_OnInteractionBegin(On.RoR2.BarrelInteraction.orig_OnInteractionBegin orig, BarrelInteraction self, Interactor activator)
        {
            orig(self, activator);
            if (activator && activator.GetComponent<CharacterBody>())
            {
                var body = activator.GetComponent<CharacterBody>();
                var itemCount = GetCount(body);
                if (itemCount > 0)
                {
                    body.AddTimedBuffAuthority(BuffsController.Anger.buffIndex, duration + stackDuration*(itemCount-1));
                }
            }
        }
    }
}
