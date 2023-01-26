using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class IronCoin : EquipmentBase<IronCoin>
    {
        //public static ConfigEntry<float> cfg;

        public override string EquipmentName => "Iron Coin";

        public override string EquipmentLangTokenName => "IRONCOIN";

        public override GameObject EquipmentModel => LoadModel();

        public override Sprite EquipmentIcon => LoadSprite();

        public override bool IsLunar => true;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected override void CreateConfig(ConfigFile config)
        {
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            int enemiesKilled = 0;
            var attackerGameObject = slot.characterBody.gameObject;
            foreach (var charMaster in CharacterMaster.readOnlyInstancesList)
            {
                if (charMaster.playerCharacterMasterController)
                    continue;

                var body = charMaster.GetBody();
                if (charMaster.inventory && body && body.healthComponent && !body.isBoss && body.healthComponent.alive && charMaster.teamIndex != slot.teamComponent.teamIndex)
                {
                    enemiesKilled++;

                    if (body.TryGetComponent<DeathRewards>(out DeathRewards deathRewards))
                    {
                        deathRewards.goldReward = 0;
                        deathRewards.expReward = 0;
                        deathRewards.bossDropTable = null;
                    }
                    if (body.isElite && body.TryGetComponent(out EquipmentSlot equipmentSlot))
                    {
                        equipmentSlot.equipmentIndex = EquipmentIndex.None;
                    }
                    var itemCount = charMaster.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger);
                    if (itemCount > 0)
                    {
                        charMaster.inventory.RemoveItem(RoR2Content.Items.InvadingDoppelganger, itemCount);
                    }
                    //var bossGroups = InstanceTracker.GetInstancesList<BossGroup>();
                    /*foreach (var bossGroup in bossGroups)
                    {
                        bossGroup.bossMemories[0].
                    }*/
                    charMaster.TrueKill(attackerGameObject, attackerGameObject, DamageType.Generic);
                }
            }
            if (enemiesKilled > 0)
            {
                CharacterMasterNotificationQueue.PushEquipmentTransformNotification(slot.characterBody.master, slot.characterBody.inventory.currentEquipmentIndex, IronCoinConsumed.instance.EquipmentDef.equipmentIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                slot.inventory.SetEquipmentIndex(IronCoinConsumed.instance.EquipmentDef.equipmentIndex);
                return true;
            }
            return false;
        }
    }
}