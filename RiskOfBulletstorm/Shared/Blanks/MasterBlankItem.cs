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
using static RiskOfBulletstorm.Shared.BlankRelated;

namespace RiskOfBulletstorm.Items
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

            if (!characterBody) return;
            if (!damageInfo.inflictor) return;
            if (!damageInfo.inflictor == blankObject) return;

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

            // BLANK LOGIC //

        }
    }
}
