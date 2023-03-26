using BepInEx.Configuration;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace RiskOfBulletstormRewrite
{
    public static class Tweaks
    {
        public static ConfigEntry<bool> cfgCenterNotifications;
        //public static ConfigEntry<bool> cfgDisableAutoPickup;
        //public static ConfigEntry<NotificationMod> cfgEnableBreachNotifications;

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
            if (cfgCenterNotifications.Value)
            {
                On.RoR2.CharacterMasterNotificationQueue.PushNotification += CharacterMasterNotificationQueue_PushNotification;
                On.RoR2.UI.GenericNotification.SetArtifact += SetArtifact;
                On.RoR2.UI.GenericNotification.SetEquipment += SetEquipment;
                On.RoR2.UI.GenericNotification.SetItem += SetItem;
            }
            /*             var languagePaths = Directory.GetFiles(Assets.assemblyDir, "*.language", SearchOption.AllDirectories);
                        foreach (var path in languagePaths)
                        {
                            AddPath(path);
                        } */
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
    }
}