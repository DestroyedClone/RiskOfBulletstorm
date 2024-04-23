using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.GameplayAdditions;
using RiskOfBulletstormRewrite.Modules;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RiskOfBulletstormRewrite.Characters.Enemies.LordofTheJammedMonster;

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

        public class NewtKickFromShopIfLOTJBehaviour : MonoBehaviour
        {
            public float age = 0;
            public float duration = 5f;

            public CharacterBody newtBody;

            public void Start()
            {
                newtBody = GetComponent<CharacterBody>();
            }

            public void FixedUpdate()
            {
                age += Time.fixedDeltaTime;
                if (age >= duration)
                {
                    if (InstanceTracker.GetInstancesList<LordOfTheJammedBodyBehaviour>().Count > 0)
                    {
                        MechanicStealing.ForceNewtToKickPlayersFromShop(newtBody, true);
                        enabled = false;
                    }
                    age = 0;
                }
            }
        }
    }
}