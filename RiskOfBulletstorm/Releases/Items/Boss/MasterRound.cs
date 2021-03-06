﻿using System.Collections.ObjectModel;
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

namespace RiskOfBulletstorm.Items
{
    public class MasterRoundNth : Item<MasterRoundNth>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What percent of max health should it increase by? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float MasterRound_MaxHealthMult { get; private set; } = 0.10f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many hits are allowed to be taken per player before invalidating the drop? (Value: Max Hits)", AutoConfigFlags.PreventNetMismatch)]
        public static int MasterRound_AllowedHits { get; private set; } = 3;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How many hits will your allowed hits increase by per stage? (Value: Additional Max Hits multiplied by Stage)", AutoConfigFlags.PreventNetMismatch)]
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
        public bool MasterRound_ShowHitInChat { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("For enemies hitting the players, should only the teleporter boss's hits count for the master round?", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_OnlyAllowTeleBoss { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the hit detection for teleporter bosses include Lunar Wisps?", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_TeleLunarWisps { get; private set; } = false;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the hit detection for teleporter bosses include Lunar Golems?", AutoConfigFlags.PreventNetMismatch)]
        public bool MasterRound_TeleLunarGolems { get; private set; } = false;

        public override string displayName => "Master Round";
        public override ItemTier itemTier => ItemTier.Boss;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Healing, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "Increases maximum health." +
            "\nGiven to those who survive the teleporter event without exceeding a certain amount of hits";

        protected override string GetDescString(string langid = null) => $"Increases <style=cIsHealing>maximum health</style> by <style=cIsHealing>{Pct(MasterRound_MaxHealthMult)} health</style> <style=cStack>(+{Pct(MasterRound_MaxHealthMult)} per stack)</style>";

        protected override string GetLoreString(string langID = null) => "Apocryphal texts recovered from cultists of the Order indicate that the Gun and the Bullet are linked somehow." +
            "\nAny who enter the Gungeon are doomed to remain, living countless lives in an effort to break the cycle." +
            "\nFew return from the deadly route that leads to the Forge. Yet fewer survive that venture into less-explored territory." +
            "\nA monument to the legendary hero greets all who challenge the Gungeon, though their identity has been lost to the ages." +
            "\nThe legendary hero felled the beast at the heart of the Gungeon with five rounds. According to the myth, the sixth remains unfired.";

        public static BodyIndex BodyIndexLunarGolem;
        public static BodyIndex BodyIndexLunarWisp;

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

        readonly Texture[] texturesOld =
        {
            Resources.Load<Texture>("Assets/Textures/Icons/MasterRoundI.png"),
            Resources.Load<Texture>("Assets/Textures/Icons/MasterRoundII.png"),
            Resources.Load<Texture>("Assets/Textures/Icons/MasterRoundIII.png"),
            Resources.Load<Texture>("Assets/Textures/Icons/MasterRoundIV.png"),
            Resources.Load<Texture>("Assets/Textures/Icons/MasterRoundV.png"),
            Resources.Load<Texture>("Assets/Textures/Icons/MasterRoundMoon.png"),
        };

        readonly string announceStartToken = "<color=#c9ab14>[Master Round] Players can take a max of {0} hits!</color>";
        readonly string playerHitToken = "<color=#ba3f0f>[Master Round] Player {0} has been hit {1} out of {2} times!</color>";
        readonly string playerFailToken = "<color=#ba3f0f>[Master Round] Player {0} failed by getting hit {1} out of {2} times!</color>";
        readonly string playerHitNameFailed = "Someone";
        //readonly string itemPickupDescToken = numberCapitalized + " Chamber" + "\nThis " + descString + " artifact indicates mastery of the " + numberString + " chamber.";
        readonly string itemPickupDescToken = "{0} Chamber \nThis {1} artifact indicates mastery of the {2} chamber.";
        readonly string itemPickupDescBannedToken = "You probably dropped this, well no interesting lines here. If you didn't drop this, well... something's wrong.";

        public MasterRoundNth()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/SpreadAmmo.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/MasterRoundI.png"); //For evolution somehow
        }

       // private Texture MoonMansTexture = Resources.Load<Texture>("Assets/Textures/Icons/MasterRoundMoon.png");



        public override void SetupBehavior()
        {
            base.SetupBehavior();

            if (Compat_ItemStats.enabled)
            {
                Compat_ItemStats.CreateItemStatDef(itemDef,
                    ((count, inv, master) => { return MasterRound_MaxHealthMult * count; },
                    (value, inv, master) => { return $"Max Health Multiplier: +{Pct(value)} health"; }
                ));
            }
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void SetupLate()
        {
            base.SetupLate();
            BodyIndexLunarGolem = BodyCatalog.FindBodyIndex("LUNARGOLEM");
            BodyIndexLunarWisp = BodyCatalog.FindBodyIndex("LUNARWISP");
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
            args.healthMultAdd += MasterRound_MaxHealthMult * GetCount(sender);
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
            if (attacker == victim)
            {
                if (!MasterRound_AllowSelfDamage) return;
            } else
            {
                var characterBody = attacker.gameObject.GetComponent<CharacterBody>();
                if (!characterBody) return;
                if (MasterRound_OnlyAllowTeleBoss)
                {
                    if (!characterBody.isBoss) return;
                    if (!MasterRound_TeleLunarWisps && characterBody.bodyIndex == BodyIndexLunarWisp) return;
                    if (!MasterRound_TeleLunarGolems && characterBody.bodyIndex == BodyIndexLunarGolem) return;
                }
            }
            MasterRoundComponent.currentHits++;
            if (MasterRound_ShowHitInChat)
            {
                if (MasterRoundComponent.currentHits <= MasterRoundComponent.allowedHits)
                {
                    var characterBody = victim.GetComponent<CharacterBody>();
                    string username = characterBody ? characterBody.GetUserName() : playerHitNameFailed;
                    string token = MasterRoundComponent.currentHits < MasterRoundComponent.allowedHits ? playerHitToken : playerFailToken;
                    Chat.SendBroadcastChat(
                    new SimpleChatMessage
                    {
                        baseToken = token,
                        paramTokens = new[] { username, MasterRoundComponent.currentHits.ToString(), MasterRoundComponent.allowedHits.ToString()
                    }
                    });
                }
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
            var playerList = PlayerCharacterMasterController.instances;
            var StageCount = Run.instance.stageClearCount;
            var maxHits = MasterRound_AllowedHits + StageCount * MasterRound_AllowedHitsPerStage;
            if (NetworkServer.active)
            {
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
            }
            if (MasterRound_AnnounceMax)
                Chat.SendBroadcastChat(new SimpleChatMessage { baseToken = announceStartToken, paramTokens = new[] { maxHits.ToString() } });
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

            //string output2 = numberCapitalized + " Chamber" +
                //"\nThis " + descString + " artifact indicates mastery of the " + numberString + " chamber."; TODO: REMOVE WHEN KNOW IT WORKS
            string output = string.Format(itemPickupDescToken, numberCapitalized, descString, numberString);
            if (bannedStages.Contains(SceneCatalog.mostRecentSceneDef.baseSceneName))
            {
                output = itemPickupDescBannedToken;
            }

            self.titleText.token = itemDef.nameToken;
            self.titleTMP.color = ColorCatalog.GetColor(itemDef.colorIndex);
            self.descriptionText.token = output;

            if (itemDef.pickupIconSprite != null)
            {
                self.iconImage.texture = itemDef.pickupIconTexture;
                //var index = Mathf.Max(clearCount, textures.Count-1);
                //self.iconImage.texture = Resources.Load<Texture>(filePaths[index]);
                //self.iconImage.texture = textures[index];
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
