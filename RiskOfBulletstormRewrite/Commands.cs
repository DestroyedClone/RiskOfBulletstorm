using RiskOfBulletstormRewrite.Equipment;
using RiskOfBulletstormRewrite.Items;
using RoR2;
using System.Collections.Generic;
using System.Linq;

namespace RiskOfBulletstormRewrite
{
    public static class Commands
    {
        public static void Initialize()
        {
            R2API.Utils.CommandHelper.AddToConsoleWhenReady();
        }

        [ConCommand(commandName = "rbs_givebulletparts",
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
        }

        [ConCommand(commandName = "rbs_getspicechance",
        flags = ConVarFlags.SenderMustBeServer,
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
            foreach (var kvp in dict )
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
            Main._logger.LogMessage(sb.ToString());
            HG.StringBuilderPool.ReturnStringBuilder(sb);
        }
    }
}