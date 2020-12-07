using RoR2;
using UnityEngine.Networking;
using TILER2;

namespace RiskOfBulletstorm.Items
{
    public class FriendshipCookie : Equipment_V2<FriendshipCookie>
    {
        public override string displayName => "Friendship Cookie";
        public override float cooldown { get; protected set; } = 0f;
        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "It's Delicious!\nRevives all players.";

        protected override string GetDescString(string langid = null) => $"Upon use, <style=cIsHealing>revives all players</style>." +
            $"\nSINGLEPLAYER: <style=cIsHealing>Gives an infusion instead.</style>" +
            $"\n<style=cDeath>Consumed on use.</style>";

        protected override string GetLoreString(string langID = null) => "Baked fresh every morning by Mom! It's to die for! Or, just maybe, to live for.";

        public FriendshipCookie()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/FriendshipCookie.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/FriendshipCookieIcon.png";
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            if (ClassicItemsCompat.enabled)
                ClassicItemsCompat.RegisterEmbryo(catalogIndex);
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

            var playerList = PlayerCharacterMasterController.instances;
            //var playerList = CharacterMaster.readOnlyInstancesList;
            int playerAmt = playerList.Count;

            bool EmbryoProc = ClassicItemsCompat.CheckEmbryoProc(instance, body) && ClassicItemsCompat.enabled;

            if (playerAmt > 1)
            {
                foreach (PlayerCharacterMasterController player in playerList)
                {
                    var master = player.master;
                    if (master.IsDeadAndOutOfLivesServer())
                    {
                        Stage.instance.RespawnCharacter(master);
                        if (EmbryoProc)
                        {
                            var healthComponent = master.GetBody()?.GetComponent<HealthComponent>();

                            if (healthComponent)
                            {
                                var fullBarrier = healthComponent.fullBarrier;
                                if (NetworkServer.active)
                                {
                                    healthComponent.AddBarrier(fullBarrier);
                                } else
                                {
                                    healthComponent.AddBarrierAuthority(fullBarrier);
                                }
                            }
                        }
                        revivedPlayers++;
                    }
                }
            }
            else if (playerAmt == 1) inventory.GiveItem(ItemIndex.Infusion, EmbryoProc ? 2 : 1);

            if (revivedPlayers > 0 || playerAmt == 1) //anyone revived or its singleplayer
            {
                inventory.SetEquipmentIndex(EquipmentIndex.None); //credit to : Rico
                return true;
            }
            return false;
        }
    }
}
