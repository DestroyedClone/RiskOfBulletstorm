﻿using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Items
{
    public class LordOfTheJammedIdentifierItem : ItemBase<LordOfTheJammedIdentifierItem>
    {
        public override string ItemName => "LordOfTheJammedBodyDisplay";

        public override string ItemLangTokenName => "LordOfTheJammedBodyDisplayLOTJDisplay";

        public override ItemTier Tier => ItemTier.NoTier;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => LoadSprite();

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.WorldUnique
        };

        public override void Init(ConfigFile config)
        {
            CreateAssets();
            CreateLang();
            CreateItem();
            Hooks();
        }

        private static readonly Material blackMat = Assets.LoadAddressable<Material>("RoR2/Base/Common/matDebugBlack.mat");

        public static void CreateAssets()
        {
            var yellowRedMat = Assets.LoadAddressable<Material>("RoR2/Base/Beetle/matSulfurBeetle.mat");
            void SetMaterial(GameObject gameObject, Material material)
            {
                SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
                if (!skinnedMeshRenderer)
                {
                    //Main._logger.LogError($"SkinnedMeshRenderer Not Found For {gameObject.name}!"); <- this one
                    MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                    if (!meshRenderer)
                    {
                        //Main._logger.LogError($"MeshRenderer also falied.");
                    }
                    else
                    {
                        meshRenderer.SetMaterial(material);
                    }
                    return;
                }
                skinnedMeshRenderer.SetMaterial(material);
            }

            GameObject clone(string path, string name)
            {
                return PrefabAPI.InstantiateClone(Assets.LoadAddressable<GameObject>(path), name, false);
            }

            skull = clone("RoR2/Base/DeathMark/mdlDeathMark.fbx", "LOTJSkull");
            skull.gameObject.AddComponent<LOTJDisplaySetup>();
            SetMaterial(skull, yellowRedMat);

            scythe = clone("RoR2/Base/HealOnCrit/mdlScythe.fbx", "LOTJWeapon_Scythe");
            var scytheMat = Assets.LoadAddressable<Material>("RoR2/Base/HealOnCrit/matScythe.mat");
            SetMaterial(scythe, scytheMat);

            shotgun = clone("RoR2/Base/Bandit2/BanditShotgun.fbx", "LOTJWeapon_Shotgun");
            var shotgunMat = Assets.LoadAddressable<Material>("RoR2/Base/Bandit2/matBandit2Shotgun.mat");
            SetMaterial(shotgun, shotgunMat);

            crown = clone("RoR2/Base/artifactworld/mdlArtifactCrown.fbx", "LOTJCrown");
            UnityEngine.Object.Destroy(crown.transform.Find("ArtifactCrownJewelMesh").gameObject);
            SetMaterial(crown.transform.Find("ArtifactCrownMesh").gameObject, yellowRedMat);
        }

        public static GameObject skull;
        public static GameObject scythe;
        public static GameObject shotgun;
        public static GameObject crown;

        public class LOTJDisplaySetup : MonoBehaviour
        {
            private SkinnedMeshRenderer meshRenderer;

            public void Start()
            {
                try
                {
                    Transform primaryTransform = transform.parent.parent.parent.parent.parent.parent.parent.parent;
                    primaryTransform.Find("BrotherHammerConcrete").gameObject.SetActive(false);
                    primaryTransform.Find("BrotherStibPieces").gameObject.SetActive(false);
                    primaryTransform.Find("BrotherClothPieces").gameObject.SetActive(false);
                    transform.parent.Find("BrotherHeadArmorMesh").gameObject.SetActive(false);
                    meshRenderer = primaryTransform.Find("BrotherBodyMesh").GetComponent<SkinnedMeshRenderer>();
                }
                catch
                {
                    Debug.Log("there's a snake in my boot!");
                }
            }

            public void FixedUpdate()
            {
                meshRenderer.SetMaterial(blackMat); ;
            }
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = ItemModel;
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = skull,
                    childName = "Head",
                    localPos = new Vector3(0.00142F, 0.05512F, 0.07683F),
                    localAngles = new Vector3(286.3012F, 14.24653F, 351.1197F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = crown,
                    childName = "Head",
localPos = new Vector3(0.5987F, 0.33382F, 0.07866F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = scythe,
                    childName = "HandL",
localPos = new Vector3(-0.66372F, -0.14386F, 0.85188F),
localAngles = new Vector3(14.15931F, 310.6348F, 336.7441F),
localScale = new Vector3(0.46704F, 0.46704F, 0.46704F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = shotgun,
                    childName = "HandL",
localPos = new Vector3(-0.06865F, 0.11579F, 0.14323F),
localAngles = new Vector3(15.76944F, 318.4268F, 346.675F),
localScale = new Vector3(0.03303F, 0.03303F, 0.07704F)
                }
            });
            return rules;
        }

        public override void Hooks()
        {
            base.Hooks();
            Inventory.onServerItemGiven += Inventory_onServerItemGiven;
            //stinky but have to do this while we dont have our own prefab body
            On.RoR2.CharacterBody.GetDisplayName += CharacterBody_GetDisplayName;
            On.RoR2.CharacterBody.GetSubtitle += CharacterBody_GetSubtitle;
            On.EntityStates.NewtMonster.SpawnState.OnEnter += SpawnState_OnEnter;
        }

        //https://github.com/Moffein/BazaarLimit/blob/21b1342309b51cdf560460fd0d11a6ee79ca4a7f/BazaarLimit/Class1.cs#L26
        private static readonly SceneDef bazaarSceneDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/bazaar/bazaar.asset").WaitForCompletion();

        private void SpawnState_OnEnter(On.EntityStates.NewtMonster.SpawnState.orig_OnEnter orig, EntityStates.NewtMonster.SpawnState self)
        {
            if (NetworkServer.active && SceneCatalog.GetSceneDefForCurrentScene() == bazaarSceneDef)
            {
                if (!self.outer.GetComponent<NewtKickFromShopIfLOTJBehaviour>())
                {
                    self.outer.gameObject.AddComponent<NewtKickFromShopIfLOTJBehaviour>();
                }
            }
            orig(self);
        }

        private string CharacterBody_GetSubtitle(On.RoR2.CharacterBody.orig_GetSubtitle orig, CharacterBody self)
        {
            if (GetCount(self) > 0)
            {
                return Language.GetString("RISKOFBULLETSTORM_LORDOFTHEJAMMED_SUBTITLE");
            }
            return orig(self);
        }

        private string CharacterBody_GetDisplayName(On.RoR2.CharacterBody.orig_GetDisplayName orig, CharacterBody self)
        {
            if (GetCount(self) > 0)
            {
                return Language.GetString("RISKOFBULLETSTORM_LORDOFTHEJAMMED_NAME");
            }
            return orig(self);
        }

        private void Inventory_onServerItemGiven(Inventory arg1, ItemIndex arg2, int arg3)
        {
            if (arg2 == ItemDef.itemIndex)
            {
                LordOfTheJammedController controller = arg1.GetComponent<LordOfTheJammedController>();
                if (!controller)
                    controller = arg1.gameObject.AddComponent<LordOfTheJammedController>();
                controller.lordMaster = arg1.GetComponent<CharacterMaster>();
            }
        }

        public class LordOfTheJammedController : MonoBehaviour
        {
            private CharacterBody lordBody;
            public CharacterMaster lordMaster;

            public static GameObject ProjectilePrefab => Assets.LoadAddressable<GameObject>("RoR2/DLC1/VoidBarnacle/VoidBarnacleBullet.prefab");
            private const float damageCoefficient = 1f;
            private const int verticalIntensity = 2;
            private const float horizontalIntensity = 1f;

            public bool subscribed = false;

            public void OnEnable()
            {
                InstanceTracker.Add(this);
            }

            public void OnDisable()
            {
                InstanceTracker.Remove(this);
            }

            public void Start()
            {
                if (!lordMaster)
                {
                    enabled = false;
                    return;
                }

                lordBody = lordMaster.GetBody();
                if (lordBody && !subscribed)
                {
                    lordMaster.onBodyStart += LordMaster_onBodyStart;
                    lordMaster.onBodyDestroyed += LordMaster_onBodyDestroyed;
                }
                LordMaster_onBodyStart(lordBody);
            }

            private void LordMaster_onBodyDestroyed(CharacterBody obj)
            {
                subscribed = false;
            }

            private void LordMaster_onBodyStart(CharacterBody obj)
            {
                lordBody = obj;
                lordBody.onSkillActivatedServer += LordBody_onSkillActivatedServer;

                subscribed = true;
            }

            private void LordBody_onSkillActivatedServer(GenericSkill genericSkill)
            {
                Throw(genericSkill.characterBody);
            }

            //https://github.com/GnomeModder/ChefMod/blob/main/thecook-vs/CHEFMod/States/Primary/Boosted/Mince.cs
            private void Throw(CharacterBody characterBody)
            {
                bool isCrit = characterBody.RollCrit();
                FireProjectileInfo info = new FireProjectileInfo()
                {
                    projectilePrefab = ProjectilePrefab,
                    position = characterBody.corePosition,
                    owner = characterBody.gameObject,
                    //damage = base.characterBody.damage * (4f / (chefPlugin.minceHorizontolIntensity.Value + intensity)),
                    damage = characterBody.damage * damageCoefficient,
                    force = 0f,
                    crit = isCrit,
                    damageColorIndex = DamageColorIndex.Default,
                    target = null,
                };

                Vector3 aimDirection = characterBody.inputBank.GetAimRay().direction;
                aimDirection.y = 0f;
                aimDirection.Normalize();
                float orientation = Mathf.Atan2(aimDirection.z, aimDirection.x);

                for (int i = -1 * verticalIntensity; i <= verticalIntensity; i++)
                {
                    float phi;
                    if (verticalIntensity != 0) phi = i * (1f / (2f * verticalIntensity)) * Mathf.PI;
                    float r = Mathf.Cos(phi);
                    int circum = Mathf.Max(1, Mathf.FloorToInt(horizontalIntensity * Mathf.PI * 2 * r));
                    for (int j = 0; j < circum; j++)
                    {
                        float theta = orientation + 2 * Mathf.PI * ((float)j / (float)circum);
                        Vector3 direction = new Vector3(r * Mathf.Cos(theta), Mathf.Sin(phi), r * Mathf.Sin(theta));

                        info.rotation = Util.QuaternionSafeLookRotation(direction);

                        ProjectileManager.instance.FireProjectile(info);
                    }
                }
            }
        }

        public class NewtKickFromShopIfLOTJBehaviour : MonoBehaviour
        {
            public float age = 0;
            public float duration = 5f;

            public void FixedUpdate()
            {
                age += Time.fixedDeltaTime;
                if (age >= duration)
                {
                    if (InstanceTracker.GetInstancesList<LordOfTheJammedController>().Count > 0)
                    {
                        var charBody = gameObject.GetComponent<CharacterBody>();
                        Tweaks.Bazaar_KickFromShop(charBody);
                        enabled = false;
                    }
                    age = 0;
                }
            }
        }
    }
}