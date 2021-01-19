using RiskOfBulletstorm.Utils;
using System.Collections.Generic;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class MagazineRack : Equipment_V2<MagazineRack>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius in meters of the Magazine Rack's zone?", AutoConfigFlags.PreventNetMismatch)]
        public float MagazineRack_Radius { get; private set; } = 3.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the duration of the Magazine Rack's zone?", AutoConfigFlags.PreventNetMismatch)]
        public float MagazineRack_Duration { get; private set; } = 2.50f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown in seconds?", AutoConfigFlags.PreventNetMismatch)]
        public override float cooldown { get; protected set; } = 90.00f;

        public override string displayName => "Magazine Rack";

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Instant Mail Order\nPlace to create a zone of (almost) no cooldowns.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Place to create a zone of infinite ammo within a radius of {MagazineRack_Radius} meters";
            desc += $"that lasts {MagazineRack_Duration} seconds.";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "Often found in gungeon doctors´ offices, this rack displays magazines of all sorts. The clips contained within should prove useful, and plentiful, for even the most inaccurate of Gungeoneers.";

        public static GameObject MagazinePrefab { get; private set; }

        public static GameObject ItemBodyModelPrefab;
        public MagazineRack()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/MagazineRack.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/MagazineRackIcon.png";
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            GameObject warbannerPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard");
            MagazinePrefab = warbannerPrefab.InstantiateClone("Bulletstorm_MagazineRackObject");

            BuffWard buffWard = MagazinePrefab.GetComponent<BuffWard>();
            buffWard.Networkradius = MagazineRack_Radius;
            buffWard.radius = MagazineRack_Radius;
            buffWard.expires = true;
            buffWard.expireDuration = MagazineRack_Duration;
            buffWard.buffType = BuffIndex.NoCooldowns;


            if (MagazinePrefab) PrefabAPI.RegisterNetworkPrefab(MagazinePrefab);

            /*if (HelperPlugin.ClassicItemsCompat.enabled)
            {
                HelperPlugin.ClassicItemsCompat.RegisterEmbryo(catalogIndex);
            }*/
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
        private static ItemDisplayRuleDict GenerateItemDisplayRules()
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
                    childName = "Base",
                    localPos = new Vector3(0.3f, 0f, -1.6f),
                    localAngles = new Vector3(270f, -45f, 0f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
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
                    localPos = new Vector3(0f, 0.04f, 0.18f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0f, 0f, 0.18f),
                    localAngles = new Vector3(0, 0, 180),
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
                    localPos = new Vector3(0.3f, 1.4f, -0.2f),
                    localAngles = new Vector3(-35f, 90f, 0f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "ThighR",
                    localPos = new Vector3(0.6f, 0f, 0f),
                    localAngles = new Vector3(-15f, 90f, 180f),
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

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            GameObject gameObject = slot.gameObject;
            if (!gameObject || !body) return false;

            //bool EmbryoProc = HelperPlugin.ClassicItemsCompat.enabled && HelperPlugin.ClassicItemsCompat.CheckEmbryoProc(instance, body);
            //return PlaceWard(body, MagazinePrefab, EmbryoProc);
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
                NetworkServer.Spawn(gameObject);
                return true;
            }
            return false;
        }
    }
}
