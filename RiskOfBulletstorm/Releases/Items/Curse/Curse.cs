using System.Collections.ObjectModel;
using R2API;
using RoR2;
using TILER2;
using RiskOfBulletstorm.Utils;
using UnityEngine;

namespace RiskOfBulletstorm.Items
{
    public class CurseController : Item_V2<CurseController>
    {
        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("How many curse is needed before Lord of the Jammed spawns? Set it to -1 to disable. (Default: 10)", AutoConfigFlags.PreventNetMismatch)]
        //public float Curse_LOTJAmount { get; private set; } = 10f;

        //[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        //[AutoConfig("Enable Jammed enemies?", AutoConfigFlags.PreventNetMismatch)]
        //public bool Curse_Enable { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Should the stacks of Curse be shown in the inventory?", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_Show { get; private set; } = true;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional damage should a Jammed enemy deal? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Curse_DamageBoost { get; private set; } = 1.00f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional crit should a Jammed enemy have? (Value: Additive)", AutoConfigFlags.PreventNetMismatch)]
        public float Curse_CritBoost { get; private set; } = 100f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional attack speed should a Jammed enemy have? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Curse_AttackSpeedBoost { get; private set; } = 0.2f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional move speed should a Jammed enemy have? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Curse_MoveSpeedBoost { get; private set; } = 0.2f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional health should a Jammed enemy have? (Value: Additive Percentage)", AutoConfigFlags.PreventNetMismatch)]
        public float Curse_HealthBoost { get; private set; } = 0.5f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow bosses to become Jammed?", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_AllowBosses { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow umbra(e) to become Jammed?", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_AllowUmbra { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow Happiest Mask ghosts to retain their Jammed status?", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_AllowGhost { get; private set; } = false;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("[Aetherium Support] Allow Unstable Design summons to become Jammed?", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_AllowUnstableDesign { get; private set; } = true;

        public override string displayName => "CurseMasterItem";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        //public GameObject curseEffect = Resources.Load<GameObject>("prefabs/effects/ImpSwipeEffect");

        public static ItemIndex curseTally;
        //public static ItemIndex curseMax;
        public static ItemIndex isJammedItem;

        public static readonly ItemIndex umbraItemIndex = ItemIndex.InvadingDoppelganger;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

            // Used to keep track of the player's curse per player //
            var curseTallyDef = new CustomItem(new ItemDef
            {
                hidden = !Curse_Show,
                name = "CurseTally",
                tier = ItemTier.NoTier,
                canRemove = false
            }, new ItemDisplayRuleDict(null));
            curseTally = ItemAPI.Add(curseTallyDef);

            // This way allows ghosts to maintain their curse status //
            var isJammedDef = new CustomItem(new ItemDef
            {
                hidden = true,
                name = "ROBInternalIsJammed",
                tier = ItemTier.NoTier,
                canRemove = false
            }, new ItemDisplayRuleDict(null));
            isJammedItem = ItemAPI.Add(isJammedDef);

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
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            CharacterBody.onBodyStartGlobal += JamEnemy;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            CharacterBody.onBodyStartGlobal -= JamEnemy;
        }

        private void JamEnemy(CharacterBody obj)
        {
            if (!obj || !obj.inventory || !obj.master) return;
            var inventory = obj.inventory;

            var teamComponent = obj.teamComponent;
            if (!teamComponent) return;

            // Ghosts inherit their previous inventories which include the isJammedItem //
            // So we can just skip the whole section if it's a ghost //
            if (CurseUtil.CheckJammedStatus(obj) && Curse_AllowGhost)
            {
                CurseUtil.JamEnemy(obj, 100);
                return;
            }

            CharacterBody mostCursedPlayer = HelperUtil.GetPlayerWithMostItemIndex(curseTally);
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

                    if (inventory.GetItemCount(umbraItemIndex) > 0)
                    {
                        // UMBRA CHECK //
                        if (Curse_AllowUmbra)
                        {
                            CurseUtil.JamEnemy(obj, RollValueBosses);
                        }
                    }
                    else
                    {
                        if (Curse_AllowBosses)
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
            else if (teamIndex == TeamIndex.Player && Curse_AllowUnstableDesign)
            {
                bool AetheriumCheck = obj.master.bodyPrefab.name.Contains("Aetherium");
                if (AetheriumCheck)
                {
                    CurseUtil.JamEnemy(obj, RollValue);
                }
            }
        }
        public class IsJammed : MonoBehaviour
        {
            public CharacterBody characterBody;
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnEnable()
            {
                //ContactDamageCooldown = ContactDamageCooldownFull;
                characterBody = gameObject.GetComponent<CharacterBody>();
                if (!characterBody.HasBuff(Shared.Buffs.BuffsController.Jammed))
                {
                    characterBody.AddBuff(Shared.Buffs.BuffsController.Jammed);
                }
                if (characterBody.inventory && characterBody.inventory.GetItemCount(CurseController.isJammedItem) == 0)
                {
                    characterBody.inventory.GiveItem(CurseController.isJammedItem);
                }
            }
        }
    }
}
