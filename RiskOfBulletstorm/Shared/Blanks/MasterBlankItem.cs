using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;
using RoR2.Projectile;
using RiskOfBulletstorm.Items;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Shared.Blanks
{
    public class MasterBlankItem : Item<MasterBlankItem>
    {
        // Controls Blank logic because i don't know how to make this run without making it an item. what a newbie
        public override string displayName => "";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";
        public static GameObject BlankObject = new GameObject(); // { get; private set; }
        public static string BlankSound = EntityStates.Treebot.Weapon.FirePlantSonicBoom.impactSoundString;
        //private readonly string BlankEffect = "Prefabs/Effects/TreebotShockwaveEffect";
        public static GameObject BlankEffect = (GameObject)Resources.Load("prefabs/effects/SonicBoomEffect");
        public static GameObject ProjectileDeleteEffect = Resources.Load<GameObject>("prefabs/effects/SmokescreenEffect");

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            //dont bother with instant removal due to increasing load.
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.HealthComponent.TakeDamage -= HealthComponent_TakeDamage;
        }
        //borrowed method from ThinkInvis' ClassicItems BoxingGlove
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            // BLANK LOGIC //
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var characterBody = attacker.GetComponent<CharacterBody>();

                if (characterBody)
                {
                    if (damageInfo.inflictor)
                    {
                        if (damageInfo.inflictor == BlankObject)
                        {
                            var prcf = 300;
                            var mass = characterBody.characterMotor?.mass ?? (characterBody.rigidbody?.mass ?? 1f);

                            Vector3 ZeroYVector3(Vector3 vector3)
                            {
                                return new Vector3(vector3.x, 0, vector3.z);
                            }

                            if (damageInfo.force == Vector3.zero)
                                damageInfo.force += Vector3.Normalize(ZeroYVector3(damageInfo.position - characterBody.corePosition)) * prcf * mass;
                            else
                                damageInfo.force += Vector3.Normalize(ZeroYVector3(damageInfo.force)) * prcf * mass;
                        }
                    }
                }
            }
            // BLANK LOGIC //

        }

        public static bool FireBlank(CharacterBody attacker, Vector3 corePosition, float blankAttackRadius, float damageMult, float projectileClearRadius, bool consumeBlank = false, bool playSound = true, bool playEffect = true)
        {
            //var body = attacker.GetComponent<CharacterBody>();
            var blankAmount = attacker.inventory.GetItemCount(Blank.instance.catalogIndex);
            if (blankAmount == 0 && consumeBlank) //if needs blank and have no blank
            {
                _logger.LogMessage("Blank: A Blank item was required, but the player had no blank! Returning false.");

                return false;
            }
            if (playSound)
            {
                Util.PlayAttackSpeedSound(BlankSound, attacker.gameObject, 0.75f);
            }

            var teamIndex = attacker.teamComponent ? attacker.teamComponent.teamIndex : TeamIndex.Player;

            // EFFECT HERE
            if (playEffect)
            {
                var characterBody = attacker.GetComponent<CharacterBody>();
                var yOffset = 0f;
                if (characterBody && characterBody.characterMotor)
                    yOffset = characterBody.characterMotor.capsuleHeight / 2;
                EffectManager.SpawnEffect(BlankEffect, new EffectData
                {
                    origin = attacker.transform.position + Vector3.up * yOffset,
                    rotation = Util.QuaternionSafeLookRotation(Vector3.up),
                    scale = 40f
                }, false);
            }

            // BLAST ATTACK HERE //
            if (damageMult > 0)
            {
                new BlastAttack
                {
                    attacker = attacker.gameObject, //who
                    inflictor = BlankObject, //how
                    position = corePosition,
                    procCoefficient = 0f,
                    losType = BlastAttack.LoSType.None,
                    falloffModel = BlastAttack.FalloffModel.None,
                    baseDamage = attacker.damage * damageMult,
                    damageType = DamageType.Stun1s,
                    //crit = self.RollCrit(),
                    radius = blankAttackRadius,
                    teamIndex = teamIndex,
                    baseForce = 900,
                    //baseForce = 2000f,
                    //bonusForce = new Vector3(0, 1600, 0)
                }.Fire();
            }

            if (projectileClearRadius != 0)
            {
                bool noRange = (projectileClearRadius == -1);
                //if (projectileClearRadius == -1) { projectileClearRadius = 999; }
                float blankRadiusSquared = projectileClearRadius * projectileClearRadius;
                List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
                List<ProjectileController> list = new List<ProjectileController>();
                int i = 0;
                int count = instancesList.Count;
                while (i < count)
                {
                    ProjectileController projectileController = instancesList[i];
                    if (projectileController.teamFilter.teamIndex != teamIndex)
                    {
                        if (noRange)
                            list.Add(projectileController);
                        else if ((projectileController.transform.position - corePosition).sqrMagnitude < blankRadiusSquared)
                            list.Add(projectileController);
                    }
                    i++;
                }
                int j = 0;
                int count2 = list.Count;
                while (j < count2)
                {
                    ProjectileController projectileController2 = list[j];
                    if (projectileController2)
                    {
                        EffectManager.SpawnEffect(BlankEffect, new EffectData
                        {
                            origin = projectileController2.gameObject.transform.position,
                            rotation = Util.QuaternionSafeLookRotation(Vector3.up),
                            scale = 1f
                        }, false);
                        UnityEngine.Object.Destroy(projectileController2.gameObject);
                    }
                    j++;
                }
            }
            if (consumeBlank)
            {
                attacker.inventory.RemoveItem(Blank.instance.catalogIndex);
            }
            return true;
        }
    }
}
