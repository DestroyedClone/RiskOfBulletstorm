using RiskOfBulletstorm.Utils;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static RiskOfBulletstorm.Utils.HelperUtil;
using GenericNotification = On.RoR2.UI.GenericNotification;
using System;
using UnityEngine.Networking;

namespace RiskOfBulletstorm.Items
{
    public class Spice : Equipment_V2<Spice>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Limit of Spice per player", AutoConfigFlags.PreventNetMismatch)]
        public int SpiceEquipment_MaxPerPlayer { get; private set; } = 40;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Spice only affects the pickups of the person running Spice. ", AutoConfigFlags.PreventNetMismatch)]
        public bool SpiceEquipment_Disconnect { get; private set; } = true;
        public override string displayName => "Spice";
        public override float cooldown { get; protected set; } = 1f;
        public override bool isEnigmaCompatible => false;
        public override bool isLunar => true;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<style=cIsUtility>Increases combat prowess</style>, <style=cDeath>with absolutely no downside!</style>";

        protected override string GetDescString(string langid = null) => $"Progressively gives stat bonuses and downsides. Consumed on use.";

        protected override string GetLoreString(string langID = null) => "A potent gun-enhancing drug from the far reaches of the galaxy. It is known to be extremely addictive, and extremely expensive.";

        public static ItemIndex SpiceTally { get; private set; }
        public static ItemIndex CurseIndex;
        public static ItemIndex GimmeSpiceIndex;

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
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Spice.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Spice.png";
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = Resources.Load<GameObject>(modelResourcePath);
                displayRules = GenerateItemDisplayRules();
            }

            base.SetupAttributes();
            var spiceTallyDef = new CustomItem(new ItemDef
            {
                hidden = true,
                name = "ROBInternalSpiceTally",
                tier = ItemTier.NoTier,
                canRemove = false
            }, new ItemDisplayRuleDict(null));
            SpiceTally = ItemAPI.Add(spiceTallyDef);

            var gimmeSpiceDef = new CustomItem(new ItemDef
            {
                hidden = true,
                name = "ROBInternalGiveSpice",
                tier = ItemTier.NoTier,
                canRemove = false
            }, new ItemDisplayRuleDict(null));
            GimmeSpiceIndex = ItemAPI.Add(spiceTallyDef);
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
                    childName = "ThighR",
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
                    childName = "ThighR",
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
                    childName = "ThighR",
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
                    childName = "ThighR",
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
                    childName = "ThighR",
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
                    childName = "ThighBackR",
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
                    childName = "ThighR",
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
                    childName = "ThighR",
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
                    childName = "ThighR",
                    localPos = new Vector3(0f, 0.04f, 0.18f),
                    localAngles = new Vector3(0f, 0f, 0f),
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
                    childName = "ThighR",
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
                    childName = "Root",
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
                    childName = "ThighR",
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
                    childName = "ThighR",
                    localPos = new Vector3(0f, 5f, 0f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = generalScale * 20f
                }
            });
            return rules;
        }
        public override void SetupLate()
        {
            base.SetupLate();
            CurseIndex = CurseController.curseTally;
        }
        public override void Install()
        {
            base.Install();
            GenericNotification.SetEquipment += GenericNotification_SetEquipment;
            StatHooks.GetStatCoefficients += StatHooks_GetStatCoefficients;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            if (SpiceEquipment_Disconnect)
                On.RoR2.GenericPickupController.AttemptGrant += GenericPickupController_AttemptGrant;
            else
                On.RoR2.PickupDropletController.CreatePickupDroplet += ReplacePickupDropletSynced;
        }

        [Server]
        private void GenericPickupController_AttemptGrant(On.RoR2.GenericPickupController.orig_AttemptGrant orig, GenericPickupController self, CharacterBody body)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.GenericPickupController::AttemptGrant(RoR2.CharacterBody)' called on client");
                return;
            }
            TeamComponent component = body.GetComponent<TeamComponent>();
            if (component && component.teamIndex == TeamIndex.Player)
            {
                Inventory inventory = body.inventory;
                if (inventory)
                {
                    var spiceCount = inventory.GetItemCount(SpiceTally);
                    if (Util.CheckRoll(spiceCount) & (pickupDef.itemIndex != ItemIndex.None || pickupDef.equipmentIndex != EquipmentIndex.None))
                    {
                        self.pickupIndex = pickupIndex;
                    }
                }
            }
            orig(self, body);
        }
        

        public override void Uninstall()
        {
            base.Install();
            GenericNotification.SetEquipment -= GenericNotification_SetEquipment;
            StatHooks.GetStatCoefficients -= StatHooks_GetStatCoefficients;
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

        private void StatHooks_GetStatCoefficients(CharacterBody sender, StatHooks.StatHookEventArgs args)
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
                var curseDebuffAmount = characterBody.GetBuffCount(BuffIndex.PermanentCurse);
                if (curseDebuffAmount < curseAmount)
                {
                    HelperUtil.AddBuffStacks(characterBody, BuffIndex.PermanentCurse, curseAmount - curseDebuffAmount);
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

        private void ReplacePickupDropletSynced(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            var spiceCount = IntOfMostSpice(SpiceEquipment_MaxPerPlayer);
            // IF all players are over the cap, then it will roll for 0
            if (Util.CheckRoll(spiceCount))
            {
                if (pickupIndex != PickupCatalog.FindPickupIndex(ItemIndex.ArtifactKey)) //safety to prevent softlocks
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
            if (inventory.GetItemCount(SpiceTally) == 0) inventory.GiveItem(CurseIndex);
            else inventory.GiveItem(CurseIndex, 2);
            inventory.GiveItem(SpiceTally);

            return false;
        }
    }
}
