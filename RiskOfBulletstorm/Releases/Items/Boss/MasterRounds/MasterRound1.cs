using System.Collections.ObjectModel;
using System.Linq;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
//using TMPro;
using TILER2;
using static TILER2.MiscUtil;
using static TILER2.StatHooks;
using RiskOfBulletstorm.Utils;
using static RoR2.Chat;
using static RiskOfBulletstorm.BulletstormPlugin;
using R2API;
using R2API.Utils;

namespace RiskOfBulletstorm.Releases.Items.Boss.MasterRounds
{
    public class MasterRound1 : Item<MasterRound1>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What additive percent of max health should your health increase by?", AutoConfigFlags.PreventNetMismatch)]
        public float MaxHealthAdditiveMultiplier { get; private set; } = 0.10f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What are the max amount of hits are allowed to be taken per player before invalidating the drop condtions?", AutoConfigFlags.PreventNetMismatch)]
        public static int MaxAllowedHits { get; private set; } = 3;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many hits will your allowed hits increase by per stage?", AutoConfigFlags.PreventNetMismatch)]
        public static int AdditionalAllowedHitsPerStage { get; private set; } = 1;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the minimum damage required from a hit before counting towards your max allowed hits?", AutoConfigFlags.PreventNetMismatch)]
        public int MinimumDamageBeforeInvalidation { get; private set; } = 5;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the player's own self damage be counted as a hit?", AutoConfigFlags.PreventNetMismatch)]
        public bool AllowSelfDamage { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the maximum amount of hits allowed be printed in chat on teleporter start?", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableAnnounceOnStart { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the chat say who gets hit in chat?", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableShowHitInChat { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("For enemies hitting the players, should only the teleporter boss's hits count for the master round?", AutoConfigFlags.PreventNetMismatch)]
        public bool OnlyAllowTeleBoss { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the hit detection for teleporter bosses include Lunar Wisps?", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableLunarWisps { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the hit detection for teleporter bosses include Lunar Golems?", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableLunarGolems { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the hit detection for teleporter bosses include Lunar Exploders?", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableLunarExploders { get; private set; } = false;
        public override string displayName => "Master Round I";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Increases maximum health." +
            "\nGiven to those who survive the teleporter event without exceeding a certain amount of blows.";

        protected override string GetDescString(string langid = null) => $"Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{Pct(MaxHealthAdditiveMultiplier)} health</style> <style=cStack>(+{Pct(MaxHealthAdditiveMultiplier)} per stack)</style>";

        protected override string GetLoreString(string langID = null) => "This rare artifact indicates mastery of the first chamber.\n" +
            "Apocryphal texts recovered from cultists of the Order indicate that the Gun and the Bullet are linked somehow.";

        public BodyIndex BodyIndexLunarGolem;
        public BodyIndex BodyIndexLunarWisp;
        public BodyIndex BodyIndexLunarExploder;

        readonly string announceStartToken = "<color=#c9ab14>[Master Round] Players can take a max of {0} hits!</color>";
        readonly string playerHitToken = "<color=#ba3f0f>[Master Round] {0} has been hit {1} out of {2} times by {3}!</color>";
        readonly string playerFailToken = "<color=#ba3f0f>[Master Round] {0} failed by getting hit {1} out of {2} times by {3}!</color>";
        readonly string playerHitNameFailed = "Someone";
        //readonly string itemPickupDescToken = numberCapitalized + " Chamber" + "\nThis " + descString + " artifact indicates mastery of the " + numberString + " chamber.";
        //readonly string itemPickupDescToken = "{0} Chamber \nThis {1} artifact indicates mastery of the {2} chamber.";
        //readonly string itemPickupDescBannedToken = "You probably dropped this, well no interesting lines here. If you didn't drop this, well... something's wrong.";

        public MasterRound1()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/SpreadAmmo.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/MasterRoundI.png"); //For evolution somehow
        }

        public override void SetupBehavior()
        {
            base.SetupBehavior();

            if (Compat_ItemStats.enabled)
            {
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return MaxHealthAdditiveMultiplier * count; },
                    (value, inv, master) => { return $"Max Health Multiplier: +{Pct(value)} health"; }
                ));
            }
        }

        public override void SetupLate()
        {
            base.SetupLate();
            BodyIndexLunarGolem = BodyCatalog.FindBodyIndex("LUNARGOLEM");
            BodyIndexLunarWisp = BodyCatalog.FindBodyIndex("LUNARWISP");
            BodyIndexLunarExploder = BodyCatalog.FindBodyIndex("EXPLODER");
        }

        public override void Install()
        {
            base.Install();
            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal += TeleporterInteraction_onTeleporterChargedGlobal;
            On.RoR2.GlobalEventManager.OnHitEnemy += MasterRound_OnHitEnemy;
            GetStatCoefficients += MasterRound1_GetStatCoefficients;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            TeleporterInteraction.onTeleporterBeginChargingGlobal -= TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterChargedGlobal -= TeleporterInteraction_onTeleporterChargedGlobal;
            On.RoR2.GlobalEventManager.OnHitEnemy -= MasterRound_OnHitEnemy;
            GetStatCoefficients -= MasterRound1_GetStatCoefficients;
        }
        public override void InstallLanguage()
        {
            base.InstallLanguage();
            ////
        }
        public override void UninstallLanguage()
        {
            base.UninstallLanguage();
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
                CheckMasterRoundEventResult(catalogIndex);
            }
        }

        private void MasterRound_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (!victim && !victim.GetComponent<CharacterBody>()) return; // no victim
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
                    string username = characterBody ? characterBody.GetUserName() : playerHitNameFailed;
                    string token = masterRoundComponent.currentHits < masterRoundComponent.allowedHits ? playerHitToken : playerFailToken;
                    Chat.SendBroadcastChat(
                    new SimpleChatMessage
                    {
                        baseToken = token,
                        paramTokens = new[] { username, masterRoundComponent.currentHits.ToString(), masterRoundComponent.allowedHits.ToString()
                    }
                    });
                }
            }
        }

        private void MasterRound1_GetStatCoefficients(CharacterBody sender, StatHookEventArgs args)
        {
            args.healthMultAdd += MaxHealthAdditiveMultiplier * GetCount(sender);
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
                        masterRoundComponent.allowedHits = maxHits+1;
                        masterRoundComponent.teleporterCharging = true;
                        InstanceTracker.Add<MasterRoundComponent>(masterRoundComponent);
                    }
                }
            }
            if (EnableAnnounceOnStart)
                Chat.SendBroadcastChat(new SimpleChatMessage { baseToken = announceStartToken, paramTokens = new[] { maxHits.ToString() } });
        }

        public void CheckMasterRoundEventResult(ItemIndex itemIndex)
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
                HelperUtil.GiveItemToPlayers(itemIndex, true, 1);
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
