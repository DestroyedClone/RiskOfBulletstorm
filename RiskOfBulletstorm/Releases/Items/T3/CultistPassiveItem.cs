﻿using RiskOfBulletstorm.Utils;
using System.Collections.ObjectModel;
using RoR2;
using R2API;
using UnityEngine;
using TILER2;
using static R2API.RecalculateStatsAPI;
using static RiskOfBulletstorm.BulletstormPlugin;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class CultistPassiveItem : Item<CultistPassiveItem>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("For each stack, what additional fraction of the stats should it contribute?", AutoConfigFlags.PreventNetMismatch)]
        public float StackModifier { get; private set; } = 0.2f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much attack speed is added per dead player?", AutoConfigFlags.PreventNetMismatch)]
        public float BaseAttackSpeedAdd { get; private set; } = 0.2f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much base damage is added per dead player?", AutoConfigFlags.PreventNetMismatch)]
        public float BaseDamageAdd { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percentage of max health is added per dead player?", AutoConfigFlags.PreventNetMismatch)]
        public float HealthMultAdd { get; private set; } = 0.25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much move speed is added per dead player?", AutoConfigFlags.PreventNetMismatch)]
        public float BaseMoveSpeedAdd { get; private set; } = 0.25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percentage of regen is added per dead player?", AutoConfigFlags.PreventNetMismatch)]
        public float RegenMultAdd { get; private set; } = 0.25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much armor is added per dead player?", AutoConfigFlags.PreventNetMismatch)]
        public float ArmorAdd { get; private set; } = 4f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much crit chance is added per dead player?", AutoConfigFlags.PreventNetMismatch)]
        public float CritAdd { get; private set; } = 7.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of health is healed upon another player dying?", AutoConfigFlags.PreventNetMismatch)]
        public float HealFractionOnDeath { get; private set; } = 0.55f;

        public override string displayName => "Number 2";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "<b>Sidekick No More</b>\nBoosts stats when alone.";

        protected override string GetDescString(string langid = null) => $"Increases" +
            $"\n <style=cIsDamage>attack speed by +{Pct(BaseAttackSpeedAdd)}</style> <style=cStack>(+{Pct(BaseAttackSpeedAdd*StackModifier)} per stack)</style>," +
            $"\n <style=cIsDamage>base damage by {BaseDamageAdd}</style> <style=cStack>(+{BaseDamageAdd*StackModifier} per stack)</style>," +
            $"\n <style=cIsHealing>health by +{Pct(HealthMultAdd)}</style> <style=cStack>(+{Pct(HealthMultAdd*StackModifier)} per stack)</style>," +
            $"\n <style=cIsUtility>move speed by +{BaseMoveSpeedAdd}</style> <style=cStack>(+{BaseMoveSpeedAdd*StackModifier} per stack)</style>," +
            $"\n <style=cIsHealing>multiplies regen by +{Pct(RegenMultAdd)}</style> <style=cStack>(+{Pct(RegenMultAdd*StackModifier)} per stack)</style>," +
            $"\n <style=cIsDamage>increases armor by +{ArmorAdd} <style=cStack>(+{ArmorAdd*StackModifier} per stack)</style>," +
            $"\n <style=cIsDamage>increases crit chance by +{CritAdd}%</style> <style=cStack>(+{CritAdd*StackModifier}% per stack)</style>" +
            $"\n for every dead player." +
            $"\n Restores <style=cIsHealing>{Pct(HealFractionOnDeath)} health</style> <style=cStack>(+{Pct(HealFractionOnDeath*StackModifier)} per stack)</style> on a players' death.";

        protected override string GetLoreString(string langID = null) => "Now that the protagonist is dead, it's time to shine!";

        public static GameObject ItemBodyModelPrefab;

        public CultistPassiveItem()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/CultistPassiveItem.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/CultistPassiveItem");
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/CultistPassiveWorn.prefab");
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
                    childName = "Head",
                    localPos = new Vector3(0, 0, 0),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(0, 0, 0)
                }
            });
            rules.Add("mdlCommandoDualies", new ItemDisplayRule[]
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
localPos = new Vector3(0F, 2.4394F, 1.0901F),
localAngles = new Vector3(286.8943F, 180F, 0F),
localScale = new Vector3(1.0389F, 4.1555F, 1.0389F)
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
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HeadBase",
localPos = new Vector3(0F, -0.1712F, 0F),
localAngles = new Vector3(0F, 180F, 180F),
localScale = new Vector3(0.6851F, 1.4182F, 0.6851F)
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
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.0014F, 0.0352F, 0.0613F),
localAngles = new Vector3(340.3147F, 359.7245F, 0.0101F),
localScale = new Vector3(0.178F, 0.6646F, 0.1751F)
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
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Head",
                    localPos = new Vector3(-0.00002F, 0.1508F, 0.0022F),
                    localAngles = new Vector3(350.9738F, 0F, 0F),
                    localScale = new Vector3(0.12F, 0.4F, 0.12F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.0001F, 0.3595F, 0.0769F),
localAngles = new Vector3(354.1696F, 359.9734F, 359.9908F),
localScale = new Vector3(0.16F, 1F, 0.16F)
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
localPos = new Vector3(0F, -3.6779F, -2.9251F),
localAngles = new Vector3(309.7346F, 179.9999F, 179.9999F),
localScale = new Vector3(8.5F, 30F, 7F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.0048F, 0.1876F, 0.2319F),
                localAngles = new Vector3(322.444F, 178.8064F, 0F),
                localScale = new Vector3(0.3342F, 0.9389F, 0.3342F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.2429F, 1.2372F, -0.3837F),
                localAngles = new Vector3(287.3049F, 211.5149F, 147.8106F),
                localScale = new Vector3(1.2967F, 4.8235F, 2.3057F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLT",
                localPos = new Vector3(-1.6634F, -0.0026F, -0.6294F),
                localAngles = new Vector3(10.2827F, 0F, 180F),
                localScale = new Vector3(0.4049F, 1.6994F, 0.2469F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(0.002F, 0.3798F, 1.0963F),
                localAngles = new Vector3(15.0241F, 0F, 0F),
                localScale = new Vector3(1.2244F, 2.6335F, 1.2244F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Mask",
                localPos = new Vector3(-0.0752F, -1.6106F, 1.492F),
                localAngles = new Vector3(274.5536F, 167.4134F, 14.3378F),
                localScale = new Vector3(1.9782F, 7.3248F, 2.6485F)
            });
            return rules;
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();
            if (Compat_ItemStats.enabled)
            {
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return (BaseAttackSpeedAdd + BaseAttackSpeedAdd * (count - 1) * StackModifier) * GetMasterDeadAmt(master); },
                    (value, inv, master) => { return $"Attack Speed: +{Pct(value)}"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return (BaseDamageAdd + BaseDamageAdd * (count - 1) * StackModifier) * GetMasterDeadAmt(master); },
                    (value, inv, master) => { return $"Base Damage: +{value}"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return (HealthMultAdd + HealthMultAdd * (count - 1) * StackModifier) * GetMasterDeadAmt(master); },
                    (value, inv, master) => { return $"Health Multiplier: +{Pct(value)}"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return (BaseMoveSpeedAdd * BaseMoveSpeedAdd * (count - 1) * StackModifier) * GetMasterDeadAmt(master); },
                    (value, inv, master) => { return $"Move Speed: +{value}"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return (RegenMultAdd * RegenMultAdd * (count - 1) * StackModifier) * GetMasterDeadAmt(master); },
                    (value, inv, master) => { return $"Regen Multiplier: +{Pct(value)}"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return (ArmorAdd + ArmorAdd * (count - 1) * StackModifier) * GetMasterDeadAmt(master); },
                    (value, inv, master) => { return $"Armor: +{value}"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return (CritAdd + CritAdd * (count - 1) * StackModifier) * GetMasterDeadAmt(master); },
                    (value, inv, master) => { return $"Crit Chance: +{value}%"; }
                ));
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return (GetMasterDeadAmt(master)); },
                    (value, inv, master) => { return $"Dead Players: {value}"; }
                ));
            }
        }
        public int GetMasterDeadAmt(CharacterMaster master)
        {
            if (master)
            {
                var comp = master.gameObject.GetComponent<CultistPassiveComponent>();
                if (comp)
                {
                    return comp.deadProtagonists;
                }    
            }
            return 0;
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

                    if (deadAmt > 0)
                    {
                        //float addAmount = CPI_singleplayer;
                        //if (component.isMultiplayer) addAmount = CPI_multiplayer;

                        args.baseAttackSpeedAdd += (BaseAttackSpeedAdd + BaseAttackSpeedAdd * (InventoryCount - 1) * StackModifier) * deadAmt;
                        args.baseDamageAdd += (BaseDamageAdd + BaseDamageAdd * (InventoryCount - 1) * StackModifier) * deadAmt;
                        args.healthMultAdd += (HealthMultAdd + HealthMultAdd * (InventoryCount - 1) * StackModifier) * deadAmt;
                        args.baseMoveSpeedAdd += (BaseMoveSpeedAdd + BaseMoveSpeedAdd * (InventoryCount - 1) * StackModifier) * deadAmt;
                        args.regenMultAdd += (RegenMultAdd + RegenMultAdd * (InventoryCount - 1) * StackModifier) * deadAmt;
                        args.armorAdd += (ArmorAdd + ArmorAdd * (InventoryCount - 1) * StackModifier) * deadAmt;
                        args.critAdd += (CritAdd + CritAdd * (InventoryCount - 1) * StackModifier) * deadAmt;
                    }
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
                        var previousAmountDead = passiveComponent.deadProtagonists;
                        passiveComponent.deadProtagonists = AmountDead;
                        passiveComponent.isMultiplayer = isMultiplayer;

                        if (body.healthComponent?.alive == true && previousAmountDead != AmountDead)
                        {
                            body.healthComponent.HealFraction(HealFractionOnDeath, default);
                        }
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
