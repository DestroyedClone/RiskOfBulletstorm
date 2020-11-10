/*
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
using EntityStates.Engi.EngiWeapon;

namespace RiskOfBulletstorm.Items
{
    public class BattleStandard : Item_V2<BattleStandard> //Change to equipment that gives cursed.
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage Bonus of companions (Default: +20%)?", AutoConfigFlags.PreventNetMismatch)]
        public float CompanionDmgBonus { get; private set; } = 0.2f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage Bonus of companions per stack (Default: +10%)?", AutoConfigFlags.PreventNetMismatch)]
        public float CompanionDmgBonusStack { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("[unused] Charm duration on enemies per stack (Default: +0.5 seconds?", AutoConfigFlags.PreventNetMismatch)]
        public float CharmDurationBonus { get; private set; } = 0.5f;

        public override string displayName => "Battle Standard";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Set Your Own!" +
            "\nImproves the effectiveness of companions and charmed enemies.";

        protected override string GetDescString(string langid = null) => $"Increases damage of companions by {Pct(CompanionDmgBonus)} + {Pct(CompanionDmgBonusStack)} per stack." +
            $"\nIncreases charm duration by {CharmDurationBonus} seconds per stack.";

        protected override string GetLoreString(string langID = null) => "Before the advancement of Gundead society, belligerent bands of Bullet Kin would rally around these banners before their raids upon the upper chambers.";

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
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody obj) //Borrowed from Chen's Gradius Mod
        {
            // This hook runs on Client and on Server
            CharacterMaster master = obj.master;
            if (master && master.minionOwnership)
            {
                CharacterMaster masterMaster = master.minionOwnership.ownerMaster;
                if (!masterMaster) Debug.LogError("Master: "+masterMaster.ToString());
                if (GetCount(masterMaster) <= 0) Debug.LogError("masterMaster Amt: " + masterMaster.ToString());


                if (masterMaster && GetCount(masterMaster) > 0)
                {
                    int InventoryCount = masterMaster.inventory.GetItemCount(catalogIndex);
                    //BlankComponent BlankComponent = master.GetComponent<BlankComponent>();
                    //if (!BlankComponent) { BlankComponent = master.gameObject.AddComponent<BlankComponent>(); }
                    int multiplier = (int)((CompanionDmgBonus + CompanionDmgBonusStack * (InventoryCount - 1))*10);
                    if (InventoryCount > 0)
                    {
                        master.inventory.GiveItem(ItemIndex.BoostDamage, multiplier);
                        Debug.Log("BoostDamage amount given: "+ multiplier.ToString(), obj);
                    } else
                    {
                        Debug.LogError("BattleStandard: No item!");
                    }
                }
            }
        }
    }
}
*/