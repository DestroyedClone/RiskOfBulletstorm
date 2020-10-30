using System.Collections.ObjectModel;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
	public class RollBomb : Item_V2<RollBomb>
	{
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many damage should Roll Bomb deal? (Default: 0.8 = 80% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float RollBombDamage { get; private set; } = 0.8f;

        public override string displayName => "Roll Bomb";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Power Charge\nDrop bomb(s) after using your utility skill.";

        protected override string GetDescString(string langid = null) => $"Using your utility <style=cIsUtility>drops 1 (+1/stack) bombs</style> for <style=cIsDamage>{Pct(RollBombDamage)} damage </style>.";

        protected override string GetLoreString(string langID = null) => "Produces a bomb when dodge rolling.\nThis strange mechanism dispenses explosives when spun.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        //public static GameObject ItemBodyModelPrefab;
        public static GameObject BombPrefab { get; private set; }

        public override void SetupBehavior()
        {
            GameObject engiMinePrefab = Resources.Load<GameObject>("prefabs/projectiles/EngiGrenadeProjectile");
            BombPrefab = engiMinePrefab.InstantiateClone("RollBomb");
            BombPrefab.transform.localScale = new Vector3(3, 3, 3);
            BombPrefab.GetComponent<ProjectileSimple>().velocity = 0; //default 50
            //BombPrefab.GetComponent<ProjectileSimple>().lifetime = 4; //default 5
            BombPrefab.GetComponent<ProjectileDamage>().damageColorIndex = DamageColorIndex.Item;
            //BombPrefab.GetComponent<ProjectileImpactExplosion>().lifetime = 2;
            BombPrefab.GetComponent<ProjectileImpactExplosion>().destroyOnEnemy = false; //default True
            BombPrefab.GetComponent<ProjectileImpactExplosion>().timerAfterImpact = false;
            UnityEngine.Object.Destroy(BombPrefab.GetComponent<ApplyTorqueOnStart>());
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
            Vector3 corePos = Util.GetCorePosition(vBody);
            GameObject vGameObject = self.gameObject;

            if (invCount > 0)
            {
                if (self.characterBody.skillLocator.utility.Equals(self))
                {
                    for (int i = 0; i < invCount; i++)
                    {
                        ProjectileManager.instance.FireProjectile(BombPrefab, corePos, MineDropDirection(),
                                          vGameObject, vBody.damage * RollBombDamage,
                                          0f, Util.CheckRoll(vBody.crit, vBody.master),
                                          DamageColorIndex.Item, null, -1f);
                    }
                }
            }
            orig(self);
        }
        private Quaternion MineDropDirection()
        {
            return Util.QuaternionSafeLookRotation(
                new Vector3(0f, 0f, 0f)
            );
        }
    }
}