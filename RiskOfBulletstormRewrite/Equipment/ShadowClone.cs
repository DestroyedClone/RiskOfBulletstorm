using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TextCore;

namespace RiskOfBulletstormRewrite.Equipment
{
    public class ShadowClone : EquipmentBase<ShadowClone>
    {
        public override string EquipmentName => "Shadow Clone";

        public override string EquipmentLangTokenName => "SHADOWCLONE";

        public override GameObject EquipmentModel => Assets.NullModel;

        public override Sprite EquipmentIcon => LoadSprite();

        public static GameObject CashProjectile = null;

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return null;
        }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
            CreateAssets();
        }

        public static void CreateAssets()
        {

        }

        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            var comp = slot.characterBody.master.GetComponent<RiskOfBulletstorm_ShadowCloneOwnerController>();
            if (!comp)
                comp = slot.characterBody.master.gameObject.AddComponent<RiskOfBulletstorm_ShadowCloneOwnerController>();
            comp.ownerMaster = slot.characterBody.master;
            comp.ownerBody = slot.characterBody;
            comp.ownerInputBank = slot.characterBody.inputBank;
            comp.ownerHealthComponent = slot.characterBody.healthComponent;
            comp.ownerInventory = slot.characterBody.inventory;


            slot.subcooldownTimer += 1f;
            return comp.TryAddShadowClone();
        }

        public class RiskOfBulletstorm_ShadowCloneOwnerController : MonoBehaviour
        {
            public List<RiskOfBulletstorm_ShadowCloneChildController> shadowClones = new List<RiskOfBulletstorm_ShadowCloneChildController>();

            public CharacterMaster ownerMaster;
            public CharacterBody ownerBody;
            public InputBankTest ownerInputBank;
            public HealthComponent ownerHealthComponent;
            public Inventory ownerInventory;

            public int maxShadowClones = 0;

            public bool TryAddShadowClone()
            {
                OwnerInventory_onInventoryChanged();
                if (shadowClones.Count < maxShadowClones
                    && ownerBody)
                {
                    GameObject body = BodyCatalog.FindBodyPrefab(ownerBody);
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(body, ownerBody.corePosition, Quaternion.identity);

                    var summonComponent = gameObject.AddComponent<RiskOfBulletstorm_ShadowCloneChildController>();
                    summonComponent.ownerMaster = ownerMaster;
                    summonComponent.ownerBody = ownerBody;
                    summonComponent.ownerInputBank = ownerInputBank;
                    summonComponent.ownerHealthComponent = ownerHealthComponent;

                    summonComponent.myBody = gameObject.GetComponent<CharacterBody>();
                    summonComponent.myHealthComponent = summonComponent.myBody.healthComponent;
                    summonComponent.myInputBank = summonComponent.myBody.inputBank;

                    NetworkServer.Spawn(gameObject);

                    return true;
                }

                return false;
            }


            public void Start()
            {
                ownerMaster.onBodyDestroyed += KillShadowClones;
                ownerInventory.onInventoryChanged += OwnerInventory_onInventoryChanged;
            }

            private void OwnerInventory_onInventoryChanged()
            {
                var equipmentStocks = ownerBody.equipmentSlot.maxStock;
                var diff = equipmentStocks - maxShadowClones;
                if (diff == 0)
                {
                    //notohing changed
                } else if (diff > 0) //stock increased
                {
                } else //stock decreased
                {
                    foreach (var shadowClone in shadowClones)
                    {
                        if (diff != 0)
                        {
                            shadowClone.myHealthComponent.Suicide();
                            diff++;
                            continue;
                        }
                        break;
                    }
                    //kill off remaining amount
                }
                maxShadowClones = equipmentStocks;
            }

            private void KillShadowClones(CharacterBody _)
            {
                foreach (var child in shadowClones)
                {
                    child.myHealthComponent.Suicide();
                }
            }

            public void OnDestroy()
            {
                ownerMaster.onBodyDestroyed -= KillShadowClones;
                ownerInventory.onInventoryChanged -= OwnerInventory_onInventoryChanged;
            }
        }

        public class RiskOfBulletstorm_ShadowCloneChildController : MonoBehaviour
        {
            public CharacterMaster ownerMaster;
            public CharacterBody ownerBody;
            public InputBankTest ownerInputBank;
            public HealthComponent ownerHealthComponent;

            public CharacterBody myBody;
            public InputBankTest myInputBank;
            public HealthComponent myHealthComponent;

            public float duration = 30f;
            public float stopwatch = 0f;

            public void Start()
            {
                On.RoR2.InputBankTest.GetAimRay += InputBankTest_GetAimRay;
                On.RoR2.InputBankTest.CheckAnyButtonDown += InputBankTest_CheckAnyButtonDown;
            }

            public void Update()
            {
                if (ownerInputBank)
                    myInputBank.moveVector = ownerInputBank.moveVector;

                stopwatch += Time.deltaTime;
                if (stopwatch >= duration)
                    myHealthComponent.Suicide();
            }

            private bool InputBankTest_CheckAnyButtonDown(On.RoR2.InputBankTest.orig_CheckAnyButtonDown orig, InputBankTest self)
            {
                if (self == myInputBank
                    && ownerInputBank)
                    return ownerInputBank.CheckAnyButtonDown();

                return orig(self);
            }

            private Ray InputBankTest_GetAimRay(On.RoR2.InputBankTest.orig_GetAimRay orig, InputBankTest self)
            {
                if (self == myInputBank
                    && ownerInputBank)
                {
                    return ownerInputBank.GetAimRay();
                }
                return orig(self);
            }

            public void OnDestroy()
            {
                On.RoR2.InputBankTest.GetAimRay -= InputBankTest_GetAimRay;
                On.RoR2.InputBankTest.CheckAnyButtonDown -= InputBankTest_CheckAnyButtonDown;
            }
        }
    }
}
