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
    public class TableTechBlank : Item<TableTechBlank>
    {
        readonly float radius = 10f;
        readonly float stackRadius = 2f;

        public override string displayName => "Table Tech Blanks";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Flip Clarity";

        protected override string GetDescString(string langid = null) => $"Opening a barrel (table) <style=cIsUtility>blanks</style> for <style=cIsDamage>100% damage</style>" +
            $" and <style=cIsUtility>clears projectiles</style> within a radius of {radius} meters <style=cStack>(+{stackRadius} per stack)</style>.";

        protected override string GetLoreString(string langID = null) => "This ancient technique will trigger a Blank when a table is flipped." +
            "\nChapter four of the \"Tabla Sutra.\" A clear table leads to a clear mind.";

        public TableTechBlank()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/SpreadAmmo.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/SpreadAmmo.png");
        }

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
                    var actualRadius = radius + stackRadius * (itemCount - 1);
                    Shared.Blanks.MasterBlankItem.FireBlank(body, self.gameObject.transform.position, actualRadius, 1f, actualRadius, false, true, true);
                }
            }
        }
    }
}
