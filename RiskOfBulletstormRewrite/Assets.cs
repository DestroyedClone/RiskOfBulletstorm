﻿using System;
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
        //https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/#post-3177001
        
        public static Sprite LoadSprite(string fileName)
        {
            return Assets.LoadNewSprite(
            Assets.assemblyDir + "\\Assets\\"+fileName
            + ".png"
            );
        }
        
        public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
        {
            Sprite NewSprite = Assets.NullSprite;
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
            //Main._logger.LogMessage($"Loading Path \"{FilePath}\"");
            //Application.Quit();
            try {
                Texture2D SpriteTexture = LoadTexture(FilePath);
                NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0 , spriteType);
            } catch {
                Main._logger.LogMessage($"Failed to load file at \"{FilePath}\"");
            }
            
    
            return NewSprite;
        }
    
        public static Sprite ConvertTextureToSprite(Texture2D texture, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
        {
            // Converts a Texture2D to a sprite, assign this texture to a new sprite and return its reference
    
            Sprite NewSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);
    
            return NewSprite;
        }
    
        public static Texture2D LoadTexture(string FilePath)
        {
    
            // Load a PNG or JPG file from disk to a Texture2D
            // Returns null if load fails
    
            Texture2D Tex2D;
            byte[] FileData;
    
            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
                if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return Tex2D;                 // If data = readable -> return texture
            }
            return null;                     // Return null if load failed
        }
        
    }
}