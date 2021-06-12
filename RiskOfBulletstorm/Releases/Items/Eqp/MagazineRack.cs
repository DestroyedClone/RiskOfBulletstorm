using RiskOfBulletstorm.Utils;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class MagazineRack : Equipment<MagazineRack>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius in meters of the Magazine Rack's zone?", AutoConfigFlags.PreventNetMismatch)]
        public float EffectRadius { get; private set; } = 3.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the duration of the Magazine Rack's zone?", AutoConfigFlags.PreventNetMismatch)]
        public float EffectDuration { get; private set; } = 2.50f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 90.00f;

        public override string displayName => "Magazine Rack";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>Instant Mail Order</b>\nPlace to create a zone of (almost) no cooldowns.";

        protected override string GetDescString(string langid = null)
        {
            return  $"Place to create a zone of <style=cIsUtility>infinite ammo</style>" +
                $" within a radius of <style=cIsUtility>{EffectRadius} meters</style>" +
                $" that lasts <style=cIsUtility>{EffectDuration} seconds</style>.";
        }

        protected override string GetLoreString(string langID = null) => "Often found in gungeon doctors\' offices, this rack displays magazines of all sorts. The clips contained within should prove useful, and plentiful, for even the most inaccurate of Gungeoneers.";

        public static GameObject MagazinePrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;
        public MagazineRack()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/MagazineRack.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/MagazineRack.png");
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            GameObject warbannerPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard");
            MagazinePrefab = warbannerPrefab.InstantiateClone("Bulletstorm_MagazineRackObject", true);

            BuffWard buffWard = MagazinePrefab.GetComponent<BuffWard>();
            buffWard.Networkradius = EffectRadius;
            buffWard.radius = EffectRadius;
            buffWard.expires = true;
            buffWard.expireDuration = EffectDuration;
            buffWard.buffDef = RoR2Content.Buffs.NoCooldowns;

            if (MagazinePrefab) PrefabAPI.RegisterNetworkPrefab(MagazinePrefab);
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

            Vector3 generalScale = new Vector3(0.05f, 0.05f, 0.05f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
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
            rules.Add("mdlCommando", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.05f, 0.05f, -0.1f),
                    localAngles = new Vector3(0, 225, 180),
                    localScale = new Vector3(0.05f, 0.05f, 0.05f)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.1f, 0.2f, 0f),
                    localAngles = new Vector3(0f, 230f, 180f),
                    localScale = new Vector3(0.05f, 0.05f, 0.05f)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0f, 0f, 0.53f),
                    localAngles = new Vector3(0f, 5f, 160f),
                    localScale = new Vector3(0.4f, 0.4f, 0.4f)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.06f, 0.03f, 0.11f),
                    localAngles = new Vector3(5f, -40f, 180f),
                    localScale = new Vector3(0.05f, 0.05f, 0.05f)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.08f, 0.3f, 0.08f),
                    localAngles = new Vector3(0f, -90f, 180f),
                    localScale = new Vector3(0.05f, 0.05f, 0.05f)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.04f, 0.2f, -0.12f),
                    localAngles = new Vector3(0f, 200f, 180f),
                    localScale = new Vector3(0.05f, 0.05f, 0.05f)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighFrontL",
localPos = new Vector3(-0.2938F, 0.2732F, -0.3406F),
localAngles = new Vector3(357.2915F, 137.2046F, 170.4112F),
localScale = new Vector3(0.0757F, 0.0757F, 0.0757F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-0.14f, 0.16f, -0.02f),
                    localAngles = new Vector3(0f, 210f, 180f),
                    localScale = new Vector3(0.05f, 0.05f, 0.05f)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-1.2f, 1f, 0f),
                    localAngles = new Vector3(0f, -90f, 180f),
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
localPos = new Vector3(-0.0075F, -0.0243F, 0.1858F),
localAngles = new Vector3(350.2282F, 353.2943F, 179.4433F),
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
localPos = new Vector3(-0.0268F, -0.0121F, 0.1306F),
localAngles = new Vector3(0F, 334.0277F, 180F),
localScale = new Vector3(0.0346F, 0.0382F, 0.0382F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0f, 0.15f, 0.12f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Root",
localPos = new Vector3(0.3F, 1.4F, -0.2F),
localAngles = new Vector3(325F, 90F, 0F),
localScale = new Vector3(0.0673F, 0.0673F, 0.0673F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "ThighR",
localPos = new Vector3(0.4197F, -0.0524F, 0.0189F),
localAngles = new Vector3(344.7791F, 96.3275F, 177.9618F),
localScale = new Vector3(0.2666F, 0.2666F, 0.2666F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(-2.6f, 0.8f, 0f),
                    localAngles = new Vector3(-20f, -90f, 180f),
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
                    localPos = new Vector3(0f, 0f, 1.6f),
                    localAngles = new Vector3(0f, 90f, 270f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
        }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4656F, 0.2195F, 0.2575F),
                localAngles = new Vector3(13.5538F, 54.0122F, 332.8876F),
                localScale = new Vector3(0.0833F, 0.0833F, 0.0833F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Hip",
                localPos = new Vector3(1.8362F, 0.502F, 0.449F),
                localAngles = new Vector3(22.8307F, 79.628F, 180.4365F),
                localScale = new Vector3(0.3292F, 0.3292F, 0.3292F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLB",
                localPos = new Vector3(0.3266F, 0.115F, -0.5678F),
                localAngles = new Vector3(359.9673F, 88.4625F, 176.1718F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "FrontFootR",
                localPos = new Vector3(0.1592F, -2.6972F, -0.3423F),
                localAngles = new Vector3(358.1402F, 151.3301F, 198.5439F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "FootR",
                localPos = new Vector3(-0.5431F, 0.2425F, 1.1356F),
                localAngles = new Vector3(1.8854F, 271.646F, 292.3944F),
                localScale = new Vector3(0.1436F, 0.1436F, 0.1436F)
            });
            return rules;
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            GameObject gameObject = slot.gameObject;
            if (!gameObject || !body) return false;

            return PlaceWard(body, MagazinePrefab, false);
        }

        public bool PlaceWard(CharacterBody body, GameObject wardObject, bool embryoProc)
        {
            if (NetworkServer.active)
            {
                GameObject gameObject = UnityEngine.Object.Instantiate(wardObject, body.transform.position, Quaternion.identity);
                gameObject.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                gameObject.GetComponent<BuffWard>().Networkradius *= embryoProc ? 1.5f : 1f;
                gameObject.GetComponent<BuffWard>().expireDuration *= embryoProc ? 1.2f : 1f;
                gameObject.GetComponent<BuffWard>().buffDef = RoR2Content.Buffs.NoCooldowns;
                NetworkServer.Spawn(gameObject);
                return true;
            }
            return false;
        }
    }
}
