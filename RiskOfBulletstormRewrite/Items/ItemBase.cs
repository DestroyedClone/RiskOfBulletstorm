﻿using BepInEx.Configuration;
using R2API;
using RiskOfBulletstormRewrite.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Items
{
    // The directly below is entirely from TILER2 API (by ThinkInvis) specifically the Item module. Utilized to implement instancing for classes.
    // TILER2 API can be found at the following places:
    // https://github.com/ThinkInvis/RoR2-TILER2
    // https://thunderstore.io/package/ThinkInvis/TILER2/

    public abstract class ItemBase<T> : ItemBase where T : ItemBase<T>
    {
        //This, which you will see on all the -base classes, will allow both you and other modders to enter through any class with this to access internal fields/properties/etc as if they were a member inheriting this -Base too from this class.
        public static T instance { get; private set; }

        public ItemBase()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBase was instantiated twice");
            instance = this as T;
        }
    }

    public abstract class ItemBase
    {
        public abstract string ItemName { get; }
        public abstract string ItemLangTokenName { get; }
        public virtual string[] ItemPickupDescParams { get; }
        public virtual string[] ItemFullDescriptionParams { get; }
        public virtual string[] ItemLogbookDescriptionParams { get; }

        public virtual bool ItemDescriptionLogbookOverride { get; } = false;

        public abstract ItemTier Tier { get; }
        public virtual ItemTag[] ItemTags { get; set; } = new ItemTag[] { };

        //like lunars or not
        public virtual bool CanBePickedUp { get; } = true;

        public abstract GameObject ItemModel { get; }
        public abstract Sprite ItemIcon { get; }

        public ItemDef ItemDef;

        public virtual bool CanRemove { get; } = true;
        public virtual bool IsSkillReplacement { get; } = false;

        public virtual bool AIBlacklisted { get; set; } = false;

        //public virtual RoR2.ExpansionManagement.ExpansionDef ExpansionDef { get; } = null;

        public virtual ItemDef ContagiousOwnerItemDef { get; } = null;

        public virtual string ParentEquipmentName { get; } = null;
        public virtual string ParentItemName { get; } = null;

        public virtual bool Hidden { get; } = false;

        public string ConfigCategory
        {
            get
            {
                return "Item: " + ItemName;
            }
        }

        public string ItemPickupToken
        {
            get
            {
                return "RISKOFBULLETSTORM_ITEM_" + ItemLangTokenName + "_PICKUP";
            }
        }

        public string ItemDescriptionToken
        {
            get
            {
                return "RISKOFBULLETSTORM_ITEM_" + ItemLangTokenName + "_DESCRIPTION";
            }
        }

        public virtual string ItemDescriptionLogbookToken
        {
            get
            {
                return "RISKOFBULLETSTORM_ITEM_" + ItemLangTokenName + "_LOGBOOK_DESCRIPTION";
            }
        }

        public virtual string WikiLink { get; }

        /// <summary>
        /// This method structures your code execution of this class. An example implementation inside of it would be:
        /// <para>CreateConfig(config);</para>
        /// <para>CreateLang();</para>
        /// <para>CreateItem();</para>
        /// <para>Hooks();</para>
        /// <para>This ensures that these execute in this order, one after another, and is useful for having things available to be used in later methods.</para>
        /// <para>P.S. CreateItemDisplayRules(); does not have to be called in this, as it already gets called in CreateItem();</para>
        /// </summary>
        /// <param name="config">The config file that will be passed into this from the main class.</param>
        public abstract void Init(ConfigFile config);

        public virtual void CreateConfig(ConfigFile config)
        { }

        protected virtual void CreateLang() //create lang (addtokens for nwo) -> modify lang (this will be kept later)
        {
            //Main._logger.LogMessage($"{ItemName} CreateLang()");
            bool formatPickup = ItemPickupDescParams?.Length > 0;
            //Main._logger.LogMessage("pickupCheck");
            bool formatDescription = ItemFullDescriptionParams?.Length > 0; //https://stackoverflow.com/a/41596301
            //Main._logger.LogMessage("descCheck");
            bool formatLogbook = ItemLogbookDescriptionParams?.Length > 0;
            if (!formatDescription && !formatPickup && !formatLogbook)
            {
                //Main._logger.LogMessage("Nothing to format.");
                return;
            }

            if (formatPickup)
            {
                LanguageOverrides.DeferToken(ItemPickupToken, ItemPickupDescParams);
            }

            if (formatDescription)
            {
                LanguageOverrides.DeferToken(ItemDescriptionToken, ItemFullDescriptionParams);
            }

            if (formatLogbook)
            {
                LanguageOverrides.DeferToken(ItemDescriptionLogbookToken, ItemLogbookDescriptionParams);
            }

            if (ItemDescriptionLogbookOverride || formatLogbook)
            {
                LanguageOverrides.logbookTokenOverrideDict.Add(ItemDescriptionToken, ItemDescriptionLogbookToken);
            }
            /*
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_NAME", ItemName);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_PICKUP", ItemPickupDesc);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_DESCRIPTION", ItemFullDescription);
            LanguageAPI.Add("ITEM_" + ItemLangTokenName + "_LORE", ItemLore);*/
        }

        public string GetChance(ConfigEntry<float> configEntry)
        {
            return ToPct(configEntry.Value);
        }

        public string ToPct(float value)
        {
            return (value * 100).ToString();
        }

        public GameObject ItemBodyModelPrefab;

        public abstract ItemDisplayRuleDict CreateItemDisplayRules();

        public static ItemTierDef ResolveTierDef(ItemTier itemTier)
        {
            return Modules.Assets.ResolveTierDef(itemTier);
        }

        protected void CreateItem()
        {
            if (AIBlacklisted)
            {
                ItemTags = new List<ItemTag>(ItemTags) { ItemTag.AIBlacklist }.ToArray();
            }

            var prefix = "RISKOFBULLETSTORM_ITEM_";
            ItemDef = ScriptableObject.CreateInstance<ItemDef>();
            ItemDef.name = prefix + ItemLangTokenName;
            ItemDef.nameToken = prefix + ItemLangTokenName + "_NAME";
            ItemDef.pickupToken = prefix + ItemLangTokenName + "_PICKUP";
            ItemDef.descriptionToken = prefix + ItemLangTokenName + "_DESCRIPTION";
            ItemDef.loreToken = prefix + ItemLangTokenName + "_LORE";
            ItemDef.pickupModelPrefab = ItemModel;
            ItemDef.pickupIconSprite = ItemIcon;
            ItemDef.hidden = false;
            ItemDef.canRemove = CanRemove;
            //ItemDef.tier = ResolveTierDef(Tier).tier;

            ItemDef.deprecatedTier = Tier;
            if (Tier == ItemTier.VoidTier1
            || Tier == ItemTier.VoidTier2
            || Tier == ItemTier.VoidTier3
            || Tier == ItemTier.VoidBoss)
            {
                ItemDef.requiredExpansion = Utils.ItemHelpers.GetSOTVExpansionDef();
                if (!ContagiousOwnerItemDef)
                {
                    Main._logger.LogWarning($"Void Item {ItemDef.name} does not have an associated ItemDef to convert from!");
                }
                else
                {
                    Main.voidConversions.Add(ContagiousOwnerItemDef, ItemDef);
                }
            }

            ItemDef.hidden = Hidden;

            if (ItemTags.Length > 0) { ItemDef.tags = ItemTags; }
            if (!CanBePickedUp) RiskOfBulletstormRewrite.Main.itemDefsThatCantBeAutoPickedUp.Add(ItemDef);

            ItemAPI.Add(new CustomItem(ItemDef, CreateItemDisplayRules()));
        }

        public virtual void Hooks()
        { }

        //Based on ThinkInvis' methods
        public int GetCount(CharacterBody body)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCount(ItemDef);
        }

        public int GetCount(CharacterMaster master)
        {
            if (!master || !master.inventory) { return 0; }

            return master.inventory.GetItemCount(ItemDef);
        }

        public int GetCountSpecific(CharacterBody body, ItemDef itemDef)
        {
            if (!body || !body.inventory) { return 0; }

            return body.inventory.GetItemCount(itemDef);
        }

        public float GetStack(float initialValue, float stackValue, int itemCount)
        {
            return initialValue + stackValue * (itemCount - 1);
        }

        public float GetStack(ConfigEntry<float> initialValue, ConfigEntry<float> stackValue, int itemCount)
        {
            return GetStack(initialValue.Value, stackValue.Value, itemCount);
        }

        public Sprite LoadSprite(string itemNameToken = "")
        {
            var token = itemNameToken == "" ? ItemLangTokenName : itemNameToken;
            return Modules.Assets.LoadSprite($"ITEM_{token}");
        }

        public GameObject LoadModel(string itemNameToken = "")
        {
            var token = itemNameToken == "" ? ItemLangTokenName : itemNameToken;
            return Modules.Assets.LoadObject($"{token}.prefab");
        }

        public static implicit operator ItemBase(Type v)
        {
            return (ItemBase)System.Activator.CreateInstance(v);
        }

        public ItemTag[] PlayerOnlyItemTags = new ItemTag[]
        {
            ItemTag.WorldUnique,
            ItemTag.CannotDuplicate,
            ItemTag.AIBlacklist,
            ItemTag.CannotCopy,
            ItemTag.CannotSteal,
            ItemTag.BrotherBlacklist
        };
    }
}