using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
	public class RollBomb : Item_V2<RollBomb>
	{
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many damage should Roll Bomb deal? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float RollBomb_Damage { get; private set; } = 2.0f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Scaled radius of Roll Bomb explosion? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float RollBomb_Radius { get; private set; } = 3f;

        public override string displayName => "Roll Bomb";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>Power Charge</b>\nDrop bomb(s) after using your utility skill.";

        protected override string GetDescString(string langid = null) => $"Using your utility <style=cIsUtility>drops 1 bomb</style> for <style=cIsDamage>{Pct(RollBomb_Damage)} damage</style>." +
            $"\n<style=cStack>(+1 bomb dropped per stack)</style>";

        protected override string GetLoreString(string langID = null) => "Produces a bomb when dodge rolling.\nThis strange mechanism dispenses explosives when spun.";
        public static GameObject BombPrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;

        public RollBomb()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/RollBomb.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/RollBomb.png";
        }

        public override void SetupBehavior()
        {
            GameObject engiMinePrefab = Resources.Load<GameObject>("prefabs/projectiles/EngiGrenadeProjectile");
            BombPrefab = engiMinePrefab.InstantiateClone("Bulletstorm_RollBomb");
            BombPrefab.transform.localScale = new Vector3(2f, 2f, 2f);
            BombPrefab.GetComponent<ProjectileSimple>().velocity = 1; //default 50
            BombPrefab.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Item;
            BombPrefab.GetComponent<ProjectileImpactExplosion>().destroyOnEnemy = false; //default True
            BombPrefab.GetComponent<ProjectileImpactExplosion>().blastRadius *= RollBomb_Radius;
            Object.Destroy(BombPrefab.GetComponent<ApplyTorqueOnStart>());

            var controller = BombPrefab.GetComponent<ProjectileController>();
            controller.transform.localScale = new Vector3(2f, 2f, 2f);
        }

        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>(modelResourcePath);
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
childName = "Chest",
localPos = new Vector3(0.2247F, 0.4057F, 0.0239F),
localAngles = new Vector3(1.1661F, 4.2149F, 333.5849F),
localScale = new Vector3(0.0204F, 0.0204F, 0.0204F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmR",
localPos = new Vector3(0.015F, 0.0509F, -0.0929F),
localAngles = new Vector3(288.4712F, 196.9971F, 164.2121F),
localScale = new Vector3(0.0198F, 0.0198F, 0.0198F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 2.3f, 2.5f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 4
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0f, 0.3f, -0.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmL",
localPos = new Vector3(0.0506F, -0.1051F, 0.0035F),
localAngles = new Vector3(0F, 0F, 243.1167F),
localScale = new Vector3(0.0123F, 0.0123F, 0.0123F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmR",
localPos = new Vector3(-0.0096F, -0.1347F, -0.0531F),
localAngles = new Vector3(348.5455F, 175.2722F, 215.3738F),
localScale = new Vector3(0.0199F, 0.0199F, 0.0199F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Base",
                    localPos = new Vector3(0f, 0.4f, -1.6f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmR",
localPos = new Vector3(0.0014F, 0.1701F, -0.1282F),
localAngles = new Vector3(278.1336F, 0F, 8.981F),
localScale = new Vector3(0.0228F, 0.0228F, 0.0228F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ClavicleR",
localPos = new Vector3(-0.7524F, 3.7467F, 2.0084F),
localAngles = new Vector3(65.6256F, 330.9936F, 343.4614F),
localScale = new Vector3(0.2786F, 0.2786F, 0.2786F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ClavicleL",
localPos = new Vector3(0.0216F, -0.0813F, -0.0867F),
localAngles = new Vector3(298.0841F, 149.9157F, 189.201F),
localScale = new Vector3(0.0127F, 0.0127F, 0.0127F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, -0.15f, -0.1f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0.02f, 0.02f, 0.02f)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmL",
localPos = new Vector3(-0.0288F, -0.1947F, 0.0032F),
localAngles = new Vector3(290.4564F, 114.5281F, 211.5726F),
localScale = new Vector3(0.0099F, 0.0099F, 0.0099F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Root",
localPos = new Vector3(0.1168F, 2.7479F, -0.2626F),
localAngles = new Vector3(356.6046F, 356.4987F, 333.4262F),
localScale = new Vector3(0.19F, 0.19F, 0.19F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-1.2798F, 3.2966F, 0.0782F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1624F, 0.1624F, 0.1624F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(8.5558F, -11.7221F, 4.3088F),
localAngles = new Vector3(336.0467F, 53.1984F, 241.5127F),
localScale = new Vector3(0.4019F, 0.4019F, 0.4019F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "UpperArmL",
                localPos = new Vector3(-0.0019F, -0.0293F, 0.0963F),
                localAngles = new Vector3(350.8554F, 332.2057F, 202.3158F),
                localScale = new Vector3(0.0385F, 0.0385F, 0.0385F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(2.0595F, 2.0331F, -0.39F),
                localAngles = new Vector3(347.6899F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Root",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            return rules;
        }

        public override void Install()
        {
            base.Install();
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
        }
        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GenericSkill.OnExecute -= GenericSkill_OnExecute;
        }
        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            CharacterBody vBody = self.characterBody;
            if (!vBody || !vBody.skillLocator) return; //null check

            var invCount = GetCount(self.characterBody);

            Vector3 corePos = Util.GetCorePosition(vBody);
            GameObject vGameObject = self.gameObject;

            if (vBody.skillLocator.FindSkill(self.skillName))
            {
                if (invCount > 0)
                {
                    if (self.characterBody.skillLocator.utility && self.characterBody.skillLocator.utility.Equals(self))
                    {
                        for (int i = 0; i < invCount; i++)
                        {
                            ProjectileManager.instance.FireProjectile(BombPrefab, corePos, RollBombFireDirection(),
                                              vGameObject, vBody.damage * RollBomb_Damage,
                                              3f, Util.CheckRoll(vBody.crit, vBody.master),
                                              DamageColorIndex.Item, null, -1f);
                        }
                    }
                }
            }
            orig(self);
        }
        private Quaternion RollBombFireDirection() //credit: chen
        {
            return Util.QuaternionSafeLookRotation(new Vector3(Random.Range(-10,10), 0f, Random.Range(-10, 10)));
        }
    }
}