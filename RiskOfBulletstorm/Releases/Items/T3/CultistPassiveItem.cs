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
    public class CultistPassiveItem : Item_V2<CultistPassiveItem>
    {
        public override string displayName => "Number 2";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage, ItemTag.Utility, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Sidekick No More\nBoosts stats when alone.";

        protected override string GetDescString(string langid = null) => $"Increases <style=cIsUtility>base movespeed by 3</style>, and <style=cIsDamage>base damage by 3</style> for every dead survivor.";

        protected override string GetLoreString(string langID = null) => "Now that the protagonist is dead, it's time to shine!";
        public CultistPassiveItem()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/CultistPassiveItem.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/CultistPassiveIcon.png";
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
            GetStatCoefficients += CultistPassiveItem_GetStatCoefficients;
            On.RoR2.Stage.RespawnCharacter += Stage_RespawnCharacter;
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath += GlobalEventManager_OnPlayerCharacterDeath;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            int InventoryCount = GetCount(self);
            CultistPassiveComponent passiveComponent = self.gameObject.GetComponent<CultistPassiveComponent>();
            if (InventoryCount > 0)
            {
                if (!passiveComponent) { passiveComponent = self.gameObject.AddComponent<CultistPassiveComponent>(); }
                UpdateComponentForEveryone();
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients -= CultistPassiveItem_GetStatCoefficients;
            On.RoR2.Stage.RespawnCharacter -= Stage_RespawnCharacter;
            On.RoR2.GlobalEventManager.OnPlayerCharacterDeath -= GlobalEventManager_OnPlayerCharacterDeath;
            On.RoR2.CharacterBody.OnInventoryChanged -= CharacterBody_OnInventoryChanged;
        }

        private void CultistPassiveItem_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            var component = sender.GetComponent<CultistPassiveComponent>();
            if (component)
            {
                var deadAmt = component.deadProtagonists;
                args.baseDamageAdd += 3 * deadAmt;
                args.baseMoveSpeedAdd += 3 * deadAmt;
            }
        }

        private void Stage_RespawnCharacter(On.RoR2.Stage.orig_RespawnCharacter orig, Stage self, CharacterMaster characterMaster)
        {
            orig(self, characterMaster);
            UpdateComponentForEveryone();
        }

        private void GlobalEventManager_OnPlayerCharacterDeath(On.RoR2.GlobalEventManager.orig_OnPlayerCharacterDeath orig, GlobalEventManager self, DamageReport damageReport, NetworkUser victimNetworkUser)
        {
            orig(self, damageReport, victimNetworkUser);
            UpdateComponentForEveryone();
        }

        private int GetDeadAmount()
        {
            int deadAmt = 0;
            foreach (CharacterMaster player in CharacterMaster.readOnlyInstancesList)
            {
                if (player.IsDeadAndOutOfLivesServer())
                {
                    deadAmt++;
                }
            }
            return deadAmt;
        }

        private void UpdateComponentForEveryone()
        {
            int AmountDead = GetDeadAmount();
            if (AmountDead > 0)
            {
                foreach (var player in CharacterMaster.readOnlyInstancesList)
                {
                    CultistPassiveComponent passiveComponent = player.GetComponent<CultistPassiveComponent>();
                    if (passiveComponent)
                    {
                        passiveComponent.deadProtagonists = AmountDead;
                    }
                }
            }
        }

        public class CultistPassiveComponent : MonoBehaviour
        {
            public int deadProtagonists = 0;
        }
    }
}
