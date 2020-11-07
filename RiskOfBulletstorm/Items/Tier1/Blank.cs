
using System.Collections.Generic;
using System.Collections.ObjectModel;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using RoR2.Projectile;
using static TILER2.MiscUtil;
using static RiskOfBulletstorm.Shared.BlankRelated;

namespace RiskOfBulletstorm.Items
{
    public class Blank : Item_V2<Blank>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Button to blank (Default: T)", AutoConfigFlags.PreventNetMismatch)]
        public string BlankButton { get; private set; } = "T";

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Gamepad Button to blank (Default: T)", AutoConfigFlags.PreventNetMismatch)]
        public string BlankButtonGamepad { get; private set; } = "T";

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Blank Cooldown (Default: 1 second)", AutoConfigFlags.PreventNetMismatch)]
        public float ConfigBlankCooldown { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage dealt to enemies on use (Default: 1 = 100% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageDealt { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Radius the projectile deals damage (Default: 10f)", AutoConfigFlags.PreventNetMismatch)]
        public float BlankRadius { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Radius of projectiles the Blank clears. Set to '0' to disable, or '-1' for infinite range (Default: -1 = Infinite)", AutoConfigFlags.PreventNetMismatch)]
        public float BlankClearRadius { get; private set; } = -1f;


        public override string displayName => "Blank";
        public string descText = "Erases all enemy projectiles";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Banish Bullets\n"+descText;

        protected override string GetDescString(string langid = null) => $"{descText} in the stage. Pushes enemies back, and deals {Pct(DamageDealt)} damage.";

        protected override string GetLoreString(string langID = null) => "12:01 PM - [Kyle] I've gathered about 7 or so of these bullets. They're a light-blue with a blue cap. Not sure of the usage.\n\n" +
            "12:42 PM - [Kyle] I tried finding one of my guns that I could load it into, but oddly it refuses to fit inside.\n\n" +
            "12:48 PM - [Kyle] Got upset at trying to shove one of the bullets in and threw it at the wall. I only heard a click before I was pushed back and momentarily stunned. I guess this is how you use it?\n\n" +
            "1:30 PM - [Kyle] Accidentally activated it while fighting a crowd of [REDACTED]. All of the bullets and [SWEAR REDACTED] coming at me vanished, and the enemies pushed the [SWEAR REDACTED] back and stunned instead of me. Think this is how you use this [SWEAR REDACTED].\n\n" +
            "4:30 PM - [Kyle] Carried a couple in my hand and tripped like an idiot, only one of them activated, fortunately. Unfortunately, my weapon broke when it was launched against the wall. I see the boss chamber up next, so I think I can take it with just these bullets.\n\n" +
            "4:33 PM - [LOG] EmptyMessage: The following transcription was automatically sent without any body. Did you accidentally send?\n\n" +
            "12:00 PM - [Kyle] Guess what happened?";

        public bool BlankUsed = false;
        public override void SetupBehavior()
        {

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
            On.RoR2.CharacterBody.FixedUpdate += CharacterBody_FixedUpdate;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.FixedUpdate -= CharacterBody_FixedUpdate;
        }
        private void CharacterBody_FixedUpdate(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            if (NetworkServer.active && self.master)
            {
                // var BlankComponent = self.master.AddComponent<BlankComponent>();
                BlankComponent BlankComponent = self.master.GetComponent<BlankComponent>();
                if (!BlankComponent) { BlankComponent = self.masterObject.AddComponent<BlankComponent>(); }

                var InventoryCount = GetCount(self);

                if (InventoryCount > 0)
                {
                    BlankComponent.BlankCooldown -= Time.fixedDeltaTime;
                    if (self.isPlayerControlled)
                    {
                        if (BlankComponent.BlankCooldown <= 0)
                        {
                            BlankUsed = false;
                            if (Input.GetKeyDown(KeyCode.T) && !BlankUsed)
                            {
                                Chat.AddMessage("Blank: Blank Used!");
                                BlankUsed = true;

                                //self.inventory.RemoveItem(catalogIndex);
                                FireBlank(self, self.corePosition, BlankRadius, DamageDealt, BlankClearRadius, true);
                                BlankComponent.BlankCooldown = ConfigBlankCooldown;
                            }
                        }
                    }
                }
            }
            orig(self);
        }
        public class BlankComponent : MonoBehaviour
        {
            public float BlankCooldown;
        }
    }
}
