
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using EntityStates.Captain.Weapon;
using On_ChargeCaptainShotgun = On.EntityStates.Captain.Weapon.ChargeCaptainShotgun; //DONT FORET TO REMOVE


namespace RiskOfBulletstorm.Items
{
    public class Blank : Item_V2<Blank>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage dealt? (Default: 0.3 = 30% damage)", AutoConfigFlags.PreventNetMismatch)]
        public float DamageDealt { get; private set; } = 0.3f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Button to blank (Default: T)", AutoConfigFlags.PreventNetMismatch)]
        public string BlankButton { get; private set; } = "T";

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Gamepad Button to blank (Default: T)", AutoConfigFlags.PreventNetMismatch)]
        public string BlankButtonGamepad { get; private set; } = "T";

        public override string displayName => "Blank";
        public override ItemTier itemTier => ItemTier.Tier1;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Banish Bullets";

        protected override string GetDescString(string langid = null) => $"Erases all enemy projectiles in the stage. Pushes enemies back, and deals {Pct(DamageDealt)} damage.";

        protected override string GetLoreString(string langID = null) => "12:01 PM - I've gathered about 7 or so of these bullets. They're a light-blue with a blue cap. Not sure of the usage.\n\n" +
            "12:42 PM - [Kyle] I tried finding one of my guns that I could load it into, but oddly it refuses to fit inside.\n\n" +
            "12:50 PM - [Kyle] Got upset at trying to shove one of the bullets in and threw it at the wall. I only heard a click before I was pushed back and momentarily stunned. I guess this is how you use it?\n\n" +
            "1:30 PM - [Kyle] Accidentally activated it while fighting a crowd of [REDACTED]. All of the bullets and [SWEAR REDACTED] coming at me vanished, and the enemies pushed the [SWEAR REDACTED] back and stunned instead of me. Think this is how you use this [SWEAR REDACTED].\n\n" +
            "4:30 PM - [Kyle] Carried a couple in my hand and tripped like an idiot, only one of them activated, fortunately. Unfortunately, my weapon broke when it was launched against the wall. I see the boss chamber up next, so I think I can take it with just these bullets.\n\n" +
            "4:33 PM - [LOG] EmptyMessage: The following transcription was automatically sent without any body. Did you forget to write?\n\n" +
            "12:00 PM - [Kyle] Guess what happened?";

        /* 1. Check Key Press
         * 2. Kill all projectiles that aren't allied
         * 3. Spawn a BlastAttack with basedmg*value with a high force aiming slightly upward with a stun damage type
         */


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
            On_ChargeCaptainShotgun.FixedUpdate += FixedUpdateHook;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On_ChargeCaptainShotgun.FixedUpdate += FixedUpdateHook;
        }
        void FixedUpdateHook(On_ChargeCaptainShotgun.orig_FixedUpdate orig, ChargeCaptainShotgun self)
        {
            if(Input.GetKeyDown(BlankButton))
            {
                Chat.AddMessage("Blank Used!");
                return;
            }
        }
        public class KillProjectiles : MonoBehaviour
        {

        }
        private bool KeyPressed()
        {
            return true;
        }
 /*       public class CheckButton : MonoBehaviour
        {
            void FixedUpdate()
            {
                if (Input.GetKeyDown("T"))
                {

                }
            }
        }*/
    }
}
