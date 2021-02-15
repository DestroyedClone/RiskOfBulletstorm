using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class Scope : Item_V2<Scope> 
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

        protected override string GetDescString(string langid = null) => $"<style=cIsUtility>Decreases firing spread by {Pct(Scope_SpreadReduction)}</style>" +
            $"\n<style=cStack>(+{Pct(Scope_SpreadReductionStack)} per stack)</style>";

        protected override string GetLoreString(string langID = null) => "4:44 - [Kate] Found this scope in a chest, reporting back." +
            "\n 4:55 - [Kate] Seems to work whether it's attached or not, but I can't really look through it if it's not actually attached. Decision: attaching it." +
            "\n 5:20 - [Kate] This is great, I can start keeping safer distances, even with the spread." +
            "\n 6:00 - [Kate] Alright, I'm keeping it. Can't explain it, feels like I'm actually tightening the spread." +
            "\n 6:02 - [Kate] Scratch that, this is ACTUALLY reducing the spread of bullets. I'm kinda curious how it works, but I'll study it back at the Breach later.";

        //private static readonly string NailgunMuzzleName = BaseNailgunState.muzzleName;
        //private static readonly GameObject NailgunTracer = BaseNailgunState.tracerEffectPrefab;
        public static GameObject ItemBodyModelPrefab;

        public Scope()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Scope.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Scope.png";
        }
        public override void SetupBehavior()
        {

        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>("@RiskOfBulletstorm:Assets/Models/Prefabs/Scope.prefab");
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
                    localPos = new Vector3(-1f, 5f, -0.9f),
                    localAngles = new Vector3(-100f, 275f, 180f),
                    localScale = generalScale * 5
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
                    localPos = new Vector3(0.09f, 0.25f, 0.7f),
                    localAngles = new Vector3(-90f, 270f, 0f),
                    localScale = generalScale
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
                    localPos = new Vector3(0.37f, 0.24f, 1f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0.3f, 0.3f, 0.6f)
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
