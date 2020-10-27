using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using EntityStates;
using EntityStates.Engi.EngiWeapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using static EntityStates.BaseState;

namespace RiskOfBulletstorm.Items
{
	public class RollBomb : Item_V2<RollBomb>
	{
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many damage should Roll Bomb deal? (Default: 0.8 = 80% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float RollBombDamage { get; private set; } = 0.8f;


        public override string displayName => "Roll Bomb";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Drop a bomb(s) after using your utility skill.";

        protected override string GetDescString(string langid = null) => $"Using your utility <style=cIsUtility>drops 1 (+1) bombs</style> for <style=cIsDamage>80% damage</style>.";

        protected override string GetLoreString(string langID = null) => "Power Charge\n\nProduces a bomb when dodge rolling.\nThis strange mechanism dispenses explosives when spun.";

        private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        //public static GameObject ItemBodyModelPrefab;
        public static GameObject bombPrefab { get; private set; }

        public override void SetupBehavior()
        {
            GameObject engiMinePrefab = Resources.Load<GameObject>("prefabs/projectiles/EngiGrenadeProjectile");
            bombPrefab = engiMinePrefab.InstantiateClone("RollBomb");
            UnityEngine.Object.Destroy(bombPrefab.GetComponent<ProjectileDeployToOwner>());
        }
        public override void Install()
        {
            base.Install();


        }
        public override void Uninstall()
        {
            base.Uninstall();

        }

        public GenericSkill activatorSkillSlot;
        public EntityStateMachine outer;
        protected SkillLocator skillLocator
        {
            get
            {
                return this.outer.commonComponents.skillLocator;
            }
        }
        protected InputBankTest inputBank
        {
            get
            {
                return this.outer.commonComponents.inputBank;
            }
        }
        public bool IsKeyDownAuthority()
        {
            if (skillLocator == null || this.activatorSkillSlot == null || inputBank == null)
            {
                return false;
            }
            switch (skillLocator.FindSkillSlot(this.activatorSkillSlot)) //TODO: Simplify
            {
                case SkillSlot.None:
                    return false;
                case SkillSlot.Primary:
                    //return inputBank.skill1.down;
                    return false;
                case SkillSlot.Secondary:
                    //return inputBank.skill2.down;
                    return false;
                case SkillSlot.Utility:
                    //return inputBank.skill3.down;
                    return true;
                case SkillSlot.Special:
                    //return inputBank.skill4.down;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        protected virtual bool KeyIsDown()
        {
            return IsKeyDownAuthority();
        }
        public void usedUtility()
        {
            //public GenericSkill activatorSkillSlot;
            if (this.KeyIsDown())//!skillLocator.FindSkillSlot(SkillSlot.Utility))
            {
                //do things
            }
        }
        public void boop()
        {
            On.EntityStates.Huntress.ArrowRain.OnEnter += (orig, self) =>
            {
                // [The code we want to run]
                Chat.AddMessage("You used Huntress's Arrow Rain!");
                // Call the original function (orig)
                // on the object it's normally called on (self)
                orig(self);
            };
        }
    }
}
