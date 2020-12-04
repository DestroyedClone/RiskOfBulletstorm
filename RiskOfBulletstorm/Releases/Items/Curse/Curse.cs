//using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.Text;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;
using TILER2;
using static TILER2.StatHooks;
using static TILER2.MiscUtil;
using System;
using RiskOfBulletstorm.Utils;

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
        [AutoConfig("How much additional damage should a Jammed enemy deal? (Default: 1 (+100%))", AutoConfigFlags.PreventNetMismatch)]
        public float Curse_DamageBoost { get; private set; } = 1.00f;
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("How much additional crit should a Jammed enemy have? (Default: 1 (+100%))", AutoConfigFlags.PreventNetMismatch)]
        public float Curse_CritBoost { get; private set; } = 1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow bosses to become Jammed? Default: true", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_AllowBosses { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow umbrae to become Jammed? Default: true", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_AllowUmbra { get; private set; } = true;

        /*[AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow Happiest Mask ghosts to retain their Jammed status?", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_AllowGhosts { get; private set; } = true;*/

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("[Aetherium Support] Allow Unstable Design spawns to become jammed?", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_AllowUnstableDesign { get; private set; } = true;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Allow Lord of the Jammed to spawn at Max curse? Default: true", AutoConfigFlags.PreventNetMismatch)]
        public bool Curse_SpawnLOTJ { get; private set; } = true;

        public override string displayName => "CurseMasterItem";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.AIBlacklist, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        //public GameObject curseEffect = Resources.Load<GameObject>("prefabs/effects/ImpSwipeEffect");

        public static ItemIndex curseTally;
        public static ItemIndex curseMax;

        public static readonly ItemIndex umbraItemIndex = ItemIndex.InvadingDoppelganger;

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();

            var curseTallyDef = new CustomItem(new ItemDef
            {
                hidden = true,
                name = "ROBInternalCurseTally",
                tier = ItemTier.NoTier,
                canRemove = false
            }, new ItemDisplayRuleDict(null));
            curseTally = ItemAPI.Add(curseTallyDef);

            var curseMaxDef = new CustomItem(new ItemDef
            {
                hidden = true,
                name = "ROBInternalCurseSpawnLOTJ",
                tier = ItemTier.NoTier,
                canRemove = false
            }, new ItemDisplayRuleDict(null));
            curseMax = ItemAPI.Add(curseMaxDef);
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
            var teamComponent = obj.teamComponent;
            if (!teamComponent) return;

            CharacterBody mostCursedPlayer = HelperUtil.GetPlayerWithMostItemIndex(curseTally);
            int PlayerItemCount = mostCursedPlayer.inventory.GetItemCount(curseTally);
            float RollValue = 0f;
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
                    var inventory = obj.inventory;

                    if (inventory)
                    {
                        if (inventory.GetItemCount(umbraItemIndex) > 0)
                        {
                            // UMBRA CHECK START //
                            if (Curse_AllowUmbra)
                            {
                                CurseUtil.JamEnemy(obj, RollValueBosses);
                            }
                        }
                        //UMBRA CHECK END //
                        else
                        {
                            if (Curse_AllowBosses)
                            {
                                CurseUtil.JamEnemy(obj, RollValueBosses);
                            }
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
                    Chat.AddMessage("Chimera Success!");
                    CurseUtil.JamEnemy(obj, RollValue);
                }
            }
        }
    }
}
