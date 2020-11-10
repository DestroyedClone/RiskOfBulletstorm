using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;

namespace RiskOfBulletstorm.Items
{
    public class PlusBullets : Item_V2<PlusBullets>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many damage should [+1 Bullets] provide with a single stack? (Default: 0.25 = 25% dmg)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageBonus { get; private set; } = 0.25f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many additional damage should each [+1 Bullets] after the first give? (Default: 0.05 = 5% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageBonusStack { get; private set; } = 0.05f;

        public override string displayName => "+1 Bullets";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "+1 To Bullet.\nMasterwork bullets deal more damage.";

        protected override string GetDescString(string langid = null) => $"Increases damage by <style=cIsDamage>{Pct(DamageBonus)} damage</style>. <style=cIsDamage>+{Pct(DamageBonusStack)} damage</style> per stack.";

        protected override string GetLoreString(string langID = null) => "Masterwork bullets.\n\nPeer-reviewed studies have shown that these bullets are precisely 1 better than normal bullets.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject ItemBodyModelPrefab;

        public override void SetupBehavior()
        {

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
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
        }
        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            int InventoryCount = damageInfo.attacker.GetComponent<CharacterBody>().inventory.GetItemCount(catalogIndex);
            if (InventoryCount > 0)
            {
                float ResultDamageMult = 1 + DamageBonus + DamageBonusStack * (InventoryCount - 1);
                Debug.Log("+1 Bullets: "+damageInfo.damage.ToString() + " increased by " + ResultDamageMult.ToString() + "%", self);
                damageInfo.damage *= ResultDamageMult;
            }
            orig(self, damageInfo, victim);
        }
    }
}
