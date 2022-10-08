using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using RoR2;
using RoR2.Items;
using RiskOfBulletstormRewrite;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace RiskOfBulletstormRewrite.Controllers
{
    public class BulletQuestController : ControllerBase<BulletQuestController>
	{
        /*  Primer: 110 casings on stage 2
        *   gunpowder: black powder mine
        *   planar lead: invis path in hollow
        *   shell casing: dragun
        */  

        static ItemDef primerDef => Items.PrimePrimer.instance.ItemDef;
        static ItemDef gunpowderDef => Items.ArcaneGunpowder.instance.ItemDef;
        static ItemDef leadDef => Items.PlanarLead.instance.ItemDef;
        static ItemDef casingDef => Items.ObsidianShellCasing.instance.ItemDef;
        
        public override void Init(ConfigFile config)
        {
            SetupConfig(config);
        }

        public static void SetupConfig(ConfigFile config)
        {
            
        }
        public void CraftShell(CharacterBody steve)
        {
            if (steve.inventory)
            {
                bool hasItem(ItemDef itemDef)
                {
                    return steve.inventory.GetItemCount(itemDef) > 0;
                }

                if (hasItem(primerDef) 
                && hasItem(gunpowderDef)
                && hasItem(leadDef)
                && hasItem(casingDef))
                {
                    steve.inventory.RemoveItem(primerDef);
                    steve.inventory.RemoveItem(gunpowderDef);
                    steve.inventory.RemoveItem(leadDef);
                    steve.inventory.RemoveItem(casingDef);
                    steve.inventory.GiveItem(Items.PastKillingBullet.instance.ItemDef);
                }
            }
        }

    }
}
