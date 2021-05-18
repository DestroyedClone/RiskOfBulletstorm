using RiskOfBulletstorm.Utils;
using R2API;
using RoR2;
using UnityEngine.Networking;
using TILER2;
using UnityEngine;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class FriendshipCookie : Equipment<FriendshipCookie>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How long are your allies immune for after respawning?", AutoConfigFlags.None)]
        public float BaseImmunityTime { get; private set; } = 3f;

        public override string displayName => "Friendship Cookie";
        public override float cooldown { get; protected set; } = 0f;
        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>It's Delicious!</b>\nRevives all players.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Upon use, <style=cIsHealing>revives all players</style>." +
            $"\nSINGLEPLAYER: <style=cIsHealing>Gives ";
            var itemDef = RoR2Content.Items.Infusion;
            desc += itemDef == null ? $"nothing" : $"{itemDef.name}";
            desc += " instead.</style>" +
            $" <style=cIsUtility>Consumes</style> on use.";

            return desc;
        }

        protected override string GetLoreString(string langID = null) => "Baked fresh every morning by Mom! It's to die for! Or, just maybe, to live for.";

        public static GameObject ItemBodyModelPrefab;
        public static ItemDef singleplayerItemDef = RoR2Content.Items.Infusion;

        public FriendshipCookie()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/FriendshipCookie.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/FriendshipCookie.png");
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            if (ItemBodyModelPrefab == null)
            {
                ItemBodyModelPrefab = modelResource;
                displayRules = GenerateItemDisplayRules();
            }
            //singleplayerItemDef = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(FriendshipCookie_ItemName));
            //if (!singleplayerItemDef)
                //singleplayerItemDef = RoR2Content.Items.Infusion;
            base.SetupAttributes();
        }

        public static ItemDisplayRuleDict GenerateItemDisplayRules()
        {
            ItemBodyModelPrefab.AddComponent<ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<ItemDisplay>().rendererInfos = ItemHelpers.ItemDisplaySetup(ItemBodyModelPrefab);

            Vector3 generalScale = new Vector3(0.1f, 0.1f, 0.1f);
            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.12f, 0.3f, 0.19f),
                    localAngles = new Vector3(-40, 0, 0),
                    localScale = new Vector3(0.03f, 0.03f, 0.03f)
        }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.1f, 0.15f, 0.16f),
                    localAngles = new Vector3(-35, -15, 0),
                    localScale = new Vector3(0.02f, 0.02f, 0.02f)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-2f, 1.5f, 3.3f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = generalScale * 4
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.25f, 0.2f, 0.19f),
                    localAngles = new Vector3(10f, -30f, 0f),
                    localScale = new Vector3(0.03f, 0.03f, 0.03f)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.12f, 0.2f, 0.1f),
                    localAngles = new Vector3(0f, 0f, 0f),
                    localScale = new Vector3(0.02f, 0.02f, 0.02f)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.1491F, 0.1988F, 0.1562F),
localAngles = new Vector3(342.2324F, 345.9447F, 2.4492F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Base",
                    localPos = new Vector3(-0.25f, 1f, 0.1f),
                    localAngles = new Vector3(-90f, 0f, 0f),
                    localScale = new Vector3(0.07f, 0.07f, 0.07f)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.1f, 0.2f, 0.18f),
                    localAngles = new Vector3(-30f, 0f, 0f),
                    localScale = new Vector3(0.02f, 0.02f, 0.02f)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.9895F, 0.4916F, -2.2411F),
localAngles = new Vector3(352.0745F, 169.898F, 1.4959F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(-0.28f, 0.29f, 0.19f),
                    localAngles = new Vector3(-10f, 0f, 0f),
                    localScale = new Vector3(0.02f, 0.02f, 0.02f)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "chest",
localPos = new Vector3(-0.1143F, 0.2396F, 0.1565F),
localAngles = new Vector3(344.6736F, 355.3099F, 1.2422F),
localScale = new Vector3(0.0297F, 0.0297F, 0.0297F)
                }
});
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.099F, 0.3478F, 0.1073F),
localAngles = new Vector3(322.5172F, 340F, 0F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)
                }
            });
            rules.Add("mdlClayBruiser", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.1078F, -0.4449F, 0.5694F),
localAngles = new Vector3(4.1483F, 35.4073F, 319.8642F),
localScale = new Vector3(0.04F, 0.04F, 0.04F)
                }
            });
            rules.Add("mdlHAND", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-1.5F, 1.9941F, 2.2246F),
localAngles = new Vector3(0F, 325F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });
            rules.Add("mdlScav", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(8.1f, -4.4f, 5f),
                    localAngles = new Vector3(0f, 90f, 0f),
                    localScale = generalScale * 5f
                }
            });
            rules.Add("mdlEquipmentDrone", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "GunBarrelBase",
                    localPos = new Vector3(0f, 0f, 1.12f),
                    localAngles = new Vector3(270f, 0f, 0f),
                    localScale = generalScale
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.0354F, 0.7154F, 0.2584F),
                localAngles = new Vector3(310.7005F, 14.4338F, 357.9766F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Chest",
                localPos = new Vector3(0.4957F, 1.6177F, -1.5472F),
                localAngles = new Vector3(322.5182F, 163.594F, 10.0309F),
                localScale = new Vector3(0.2601F, 0.2601F, 0.2601F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(0F, -0.2602F, 0.4547F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Center",
                localPos = new Vector3(-0.3983F, -0.0585F, 1.4553F),
                localAngles = new Vector3(3.4644F, 356.2471F, 359.7729F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Mask",
                localPos = new Vector3(1.0482F, 0.0052F, -1.1523F),
                localAngles = new Vector3(292.3259F, 90.0923F, 88.7283F),
                localScale = new Vector3(0.231F, 0.231F, 0.231F)
            });
            return rules;
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body || !body.healthComponent) return false;
            Inventory inventory = body.inventory;
            if (!inventory) return false;
            int revivedPlayers = 0;

            var playerList = PlayerCharacterMasterController.instances;
            //var playerList = CharacterMaster.readOnlyInstancesList;
            int playerAmt = playerList.Count;

            //bool EmbryoProc = HelperPlugin.ClassicItemsCompat.CheckEmbryoProc(instance, body) && HelperPlugin.ClassicItemsCompat.enabled;
            bool EmbryoProc = false;

            if (playerAmt > 1)
            {
                foreach (PlayerCharacterMasterController player in playerList)
                {
                    var master = player.master;
                    if (master.IsDeadAndOutOfLivesServer())
                    {
                        var respawnLength = BaseImmunityTime;
                        //if (EmbryoProc)
                            //respawnLength = FriendshipCookie_EmbryoImmunityTime;

                        RespawnExtraLife(master, respawnLength, body.master);
                        //Stage.instance.RespawnCharacter(master);
                        if (EmbryoProc)
                        {
                            var healthComponent = master.GetBody()?.GetComponent<HealthComponent>();

                            if (healthComponent)
                            {
                                var fullBarrier = healthComponent.fullBarrier;
                                if (NetworkServer.active)
                                {
                                    healthComponent.AddBarrier(fullBarrier);
                                } else
                                {
                                    healthComponent.AddBarrierAuthority(fullBarrier);
                                }
                            }
                        }
                        revivedPlayers++;
                    }
                }
            }
            else if (playerAmt == 1) inventory.GiveItem(RoR2Content.Items.Infusion, EmbryoProc ? 2 : 1);

            if (revivedPlayers > 0 || playerAmt == 1) //anyone revived or its singleplayer
            {
                inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico
                return true;
            }
            return false;
        }

        public void RespawnExtraLife(CharacterMaster master, float BuffTimer = 3f, CharacterMaster summoner = null)
        {
            var RespawnPosition = master.deathFootPosition;
            if (summoner)
                RespawnPosition = summoner.GetBody().footPosition;
            else if (master.killedByUnsafeArea)
            {
                RespawnPosition = (TeleportHelper.FindSafeTeleportDestination(master.deathFootPosition, master.bodyPrefab.GetComponent<CharacterBody>(), RoR2Application.rng) ?? master.deathFootPosition);
            }

            master.Respawn(RespawnPosition, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f));
            master.GetBody().AddTimedBuff(RoR2Content.Buffs.Immune, BuffTimer);
            GameObject gameObject = Resources.Load<GameObject>("Prefabs/Effects/HippoRezEffect");
            if (master.bodyInstanceObject)
            {
                foreach (EntityStateMachine entityStateMachine in master.bodyInstanceObject.GetComponents<EntityStateMachine>())
                {
                    entityStateMachine.initialStateType = entityStateMachine.mainStateType;
                }
                if (gameObject)
                {
                    EffectManager.SpawnEffect(gameObject, new EffectData
                    {
                        origin = RespawnPosition,
                        rotation = master.bodyInstanceObject.transform.rotation
                    }, true);
                }
            }
        }

    }
}
