//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;


namespace RiskOfBulletstorm.Items
{
    public class RingMiserlyProtection : Item_V2<RingMiserlyProtection> //switch to a buff system due to broken
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Health Increase. Default: +100%", AutoConfigFlags.PreventNetMismatch)]
        public float RingMiserlyProtection_HealthBonus { get; private set; } = 1.0f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Health Increase Stack. Default: +50%", AutoConfigFlags.PreventNetMismatch)]
        public float RingMiserlyProtection_HealthBonusStack { get; private set; } = 0.5f;
        public override string displayName => "Ring of Miserly Protection";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.Cleansable });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Aids The Fiscally Responsible\n<style=cHealth>Increases health substantially.</style> <style=cDeath>Any shrine purchases will shatter a ring.</style>";

        protected override string GetDescString(string langid = null) => $"Grants <style=cHealth>+{Pct(RingMiserlyProtection_HealthBonus)}</style> <style=cStack>(+{Pct(RingMiserlyProtection_HealthBonusStack)} per stack)</style> health." +
            $"\n <style=cDeath>...but breaks a ring completely upon using a shrine.</style> ";

        protected override string GetLoreString(string langID = null) => "Before the Shopkeep opened his shop, he was an avaricious and miserly man. He remains careful about any expenditures, but through capitalism he has purged himself of negative emotion.";

        private readonly GameObject ShatterEffect = Resources.Load<GameObject>("prefabs/effects/ShieldBreakEffect");

        public RingMiserlyProtection()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/RingMiserlyProtection.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/RingMiserlyProtectionIcon.png";
        }
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
            ShrineChanceBehavior.onShrineChancePurchaseGlobal += ShrineChanceBehavior_onShrineChancePurchaseGlobal;
            GetStatCoefficients += BoostHealth;
        }

        private void ShrineChanceBehavior_onShrineChancePurchaseGlobal(bool outcome, Interactor interactor)
        {
            if (outcome)
            {
                var body = interactor.gameObject.GetComponent<CharacterBody>();
                if (!body)
                {
                    var inventory = body.inventory;
                    if (inventory)
                    {
                        var InventoryCount = body.inventory.GetItemCount(catalogIndex);
                        if (InventoryCount > 0)
                        {
                            body.inventory.RemoveItem(catalogIndex);
                            Util.PlaySound("Play_char_glass_death", body.gameObject);
                            EffectManager.SimpleEffect(ShatterEffect, body.gameObject.transform.position, Quaternion.identity, true);
                            body.RecalculateStats();
                        }
                    }
                }
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            ShrineChanceBehavior.onShrineChancePurchaseGlobal -= ShrineChanceBehavior_onShrineChancePurchaseGlobal;
            GetStatCoefficients -= BoostHealth;
        }

        private void BoostHealth(CharacterBody sender, StatHookEventArgs args)
        {
            var invCount = GetCount(sender);
            if (invCount > 0) args.healthMultAdd += RingMiserlyProtection_HealthBonus + RingMiserlyProtection_HealthBonusStack * (invCount - 1);
        }
    }
}
