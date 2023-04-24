using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    public class CurseTally : ItemBase<CurseTally>
    {
        public override string ItemName => "CurseTally";

        public override string ItemLangTokenName => "CURSETALLY";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite(); //sprite is fine, its gonna show up in inv

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
        };

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void Hooks()
        {
            base.Hooks();
            Inventory.onServerItemGiven += Inventory_onServerItemGiven;
        }

        private void Inventory_onServerItemGiven(Inventory inventory, ItemIndex itemIndex, int _)
        {
            if (itemIndex == ItemDef.itemIndex)
            {
                if (inventory.TryGetComponent(out CharacterMaster characterMaster))
                {
                    var body = characterMaster.GetBody();
                    if (body)
                    {
                        /*EffectData effectData = new EffectData()
                        {
                            origin = body.gameObject.transform.position
                        };
                        EffectManager.SpawnEffect(CharacterBody.AssetReferences.deathmarkEffectPrefab,
                            effectData, true);*/
                        body.UpdateSingleTemporaryVisualEffect(ref body.deathmarkEffectInstance, CharacterBody.AssetReferences.deathmarkEffectPrefab,
                            body.radius, true);
                    }
                }
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }
    }
}