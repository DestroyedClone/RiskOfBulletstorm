using RoR2;
using System.Reflection;
using UnityEngine;

namespace RiskOfBulletstormRewrite
{
    internal class Assets
    {
        internal static GameObject NullModel = LegacyResourcesAPI.Load<GameObject>("prefabs/nullmodel");
        internal static Sprite NullSprite = LegacyResourcesAPI.Load<Sprite>("textures/itemicons/texnullicon");

        //the bundle to load assets from
        public static AssetBundle mainAssetBundle;

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Main.pluginInfo.Location);
            }
        }

        public static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RiskOfBulletstormRewrite.riskofbulletstormbundle"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }
            /*
            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("PaladinMod.Paladin.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }*/
        }

        //https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/#post-3177001

        public static Sprite LoadSprite(string path)
        {
            return mainAssetBundle.LoadAsset<Sprite>(path);
        }

        /*

        public static Sprite LoadSprite(string fileName)
        {
            return Assets.LoadNewSprite(
            Assets.assemblyDir + "\\Assets\\" + fileName
            + ".png"
            );
        }

        public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f, SpriteMeshType spriteType = SpriteMeshType.Tight)
        {
            Sprite NewSprite = Assets.NullSprite;
            // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
            //Main._logger.LogMessage($"Loading Path \"{FilePath}\"");
            //Application.Quit();
            try
            {
                Texture2D SpriteTexture = LoadTexture(FilePath);
                NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit, 0, spriteType);
            }
            catch
            {
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
                //http://answers.unity.com/comments/1095919/view.html
                Tex2D = new Texture2D(2, 2, TextureFormat.BGRA32, false);           // Create new "empty" texture
                if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                    return Tex2D;                 // If data = readable -> return texture
            }
            return null;                     // Return null if load failed
        }
        */
    }
}