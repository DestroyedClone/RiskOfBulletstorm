using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items.Legendary
{
    public class VorpalBullets : ItemBase<VorpalBullets>
    {
        public ConfigEntry<float> cfgChance;
        public ConfigEntry<float> cfgChancePerStack;
        public ConfigEntry<float> cfgDamageMultiplier;


        public override string ItemName => "Vorpal Bullets";

        public override string ItemLangTokenName => "VORPALBULLETS";

        public override ItemTier Tier => ItemTier.Tier3;

        public override GameObject ItemModel => Assets.NullModel;

        public override Sprite ItemIcon => Assets.NullSprite;

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            cfgChance = config.Bind(ConfigCategory, "Percent Chance", 1f, "The chance in percentage of the item to proc. 100 = 100%");
            cfgChancePerStack = config.Bind(ConfigCategory, "Percent Chance Per Stack", 1f, "The chance of the item per stack to proc. 100 = 100%");
            cfgDamageMultiplier = config.Bind(ConfigCategory, "Damage Multiplier On Proc", 20f, "The amount of damage that the resulting damage is multiplied by." +
                "\nBase Damage 2, Value 20, so resulting damage is 40");

        }

        public override void Hooks()
        {
            base.Hooks();
            On.RoR2.BulletAttack.Fire += BulletAttack_Fire;
        }

        private void BulletAttack_Fire(On.RoR2.BulletAttack.orig_Fire orig, BulletAttack self)
        {
            orig(self);
            if (self.owner
                && self.owner.TryGetComponent<CharacterBody>(out CharacterBody ownerBody))
            {
                var itemCount = GetCount(ownerBody);
                if (itemCount > 0)
                {
                    var chance = cfgChance.Value + cfgChancePerStack.Value * (itemCount - 1);
                    if (Util.CheckRoll(chance, ownerBody.master ? ownerBody.master : null))
                    {
                        self.damage *= cfgDamageMultiplier.Value;
                        PerformVisuals(ownerBody.gameObject);
                    }
                }
            }
        }

        public static GameObject BlankEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/SonicBoomEffect");
        public void PerformVisuals(GameObject attackerObject)
        {
            EffectManager.SpawnEffect(BlankEffect, new EffectData
            {
                origin = attackerObject.transform.position,
                rotation = Util.QuaternionSafeLookRotation(Vector3.up),
                scale = 40f
            }, false);
        }

        public class RBS_VorpalBulletsController : MonoBehaviour
        {
            public float stopwatch = 0;

            void Start()
            {
                StartCoroutine(SlowDownTime(2));
            }

            //https://docs.unity3d.com/ScriptReference/WaitForSecondsRealtime.html
            IEnumerator SlowDownTime(float duration)
            {
                //SlowDownTime()
                print(Time.time);
                yield return new WaitForSecondsRealtime(5);
                print(Time.time);
                //ResumeTime();
            }

            public void SetTimeScale()
            {

            }
        }
    }
}
