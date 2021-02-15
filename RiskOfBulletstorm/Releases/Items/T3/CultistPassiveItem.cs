using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using RoR2;
using R2API;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;

namespace RiskOfBulletstorm.Items
{
    public class CultistPassiveItem : Item_V2<CultistPassiveItem>
    {

        public override string displayName => "Number 2";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "<b>Sidekick No More</b>\nBoosts stats when alone.";

        protected override string GetDescString(string langid = null) => $"Increases <style=cIsUtility>stats</style> by a certain amount." +
            $"<style=cStack>+same per stack </style> for every dead survivor.";

        protected override string GetLoreString(string langID = null) => "Now that the protagonist is dead, it's time to shine!";

        public static GameObject ItemBodyModelPrefab;

        public CultistPassiveItem()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/CultistPassiveItem.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/CultistPassiveItem";
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>("@RiskOfBulletstorm:Assets/Models/Prefabs/CultistPassiveWorn.prefab");
                displayRules = GenerateItemDisplayRules();
            }

            base.SetupAttributes();
        }
        public static ItemDisplayRuleDict GenerateItemDisplayRules()
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, -0.03f, 0f),
                    localAngles = new Vector3(-20, 0, 0),
                    localScale = new Vector3(0.18f, 0.5f, 0.18f)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0f, -0.055f),
                    localAngles = new Vector3(-45f, 0f, 0f),
                    localScale = new Vector3(0.14f, 0.5f, 0.14f)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 2.5f, 1.5f),
                    localAngles = new Vector3(-90f, 180f, 0f),
                    localScale = new Vector3(1f,4f,1f),
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HeadCenter",
localPos = new Vector3(0.0003F, 0.0458F, -0.0384F),
localAngles = new Vector3(342.5341F, 1.0665F, 359.5671F),
localScale = new Vector3(0.17F, 0.5F, 0.195F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(-30f, 0f, 0f),
                    localScale = new Vector3(0.1f, 0.5f, 0.14f)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0.02f, -0.01f),
                    localAngles = new Vector3(-25f, 0f, 0f),
                    localScale = new Vector3(0.13f, 0.5f, 0.14f)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[] //todo
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 1.2f, 0.2f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = new Vector3(1f,1f,1f)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0.03f, -0.01f),
                    localAngles = new Vector3(-15f, 0f, 0f),
                    localScale = new Vector3(0.14f, 0.5f, 0.15f)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, -0.6f, -0.45f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = new Vector3(1.9f, 6f, 1.8f)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, -0.05f, 0f),
                    localAngles = new Vector3(20f, 0f, 0f),
                    localScale = new Vector3(0.14f, 0.5f, 0.14f)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[] //todo: child
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0f, 0.18f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(2f,2f,2f)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 0f),
                    localAngles = new Vector3(-10f, -45f, -2.167f),
                    localScale = new Vector3(0.1f, 0.5f, 0.1f)
                }
            }) ;
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.4f, 0.07f),
                    localAngles = new Vector3(6f, 0f, 0f),
                    localScale = new Vector3(0.16f, 1f, 0.16f)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.65f, 2f, 0.6f),
                    localAngles = new Vector3(-5f, 0f, 0f),
                    localScale = new Vector3(0.8f, 4f, 0.7f)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, -3.5f, -3f),
                    localAngles = new Vector3(120f, 180f, 180f),
                    localScale = new Vector3(8.5f, 30f, 7f)
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
            GetStatCoefficients += CultistPassiveItem_GetStatCoefficients;
            On.RoR2.Stage.RespawnCharacter += Stage_RespawnCharacter;
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath += GlobalEventManager_OnPlayerCharacterDeath;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Adding component in case it doesnt exist then modifying it")]
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            int InventoryCount = GetCount(self);
            if (InventoryCount > 0)
            {
                CultistPassiveComponent passiveComponent = self.gameObject.GetComponent<CultistPassiveComponent>();
                if (!passiveComponent) { passiveComponent = self.gameObject.AddComponent<CultistPassiveComponent>(); }
                UpdateComponentForEveryone();
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients -= CultistPassiveItem_GetStatCoefficients;
            On.RoR2.Stage.RespawnCharacter -= Stage_RespawnCharacter;
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath -= GlobalEventManager_OnPlayerCharacterDeath;
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
        }

        private void CultistPassiveItem_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            var component = sender.GetComponent<CultistPassiveComponent>();
            var InventoryCount = GetCount(sender);
            if (InventoryCount > 0)
            {
                if (component)
                {
                    var deadAmt = component.deadProtagonists;

                    //float addAmount = CPI_singleplayer;
                    //if (component.isMultiplayer) addAmount = CPI_multiplayer;
                    
                    float multAmount = deadAmt * InventoryCount;

                    args.baseAttackSpeedAdd += 0.4f * multAmount;
                    args.baseDamageAdd += 10f * multAmount;
                    args.healthMultAdd += multAmount;
                    args.baseMoveSpeedAdd += 1f * multAmount;
                    args.regenMultAdd += 0.5f * multAmount;
                    args.armorAdd += 8 * multAmount;
                    args.critAdd += 15 * multAmount;
                }
            }
        }

        private void Stage_RespawnCharacter(On.RoR2.Stage.orig_RespawnCharacter orig, Stage self, CharacterMaster characterMaster)
        {
            orig(self, characterMaster);
            UpdateComponentForEveryone();
        }

        private void GlobalEventManager_OnPlayerCharacterDeath(On.RoR2.GlobalEventManager.orig_OnPlayerCharacterDeath orig, GlobalEventManager self, DamageReport damageReport, NetworkUser victimNetworkUser)
        {
            orig(self, damageReport, victimNetworkUser);
            UpdateComponentForEveryone();
        }

        private int[] GetDeadAmount()
        {
            int deadAmt = 0;
            var list = PlayerCharacterMasterController.instances;
            // return: dead Amount; 0 = singleplayer, 1 = multiplayer
            if (list.Count == 1)
                return new int[] { 1, 0 };
            foreach (var player in list)
            {
                if (player.master.IsDeadAndOutOfLivesServer())
                {
                    deadAmt++;
                }
            }
            return new int[] { deadAmt, 1 };
        }

        private void UpdateComponentForEveryone()
        {
            var result = GetDeadAmount();
            int AmountDead = result[0];
            bool isMultiplayer = result[1] == 1;
            var list = PlayerCharacterMasterController.instances;
            foreach (var player in list)
            {
                var body = player.master?.GetBody();
                if (body)
                {
                    var passiveComponent = body.GetComponent<CultistPassiveComponent>();
                    if (passiveComponent)
                    {
                        passiveComponent.deadProtagonists = AmountDead;
                        passiveComponent.isMultiplayer = isMultiplayer;
                    }
                }
            }
            
        }

        public class CultistPassiveComponent : MonoBehaviour
        {
            public int deadProtagonists = 0;
            public bool isMultiplayer = false;
        }
    }
}
