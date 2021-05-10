using System.Collections.ObjectModel;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.Shared.Blanks.MasterBlankItem;
using RiskOfBulletstorm.Utils;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class Blank : Item<Blank>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What Keyboard button should activate the Blank?", AutoConfigFlags.None)]
        public KeyCode BlankButton { get; private set; } = KeyCode.T;
        //https://docs.unity3d.com/ScriptReference/KeyCode.html

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What Gamepad button should activate the Blank? (Default: T)", AutoConfigFlags.None)]
        public string BlankButtonGamepad { get; private set; } = "T";

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the cooldown for activating a Blank?", AutoConfigFlags.PreventNetMismatch)]
        public float ConfigBlankCooldown { get; private set; } = 3f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much damage should Blanks deal to nearby enemies? (Value: Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Blank_DamageDealt { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius in meters that Blanks deal damage when activated?", AutoConfigFlags.PreventNetMismatch)]
        public float BlankRadius { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What is the radius in meters that Blanks will clear projectiles? (Set to '0' to disable, or '-1' for infinite range)", AutoConfigFlags.PreventNetMismatch)]
        public float BlankClearRadius { get; private set; } = -1f;


        public override string displayName => "Blank";
        public string descText = "Erases all enemy projectiles";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "<b>Banish Bullets<b>\n"+descText;

        protected override string GetDescString(string langid = null)
        {
            var desc = $"";
            if (BlankClearRadius != 0)
            desc = $"<style=cIsUtility>{descText}</style>";
            if (BlankClearRadius == -1) desc += $" in the stage.";
            else desc += $"within a radius of {BlankClearRadius} meters";
            desc += $" <style=cIsUtility>Pushes enemies back</style>, and <style=cIsDamage>deals {Pct(Blank_DamageDealt)} damage</style>";
            if (BlankRadius == BlankClearRadius) desc += $".";
            else desc += $"within {BlankRadius}";
            desc += $"\nConsumed on use." +
                $"\nCooldown: {ConfigBlankCooldown} seconds" +
                $"\nPress "+BlankButton.ToString()+"/"+BlankButtonGamepad.ToString()+ " to activate";
            return desc;
        }

        protected override string GetLoreString(string langID = null) => "12:01 PM - [Kyle] I've gathered about 7 or so of these bullets. They're a light-blue with a blue cap. Not sure of the usage.\n\n" +
            "12:42 PM - [Kyle] I tried finding one of my guns that I could load it into, but oddly it refuses to fit inside.\n\n" +
            "12:48 PM - [Kyle] Got upset at trying to shove one of the bullets in and threw it at the wall. I only heard a click before I was pushed back and momentarily stunned. I guess this is how you use it?\n\n" +
            "1:30 PM - [Kyle] Accidentally activated it while fighting a crowd of [REDACTED]. All of the bullets and [SWEAR REDACTED] coming at me vanished, and the enemies pushed the [SWEAR REDACTED] back and stunned instead of me. Think this is how you use this [SWEAR REDACTED].\n\n" +
            "4:30 PM - [Kyle] Carried a couple in my hand and tripped like an idiot, only one of them activated, fortunately. Unfortunately, my weapon broke when it was launched against the wall. I see the boss chamber up next, so I think I can take it with just these bullets.\n\n" +
            "4:33 PM - [LOG] EmptyMessage: The following transcription was automatically sent without any body. Did you accidentally hit send?\n\n" +
            "12:00 PM - [Kyle] Guess what happened?";

        public static GameObject ItemBodyModelPrefab;

        public Blank()
        {
            modelResource = assetBundle.LoadAsset<GameObject>("Assets/Models/Prefabs/Blank.prefab");
            iconResource = assetBundle.LoadAsset<Sprite>("Assets/Textures/Icons/Blank.png");
        }
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
            On.RoR2.CharacterBody.Start += CharacterBody_Start;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.Start -= CharacterBody_Start;
        }
        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (NetworkServer.active && self.isPlayerControlled)
            {
                var masterObj = self.masterObject;
                BlankComponent BlankComponent = masterObj.GetComponent<BlankComponent>();
                if (!BlankComponent) { BlankComponent = masterObj.AddComponent<BlankComponent>(); }
                BlankComponent.characterBody = self;
            }
        }

        public class BlankComponent : MonoBehaviour
        {
            public float BlankCooldown = instance.ConfigBlankCooldown;
            public float BlankCooldownTime = 0;
            public bool isReady = false;
            LocalUser localUser;
            public CharacterBody characterBody;

            public void OnEnable()
            {
                localUser = LocalUserManager.readOnlyLocalUsersList[0];
            }

            public void Update()
            {
                BlankCooldownTime -= Time.fixedDeltaTime;

                if (BlankCooldownTime <= 0)
                {
                    isReady = true;
                }

                if (!localUser.isUIFocused && characterBody.inventory.GetItemCount(instance.catalogIndex) > 0) //TY KingEnderBrine for the help
                {
                    if (Input.GetKeyDown(instance.BlankButton) && isReady)
                    {
                        isReady = false;
                        FireBlank(characterBody, characterBody.corePosition, instance.BlankRadius, instance.Blank_DamageDealt, instance.BlankClearRadius, true);
                        BlankCooldownTime = BlankCooldown;
                    }
                }
            }
        }
    }
}
