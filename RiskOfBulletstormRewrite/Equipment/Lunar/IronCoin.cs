using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Utils;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class IronCoin : EquipmentBase<IronCoin>
    {
        public static ConfigEntry<float> cfgDamageToPlayers;
        public static ConfigEntry<IronCoinEnum> cfgEnemyPlayerDamageType;
        public static ConfigEntry<float> cfgCooldown;
        public override float Cooldown => cfgCooldown.Value;

        public override string EquipmentName => "Iron Coin";

        public override string EquipmentLangTokenName => "IRONCOIN";

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();
        public override bool CanBeRandomlyTriggered => false;

        public override bool IsLunar => true;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        public enum IronCoinEnum
        {
            Nonfatal,
            Fatal,
            InstantKill
        }

        protected override void CreateConfig(ConfigFile config)
        {
            cfgCooldown = config.Bind(ConfigCategory, CooldownName, 60f, CooldownDescription);
            cfgDamageToPlayers = config.Bind(ConfigCategory, "Damage Dealt To Enemy Players", 0.05f, "If flipped by an enemy, deals this amount of damage to enemy players." +
                "\nDamage dealt is equal to config value multiplied by enemy player's max health." +
                "\nEx: 0.05 -> 5% of the enemy player's max health." +
                "\nThis value is affected by the friendly fire damage modification.");
            cfgEnemyPlayerDamageType = config.Bind(ConfigCategory, "Damage Type Dealt To Players",
                IronCoinEnum.Nonfatal,
                "Nonfatal - Deals damage based off the other config value, but can not kill the player" +
                "\n Fatal - Damage dealt can kill the player" +
                "\n  InstantKill - Damage dealt instantly kills the player, ignores damage dealt.");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = EquipmentModel;
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
                    childName = "Chest",
localPos = new Vector3(-0.00141F, 0.24461F, 0.21482F),
localAngles = new Vector3(355.4193F, 91.99109F, 31.98411F),
localScale = new Vector3(0.01F, 0.01F, 0.01F)
                }
            });
            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(-0.00204F, 0.17007F, 0.17426F),
localAngles = new Vector3(18.47152F, 73.89497F, 9.3659F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)
                }
            });
            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.06284F, 3.6339F, 0.20165F),
localAngles = new Vector3(58.87366F, 354.6468F, 311.2233F),
localScale = new Vector3(0.1067F, 0.1067F, 0.1067F)
                }
            });
            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "MuzzleRight",
localPos = new Vector3(0.1408F, -0.18521F, -0.24458F),
localAngles = new Vector3(0.88914F, 359.6638F, 138.5349F),
localScale = new Vector3(0.02038F, 0.02038F, 0.02038F)
                }
            });

            rules.Add("mdlEngiTurret", new ItemDisplayRule[] //NOPE
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(0.27077F, 0.71612F, 0.79915F),
localAngles = new Vector3(0.68513F, 0.72842F, 316.6972F),
localScale = new Vector3(0.06191F, 0.06191F, 0.06191F)
                }
            });
            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
            childName = "Chest",
localPos = new Vector3(-0.12552F, 0.3082F, 0.00194F),
localAngles = new Vector3(355.0641F, 3.66094F, 336.4612F),
localScale = new Vector3(0.01799F, 0.01799F, 0.01799F)
                }
            });
            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HandR",
localPos = new Vector3(-0.04873F, 0.22772F, -0.023F),
localAngles = new Vector3(346.7007F, 341.2188F, 139.5535F),
localScale = new Vector3(0.00779F, 0.00779F, 0.00779F)
                }
            });
            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "HeadBase",
localPos = new Vector3(0.02712F, -0.54159F, -0.39199F),
localAngles = new Vector3(344.4032F, 180F, 180F),
localScale = new Vector3(0.1341F, 0.1341F, 0.1341F)
                }
            });
            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Head",
localPos = new Vector3(-0.00081F, 0.20497F, 0.12839F),
localAngles = new Vector3(359.8913F, 79.40346F, 359.5287F),
localScale = new Vector3(0.01266F, 0.01266F, 0.01258F)
                }
            });
            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
            childName = "Head",
localPos = new Vector3(-0.02219F, 3.46377F, 1.3757F),
localAngles = new Vector3(2.92278F, 82.81886F, 63.1954F),
localScale = new Vector3(0.18789F, 0.18789F, 0.18789F)
                }
            });
            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "Chest",
localPos = new Vector3(0.11037F, 0.18249F, 0.18749F),
localAngles = new Vector3(26.74645F, 105.2424F, 31.18615F),
localScale = new Vector3(0.01335F, 0.01335F, 0.01335F)
                }
            });
            rules.Add("mdlBrother", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
childName = "chest",
localPos = new Vector3(0F, -0.00001F, 0.15252F),
localAngles = new Vector3(0F, 86.30653F, 42.68443F),
localScale = new Vector3(0.05256F, 0.05256F, 0.05256F)
                }
            });
            rules.Add("mdlBandit", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
localPos = new Vector3(-0.00132F, 0.3328F, 0.08185F),
localAngles = new Vector3(350.4996F, 93.33881F, 12.07599F),
localScale = new Vector3(0.01755F, 0.01755F, 0.01755F)
                }
            });
            rules.Add("mdlBandit2", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
            childName = "Head",
localPos = new Vector3(-0.02215F, 0.11882F, 0.22856F),
localAngles = new Vector3(359.9695F, 359.802F, 319.1974F),
localScale = new Vector3(0.0191F, 0.0191F, 0.0191F)
                }
            });
            rules.Add("mdlVoidSurvivor", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.06572F, 0.09146F, 0.13573F),
                localAngles = new Vector3(0F, 96.41827F, 0F),
                localScale = new Vector3(0.01731F, 0.01731F, 0.01731F)
            });
            rules.Add("mdlRailGunner", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0F, 0.1831F, 0.07933F),
                localAngles = new Vector3(0F, 96.48205F, 0F),
                localScale = new Vector3(0.01628F, 0.01628F, 0.01502F)
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
localPos = new Vector3(0F, 4.8685F, 0.0438F),
localAngles = new Vector3(288.4044F, 180F, 180F),
localScale = new Vector3(1F, 1F, 1F)
                }
            }); rules.Add("mdlBeetle", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(0.0013F, 0.1559F, -0.2403F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F)
            });
            rules.Add("mdlLemurian", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Head",
                localPos = new Vector3(-0.1594F, 3.6456F, 0.0645F),
                localAngles = new Vector3(279.4401F, 195.4454F, 161.8801F),
                localScale = new Vector3(0.4099F, 0.4099F, 0.4099F)
            });
            rules.Add("mdlLunarGolem", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "MuzzleLB",
                localPos = new Vector3(-1.6752F, -0.2F, -0.468F),
                localAngles = new Vector3(2.6768F, 179.4175F, 179.4478F),
                localScale = new Vector3(0.1793F, 0.1793F, 0.1793F)
            });
            rules.Add("mdlNullifier", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Muzzle",
                localPos = new Vector3(0.0002F, -0.189F, 1.9457F),
                localAngles = new Vector3(24.2706F, 0.0024F, 0.024F),
                localScale = new Vector3(0.2908F, 0.2908F, 0.2908F)
            });
            rules.Add("mdlGravekeeper", new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = ItemBodyModelPrefab,
                childName = "Mask",
                localPos = new Vector3(0F, 0.0344F, -1.6055F),
                localAngles = new Vector3(88.6293F, 0F, 0F),
                localScale = new Vector3(0.425F, 0.425F, 0.425F)
            });
            return rules;
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            int enemiesKilled = 0;
            var attackerGameObject = slot.characterBody.gameObject;
            foreach (var charMaster in CharacterMaster.readOnlyInstancesList)
            {
                var charBody = charMaster.GetBody();
                if (charMaster.playerCharacterMasterController
                    && charBody
                    && FriendlyFireManager.ShouldDirectHitProceed(charMaster.GetBody().healthComponent, slot.characterBody.teamComponent.teamIndex))
                {
                    switch (cfgEnemyPlayerDamageType.Value)
                    {
                        case IronCoinEnum.Nonfatal:
                        case IronCoinEnum.Fatal:
                            DamageInfo ironCoinDamage = new DamageInfo()
                            {
                                attacker = slot.gameObject,
                                crit = false,
                                damage = charBody.healthComponent.fullCombinedHealth * cfgDamageToPlayers.Value,
                                damageType = cfgEnemyPlayerDamageType.Value == IronCoinEnum.Nonfatal ? DamageType.NonLethal : DamageType.Generic,
                                position = charBody.corePosition,
                                procCoefficient = 0,
                            };
                            charBody.healthComponent.TakeDamage(ironCoinDamage);
                            enemiesKilled++;
                            break;

                        default:
                            charBody.healthComponent.Suicide(slot.gameObject);
                            enemiesKilled++;
                            break;
                    }
                    continue;
                }

                var body = charMaster.GetBody();
                if (charMaster.inventory && body && body.healthComponent && !body.isBoss && body.healthComponent.alive && charMaster.teamIndex != slot.teamComponent.teamIndex)
                {
                    enemiesKilled++;

                    if (body.TryGetComponent<DeathRewards>(out DeathRewards deathRewards))
                    {
                        deathRewards.goldReward = 0;
                        deathRewards.expReward = 0;
                        deathRewards.bossDropTable = null;
                    }
                    if (body.isElite && body.TryGetComponent(out EquipmentSlot equipmentSlot))
                    {
                        charMaster.inventory.SetEquipmentIndex(EquipmentIndex.None);
                    }
                    var itemCount = charMaster.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger);
                    if (itemCount > 0)
                    {
                        charMaster.inventory.RemoveItem(RoR2Content.Items.InvadingDoppelganger, itemCount);
                    }
                    //var bossGroups = InstanceTracker.GetInstancesList<BossGroup>();
                    /*foreach (var bossGroup in bossGroups)
                    {
                        bossGroup.bossMemories[0].
                    }*/
                    charMaster.TrueKill(attackerGameObject, attackerGameObject, DamageType.Generic);
                }
            }
            if (enemiesKilled > 0)
            {
                CharacterMasterNotificationQueue.PushEquipmentTransformNotification(slot.characterBody.master, slot.characterBody.inventory.currentEquipmentIndex, IronCoinConsumed.Instance.EquipmentDef.equipmentIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                slot.inventory.SetEquipmentIndex(IronCoinConsumed.Instance.EquipmentDef.equipmentIndex);
                return true;
            }
            return false;
        }
    }
}