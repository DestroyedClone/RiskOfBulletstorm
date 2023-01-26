using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class BulletQuestController : ControllerBase<BulletQuestController>
    {
        /*  Primer: 110 casings on stage 2
        *   gunpowder: black powder mine
        *   planar lead: invis path in hollow
        *   shell casing: dragun
        */

        private static ItemDef PrimerDef => Items.PrimePrimer.instance.ItemDef;
        private static ItemDef GunpowderDef => Items.ArcaneGunpowder.instance.ItemDef;
        private static ItemDef LeadDef => Items.PlanarLead.instance.ItemDef;
        private static ItemDef CasingDef => Items.ObsidianShellCasing.instance.ItemDef;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            Hooks();
        }

        public override void Hooks()
        {
            base.Hooks();
            On.RoR2.BazaarController.OnStartServer += BazaarOnStartServer;
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