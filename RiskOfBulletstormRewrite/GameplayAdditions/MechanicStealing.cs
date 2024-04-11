using BepInEx.Configuration;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine;
using R2API;

namespace RiskOfBulletstormRewrite.GameplayAdditions
{
    internal static class MechanicStealing
    {
        public static ConfigEntry<bool> cfgCanStealFromNewt;
        public static ConfigEntry<bool> cfgNewtLeavesShop;

        public static GameObject StompEffect => EntityStates.NewtMonster.KickFromShop.chargeEffectPrefab;
        public static GameObject ItemTakenEffect = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>("RoR2/Base/JumpBoost/BoostJumpEffect.prefab").WaitForCompletion();

        public static void Init(ConfigFile config)
        {
            var category = "Gameplay Modifications";
            cfgCanStealFromNewt = config.Bind(category, "Steal From Bazaar", true, "If true, you can steal from the bazaar while cloaked. But failing to steal will close the shop for the rest of the run.");
            if (cfgCanStealFromNewt.Value)
            {
                //On.RoR2.ShopTerminalBehavior.Start += ShopTerminalBehavior_Start;
                SceneManager.sceneLoaded += SceneManager_sceneLoaded;
                SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
            }
            cfgNewtLeavesShop = config.Bind(category, "Enraged Newt Leaves Shop", true, "If true, then Newt will instead leave the shop and lock up remaining purchases instead of blocking the shop.");
            if (cfgNewtLeavesShop.Value)
            {
                On.EntityStates.NewtMonster.KickFromShop.FixedUpdate += KickFromShop_FixedUpdate;
            } 
        }
        private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (loadSceneMode == LoadSceneMode.Single && scene.name == "bazaar")
            {
                On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
                On.RoR2.PurchaseInteraction.CanBeAffordedByInteractor += PurchaseInteraction_CanBeAffordedByInteractor;
                On.RoR2.PurchaseInteraction.GetContextString += PurchaseInteraction_GetContextString;
                if (ShouldShopClose())
                {
                    PreventNewtSpawn();
                    GameObject tempObj = new GameObject("DelayLockDownShop");
                    tempObj.AddComponent<DelayLockDownShop>();
                }
            }
        }

        public class DelayLockDownShop : MonoBehaviour
        {
            public float stopwatch = 1;

            public void FixedUpdate()
            {
                stopwatch -= Time.fixedDeltaTime;
                if (stopwatch > 0) return;
                LockDownShop();
                enabled = false;
            }
        }

        private static void SceneManager_sceneUnloaded(Scene scene)
        {
            if (scene.name == "bazaar")
            {
                On.RoR2.PurchaseInteraction.OnInteractionBegin -= PurchaseInteraction_OnInteractionBegin;
                On.RoR2.PurchaseInteraction.CanBeAffordedByInteractor -= PurchaseInteraction_CanBeAffordedByInteractor;
                On.RoR2.PurchaseInteraction.GetContextString -= PurchaseInteraction_GetContextString;
            }
        }

        private static void KickFromShop_FixedUpdate(On.EntityStates.NewtMonster.KickFromShop.orig_FixedUpdate orig, EntityStates.NewtMonster.KickFromShop self)
        {
            orig(self);
            if (self.hasAttacked && SceneInfo.instance)
            {
                GameObject gameObject = SceneInfo.instance.transform.Find("KickOutOfShop").gameObject;
                if (gameObject)
                {
                    gameObject.gameObject.SetActive(false);
                    LockDownShop();
                    EffectManager.SimpleEffect(StompEffect, self.outer.commonComponents.characterBody.corePosition, Quaternion.identity, false);
                    if (NetworkServer.active)
                    {
                        self.outer.commonComponents.characterBody.AddBuff(RoR2Content.Buffs.Cloak);
                        self.outer.commonComponents.characterBody.master.TrueKill();
                    }
                }
            }
        }

        public static void ForceNewtToKickPlayersFromShop(CharacterBody newtBody)
        {
            if (newtBody && newtBody.healthComponent)
            {
                newtBody.healthComponent.health = 500;
                newtBody.AddBuff(RoR2Content.Buffs.Immune);
                //idk how else to force the kickout
            }
        }
        private static void SpawnState_OnEnter(On.EntityStates.NewtMonster.SpawnState.orig_OnEnter orig, EntityStates.NewtMonster.SpawnState self)
        {
            orig(self);
            if (NetworkServer.active)
                if (ShouldShopClose())
                {
                    ForceNewtToKickPlayersFromShop(self.outer.commonComponents.characterBody);
                }
        }
        public static bool ShouldShopClose()
        {
            return RoR2.Util.GetItemCountForTeam(TeamIndex.Player, Items.BannedFromBazaarTally.instance.ItemDef.itemIndex, false) > 0;
        }
        private static void PreventNewtSpawn()
        {
            GameObject.Find("HOLDER: Store/HOLDER: Store Platforms/ShopkeeperPosition/SpawnShopkeeperTrigger").SetActive(false);
        }
        private static void LockDownShop()
        {
            PreventNewtSpawn();
            var shop = GameObject.Find("HOLDER: Store/LunarShop/LunarTable/");
            foreach (var buy in shop.GetComponentsInChildren<PurchaseInteraction>())
            {
                EffectManager.SimpleEffect(ItemTakenEffect, buy.transform.position, Quaternion.Euler(Vector3.up), false);
                buy.transform.Find("Display").gameObject.SetActive(false);
                buy.enabled = false;
            }
            GameObject.Find("HOLDER: Store/LunarShop/LunarRecycler/").GetComponent<PurchaseInteraction>().enabled = false;
            var seershop = GameObject.Find("HOLDER: Store/SeerShop/");
            foreach (var item in seershop.GetComponentsInChildren<PurchaseInteraction>())
            {
                EffectManager.SimpleEffect(ItemTakenEffect, item.transform.position, Quaternion.Euler(Vector3.up), false);
                item.transform.Find("HologramPivot").gameObject.SetActive(false);
                item.enabled = false;
                item.transform.Find("Model/DisplayPivot/").gameObject.SetActive(false);
            }
            var soupShop = GameObject.Find("HOLDER: Store/CauldronShop/");
            foreach (var item in soupShop.GetComponentsInChildren<PurchaseInteraction>())
            {
                EffectManager.SimpleEffect(ItemTakenEffect, item.transform.position, Quaternion.Euler(Vector3.up), false);
                item.enabled = false;
                item.transform.Find("HologramPivot").gameObject.SetActive(false);
                item.transform.Find("mdlBazaarCauldron/PickupDisplay/").gameObject.SetActive(false);
            }
        }
        private static string PurchaseInteraction_GetContextString(On.RoR2.PurchaseInteraction.orig_GetContextString orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);
            if (InteractorCanSteal(activator))
            {
                return Language.GetString("RISKOFBULLETSTORM_COST_STEAL_FORMAT");
            }

            return original;
        }
        private static void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            var originalCost = self.Networkcost;
            if (self.GetComponent<ShopTerminalBehavior>()
                && activator.TryGetComponent(out CharacterBody characterBody)
                && characterBody.hasCloakBuff
                && characterBody.inventory)
            {
                var stolenItemCount = Items.StolenItemTally.instance.GetCount(characterBody);
                var rollchance = 100 / (stolenItemCount + 1);
                if (Util.CheckRoll(rollchance))
                {
                    self.Networkcost = 0;
                    characterBody.inventory.GiveItem(Items.CurseTally.instance.ItemDef);
                    characterBody.inventory.GiveItem(Items.StolenItemTally.instance.ItemDef);
                }
                else
                {
                    characterBody.inventory.GiveItem(Items.BannedFromBazaarTally.instance.ItemDef);
                    NewtSaySteal();
                    var shopkeeperBodyIndex = BodyCatalog.FindBodyIndex("ShopkeeperBody");
                    foreach (var body in CharacterBody.readOnlyInstancesList)
                    {
                        if (body.bodyIndex == shopkeeperBodyIndex)
                        {
                            ForceNewtToKickPlayersFromShop(body);
                            break;
                        }
                    }
                }
            }
            orig(self, activator);
            self.Networkcost = originalCost;
        }

        private static void NewtSaySteal()
        {
            //var sfxLocator = BazaarController.instance.shopkeeper.GetComponent<SfxLocator>();
            Chat.SendBroadcastChat(new Chat.NpcChatMessage
            {
                baseToken = "RISKOFBULLETSTORM_DIALOGUE_NEWT_STEALRESPONSE_" + UnityEngine.Random.RandomRangeInt(0, 4),
                formatStringToken = "RISKOFBULLETSTORM_DIALOGUE_NEWT_FORMAT",
                //sender = BazaarController.instance.shopkeeper,
                //sound = sfxLocator?.barkSound
            });
        }

        public static bool InteractorCanSteal(Interactor activator)
        {
            if (activator.TryGetComponent(out CharacterBody characterBody)
                && characterBody.hasCloakBuff)
                return true;
            return false;
        }

        private static bool PurchaseInteraction_CanBeAffordedByInteractor(On.RoR2.PurchaseInteraction.orig_CanBeAffordedByInteractor orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);

            if (self.GetComponent<ShopTerminalBehavior>()
                && InteractorCanSteal(activator))
            {
                original = true;
            }
            return original;
        }

    }
}
