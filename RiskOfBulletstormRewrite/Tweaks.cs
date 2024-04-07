using BepInEx.Configuration;
using RoR2;
using RoR2.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RiskOfBulletstormRewrite
{
    public static class Tweaks
    {
        public static ConfigEntry<bool> cfgCenterNotifications;

        //public static ConfigEntry<NotificationMod> cfgEnableBreachNotifications;
        //public static ConfigEntry<bool> cfgDropEquipment;
        public static ConfigEntry<bool> cfgCanStealFromNewt;

        //public static ConfigEntry<bool> cfgDropEquipmentFromInaccesibleSlots;

        public enum NotificationMod
        {
            disable,
            bulletstorm,
            all
        }

        public const string category = "Tweaks";

        public static void Init(ConfigFile config)
        {
            cfgCenterNotifications = config.Bind(category, "Center Notification Text", false, "If true, then notification text will be centered.");
            //cfgDisableAutoPickup = config.Bind(category, "Disable Auto Pickups", false);
            //cfgEnableBreachNotifications = config.Bind(category, "Modify Achievement Notificaton", NotificationMod.bulletstorm, "");
            //cfgDropEquipment = config.Bind(category, "Drop Equipment", true, "If true, you can drop your equipment by holding [interact] and using your equipment.");
            if (cfgCenterNotifications.Value)
            {
                On.RoR2.CharacterMasterNotificationQueue.PushNotification += CharacterMasterNotificationQueue_PushNotification;
                On.RoR2.UI.GenericNotification.SetArtifact += SetArtifact;
                On.RoR2.UI.GenericNotification.SetEquipment += SetEquipment;
                On.RoR2.UI.GenericNotification.SetItem += SetItem;
            }
            On.EntityStates.NewtMonster.SpawnState.OnEnter += SpawnState_OnEnter;
            cfgCanStealFromNewt = config.Bind(category, "Steal From Bazaar", true, "If true, you can steal from the bazaar while cloaked. But failing to steal will close the shop for the rest of the run.");
            if (cfgCanStealFromNewt.Value)
            {
                //On.RoR2.ShopTerminalBehavior.Start += ShopTerminalBehavior_Start;
                SceneManager.sceneLoaded += SceneManager_sceneLoaded;
                SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
            }
            //cfgDropEquipmentFromInaccesibleSlots = config.Bind(category, "Drop Inaccessible Equipment Slots", true, "If true, then any items that are in equipment slots that are inaccessible will be dropped.");
            //switch (cfgEnableBreachNotifications.Value)
            //{
            //    case NotificationMod.disable:
            //        break;
            //    case NotificationMod.bulletstorm:
            //        On.RoR2.UI.AchievementNotificationPanel.SetAchievementDef += AchievementNotificationPanel_SetAchievementDef;
            //        break;
            //    case NotificationMod.all:
            //        break;
            //}

            //if (cfgDropEquipment.Value)
            //On.RoR2.UI.EquipmentIcon.Update += EquipmentIcon_Update;
        }

        public static void Bazaar_KickFromShop(CharacterBody newtBody)
        {
            if (newtBody && newtBody.healthComponent)
            {
                newtBody.healthComponent.health = 500;
                newtBody.healthComponent.godMode = true;
                //idk how else to force the kickout
            }
        }

        private static void SpawnState_OnEnter(On.EntityStates.NewtMonster.SpawnState.orig_OnEnter orig, EntityStates.NewtMonster.SpawnState self)
        {
            orig(self);
            if (NetworkServer.active)
                if (RoR2.Util.GetItemCountForTeam(TeamIndex.Player, Items.BannedFromBazaarTally.instance.ItemDef.itemIndex, false) > 0)
                {
                    Bazaar_KickFromShop(self.outer.commonComponents.characterBody);
                }
        }

        private static void SceneManager_sceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (loadSceneMode == LoadSceneMode.Single && scene.name == "bazaar")
            {
                On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
                On.RoR2.PurchaseInteraction.CanBeAffordedByInteractor += PurchaseInteraction_CanBeAffordedByInteractor;
            }
        }

        private static void SceneManager_sceneUnloaded(Scene scene)
        {
            if (scene.name == "bazaar")
            {
                On.RoR2.PurchaseInteraction.OnInteractionBegin -= PurchaseInteraction_OnInteractionBegin;
                On.RoR2.PurchaseInteraction.CanBeAffordedByInteractor -= PurchaseInteraction_CanBeAffordedByInteractor;
            }
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
                            Bazaar_KickFromShop(body);
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

        private static bool PurchaseInteraction_CanBeAffordedByInteractor(On.RoR2.PurchaseInteraction.orig_CanBeAffordedByInteractor orig, PurchaseInteraction self, Interactor activator)
        {
            var original = orig(self, activator);

            if (self.GetComponent<ShopTerminalBehavior>()
                && activator.TryGetComponent(out CharacterBody characterBody)
                && characterBody.hasCloakBuff)
            {
                original = true;
            }
            return original;
        }

        private static void AchievementNotificationPanel_SetAchievementDef(On.RoR2.UI.AchievementNotificationPanel.orig_SetAchievementDef orig, AchievementNotificationPanel self, AchievementDef achievementDef)
        {
            orig(self, achievementDef);
        }

        /*
       private static void AddPath(string path)
       {
           if (path != null)
           {
               var fileText = File.ReadAllText(path);
           }
       }

       private static void Add(string file)
       {
           var dict = LoadFile(file);
           if (dict)
           {
               Add(dict);
           }
       }

       private static void Add(Dictionary<string, string> tokenDictionary)
       {
       } */

        #region centertext

        private static void CenterTextMesh(GenericNotification genericNotification)
        {
            genericNotification.titleText.textMeshPro.alignment = TMPro.TextAlignmentOptions.Center;
            genericNotification.descriptionText.textMeshPro.alignment = TMPro.TextAlignmentOptions.Center;
        }

        private static void SetItem(On.RoR2.UI.GenericNotification.orig_SetItem orig, GenericNotification self, ItemDef itemDef)
        {
            orig(self, itemDef);
            CenterTextMesh(self);
        }

        private static void SetEquipment(On.RoR2.UI.GenericNotification.orig_SetEquipment orig, GenericNotification self, EquipmentDef equipmentDef)
        {
            orig(self, equipmentDef);
            CenterTextMesh(self);
        }

        private static void SetArtifact(On.RoR2.UI.GenericNotification.orig_SetArtifact orig, GenericNotification self, ArtifactDef artifactDef)
        {
            orig(self, artifactDef);
            CenterTextMesh(self);
        }

        private static void CharacterMasterNotificationQueue_PushNotification(On.RoR2.CharacterMasterNotificationQueue.orig_PushNotification orig, CharacterMasterNotificationQueue self, CharacterMasterNotificationQueue.NotificationInfo notificationInfo, float duration)
        {
            orig(self, notificationInfo, duration);
        }

        #endregion centertext

        /*

                #region DropEquipment

                public static void DropEquipment(EquipmentSlot slot, EquipmentDef equipmentDef)
                {
                    if (!cfgDropEquipment.Value) return;
                    var aimRay = slot.GetAimRay();
                    PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(equipmentDef.equipmentIndex),
                        aimRay.origin, aimRay.direction * 20f);
                    slot.characterBody.inventory.SetEquipmentIndex(EquipmentIndex.None);
                    CharacterMasterNotificationQueue.PushEquipmentTransformNotification(slot.characterBody.master, slot.characterBody.inventory.currentEquipmentIndex, EquipmentIndex.None, CharacterMasterNotificationQueue.TransformationType.Default);
                }

                private static void EquipmentIcon_Update(On.RoR2.UI.EquipmentIcon.orig_Update orig, RoR2.UI.EquipmentIcon self)
                {
                    orig(self);
                    if (self.targetEquipmentSlot
                        && EquipmentCanBeDropped(self.targetEquipmentSlot.equipmentIndex)
                        && !self.displayAlternateEquipment)
                    {
                        if (self.stockText)
                        {
                            bool shouldShowStock = self.stockText.gameObject.activeSelf;
                            self.stockText.gameObject.SetActive(true);
                            StringBuilder stringBuilder2 = HG.StringBuilderPool.RentStringBuilder();
                            //var equipmentSlotCount = self.targetInventory.GetEquipmentSlotCount();
                            string dropText = $"<size=45%>Drop Equip Mod: [Interact]</size>";
                            string colorMod;
                            if (self.playerCharacterMasterController
                                && self.playerCharacterMasterController.networkUser
                                && PlayerCharacterMasterController.CanSendBodyInput(self.playerCharacterMasterController.networkUser, out LocalUser localUser, out Player inputPlayer, out CameraRigController _)
                                && inputPlayer.GetButton(5)
                                )
                            {
                                colorMod = "green";
                            }
                            else
                            {
                                colorMod = "red";
                            }
                            stringBuilder2.Append(self.stockText.text);
                            stringBuilder2.Append($"<color={colorMod}>{dropText}</color>");
                            if (shouldShowStock) stringBuilder2.Append("\n");

                            if (shouldShowStock)//self.stockText.text.IsNullOrWhiteSpace())
                            {
                                //stringBuilder2.AppendInt(self.currentDisplayData.stock, 1U, uint.MaxValue);
                                stringBuilder2.Append($"x{self.currentDisplayData.stock}");
                            }

                            self.stockText.SetText(stringBuilder2);
                            HG.StringBuilderPool.ReturnStringBuilder(stringBuilder2);
                        }
                    }
                }

                public static bool PlayerCharacterMasterCanSendBodyInput(EquipmentSlot equipmentSlot, out LocalUser localUser, out Player player, out CameraRigController cameraRigController)
                {
                    if (equipmentSlot.characterBody
                            && equipmentSlot.characterBody.master
                            && equipmentSlot.characterBody.master.playerCharacterMasterController
                            && PlayerCharacterMasterController.CanSendBodyInput(equipmentSlot.characterBody.master.playerCharacterMasterController.networkUser,
                            out localUser, out player, out cameraRigController))
                        return true;
                    localUser = null;
                    player = null;
                    cameraRigController = null;
                    return false;
                }

                public static bool EquipmentCanBeDropped(EquipmentDef equipmentDef)
                {
                    return EquipmentCanBeDropped(equipmentDef.equipmentIndex);
                }

                public static bool EquipmentCanBeDropped(EquipmentIndex equipmentIndex)
                {
                    return equipmentIndex != EquipmentIndex.None;
                }

                #endregion DropEquipment

        */
    }
}