using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using ThinkInvisible.ClassicItems;
using static RiskOfBulletstorm.Utils.HelperUtil;
using GenericNotification = On.RoR2.UI.GenericNotification;

namespace RiskOfBulletstorm.Items
{
    public class Spice : Equipment_V2<Spice>
    {
        public override string displayName => "Spice";
        public override float cooldown { get; protected set; } = 0f;
        public override bool isEnigmaCompatible => false;
        public override bool isLunar => true;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<style=cIsUtility>Increases combat prowess</style>, <style=cDeath>with absolutely no downside!</style>";

        protected override string GetDescString(string langid = null) => $"Progressively gives stat bonuses and downsides. Consumed on use.";

        protected override string GetLoreString(string langID = null) => "A potent gun-enhancing drug from the far reaches of the galaxy. It is known to be extremely addictive, and extremely expensive.";

        public static ItemIndex SpiceTally { get; private set; }
        public static ItemIndex CurseIndex;

        public string[] SpiceDescArray =
        {
            "A tantalizing cube of power.",
            "One more couldn't hurt.",
            "Just one more hit...",
            "MORE"
        };

        /*
        //health, attack speed, shot accuracy, enemy bullet speed, damage
        public static readonly float[,] SpiceBonusesAdditive = new float[,]
        {
            { 0f,   0f,      0f,    0f,     0f },
            { +1f,  +0.2f,  +0.25f, 0f,     0f },
            { +1f,  +0.2f,  0f,     -0.1f,  0f }, 
            { -1f,  0f,     0f,     -0.05f, +0.2f }, 
            { -1f,  0f,     -0.1f,  0f,     +0.15f }, 
        };
        */ //documentation purposes
        public static readonly float[] SpiceBonusesAdditive = new float[] { -0.05f, 0f, -0.1f, 0f, +0.15f };

        public static readonly float[,] SpiceBonuses = new float[,]
        {
            { 0f, 0f, 0f, 0f, 0f }, //0
            { +0.25f, +0.2f, +0.25f, 0f, 0f }, //1
            { +0.50f, +0.4f, +0.25f, -0.1f, 0f }, //2
            { +0.25f, +0.4f, +0.25f, -0.15f, +0.2f }, //3
            { 0f, +0.4f, +0.15f, -0.15f, +0.35f }, //4
        };

        public static readonly float[] SpiceBonusesConstantMaxed = new float[] {0f, 0.4f, 0f, -0.15f, 0f };

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
                name = "ROBInternalSpiceTally",
                tier = ItemTier.NoTier,
                canRemove = false
            }, new ItemDisplayRuleDict(null));
            SpiceTally = ItemAPI.Add(spiceTallyDef);
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }

        public override void SetupLate()
        {
            base.SetupLate();
            CurseIndex = CurseController.curseTally;
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.PickupDropletController.CreatePickupDroplet += PickupDropletController_CreatePickupDroplet;
            GenericNotification.SetEquipment += GenericNotification_SetEquipment;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.PickupDropletController.CreatePickupDroplet -= PickupDropletController_CreatePickupDroplet;
            GenericNotification.SetEquipment -= GenericNotification_SetEquipment;
        }
        private void GenericNotification_SetEquipment(GenericNotification.orig_SetEquipment orig, RoR2.UI.GenericNotification self, EquipmentDef equipmentDef)
        {
            orig(self, equipmentDef);
            if (equipmentDef.equipmentIndex == catalogIndex)
            {
                var LocalUserList = LocalUserManager.readOnlyLocalUsersList;
                var localUser = LocalUserList[0];
                var inventoryCount = localUser.cachedBody.inventory.GetItemCount(SpiceTally);
                var index = Mathf.Max(inventoryCount, SpiceDescArray.Length - 1);
                self.descriptionText.token = SpiceDescArray[index];
            }
        }

        private void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            //var body = PlayerCharacterMasterController.instances[0].master.GetBody();
            var characterBody = GetPlayerWithMostItemIndex(SpiceTally);
            if (characterBody != null)
            {
                var SpiceReplaceChance = characterBody.inventory.GetItemCount(SpiceTally);
                if (Util.CheckRoll(SpiceReplaceChance))
                {
                    if (pickupIndex != PickupCatalog.FindPickupIndex(ItemIndex.ArtifactKey)) //safety to prevent softlocks
                        pickupIndex = PickupCatalog.FindPickupIndex(catalogIndex);
                }
            }
            orig(pickupIndex, position, velocity);
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

            if (inventory.GetItemCount(SpiceTally) == 0) inventory.GiveItem(CurseIndex);
            else inventory.GiveItem(CurseIndex, 2);
            inventory.GiveItem(SpiceTally);
            inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico

            return false;
        }
    }
}
