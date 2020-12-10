
using System.Collections.Generic;
using System.Collections.ObjectModel;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.Engi.EngiWeapon;
using EntityStates.Merc.Weapon;
using EntityStates.Mage.Weapon;
using EntityStates.Toolbot;
using EntityStates.Treebot.Weapon;
using EntityStates.Captain.Weapon;

namespace RiskOfBulletstorm.Items
{
    public class RingTriggers : Item_V2<RingTriggers>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius? (Value: Degrees, Min: 0, Max 360)", AutoConfigFlags.PreventNetMismatch)]
        public float RingTriggers_Radius { get; private set; } = 360f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many times to fire within the radius? (Min: 0)", AutoConfigFlags.PreventNetMismatch)]
        public float RingTriggers_FireAmount { get; private set; } = 8f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many seconds should Ring of Triggers fire?", AutoConfigFlags.PreventNetMismatch)]
        public float RingTriggers_Duration { get; private set; } = 3f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional seconds should Ring of Triggers fire per stack?", AutoConfigFlags.PreventNetMismatch)]
        public float RingTriggers_DurationStack { get; private set; } = 1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What should the cooldown in seconds for the Ring of Triggers' ability??", AutoConfigFlags.PreventNetMismatch)]
        public float RingTriggers_Cooldown { get; private set; } = 10f;
        public override string displayName => "Ring of Triggers";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.EquipmentRelated });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Items == Guns\nSummons bullets on active item use.";

        protected override string GetDescString(string langid = null) => $"Using your <style=cIsDamage>equipment</style> fires your firstmost combat skill " +
            $"{RingTriggers_FireAmount} times within {RingTriggers_Radius} degrees for {RingTriggers_Duration} seconds +{RingTriggers_DurationStack} per stack";

        protected override string GetLoreString(string langID = null) => "This ring bestows upon its bearer the now obvious knowledge that within the walls of the Gungeon all items are actually guns in strange, distorted form. An artifact of the Order's belief in transgunstantiation.";

        private static float degrees;
        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            degrees = RingTriggers_Radius / RingTriggers_FireAmount;
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.EquipmentSlot.Execute += EquipmentSlot_Execute;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.EquipmentSlot.Execute -= EquipmentSlot_Execute;
        }

        private void EquipmentSlot_Execute(On.RoR2.EquipmentSlot.orig_Execute orig, EquipmentSlot self)
        {
            if (NetworkServer.active)
            {
                var characterBody = self.characterBody;
                if (characterBody)
                {
                    var gameObject = characterBody.gameObject;
                    var inputBank = characterBody.inputBank;
                    var master = characterBody.master;

                    /*
                    if (inputBank)
                    {
                        var skill1 = inputBank.skill1;
                        var wasPressed = skill1.down;
                        var lastDirection = inputBank.aimDirection;
                        var segment = RingTriggers_Radius / Mathf.Max(RingTriggers_FireAmount,1);
                        master.enabled = false;
                        for (int i = 0; i < RingTriggers_FireAmount; i++)
                        {
                            skill1.PushState(true);
                            inputBank.aimDirection = new Vector3(inputBank.aimDirection.x, segment*(i+1), inputBank.aimDirection.y);
                        }
                        skill1.PushState(wasPressed);
                        inputBank.aimDirection = lastDirection;
                        master.enabled = true;
                    }*/

                    var skillLocator = characterBody.skillLocator;
                    if (skillLocator)
                    {
                        var primary = skillLocator.primary;
                        //var secondary = skillLocator.secondary;
                        //var utility = skillLocator.utility;
                        //var special = skillLocator.special;
                        var attackType = "";

                        var soundString = "";

                        BulletAttack bulletAttack = new BulletAttack
                        {
                            owner = gameObject,
                            weapon = gameObject,
                            origin = inputBank.aimOrigin,
                            isCrit = Util.CheckRoll(characterBody.crit, master),
                            damage = characterBody.damage
                        };

                        OverlapAttack overlapAttack = new OverlapAttack
                        {
                            attacker = gameObject,
                            inflictor = gameObject,
                            teamIndex = characterBody.teamComponent.teamIndex
                        };

                        FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                        {
                            owner = gameObject,
                            crit = Util.CheckRoll(characterBody.crit, master),
                            damage = characterBody.damage,
                            force = 0
                        };

                        if (primary)
                        {
                            switch (skillLocator.primary.skillName)
                            {
                                case "Double Tap":
                                    attackType = "bullet";
                                    bulletAttack.minSpread = FirePistol.minSpread;
                                    bulletAttack.maxSpread = FirePistol.maxSpread;
                                    bulletAttack.damage *= FirePistol.damageCoefficient;
                                    bulletAttack.force = FirePistol.force;
                                    bulletAttack.hitEffectPrefab = FirePistol.hitEffectPrefab;
                                    break;
                                case "Laser Sword":
                                    attackType = "melee";
                                    overlapAttack.damageType = DamageType.ApplyMercExpose;
                                    overlapAttack.damage = characterBody.damage;
                                    soundString = EntityStates.Merc.Weapon.GroundLight2.slash3Sound;
                                    break;
                                case "Bouncing Grenades":
                                    attackType = "projectile";
                                    fireProjectileInfo.projectilePrefab = FireGrenades.projectilePrefab;
                                    fireProjectileInfo.damage *= FireGrenades.damageCoefficient;
                                    fireProjectileInfo.force = 0;
                                    fireProjectileInfo.target = null;
                                    fireProjectileInfo.speedOverride = -1;
                                    soundString = FireGrenades.attackSoundString;
                                    break;
                                case "Strafe":
                                    attackType = "orb";
                                    break;
                                case "Flurry":
                                    attackType = "orb";
                                    break;
                                case "Flame Bolt":
                                    attackType = "projectile";
                                    fireProjectileInfo.projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/MageFirebolt");
                                    fireProjectileInfo.damage *= 2.2f;
                                    fireProjectileInfo.force = 20f;
                                    break;
                                case "Plasma Bolt":
                                    attackType = "projectile";
                                    fireProjectileInfo.projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/MageLightningboltBasic");
                                    fireProjectileInfo.damage *= 2.2f;
                                    fireProjectileInfo.force = 20f;
                                    break;
                                case "Auto-Nailgun":
                                    attackType = "bullet";
                                    bulletAttack.bulletCount = 1;
                                    bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                                    bulletAttack.force = BaseNailgunState.force;
                                    bulletAttack.HitEffectNormal = BaseNailgunState.hitEffectPrefab;
                                    bulletAttack.procChainMask = default;
                                    bulletAttack.procCoefficient = BaseNailgunState.procCoefficient;
                                    bulletAttack.maxDistance = BaseNailgunState.maxDistance;
                                    bulletAttack.radius = 0f;
                                    bulletAttack.minSpread = 0f;
                                    bulletAttack.hitEffectPrefab = BaseNailgunState.hitEffectPrefab;
                                    bulletAttack.maxSpread = 0f;
                                    bulletAttack.smartCollision = false;
                                    bulletAttack.sniper = false;
                                    bulletAttack.tracerEffectPrefab = BaseNailgunState.tracerEffectPrefab;
                                    break;
                                case "Rebar Puncher":
                                    attackType = "bullet";
                                    bulletAttack.stopperMask = LayerIndex.world.mask;
                                    bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
                                    bulletAttack.hitEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/ImpactSpear");
                                    break;
                                case "Scrap Launcher":
                                    attackType = "projectile";
                                    fireProjectileInfo.projectilePrefab = Resources.Load<GameObject>("prefabs/projectiles/ToolbotGrenadeLauncherProjectile");
                                    fireProjectileInfo.damage *= 3.6f;
                                    break;
                                case "Power-Saw":
                                    attackType = "melee";
                                    overlapAttack.damage = FireBuzzsaw.damageCoefficientPerSecond * characterBody.damage / FireBuzzsaw.baseFireFrequency;
                                    overlapAttack.procCoefficient = FireBuzzsaw.procCoefficientPerSecond / FireBuzzsaw.baseFireFrequency;
                                    overlapAttack.hitEffectPrefab = FireBuzzsaw.impactEffectPrefab;
                                    //Transform modelTransform = characterBody.gameObject.GetModelTransform();
                                    //overlapAttack.hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Buzzsaw");
                                    break;
                                case "DIRECTIVE: Inject":
                                    attackType = "projectile";
                                    fireProjectileInfo.projectilePrefab = FireSyringe.finalProjectilePrefab;
                                    fireProjectileInfo.damage *= FireSyringe.damageCoefficient;
                                    fireProjectileInfo.force = FireSyringe.force;
                                    soundString = FireSyringe.finalAttackSound;
                                    break;
                                case "Knuckleboom":
                                    attackType = "melee";
                                    break;
                                case "Vicious Wounds":
                                    attackType = "melee";
                                    break;
                                case "Vulcan Shotgun":
                                    attackType = "bullet";
                                    bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                                    bulletAttack.procCoefficient = 0.75f;
                                    bulletAttack.bulletCount = 8;
                                    bulletAttack.damage *= 1.2f;
                                    bulletAttack.maxSpread = 5f;
                                    break;
                                // Enforcer //
                                case "Riot Shotgun":
                                    attackType = "bullet";
                                    break;
                                case "Super Shotgun":
                                    attackType = "bullet";
                                    break;
                                case "Assault Rifle":
                                    attackType = "bullet";
                                    break;
                                default:
                                    break;
                            }
                        }

                        switch (attackType)
                        {
                            case "bullet":
                                for (int i = 0; i < RingTriggers_FireAmount; i++)
                                {
                                    bulletAttack.origin = characterBody.characterMotor.capsuleCollider.center;
                                    bulletAttack.aimVector = new Vector3(0,degrees*i,0);
                                    bulletAttack.Fire();
                                    Util.PlaySound(soundString, gameObject);
                                }

                                break;
                            case "projectile":

                                for (int i = 0; i < RingTriggers_FireAmount; i++)
                                {
                                    fireProjectileInfo.position = characterBody.characterMotor.capsuleCollider.center;
                                    fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(new Vector3(0, degrees * i, 0));
                                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                                    Util.PlaySound(soundString, gameObject);
                                }
                                break;
                            default:
                                break;
                        }

                    }
                }
            }
            orig(self);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity Engine")]
        public class RingTriggersComponent : MonoBehaviour
        {
            public float cooldown = instance.RingTriggers_Cooldown;
            private float lifetime = 0f;
            private bool isReady = true;

            void FixedUpdate()
            {
                if (lifetime >= 0)
                    lifetime -= Time.deltaTime;
                else
                {
                    isReady = true;
                }
            }


        }
    }
}
