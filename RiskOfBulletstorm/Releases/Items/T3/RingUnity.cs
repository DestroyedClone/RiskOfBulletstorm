using System.Collections.ObjectModel;
using RoR2;
using TILER2;
using static TILER2.StatHooks;

//TY Harb

namespace RiskOfBulletstorm.Items
{
    public class Unity : Item_V2<Unity>
    {
        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage increase on single stack? (Default: 0.1)" +
            "\nKeep in mind that this number is MULTIPLIED by the amount of TOTAL items.",
            AutoConfigFlags.PreventNetMismatch)]
        public float RingUnity_DamageBonus { get; private set; } = 0.1f;

        [AutoConfigUpdateActions(AutoConfigUpdateActionTypes.InvalidateLanguage)]
        [AutoConfig("Damage increase on subsequent stacks (Default: 0.01)" +
            "\nKeep in mind that this number is MULTIPLIED by the amount of TOTAL items.", AutoConfigFlags.PreventNetMismatch)]
        public float RingUnity_DamageBonusStack { get; private set; } = 0.01f;
        public override string displayName => "Unity";
        public override ItemTier itemTier => ItemTier.Tier3;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Damage });

        protected override string GetNameString(string langID = null) => displayName;

        protected override string GetPickupString(string langID = null) => "Our Powers Combined\nIncreased combat effectiveness per item.";

        protected override string GetDescString(string langid = null) => $"<style=cIsDamage>+{RingUnity_DamageBonus}base damage</style> per unique item in inventory" +
            $"\n<style=cStack>(+{RingUnity_DamageBonusStack}base per stack)</style>";

        protected override string GetLoreString(string langID = null) => "This ring takes a small amount of power from each gun carried and adds it to the currently equipped gun.";

        //FieldInfo itemStacksField;

        public Unity()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/RingUnity.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/RingUnityIcon.png";
        }

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
            GetStatCoefficients += BoostDamage;
            On.RoR2.Run.Start += Run_Start;
        }

        private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
        {
            orig(self);
            //try caching the item count?
        }

        public override void Uninstall()
        {
            base.Uninstall();
            GetStatCoefficients -= BoostDamage;
            On.RoR2.Run.Start -= Run_Start;
        }

        static int GetUniqueItemCount(CharacterBody characterBody)
        {
            int num = 0;
            ItemIndex itemIndex = ItemIndex.Syringe;
            ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
            while (itemIndex < itemCount)
            {
                if (characterBody.inventory.GetItemCount(itemIndex) > 0)
                    num++;
                itemIndex++;
            }
            return num;
        }


        private void BoostDamage(CharacterBody sender, StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (!inventory) return;
            var UnityInventoryCount = inventory.GetItemCount(catalogIndex);

            if (UnityInventoryCount > 0)
            {
                args.baseDamageAdd += GetUniqueItemCount(sender) * (RingUnity_DamageBonus + (RingUnity_DamageBonusStack * (UnityInventoryCount - 1)));
            }
        }
    }
}
