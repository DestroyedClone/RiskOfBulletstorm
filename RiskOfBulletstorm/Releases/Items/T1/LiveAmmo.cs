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
        public override string displayName => "Live Ammo";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "I'm A Bullet Too!\nReduced contact damage and explodes after Utility use.";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "Who needs bullets when you ARE a bullet?";

        private readonly BlastAttack blastAttack;
        private readonly GameObject LiveAmmoObject;

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
            //Vector3 corePos = Util.GetCorePosition(vBody);
            GameObject vGameObject = self.gameObject;

            if (vBody.skillLocator.FindSkill(self.skillName))
            {
                if (invCount > 0)
                {
                    if (self.characterBody.skillLocator.utility.Equals(self))
                    {
                        blastAttack.attacker = vGameObject;
                        blastAttack.baseDamage = vBody.baseDamage;
                        blastAttack.crit = vBody.RollCrit();
                        blastAttack.damageColorIndex = DamageColorIndex.Default;
                        blastAttack.inflictor = LiveAmmoObject;
                        blastAttack.teamIndex = vBody.teamComponent.teamIndex;
                        blastAttack.radius = 6f;
                        blastAttack.Fire();
                    }
                }
            }
            orig(self);
        }
    }
}
