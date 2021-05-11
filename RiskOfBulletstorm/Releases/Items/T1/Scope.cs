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
        public static float Scope_SpreadReduction { get; private set; } = 0.10f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much should each Scope reduce spread by per stack? (Value: Subtractive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public static float Scope_SpreadReductionStack { get; private set; } = 0.05f;
        public override string displayName => "Scope";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "<b>Steady Aim</b>\nA standard scope. Increases accuracy!";

        protected override string GetDescString(string langid = null) => $"Decreases <style=cIsUtility>firing spread</style> by <style=cIsUtility>{Pct(Scope_SpreadReduction)}</style>" +
            $" <style=cStack>(+{Pct(Scope_SpreadReductionStack)} per stack)</style>";

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
                    ((count, inv, master) => { return Scope_SpreadReduction + Scope_SpreadReductionStack * (count - 1); },
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
localPos = new Vector3(-0.24F, 0.4F, -0.07F),
localAngles = new Vector3(280.5324F, 270F, 180F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandR",
localPos = new Vector3(0.2383F, 0.4016F, 0.0251F),
localAngles = new Vector3(273.2098F, 81.7268F, 218.8035F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
            });
            rules.Add("mdlCommandoDualies", new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(-0.24f, 0.4f, -0.07f),
                    localAngles = new Vector3(270, 90, 0),
                    localScale = generalScale * 0.5f
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandR",
                    localPos = new Vector3(0.24f, 0.4f, 0.04f),
                    localAngles = new Vector3(-90f, 300f, 0f),
                    localScale = generalScale * 0.5f
                },
});
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Muzzle",
localPos = new Vector3(-0.0418F, -0.0578F, 0.0333F),
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
localPos = new Vector3(-1.3296F, 4.4384F, -0.8709F),
localAngles = new Vector3(271.0498F, 275.0005F, 179.9995F),
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
localPos = new Vector3(-0.1529F, 0.4954F, 0.3552F),
localAngles = new Vector3(272.366F, 313.7744F, 223.7704F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "CannonHeadR",
localPos = new Vector3(-0.0912F, 0.4954F, 0.3552F),
localAngles = new Vector3(272.3658F, 313.7745F, 223.7703F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });
            rules.Add("mdlEngiTurret", new ItemDisplayRule[] // 
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
localPos = new Vector3(0.1917F, 0.27F, -0.0934F),
localAngles = new Vector3(270F, 308.4749F, 0F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "LowerArmR",
localPos = new Vector3(0.1305F, 0.34F, 0.1554F),
localAngles = new Vector3(270F, 227.3861F, 0F),
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
localPos = new Vector3(-0.7217F, 0.0102F, -0.3422F),
localAngles = new Vector3(0F, 249.2628F, 180F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "WeaponPlatform",
localPos = new Vector3(-0.1196F, 0.0944F, 0.6998F),
localAngles = new Vector3(270F, 180F, 0F),
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
localPos = new Vector3(0.0074F, 0.05F, 0.3045F),
localAngles = new Vector3(280.7216F, 21.4809F, 184.5103F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "MechHandL",
localPos = new Vector3(-0.2669F, 0.1564F, 0.2405F),
localAngles = new Vector3(288.2362F, 329.5789F, 182.009F),
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
                    localPos = new Vector3(-0.8f, 7.5f, 0f),
                    localAngles = new Vector3(-30f, 180f, 1.17f),
                    localScale = generalScale * 8f
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleGun",
                    localPos = new Vector3(0.12f, -0.15f, 0f),
                    localAngles = new Vector3(0f, 0f, 240f),
                    localScale = new Vector3(0.05f, 0.05f, 0.1f)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0.02f, 0.015f, 0.2f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0.02f, 0.02f, 0.04f)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Shotgun",
localPos = new Vector3(0.0656F, 0.4F, -0.13F),
localAngles = new Vector3(274.8828F, 180F, 180F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "PistolMeshHand", //BanditPistolMeshHand
                    localPos = new Vector3(-0.03f, -0.16f, 0.14f),
                    localAngles = new Vector3(90f, 260f, 90f),
                    localScale = generalScale * 0.3f
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "PistolMeshHip",
localPos = new Vector3(0.0294F, -0.2215F, 0.1904F),
localAngles = new Vector3(83.6198F, 0F, 0F),
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
localPos = new Vector3(0.37F, 0.1355F, 1.0001F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.3F, 0.3F, 0.6F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Hammer",
                    localPos = new Vector3(-2.3f, 14.5f, 2f),
                    localAngles = new Vector3(-10f, -65f, -2.6f),
                    localScale = new Vector3(0.9f, 0.9f, 1.1f)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Weapon",
                    localPos = new Vector3(2.4f, 0f, 8f),
                    localAngles = new Vector3(80f, 0f, 0f),
                    localScale = new Vector3(2f, 2f, 2f)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.0636F, 0.4873F, -0.3886F),
                localAngles = new Vector3(355.4272F, 179.1889F, 182.2938F),
                localScale = new Vector3(0.0625F, 0.0625F, 0.0625F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-1.0927F, 1.8798F, -0.4661F),
                localAngles = new Vector3(359.9318F, 252.1513F, 178.6301F),
                localScale = new Vector3(0.2454F, 0.2862F, 0.1384F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLT",
                localPos = new Vector3(-1.9034F, 0.3452F, -0.4486F),
                localAngles = new Vector3(-0.0014F, 359.537F, 178.5127F),
                localScale = new Vector3(0.1961F, 0.1961F, 0.1961F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(-0.1374F, -0.435F, 1.0132F),
                localAngles = new Vector3(29.9611F, 359.8247F, 180.4726F),
                localScale = new Vector3(0.1228F, 0.1228F, 0.0959F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleJar",
                localPos = new Vector3(2.8699F, -3.7069F, 2.6919F),
                localAngles = new Vector3(86.598F, 38.9854F, 2.3789F),
                localScale = new Vector3(0.6916F, 0.6916F, 0.6916F)
            });
            return rules;
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}
