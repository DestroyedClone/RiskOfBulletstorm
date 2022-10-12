using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using RoR2;

namespace RiskOfBulletstormRewrite
{
    class Assets
    {
        internal static GameObject NullModel = LegacyResourcesAPI.Load<GameObject>("prefabs/nullmodel");
        internal static Sprite NullSprite = LegacyResourcesAPI.Load<Sprite>("textures/itemicons/texnullicon");


        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Main.pluginInfo.Location);
            }
        }

    public static Texture2D LoadPNG(string filePath) 
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))     {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
    
    }
}