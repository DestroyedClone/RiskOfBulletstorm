using System.Collections.ObjectModel;
using System.Linq;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using RiskOfBulletstorm.Utils;

namespace RiskOfBulletstorm.Items
{
    public class MasterRoundNth : Item_V2<MasterRoundNth>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much maximum health is increased per Master Round?", AutoConfigFlags.PreventNetMismatch)]
        public int MasterRound_MaxHealthAdd { get; private set; } = 150;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many hits are allowed to be taken per player before invalidating the spawn? (Value: Max Hits)", AutoConfigFlags.PreventNetMismatch)]
        public static int MasterRound_AllowedHits { get; private set; } = 3;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many hits will your allowed hits increase by per stage? (Value: Max Hits * Stage)", AutoConfigFlags.PreventNetMismatch)]
        public static int MasterRound_AllowedHitsPerStage { get; private set; } = 1;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the minimum damage required from a hit before counting?", AutoConfigFlags.PreventNetMismatch)]
        public int MasterRound_MinimumDamage { get; private set; } = 5;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the player's own damage count as a hit?", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_AllowSelfDamage { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the maximum amount of hits allowed be printed in chat on teleporter start?", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_AnnounceMax { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the chat say who gets hit in chat?", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_ShowHitInChat { get; private set; } = false;

        public override string displayName => "Master Round";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Increases maximum health." +
            "\nGiven to those who survive the teleporter event without exceeding a certain amount of hits";

        protected override string GetDescString(string langid = null) => $"Increases maximum health by <style=cIsHealing>{MasterRound_MaxHealthAdd} health</style> <style=cStack>(+{MasterRound_MaxHealthAdd} max health per stack)</style>";

        protected override string GetLoreString(string langID = null) => "Apocryphal texts recovered from cultists of the Order indicate that the Gun and the Bullet are linked somehow." +
            "\nAny who enter the Gungeon are doomed to remain, living countless lives in an effort to break the cycle." +
            "\nFew return from the deadly route that leads to the Forge. Yet fewer survive that venture into less-explored territory." +
            "\nA monument to the legendary hero greets all who challenge the Gungeon, though their identity has been lost to the ages." +
            "\nThe legendary hero felled the beast at the heart of the Gungeon with five rounds. According to the myth, the sixth remains unfired.";

        readonly string[] adjustedDesc =
        {
            "questionable",
            "rare",
            "potent",
            "exceptional",
            "extraordinary",
            "unfathomable",
            "unprecedented"
        };

        readonly string[] bannedStages =
        {
            "mysteryspace",
            "limbo",
            "bazaar",
            "artifactworld",
            "goldshores",
            "arena"
        };

        readonly string[] filePaths = 
        {
            "@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundI.png",
            "@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundII.png",
            "@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundIII.png",
            "@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundIV.png",
            "@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundV.png",
            "@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundMoon.png",
        };

        public MasterRoundNth()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/SpreadAmmo.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundI.png"; //For evolution somehow
        }

        private Texture MoonMansTexture = Resources.Load<Texture>("@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundMoon.png");



        public override void SetupBehavior()
        {
            base.SetupBehavior();
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
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal += TeleporterInteraction_onTeleporterChargedGlobal;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            GetStatCoefficients += MasterRoundNth_GetStatCoefficients;
            On.RoR2.UI.GenericNotification.SetItem += GenericNotification_SetItem;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal -= TeleporterInteraction_onTeleporterChargedGlobal;
            On.RoR2.GlobalEventManager.OnHitEnemy -= GlobalEventManager_OnHitEnemy;
            GetStatCoefficients -= MasterRoundNth_GetStatCoefficients;
            On.RoR2.UI.GenericNotification.SetItem -= GenericNotification_SetItem;
        }

        private void MasterRoundNth_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            args.baseHealthAdd += MasterRound_MaxHealthAdd * GetCount(sender);
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (!victim) return;
            var component = victim.gameObject.GetComponent<MasterRoundComponent>();
            if (!component) return;
            if (!MasterRound_AllowSelfDamage && damageInfo.attacker == victim) return;
            if (damageInfo.rejected || damageInfo.damage < MasterRound_MinimumDamage) return;
            if (!component.teleporterCharging) return;
            component.currentHits++;
            if (MasterRound_ShowHitInChat)
            {
                Chat.AddMessage("[MASTER_ROUND] " + victim.name + " has " + component.currentHits + "/" + component.allowedHits);
            }
        }

        private void TeleporterInteraction_onTeleporterChargedGlobal(TeleporterInteraction obj)
        {
            if (NetworkServer.active)
            {
                var comps = UnityEngine.Object.FindObjectsOfType<MasterRoundComponent>();
                foreach (var component in comps)
                {
                    component.teleporterCharging = false;
                }
                Check();
            }
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            var playerList = PlayerCharacterMasterController.instances;
            var StageCount = Run.instance.stageClearCount;
            var maxHits = MasterRound_AllowedHits + StageCount * MasterRound_AllowedHitsPerStage;
            foreach (var player in playerList)
            {
                var body = player.master.GetBody();
                if (body)
                {
                    var MasterRoundComponent = body.gameObject.GetComponent<MasterRoundComponent>();
                    if (!MasterRoundComponent) MasterRoundComponent = body.gameObject.AddComponent<MasterRoundComponent>();
                    MasterRoundComponent.allowedHits = maxHits;
                    MasterRoundComponent.teleporterCharging = true;
                }
            }
            if (MasterRound_AnnounceMax)
                Chat.AddMessage("[MASTER_ROUND] Max Hits: "+ maxHits);
        }
        private void GenericNotification_SetItem(On.RoR2.UI.GenericNotification.orig_SetItem orig, GenericNotification self, ItemDef itemDef)
        {
            orig(self, itemDef);
            if (itemDef.itemIndex != catalogIndex) return;
            var StageCount = Mathf.Max(Run.instance.stageClearCount + 1, 1);

            string numberString = HelperUtil.NumbertoOrdinal(StageCount);
            string numberCapitalized = char.ToUpper(numberString[0]) + numberString.Substring(1);
            string descString = adjustedDesc[Mathf.Clamp(StageCount, 0, adjustedDesc.Length)];

            //https://www.dotnetperls.com/uppercase-first-letter

            string output = numberCapitalized + " Chamber" +
                "\nThis " + descString + " artifact indicates mastery of the " + numberString + " chamber.";
            if (bannedStages.Contains(SceneCatalog.mostRecentSceneDef.baseSceneName))
            {
                output = "huh? how did you...";
            }
            self.descriptionText.token = output;


            self.iconImage.texture = MoonMansTexture;
        }

        private void Check()
        {
            bool success = true;

            var comps = UnityEngine.Object.FindObjectsOfType<MasterRoundComponent>();
            foreach (var component in comps)
            {
                if (component.currentHits > component.allowedHits)
                {
                    success = false;
                    break;
                }
            }
            if (success)
            {
                HelperUtil.GiveItemToPlayers(catalogIndex);
            }
        }

        public class MasterRoundComponent : MonoBehaviour
        {
            public bool teleporterCharging = false;
            public int currentHits = 0;
            public int allowedHits = 7;
        }
    }
}
