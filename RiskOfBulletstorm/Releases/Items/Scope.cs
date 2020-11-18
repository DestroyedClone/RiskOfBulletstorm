//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using RoR2.Projectile;

namespace RiskOfBulletstorm.Items
{
    public class Scope : Item_V2<Scope> 
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Bullet Spread Reduction (Default: 10%)", AutoConfigFlags.PreventNetMismatch)]
        public float SpreadReduction { get; private set; } = 0.10f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Bullet Spread Reduction Stack (Default: 5%)", AutoConfigFlags.PreventNetMismatch)]
        public float SpreadReductionStack { get; private set; } = 0.05f;
        public override string displayName => "Scope";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Steady Aim\nA standard scope. Increases accuracy!";

        protected override string GetDescString(string langid = null) => $"Decreases bullet spread by {Pct(SpreadReduction)}" +
            $"\n(+{Pct(SpreadReductionStack)} per stack)";

        protected override string GetLoreString(string langID = null) => "4:44 - [Kate] Found this scope in a chest, reporting back." +
            "\n 4:55 - [Kate] Seems to work pretty well whether it's attached or not." +
            "\n 5:20 - [Kate] This really works quite well, my shots are hitting more often." +
            "\n 6:00 - [Kate] Alright, I'm keeping it. Can't explain it, feels like I'm actually tightening the spread." +
            "\n 6:02 - [Kate] Scratch that, this is ACTUALLY reducing the spread of bullets. I'm kinda curious how it works, but I'll study it back at the Breach later.";

        public static GameObject ItemBodyModelPrefab;
        //private static GameObject REXPrefab = EntityStates.Treebot.Weapon.FireSyringe.projectilePrefab;

        public Scope()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Scope.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/ScopeIcon.png";
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
        private static ItemDisplayRuleDict GenerateItemDisplayRules()
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            //ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.1f, 0.1f, 0.1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(-0.15f, 0.4f, -0.05f),
                    localAngles = new Vector3(270, 0, 0),
                    localScale = generalScale * 0.5f
                }
            });
            rules.Add("mdlCommandoDualies", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(-0.15f, 0.4f, -0.05f),
                    localAngles = new Vector3(270, 0, 0),
                    localScale = generalScale * 0.5f
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandR",
                    localPos = new Vector3(0.2f, 0.5f, 0.1f),
                    localAngles = new Vector3(289f, 7f, 7f),
                    localScale = generalScale * 0.5f
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Muzzle",
                    localPos = new Vector3(-0.06f, -0.1f, 0.1f),
                    localAngles = new Vector3(0, 0, 180f),
                    localScale = generalScale * 0.5f
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(-1f, 5f, -1.6f),
                    localAngles = new Vector3(-180f, 160f, 108f),
                    localScale = generalScale * 10
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[] // 
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "CannonHeadL",
                    localPos = new Vector3(-0.1f, 0.5f, 0.2f),
                    localAngles = new Vector3(-90f, 90f, 0f),
                    localScale = generalScale
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "CannonHeadR",
                    localPos = new Vector3(-0.1f, 0.5f, 0.2f),
                    localAngles = new Vector3(-90f, 90f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LowerArmL",
                    localPos = new Vector3(0.2f, 0.27f, -0.1f),
                    localAngles = new Vector3(-90f, 5f, 0f),
                    localScale = generalScale * 0.5f
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "LowerArmR",
                    localPos = new Vector3(0.08f, 0.34f, 0.05f),
                    localAngles = new Vector3(-90f, 180f, 0f),
                    localScale = generalScale * 0.5f
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[] //TODO
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Stomach",
                    localPos = new Vector3(0f, 0f, 0.2f),
                    localAngles = new Vector3(0f, 180f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "WeaponPlatform",
                    localPos = new Vector3(-0.85f, 0.25f, 0f),
                    localAngles = new Vector3(0f, 180f, 90f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[] //TODO
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Stomach",
                    localPos = new Vector3(0f, 0.05f, 0.3f),
                    localAngles = new Vector3(0f, 180f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[] //TODO
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Stomach",
                    localPos = new Vector3(0f, 0.5f, -2.6f),
                    localAngles = new Vector3(-20f, 180f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "MuzzleGun",
                    localPos = new Vector3(-0.17f, 0.05f, 0f),
                    localAngles = new Vector3(-14.4f, 0f, 240f),
                    localScale = new Vector3(0.05f, 0.05f, 0.1f)
                }
            }) ;
            rules.Add("mdlBrother", new ItemDisplayRule[] //TODO
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Stomach",
                    localPos = new Vector3(0.05f, 0.1f, 0.22f),
                    localAngles = new Vector3(5f, 210f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "BanditShotgunMesh",
                    localPos = new Vector3(0.05f, -0.15f, 0.23f),
                    localAngles = new Vector3(90f, 0f, 0f),
                    localScale = generalScale * 0.5f
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "BanditPistolMeshHand",
                    localPos = new Vector3(0.05f, -0.25f, 0.2f),
                    localAngles = new Vector3(90f, 0f, 0f),
                    localScale = generalScale * 0.3f
                }
            });
            rules.Add("mdlSniper", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ScopePoint",
                    localPos = new Vector3(0.07f, 0.09f, 0.7f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 0.5f
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[] //TODO
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.1f, 0.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 2f
                }
            });
            rules.Add("mdlEnforcer", new ItemDisplayRule[] //TODO
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.1f, 0.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 2f
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
            On.RoR2.BulletAttack.Fire += BulletAttack_Fire;
            //On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileManager_FireProjectile_FireProjectileInfo;
        }

        /*private void ProjectileManager_FireProjectile_FireProjectileInfo(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo fireProjectileInfo)
        { Doesnt work hahahahahahhah :(
            var owner = fireProjectileInfo.owner;
            if (owner)
            {
                var cb = owner.GetComponent<CharacterBody>();
                if (cb)
                {
                    int invcount = cb.inventory.GetItemCount(catalogIndex);
                    if (invcount > 2)
                    {
                        if (cb.inputBank)
                        {
                            var aimDir = Util.QuaternionSafeLookRotation(cb.inputBank.aimDirection);

                            if (fireProjectileInfo.projectilePrefab == REXPrefab)
                            {
                                fireProjectileInfo.rotation = aimDir;
                            }
                        }
                    }
                }
            }
            orig(self, fireProjectileInfo);
        }*/

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.BulletAttack.Fire -= BulletAttack_Fire;
        }

        private void BulletAttack_Fire(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            //doesn't work on MULT?????
            int InventoryCount = self.owner.GetComponent<CharacterBody>().inventory.GetItemCount(catalogIndex);
            if (InventoryCount > 0)
            {
                var ResultMult = 1 - (SpreadReduction + SpreadReductionStack * (InventoryCount - 1));
                //self.maxSpread = Mathf.Max(self.maxSpread * ResultMult * 0.75f, 2 * ResultMult);

                //var oldMax = self.maxSpread;
                self.maxSpread = Mathf.Max(self.maxSpread * ResultMult, 0);

                //var oldMin = self.minSpread;
                self.minSpread = Mathf.Min(0, self.minSpread * ResultMult);

                //self.owner.GetComponent<CharacterBody>().SetSpreadBloom(ResultMult, false);
                //Debug.Log("Scope: Max:" + oldMax.ToString() + "=>" + self.maxSpread.ToString());
                //Debug.Log("Scope: Min:" + oldMin.ToString() + "=>" + self.minSpread.ToString());

            }
            orig(self);
        }
    }
}
