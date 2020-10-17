//using System;
//using System.Collections.Generic;
//using System.Text;
//using R2API;
using RoR2;
//using UnityEngine;
using TILER2;
//using static TILER2.StatHooks;
//using static TILER2.MiscUtil;


namespace RiskOfBulletstorm.Items
{
    public class EnragingPhoto : Item<EnragingPhoto>
    {
        public override string displayName => "Enraging Photo";
        public override ItemTier itemTier => RoR2.ItemTier.Tier2;
        protected override string NewLangName(string langID = null) => displayName;

        protected override string NewLangPickup(string langID = null) => "cum";

        protected override string NewLangDesc(string langid = null) => "shit";
        protected override string NewLangLore(string langID = null) => "fuck";

        //private static BuffIndex EnragedBuff;

        public EnragingPhoto()
        {} 
        protected override void LoadBehavior()
        {
           // Chat.AddMessage("Item Get!");
        }

        protected override void UnloadBehavior()
        {
            // Chat.AddMessage("Item Lost?");
        }
    }
}
