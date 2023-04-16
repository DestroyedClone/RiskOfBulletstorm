using BepInEx;
using BepInEx.Configuration;
using RoR2;
using RoR2.UI;
using System.Text;
using UnityEngine;
using Rewired;
using Rewired.Dev;
using static RoR2.MasterSpawnSlotController;

namespace RiskOfBulletstormRewrite
{
    public static class Tweaks
    {
        public static ConfigEntry<bool> cfgCenterNotifications;
        //public static ConfigEntry<bool> cfgDisableAutoPickup;
        //public static ConfigEntry<NotificationMod> cfgEnableBreachNotifications;
        public static ConfigEntry<bool> cfgDropEquipment;

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
            cfgDropEquipment = config.Bind(category, "Drop Equipment", true, "If true, you can drop your equipment by holding [interact] and using your equipment.");
            if (cfgCenterNotifications.Value)
            {
                On.RoR2.CharacterMasterNotificationQueue.PushNotification += CharacterMasterNotificationQueue_PushNotification;
                On.RoR2.UI.GenericNotification.SetArtifact += SetArtifact;
                On.RoR2.UI.GenericNotification.SetEquipment += SetEquipment;
                On.RoR2.UI.GenericNotification.SetItem += SetItem;
            }
            //if (cfgDisableAutoPickup.Value)
            //On.RoR2.GenericPickupController.OnTriggerStay += NoAutoPickup;
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

        private static void AchievementNotificationPanel_SetAchievementDef(On.RoR2.UI.AchievementNotificationPanel.orig_SetAchievementDef orig, AchievementNotificationPanel self, AchievementDef achievementDef)
        {
            orig(self, achievementDef);
        }

        private static void NoAutoPickup(On.RoR2.GenericPickupController.orig_OnTriggerStay orig, GenericPickupController self, Collider other)
        {
            return;
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
                        && PlayerCharacterMasterController.CanSendBodyInput(self.playerCharacterMasterController.networkUser, out LocalUser localUser, out Player inputPlayer, out CameraRigController cameraRigController)
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

        #endregion
    }
}