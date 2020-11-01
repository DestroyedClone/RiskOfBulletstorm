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
using EliteSpawningOverhaul;


namespace RiskOfBulletstorm.Items
{
    public class CreateDog : Equipment_V2<CreateDog>
    {

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Can you pet the dog?", AutoConfigFlags.PreventNetMismatch)]
        public bool EnableDogPet { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Dog Name 1?", AutoConfigFlags.PreventNetMismatch)]
        public string DogName1 { get; private set; } = "Chip";
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Dog Name 2?", AutoConfigFlags.PreventNetMismatch)]
        public string DogName2 { get; private set; } = "Charles";
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Dog Name 3?", AutoConfigFlags.PreventNetMismatch)]
        public string DogName3 { get; private set; } = "Christopher";

        public override string displayName => "Spawn Dog";
        public string descText = "Junior II\nA faithful companion.";
        public override float cooldown { get; protected set; } = 120f;

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Do You Have Yours?\n" + descText;

        protected override string GetDescString(string langid = null)
        {
            var descText = $"Spawns a dog";
            if (EnableDogPet)
            {
                descText += $"\nInteract to pet.";
            }
            return descText;
        }

        protected override string GetLoreString(string langID = null) => "";

        public static GameObject characterPrefab;
        public GameObject friendMaster;
        public string nameModifier = " {0}";

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            CreatePrefab();
            CreateDoppelganger();
            CreateDogNames();

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
        }

        public override void Uninstall()
        {
            base.Uninstall();
        }
        internal static void CreatePrefab()
        {
            characterPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/BeetleBody"), "DogBody", true);

            //CharacterMotor characterMotor = characterPrefab.GetComponent<CharacterMotor>();
            string name = "the Beetle";
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

        }

        public static EliteIndex dogEliteIndex1;
        public static BuffIndex dogBuffIndex1;

        public static EliteIndex dogEliteIndex2;
        public static BuffIndex dogBuffIndex2;

        public static EliteIndex dogEliteIndex3;
        public static BuffIndex dogBuffIndex3;
        private void CreateDogNames() //a disgusting hack, please dont do this
        {

            var dogEliteDef1 = new CustomElite(
            new EliteDef
            {
                name = "Named 1",
                modifierToken = "NAMED_TOKEN_1",
                color = new Color32(150, 10, 10, 255),
            }, 1);
            dogEliteIndex1 = EliteAPI.Add(dogEliteDef1);
            LanguageAPI.Add(dogEliteDef1.EliteDef.modifierToken, DogName1 + nameModifier);

            var dogBuffDef1 = new CustomBuff(
            new BuffDef
            {
                name = "Affix_Jammed",
                buffColor = new Color32(10, 150, 10, 255),
                iconPath = "",
                eliteIndex = dogEliteIndex1,
                canStack = false
            });
            dogBuffIndex1 = BuffAPI.Add(dogBuffDef1);

            var dogEliteDef2 = new CustomElite(
            new EliteDef
            {
                name = "Named 2",
                modifierToken = "NAMED_TOKEN_2",
                color = new Color32(10, 10, 150, 255),
            }, 1);
            dogEliteIndex2 = EliteAPI.Add(dogEliteDef2);
            LanguageAPI.Add(dogEliteDef2.EliteDef.modifierToken, DogName2 + nameModifier);

            var dogBuffDef2 = new CustomBuff(
            new BuffDef
            {
                name = "Affix_Jammed",
                buffColor = new Color32(150, 10, 10, 255),
                iconPath = "",
                eliteIndex = dogEliteIndex2,
                canStack = false
            });
            dogBuffIndex2 = BuffAPI.Add(dogBuffDef2);

            var dogEliteDef3 = new CustomElite(
            new EliteDef
            {
                name = "Named 3",
                modifierToken = "NAMED_TOKEN_3",
                color = new Color32(150, 10, 10, 255),
            }, 1);
            dogEliteIndex3 = EliteAPI.Add(dogEliteDef3);
            LanguageAPI.Add(dogEliteDef3.EliteDef.modifierToken, DogName3 + nameModifier);

            var dogBuffDef3 = new CustomBuff(
            new BuffDef
            {
                name = "Affix_Jammed",
                buffColor = new Color32(150, 10, 10, 255),
                iconPath = "",
                eliteIndex = dogEliteIndex3,
                canStack = false
            });
            dogBuffIndex3 = BuffAPI.Add(dogBuffDef3);
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