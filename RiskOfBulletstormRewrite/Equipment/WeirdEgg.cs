using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class WeirdEgg : EquipmentBase<WeirdEgg>
    {
        public static ConfigEntry<float> cfg;

        public override string EquipmentName => "Weird Egg";

        public override string EquipmentLangTokenName => "WEIRDEGG";

        public override string[] EquipmentFullDescriptionParams => new string[]
        {

        };

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        /*  Needs to be able to be dropped, so install YEET? or implement our own? 
        *   1. When Shot: Drop item based on carry time vs chamber time?
        *   2. When dropped into fire source: fire source vs fire weapon?
            3. Needs to be fed -> transform into snake or use modelswap ES?
            4. pick up as item, disable item drop for weird egg afterwards.

            Requirement: CharacterBody (pot?) w/ custom entitystates
        */  

        protected override void CreateConfig(ConfigFile config)
        {

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            if (slot?.characterBody?.healthComponent?.health < slot.characterBody.healthComponent.fullHealth && Util.HasEffectiveAuthority(slot.gameObject))
            {
                slot.characterBody.healthComponent.CallCmdHealFull();

                slot.inventory.SetEquipmentIndex(EquipmentIndex.None);
                CharacterMasterNotificationQueue.PushEquipmentTransformNotification(slot.characterBody.master, WeirdEgg.instance.EquipmentDef.equipmentIndex, EquipmentIndex.None, CharacterMasterNotificationQueue.TransformationType.Default);
                return true;
            }
            return false;
        }


    }
}
