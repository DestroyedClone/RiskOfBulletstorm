﻿using System.Collections.ObjectModel;
using R2API;
using RoR2;
using TILER2;
using RiskOfBulletstorm.Utils;
using UnityEngine;
using static RiskOfBulletstorm.BulletstormPlugin;

namespace RiskOfBulletstorm.Items
{
    public class CurseController : Item<CurseController>
    {
        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("How many curse is needed before Lord of the Jammed spawns? Set it to -1 to disable. (Default: 10)", AutoConfigFlags.PreventNetMismatch)]
        //public float Curse_LOTJAmount { get; private set; } = 10f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the stacks of Curse be shown in the inventory?", AutoConfigFlags.PreventNetMismatch)]
        public bool ShowInInventory { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Enable Jammed enemies?", AutoConfigFlags.PreventNetMismatch)]
        public bool JammedEnable { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional damage should a Jammed enemy deal? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float JammedDamageBoost { get; private set; } = 1.00f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional crit should a Jammed enemy have? (Value: Additive)", AutoConfigFlags.PreventNetMismatch)]
        public float JammedCritBoost { get; private set; } = 100f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional attack speed should a Jammed enemy have? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float JammedAttackSpeedBoost { get; private set; } = 0.2f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional move speed should a Jammed enemy have? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float JammedMoveSpeedBoost { get; private set; } = 0.2f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional health should a Jammed enemy have? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float JammedHealthBoost { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow bosses to become Jammed?", AutoConfigFlags.PreventNetMismatch)]
        public bool JammedAllowBosses { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow umbra(e) to become Jammed?", AutoConfigFlags.PreventNetMismatch)]
        public bool JammedAllowUmbra { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow Happiest Mask ghosts to retain their Jammed status?", AutoConfigFlags.PreventNetMismatch)]
        public bool JammedAllowGhost { get; private set; } = false;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("[Aetherium Support] Allow Unstable Design summons to become Jammed?", AutoConfigFlags.PreventNetMismatch)]
        public bool JammedAllowUnstableDesign { get; private set; } = true;

        public override string displayName => "CurseMasterItem";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        //public GameObject curseEffect = Resources.Load<GameObject>("prefabs/effects/ImpSwipeEffect");
        public GameObject jammedFire;

        public ItemDef curseTally;
        //public static ItemIndex curseMax;
        public static ItemDef isJammedItem;

        public static readonly ItemDef umbraItemDef = RoR2.RoR2Content.Items.InvadingDoppelganger;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
            GameObject wispBody = Resources.Load<GameObject>("prefabs/characterbodies/WispBody");
            var wispFire = wispBody.transform.Find("Model Base/mdlWisp1Mouth/WispArmature/ROOT/Base/Fire").gameObject;
            jammedFire = wispFire.InstantiateClone("Bulletstorm_JammedFire",true);
            jammedFire.transform.localPosition = Vector3.zero;
            jammedFire.GetComponent<ParticleSystemRenderer>().material = Resources.Load<Material>("materials/matClayGooDebuff");
            // https://discordapp.com/channels/562704639141740588/562704639569428506/810301262432174090
            var main = jammedFire.GetComponent<ParticleSystem>().main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

            // Used to keep track of the player's curse per player //
            curseTally = ScriptableObject.CreateInstance<ItemDef>();
            curseTally.hidden = !ShowInInventory;
            curseTally.name = modInfo.shortIdentifier + "CURSETALLY_NAME";
            curseTally.tier = ItemTier.NoTier;
            curseTally.canRemove = false;
            curseTally.nameToken = curseTally.name;
            curseTally.pickupToken = modInfo.shortIdentifier + "CURSETALLY_PICKUP";
            curseTally.loreToken = "";
            curseTally.descriptionToken = modInfo.shortIdentifier + "CURSETALLY_DESC";
            curseTally.pickupIconSprite = Resources.Load<Sprite>("materials/matWeakOverlay");
            ItemAPI.Add(new CustomItem(curseTally, new ItemDisplayRuleDict()));

            // This way allows ghosts to maintain their curse status //
            isJammedItem = ScriptableObject.CreateInstance<ItemDef>();
            isJammedItem.hidden = true;
            isJammedItem.name = modInfo.shortIdentifier + "ISJAMMED";
            isJammedItem.tier = ItemTier.NoTier;
            isJammedItem.canRemove = false;
            isJammedItem.nameToken = isJammedItem.name;
            isJammedItem.pickupToken = modInfo.shortIdentifier + "ISJAMMED_PICKUP";
            isJammedItem.loreToken = "";
            isJammedItem.descriptionToken = modInfo.shortIdentifier + "ISJAMMED_DESC";
            isJammedItem.pickupIconSprite = Resources.Load<Sprite>("materials/matFullCrit");
            ItemAPI.Add(new CustomItem(isJammedItem, new ItemDisplayRuleDict()));

            // Used to track who to spawn the Lord of the Jammed on //
            // Currently unused //
            /*
            var curseMaxDef = new CustomItem(new ItemDef
            {
                hidden = true,
                name = "ROBInternalCurseSpawnLOTJ",
                tier = ItemTier.NoTier,
                canRemove = false
            }, new ItemDisplayRuleDict(null));
            curseMax = ItemAPI.Add(curseMaxDef);*/
        }
        public override void Install()
        {
            base.Install();
            if (JammedEnable)
            {
                CharacterBody.onBodyStartGlobal += JamEnemy;
                On.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
            }
        }
        public override void Uninstall()
        {
            base.Uninstall();
            CharacterBody.onBodyStartGlobal -= JamEnemy;
            On.RoR2.CharacterModel.UpdateOverlays -= CharacterModel_UpdateOverlays;
        }

        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);

            if (self && self.body)
            {
                var isJammed = self.body.GetComponent<IsJammed>();
                if (isJammed && !isJammed.Overlay)
                {
                    TemporaryOverlay overlay = self.gameObject.AddComponent<TemporaryOverlay>();
                    overlay.duration = float.PositiveInfinity;
                    overlay.alphaCurve = AnimationCurve.Constant(0f, 0f, 0.54f);
                    overlay.animateShaderAlpha = true;
                    overlay.destroyComponentOnEnd = true;
                    overlay.originalMaterial = Resources.Load<Material>("Materials/matFullCrit");
                    overlay.AddToCharacerModel(self);
                    isJammed.Overlay = overlay;
                }
                return;
            }
        }

        public override void InstallLanguage()
        {
            base.InstallLanguage();
            LanguageAPI.Add(curseTally.nameToken, "Current Curse");
            LanguageAPI.Add(curseTally.descriptionToken, "Affects various factors of the game.");
            LanguageAPI.Add(isJammedItem.nameToken, "ISJAMMED");
            LanguageAPI.Add(isJammedItem.descriptionToken, "Internal used to show this character is Jammed.");
        }

        public override void UninstallLanguage()
        {
            base.UninstallLanguage();
            //??
        }

        private void JamEnemy(CharacterBody obj)
        {
            if (!obj || !obj.inventory || !obj.master) return;
            var inventory = obj.inventory;

            var teamComponent = obj.teamComponent;
            if (!teamComponent) return;

            // Ghosts inherit their previous inventories which include the isJammedItem //
            // So we can just skip the whole section if it's a ghost //
            if (CurseUtil.CheckJammedStatus(obj) && JammedAllowGhost)
            {
                CurseUtil.JamEnemy(obj, 100);
                return;
            }

            CharacterBody mostCursedPlayer = HelperUtil.GetPlayerWithMostItemDef(curseTally);
            if (!mostCursedPlayer) return;
            int PlayerItemCount = mostCursedPlayer.inventory.GetItemCount(curseTally);
            float RollValue;
            float RollValueBosses = 0f;

            var teamIndex = teamComponent.teamIndex;

            switch (PlayerItemCount)
            {
                case 0:
                    RollValue = 0f;
                    break;
                case 1:             //0.5
                case 2:             //1
                case 3:             //1.5
                case 4:             //2
                    RollValue = 1f;
                    break;
                case 5:             //2.5
                case 6:             //3
                case 7:             //3.5
                case 8:             //4
                    RollValue = 2f;
                    break;
                case 9:              //4.5
                case 10:             //5
                case 11:             //5.5
                case 12:             //6
                    RollValue = 5f;
                    break;
                case 13:             //6.5
                case 14:             //7
                case 15:             //7.5
                case 16:             //8
                    RollValue = 10f;
                    RollValueBosses = 20f;
                    break;
                case 17:             //8.5
                case 18:             //9
                case 19:             //9.5
                    RollValue = 25f;
                    RollValueBosses = 30f;
                    break;
                default:             //10 and default
                    RollValue = 50f;
                    RollValueBosses = 50f;
                    break;
            } //adjusts jammed chance

            // NORMAL CHECK //
            if (teamIndex != TeamIndex.Player)
            {
                //BOSS CHECK
                if (obj.isBoss)
                {
                    if (inventory.GetItemCount(umbraItemDef) > 0)
                    {
                        // UMBRA CHECK //
                        if (JammedAllowUmbra)
                        {
                            CurseUtil.JamEnemy(obj, RollValueBosses);
                        }
                    }
                    else
                    {
                        if (JammedAllowBosses)
                        {
                            CurseUtil.JamEnemy(obj, RollValueBosses);
                        }
                    }
                }
                else
                {
                    CurseUtil.JamEnemy(obj, RollValue);
                }
            }
            // AETHERIUM SUPPORT //
            else if (teamIndex == TeamIndex.Player && JammedAllowUnstableDesign)
            {
                bool AetheriumCheck = obj.master.bodyPrefab.name.Contains("UnstableDesignAetherium");
                if (AetheriumCheck)
                {
                    CurseUtil.JamEnemy(obj, RollValue);
                }
            }
        }
        public class IsJammed : MonoBehaviour
        {
            public CharacterBody characterBody;
            public TemporaryOverlay Overlay;
            public GameObject fireEffect;
            public CharacterDeathBehavior characterDeathBehavior;
            
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnEnable()
            {
                //ContactDamageCooldown = ContactDamageCooldownFull;
                characterBody = gameObject.GetComponent<CharacterBody>();
                if (!characterBody)
                {
                    _logger.LogError("No characterbody found for jammed enemy!");
                    return;
                }
                if (!characterBody.HasBuff(Shared.Buffs.BuffsController.Jammed))
                {
                    characterBody.AddBuff(Shared.Buffs.BuffsController.Jammed);
                }
                if (characterBody.inventory && characterBody.inventory.GetItemCount(CurseController.isJammedItem) == 0)
                {
                    characterBody.inventory.GiveItem(CurseController.isJammedItem);
                }
                ModelLocator modelLocator = gameObject.GetComponent<ModelLocator>();
                if (modelLocator)
                {
                    if (!fireEffect)
                    {
                        fireEffect = Object.Instantiate<GameObject>(instance.jammedFire, modelLocator.modelTransform.Find("ROOT").transform);
                        //fireEffect.transform.parent = modelLocator.modelTransform;
                    }
                }
                var deathBehaviour = gameObject.GetComponent<CharacterDeathBehavior>();
                if (deathBehaviour)
                {
                    characterDeathBehavior = deathBehaviour;
                }
            }
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void FixedUpdate()
            {
                if (characterBody && characterBody.healthComponent && characterBody.healthComponent.health <= 0)
                {
                    if (fireEffect)
                    {
                        Destroy(fireEffect);
                        enabled = false;
                    }
                }
            }
        }
    }
}
