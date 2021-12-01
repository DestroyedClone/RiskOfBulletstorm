using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RiskOfBulletstormRewrite
{
    class Assets
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