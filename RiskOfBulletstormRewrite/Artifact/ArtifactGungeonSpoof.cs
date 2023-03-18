using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Artifact
{
    internal class ArtifactGungeonSpoof : ArtifactBase<ArtifactGungeonSpoof>
    {
        public override string ArtifactName => "Artifact of the Gungeon Mimic";

        public override string ArtifactLangTokenName => "GUNGEONSPOOF";

        public override Sprite ArtifactEnabledIcon => LoadSprite(true);

        public override Sprite ArtifactDisabledIcon => LoadSprite(false);

        public override void Init(ConfigFile config)
        {
            CreateLang();
            CreateArtifact();
            CreateConfig(config);
            Hooks();
        }

        public static ConfigEntry<float> cfgIFrameDuration;
        public static ConfigEntry<float> cfgStatDivider;

        public override void CreateConfig(ConfigFile config)
        {
            cfgIFrameDuration = config.Bind(ConfigCategory, "I-Frames Duration", 0.5f, "The amount of seconds of immunity after getting hit.");
            cfgStatDivider = config.Bind(ConfigCategory, "Health and Damage Division Coefficient", 8f, "The value that the body's maxHealth and baseDamage are divided by.");
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
            R2API.RecalculateStatsAPI.GetStatCoefficients -= GungeonMimic_StatCoefficients;
            On.RoR2.HealthComponent.ServerFixedUpdate -= PreventBarrierLoss;
            On.RoR2.HealthComponent.AddBarrier -= HealthComponent_AddBarrier;
            On.RoR2.Inventory.CalculateEquipmentCooldownScale -= IncreaseCooldowns;
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
                //RunArtifactManager.instance.SetArtifactEnabledServer(ArtifactAdaptiveArmor.instance.ArtifactDef, true);
                //RunArtifactManager.instance.SetArtifactEnabledServer(ArtifactEquipmentRecharge.instance.ArtifactDef, true);
                //RunArtifactManager.instance.SetArtifactEnabledServer(ArtifactUpSpeedOOC.instance.ArtifactDef, true);
                //RunArtifactManager.instance.SetArtifactEnabledServer(ArtifactCoyoteTime.instance.ArtifactDef, true);
            }
            R2API.RecalculateStatsAPI.GetStatCoefficients += GungeonMimic_StatCoefficients;
            On.RoR2.HealthComponent.ServerFixedUpdate += PreventBarrierLoss;
            On.RoR2.HealthComponent.AddBarrier += HealthComponent_AddBarrier;
            On.RoR2.Inventory.CalculateEquipmentCooldownScale += IncreaseCooldowns;
        }

        private float IncreaseCooldowns(On.RoR2.Inventory.orig_CalculateEquipmentCooldownScale orig, Inventory self)
        {
            var original = orig(self);

            original *= 1.25f;

            return original;
        }

        private void HealthComponent_AddBarrier(On.RoR2.HealthComponent.orig_AddBarrier orig, HealthComponent self, float value)
        {
            value = Mathf.RoundToInt(value / cfgStatDivider.Value);
            orig(self, value);
        }

        private void PreventBarrierLoss(On.RoR2.HealthComponent.orig_ServerFixedUpdate orig, HealthComponent self)
        {
            self.body.barrierDecayRate = 0;
            orig(self);
        }

        private void GungeonMimic_StatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                if (sender.inventory)
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
            if (NetworkServer.active)
            {
                if (body && !damageInfo.rejected)
                {
                    if (body.isPlayerControlled && !body.HasBuff(RoR2Content.Buffs.Immune))
                    {
                        //this it to make sure it stays between
                        //0-2 damage (0hearts to 1 heart)
                        damageInfo.damage = Mathf.RoundToInt(damageInfo.damage);

                        if (damageInfo.damage < 1)
                        {
                            damageInfo.rejected = true;
                        }
                        else
                        {
                            if (damageInfo.crit)
                            {
                                damageInfo.damage = 1;
                            }
                            else
                            {
                                damageInfo.damage = 2;
                            }
                        }
                        //temporary unsetting this so we dont have to deal with
                        //weird DoTs
                        foreach (var dt in new DamageType[]{
                            DamageType.BleedOnHit,
                            DamageType.BlightOnHit,
                            DamageType.BonusToLowHealth,
                            DamageType.BypassArmor,
                            DamageType.BypassBlock, //needs to not pierce IFrames
                            DamageType.BypassOneShotProtection,
                            DamageType.IgniteOnHit,
                            DamageType.PercentIgniteOnHit,
                            DamageType.PoisonOnHit,
                            DamageType.SuperBleedOnCrit
                        })
                        { //stackoverflow unsetting an enum flag
                            damageInfo.damageType &= ~dt;
                        }
                        modified = true;
                    }
                }
            }

            orig(self, damageInfo);
            if (NetworkServer.active && modified)
            {
                self.body.AddTimedBuff(RoR2Content.Buffs.Immune, cfgIFrameDuration.Value);
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

        [Server] //>using something you dont know what its for
        private void ApplyBodyModifier(CharacterBody body)
        {
            body.baseMaxHealth = Mathf.RoundToInt(body.baseMaxHealth / cfgStatDivider.Value);
            body.levelMaxHealth = 0;
            body.baseRegen = 0;
            body.levelRegen = 0;
            body.barrierDecayRate = 0;
            body.crit = 0;
            float armorToConvertToBarrier = body.baseArmor;
            body.baseArmor = 0;
            body.levelArmor = 0;

            body.baseDamage = Mathf.RoundToInt(body.baseDamage / cfgStatDivider.Value);
            body.levelDamage = 0;
            body.autoCalculateLevelStats = false;
            body.RecalculateStats();

            if (armorToConvertToBarrier > 0 && body.healthComponent)
            {
                var evaluatedBarrier = Mathf.RoundToInt(armorToConvertToBarrier / cfgStatDivider.Value);
                body.healthComponent.AddBarrier(evaluatedBarrier);
            }
        }
    }
}