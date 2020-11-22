//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using RoR2.Projectile;
using RiskOfBulletstorm.Items;

namespace RiskOfBulletstorm.Shared.Blanks
{
    public class MasterBlankItem : Item_V2<MasterBlankItem>
    {
        // Controls Blank logic because i don't know how to make this run without making it an item. what a newbie
        public override string displayName => "";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";
        public static GameObject BlankObject { get; private set; }

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
            //dont bother with instant removal due to increasing load
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
            var characterBody = damageInfo.attacker.GetComponent<CharacterBody>();

            if (characterBody)
            {
                if (damageInfo.inflictor)
                {
                    if (damageInfo.inflictor == BlankObject)
                    {
                        var prcf = 50;
                        var mass = characterBody.characterMotor?.mass ?? (characterBody.rigidbody?.mass ?? 1f);

                        Vector3 ZeroYVector3(Vector3 vector3)
                        {
                            return new Vector3(vector3.x, 0, vector3.z);
                        }

                        if (damageInfo.force == Vector3.zero)
                            damageInfo.force += Vector3.Normalize(ZeroYVector3(damageInfo.position - characterBody.corePosition)) * prcf * mass;
                        else
                            damageInfo.force += Vector3.Normalize(ZeroYVector3(damageInfo.force)) * prcf * mass;
                        Debug.Log("Blank Force Applied!", self);
                    }
                }
            }

            // BLANK LOGIC //

        }

        public static bool FireBlank(CharacterBody attacker, Vector3 corePosition, float blankRadius, float damageMult, float projectileClearRadius, bool consumeBlank = false)
        {
            //var body = attacker.GetComponent<CharacterBody>();
            var blankAmount = attacker.inventory.GetItemCount(Blank.instance.catalogIndex);
            if (blankAmount == 0 && consumeBlank) //if needs blank and have no blank
            {
                //Debug.LogError("[RiskOfBulletstorm]: Blank was required, but player had no blank!");
                return false;
            }
            Util.PlaySound(EntityStates.Engi.EngiWeapon.FireMines.throwMineSoundString, attacker.gameObject); //SOUND PLAY
            // EFFECT HERE
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
                radius = blankRadius,
                teamIndex = TeamIndex.Player,
                baseForce = 900,
                //baseForce = 2000f,
                //bonusForce = new Vector3(0, 1600, 0)
            }.Fire();

            if (projectileClearRadius != 0)
            {
                if (projectileClearRadius == -1) { projectileClearRadius = 999; }
                float blankRadiusSquared = projectileClearRadius * projectileClearRadius;
                List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
                List<ProjectileController> list = new List<ProjectileController>();
                int i = 0;
                int count = instancesList.Count;
                while (i < count)
                {
                    ProjectileController projectileController = instancesList[i];
                    if (projectileController.teamFilter.teamIndex != TeamIndex.Player && (projectileController.transform.position - corePosition).sqrMagnitude < blankRadiusSquared)
                    {
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
