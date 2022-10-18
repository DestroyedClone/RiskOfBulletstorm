using RiskOfBulletstormRewrite.Items;
using RoR2;

namespace RiskOfBulletstormRewrite
{
    public static class Commands
    {
        public static void Initialize()
        {
            //R2API.Utils.CommandHelper.AddToConsoleWhenReady();
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
    }
}