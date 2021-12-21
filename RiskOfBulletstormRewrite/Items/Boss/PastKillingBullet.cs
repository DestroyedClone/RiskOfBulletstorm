using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;

namespace RiskOfBulletstormRewrite.Items
{
    public class PastKillingBullet : ItemBase<PastKillingBullet>
    {
        public override string ItemName => "Past Killing Bullet";

        public override string ItemLangTokenName => "PASTKILLINGBULLET";

        public override ItemTier Tier => ItemTier.Boss;

        public override GameObject ItemModel => Resources.Load<GameObject>("prefabs/nullmodel");

        public override Sprite ItemIcon => Resources.Load<Sprite>("textures/itemicons/texnullicon");

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Any,
            ItemTag.AIBlacklist,
            ItemTag.BrotherBlacklist,
            ItemTag.CannotCopy,
            ItemTag.CannotSteal,
            ItemTag.WorldUnique
        };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            On.RoR2.GenericPickupController.OnTriggerStay += PreventBulletAutoPickup;
        }

        private void PreventBulletAutoPickup(On.RoR2.GenericPickupController.orig_OnTriggerStay orig, GenericPickupController self, Collider other)
        {
            if (self.pickupIndex != PickupCatalog.FindPickupIndex(PastKillingBullet.instance.ItemDef.itemIndex))
                orig(self, other);
            else
                return;
        }
    }
}
