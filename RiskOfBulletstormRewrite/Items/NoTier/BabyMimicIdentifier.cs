
using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Items
{
    public class BabyMimicIdentifier : ItemBase<BabyMimicIdentifier>
    {
        public override string ItemName => "BabyMimicIdentifier";

        public override string ItemLangTokenName => "BabyMimicIdentifier";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
        };

        public override void Init(ConfigFile config)
        {
            return;
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
            On.RoR2.Util.GetBestBodyName += ChangeNameToMimic;
            On.RoR2.CharacterMaster.SetUpGummyClone += SetupMimicStuff;
        }

        //hooking because its called twice and its simpler to just hook this
        private void SetupMimicStuff(On.RoR2.CharacterMaster.orig_SetUpGummyClone orig, CharacterMaster self)
        {
            orig(self);
			if (NetworkServer.active && self.inventory && self.inventory.GetItemCount(Items.BabyMimicIdentifier.instance.ItemDef) > 0)
			{
				CharacterBody body = self.GetBody();
				if (body)
				{
					body.portraitIcon = Items.BabyGoodMimic.mimicIcon.texture;
                    body.subtitleNameToken = "RISKOFBULLETSTORM_BABYGOODMIMIC_BODY_SUBTITLE";
				}
			}
        }

        private string ChangeNameToMimic(On.RoR2.Util.orig_GetBestBodyName orig, GameObject bodyObject)
        {
            var original = orig(bodyObject);
            if (bodyObject && bodyObject.TryGetComponent<CharacterBody>(out CharacterBody characterBody))
            {
                if (characterBody.inventory)
                {
					if (characterBody.inventory.GetItemCount(Items.BabyMimicIdentifier.instance.ItemDef) > 0)
					{
						return RoR2.Language.GetStringFormatted("RISKOFBULLETSTORM_BODY_MODIFIER_MIMIC", new object[]
						{
							original
						});
					}
                }
            }
            return original;
        }
    }
}
