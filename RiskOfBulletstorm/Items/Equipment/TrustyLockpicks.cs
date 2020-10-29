/*
using RoR2;
using UnityEngine;
using TILER2;
using static TILER2.MiscUtil;

namespace RiskofBulletstorm.Items
{
    public class TrustyLockpicks : Equipment_V2<TrustyLockpicks>
    {
        public override string displayName => "Skeleton Key";
        public string descText = "Chance to pick locks. Can only be used once per lock.";

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance to break (Default: 0.5 = 50%).", AutoConfigFlags.None, 0f, float.MaxValue)]
        public float unlockChance { get; private set; } = 0.5f;

        public override float cooldown { get; protected set; } = 2f;
        protected override string GetNameString(string langid = null) => displayName;
        protected override string GetPickupString(string langid = null) => "Who Needs Keys?\n"+descText;
        protected override string GetDescString(string langid = null) => "{Pct(unlockChance)} {descText}";
        protected override string GetLoreString(string langid = null) => "These lockpicks have never let the Pilot down, except for the many times they did.";

        public TrustyLockpicks() { }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            if (!slot.characterBody) return false;
            if (SceneCatalog.mostRecentSceneDef.baseSceneName == "bazaar") return false;
            var sphpos = slot.characterBody.transform.position;
            var sphrad = 3f;

            Collider[] sphits = Physics.OverlapSphere(sphpos, sphrad, LayerIndex.defaultLayer.mask, QueryTriggerInteraction.Collide);
            bool foundAny = false;
            foreach (Collider c in sphits)
            {
                var ent = EntityLocator.GetEntity(c.gameObject);
                if (!ent) continue;
                var cptChest = ent.GetComponent<ChestBehavior>();
                if (!cptChest) continue;
                var cptPurch = ent.GetComponent<PurchaseInteraction>();
                Util.CheckRoll(unlockChance, 0, slot);
                if (cptPurch && cptPurch.available && cptPurch.costType == CostTypeIndex.Money)
                {
                    cptPurch.SetAvailable(false);
                    cptChest.Open();
                    foundAny = true;
                }
            }
            return foundAny;
        }
    }
}*/