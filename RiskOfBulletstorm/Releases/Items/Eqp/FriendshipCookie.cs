using EntityStates.Treebot.Weapon;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;
using ThinkInvisible.ClassicItems;

namespace RiskOfBulletstorm.Items
{
    public class FriendshipCookie : Equipment_V2<FriendshipCookie>
    {
        public override string displayName => "Friendship Cookie";
        public override float cooldown { get; protected set; } = 0f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "It's Delicious!\nBaked fresh every morning by Mom! It's to die for! Or, just maybe, to live for.";

        protected override string GetDescString(string langid = null) => $"Upon use, <style=cIsHealing>revives all players</style>.";

        protected override string GetLoreString(string langID = null) => "Baked fresh every morning by Mom! It's to die for! Or, just maybe, to live for.";


        public override void SetupBehavior()
        {
            base.SetupBehavior();
            Embryo_V2.instance.Compat_Register(catalogIndex);
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
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;
            HealthComponent health = body.healthComponent;
            if (!health) return false;
            Inventory inventory = body.inventory;
            if (!inventory) return false;
            int revivedPlayers = 0;

            for (int i = 0; i < CharacterMaster.readOnlyInstancesList.Count; i++)
            {
                var player = CharacterMaster.readOnlyInstancesList[i];
                if (player.IsDeadAndOutOfLivesServer())
                {
                    Stage.instance.RespawnCharacter(player);
                    revivedPlayers++;
                }
            }
            if (revivedPlayers > 0)
            {
                inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico
                return true;
            }
            return false;
        }
    }
}
