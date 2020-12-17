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
        protected override string GetPickupString(string langID = null) => "Sidekick No More\nBoosts stats when alone.";

        protected override string GetDescString(string langid = null) => $"Increases <style=cIsUtility>base movespeed by {statboost}</style>, and <style=cIsDamage>base damage by {statboost}</style> for every dead survivor.";

        protected override string GetLoreString(string langID = null) => "Now that the protagonist is dead, it's time to shine!";

        private readonly int statboost = 6;

        public static GameObject ItemBodyModelPrefab;

        public CultistPassiveItem()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/CultistPassiveItem.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/CultistPassiveIcon.png";
        }
        public override void SetupBehavior()
        {

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
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0.2f, 0.22f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0.1f, 0.2f, 0.13f),
                    localAngles = new Vector3(0f, 0f, 90f),
                    localScale = new Vector3(0.08f, 0.08f, 0.08f)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 3.4f, -1.3f),
                    localAngles = new Vector3(60f, 0f, 0f),
                    localScale = generalScale * 16f
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0.5f, 0.22f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 0.25f),
                    localAngles = new Vector3(0f, 1f, -0.06f),
                    localScale = generalScale * 5f
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0f, 0.13f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0.1f, 0.2f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 1.2f, 0.2f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = generalScale * 8f
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0.05f, 0.15f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 5.2f, 0.3f),
                    localAngles = new Vector3(90f, 0f, 0f),
                    localScale = generalScale * 8
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.04f, 0.18f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[] //todo
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HeadCenter",
                    localPos = new Vector3(0f, 0f, 0.18f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = generalScale
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0.15f, 0.12f),
                    localAngles = new Vector3(-20f, 0f, 0f),
                    localScale = generalScale
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
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
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 0f, 2.4f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 10f
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(0f, 5f, 0f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = generalScale * 20f
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
                    args.baseDamageAdd += statboost * deadAmt;
                    args.baseMoveSpeedAdd += statboost * deadAmt;
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

        private int GetDeadAmount()
        {
            int deadAmt = 0;
            var list = PlayerCharacterMasterController.instances;
            if (list.Count == 1)
                return 1;
            foreach (var player in list)
            {
                if (player.master.IsDeadAndOutOfLivesServer())
                {
                    deadAmt++;
                }
            }
            return deadAmt;
        }

        private void UpdateComponentForEveryone()
        {
            int AmountDead = GetDeadAmount();
            var list = PlayerCharacterMasterController.instances;
            foreach (var player in list)
            {
                var body = player.master?.GetBody();
                if (body)
                {
                    var passiveComponent = body.GetComponent<CultistPassiveComponent>();
                    if (passiveComponent) passiveComponent.deadProtagonists = AmountDead;
                }
            }
            
        }

        public class CultistPassiveComponent : MonoBehaviour
        {
            public int deadProtagonists = 0;
        }
    }
}
