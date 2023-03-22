using BepInEx.Configuration;
using RiskOfBulletstormRewrite.Items;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class BulletQuestController : ControllerBase<BulletQuestController>
    {
        /*  Primer: 110 casings on stage 2
         *      Special Purchase next to teleporter
        *   gunpowder: black powder mine
        *       Special Interaction in skies on stage 3 area
        *   planar lead: invis path in hollow
        *       
        *   shell casing: dragun
        */

        private static ItemDef PrimerDef => Items.PrimePrimer.instance.ItemDef;
        private static ItemDef GunpowderDef => Items.ArcaneGunpowder.instance.ItemDef;
        private static ItemDef LeadDef => Items.PlanarLead.instance.ItemDef;
        private static ItemDef CasingDef => Items.ObsidianShellCasing.instance.ItemDef;

        public int CurrentPrimerCost { get; set; }
        public static bool EnablePrimerPurchase = true;

        public override void Init(ConfigFile config)
        {
            return;
            CreateConfig(config);
            Hooks();
        }

        public override void Hooks()
        {
            base.Hooks();
            On.RoR2.BazaarController.OnStartServer += BazaarOnStartServer;
            On.RoR2.Chat.SendBroadcastChat_ChatMessageBase += Chat_SendBroadcastChat_ChatMessageBase;
            Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            On.RoR2.TeleporterInteraction.Start += TeleporterInteraction_Start;

        }

        private void TeleporterInteraction_Start(On.RoR2.TeleporterInteraction.orig_Start orig, TeleporterInteraction self)
        {
            orig(self);

        }

        private void Stage_onStageStartGlobal(Stage stage)
        {
            if (NetworkServer.active)
            {
                switch (Run.instance.stageClearCount)
                {
                    case 1:
                        EnablePrimerPurchase = true;
                        CurrentPrimerCost = Run.instance.GetDifficultyScaledCost(25) * 2;
                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage()
                        {
                            baseToken = "RISKOFBULLETSTORM_DIALOGUE_GUNQUEST_PURCHASE",
                            paramTokens = new string[] { PrimePrimer.instance.ItemDef.nameToken, CurrentPrimerCost.ToString() }
                        });
                        break;
                    case 2:

                        EnablePrimerPurchase = false;
                        break;
                    default:
                        EnablePrimerPurchase = false;
                        break;
                }
            }
        }

        private void Chat_SendBroadcastChat_ChatMessageBase(On.RoR2.Chat.orig_SendBroadcastChat_ChatMessageBase orig, ChatMessageBase message)
        {
            orig(message);
            if (!EnablePrimerPurchase) return;
            if (message is Chat.UserChatMessage chatMsg)
            {
                if (chatMsg.sender && chatMsg.text.ToLower() == "/buy") //sender is a networkuser gameobject
                {
                    chatMsg.sender.TryGetComponent<NetworkUser>(out NetworkUser networkUser);
                    PurchasePrimePrimer(networkUser.master);
                }
            }
        }

        public void PurchasePrimePrimer(CharacterMaster purchaserMaster)
        {
            if (purchaserMaster.money >= CurrentPrimerCost)
            {
                purchaserMaster.money -= (uint)CurrentPrimerCost;
                EnablePrimerPurchase = false;
                purchaserMaster.inventory.GiveItem(Items.PrimePrimer.instance.ItemDef);
            }
        }

        public void BazaarOnStartServer(On.RoR2.BazaarController.orig_OnStartServer orig, BazaarController self)
        {
            orig(self);
            foreach (var player in PlayerCharacterMasterController.instances)
            {
                if (CraftShell(player.master.GetBody(), self.seerStations[0].transform.position))
                {
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage()
                    {
                        baseToken = "Gunsmith: Here's your bullet."
                    });
                    break;
                }
            }
        }

        public override void CreateConfig(ConfigFile config)
        {

        }



        /// <summary>
        /// This method effectively creates the Bullet That Can Kill The Past for the player.
        /// </summary>
        /// <param name="steve">The characterbody of the person crafting.</param>
        /// <param name="position">The position where the bullet is dropped.</param>
        public bool CraftShell(CharacterBody steve, Vector3 position)
        {
            if (steve && steve.inventory)
            {
                bool hasItem(ItemDef itemDef)
                {
                    return steve.inventory.GetItemCount(itemDef) > 0;
                }

                if (hasItem(PrimerDef)
                && hasItem(GunpowderDef)
                && hasItem(LeadDef)
                && hasItem(CasingDef))
                {
                    steve.inventory.RemoveItem(PrimerDef);
                    steve.inventory.RemoveItem(GunpowderDef);
                    steve.inventory.RemoveItem(LeadDef);
                    steve.inventory.RemoveItem(CasingDef);
                    //steve.inventory.GiveItem(Items.PastKillingBullet.instance.ItemDef);
                    PickupDropletController.CreatePickupDroplet(
                        PickupCatalog.itemIndexToPickupIndex[(int)Items.PastKillingBullet.instance.ItemDef.itemIndex],
                        position,
                        Vector3.zero
                    );
                    return true;
                }
            }
            return false;
        }
    }
}