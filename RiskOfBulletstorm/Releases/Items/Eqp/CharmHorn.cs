using RiskOfBulletstorm.Utils;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class CharmHorn : Equipment_V2<CharmHorn>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius to charm enemies? (Value: Meters)", AutoConfigFlags.PreventNetMismatch)]
        public float CharmHorn_Radius { get; private set; } = 20f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the duration enemies are charmed?", AutoConfigFlags.PreventNetMismatch)]
        public float CharmHorn_Duration { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 85.00f;

        public override string displayName => "Charm Horn";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "The Call Of Duty\nWhen blown, this horn will call those nearby to aid you.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Upon use, blows the horn to <style=cIsUtility>charm</style> enemies within <style=cIsUtility>{CharmHorn_Radius} meters</style> for {CharmHorn_Duration} seconds.";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "There are strange inconsistencies in the behavior of the Gundead. Originally thought to be heartless killing machines, they have been known to capture certain invaders for unknown purposes. Furthermore, evidence of a crude religion has been discovered. Perhaps, one day, they could be reasoned with?";

        public static GameObject CharmWardPrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;

        public CharmHorn()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/CharmHorn.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/CharmHorn.png";
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();

            GameObject warbannerPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard");
            CharmWardPrefab = warbannerPrefab.InstantiateClone("Bulletstorm_CharmHornWard");

            BuffWard buffWard = CharmWardPrefab.GetComponent<BuffWard>();
            buffWard.expires = true;
            buffWard.expireDuration = 0.6f;
            buffWard.buffDuration = CharmHorn_Duration;
            buffWard.invertTeamFilter = true;
            buffWard.animateRadius = false;
            buffWard.floorWard = false;
            buffWard.buffType = Shared.Buffs.BuffsController.Charm;

            SkinnedMeshRenderer mesh = CharmWardPrefab.GetComponentInChildren<SkinnedMeshRenderer>();
            mesh.material.color = new Color32(217, 20, 194, 255);
            if (CharmWardPrefab) PrefabAPI.RegisterNetworkPrefab(CharmWardPrefab);
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

            Vector3 generalScale = new Vector3(0.05f, 0.05f, 0.05f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.0621F, -0.0474F, 0.106F),
localAngles = new Vector3(15.6159F, 257.5474F, 97.0593F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.0861F, -0.0975F, 0.0275F),
localAngles = new Vector3(31.9993F, 189.4607F, 98.4429F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0f, 1f, 1.1f),
                    localAngles = new Vector3(290f, 60f, -5f),
                    localScale = generalScale * 10f
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.18f, 0f, 0f),
                    localAngles = new Vector3(20f, 90f, 90f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.1166F, 0.0031F, 0.0357F),
localAngles = new Vector3(13.4546F, 233.7623F, 102.1007F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.1267F, -0.0107F, -0.0233F),
localAngles = new Vector3(11.7633F, 223.1471F, 77.6447F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighBackR",
                    localPos = new Vector3(0.7f, 0.1f, 0.6f),
                    localAngles = new Vector3(30f, -90f, 90f),
                    localScale = generalScale * 2
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0.37f, -0.1f, 0f),
                    localAngles = new Vector3(0f, -90f, 90f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-1.76f, 0f, 0f),
                    localAngles = new Vector3(0f, -110f, 90f),
                    localScale = generalScale * 8
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.1039F, -0.2101F, 0.2421F),
localAngles = new Vector3(353.9362F, 297.0466F, 97.9667F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.15f, 0f, 0.1f),
                    localAngles = new Vector3(0, -90, 90),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(-0.0046F, 0.0135F, 0.1629F),
localAngles = new Vector3(7.5955F, 108.9757F, 81.1478F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Root",
                    localPos = new Vector3(0.4f, 1.4f, -0.3f),
                    localAngles = new Vector3(0f, 15f, -75f),
                    localScale = generalScale * 2f
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0.8f, 0.3f, 1f),
                    localAngles = new Vector3(353f, 40f, 90f),
                    localScale = generalScale * 10f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-3f, 0f, 2f),
                    localAngles = new Vector3(351f, 270f, 120f),
                    localScale = generalScale * 20f
                }
            });
            rules.Add("mdlEquipmentDrone", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunBarrelBase",
                    localPos = new Vector3(0f, 1.65f, 0.35f),
                    localAngles = new Vector3(0f, 90f, -80f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
        }
            }); 
            rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4628F, 0.1143F, 0.3728F),
                localAngles = new Vector3(316.3471F, 309.305F, 217.9808F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4957F, 0.1282F, 0.3521F),
                localAngles = new Vector3(330.3268F, 318.3562F, 295.7811F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
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
        public override void SetupLate()
        {
            base.SetupLate();


        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            GameObject gameObject = slot.gameObject;
            if (!gameObject || !body) return false;
            float multiplier = 1.0f;

            CharmNearby(body, multiplier);
            return true;
        }

        public void CharmNearby(CharacterBody body, float radius)
        {
            if (NetworkServer.active)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(CharmWardPrefab, body.transform.position, Quaternion.identity);
                gameObject.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                BuffWard buffWard = gameObject.GetComponent<BuffWard>();
                buffWard.buffType = Shared.Buffs.BuffsController.Charm;
                buffWard.GetComponent<BuffWard>().Networkradius *= radius;
                buffWard.GetComponent<BuffWard>().radius *= radius;
                NetworkServer.Spawn(gameObject);
            }
        }

    }
}
