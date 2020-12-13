using System.Collections.ObjectModel;
using R2API;
using RoR2;
using UnityEngine;
using TILER2;
using static RiskOfBulletstorm.Items.TrustyLockpicks;
using static RoR2.Highlight;

namespace RiskOfBulletstorm.Items
{
    public class Key : Item_V2<Key>
    {
        public override string displayName => "Key";
        public override ItemTier itemTier => ItemTier.NoTier;
        public override ReadOnlyCollection<ItemTag> itemTags => new ReadOnlyCollection<ItemTag>(new[] { ItemTag.Utility, ItemTag.WorldUnique });

        protected override string GetNameString(string langID = null) => displayName;
        private readonly string desc = $"<style=cIsUtility>Can unlock chests.</style>\nConsumed on use.";
        protected override string GetPickupString(string langID = null) => desc;

        protected override string GetDescString(string langid = null) => desc;

        protected override string GetLoreString(string langID = null) => "";

        private readonly HighlightColor yellow = HighlightColor.interactive;
        private readonly HighlightColor white = HighlightColor.pickup;
        private readonly HighlightColor red = HighlightColor.teleporter;
        //private readonly HighlightColor purple = HighlightColor.unavailable;
        private readonly string prefix = "BLTSTRM_";
        private readonly string suffixBroken = " (Failed Unlock)";
        private readonly string contextKey = "<color=#95ccbd>[Interact] Unlock with Key?</color>\n";
        private readonly string contextLockpicks = "<color=#146dc7>[Equipment] Unlock with Trusty Lockpicks?</color>\n";

        public Key()
        {
            modelResourcePath = "@RiskOfBulletstorm:Assets/Models/Prefabs/Key.prefab";
            iconResourcePath = "@RiskOfBulletstorm:Assets/Textures/Icons/KeyIcon.png";
        }
        public override void SetupBehavior()
        {

        }

        private readonly string[,] chestKeys =
        {
            { "CHEST1_STEALTHED", "Cloaked Chest" },
            { "CHEST1", "Chest" },
            { "CATEGORYCHEST_HEALING", "Chest - Healing" },
            { "CATEGORYCHEST_DAMAGE", "Chest - Damage" },
            { "CATEGORYCHEST_UTILITY", "Chest - Utility" },
            { "CHEST2", "Large Chest" },
            { "GOLDCHEST", "Legendary Chest" },
            { "EQUIPMENTBARREL", "Equipment Barrel" },
        };

        private readonly string[] chestContexts =
        {
            "cloaked chest",
            "chest",
            "Chest - Healing",
            "Chest - Damage",
            "Chest - Utility",
            "large chest",
            "Legendary Chest",
            "equipment barrel"
        };
        private void AddLanguageTokens()
        {
            for (int i = 0; i < chestKeys.GetLength(0)-1; i++)
            {
                //Debug.Log("Language API: Currently on index "+i);
                // broken lock //
                var outputA = prefix + chestKeys[i, 0] + "_NAME";
                var outputB = chestKeys[i, 1] + suffixBroken;
                //Debug.Log("Language API: Using index ["+i+"] attempting to add key "+outputA+" with string "+outputB+"");
                LanguageAPI.Add(prefix + chestKeys[i, 0] + "_NAME", chestKeys[i, 1] + suffixBroken);
                //Debug.Log("Language API: Success!");

                // context strings //
                var contextString = "Open " + chestContexts[i];
                // key //
                LanguageAPI.Add(prefix + chestKeys[i, 0] + "_CONTEXT_KEY", contextKey + contextString);
                // lockpick //
                LanguageAPI.Add(prefix + chestKeys[i, 0] + "_CONTEXT_LOCKPICK", contextLockpicks + contextString);
                // both //
                LanguageAPI.Add(prefix + chestKeys[i, 0] + "_CONTEXT_BOTH", contextKey + contextLockpicks + contextString);
            }
        }

        public override void SetupAttributes()
        {
            base.SetupAttributes();

            AddLanguageTokens();

        }
        public override void SetupConfig()
        {
            base.SetupConfig();
        }
        public override void Install()
        {
            base.Install();
            On.RoR2.Interactor.PerformInteraction += Interactor_PerformInteraction;
            On.RoR2.PurchaseInteraction.GetInteractability += PurchaseInteraction_GetInteractability;
        }

        private Interactability PurchaseInteraction_GetInteractability(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            var gameObject = self.gameObject;
            TrustyLockpicksComponent component = gameObject.GetComponent<TrustyLockpicksComponent>();
            if (!component) component = gameObject.AddComponent<TrustyLockpicksComponent>();
            Highlight highlight = gameObject.GetComponent<Highlight>();
            PurchaseInteraction purchaseInteraction = gameObject.GetComponent<PurchaseInteraction>();
            CharacterBody characterBody = activator.GetComponent<CharacterBody>();
            Interactability Result(HighlightColor highlightColor, string contextTokenType = "", Interactability interactability = Interactability.Available)
            {
                //var resultContext = (prefix + purchaseInteraction.contextToken);
                if (highlight) highlight.highlightColor = highlightColor;
                if (purchaseInteraction)
                {
                    string context = "";
                    switch (contextTokenType)
                    {
                        case "key":
                            context = prefix + purchaseInteraction.contextToken + "_KEY";
                            break;
                        case "lockpick":
                            context = prefix + purchaseInteraction.contextToken + "_LOCKPICK";
                            break;
                        case "both":
                            context = prefix + purchaseInteraction.contextToken + "_BOTH";
                            break;
                        default:
                            context = component.oldContext;
                            break;
                    }

                    if (purchaseInteraction.contextToken == component.oldContext) purchaseInteraction.contextToken = context;
                }
                return interactability;
            }

            // If it's been picked and failed before //
            if (component && component.failed)
            {
                Result(red);
                return orig(self, activator);
            }
            if (characterBody)
            {
                Inventory inventory = characterBody.inventory;
                if (inventory)
                {
                    if (self.isShrine == false && self.available && self.costType == CostTypeIndex.Money) //if not shrine, is available, and is not a lunar pod
                    {
                        bool HasKey = characterBody.inventory.GetItemCount(catalogIndex) > 0;
                        bool LockpicksActive = inventory.GetEquipmentIndex() == TrustyLockpicks.instance.catalogIndex;
                        bool EquipmentReady = inventory.GetEquipmentRestockableChargeCount(0) > 0;
                        bool LockpicksReady = LockpicksActive && EquipmentReady;
                        if (component.oldContext == "") component.oldContext = purchaseInteraction.contextToken;

                        if (HasKey) //a
                        {
                            if (LockpicksReady) return Result(yellow, "both"); //a b : has both
                            else return Result(white, "key");  //a !b : only has key
                        }
                        else //!a
                        {
                            if (LockpicksReady) return Result(yellow, "lockpick"); //!a b : only lockpicks
                            if (component && component.oldContext != "") purchaseInteraction.contextToken = component.oldContext;
                            Result(yellow); //!a !b :has neither
                            return orig(self, activator);
                        }
                    }
                }
            }
            if (component && component.oldContext != "") purchaseInteraction.contextToken = component.oldContext;
            Result(yellow);
            return orig(self, activator);
        }

        public override void Uninstall()
        {
            base.Uninstall();
            On.RoR2.Interactor.PerformInteraction -= Interactor_PerformInteraction;
            On.RoR2.PurchaseInteraction.GetInteractability -= PurchaseInteraction_GetInteractability;
        }

        private void Interactor_PerformInteraction(On.RoR2.Interactor.orig_PerformInteraction orig, Interactor self, GameObject interactableObject)
        {
            PurchaseInteraction purchaseInteraction = interactableObject.GetComponent<PurchaseInteraction>();
            if (purchaseInteraction)
            {
                TrustyLockpicksComponent component = interactableObject.GetComponent<TrustyLockpicksComponent>();
                if (!component || (component && !component.failed))
                {
                    CharacterBody characterBody = self.GetComponent<CharacterBody>();
                    if (characterBody)
                    {
                        Inventory inventory = characterBody.inventory;
                        if (inventory)
                        {
                            int InventoryCount = characterBody.inventory.GetItemCount(catalogIndex);
                            if (InventoryCount > 0)
                            {
                                purchaseInteraction.GetInteractability(self);

                                if (purchaseInteraction.isShrine == false && purchaseInteraction.available && purchaseInteraction.costType == CostTypeIndex.Money) //if not shrine, is available, and is not a lunar pod
                                {
                                    purchaseInteraction.SetAvailable(false);
                                    purchaseInteraction.Networkavailable = false;

                                    purchaseInteraction.gameObject.GetComponent<ChestBehavior>().Open();

                                    //purchaseInteraction.cost = 0;
                                    //purchaseInteraction.Networkcost = 0;

                                    purchaseInteraction.onPurchase.Invoke(self);
                                    purchaseInteraction.lastActivator = self;

                                    inventory.RemoveItem(catalogIndex);
                                }
                            }
                        }
                    }
                }
            }
            orig(self, interactableObject);
        }
    }
}
