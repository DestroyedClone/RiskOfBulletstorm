/*
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using Unity;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using EntityStates.ScavMonster;
using EntityStates.Engi.EngiWeapon;
//using Aetherium.Utils;
using RoR2.CharacterAI;
using RoR2.Skills;
using System.Net;
using RoR2.Projectile;


namespace RiskOfBulletstorm.Items
{
    public class Dog : Item_V2<Dog>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Can you pet the dog?", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableDogPet { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Chance to dig up an item? (Default 0.1 = 10%)", AutoConfigFlags.PreventNetMismatch)]
        public float DogDigChance { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Additional chance to dig up an item? (Default 0.05 = 5%)", AutoConfigFlags.PreventNetMismatch)]
        public float DogDigChanceStack { get; private set; } = 0.05f;

        //
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("What should be our duration between summoning the Lunar Chimera? (Default: 30 (30 seconds))", AutoConfigFlags.PreventNetMismatch)]
        public float lunarChimeraResummonCooldownDuration { get; private set; } = 30f;

        //


        public override string displayName => "Dog";
        public override ItemTier itemTier => ItemTier.Tier2;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Junior II\nA faithful companion. Finds items on teleporter event.";

        protected override string GetDescString(string langid = null)
        {
            var descText = $"{Pct(DogDigChance)} (+{Pct(DogDigChanceStack)} chance per stack) chance to dig up a pickup upon teleporter event.";
            if (EnableDogPet)
            {
                descText += $"\nInteract to pet.";
            }
            return descText;
        }
        protected override string GetLoreString(string langID = null) => "Keeps the Hunter company. He has a good nose for treasure, but all attempts to train him in combat have failed.";

        //private static List<RoR2.CharacterBody> Playername = new List<RoR2.CharacterBody>();

        public static GameObject ItemBodyModelPrefab;
        public static GameObject characterPrefab;
        public GameObject friendMaster;
        public bool RecentlySpawned = false;

        public override void SetupBehavior()
        {
            CreatePrefab();
            CreateDoppelganger();
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

            On.RoR2.CharacterBody.FixedUpdate += SummonFriend;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.CharacterBody.FixedUpdate -= SummonFriend;
        }
        internal static void CreatePrefab()
        {
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/BeetleBody"), "DogBody", true);

            //CharacterMotor characterMotor = characterPrefab.GetComponent<CharacterMotor>();
            string[] names = {
                                "1 Chip",
                                "2 Chap",
                                "3 Fido",
                                "4 Spot"
                            };
            string name = names[UnityEngine.Random.Range(0, names.Length)];
            LanguageAPI.Add("DOG_NAME", name);
            LanguageAPI.Add("DOG_SUBTITLE", "The Petted");
            LanguageAPI.Add("DOG_INTERACT", "Pet the beetle");
            CharacterBody bodyComponent = characterPrefab.GetComponent<CharacterBody>();
            bodyComponent.bodyIndex = -1; //def: 19
            bodyComponent.baseNameToken = "DOG_NAME"; // name token
            bodyComponent.subtitleNameToken = "DOG_SUBTITLE"; // subtitle token- used for umbras
            bodyComponent.bodyFlags = CharacterBody.BodyFlags.SprintAnyDirection;
            //bodyComponent.mainRootSpeed = 0;
            bodyComponent.baseMaxHealth = 10000;
            bodyComponent.levelMaxHealth = 10000;
            bodyComponent.baseRegen = 1000f;
            bodyComponent.levelRegen = 1000f;
            bodyComponent.baseMaxShield = 0;
            bodyComponent.levelMaxShield = 0;
            //bodyComponent.baseMoveSpeed = 12;
            bodyComponent.levelMoveSpeed = 0;
            //bodyComponent.baseAcceleration = 80;
            //bodyComponent.baseJumpPower = 0;
            bodyComponent.levelJumpPower = 0;
            //bodyComponent.baseDamage = 15;
            //bodyComponent.levelDamage = 1.5f;
            bodyComponent.baseAttackSpeed = 1;
            bodyComponent.levelAttackSpeed = 0;
            bodyComponent.baseCrit = 0;
            //bodyComponent.levelCrit = 0;
            bodyComponent.baseArmor = 10000; // 0.0099 damage multiplier
            bodyComponent.levelArmor = 0;
            bodyComponent.baseJumpCount = 0;
            bodyComponent.sprintingSpeedMultiplier = 1.45f;

            HealthComponent healthComponent = characterPrefab.GetComponent<HealthComponent>();
            healthComponent.dontShowHealthbar = true;

            Highlight highlight = characterPrefab.AddComponent<Highlight>() as Highlight;
            highlight.pickupIndex = PickupCatalog.FindPickupIndex(ItemIndex.Syringe);
            //highlight.targetRenderer = Sphere(UnityEngine.MeshRenderer);
            highlight.strength = 1;
            highlight.highlightColor = Highlight.HighlightColor.interactive;
            highlight.isOn = true; //false

            PurchaseInteraction purchaseInteraction = characterPrefab.AddComponent<PurchaseInteraction>() as PurchaseInteraction;
            purchaseInteraction.displayNameToken = "DOG_NAME";
            purchaseInteraction.contextToken = "DOG_INTERACT";
            purchaseInteraction.costType = CostTypeIndex.None;
            purchaseInteraction.available = true;
            purchaseInteraction.cost = 0;
            purchaseInteraction.automaticallyScaleCostWithDifficulty = false;

            /*DeathRewards deathRewards = characterPrefab.GetComponent<DeathRewards>();
            if (deathRewards)
            {
                deathRewards.expReward = 0;
               deathRewards.goldReward = 0;
            }
        }

        private void CreateDoppelganger()
        {
            // set up the doppelganger for artifact of vengeance here
            // quite simple, gets a bit more complex if you're adding your own ai, but commando ai will do

            friendMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/BeetleMaster"), "DogMaster", true);

            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(friendMaster);
            };

            CharacterMaster component = friendMaster.GetComponent<CharacterMaster>();
            component.bodyPrefab = characterPrefab;
        }

        private void SummonFriend(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            if (NetworkServer.active && self.master)
            {
                var LunarChimeraComponent = self.master.GetComponent<LunarChimeraComponent>();
                if (!LunarChimeraComponent) { LunarChimeraComponent = self.masterObject.AddComponent<LunarChimeraComponent>(); }

                var SummonerBodyMaster = self.master;
                if (SummonerBodyMaster) //Check if we're a minion or not. If we are, we don't summon a friend.
                {
                    if (SummonerBodyMaster.teamIndex == TeamIndex.Player && !self.isPlayerControlled)
                    {
                        orig(self);
                        return;
                    }
                }

                int InventoryCount = GetCount(self);
                if (InventoryCount > 0)
                {
                    if (LunarChimeraComponent.LastChimeraSpawned == null || !LunarChimeraComponent.LastChimeraSpawned.master || !LunarChimeraComponent.LastChimeraSpawned.master.hasBody)
                    {
                        LunarChimeraComponent.LastChimeraSpawned = null;
                        LunarChimeraComponent.ResummonCooldown -= Time.fixedDeltaTime;
                        if (LunarChimeraComponent.ResummonCooldown <= 0f)
                        {
                            var minDistance = 10f;
                            var maxDistance = 20f;


                            MasterSummon masterSummon = new MasterSummon
                            {
                                masterPrefab = characterPrefab,
                                position = self.transform.position + new Vector3(UnityEngine.Random.Range(minDistance, maxDistance),
                                5,
                                UnityEngine.Random.Range(minDistance, maxDistance)),
                                rotation = self.transform.rotation,
                                summonerBodyObject = self.gameObject,
                                ignoreTeamMemberLimit = true,
                                teamIndexOverride = new TeamIndex?(TeamIndex.Player)
                            };
                            masterSummon.Perform();
                            RecentlySpawned = true;

                            if (RecentlySpawned)
                            {
                                LunarChimeraComponent.ResummonCooldown = lunarChimeraResummonCooldownDuration;
                                RecentlySpawned = false;
                            }
                        }
                    }
                }
            }
            orig(self);
        }


        public class LunarChimeraComponent : MonoBehaviour
        {
            public CharacterBody LastChimeraSpawned;
            public float ResummonCooldown;
        }
    }
}
*/