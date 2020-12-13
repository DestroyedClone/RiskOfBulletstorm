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
using EntityStates.Mage.Weapon;

namespace RiskOfBulletstorm.Items
{
    public class LiveAmmo : Item_V2<LiveAmmo>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much damage should the explosion deal? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float LiveAmmo_DamageDealt { get; private set; } = 1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much damage should the explosion deal per item stack? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float LiveAmmo_DamageDealtStack { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much force should the explosion propel?", AutoConfigFlags.PreventNetMismatch)]
        public float LiveAmmo_ForceCoefficient { get; private set; } = 50f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much force should the explosion propel per item stack?", AutoConfigFlags.PreventNetMismatch)]
        public float LiveAmmo_ForceCoefficientStack { get; private set; } = 25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius of the explosion in meters?", AutoConfigFlags.PreventNetMismatch)]
        public float LiveAmmo_Radius { get; private set; } = 8f;
        public override string displayName => "Live Ammo";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "I'm A Bullet Too!\nCreates an explosion that launches you on utility use.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Using your utility <style=cIsDamage>explodes</style> with a radius of <style=cIsUtility>{LiveAmmo_Radius}</style> for <style=cIsDamage>{Pct(LiveAmmo_DamageDealt)} damage </style>" +
                $"<style=cStack>(+{Pct(LiveAmmo_DamageDealtStack)} damage per stack </style> that propels you towards your aim with {Pct(LiveAmmo_ForceCoefficient)} force " +
                $"<style=cStack>(+{Pct(LiveAmmo_ForceCoefficientStack)} force per stack</style>.";

            return desc;
        }

        protected override string GetLoreString(string langID = null) => "Who needs bullets when can BECOME a bullet?";


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
            On.RoR2.GenericSkill.OnExecute += GenericSkill_OnExecute;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_GameObject_Vector3_Quaternion_GameObject_float_float_bool_DamageColorIndex_GameObject_float += Longassname;
        }

        private void Longassname(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_GameObject_Vector3_Quaternion_GameObject_float_float_bool_DamageColorIndex_GameObject_float orig, RoR2.Projectile.ProjectileManager self, GameObject prefab, Vector3 position, Quaternion rotation, GameObject owner, float damage, float force, bool crit, DamageColorIndex damageColorIndex, GameObject target, float speedOverride)
        {
            var characterBody = owner.GetComponent<CharacterBody>();
            if (prefab.name == PrepWall.projectilePrefab.name && characterBody)
            {
                FireAmmo(characterBody, true);
            }
            orig(self, prefab, position, rotation, owner, damage, force, crit, damageColorIndex, target, speedOverride);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GenericSkill.OnExecute -= GenericSkill_OnExecute;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_GameObject_Vector3_Quaternion_GameObject_float_float_bool_DamageColorIndex_GameObject_float -= Longassname;
        }

        private void FireAmmo(CharacterBody characterBody, bool halve = false)
        {
            var invCount = characterBody.inventory.GetItemCount(catalogIndex);
            Vector3 corePos = Util.GetCorePosition(characterBody);
            //var prcf = 15;
            //var mass = characterBody.characterMotor?.mass ?? (characterBody.rigidbody?.mass ?? 1f);

            new BlastAttack
            {
                attacker = characterBody.gameObject,
                baseDamage = characterBody.baseDamage * (LiveAmmo_DamageDealt + LiveAmmo_DamageDealtStack * (invCount - 1)) * (halve ? 0.45f : 1f),
                crit = characterBody.RollCrit(),
                damageColorIndex = DamageColorIndex.Default,
                teamIndex = characterBody.teamComponent.teamIndex,
                radius = LiveAmmo_Radius,
                position = corePos,
                procCoefficient = 0f
            }.Fire();
            /*
            Vector3 ZeroYVector3(Vector3 vector3)
            {
                return new Vector3(vector3.x, 0, vector3.z);
            }*/

            if (characterBody.inputBank)
            {
                var vector = characterBody.inputBank.aimDirection * (LiveAmmo_ForceCoefficient + LiveAmmo_ForceCoefficientStack * (invCount - 1));
                vector *= halve ? 0.35f : 1f;
                //var vector2 = Vector3.Normalize(ZeroYVector3(vector));
                characterBody.characterMotor.velocity += vector;
            }
        }

        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            CharacterBody vBody = self.characterBody;
            if (!vBody || !vBody.skillLocator) return; //null check
            var invCount = GetCount(self.characterBody);

            if (invCount > 0)
            {
                if (self.characterBody.baseNameToken != "MAGE_BODY_NAME")
                {
                    if (self.characterBody.skillLocator.utility && vBody.skillLocator.FindSkill(self.skillName))
                    {
                        if (self.characterBody.skillLocator.utility.Equals(self))
                        {
                            FireAmmo(vBody);
                        }
                    }
                }
            }
            orig(self);
        }
    }
}
