﻿using RiskOfBulletstorm.Utils;
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
using EntityStates;

namespace RiskOfBulletstorm.Items
{
    public class Scope : Item_V2<Scope> 
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Bullet Spread Reduction (Default: 10%)", AutoConfigFlags.PreventNetMismatch)]
        public float Scope_SpreadReduction { get; private set; } = 0.10f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Bullet Spread Reduction Stack (Default: 5%)", AutoConfigFlags.PreventNetMismatch)]
        public float Scope_SpreadReductionStack { get; private set; } = 0.05f;
        public override string displayName => "Scope";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Steady Aim\nA standard scope. Increases accuracy!";

        protected override string GetDescString(string langid = null) => $"<style=cIsUtility>Decreases bullet spread by {Pct(Scope_SpreadReduction)}</style>" +
            $"\n<style=cStack>(+{Pct(Scope_SpreadReductionStack)} per stack)</style>";

        protected override string GetLoreString(string langID = null) => "4:44 - [Kate] Found this scope in a chest, reporting back." +
            "\n 4:55 - [Kate] Seems to work pretty well whether it's attached or not." +
            "\n 5:20 - [Kate] This really works quite well, my shots are hitting more often." +
            "\n 6:00 - [Kate] Alright, I'm keeping it. Can't explain it, feels like I'm actually tightening the spread." +
            "\n 6:02 - [Kate] Scratch that, this is ACTUALLY reducing the spread of bullets. I'm kinda curious how it works, but I'll study it back at the Breach later.";

        public static GameObject ItemBodyModelPrefab;
        private static readonly GameObject REXPrefab = EntityStates.Treebot.Weapon.FireSyringe.projectilePrefab;
        //private static readonly string NailgunMuzzleName = BaseNailgunState.muzzleName;
        //private static readonly GameObject NailgunTracer = BaseNailgunState.tracerEffectPrefab;
        private static readonly GameObject SawPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Sawmerang");

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
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.1f, 0.1f, 0.1f);

            /*var GetEnforcerIndex = BodyCatalog.FindBodyIndex("EnforcerBody");
            var GetEnforcerPrefab = BodyCatalog.GetBodyPrefab(GetEnforcerIndex);
            if (GetEnforcerPrefab)
            {
                var GetEnforcerModel = GetEnforcerPrefab.GetComponentsInChildren<CharacterModel>()[22];
                if (GetEnforcerModel)
                {

                }

            }*/

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
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
/*                new ItemDisplayRule //Sniper
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ScopePoint",
                    localPos = new Vector3(0.07f, 0.09f, 0.7f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 0.5f
                }*/
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
                    localPos = new Vector3(-1f, 5f, -0.9f),
                    localAngles = new Vector3(-100f, 275f, 180f),
                    localScale = generalScale * 5
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
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(-0.7f, 0f, -0.4f),
                    localAngles = new Vector3(0f, -110f, 180f),
                    localScale = generalScale * 0.5f
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
                    localPos = new Vector3(0f, 0.05f, 0.3f),
                    localAngles = new Vector3(-90f, 270f, 0f),
                    localScale = generalScale
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
                    childName = "Shotgun", //BanditShotgunMesh
                    localPos = new Vector3(0.05f, 0.4f, -0.13f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = generalScale * 0.5f
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
                    childName = "PistolMeshHip", //BanditPistolMeshHand
                    localPos = new Vector3(0.05f, -0.2f, 0.2f),
                    localAngles = new Vector3(90f, 0f, 0f),
                    localScale = generalScale * 0.3f
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
            On.RoR2.BulletAttack.Fire += BulletAttack_Fire;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileManager_FireProjectile_FireProjectileInfo;
        }

        private void ProjectileManager_FireProjectile_FireProjectileInfo(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo fireProjectileInfo)
        {
            GameObject owner = fireProjectileInfo.owner;
            if (owner)
            {
                CharacterBody cb = owner.GetComponent<CharacterBody>();
                if (cb)
                {
                    Inventory inventory = cb.inventory;
                    if (inventory)
                    {
                        int InventoryCount = cb.inventory.GetItemCount(catalogIndex);
                        if (InventoryCount > 0)
                        {
                            var ResultMult = (Scope_SpreadReduction + Scope_SpreadReductionStack * (InventoryCount - 1));
                            InputBankTest input = cb.inputBank;
                            if (input)
                            {
                                Quaternion aimDir = Util.QuaternionSafeLookRotation(input.aimDirection);
                                Quaternion rotation = fireProjectileInfo.rotation;

                                Quaternion UpdatedAngle = Quaternion.Lerp(rotation, aimDir, ResultMult);

                                if (fireProjectileInfo.projectilePrefab == REXPrefab)
                                {
                                    //Chat.AddMessage("Syringe fired");
                                }
                                //if (inventory.currentEquipmentIndex == EquipmentIndex.Saw)
                                else if (fireProjectileInfo.projectilePrefab == SawPrefab)
                                {
                                    //Chat.AddMessage("Saws fired");
                                }
                                //Chat.AddMessage("Scope Lerp: " + aimDir + " and " + rotation + " resulting " + UpdatedAngle);
                                fireProjectileInfo.rotation = UpdatedAngle;
                            }
                        }
                    }
                }
            }
            orig(self, fireProjectileInfo);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.BulletAttack.Fire -= BulletAttack_Fire;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo -= ProjectileManager_FireProjectile_FireProjectileInfo;
        }

        private void BulletAttack_Fire(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            //doesn't work on MULT?????
            CharacterBody characterBody = self.owner.GetComponent<CharacterBody>();
            if (characterBody)
            {
                int InventoryCount = characterBody.inventory.GetItemCount(catalogIndex);
                if (InventoryCount > 0)
                {
                    var ResultMult = 1 - (Scope_SpreadReduction + Scope_SpreadReductionStack * (InventoryCount - 1));
                    //self.maxSpread = Mathf.Max(self.maxSpread * ResultMult * 0.75f, 2 * ResultMult);

                    characterBody.SetSpreadBloom(Mathf.Min(0, characterBody.spreadBloomAngle * ResultMult), false); //should affect MULT

                    //var oldMax = self.maxSpread;
                    self.maxSpread = Mathf.Max(self.maxSpread * ResultMult, 0);

                    //var oldMin = self.minSpread;
                    self.minSpread = Mathf.Min(0, self.minSpread * ResultMult);

                    self.spreadPitchScale = Mathf.Min(0, self.spreadPitchScale * ResultMult);
                    self.spreadYawScale = Mathf.Min(0, self.spreadYawScale * ResultMult);


                    //self.owner.GetComponent<CharacterBody>().SetSpreadBloom(ResultMult, false);
                    //Debug.Log("Scope: Max:" + oldMax.ToString() + "=>" + self.maxSpread.ToString());
                    //Debug.Log("Scope: Min:" + oldMin.ToString() + "=>" + self.minSpread.ToString());

                }
            }
            orig(self);
        }
    }
}
