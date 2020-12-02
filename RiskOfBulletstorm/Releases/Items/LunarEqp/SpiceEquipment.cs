using EntityStates.Treebot.Weapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;
using ThinkInvisible.ClassicItems;
using RiskOfBulletstorm;
using static RiskOfBulletstorm.Utils.HelperUtil;
using GenericNotification = On.RoR2.UI.GenericNotification;

namespace RiskOfBulletstorm.Items
{
    public class Spice : Equipment_V2<Spice>
    {

        public override string displayName => "Spice";
        public override float cooldown { get; protected set; } = 0f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<style=cIsUtility>Increases combat prowess</style>, <style=cDeath>with absolutely no downside!</style>";

        protected override string GetDescString(string langid = null) => $"Progressively gives stat bonuses and downsides.";

        protected override string GetLoreString(string langID = null) => "A potent gun-enhancing drug from the far reaches of the galaxy. It is known to be extremely addictive, and extremely expensive.";

        public ItemIndex SpiceTally { get; private set; }

        public string[] SpiceDescArray =
        {
            "A tantalizing cube of power.",
            "One more couldn't hurt.",
            "Just one more hit...",
            "MORE"
        };

        public Spice()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Spice.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/Spice.png";
        }
        public override void SetupBehavior()
        {
            base.SetupBehavior();
            Embryo_V2.instance.Compat_Register(catalogIndex);
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            var spiceTallyDef = new CustomItem(new ItemDef
            {
                hidden = true,
                name = "InternalSpiceTally",
                tier = ItemTier.NoTier,
                canRemove = false
            }, new ItemDisplayRuleDict(null));
            SpiceTally = ItemAPI.Add(spiceTallyDef);
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.PickupDropletController.CreatePickupDroplet += PickupDropletController_CreatePickupDroplet;
            GenericNotification.SetItem += SetNotificationItemHook;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PickupDropletController.CreatePickupDroplet -= PickupDropletController_CreatePickupDroplet;
            GenericNotification.SetItem -= SetNotificationItemHook;
        }
        private void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            //var body = PlayerCharacterMasterController.instances[0].master.GetBody();
            var characterBody = GetPlayerWithMostItemIndex(SpiceTally);
            var SpiceReplaceChance = characterBody.inventory.GetItemCount(SpiceTally);
            if (Util.CheckRoll(SpiceReplaceChance))
            {
                if (pickupIndex != PickupCatalog.FindPickupIndex(ItemIndex.ArtifactKey)) //safety to prevent softlocks
                    pickupIndex = PickupCatalog.FindPickupIndex(catalogIndex);
            }
            orig(pickupIndex, position, velocity);
        }

        private void SetNotificationItemHook(GenericNotification.orig_SetItem orig, RoR2.UI.GenericNotification self, ItemDef itemDef)
        {
            //self.descriptionText.token = itemDef.descriptionToken;
            orig(self, itemDef);
            if (equipmentDef.equipmentIndex == catalogIndex)
            {
                var LocalUserList = LocalUserManager.readOnlyLocalUsersList;
                var localUser = LocalUserList[0];
                var inventoryCount = localUser.cachedBody.inventory.GetItemCount(SpiceTally);
                var index = Mathf.Max(inventoryCount, SpiceDescArray.Length - 1);
                self.descriptionText.token = SpiceDescArray[index];
            }
        }

        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;
            Inventory inventory = body.inventory;
            if (!inventory) return false;
            //var spiceCount = inventory.GetItemCount(SpiceTally);

            inventory.GiveItem(SpiceTally);
            inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico

            return false;
        }
    }
}
