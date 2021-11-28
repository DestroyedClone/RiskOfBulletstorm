using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RiskOfBulletstormRewrite
{
    class Assets //Meant to be used to hold assetbundle shit... this mod does not use that, though, but still
    {
        internal static string assemblyDir
        {
            get
            {
                return Path.GetDirectoryName(Main.pluginInfo.Location);
            }
        }
    }
}