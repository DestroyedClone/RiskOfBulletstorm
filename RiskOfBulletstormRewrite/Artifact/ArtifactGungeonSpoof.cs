using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using R2API;
using R2API.Utils;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactGungeonSpoof : ArtifactBase<ArtifactGungeonSpoof>
    {
        public override string ArtifactName => "Artifact of the Gungeon Mimic";

        public override string ArtifactLangTokenName => "GUNGEONSPOOF";

        public override Sprite ArtifactEnabledIcon => Assets.NullSprite;

        public override Sprite ArtifactDisabledIcon => Assets.NullSprite;

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateArtifact();
            Hooks();
        }

        public override void Hooks()
        {
            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;
        }

        private void RunArtifactManager_onArtifactDisabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            RoR2.CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
            GlobalEventManager.onServerDamageDealt -= DamageDealt;
            On.RoR2.HealthComponent.TakeDamage -= OnTakeDamage;
            //On.RoR2.HealthComponent.Heal -= OnHeal;
            Run.onRunStartGlobal -= StartRun;
            R2API.RecalculateStatsAPI.GetStatCoefficients -= Mustache_StatCoefficients;
        }


        private void RunArtifactManager_onArtifactEnabledGlobal([JetBrains.Annotations.NotNull] RunArtifactManager runArtifactManager, [JetBrains.Annotations.NotNull] ArtifactDef artifactDef)
        {
            if (artifactDef != ArtifactDef)
            {
                return;
            }
            RoR2.CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            GlobalEventManager.onServerDamageDealt += DamageDealt;
            On.RoR2.HealthComponent.TakeDamage += OnTakeDamage;
            //On.RoR2.HealthComponent.Heal += OnHeal;
            Run.onRunStartGlobal += StartRun;
            if (NetworkServer.active)
            {
                RunArtifactManager.instance.SetArtifactEnabledServer(ArtifactAdaptiveArmor.instance.ArtifactDef, true);
                RunArtifactManager.instance.SetArtifactEnabledServer(ArtifactEquipmentRecharge.instance.ArtifactDef, true);
                RunArtifactManager.instance.SetArtifactEnabledServer(ArtifactUpSpeedOOC.instance.ArtifactDef, true);
            }
            R2API.RecalculateStatsAPI.GetStatCoefficients += Mustache_StatCoefficients;
        }
        
        private void Mustache_StatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                var inventory = sender.inventory;
                int steakCount = inventory.GetItemCount(RoR2Content.Items.FlatHealth);
                if (steakCount > 0)
                {
                    args.baseHealthAdd -= 24 * steakCount;
                }
                int slugCount = inventory.GetItemCount(RoR2Content.Items.HealWhileSafe);
                if (slugCount > 0 && sender.outOfCombat)
                {
                    args.baseRegenAdd -= 2;
                }
            }
        }

        private void StartRun(Run run)
        {
            foreach (var item in new ItemDef[]{
                RoR2Content.Items.Mushroom,
                RoR2Content.Items.IgniteOnKill,
                RoR2Content.Items.Medkit,
                //monstertooth,
                RoR2Content.Items.ArmorPlate,
                //feather?
                RoR2Content.Items.Infusion,
                RoR2Content.Items.Seed,
                //guiotinne?
                RoR2Content.Items.ParentEgg,
                RoR2Content.Items.Knurl
            })
            {
                run.DisableItemDrop(item.itemIndex);
            }
        }

        private float OnHeal(On.RoR2.HealthComponent.orig_Heal orig, RoR2.HealthComponent self, float amount, RoR2.ProcChainMask procChainMask, bool nonRegen)
        {
            var original = orig(self, amount, procChainMask, nonRegen);
            if (!nonRegen)
                amount = 0;
            return original;
        }

        private void OnTakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            bool modified = false;
            var body = self.body;
            if (body && !damageInfo.rejected)
            {
                if (body.isPlayerControlled)
                {
                    //this it to make sure it stays between
                    //0-2 damage (0hearts to 1 heart)
                    damageInfo.damage = Mathf.RoundToInt(damageInfo.damage);
                    
                    if (damageInfo.damage < 1)
                    {
                        damageInfo.rejected = true;
                    } else {
                        if (damageInfo.crit)
                        {
                            damageInfo.damage = 1;
                        } else {
                            damageInfo.damage = 2;
                        }
                    }
                    foreach (var dt in new DamageType[]{
                        DamageType.BleedOnHit,
                        DamageType.BlightOnHit,
                        DamageType.BonusToLowHealth,
                        DamageType.BypassArmor,
                        DamageType.BypassBlock,
                        DamageType.BypassOneShotProtection,
                        DamageType.IgniteOnHit,
                        DamageType.PercentIgniteOnHit,
                        DamageType.PoisonOnHit,
                        DamageType.SuperBleedOnCrit
                    })
                    {
                        damageInfo.damageType |= dt;
                    }
                    modified = true;
                }
            }
            orig(self, damageInfo);
            if (modified)
            {
                self.body.AddTimedBuff(RoR2Content.Buffs.Immune, 1f);
            }
        }

        private void DamageDealt(DamageReport damageReport)
        {

        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody characterBody)
        {
            if (NetworkServer.active && characterBody.inventory && characterBody.healthComponent)
            {
                ApplyBodyModifier(characterBody);
            }
        }

        private void ApplyBodyModifier(CharacterBody body)
        {
            body.baseMaxHealth = Mathf.RoundToInt(body.baseMaxHealth / 4);
            body.levelMaxHealth = 0;
            body.healthComponent.health = body.baseMaxHealth;
            body.baseRegen = 0;
            body.levelRegen = 0;
            body.barrierDecayRate = 0;
            body.crit = 0;
            body.baseArmor = 0;
            body.levelArmor = 0;

            body.baseDamage = Mathf.RoundToInt(body.baseDamage / 4);
            body.levelDamage = 0;
            body.RecalculateStats();
        }
    }
}