using RoR2;
using UnityEngine.Networking;
using TILER2;
using UnityEngine;
namespace RiskOfBulletstorm.Items
{
    public class FriendshipCookie : Equipment_V2<FriendshipCookie>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the duration of immunity after respawning someone? (Default: 3.0 seconds)", AutoConfigFlags.None)]
        public float FriendshipCookie_BaseImmunityTime { get; private set; } = 3f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("[Classic Items] Beating Embryo: What is the duration of immunity if it procs? (Default: 9.0 seconds)", AutoConfigFlags.None)]
        public float FriendshipCookie_EmbryoImmunityTime { get; private set; } = 9f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the ItemIndex of the item to be given in singleplayer? (Default: ItemIndex.Infusion)", AutoConfigFlags.None)]
        public ItemIndex FriendshipCookie_ItemIndex { get; private set; } = ItemIndex.Infusion;

        public override string displayName => "Friendship Cookie";
        public override float cooldown { get; protected set; } = 0f;
        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "It's Delicious!\nRevives all players.";

        protected override string GetDescString(string langid = null)
        {
            var desc = $"Upon use, <style=cIsHealing>revives all players</style>." +
            $"\nSINGLEPLAYER: <style=cIsHealing>Gives ";
            var itemDef = ItemCatalog.GetItemDef(FriendshipCookie_ItemIndex);
            if (itemDef == null)
                desc += $"nothing";
            else
                desc += $"{itemDef.name}";
            desc += " instead.</style>" +
            $"\n<style=cDeath>Consumed on use.</style>";

            return desc;
        }

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
                        var respawnLength = FriendshipCookie_BaseImmunityTime;
                        if (EmbryoProc)
                            respawnLength = FriendshipCookie_EmbryoImmunityTime;

                        RespawnExtraLife(master, false, true, respawnLength, body.master);
                        //Stage.instance.RespawnCharacter(master);
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

        public void RespawnExtraLife(CharacterMaster master, bool GiveSpentDio = true, bool TryToGroundFootPosition = true, float BuffTimer = 3f, CharacterMaster summoner = null)
        {
            var RespawnPosition = master.deathFootPosition;
            if (GiveSpentDio) master.inventory.GiveItem(ItemIndex.ExtraLifeConsumed, 1);

            if (summoner)
                RespawnPosition = summoner.GetBody().footPosition;

            master.PlayExtraLifeSFX();
            master.Respawn(RespawnPosition, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f), TryToGroundFootPosition);
            master.GetBody().AddTimedBuff(BuffIndex.Immune, BuffTimer);
            GameObject gameObject = Resources.Load<GameObject>("Prefabs/Effects/HippoRezEffect");
            if (master.bodyInstanceObject)
            {
                foreach (EntityStateMachine entityStateMachine in master.bodyInstanceObject.GetComponents<EntityStateMachine>())
                {
                    entityStateMachine.initialStateType = entityStateMachine.mainStateType;
                }
                if (gameObject)
                {
                    EffectManager.SpawnEffect(gameObject, new EffectData
                    {
                        origin = master.deathFootPosition,
                        rotation = master.bodyInstanceObject.transform.rotation
                    }, true);
                }
            }
            master.ResetLifeStopwatch();
        }

    }
}
