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
using static R2API.DirectorAPI;
using static RiskOfBulletstorm.Utils.HelperUtil;
using static RiskOfBulletstorm.Utils.SpawnUtil;

namespace RiskOfBulletstorm.Items
{
    public class BulletstormChestController : Item_V2<BulletstormChestController>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Enable Mimics? Default: True" +
            "\nNote, these aren't related to Hailstorm's mimics", AutoConfigFlags.PreventNetMismatch)]
        public bool BCC_EnableMimics { get; private set; } = true;
        public override string displayName => "BulletstormChestsController";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.WorldUnique, ItemTag.AIBlacklist });

        protected override string GetNameString(string langID = null) => displayName;
        protected override string GetPickupString(string langID = null) => "";

        protected override string GetDescString(string langid = null) => $"";

        protected override string GetLoreString(string langID = null) => "";

        public override void SetupBehavior()
        {
            base.SetupBehavior();
        }
        public override void SetupAttributes()
        {
            base.SetupAttributes();
        }
        public override void SetupLate()
        {
            base.SetupLate();
        }

        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.ChestBehavior.Awake += ChestBehavior_Awake;
            On.RoR2.ChestBehavior.Open += ChestBehavior_Open;
        }

        private void ChestBehavior_Open(On.RoR2.ChestBehavior.orig_Open orig, ChestBehavior self)
        {
            throw new NotImplementedException();
        }

        private void ChestBehavior_Awake(On.RoR2.ChestBehavior.orig_Awake orig, ChestBehavior self)
        {
            orig(self);
            var mimicSpawnChance = GetMimicSpawnChance();
            var gameObject = self.gameObject;
            if (Util.CheckRoll(mimicSpawnChance))
            {
                var component = gameObject.GetComponent<MimicComponent>();
                if (!component) component = gameObject.AddComponent<MimicComponent>();
                component.heldItemIndex = PickupCatalog.GetPickupDef(self.dropPickup).itemIndex;
                self.Invoke("Open", 0);
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.ChestBehavior.Awake -= ChestBehavior_Awake;
        }

        public class MimicComponent : MonoBehaviour
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnEnable()
            {
                healthComponent = gameObject.AddComponent<HealthComponent>();

                healthComponent.health = 1;
                healthComponent.dontShowHealthbar = true;
                characterBody = gameObject.AddComponent<CharacterBody>();
                characterBody.

            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "UnityEngine")]
            void OnDisable()
            {

            }

            public ItemIndex heldItemIndex = ItemIndex.AACannon;
            public HealthComponent healthComponent;
            public CharacterBody characterBody;
        }
    }
}
