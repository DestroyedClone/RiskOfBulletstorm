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
using RoR2.CharacterAI;
using RoR2.Skills;
using System.Net;
using RoR2.Projectile;
using EliteSpawningOverhaul;


namespace RiskOfBulletstorm.Items
{
    public class CreateDog : Equipment_V2<CreateDog>
    {

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Can you pet the beetle?", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableDogPet { get; private set; } = true;

        private readonly bool BarkAtMimics = false;

        public override string displayName => "Spawn Beetle";
        public override float cooldown { get; protected set; } = 45f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Junior II\nA faithful companion.";

        protected override string GetDescString(string langid = null)
        {
            var descText = $"Spawns a beetle." +
                $"\nHas a chance to dig up a pickup";
            if (BarkAtMimics)
            {
                descText += "Barks at mimics.";
            }
            if (EnableDogPet)
            {
                descText += $"\nInteract to pet.";
            }
            return descText;
        }

        protected override string GetLoreString(string langID = null) => "Keeps the Survivor company. He has a good nose for treasure, but all attempts to train him in combat have failed.";

        public static GameObject characterPrefab;
        public GameObject friendMaster;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            CreatePrefab();
            CreateDoppelganger();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
            equipmentDef.canDrop = false;
            equipmentDef.enigmaCompatible = false;
        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            if (EnableDogPet) On.RoR2.PurchaseInteraction.OnInteractionBegin += PetBeetle;
        }

        private void PetBeetle(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            orig(self, activator);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            if (EnableDogPet) On.RoR2.PurchaseInteraction.OnInteractionBegin -= PetBeetle;
        }
        internal static void CreatePrefab()
        {
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/BeetleBody"), "DogBody", true);

            //CharacterMotor characterMotor = characterPrefab.GetComponent<CharacterMotor>();
            string name = "Chip the Beetle";
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

            int ArtifactShellBodyIndex = BodyCatalog.FindBodyIndex("ArtifactShellBody");
            GameObject ArtifactShellBodyGameObject = BodyCatalog.GetBodyPrefab(ArtifactShellBodyIndex);
                Highlight ASBHighlight = ArtifactShellBodyGameObject.GetComponent<Highlight>();
                Renderer ASBHtargetRenderer = ASBHighlight.targetRenderer;

            Highlight highlight = characterPrefab.AddComponent<Highlight>() as Highlight;
            highlight.pickupIndex = PickupCatalog.FindPickupIndex(ItemIndex.Syringe);
            if (ASBHtargetRenderer)
            {
                highlight.targetRenderer = ASBHtargetRenderer;
            } else
            {
                Debug.Log("Not found! awdasdadwdas");
            }
            highlight.strength = 1;
            highlight.highlightColor = Highlight.HighlightColor.interactive;
            highlight.isOn = false; //false

            PurchaseInteraction purchaseInteraction = characterPrefab.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = "DOG_NAME";
            purchaseInteraction.contextToken = "DOG_INTERACT";
            purchaseInteraction.costType = CostTypeIndex.None;
            purchaseInteraction.available = true;
            purchaseInteraction.cost = 0;
            purchaseInteraction.automaticallyScaleCostWithDifficulty = false;
            purchaseInteraction.ignoreSpherecastForInteractability = true;
            purchaseInteraction.setUnavailableOnTeleporterActivated = false;
            purchaseInteraction.isShrine = false;
            purchaseInteraction.isGoldShrine = false;

            RoR2.Hologram.HologramProjector hologramProjector = characterPrefab.AddComponent<RoR2.Hologram.HologramProjector>();
            hologramProjector.displayDistance = 15;

            GenericDisplayNameProvider genericDisplayNameProvider = characterPrefab.AddComponent<RoR2.GenericDisplayNameProvider>();
            genericDisplayNameProvider.displayToken = "DOG_NAME";
        }

        private void CreateDoppelganger()
        {

            friendMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/BeetleMaster"), "DogMaster", true);

            MasterCatalog.getAdditionalEntries += delegate (List<GameObject> list)
            {
                list.Add(friendMaster);
            };

            CharacterMaster component = friendMaster.GetComponent<CharacterMaster>();
            component.bodyPrefab = characterPrefab;
        }
        protected override bool PerformEquipmentAction(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body) return false;

            new MasterSummon
            {
                masterPrefab = characterPrefab,
                position = body.transform.position + new Vector3(0, 5, 0),
                rotation = body.transform.rotation,
                summonerBodyObject = body.gameObject,
                ignoreTeamMemberLimit = true,
                teamIndexOverride = new TeamIndex?(TeamIndex.Player)
            }.Perform();
            return true;
        }
    }
}
*/