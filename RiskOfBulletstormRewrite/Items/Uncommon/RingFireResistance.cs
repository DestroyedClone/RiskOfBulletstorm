using BepInEx.Configuration;
using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Items
{
    public class RingFireResistance : ItemBase<RingFireResistance>
    {
        public static ConfigEntry<float> cfgBaseResist;
        //public static ConfigEntry<float> cfgStackResist;

        public override string ItemName => "Ring of Fire Resistance";

        public override string ItemLangTokenName => "RINGFIRERESISTANCE";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemTag[] ItemTags => new ItemTag[]
        {
            ItemTag.Healing
        };

        public override string[] ItemFullDescriptionParams => new string[]
        {
            GetChance(cfgBaseResist)
        };

        public static GameObject fireTrailSegmentPrefab = null;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
            ModifyPrefabs();
        }

        public void ModifyPrefabs()
        {
            var fireTrailGameObject = Resources.Load<GameObject>("Prefabs/FireTrail");
            if (fireTrailGameObject)
            {
                fireTrailSegmentPrefab = fireTrailGameObject.GetComponent<DamageTrail>().segmentPrefab;
            }
        }

        private float GetMultiplier(int itemCount)
        {
            return 1 - (Utils.ItemHelpers.GetHyperbolicValue(cfgBaseResist.Value, itemCount));
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgBaseResist = config.Bind(ConfigCategory, "Fire Resistance", 0.1f, "Uses Hyperbolic stacking, see the ror2 wiki.");
            //cfgStackResist = config.Bind(ConfigCategory, "Hyperbolic Stack Fire Resistance", 0.25f, "");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            On.RoR2.DotController.InflictDot_GameObject_GameObject_DotIndex_float_float += ReduceFireDot;
            On.RoR2.DamageTrail.DoDamage += DamageTrail_DoDamage;
        }

        private void DamageTrail_DoDamage(On.RoR2.DamageTrail.orig_DoDamage orig, DamageTrail self)
        {
            if (self.segmentPrefab == fireTrailSegmentPrefab)
            {
                if (!NetworkServer.active || self.pointsList.Count == 0)
                {
                    return;
                }
                Vector3 b = self.pointsList[self.pointsList.Count - 1].position;
                HashSet<GameObject> hashSet = new HashSet<GameObject>();
                TeamIndex attackerTeamIndex = TeamIndex.Neutral;
                float damage = self.damagePerSecond * self.damageUpdateInterval;
                if (self.owner)
                {
                    hashSet.Add(self.owner);
                    attackerTeamIndex = TeamComponent.GetObjectTeam(self.owner);
                }
                DamageInfo damageInfo = new DamageInfo
                {
                    attacker = self.owner,
                    inflictor = self.gameObject,
                    crit = false,
                    damage = damage,
                    damageColorIndex = DamageColorIndex.Item,
                    damageType = DamageType.Generic,
                    force = Vector3.zero,
                    procCoefficient = 0f
                };
                for (int i = self.pointsList.Count - 2; i >= 0; i--)
                {
                    Vector3 position = self.pointsList[i].position;
                    Vector3 forward = position - b;
                    Vector3 halfExtents = new Vector3(self.radius, self.height, forward.magnitude);
                    Vector3 center = Vector3.Lerp(position, b, 0.5f);
                    Quaternion orientation = Util.QuaternionSafeLookRotation(forward);
                    Collider[] array = Physics.OverlapBox(center, halfExtents, orientation, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
                    for (int j = 0; j < array.Length; j++)
                    {
                        HurtBox component = array[j].GetComponent<HurtBox>();
                        if (component)
                        {
                            HealthComponent healthComponent = component.healthComponent;
                            if (healthComponent)
                            {
                                GameObject gameObject = healthComponent.gameObject;
                                if (!hashSet.Contains(gameObject))
                                {
                                    hashSet.Add(gameObject);
                                    if (FriendlyFireManager.ShouldSplashHitProceed(healthComponent, attackerTeamIndex))
                                    {
                                        damageInfo.position = array[j].transform.position;
                                        if (healthComponent.body)
                                        {
                                            var itemCount = GetCount(healthComponent.body);
                                            if (itemCount > 0)
                                            {
                                                damageInfo.damage *= GetMultiplier(itemCount);
                                            }
                                            healthComponent.TakeDamage(damageInfo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    b = position;
                }
            }
            else
                orig(self);
        }

        private void ReduceFireDot(On.RoR2.DotController.orig_InflictDot_GameObject_GameObject_DotIndex_float_float orig, GameObject victimObject, GameObject attackerObject, DotController.DotIndex dotIndex, float duration, float damageMultiplier)
        {
            if (dotIndex == DotController.DotIndex.PercentBurn || dotIndex == DotController.DotIndex.Burn)
            {
                if (victimObject && victimObject.GetComponent<CharacterBody>())
                {
                    var characterBody = victimObject.GetComponent<CharacterBody>();
                    var itemCount = GetCount(characterBody);
                    if (itemCount > 0)
                    {
                        damageMultiplier *= GetMultiplier(itemCount);
                        //_logger.LogMessage($"DoT: dmgMult({damageMultiplier})   reduction({1-multiplier:F2})");
                    }
                }
            }
            orig(victimObject, attackerObject, dotIndex, duration, damageMultiplier);
        }
    }
}