using RiskOfBulletstorm.Utils;
using R2API;
using RoR2;
using UnityEngine.Networking;
using TILER2;
using UnityEngine;
namespace RiskOfBulletstorm.Items
{
    public class FriendshipCookie : Equipment_V2<FriendshipCookie>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the duration of immunity after respawning someone?", AutoConfigFlags.None)]
        public float FriendshipCookie_BaseImmunityTime { get; private set; } = 3f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the ItemIndex of the item to be given in singleplayer?", AutoConfigFlags.None)]
        public ItemIndex FriendshipCookie_ItemIndex { get; private set; } = ItemIndex.Infusion;

        public override string displayName => "Friendship Cookie";
        public override float cooldown { get; protected set; } = 0f;
        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>It's Delicious!</b>\nRevives all players.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Upon use, <style=cIsHealing>revives all players</style>." +
            $"\nSINGLEPLAYER: <style=cIsHealing>Gives ";
            var itemDef = ItemCatalog.GetItemDef(FriendshipCookie_ItemIndex);
            if (itemDef == null)
                desc += $"nothing";
            else
                desc += $"{itemDef.name}";
            desc += " instead.</style>" +
            $"\n<style=cDeath>Consumed on use.</style>";

            return desc;
        }

        protected override string GetLoreString(string langID = null) => "Baked fresh every morning by Mom! It's to die for! Or, just maybe, to live for.";

        public static GameObject ItemBodyModelPrefab;

        public FriendshipCookie()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/FriendshipCookie.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/FriendshipCookie.png";
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();
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
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;
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
                        var respawnLength = FriendshipCookie_BaseImmunityTime;
                        //if (EmbryoProc)
                            //respawnLength = FriendshipCookie_EmbryoImmunityTime;

                        RespawnExtraLife(master, false, true, respawnLength, body.master);
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
            else if (playerAmt == 1) inventory.GiveItem(ItemIndex.Infusion, EmbryoProc ? 2 : 1);

            if (revivedPlayers > 0 || playerAmt == 1) //anyone revived or its singleplayer
            {
                inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico
                return true;
            }
            return false;
        }

        public void RespawnExtraLife(CharacterMaster master, bool GiveSpentDio = true, bool TryToGroundFootPosition = true, float BuffTimer = 3f, CharacterMaster summoner = null)
        {
            var RespawnPosition = master.deathFootPosition;
            if (GiveSpentDio) master.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, 1);

            if (summoner)
                RespawnPosition = summoner.GetBody().footPosition;

            master.PlayExtraLifeSFX();
            master.Respawn(RespawnPosition, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), TryToGroundFootPosition);
            master.GetBody().AddTimedBuff(BuffIndex.Immune, BuffTimer);
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
                        origin = master.deathFootPosition,
                        rotation = master.bodyInstanceObject.transform.rotation
                    }, true);
                }
            }
            master.ResetLifeStopwatch();
        }

    }
}
