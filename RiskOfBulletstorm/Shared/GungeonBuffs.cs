using System.Collections.Generic;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using RoR2.Artifacts;
using System.Linq;
using RoR2.Networking;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;


namespace RiskOfBulletstorm.Items
{
    public class GungeonBuffController : Item_V2<GungeonBuffController>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("CHARM: Should Bosses (including Umbras) get charmed?", AutoConfigFlags.PreventNetMismatch)]
        public bool Config_Charm_Boss { get; private set; } = false;

        public override string displayName => "GungeonBuffController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";
        //public static BuffIndex Burn { get; private set; } 
        //public static BuffIndex Poison { get; private set; }
        //public static BuffIndex Curse { get; private set; }
        //public static BuffIndex Stealth { get; private set; }
        //public static BuffIndex Petrification { get; private set; }
        public static BuffIndex Anger { get; private set; } // done
        //public static BuffIndex Buffed { get; private set; }
        //public static BuffIndex BurnEnemy { get; private set; } //burn without heal negation
        //public static BuffIndex PoisonEnemy { get; private set; } //blight
        public static BuffIndex Charm { get; private set; } // done
        //public static BuffIndex Encheesed { get; private set; }
        //public static BuffIndex Fear { get; private set; } //classic items' fear
        public static BuffIndex Jammed { get; private set; } // done
        //public static BuffIndex Slow { get; private set; } // slow
        //public static BuffIndex Freeze { get; private set; } 
        //public static BuffIndex Stun { get; private set; } // existing status from damagetype
        //public static BuffIndex Weakened { get; private set; }
        //public static BuffIndex Tangled { get; private set; }
        //public static BuffIndex Encircled { get; private set; }
        //public static BuffIndex Glittered { get; private set; }
        //public static BuffIndex Bloody { get; private set; }

        //private static readonly float HeartValue = 50f;

        private ItemIndex SpiceTally;

        private float[] SpiceBonusesAdditive;
        private float[,] SpiceBonuses;
        private float[] SpiceBonusesConstantMaxed;

        private readonly CurseController curse = Items.CurseController.instance;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        //https://enterthegungeon.gamepedia.com/Status_Effects
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            var angerBuff = new CustomBuff(
            new BuffDef
            {
                buffColor = Color.red,
                canStack = false,
                isDebuff = false,
                name = "Enraged",
            });
            Anger = BuffAPI.Add(angerBuff);

            var charmedBuff = new CustomBuff(
            new BuffDef
            {
                name = "Charmed",
                buffColor = new Color32(201, 42, 193, 255),
                canStack = false,
                isDebuff = true,
                iconPath = "",
            });
            Charm = BuffAPI.Add(charmedBuff);

            var jammedBuff = new CustomBuff(
            new BuffDef
            {
                name = "Affix_Jammed",
                buffColor = new Color32(150, 10, 10, 255),
                canStack = false,
                isDebuff = false,
                iconPath = "",
                //eliteIndex = JammedEliteIndex,
            });
            Jammed = BuffAPI.Add(jammedBuff);
        }

        public override void SetupLate()
        {
            base.SetupLate();
            //so we dont crash
            {
                SpiceTally = Spice.SpiceTally;
                SpiceBonusesAdditive = Spice.SpiceBonusesAdditive;
                SpiceBonuses = Spice.SpiceBonuses;
                SpiceBonusesConstantMaxed = Spice.SpiceBonusesConstantMaxed;
            } //Spice Setup

        }
        public override void Install()
        {
            base.Install();
            GetStatCoefficients += AddRewards;
            GetStatCoefficients += AddSpiceRewards;
            On.RoR2.HealthComponent.TakeDamage += SPICE_HealthComponent_TakeDamage;
            // CHARM //
            {
                // Spawn //
                On.RoR2.CharacterBody.Start += Charmed_AddComponent;
                // Buff //
                On.RoR2.CharacterBody.AddBuff += Charmed_EnableComponent;
                On.RoR2.CharacterBody.RemoveBuff += Charmed_DisableComponent;
                // AI //
                On.RoR2.CharacterAI.BaseAI.FindEnemyHurtBox += BaseAI_FindEnemyHurtBox;
                On.RoR2.CharacterAI.BaseAI.OnBodyDamaged += BaseAI_OnBodyDamaged;
                // FriendlyFire Bypass //
                On.RoR2.BulletAttack.DefaultHitCallback += BulletAttack_DefaultHitCallback;
                On.RoR2.DamageTrail.DoDamage += DamageTrail_DoDamage;
                On.RoR2.OverlapAttack.HurtBoxPassesFilter += OverlapAttack_HurtBoxPassesFilter;
                On.RoR2.Projectile.ProjectileSingleTargetImpact.OnProjectileImpact += ProjectileSingleTargetImpact_OnProjectileImpact;
            } // Charm Hooks
        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients -= AddRewards;
            GetStatCoefficients -= AddSpiceRewards;
            On.RoR2.HealthComponent.TakeDamage -= SPICE_HealthComponent_TakeDamage;
            // CHARM //
            {
                // Spawn //
                On.RoR2.CharacterBody.Start -= Charmed_AddComponent;
                // Buff //
                On.RoR2.CharacterBody.AddBuff -= Charmed_EnableComponent;
                On.RoR2.CharacterBody.RemoveBuff -= Charmed_DisableComponent;
                // AI //
                On.RoR2.CharacterAI.BaseAI.FindEnemyHurtBox -= BaseAI_FindEnemyHurtBox;
                On.RoR2.CharacterAI.BaseAI.OnBodyDamaged -= BaseAI_OnBodyDamaged;
                // FriendlyFire Bypass //
                On.RoR2.BulletAttack.DefaultHitCallback -= BulletAttack_DefaultHitCallback;
                On.RoR2.DamageTrail.DoDamage -= DamageTrail_DoDamage;
                On.RoR2.OverlapAttack.HurtBoxPassesFilter -= OverlapAttack_HurtBoxPassesFilter;
                On.RoR2.Projectile.ProjectileSingleTargetImpact.OnProjectileImpact -= ProjectileSingleTargetImpact_OnProjectileImpact;
            } // Charm Hooks
        }
        private void AddSpiceRewards(CharacterBody sender, StatHookEventArgs args)
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
                        args.healthMultAdd *= 1 + SpiceBonuses[SpiceTallyCount, 0];
                        args.attackSpeedMultAdd += SpiceBonuses[SpiceTallyCount, 1];
                        //accuracy 
                        //enemy bullet speed
                        //damage
                        break;
                    default:
                        //var baseHealthAdd = HeartValue * SpiceBonusesAdditive[0] * (SpiceTallyCount - 4);
                        //args.baseHealthAdd += baseHealthAdd;
                        args.healthMultAdd *= Math.Min(0.1f, 1 + SpiceBonusesAdditive[0] * (SpiceTallyCount - 4));
                        //health, attack speed, shot accuracy, enemy bullet speed, damage
                        args.attackSpeedMultAdd += SpiceBonusesConstantMaxed[1];
                        //accuracy
                        //enemy
                        //damage
                        break;
                }
            }
        }
        private void AddRewards(CharacterBody sender, StatHookEventArgs args)
        {
            /*
             * JAMMED
            [0] Health: Handled Here
            [1] Attack Speed: Handled Here
            [2] Accuracy: BulletstormExtraStatsController
            [3] Enemy Bullet Speed: BulletstormExtraStatsController
            [4] Damage: HealthComponent_TakeDamage
            */
            if (sender.HasBuff(Anger)) { args.damageMultAdd += EnragingPhoto.instance.EnragingPhoto_DmgBoost; }
            if (sender.HasBuff(Jammed))
            {
                args.damageMultAdd += curse.Curse_DamageBoost;
                args.critAdd += curse.Curse_CritBoost;
                args.attackSpeedMultAdd += curse.Curse_AttackSpeedBoost;
                args.moveSpeedMultAdd += curse.Curse_MoveSpeedBoost;
                args.baseHealthAdd += curse.Curse_HealthBoost;
            }
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "Used Values")]
        private void SPICE_HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
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
                        //var DamageMult = 0f;
                        var SpiceMult = 0f;
                        switch (SpiceTallyCount)
                        {
                            case 0: //
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                                SpiceMult = SpiceBonuses[SpiceTallyCount, 4];
                                break;
                            default: //also 5
                                SpiceMult = SpiceBonuses[4, 4] + SpiceBonusesAdditive[4] * (SpiceTallyCount - 4);
                                break;
                        }
                        //DamageMult = SpiceMult;
                        damageInfo.damage *= 1 + SpiceMult;
                    }
                }
            }
            orig(self, damageInfo);
        }

        /*
        *   CHARM
        *   CHARM
        */

        // Spawn //

        private void Charmed_AddComponent(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);

            if (!self.isPlayerControlled)
            {
                var baseAI = self.masterObject.GetComponent<BaseAI>();
                if (baseAI)
                {
                    var isCharmed = self.gameObject.GetComponent<IsCharmed>();
                    if (!isCharmed) isCharmed = self.gameObject.AddComponent<IsCharmed>();

                    isCharmed.characterBody = self;
                    isCharmed.teamComponent = self.teamComponent;
                    isCharmed.baseAI = baseAI;
                    isCharmed.oldTeamIndex = self.teamComponent.teamIndex;
                }
            }
        }

        // Buff //
        private void Charmed_EnableComponent(On.RoR2.CharacterBody.orig_AddBuff orig, CharacterBody self, BuffIndex buffType)
        {
            if (buffType == Charm)
            {
                if (!Config_Charm_Boss && self.isBoss) //prevents adding the buff if it's a boss and the config is disabled
                    return;

                var isCharmed = self.gameObject.GetComponent<IsCharmed>();
                if (isCharmed && !isCharmed.enabled)
                {
                    isCharmed.enabled = true;
                }
            }
            orig(self, buffType);
        }
        private void Charmed_DisableComponent(On.RoR2.CharacterBody.orig_RemoveBuff orig, CharacterBody self, BuffIndex buffType)
        {
            if (buffType == Charm)
            {
                var isCharmed = self.gameObject.GetComponent<IsCharmed>();
                if (isCharmed && isCharmed.enabled)
                    isCharmed.enabled = false;
            }
            orig(self, buffType);
        }
        // AI //
        private void BaseAI_OnBodyDamaged(On.RoR2.CharacterAI.BaseAI.orig_OnBodyDamaged orig, BaseAI self, DamageReport damageReport)
        {
            var isCharmed = self.body.gameObject.GetComponent<IsCharmed>();
            if (isCharmed && isCharmed.enabled)
            {
                DamageInfo damageInfo = damageReport.damageInfo;
                var noTarget = (!self.currentEnemy.gameObject || self.enemyAttention <= 0f);
                var attackerNotSelf = damageInfo.attacker != self.body.gameObject;
                var retaliate = (!self.neverRetaliateFriendlies || !damageReport.isFriendlyFire);
                var attackerIsCharmerTeam = damageReport.attackerTeamIndex == isCharmed.GetOppositeTeamIndex(isCharmed.GetOldTeam());
                if (attackerIsCharmerTeam)
                {
                    //Debug.Log("BaseAI: Target was from the charmer's team.");
                    if (noTarget && attackerNotSelf && retaliate)
                    {
                        return;
                    }
                }
            }
            orig(self, damageReport);
        }
        private HurtBox BaseAI_FindEnemyHurtBox(On.RoR2.CharacterAI.BaseAI.orig_FindEnemyHurtBox orig, RoR2.CharacterAI.BaseAI self, float maxDistance, bool full360Vision, bool filterByLoS)
        {
            var isCharmed = self.body.gameObject.GetComponent<IsCharmed>();
            if (isCharmed && isCharmed.enabled)
            {
                self.enemySearch.viewer = self.body;
                self.enemySearch.teamMaskFilter = TeamMask.allButNeutral;
                self.enemySearch.teamMaskFilter.RemoveTeam(isCharmed.GetOppositeTeamIndex(isCharmed.GetOldTeam()));
                self.enemySearch.sortMode = BullseyeSearch.SortMode.Distance;
                self.enemySearch.minDistanceFilter = self.body.radius;
                self.enemySearch.maxDistanceFilter = maxDistance;
                self.enemySearch.searchOrigin = self.bodyInputBank.aimOrigin;
                self.enemySearch.searchDirection = self.bodyInputBank.aimDirection;
                self.enemySearch.maxAngleFilter = (full360Vision ? 180f : 90f);
                self.enemySearch.filterByLoS = filterByLoS;
                self.enemySearch.RefreshCandidates();
                var list = self.enemySearch.GetResults().ToList();
                //Debug.Log("findennemyhurtbox: "+ list.FirstOrDefault<HurtBox>());
                if (list.Count > 1) //If there are targets
                    list.RemoveAt(0); //remove the first one because its usually themself

                if (list.Count > 0) //now list doesn't include self
                    return list.FirstOrDefault<HurtBox>(); //if there's still a target
                else
                {
                    return null;
                }
            }
            return orig(self, maxDistance, full360Vision, filterByLoS);
        }
        // FriendlyFire Bypass //
        private bool BulletAttack_DefaultHitCallback(On.RoR2.BulletAttack.orig_DefaultHitCallback orig, BulletAttack self, ref BulletAttack.BulletHit hitInfo)
        {
            var owner = self.owner;
            if (owner) //is there an owner
            {
                var component = owner.gameObject.GetComponent<IsCharmed>();
                if (component && component.enabled) //are they charmed?
                {
                    bool result = false;
                    if (hitInfo.collider)
                    {
                        result = ((1 << hitInfo.collider.gameObject.layer & self.stopperMask) == 0);
                    }
                    if (self.hitEffectPrefab)
                    {
                        EffectManager.SimpleImpactEffect(self.hitEffectPrefab, hitInfo.point, self.HitEffectNormal ? hitInfo.surfaceNormal : (-hitInfo.direction), true);
                    }
                    if (hitInfo.collider)
                    {
                        SurfaceDef objectSurfaceDef = SurfaceDefProvider.GetObjectSurfaceDef(hitInfo.collider, hitInfo.point);
                        if (objectSurfaceDef && objectSurfaceDef.impactEffectPrefab)
                        {
                            EffectData effectData = new EffectData
                            {
                                origin = hitInfo.point,
                                rotation = Quaternion.LookRotation(hitInfo.surfaceNormal),
                                color = objectSurfaceDef.approximateColor,
                                surfaceDefIndex = objectSurfaceDef.surfaceDefIndex
                            };
                            EffectManager.SpawnEffect(objectSurfaceDef.impactEffectPrefab, effectData, true);
                        }
                    }
                    if (self.isCrit)
                    {
                        EffectManager.SimpleImpactEffect(Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/Critspark"), hitInfo.point, self.HitEffectNormal ? hitInfo.surfaceNormal : (-hitInfo.direction), true);
                    }
                    GameObject entityObject = hitInfo.entityObject;
                    if (entityObject)
                    {
                        float num = 1f;
                        switch (self.falloffModel)
                        {
                            case BulletAttack.FalloffModel.None:
                                num = 1f;
                                break;
                            case BulletAttack.FalloffModel.DefaultBullet:
                                num = 0.5f + Mathf.Clamp01(Mathf.InverseLerp(60f, 25f, hitInfo.distance)) * 0.5f;
                                break;
                            case BulletAttack.FalloffModel.Buckshot:
                                num = 0.25f + Mathf.Clamp01(Mathf.InverseLerp(25f, 7f, hitInfo.distance)) * 0.75f;
                                break;
                        }
                        DamageInfo damageInfo = new DamageInfo
                        {
                            damage = self.damage * num,
                            crit = self.isCrit,
                            attacker = self.owner,
                            inflictor = self.weapon,
                            position = hitInfo.point,
                            force = hitInfo.direction * (self.force * num),
                            procChainMask = self.procChainMask,
                            procCoefficient = self.procCoefficient,
                            damageType = self.damageType,
                            damageColorIndex = self.damageColorIndex
                        };
                        damageInfo.ModifyDamageInfo(hitInfo.damageModifier);
                        HealthComponent healthComponent = null;
                        if (hitInfo.hitHurtBox)
                        {
                            healthComponent = hitInfo.hitHurtBox.healthComponent;
                        }
                        bool flag = healthComponent;
                        if (NetworkServer.active)
                        {
                            if (flag)
                            {
                                healthComponent.TakeDamage(damageInfo);
                                GlobalEventManager.instance.OnHitEnemy(damageInfo, hitInfo.entityObject);
                            }
                            GlobalEventManager.instance.OnHitAll(damageInfo, hitInfo.entityObject);
                        }
                        else if (ClientScene.ready)
                        {
                            BulletAttack.messageWriter.StartMessage(53);
                            BulletAttack.messageWriter.Write(entityObject);
                            BulletAttack.messageWriter.Write(damageInfo);
                            BulletAttack.messageWriter.Write(flag);
                            BulletAttack.messageWriter.FinishMessage();
                            ClientScene.readyConnection.SendWriter(BulletAttack.messageWriter, QosChannelIndex.defaultReliable.intVal);
                        }
                    }
                    return result;
                }
            }

            return orig(self, ref hitInfo);
        }
        private void ProjectileSingleTargetImpact_OnProjectileImpact(On.RoR2.Projectile.ProjectileSingleTargetImpact.orig_OnProjectileImpact orig, RoR2.Projectile.ProjectileSingleTargetImpact self, RoR2.Projectile.ProjectileImpactInfo impactInfo)
        {
            if (!self.alive)
            {
                return;
            }
            var owner = self.projectileController.owner;
            if (owner)
            {
                var isCharmed = owner.gameObject.GetComponent<IsCharmed>();
                if (isCharmed && isCharmed.enabled)
                {
                    Collider collider = impactInfo.collider;
                    if (collider)
                    {
                        DamageInfo damageInfo = new DamageInfo();
                        if (self.projectileDamage)
                        {
                            damageInfo.damage = self.projectileDamage.damage;
                            damageInfo.crit = self.projectileDamage.crit;
                            damageInfo.attacker = self.projectileController.owner;
                            damageInfo.inflictor = self.gameObject;
                            damageInfo.position = impactInfo.estimatedPointOfImpact;
                            damageInfo.force = self.projectileDamage.force * self.transform.forward;
                            damageInfo.procChainMask = self.projectileController.procChainMask;
                            damageInfo.procCoefficient = self.projectileController.procCoefficient;
                            damageInfo.damageColorIndex = self.projectileDamage.damageColorIndex;
                            damageInfo.damageType = self.projectileDamage.damageType;
                        }
                        else
                        {
                            Debug.Log("No projectile damage component!");
                        }
                        HurtBox component = collider.GetComponent<HurtBox>();
                        if (component)
                        {
                            HealthComponent healthComponent = component.healthComponent;
                            if (healthComponent)
                            {
                                if (healthComponent.gameObject == self.projectileController.owner)
                                {
                                    return;
                                }
                                Util.PlaySound(self.enemyHitSoundString, self.gameObject);
                                if (NetworkServer.active)
                                {
                                    damageInfo.ModifyDamageInfo(component.damageModifier);
                                    healthComponent.TakeDamage(damageInfo);
                                    GlobalEventManager.instance.OnHitEnemy(damageInfo, component.healthComponent.gameObject);
                                }

                                self.alive = false;
                            }
                        }
                        else if (self.destroyOnWorld)
                        {
                            self.alive = false;
                        }
                        damageInfo.position = self.transform.position;
                        if (NetworkServer.active)
                        {
                            GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
                        }
                    }
                    if (!self.alive)
                    {
                        if (NetworkServer.active && self.impactEffect)
                        {
                            EffectManager.SimpleImpactEffect(self.impactEffect, impactInfo.estimatedPointOfImpact, -self.transform.forward, !self.projectileController.isPrediction);
                        }
                        Util.PlaySound(self.hitSoundString, self.gameObject);
                        if (self.destroyWhenNotAlive)
                        {
                            UnityEngine.Object.Destroy(self.gameObject);
                        }
                    }
                    return;
                }
            }

            orig(self, impactInfo);
        }
        private bool OverlapAttack_HurtBoxPassesFilter(On.RoR2.OverlapAttack.orig_HurtBoxPassesFilter orig, OverlapAttack self, HurtBox hurtBox)
        {
            var owner = self.attacker;
            if (owner)
            {
                var charm = owner.gameObject.GetComponent<IsCharmed>();
                if (charm && charm.enabled)
                { //removed friendlyfire check
                    return !hurtBox.healthComponent || ((!(hurtBox.healthComponent.gameObject == self.attacker) || self.attackerFiltering != AttackerFiltering.NeverHit) && (!(self.attacker == null) || !(hurtBox.healthComponent.gameObject.GetComponent<MaulingRock>() != null)) && !self.ignoredHealthComponentList.Contains(hurtBox.healthComponent));
                }
            }

            return orig(self, hurtBox);
        }
        private void DamageTrail_DoDamage(On.RoR2.DamageTrail.orig_DoDamage orig, DamageTrail self)
        {
            var owner = self.owner;
            if (self.owner)
            {
                var component = owner.gameObject.GetComponent<IsCharmed>();
                if (component && component.enabled)
                {
                    if (self.pointsList.Count == 0)
                    {
                        return;
                    }
                    float damage = self.damagePerSecond * self.updateInterval;
                    Vector3 vector = self.pointsList[self.pointsList.Count - 1].position;
                    HashSet<GameObject> hashSet = new HashSet<GameObject>
                    {
                        owner
                    };
                    for (int i = self.pointsList.Count - 2; i >= 0; i--)
                    {
                        Vector3 position = self.pointsList[i].position;
                        Vector3 direction = position - vector;
                        RaycastHit[] array = Physics.SphereCastAll(new Ray(vector, direction), self.radius, direction.magnitude, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.UseGlobal);
                        for (int j = 0; j < array.Length; j++)
                        {
                            Collider collider = array[j].collider;
                            if (collider.gameObject)
                            {
                                HurtBox component1 = collider.GetComponent<HurtBox>();
                                if (component1)
                                {
                                    HealthComponent healthComponent = component1.healthComponent;
                                    if (healthComponent)
                                    {
                                        GameObject gameObject = healthComponent.gameObject;
                                        if (!hashSet.Contains(gameObject))
                                        {
                                            hashSet.Add(gameObject); //friendlyfire normally here
                                            healthComponent.TakeDamage(new DamageInfo
                                            {
                                                position = array[j].point,
                                                attacker = self.owner,
                                                inflictor = self.gameObject,
                                                crit = false,
                                                damage = damage,
                                                damageColorIndex = DamageColorIndex.Item,
                                                damageType = DamageType.Generic,
                                                force = Vector3.zero,
                                                procCoefficient = 0f
                                            });
                                        }
                                    }
                                }
                            }
                        }
                        vector = position;
                    }
                    return;
                }
            }
            orig(self);
        }

        public class IsCharmed : MonoBehaviour
        {
            //public float duration = CharmHorn.instance.CharmHorn_Duration;
            public CharacterBody characterBody;
            public TeamComponent teamComponent;
            public TeamIndex oldTeamIndex;
            public BaseAI baseAI;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnEnable()
            {
                //if (characterBody.GetBuffCount(Charm) <= 0)
                //characterBody.AddTimedBuff(Charm, duration);
                //Debug.Log("Charm: OnEnable, last target was "+ baseAI.currentEnemy.characterBody.name);
                if (baseAI.currentEnemy.characterBody.teamComponent.teamIndex == GetOppositeTeamIndex(oldTeamIndex))
                {
                    ResetTarget();
                }
            }

            public void ResetTarget()
            {
                baseAI.currentEnemy.Reset();
                baseAI.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                //Debug.Log("Charm: Changed Target to " + baseAI.currentEnemy.characterBody.name);
            }

            public TeamIndex GetOppositeTeamIndex(TeamIndex teamIndex)
            {
                if (teamIndex == TeamIndex.Player) return TeamIndex.Monster;
                else if (teamIndex == TeamIndex.Monster) return TeamIndex.Player;
                else return teamIndex;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void FixedUpdate()
            {
                if (!characterBody.HasBuff(Charm))
                {
                    enabled = false;
                }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnDisable()
            {
                if (characterBody.HasBuff(Charm))
                    characterBody.RemoveBuff(Charm);
                characterBody.teamComponent.teamIndex = oldTeamIndex;
                ResetTarget();
            }

            public TeamIndex GetOldTeam()
            {
                return oldTeamIndex;
            }
        }

        // END CHARM //


        public class IsJammed : MonoBehaviour
        {
            public CharacterBody characterBody;
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnEnable()
            {
                //ContactDamageCooldown = ContactDamageCooldownFull;
                characterBody = gameObject.GetComponent<CharacterBody>();
                if (!characterBody.HasBuff(Jammed))
                {
                    characterBody.AddBuff(Jammed);
                }
            }
        }

    }
}
