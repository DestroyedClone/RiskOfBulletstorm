using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace RiskOfBulletstormRewrite
{
    class Assets
    {
        internal static GameObject NullModel = Resources.Load<GameObject>("prefabs/nullmodel");
        internal static Sprite NullSprite = Resources.Load<Sprite>("textures/itemicons/texnullicon");


        internal static string assemblyDir
        {
            get
            {
                return Path.GetDirectoryName(Main.pluginInfo.Location);
            }
        }
    }
}