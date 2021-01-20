using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using static R2API.Utils.CommandHelper;
using R2API.Utils;
using RoR2;
using UnityEngine;
using EntityStates;

namespace RiskOfBulletstorm
{
    public static class ConsoleCommands
    {
        [ConCommand(commandName = "ROB_deathstateclear", flags = ConVarFlags.ExecuteOnServer, helpText = "Destroys your deathstate so that your corpse keeps the idleanims.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Empty Arg required")]
        private static void DeathStateClear(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            GameObject bodyInstanceObject = localMaster.bodyInstanceObject;
            var deathstate = bodyInstanceObject.GetComponent<CharacterDeathBehavior>();
            if (deathstate) deathstate.deathState = new SerializableEntityStateType();
        }

        [ConCommand(commandName = "ROB_toggleview", flags = ConVarFlags.ExecuteOnServer, helpText = "Draws a line from where you're aiming towards.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Empty Arg required")]
        private static void DrawViewLine(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            GameObject bodyInstanceObject = localMaster.bodyInstanceObject;
            CharacterBody characterBody = bodyInstanceObject.GetComponent<CharacterBody>();
            LaserPointerController laserPointerController = bodyInstanceObject.GetComponent<LaserPointerController>();
            if (laserPointerController)
            {
                UnityEngine.Object.Destroy(laserPointerController);
                UnityEngine.Object.Destroy(bodyInstanceObject.GetComponent<LaserPointer>());
            } else
            {
                laserPointerController = bodyInstanceObject.AddComponent<LaserPointerController>();
                LaserPointer laserPointer = bodyInstanceObject.AddComponent<LaserPointer>();
                laserPointer.laserDistance = 10000f;
                laserPointerController.source = characterBody.inputBank;
                laserPointerController.beam = laserPointer.line;
            }
        }
        [ConCommand(commandName = "ROB_godark", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets equipment slot to specified index.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Empty Arg required")]
        private static void GoDark(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var cb = localMaster.bodyInstanceObject.GetComponent<CharacterBody>();
            if (cb)
            {
                if (cb.HasBuff(BuffIndex.Cloak))
                    cb.RemoveBuff(BuffIndex.Cloak);
                else
                    cb.AddBuff(BuffIndex.Cloak);
            }
        }
        [ConCommand(commandName = "ROB_teleport", flags = ConVarFlags.ExecuteOnServer, helpText = "Teleport to specified coords. [x] [y] [z]")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Empty Arg required")]
        private static void TeleportPos(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var body = localMaster.bodyInstanceObject;
            var cb = body.GetComponent<CharacterBody>();
            var rbm = body.GetComponent<RigidbodyMotor>();
            var position = new Vector3(args.GetArgFloat(0), args.GetArgFloat(1), args.GetArgFloat(2));
            if (cb)
            {
                if (cb.characterMotor)
                {
                    Debug.Log("Teleported charactermotor");
                    cb.characterMotor.Motor.SetPositionAndRotation(position, Quaternion.identity, true);
                }
                else if (rbm)
                {
                    Debug.Log("Teleported rigidbody");
                    rbm.rigid.position = position;
                }
            }
        }

        [ConCommand(commandName = "ROB_list_equipment", flags = ConVarFlags.None, helpText = "Prints the equipment equipped.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Empty Arg required")]
        private static void ListPlayerEquipment(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var inventory = localMaster.inventory;
            if (inventory)
            {
                var equipmentStateSlots = inventory.equipmentStateSlots;
                var length = equipmentStateSlots.Length;
                if (length > 0)
                {
                    for (int i = 0; i <= length; i++)
                    {
                        var eqpName = "None";
                        var charges = -6;
                        var cooldown = -7;
                        if (i < equipmentStateSlots.Length) //prevents out of bounds error from unset slots
                        {
                            var eqp = equipmentStateSlots[i];
                            if (eqp.equipmentIndex != EquipmentIndex.None)
                            {
                                eqpName = eqp.equipmentDef.name;
                            }
                            charges = eqp.charges;
                            cooldown = eqp.isPerfomingRecharge ? Mathf.Max((int)eqp.chargeFinishTime.timeUntil, 0) : cooldown;
                        }
                        // Slot 0: "[1] Bomb 5x CD:10"
                        Debug.Log(
                            "[" + (i) + "] " +
                            eqpName +
                            (charges == -6 ? "" : " " + charges + "x") +
                            (cooldown == -7 ? "" : " CD:" + cooldown + " ")
                            );
                    }
                }
            }
        }

        [ConCommand(commandName = "ROB_set_equipment_slot", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets equipment slot to specified index.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        private static void SetEquipmentSlot(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var inventory = localMaster.inventory;
            if (inventory)
            {
                var value = (byte)args.GetArgInt(0);
                inventory.SetActiveEquipmentSlot(value);
            }
        }

        [ConCommand(commandName = "ROB_set_equipment_slot_safe", flags = ConVarFlags.ExecuteOnServer, helpText = "Sets equipment slot to specified index. Bounded by backpack..")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        private static void SetEquipmentSlotSafe(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var inventory = localMaster.inventory;
            var component = localMaster.GetComponent<Items.Backpack.BackpackComponent>();
            if (inventory && component)
            {
                var value = (byte)args.GetArgInt(0);
                inventory.SetActiveEquipmentSlot(Math.Min(component.selectableSlot, value));
            }
        }

        [ConCommand(commandName = "ROB_target_enable", flags = ConVarFlags.ExecuteOnServer, helpText = "Allow yourself to use the target commands")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void TargetToggle(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var bodyObject = localMaster.GetBodyObject();
            if (bodyObject)
            {
                var component = bodyObject.GetComponent<ROBConsoleCommand>();
                if (!component)
                {
                    bodyObject.AddComponent<ROBConsoleCommand>();
                    Debug.Log("Gave component!");
                    return;
                }
            }
        }

        private static ROBConsoleCommand HasComponent(CharacterMaster characterMaster)
        {
            var bodyObject = characterMaster.GetBodyObject();
            if (bodyObject)
            {
                var component = bodyObject.GetComponent<ROBConsoleCommand>();
                if (!component)
                {
                    Debug.LogError("Player is missing component. Do ROB_target_component, and try again.");
                    return null;
                }
                return component;
            }
            Debug.LogError("Player's body object is missing.");
            return null;
        }

        [ConCommand(commandName = "ROB_target_hit", flags = ConVarFlags.ExecuteOnServer, helpText = "Call the command then attack an enemy to mark them for targeted commands. Run again to stop.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void TargetChoose(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component)
            {
                var state = component.GetState();
                if (state != 1)
                {
                    component.SetState(1);
                    Debug.Log("Attack an enemy to target them. Run the command again to cancel.");
                    On.RoR2.HealthComponent.TakeDamage += TargetEnemyHook;
                    return;
                }
                component.SetState(0);
                On.RoR2.HealthComponent.TakeDamage -= TargetEnemyHook;
                Debug.Log("No longer targeting.");
                return;
            }
        }

        private static void TargetEnemyHook(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            if (damageInfo.attacker.gameObject == localMaster.GetBodyObject())
            {

                var component = HasComponent(localMaster);
                if (component)
                {
                    damageInfo.rejected = true;
                    damageInfo.procCoefficient = -1;
                    damageInfo.crit = false;
                    component.SetBody(self.body);
                    Chat.AddMessage("Body assigned to " + self.body.GetDisplayName());
                }
            }
            orig(self, damageInfo);
        }

        [ConCommand(commandName = "ROB_target_give_item", flags = ConVarFlags.ExecuteOnServer, helpText = "Gives the target item(s). Syntax: [itemindex] [amount]")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        private static void TargetGiveItem(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component && component.HasBody())
            {
                var inventory = component.targetedBody.inventory;
                if (inventory)
                { //https://stackoverflow.com/questions/23563960/how-to-get-enum-value-by-string-or-int
                    ItemIndex itemIndex = (ItemIndex)args.GetArgInt(0);
                    int itemCount = args.GetArgInt(1);

                    inventory.GiveItem(itemIndex, itemCount);
                    Chat.AddMessage("Gave "+itemIndex+" x"+itemCount+" to "+component.targetedBody.GetDisplayName());
                }
            }
        }

        [ConCommand(commandName = "ROB_target_remove_item", flags = ConVarFlags.ExecuteOnServer, helpText = "Removes the target item(s). Syntax: [itemindex] [amount]")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        private static void TargetRemoveItem(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component && component.HasBody())
            {
                var inventory = component.targetedBody.inventory;
                if (inventory)
                { //https://stackoverflow.com/questions/23563960/how-to-get-enum-value-by-string-or-int
                    ItemIndex itemIndex = (ItemIndex)args.GetArgInt(0);
                    var targetItemCount = inventory.GetItemCount(itemIndex);

                    int itemCount = args.GetArgInt(1);
                    if (itemCount < 0)
                    {
                        inventory.RemoveItem(itemIndex, targetItemCount);
                        Chat.AddMessage("Removed " + itemIndex + " x" + itemCount + " from " + component.targetedBody.GetDisplayName());
                    } else
                    {
                        var amountToRemove = Mathf.Max(itemCount, targetItemCount);
                        inventory.RemoveItem(itemIndex, amountToRemove);
                        Chat.AddMessage("Removed " + itemIndex + " x" + amountToRemove + " from " + component.targetedBody.GetDisplayName());
                    }
                }
            }
        }

        [ConCommand(commandName = "ROB_target_give_equip", flags = ConVarFlags.ExecuteOnServer, helpText = "Gives the target item(s). Syntax: [itemindex] [amount]")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        private static void TargetGiveEquip(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component && component.HasBody())
            {
                var inventory = component.targetedBody.inventory;
                if (inventory)
                { //https://stackoverflow.com/questions/23563960/how-to-get-enum-value-by-string-or-int
                    EquipmentIndex equipmentIndex = (EquipmentIndex)args.GetArgInt(0);

                    inventory.SetEquipmentIndex(equipmentIndex);
                    Chat.AddMessage("Gave " + equipmentIndex + " to " + component.targetedBody.GetDisplayName());
                }
            }
        }


        [ConCommand(commandName = "ROB_target_inventory", flags = ConVarFlags.ExecuteOnServer, helpText = "Prints their inventory.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void TargetCheckItems(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component && component.HasBody())
            {
                var inventory = component.targetedBody.inventory;
                if (inventory)
                { //https://stackoverflow.com/questions/23563960/how-to-get-enum-value-by-string-or-int
                    var ChatQueue = component.targetedBody.GetDisplayName() + "'s inventory:\n";
                    ItemIndex itemIndexIterate = ItemIndex.Syringe;
                    ItemIndex itemCountIterate = (ItemIndex)ItemCatalog.itemCount;
                    while (itemIndexIterate < itemCountIterate)
                    {
                        var itemCount = inventory.GetItemCount(itemIndexIterate);
                        if (itemCount > 0)
                            ChatQueue += itemIndexIterate + " x" + itemCount + "\n";
                        itemIndexIterate++;
                    }

                    Debug.Log(ChatQueue);
                }
            }
        }

        [ConCommand(commandName = "ROB_target_kill", flags = ConVarFlags.ExecuteOnServer, helpText = "Calls the targets healthcomponent to suicide")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        private static void TargetSuicide(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component && component.HasBody())
            {
                var healthComponent = component.targetedBody.healthComponent;
                if (healthComponent)
                {
                    healthComponent.Suicide();
                    Debug.Log("Suicided target");
                }
            }
        }

        [ConCommand(commandName = "ROB_target_takedamage", flags = ConVarFlags.ExecuteOnServer, helpText = "[damage] [isCrit] [rejected] [damageType]")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Console Command")]
        private static void TargetTakeDamage(ConCommandArgs args)
        {
            var localMaster = PlayerCharacterMasterController.instances[0].master;
            var component = HasComponent(localMaster);
            if (component && component.HasBody())
            {
                var healthComponent = component.targetedBody.healthComponent;
                if (healthComponent)
                {

                    healthComponent.TakeDamage(new DamageInfo
                    {
                        damage = args.GetArgInt(0),
                        crit = args.GetArgBool(1),
                        rejected = args.GetArgBool(2),
                        damageType = (DamageType)args.GetArgInt(3)
                    });
                }
            }
        }

        public class ROBConsoleCommand : MonoBehaviour
        {
            byte state = 0;
            public CharacterBody targetedBody = null;

            public void SetState(byte value)
            {
                state = value;
            }
            public byte GetState()
            {
                return state;
            }
            public void SetBody(CharacterBody characterBody)
            {
                targetedBody = characterBody;
            }
            public bool HasBody()
            {
                if (targetedBody && targetedBody != null)
                {
                    return true;
                }
                Debug.LogError("Body not found!.");
                return false;
            }
        }

        public class LookLaser : MonoBehaviour
        {

        }
    }
}
