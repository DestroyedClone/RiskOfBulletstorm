using System.Collections.ObjectModel;
using System.Linq;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using TILER2;
using static TILER2.StatHooks;
using RiskOfBulletstorm.Utils;
using static RoR2.Chat;

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
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("For enemies hitting the players, should only the teleporter boss's hits count for the master round?", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_OnlyAllowTeleBoss { get; private set; } = true;

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

        readonly Texture[] textures =
        {
            Resources.Load<Texture>("@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundI.png"),
            Resources.Load<Texture>("@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundII.png"),
            Resources.Load<Texture>("@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundIII.png"),
            Resources.Load<Texture>("@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundIV.png"),
            Resources.Load<Texture>("@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundV.png"),
            Resources.Load<Texture>("@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundMoon.png"),
        };

        public MasterRoundNth()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/SpreadAmmo.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundI.png"; //For evolution somehow
        }

       // private Texture MoonMansTexture = Resources.Load<Texture>("@RiskOfBulletstorm:Assets/Textures/Icons/MasterRoundMoon.png");



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
            On.RoR2.GlobalEventManager.OnHitEnemy += MasterRound_OnHitEnemy;
            GetStatCoefficients += MasterRoundNth_GetStatCoefficients;
            On.RoR2.UI.GenericNotification.SetItem += GenericNotification_SetItem;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal -= TeleporterInteraction_onTeleporterChargedGlobal;
            On.RoR2.GlobalEventManager.OnHitEnemy -= MasterRound_OnHitEnemy;
            GetStatCoefficients -= MasterRoundNth_GetStatCoefficients;
            On.RoR2.UI.GenericNotification.SetItem -= GenericNotification_SetItem;
        }

        private void MasterRoundNth_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            args.baseHealthAdd += MasterRound_MaxHealthAdd * GetCount(sender);
        }

        private void MasterRound_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (!victim) return; // no victim
            var MasterRoundComponent = victim.gameObject.GetComponent<MasterRoundComponent>();
            if (!MasterRoundComponent) return; // no component aka not a player
            if (!MasterRoundComponent.teleporterCharging) return; // charging check
            if (damageInfo.rejected || damageInfo.damage < MasterRound_MinimumDamage) return; //minimum damage check
            var attacker = damageInfo.attacker;
            if (!attacker) return; // TODO: Figure out if this donkey conflicts, because I feel like it might
            if (attacker == victim) //TODO: Figure out if I can switch case here.
            {
                if (!MasterRound_AllowSelfDamage) return;
            } else
            {
                var characterBody = attacker.gameObject.GetComponent<CharacterBody>();
                if (!characterBody) return;
                if (!characterBody.isBoss && MasterRound_OnlyAllowTeleBoss) return; // not a boss and only bosses
            }
            MasterRoundComponent.currentHits++;
            if (MasterRound_ShowHitInChat)
            {
                var characterBody = victim.GetComponent<CharacterBody>();
                string username = characterBody ? characterBody.GetUserName() : "Someone";
                //Chat.AddMessage("[MASTER_ROUND] " + victim.name + " has " + component.currentHits + "/" + component.allowedHits);
                Chat.SendBroadcastChat(
                    new SimpleChatMessage
                    { baseToken = "<color=#ba3f0f>[Master Round] Player {0} has been hit {1} out of {2} times!</color>", 
                        paramTokens = new[] { username, MasterRoundComponent.currentHits.ToString(), MasterRoundComponent.allowedHits.ToString()
                    }});
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
                MasterRound_CheckResult(catalogIndex);
            }
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            MasterRound_Start();
        }

        public void MasterRound_Start()
        {
            if (!NetworkServer.active) return;
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
                Chat.SendBroadcastChat(new SimpleChatMessage { baseToken = "<color=#c9ab14>[Master Round] Players can take a max of {0} hits!</color>", paramTokens = new[] { maxHits.ToString() } });
        }

        private void GenericNotification_SetItem(On.RoR2.UI.GenericNotification.orig_SetItem orig, GenericNotification self, ItemDef itemDef)
        {
            if (itemDef.itemIndex != catalogIndex)
            {
                orig(self, itemDef);
                return;
            }
            var clearCount = Run.instance.stageClearCount;
            var StageCount = Mathf.Max(clearCount + 1, 1);

            string numberString = HelperUtil.NumbertoOrdinal(StageCount);
            string numberCapitalized = char.ToUpper(numberString[0]) + numberString.Substring(1);
            string descString = adjustedDesc[Mathf.Clamp(StageCount, 0, adjustedDesc.Length-1)];

            //https://www.dotnetperls.com/uppercase-first-letter

            string output = numberCapitalized + " Chamber" +
                "\nThis " + descString + " artifact indicates mastery of the " + numberString + " chamber.";
            if (bannedStages.Contains(SceneCatalog.mostRecentSceneDef.baseSceneName))
            {
                output = "huh? how did you...";
            }

            self.titleText.token = itemDef.nameToken;
            self.titleTMP.color = ColorCatalog.GetColor(itemDef.colorIndex);
            self.descriptionText.token = output;

            if (itemDef.pickupIconPath != null)
            {
                //self.iconImage.texture = Resources.Load<Texture>(itemDef.pickupIconPath);
                var index = Mathf.Max(clearCount, textures.Length-1);
                //self.iconImage.texture = Resources.Load<Texture>(filePaths[index]);
                self.iconImage.texture = textures[index];
            }

        }

        public void MasterRound_CheckResult(ItemIndex itemIndex)
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
                HelperUtil.GiveItemToPlayers(itemIndex);
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
