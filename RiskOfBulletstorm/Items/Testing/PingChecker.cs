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
using R2API.Networking;

//Used https://github.com/DaKo51/High-Priority-Item/blob/master/HighPriorityItem.cs for reference

namespace RiskOfBulletstorm.Items
{
    public class PingChecker : Item_V2<PingChecker>
    {
        public override string displayName => "PingChecker";
        public override ItemTier itemTier => ItemTier.Lunar;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "CREDIT TO DaKo51";

        protected override string GetDescString(string langid = null) => $"CREDIT TO DaKo51";

        protected override string GetLoreString(string langID = null) => "CREDIT TO DaKo51";

        private GameObject currentEnemy;

        private PingerController[] pingerController;

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
            On.RoR2.Networking.GameNetworkManager.OnServerDisconnect += GameNetworkManager_OnServerDisconnect;
            On.RoR2.PlayerCharacterMasterController.OnBodyStart += PlayerCharacterMasterController_OnBodyStart;
        }

        private void PlayerCharacterMasterController_OnBodyStart(On.RoR2.PlayerCharacterMasterController.orig_OnBodyStart orig, PlayerCharacterMasterController self)
        {
            orig(self);
            FetchPingControllers(out pingerController);
        }

        private void GameNetworkManager_OnServerDisconnect(On.RoR2.Networking.GameNetworkManager.orig_OnServerDisconnect orig, RoR2.Networking.GameNetworkManager self, NetworkConnection conn)
        {
            orig(self, conn);
            FetchPingControllers(out pingerController);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
            On.RoR2.Networking.GameNetworkManager.OnServerDisconnect -= GameNetworkManager_OnServerDisconnect;
            On.RoR2.PlayerCharacterMasterController.OnBodyStart -= PlayerCharacterMasterController_OnBodyStart;
        }
        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);

            CharacterBody body = damageInfo.attacker.GetComponent<CharacterBody>();
            var InventoryCount = body.inventory.GetItemCount(catalogIndex);

            //Check if item is in Inventory
            if (body.inventory)
            {
                if (InventoryCount <= 0)
                    return;

                if (pingerController.Length <= 0)
                    if (!FetchPingControllers(out pingerController))
                        return;
            }


            string cutText(float value) //truncating method for Scouter text specifically
            {
                var text = value.ToString();
                int maxText = Math.Min(text.Length, InventoryCount);
                var allowedText = text.Substring(Math.Max(0, text.Length - maxText));
                string blockedText = new String('?', text.Length - maxText);
                return blockedText + allowedText;
            }

            CharacterBody component = victim.gameObject.GetComponent<CharacterBody>();
            string EnemyName = component.name;
            EnemyName = EnemyName.Substring(0, Math.Max(0, EnemyName.Length - 8));
            var EnemyHealthMax = component.maxHealth;
            var EnemyHealth = (int)component.healthComponent.health;
            var EnemyShieldMax = component.maxShield;
            var EnemyShield = (int)component.healthComponent.shield;
            var DamageType = damageInfo.damageType.ToString();
            var BaseDamage = damageInfo.damage; //ONLY SHOWS BASE DAMAGE DEALT
            orig(self, damageInfo, victim);
            //var Damage = (int)(EnemyHealth - component.healthComponent.health);
            if (InventoryCount < 2) { DamageType = "???"; }
            switch (InventoryCount)
            {
                case 0:
                    break;
            }
            var ScouterMsg = "\n==||||||" + EnemyName.ToString().ToUpper() + "||||||==" +
                "\n <color=#e32051>FleshHP</color>: " + cutText(EnemyHealth) + " / " + cutText(EnemyHealthMax) +
                "\n <color=#2095e3>ShieldHP</color>:" + cutText(EnemyShield) + " / " + cutText(EnemyShieldMax) +
                "\n Damage Received" + cutText(BaseDamage) + " (" + DamageType + ")" +
                "\n||||||SCOUTER||||||";

            // Iterate through every player available and check their pings
            foreach (PingerController pingerControllers in pingerController)
            {
                GameObject selectedEnemy = pingerControllers.currentPing.targetGameObject;
                currentEnemy = (victim.Equals(selectedEnemy) ? selectedEnemy : null);

                if (currentEnemy)
                {
                    Chat.AddMessage(ScouterMsg);
                }
            }
        }
        private bool FetchPingControllers(out PingerController[] pingerControllers)
        {
            if (NetworkUser.readOnlyInstancesList.Count <= 0)
            {
                pingerControllers = null;
                return false;
            }

            List<PingerController> listPingerControllers = new List<PingerController>();

            foreach (NetworkUser nu in NetworkUser.readOnlyInstancesList)
            {
                if (nu.masterController)
                {
                    PingerController foundPc = nu.masterController.GetComponent<PingerController>();

                    if (foundPc)
                        listPingerControllers.Add(foundPc);
                }
            }

            if (listPingerControllers.Count <= 0)
            {
                pingerControllers = null;
                return false;
            }

            pingerControllers = listPingerControllers.ToArray();
            return true;
        }
    }
}
