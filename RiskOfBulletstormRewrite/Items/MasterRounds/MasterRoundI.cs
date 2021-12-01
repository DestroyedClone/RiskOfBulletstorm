using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;
using static RiskOfBulletstormRewrite.Main;
using static RiskOfBulletstormRewrite.Utils.ItemHelpers;
using static RiskOfBulletstormRewrite.Controllers.MasterRoundController;

namespace RiskOfBulletstormRewrite.Items
{
    public class MasterRoundI : ItemBase<MasterRoundI>
    {
        public override string ItemName => "Master Round I";

        public override string ItemLangTokenName => "MASTERROUNDI";

        public override string ItemPickupDesc => GetItemPickupStr();

        public override string ItemFullDescription => GetItemDescStr();

        public override string ItemLore => "<style=cStack>DECODING MESSAGE - KATE ****** - G*****N EXPLORATORY TEAM" +
            "\n [SYS] Filter: [messages from:KATE_OPERATOR sort:chronological date:today] | Showing 6 recent results" +
            "\n\n4:44 - [Kate] Found this scope in a chest, reporting back. It's got a green lens, and looks like a sniper scope." +
            "\n4:55 - [Kate] Seems to work whether it's attached or not, but I can't really look through it if it's not actually attached. Decision: attaching it." +
            "\n5:20 - [Kate] Not really sure it fits this place, since its just a scope. Then again, there was some duct tape in a chest, so maybe its not that weird..." +
            "\n6:00 - [Kate] I'm requesting a possible analysis sometime, I don't think this is just a scope. I'm hitting a lot more of my shots. Maybe I'm just more focused." +
            "\n6:02 - [Kate] Scratch that, this is ACTUALLY reducing the spread of bullets. I'm kinda curious how it works, but I'll study it at the shop." +
            "\n3:00 - [Kate] I am now back in the Breach. The shopkeeper did not like me testing the scope.";

        public override ItemTier Tier => ItemTier.Boss;

        public override ItemTag[] ItemTags => defaultItemTags;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("Assets/Models/Prefabs/Scope.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("Assets/Textures/Icons/Scope.png");

        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }
        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }
    }
}