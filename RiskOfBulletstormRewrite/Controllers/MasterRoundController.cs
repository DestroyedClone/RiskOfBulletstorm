using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using RoR2.Projectile;
using System;
using BepInEx;
using BepInEx.Configuration;
using static RiskOfBulletstormRewrite.Main;
using System.Net;
using R2API;
using static R2API.RecalculateStatsAPI;
using UnityEngine.Networking;
using static RoR2.Chat;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class MasterRoundController : ControllerBase<MasterRoundController>
    {
        public static float MaxHealthAdditiveMultiplier { get; private set; } = 0.10f;
        public static int MaxAllowedHits { get; private set; } = 3;
        public static int AdditionalAllowedHitsPerStage { get; private set; } = 1;
        public static int MinimumDamageBeforeInvalidation { get; private set; } = 5;
        public static bool AllowSelfDamage { get; private set; } = false;
        public static bool EnableAnnounceOnStart { get; private set; } = true;
        public static bool EnableShowHitInChat { get; private set; } = true;
        public static bool OnlyAllowTeleBoss { get; private set; } = true;
        public static bool EnableLunarWisps { get; private set; } = false;
        public static bool EnableLunarGolems { get; private set; } = false;
        public static bool EnableLunarExploders { get; private set; } = false;
        public static string CategoryName => "MasterRounds";

        public static BodyIndex BodyIndexLunarGolem;
        public static BodyIndex BodyIndexLunarWisp;
        public static BodyIndex BodyIndexLunarExploder;

        public static ItemDef[] MasterRoundDefs = new ItemDef[0];

        public static Sprite staticIcon = MainAssets.LoadAsset<Sprite>("Assets/Textures/Icons/MasterRoundI.png");

        public static ItemTag[] defaultItemTags = new ItemTag[] { ItemTag.AIBlacklist, ItemTag.WorldUnique };

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            SetupLanguage();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            MaxHealthAdditiveMultiplier = config.Bind(CategoryName, "", 0.10f, "What additive percent of max health should your health increase by?").Value;
            MaxAllowedHits = config.Bind(CategoryName, "", 3, "What are the max amount of hits are allowed to be taken per player before invalidating the drop condtions?").Value;
            AdditionalAllowedHitsPerStage = config.Bind(CategoryName, "", 1, "How many hits will your allowed hits increase by per stage ? ").Value;
            MinimumDamageBeforeInvalidation = config.Bind(CategoryName, "", 5, "What is the minimum damage required from a hit before counting towards your max allowed hits?").Value;
            AllowSelfDamage = config.Bind(CategoryName, "", false, "Should the player's own self damage be counted as a hit?").Value;
            EnableAnnounceOnStart = config.Bind(CategoryName, "", true, "Should the maximum amount of hits allowed be printed in chat on teleporter start?").Value;
            EnableShowHitInChat = config.Bind(CategoryName, "", true, "Should the chat say who gets hit in chat?").Value;
            OnlyAllowTeleBoss = config.Bind(CategoryName, "", true, "For enemies hitting the players, should only the teleporter boss's hits count for the master round?").Value;
            EnableLunarWisps = config.Bind(CategoryName, "", false, "Should the hit detection for teleporter bosses include Lunar Wisps?").Value;
            EnableLunarGolems = config.Bind(CategoryName, "", true, "Should the hit detection for teleporter bosses include Lunar Golems?").Value;
            EnableLunarExploders = config.Bind(CategoryName, "", false, "Should the hit detection for teleporter bosses include Lunar Exploders?").Value;
        }

        public override void Hooks()
        {
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal += TeleporterInteraction_onTeleporterChargedGlobal;
            On.RoR2.GlobalEventManager.OnHitEnemy += MasterRound_OnHitEnemy;
            GetStatCoefficients += MasterRoundController_GetStatCoefficients;
            
        }

        [RoR2.SystemInitializer(dependencies: typeof(RoR2.MasterCatalog))]
        private static void MasterCatalog_Init()
        {
            BodyIndexLunarGolem = BodyCatalog.FindBodyIndex("LUNARGOLEM");
            BodyIndexLunarWisp = BodyCatalog.FindBodyIndex("LUNARWISP");
            BodyIndexLunarExploder = BodyCatalog.FindBodyIndex("EXPLODER");
        }

        [RoR2.SystemInitializer(dependencies: typeof(RoR2.ItemCatalog))]
        private static void ItemCatalog_Init()
        {
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

        private static void SetupLanguage()
        {
            LanguageAPI.Add("RISKOFBULLETSTORM_MASTERROUND_START", "<color=#c9ab14>[MR] Each player can take a max of {0} hits!</color>", "en");
            LanguageAPI.Add("RISKOFBULLETSTORM_MASTERROUND_HIT", "<color=#ba3f0f>[MR] {0} has been hit {1} out of {2} times by {3}!</color>", "en");
            LanguageAPI.Add("RISKOFBULLETSTORM_MASTERROUND_FAIL", "<color=#ba3f0f>[MR] {0} failed by getting hit {1} out of {2} times by {3}!</color>", "en");
        }

        private static void MasterRoundController_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender?.inventory)
            {
                var itemCount = 0;
                foreach (var itemDef in MasterRoundDefs)
                {
                    itemCount += sender.inventory.GetItemCount(itemDef);
                }
                args.damageMultAdd += itemCount * MaxHealthAdditiveMultiplier;
            }
        }


        private static void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            StartMasterRoundEvent();
        }

        private static void TeleporterInteraction_onTeleporterChargedGlobal(TeleporterInteraction obj)
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

        private static void MasterRound_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
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
                    string victimName = characterBody ? characterBody.GetUserName() : characterBody.GetDisplayName();
                    string token = masterRoundComponent.currentHits < masterRoundComponent.allowedHits ? "RISKOFBULLETSTORM_MASTERROUND_HIT" : "RISKOFBULLETSTORM_MASTERROUND_FAIL";
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

        public static void StartMasterRoundEvent()
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
                Chat.SendBroadcastChat(new SimpleChatMessage { baseToken = "RISKOFBULLETSTORM_MASTERROUND_START", paramTokens = new[] { maxHits.ToString() } });
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
            return $"Increases maximum health by <style=cIsHealing>+" + Utils.ItemHelpers.Pct(MaxHealthAdditiveMultiplier) + "</style>";
        }

        public static string GetItemPickupStr()
        {
            return $"Increases maximum health.";
        }

        public static void CheckMasterRoundEventResult()
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
                Utils.ItemHelpers.GiveItemToPlayers(MasterRoundToGive, true, 1);
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
