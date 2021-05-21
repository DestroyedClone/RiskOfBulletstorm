using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class Scope : Item<Scope> 
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much should one Scope reduce spread? (Value: Subtractive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public static float SpreadReduction { get; private set; } = 0.10f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much should each Scope reduce spread by per stack? (Value: Subtractive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public static float SpreadReductionPerStack { get; private set; } = 0.05f;
        public override string displayName => "Scope";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "<b>Steady Aim</b>\nA standard scope. Increases accuracy!";

        protected override string GetDescString(string langid = null) => $"Decreases <style=cIsUtility>firing spread</style> by <style=cIsUtility>{Pct(SpreadReduction)}</style>" +
            $" <style=cStack>(+{Pct(SpreadReductionPerStack)} per stack)</style>";

        protected override string GetLoreString(string langID = null) => "<style=cStack>DECODING MESSAGE - KATE ****** - G*****N EXPLORATORY TEAM" + 
            "\n [SYS] Filter: [messages from:KATE_OPERATOR sort:chronological date:today] | Showing 6 recent results" + 
            "\n\n4:44 - [Kate] Found this scope in a chest, reporting back. It's got a green lens, and looks like a sniper scope." +
            "\n4:55 - [Kate] Seems to work whether it's attached or not, but I can't really look through it if it's not actually attached. Decision: attaching it." +
            "\n5:20 - [Kate] Not really sure it fits this place, since its just a scope. Then again, there was some duct tape in a chest, so maybe its not that weird..." +
            "\n6:00 - [Kate] I'm requesting a possible analysis sometime, I don't think this is just a scope. I'm hitting a lot more of my shots. Maybe I'm just more focused." +
            "\n6:02 - [Kate] Scratch that, this is ACTUALLY reducing the spread of bullets. I'm kinda curious how it works, but I'll study it at the shop." +
            "\n3:00 - [Kate] I am now back in the Breach. The shopkeeper did not like me testing the scope.";

        //private static readonly string NailgunMuzzleName = BaseNailgunState.muzzleName;
        //private static readonly GameObject NailgunTracer = BaseNailgunState.tracerEffectPrefab;
        public static GameObject ItemBodyModelPrefab;

        public Scope()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/Scope.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/Scope.png");
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();
            if (Compat_ItemStats.enabled)
            {
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return SpreadReduction + SpreadReductionPerStack * (count - 1); },
                    (value, inv, master) => { return $"Spread Reduction (Scope): {Pct(value)}"; }
                ));
            }
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = modelResource;
                displayRules = GenerateItemDisplayRules();
            }
            base.SetupAttributes();
        }
        public static ItemDisplayRuleDict GenerateItemDisplayRules()
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.1f, 0.1f, 0.1f);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(-0.20307F, 0.36353F, -0.02385F),
                    localAngles = new Vector3(279.3445F, 255.0025F, 197.1369F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandR",
                    localPos = new Vector3(0.16652F, 0.38849F, 0.00585F),
                    localAngles = new Vector3(280.6462F, 356.0192F, 276.5007F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0, 0, 0)
                }
            });
            rules.Add("mdlCommandoDualies", new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(-0.20307F, 0.36353F, -0.02385F),
                    localAngles = new Vector3(279.3445F, 255.0025F, 197.1369F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandR",
                    localPos = new Vector3(0.16652F, 0.38849F, 0.00585F),
                    localAngles = new Vector3(280.6462F, 356.0192F, 276.5007F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
});
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Muzzle",
                    localPos = new Vector3(0.00284F, -0.03001F, -0.08286F),
                    localAngles = new Vector3(0F, 0F, 180F),
                    localScale = new Vector3(0.0341F, 0.0341F, 0.0341F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-1.2878F, 5.52187F, -0.15921F),
                    localAngles = new Vector3(273.0957F, 275F, 180F),
                    localScale = new Vector3(0.5F, 0.5F, 0.5F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "CannonHeadL",
                    localPos = new Vector3(-0.05786F, 0.40173F, 0.28302F),
                    localAngles = new Vector3(283.4464F, 350.6581F, 187.0538F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "CannonHeadR",
                    localPos = new Vector3(0.05786F, 0.40173F, 0.28302F),
                    localAngles = new Vector3(283.4464F, 350.6581F, 187.0538F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlEngiTurret", new ItemDisplayRule[] // todo
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.25f, 0.9f, 0.7f),
                    localAngles = new Vector3(-90f, 90f, 0f),
                    localScale = generalScale * 2f
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.1f, 0.5f, 0.2f),
                    localAngles = new Vector3(-90f, 90f, 0f),
                    localScale = generalScale *2f
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(0.02112F, 0.22381F, -0.12105F),
                    localAngles = new Vector3(279.9348F, 190.1406F, 178.4321F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LowerArmR",
                    localPos = new Vector3(0.02328F, 0.20686F, 0.12726F),
                    localAngles = new Vector3(285.8345F, 7.64252F, 174.5907F),
                    localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(-0.47701F, 0.09434F, -0.17729F),
                    localAngles = new Vector3(3.40756F, 249.2628F, 180F),
                    localScale = new Vector3(0.03F, 0.03F, 0.05F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "WeaponPlatform",
                    localPos = new Vector3(0.00171F, -0.03728F, 0.55556F),
                    localAngles = new Vector3(277.4179F, 5F, 176.141F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MechHandR",
                    localPos = new Vector3(0.09043F, 0.06787F, 0.20701F),
                    localAngles = new Vector3(288.1208F, 23.35031F, 182.696F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MechHandL",
                    localPos = new Vector3(-0.07585F, 0.0381F, 0.21102F),
                    localAngles = new Vector3(286.8578F, 338.4095F, 181.5933F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.15831F, 6.68721F, -0.4692F),
                    localAngles = new Vector3(322.0836F, 179.7957F, 1.28439F),
                    localScale = new Vector3(0.8F, 0.8F, 0.8F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleGun",
                    localPos = new Vector3(0.10589F, -0.00867F, -0.09594F),
                    localAngles = new Vector3(2.8895F, 0.74889F, 270.0714F),
                    localScale = new Vector3(0.05F, 0.05F, 0.1F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[] //muzzle right can work too but he has diff forms, same model
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00003F, -0.00121F, 0.0946F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.02F, 0.02F, 0.04F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Shotgun",
                    localPos = new Vector3(0.00234F, 0.2396F, -0.0771F),
                    localAngles = new Vector3(273.1166F, 223.3013F, 136.4042F),
                    localScale = new Vector3(0.03F, 0.03F, 0.03F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "PistolMeshHip",
                    localPos = new Vector3(0.00587F, -0.16079F, 0.16504F),
                    localAngles = new Vector3(81.86201F, 33.93105F, 25.95335F),
                    localScale = new Vector3(0.03F, 0.03F, 0.025F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "PistolMeshHand",
                    localPos = new Vector3(-0.01088F, -0.15338F, 0.15399F),
                    localAngles = new Vector3(84.97465F, 351.1897F, 0F),
                    localScale = new Vector3(0.02F, 0.02F, 0.025F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MainWeapon",
                    localPos = new Vector3(-0.15541F, 0.431F, 0.00345F),
                    localAngles = new Vector3(276.674F, 289.3346F, 166.2988F),
                    localScale = new Vector3(0.04F, 0.04F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "SideWeapon",
                    localPos = new Vector3(-0.00431F, -0.17272F, 0.16614F),
                    localAngles = new Vector3(85.54737F, 0F, 0F),
                    localScale = new Vector3(0.03F, 0.03F, 0.03F)
                }
            });
            //rules.Add("mdlSniper", new ItemDisplayRule[] //?
            //{
            //    new ItemDisplayRule
            //    {
            //        ruleType = ItemDisplayRuleType.ParentedPrefab,
            //        followerPrefab = ItemBodyModelPrefab,
            //        childName = "ScopePoint",
            //        localPos = new Vector3(0.07f, 0.09f, 0.7f),
            //        localAngles = new Vector3(0f, 0f, 0f),
            //        localScale = generalScale * 0.5f
            //    }
            //});
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Muzzle",
                    localPos = new Vector3(0.2083F, 0.35881F, -1.87173F),
                    localAngles = new Vector3(351.2786F, 3.28465F, 335.1005F),
                    localScale = new Vector3(0.2F, 0.2F, 0.15F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hammer",
                    localPos = new Vector3(-0.55614F, 13.75842F, 0.55696F),
                    localAngles = new Vector3(350.0659F, 299.6119F, 345.7428F),
                    localScale = new Vector3(0.9F, 0.9F, 1F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Weapon",
                    localPos = new Vector3(0.42151F, 8.62597F, 5.57601F),
                    localAngles = new Vector3(85.38794F, 0F, 0F),
                    localScale = new Vector3(2F, 2F, 1.8F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.06406F, 0.49998F, -0.38402F),
                localAngles = new Vector3(343.0749F, 178.6757F, 182.3901F),
                localScale = new Vector3(0.0625F, 0.0625F, 0.0625F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-1.01006F, 2.06226F, -0.10432F),
                localAngles = new Vector3(359.9318F, 252.1513F, 178.6301F),
                localScale = new Vector3(0.2454F, 0.2862F, 0.1384F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLT",
                localPos = new Vector3(-1.69412F, -0.24674F, -0.4797F),
                localAngles = new Vector3(2.51877F, 359.4715F, 178.5113F),
                localScale = new Vector3(0.1961F, 0.1961F, 0.1961F)
            });
            // lunar wisp todo
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(0.00941F, -0.13842F, 2.04243F),
                localAngles = new Vector3(0F, 0F, 183.8828F),
                localScale = new Vector3(0.1228F, 0.1228F, 0.0959F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleJar",
                localPos = new Vector3(2.53472F, -3.04584F, 2.24337F),
                localAngles = new Vector3(74.70916F, 37.12689F, 0.53514F),
                localScale = new Vector3(0.6916F, 0.6916F, 0.6916F)
            });
            return rules;
        }
    }
}
