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

namespace RiskOfBulletstorm.Items
{
    public class LiveAmmo : Item_V2<LiveAmmo>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much damage should the explosion deal? (Default: 1.0 = +100% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float LiveAmmo_DamageDealt { get; private set; } = 1f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much damage should the explosion deal per item stack? (Default: 0.5 = +50% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float LiveAmmo_DamageDealtStack { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much force should the explosion propel? (Default: 50.0)", AutoConfigFlags.PreventNetMismatch)]
        public float LiveAmmo_ForceCoefficient { get; private set; } = 50f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much force should the explosion propel per item stack? (Default: 25)", AutoConfigFlags.PreventNetMismatch)]
        public float LiveAmmo_ForceCoefficientStack { get; private set; } = 25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius of the explosion? (Default: 8m)", AutoConfigFlags.PreventNetMismatch)]
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
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GenericSkill.OnExecute -= GenericSkill_OnExecute;
        }

        private void GenericSkill_OnExecute(On.RoR2.GenericSkill.orig_OnExecute orig, GenericSkill self)
        {
            var invCount = GetCount(self.characterBody);
            CharacterBody vBody = self.characterBody;
            if (!vBody || !vBody.skillLocator) return; //null check
            Vector3 corePos = Util.GetCorePosition(vBody);
            GameObject vGameObject = self.gameObject;

            if (vBody.skillLocator.FindSkill(self.skillName))
            {
                if (invCount > 0)
                {
                    if (self.characterBody.skillLocator.utility.Equals(self))
                    {
                        var blastAttack = new BlastAttack
                        {
                            attacker = vGameObject,
                            baseDamage = vBody.baseDamage * (LiveAmmo_DamageDealt + LiveAmmo_DamageDealtStack * (invCount - 1)),
                            crit = vBody.RollCrit(),
                            damageColorIndex = DamageColorIndex.Default,
                            teamIndex = vBody.teamComponent.teamIndex,
                            radius = LiveAmmo_Radius,
                            position = corePos
                        }.Fire();

                        if (vBody.inputBank)
                        {
                            vBody.characterMotor.velocity += vBody.inputBank.aimDirection * (LiveAmmo_ForceCoefficient + LiveAmmo_ForceCoefficientStack * (invCount - 1));
                        }
                    }
                }
            }
            orig(self);
        }
    }
}
