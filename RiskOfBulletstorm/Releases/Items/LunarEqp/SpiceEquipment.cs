﻿using RiskOfBulletstorm.Utils;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using GenericNotification = On.RoR2.UI.GenericNotification;
using System;
using UnityEngine.Networking;
using static RiskOfBulletstorm.BulletstormPlugin;
using static R2API.RecalculateStatsAPI;

namespace RiskOfBulletstorm.Items
{
    public class Spice : Equipment<Spice>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Limit of Spice per player", AutoConfigFlags.PreventNetMismatch)]
        public int SpiceEquipment_MaxPerPlayer { get; private set; } = 40;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Spice only affects the pickups of the person running Spice. Experimental.", AutoConfigFlags.PreventNetMismatch)]
        public bool SpiceEquipment_Disconnect { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the amount of spice consumed be shown in the inventory?", AutoConfigFlags.PreventNetMismatch)]
        public bool SpiceEquipment_ShowTally { get; private set; } = true;
        public override string displayName => "Spice";
        public override float cooldown { get; protected set; } = 1f;
        public override bool isEnigmaCompatible => false;
        public override bool isLunar => true;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<style=cIsUtility>Increases combat prowess</style>, <style=cDeath>with absolutely no downside!</style>";

        protected override string GetDescString(string langid = null) => $"Progressively gives stat <style=cIsUtility>bonuses</style> and <style=cDeath>downsides</style>. <style=cIsUtility>Consumes</style> on use.";

        protected override string GetLoreString(string langID = null) => "A potent gun-enhancing drug from the far reaches of the galaxy. It is known to be extremely addictive, and extremely expensive.";

        public static ItemDef SpiceTally { get; private set; }
        public static ItemDef CurseDef => CurseController.instance.curseTally;

        public static GameObject ItemBodyModelPrefab;

        public string[] SpiceDescArray =
        {
            "A tantalizing cube of power.",
            "One more couldn't hurt.",
            "Just one more hit...",
            "MORE"
        };

        /*
        //health, attack speed, shot accuracy, enemy bullet speed, damage
        public static readonly float[,] SpiceBonusesAdditive = new float[,]
        {
            { 0f,   0f,      0f,    0f,     0f },
            { +1f,  +0.2f,  +0.25f, 0f,     0f },
            { +1f,  +0.2f,  0f,     -0.1f,  0f }, 
            { -1f,  0f,     0f,     -0.05f, +0.2f }, 
            { -1f,  0f,     -0.1f,  0f,     +0.15f }, 
        };
        */ //documentation purposes

        public static float heartValue = 0.25f;
        public static float atkSpdBonus = 0.2f;


        public static readonly float[,] SpiceBonusesConstant = new float[,]
        {
            { 0f,               0f,                 0f,     0f,     0f }, //0
            { +heartValue,      +atkSpdBonus,       +0.25f, 0f,     0f }, //1
            { +heartValue*2f,   +atkSpdBonus*2f,    +0.25f, -0.1f,  0f }, //2
            { +heartValue,      +atkSpdBonus*2f,    +0.25f, -0.15f, +0.2f }, //3
            { 0f,               +atkSpdBonus*2f,    +0.15f, -0.15f, +0.35f }, //4
            { 0f,               +atkSpdBonus*2f,    +0f,    -0.15f, +0.5f }, //maxed
        };
        public static readonly float[] SpiceBonusesAdditive = new float[] 
            { -0.05f,           0f,                 -0.1f,  0f,     +0.15f };

        public Spice()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/Spice.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/Spice.png");
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = modelResource;
                displayRules = GenerateItemDisplayRules();
            }

            base.SetupAttributes();

            SpiceTally = ScriptableObject.CreateInstance<ItemDef>();
            SpiceTally.hidden = !SpiceEquipment_ShowTally;
            SpiceTally.name = SpiceEquipment_ShowTally ? "SpiceTally" : modInfo.shortIdentifier + "INTERNALSPICECOUNT";
            SpiceTally.tier = ItemTier.NoTier;
            SpiceTally.canRemove = false;
            SpiceTally.descriptionToken = modInfo.shortIdentifier + "_SPICETALLY_DESC";
            SpiceTally.pickupToken = SpiceTally.descriptionToken;
            SpiceTally.nameToken = "ITEM_SPICETALLY_NAME";
            SpiceTally.loreToken = "";
            SpiceTally.pickupIconSprite = iconResource;
            ItemAPI.Add(new CustomItem(SpiceTally, new ItemDisplayRuleDict()));
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
            rules.Add("mdlCommandoDualies", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Pelvis",
                    localPos = new Vector3(0.3127F, 0.2268F, 0.0115F),
                    localAngles = new Vector3(35.055F, 324.7142F, 131.8756F),
                    localScale = new Vector3(0.0149F, 0.0149F, 0.0149F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandR",
localPos = new Vector3(-0.0226F, 0.1215F, -0.0539F),
localAngles = new Vector3(325.304F, 180F, 180F),
localScale = new Vector3(0.0145F, 0.0145F, 0.0145F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0F, 2.0949F, 0.421F),
localAngles = new Vector3(55.8452F, 0F, 0F),
localScale = new Vector3(0.5228F, 0.5228F, 0.5228F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandL",
localPos = new Vector3(0.2957F, 0.3298F, -0.0397F),
localAngles = new Vector3(345.2619F, 352.6291F, 109.8122F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)
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
childName = "LowerArmL",
localPos = new Vector3(0F, 0.4349F, 0.0062F),
localAngles = new Vector3(75.537F, 180F, 180F),
localScale = new Vector3(0.0214F, 0.0214F, 0.0214F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "LowerArmR",
localPos = new Vector3(0.0128F, 0.452F, 0.0275F),
localAngles = new Vector3(47.0482F, 173.5791F, 194.3338F),
localScale = new Vector3(0.0171F, 0.0171F, 0.0171F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "PlatformBase",
localPos = new Vector3(0.0279F, 1.4727F, -0.0124F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.0706F, 0.0706F, 0.0706F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.1039F, 0.3292F, -0.0189F),
localAngles = new Vector3(0.1713F, 0.9598F, 348.9969F),
localScale = new Vector3(0.0238F, 0.0238F, 0.0238F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Finger31L",
localPos = new Vector3(0.2254F, 0.2366F, -0.7522F),
localAngles = new Vector3(344.7061F, 343.9162F, 290.9268F),
localScale = new Vector3(0.2799F, 0.2799F, 0.2799F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Pelvis",
localPos = new Vector3(-0.2401F, -0.1163F, 0.0168F),
localAngles = new Vector3(0F, 0F, 180.6862F),
localScale = new Vector3(0.0268F, 0.0268F, 0.0268F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "UpperArmL",
localPos = new Vector3(-0.0153F, -0.0727F, 0.0077F),
localAngles = new Vector3(3.0261F, 151.0953F, 156.1571F),
localScale = new Vector3(0.0262F, 0.0231F, 0.0262F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Stomach",
localPos = new Vector3(-0.1298F, 0.075F, 0.0911F),
localAngles = new Vector3(3.969F, 0F, 0F),
localScale = new Vector3(0.0211F, 0.0211F, 0.0211F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "HandL",
                    localPos = new Vector3(0.01788F, 0.12478F, -0.01679F),
                    localAngles = new Vector3(40.54588F, 301.7927F, 121.0838F),
                    localScale = new Vector3(0.0149F, 0.0149F, 0.0149F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Root",
localPos = new Vector3(0.1513F, 2.4752F, -0.5664F),
localAngles = new Vector3(4.1557F, 358.4928F, 340.0452F),
localScale = new Vector3(0.0696F, 0.0696F, 0.0696F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(-0.2881F, 1.5729F, 0.419F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.5F, 0.0694F, 0.5F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandL",
localPos = new Vector3(0.4052F, 3.0006F, -0.1408F),
localAngles = new Vector3(348.0934F, 258.0539F, 186.6205F),
localScale = new Vector3(0.4205F, 0.4205F, 0.4205F)
                }
            });
            rules.Add("mdlEquipmentDrone", new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunBarrelBase",
                    localPos = new Vector3(0f, 0f, 1.7f),
                    localAngles = new Vector3(0f, 0f, 90f),
                    localScale = new Vector3(0.2f, 0.2f, 0.2f)
                }
});
            rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.0044F, 0.0647F, -0.1676F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.0249F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.1276F, 2.9217F, 0.4448F),
                localAngles = new Vector3(275.543F, 332.9316F, 24.6977F),
                localScale = new Vector3(0.3943F, 0.0988F, 0.3943F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(-0.0688F, 1.7196F, -0.1985F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.1539F, 0.1539F, 0.1539F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(0F, 0.48F, 0.867F),
                localAngles = new Vector3(26.9185F, 0F, 0F),
                localScale = new Vector3(0.2443F, 0.2443F, 0.2443F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleJar",
                localPos = new Vector3(2.0046F, -2.7849F, 1.7857F),
                localAngles = new Vector3(352.9601F, 323.766F, 252.3455F),
                localScale = new Vector3(0.4172F, 0.4172F, 0.4172F)
            });
            return rules;
        }
        public override void Install()
        {
            base.Install();
            GenericNotification.SetEquipment += GenericNotification_SetEquipment;
            GetStatCoefficients += StatHooks_GetStatCoefficients;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            if (SpiceEquipment_Disconnect)
                On.RoR2.GenericPickupController.AttemptGrant += GenericPickupController_AttemptGrant;
            else
                On.RoR2.PickupDropletController.CreatePickupDroplet += ReplacePickupDropletSynced;
        }

        public override void InstallLanguage()
        {
            base.InstallLanguage();
            LanguageAPI.Add("ITEM_SPICETALLY_NAME", "Spice (Consumed)");
            LanguageAPI.Add("ITEM_SPICETALLY_DESC", "Grants various stat changes. Most notably, increased damage and reduced accuracy and health.");
        }
        
        public override void Uninstall()
        {
            base.Install();
            GenericNotification.SetEquipment -= GenericNotification_SetEquipment;
            GetStatCoefficients -= StatHooks_GetStatCoefficients;
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
            if (SpiceEquipment_Disconnect)
                On.RoR2.GenericPickupController.AttemptGrant -= GenericPickupController_AttemptGrant;
            else
                On.RoR2.PickupDropletController.CreatePickupDroplet -= ReplacePickupDropletSynced;
        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                CharacterBody body = attacker.GetComponent<CharacterBody>();
                if (body)
                {
                    var inventory = body.inventory;
                    if (inventory)
                    {
                        var SpiceTallyCount = inventory.GetItemCount(SpiceTally);
                        float SpiceMult;
                        switch (SpiceTallyCount)
                        {
                            case 0: //
                            case 1:
                            case 2:
                            case 3:
                                SpiceMult = 0f;
                                break;
                            case 4:
                                SpiceMult = SpiceBonusesConstant[SpiceTallyCount, 4];
                                break;
                            default: //also 5
                                SpiceMult = SpiceBonusesConstant[4, 4] + SpiceBonusesAdditive[4] * (SpiceTallyCount - 4);
                                break;
                        }
                        damageInfo.damage *= 1 + SpiceMult;
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void StatHooks_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var SpiceTallyCount = sender.inventory.GetItemCount(SpiceTally);
                switch (SpiceTallyCount)
                {
                    case 0:
                        break;
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        //health, attack speed, shot accuracy, enemy bullet speed, damage
                        //args.baseHealthAdd += HeartValue * SpiceBonuses[SpiceTallyCount, 0];
                        args.healthMultAdd += 1 + SpiceBonusesConstant[SpiceTallyCount, 0];
                        args.attackSpeedMultAdd += SpiceBonusesConstant[SpiceTallyCount, 1];
                        //accuracy 
                        //enemy bullet speed
                        //damage
                        break;
                    default:
                        SpiceTallyCount -= 4;
                        //var baseHealthAdd = HeartValue * SpiceBonusesAdditive[0] * (SpiceTallyCount - 4);
                        //args.baseHealthAdd += baseHealthAdd;
                        int debuffCount = SpiceTallyCount * (int)Math.Abs(SpiceBonusesAdditive[0]*100);
                        GiveCursedDebuffMinimum(sender, debuffCount);
                        //health, attack speed, shot accuracy, enemy bullet speed, damage
                        args.attackSpeedMultAdd += SpiceBonusesConstant[5, 1];
                        //accuracy
                        //enemy
                        //damage
                        break;
                }
            }
        }

        public void GiveCursedDebuffMinimum(CharacterBody characterBody, int curseAmount)
        {
            if (characterBody && characterBody.healthComponent)
            {
                var curseDebuffAmount = characterBody.GetBuffCount(RoR2Content.Buffs.PermanentCurse);
                if (curseDebuffAmount < curseAmount)
                {
                    HelperUtil.AddBuffStacks(characterBody, RoR2Content.Buffs.PermanentCurse, curseAmount - curseDebuffAmount);
                }
            }
        }

        private void GenericNotification_SetEquipment(GenericNotification.orig_SetEquipment orig, RoR2.UI.GenericNotification self, EquipmentDef equipmentDef)
        {
            orig(self, equipmentDef);
            if (equipmentDef.equipmentIndex == catalogIndex)
            {
                var LocalUserList = LocalUserManager.readOnlyLocalUsersList;
                var localUser = LocalUserList[0];
                var inventoryCount = localUser.cachedBody.inventory.GetItemCount(SpiceTally);
                var index = Mathf.Min(inventoryCount, SpiceDescArray.Length - 1);
                self.descriptionText.token = SpiceDescArray[index];
            }
        }


        [Server]
        private void GenericPickupController_AttemptGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::AttemptGrant(RoR2.CharacterBody)' called on client");
                return;
            }
            if (!self.gameObject.GetComponent<SpiceRolled>())
            {
                self.gameObject.AddComponent<SpiceRolled>();
                TeamComponent component = body.GetComponent<TeamComponent>();
                if (component && component.teamIndex == TeamIndex.Player)
                {
                    Inventory inventory = body.inventory;
                    if (inventory)
                    {
                        if (pickupDef.itemIndex != ItemIndex.None || pickupDef.equipmentIndex != EquipmentIndex.None)
                        {
                            var spiceCount = inventory.GetItemCount(SpiceTally);
                            var spiceCountAdjusted = spiceCount < SpiceEquipment_MaxPerPlayer ? spiceCount : 0;
                            if (Util.CheckRoll(spiceCountAdjusted))
                            {
                                self.pickupIndex = pickupIndex;
                            }
                        }
                    }
                }
            }
            orig(self, body);
        }

        private void ReplacePickupDropletSynced(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            var spiceCount = IntOfMostSpice(SpiceEquipment_MaxPerPlayer);
            // IF all players are over the cap, then it will roll for 0
            if (Util.CheckRoll(spiceCount))
            {
                if (pickupIndex != PickupCatalog.FindPickupIndex(RoR2Content.Items.ArtifactKey.itemIndex)) //safety to prevent softlocks
                    pickupIndex = PickupCatalog.FindPickupIndex(catalogIndex);
            }
            orig(pickupIndex, position, velocity);
        }

        private int IntOfMostSpice(int cap = 40)
        {
            var instances = PlayerCharacterMasterController.instances;
            var largestStack = 0;
            foreach (PlayerCharacterMasterController playerCharacterMaster in instances)
            {
                var master = playerCharacterMaster.master;
                if (master && master.inventory)
                {
                    var itemCount = master.inventory.GetItemCount(SpiceTally);
                    if (itemCount > largestStack && itemCount < cap)
                    {
                        largestStack = itemCount;
                    }
                }
            }
            return largestStack;
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;
            Inventory inventory = body.inventory;
            if (!inventory) return false;

            inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico
            if (inventory.GetItemCount(SpiceTally) == 0) inventory.GiveItem(CurseDef);
            else inventory.GiveItem(CurseDef, 2);
            inventory.GiveItem(SpiceTally);

            return false;
        }

        // used to prevent hotswapping spice on client mode for equipment
        public class SpiceRolled : MonoBehaviour
        {

        }
    }
}
