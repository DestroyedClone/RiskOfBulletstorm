using RiskOfBulletstormRewrite.Equipment;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace RiskOfBulletstormRewrite
{
    public static class Commands
    {
        public static void Initialize()
        {
        }

        /*[ConCommand(commandName = "rbs_givebulletparts",
        flags = ConVarFlags.SenderMustBeServer,
        helpText = "rbs_givebulletparts - Gives the bullet parts.")]
        public static void CCGiveBulletParts(ConCommandArgs args)
        {
            var master = args.senderMaster;
            if (args.senderMaster && args.senderMaster.inventory)
            {
                args.senderMaster.inventory.GiveItem(ArcaneGunpowder.instance.ItemDef);
                args.senderMaster.inventory.GiveItem(ObsidianShellCasing.instance.ItemDef);
                args.senderMaster.inventory.GiveItem(PlanarLead.instance.ItemDef);
                args.senderMaster.inventory.GiveItem(PrimePrimer.instance.ItemDef);
            }
        }*/


        [ConCommand(commandName = "rbs_spawnprefab",
        flags = ConVarFlags.ExecuteOnServer,
        helpText = "rbs_spawnprefab path x y z")]
        public static void CCSpawnPrefab(ConCommandArgs args)
        {
            var reference = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(args.GetArgString(0)).WaitForCompletion();
            var position = args.senderBody.corePosition;
            if (args.Count > 1)
            {
                position = new Vector3(args.GetArgFloat(1), args.GetArgFloat(2), args.GetArgFloat(3));
            }
            var copy = UnityEngine.Object.Instantiate(reference);
            copy.transform.position = position;
        }
        [ConCommand(commandName = "rbs_spawneffect",
        flags = ConVarFlags.ExecuteOnServer,
        helpText = "rbs_spawneffect path x y z")]
        public static void CCSpawnEffect(ConCommandArgs args)
        {
            var reference = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(args.GetArgString(0)).WaitForCompletion();
            var position = args.senderBody.corePosition;
            if (args.Count > 1)
            {
                position = new Vector3(args.GetArgFloat(1), args.GetArgFloat(2), args.GetArgFloat(3));
            }
            var copy = UnityEngine.Object.Instantiate(reference);
            copy.transform.position = position;
            EffectManager.SimpleMuzzleFlash(reference, args.senderBody.gameObject, "HealthBarOrigin", false);
        }

        [ConCommand(commandName = "rbs_getspicechance",
        flags = ConVarFlags.ExecuteOnServer,
        helpText = "rbs_getspicechance")]
        public static void CCGetSpiceChance(ConCommandArgs args)
        {
            var run = Run.instance;
            var pickupIndex = PickupCatalog.FindPickupIndex(Spice2.Instance.EquipmentDef.equipmentIndex);
            var dict = new Dictionary<string, List<PickupIndex>>()
            {
                { "availableBossDropList", run.availableBossDropList },
                { "availableEquipmentDropList", run.availableEquipmentDropList },
                { "availableLunarCombinedDropList", run.availableLunarCombinedDropList },
                { "availableLunarEquipmentDropList", run.availableLunarEquipmentDropList },
                { "availableLunarItemDropList", run.availableLunarItemDropList },
                { "availableTier1DropList", run.availableTier1DropList },
                { "availableTier2DropList", run.availableTier2DropList },
                { "availableTier3DropList", run.availableTier3DropList },
                { "availableVoidBossDropList", run.availableVoidBossDropList },
                { "availableVoidTier1DropList", run.availableVoidTier1DropList },
                { "availableVoidTier2DropList", run.availableVoidTier2DropList },
                { "availableVoidTier3DropList", run.availableVoidTier3DropList },
            };
            var sb = HG.StringBuilderPool.RentStringBuilder();
            foreach (var kvp in dict)
            {
                var spiceCount = 0;
                foreach (var index in kvp.Value)
                {
                    if (index == pickupIndex)
                        spiceCount++;
                }
                var chance = (spiceCount / kvp.Value.Count) * 100;
                sb.AppendLine($"{kvp.Key} Count: {spiceCount}/{kvp.Value.Count} ({chance}% chance)");
            }
            Debug.Log(sb.ToString());
            HG.StringBuilderPool.ReturnStringBuilder(sb);
        }

        /*
        [ConCommand(commandName = "rbs_backpacksetslot",
        flags = ConVarFlags.ExecuteOnServer,
        helpText = "rbs_backpacksetslot index - Sets your equipment slot to the selected slot.")]
        public static void CCBackpackSetSlot(ConCommandArgs args)
        {
            if (!Run.instance)
            {
                Debug.LogWarning("Can't be used outside of a run!");
                return;
            }
            args.CheckArgumentCount(1);
            int? index = args.TryGetArgInt(0);
            if (!index.HasValue)
            {
                Debug.LogError("Couldn't parse the index!");
                return;
            }
            if (!args.senderMaster)
            {
                Debug.LogError("No character master found!");
                return;
            }
            if (args.senderMaster.TryGetComponent(out Backpack.BackpackComponent backpackComponent))
            {
                //var sb = HG.StringBuilderPool.RentStringBuilder();
                if (index > backpackComponent.selectableSlot)
                {
                    index = backpackComponent.selectableSlot;
                }
                backpackComponent.SetActiveEquipmentSlot((byte)index);
            }
        }

        [ConCommand(commandName = "rbs_backpacklist",
        flags = ConVarFlags.ExecuteOnServer,
        helpText = "rbs_backpacklist - Lists your equipment in console.")]
        public static void CCBackpackList(ConCommandArgs args)
        {
            if (args.senderMaster && args.senderMaster.inventory)
            {
                var inventory = args.senderMaster.inventory;
                if (args.senderMaster.TryGetComponent(out Backpack.BackpackComponent backpackComponent))
                {
                    var equipmentStateSlots = inventory.equipmentStateSlots;
                    var stringBuilder = HG.StringBuilderPool.RentStringBuilder();
                    if (equipmentStateSlots.Length > 0)
                    {
                        for (int i = 0; i <= backpackComponent.selectableSlot; i++)
                        {
                            var eqpName = "None";
                            var charges = -6;
                            var cooldown = -7;
                            if (i < equipmentStateSlots.Length) //prevents out of bounds error from unset slots
                            {
                                var eqp = equipmentStateSlots[i];
                                if (eqp.equipmentIndex != EquipmentIndex.None)
                                {
                                    eqpName = RoR2.Language.GetString(eqp.equipmentDef.nameToken);
                                }
                                charges = eqp.charges;
                                cooldown = eqp.isPerfomingRecharge ? Mathf.Max((int)eqp.chargeFinishTime.timeUntil, 0) : cooldown;
                            }
                            // Slot 0: "[1] Bomb 5x CD:10"
                            stringBuilder.AppendLine(
                                "[" + (i) + "] " +
                                eqpName +
                                (charges == -6 ? "" : " " + charges + "x") +
                                (cooldown == -7 ? "" : " CD:" + cooldown + " ")
                                );
                        }
                    }
                    HG.StringBuilderPool.ReturnStringBuilder(stringBuilder);
                }
            }
            else
            {
                Debug.LogError("No character master found!");
            }
        }*/
    }
}