using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using TILER2;
using UnityEngine.Networking;
using static TILER2.MiscUtil;
using static TILER2.StatHooks;
using RiskOfBulletstorm.Utils;
using static RoR2.Chat;
using R2API;

namespace RiskOfBulletstorm.Items
{
    public class MasterRoundController : Item<MasterRoundController>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What additive percent of max health should your health increase by?", AutoConfigFlags.PreventNetMismatch)]
        public static float MaxHealthAdditiveMultiplier { get; private set; } = 0.10f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What are the max amount of hits are allowed to be taken per player before invalidating the drop condtions?", AutoConfigFlags.PreventNetMismatch)]
        public static int MaxAllowedHits { get; private set; } = 3;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many hits will your allowed hits increase by per stage?", AutoConfigFlags.PreventNetMismatch)]
        public static int AdditionalAllowedHitsPerStage { get; private set; } = 1;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the minimum damage required from a hit before counting towards your max allowed hits?", AutoConfigFlags.PreventNetMismatch)]
        public static int MinimumDamageBeforeInvalidation { get; private set; } = 5;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the player's own self damage be counted as a hit?", AutoConfigFlags.PreventNetMismatch)]
        public static bool AllowSelfDamage { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the maximum amount of hits allowed be printed in chat on teleporter start?", AutoConfigFlags.PreventNetMismatch)]
        public static bool EnableAnnounceOnStart { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the chat say who gets hit in chat?", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableShowHitInChat { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("For enemies hitting the players, should only the teleporter boss's hits count for the master round?", AutoConfigFlags.PreventNetMismatch)]
        public static bool OnlyAllowTeleBoss { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the hit detection for teleporter bosses include Lunar Wisps?", AutoConfigFlags.PreventNetMismatch)]
        public static bool EnableLunarWisps { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the hit detection for teleporter bosses include Lunar Golems?", AutoConfigFlags.PreventNetMismatch)]
        public static bool EnableLunarGolems { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the hit detection for teleporter bosses include Lunar Exploders?", AutoConfigFlags.PreventNetMismatch)]
        public static bool EnableLunarExploders { get; private set; } = false;

        public override string displayName => "Master Round Controller";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public BodyIndex BodyIndexLunarGolem;
        public BodyIndex BodyIndexLunarWisp;
        public BodyIndex BodyIndexLunarExploder;

        public static ItemDef[] MasterRoundDefs = new ItemDef[0];

        public override void SetupLate()
        {
            base.SetupLate();
            BodyIndexLunarGolem = BodyCatalog.FindBodyIndex("LUNARGOLEM");
            BodyIndexLunarWisp = BodyCatalog.FindBodyIndex("LUNARWISP");
            BodyIndexLunarExploder = BodyCatalog.FindBodyIndex("EXPLODER");

            var listOfMasterRounds = new List<ItemDef>()
            {
                MasterRoundI.instance.itemDef,
                MasterRoundII.instance.itemDef,
                MasterRoundIII.instance.itemDef,
                MasterRoundIV.instance.itemDef,
                MasterRoundV.instance.itemDef,
            };
            MasterRoundDefs = listOfMasterRounds.ToArray();
        }
        public override void InstallLanguage()
        {
            base.InstallLanguage();
            LanguageAPI.Add(modInfo.shortIdentifier + "_MASTERROUND_START", "<color=#c9ab14>[MR] Each player can take a max of {0} hits!</color>", "en");
            LanguageAPI.Add(modInfo.shortIdentifier + "_MASTERROUND_HIT", "<color=#ba3f0f>[MR] {0} has been hit {1} out of {2} times by {3}!</color>", "en");
            LanguageAPI.Add(modInfo.shortIdentifier + "_MASTERROUND_FAIL", "<color=#ba3f0f>[MR] {0} failed by getting hit {1} out of {2} times by {3}!</color>", "en");
        }

        public override void Install()
        {
            base.Install();
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal += TeleporterInteraction_onTeleporterChargedGlobal;
            On.RoR2.GlobalEventManager.OnHitEnemy += MasterRound_OnHitEnemy;
            GetStatCoefficients += MasterRoundController_GetStatCoefficients;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal -= TeleporterInteraction_onTeleporterChargedGlobal;
            On.RoR2.GlobalEventManager.OnHitEnemy -= MasterRound_OnHitEnemy;
            GetStatCoefficients -= MasterRoundController_GetStatCoefficients;
        }

        private void MasterRoundController_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var inv = sender.inventory;
                var itemCount = 0;
                foreach (var itemDef in MasterRoundDefs)
                {
                    itemCount += inv.GetItemCount(itemDef);
                }
                args.damageMultAdd += itemCount * MaxHealthAdditiveMultiplier;
            }
        }


        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            StartMasterRoundEvent();
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
                CheckMasterRoundEventResult();
            }
        }

        private void MasterRound_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (!victim && !victim.GetComponent<CharacterBody>()) return; // no victim
            if (!victim.GetComponent<CharacterBody>().masterObject) return;
            var masterRoundComponent = victim.GetComponent<CharacterBody>().masterObject.GetComponent<MasterRoundComponent>();
            if (!masterRoundComponent || !masterRoundComponent.teleporterCharging) return; // no component aka not a player
            if (damageInfo.rejected || damageInfo.damage < MinimumDamageBeforeInvalidation) return; //minimum damage check
            var attacker = damageInfo.attacker;
            if (!attacker) return; // TODO: Figure out if this donkey conflicts, because I feel like it might
            if (attacker == victim)
            {
                if (!AllowSelfDamage) return;
            }
            else
            {
                var characterBody = attacker.gameObject.GetComponent<CharacterBody>();
                if (!characterBody) return;
                if (OnlyAllowTeleBoss)
                {
                    if (!characterBody.isBoss) return;
                    if (!EnableLunarWisps && characterBody.bodyIndex == BodyIndexLunarWisp) return;
                    if (!EnableLunarGolems && characterBody.bodyIndex == BodyIndexLunarGolem) return;
                    if (!EnableLunarExploders && characterBody.bodyIndex == BodyIndexLunarExploder) return;
                }
            }
            masterRoundComponent.currentHits++;
            if (EnableShowHitInChat)
            {
                if (masterRoundComponent.currentHits <= masterRoundComponent.allowedHits)
                {
                    var characterBody = victim.GetComponent<CharacterBody>();
                    string victimName = characterBody ? characterBody.GetUserName() : "Someone";
                    string token = masterRoundComponent.currentHits < masterRoundComponent.allowedHits ? modInfo.shortIdentifier + "_MASTERROUND_HIT" : modInfo.shortIdentifier + "_MASTERROUND_FAIL";
                    string attackerName = damageInfo.attacker.GetComponent<CharacterBody>().GetDisplayName();
                    Chat.SendBroadcastChat(
                    new SimpleChatMessage
                    {
                        baseToken = token,
                        paramTokens = new[] { victimName, masterRoundComponent.currentHits.ToString(), masterRoundComponent.allowedHits.ToString(), attackerName
                    }
                    });
                }
            }
        }

        public void StartMasterRoundEvent()
        {
            var playerList = PlayerCharacterMasterController.instances;
            var StageCount = Run.instance.stageClearCount;
            var maxHits = MaxAllowedHits + StageCount * AdditionalAllowedHitsPerStage;
            if (NetworkServer.active)
            {
                foreach (var player in playerList)
                {
                    var body = player.master.GetBody();
                    if (body && body.healthComponent && body.healthComponent.alive)
                    {
                        var masterRoundComponent = player.master.gameObject.GetComponent<MasterRoundComponent>();
                        if (!masterRoundComponent) masterRoundComponent = player.master.gameObject.AddComponent<MasterRoundComponent>();
                        masterRoundComponent.allowedHits = maxHits + 1;
                        masterRoundComponent.teleporterCharging = true;
                        InstanceTracker.Add<MasterRoundComponent>(masterRoundComponent);
                    }
                }
            }
            if (EnableAnnounceOnStart)
                Chat.SendBroadcastChat(new SimpleChatMessage { baseToken = modInfo.shortIdentifier + "_MASTERROUND_START", paramTokens = new[] { maxHits.ToString() } });
        }

        public static int GetFalseMasterRoundNumber(int stageCount)
        {
            int trueStageCont = (stageCount + 1) % (MasterRoundDefs.Length + 1);
            return trueStageCont;
        }
        public static int GetTrueMasterRoundNumber(int stageCount)
        {
            int trueStageCont = (stageCount) % MasterRoundDefs.Length;
            return trueStageCont;
        }

        public static string GetItemDescStr()
        {
            return $"Increases maximum health by <style=cIsHealing>+" +Pct(MaxHealthAdditiveMultiplier)+"</style>";
        }

        public static string GetItemPickupStr()
        {
            return $"Increases maximum health.";
        }

        public void CheckMasterRoundEventResult()
        {
            bool success = true;

            foreach (var component in InstanceTracker.GetInstancesList<MasterRoundComponent>())
            {
                Chat.AddMessage(component.currentHits + " / " + component.allowedHits);
                if (component.currentHits > component.allowedHits)
                {
                    success = false;
                    break;
                }
            }
            if (success)
            {
                var currentStage = Run.instance.stageClearCount;
                var MasterRoundToGive = MasterRoundDefs[GetTrueMasterRoundNumber(currentStage)];
                HelperUtil.GiveItemToPlayers(MasterRoundToGive.itemIndex, true, 1);
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
