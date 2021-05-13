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
    public class TableTechStun : Item<TableTechStun>
    {
        readonly float radius = 10f;
        readonly float stackRadius = 2f;

        public override string displayName => "Table Tech Stun";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Flip Showmanship";

        protected override string GetDescString(string langid = null) => $"Opening a barrel (table) <style=cIsDamage>stuns</style> nearby enemies within a radius of {radius} meters <style=cStack>(+{stackRadius} per stack)</style>.";

        protected override string GetLoreString(string langID = null) => "This ancient technique briefly stuns enemies whenever a table is flipped." +
            "\nChapter one of the \"Tabla Sutra.\" And when a table is masterfully flipped, what a most magnificent sight! Surely all who witness the flip of a true master can only stand transfixed, agog.";
        public TableTechStun()
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
                    new BlastAttack()
                    {
                        attacker = body.gameObject,
                        inflictor = self.gameObject,
                        baseDamage = body.damage * 1f,
                        damageColorIndex = DamageColorIndex.Item,
                        damageType = DamageType.Stun1s,
                        losType = BlastAttack.LoSType.None,
                        position = self.gameObject.transform.position,
                        procCoefficient = 0f,
                        radius = actualRadius,
                        teamIndex = body.teamComponent.teamIndex,
                    }.Fire();
                }
            }
        }
    }
}
