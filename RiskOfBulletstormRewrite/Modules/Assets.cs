using RoR2;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskOfBulletstormRewrite.Modules
{
    internal static class Assets
    {
        internal static GameObject NullModel = LoadAsset<GameObject>("RoR2/Base/Core/NullModel.prefab");
        internal static Sprite NullSprite = LoadAsset<Sprite>("RoR2/Base/Core/texNullIcon.png");

        internal static ItemTierDef itemLunarTierDef = LoadAsset<ItemTierDef>("RoR2/Base/Common/LunarTierDef.asset");
        internal static ItemTierDef itemBossTierDef = LoadAsset<ItemTierDef>("RoR2/Base/Common/BossTierDef.asset");
        internal static ItemTierDef itemTier1Def = LoadAsset<ItemTierDef>("RoR2/Base/Common/Tier1Def.asset");
        internal static ItemTierDef itemTier2Def = LoadAsset<ItemTierDef>("RoR2/Base/Common/Tier2Def.asset");
        internal static ItemTierDef itemTier3Def = LoadAsset<ItemTierDef>("RoR2/Base/Common/Tier3Def.asset");
        internal static ItemTierDef itemVoidBossTierDef = LoadAsset<ItemTierDef>("RoR2/DLC1/Common/VoidBossDef.asset");
        internal static ItemTierDef itemVoidTier1Def = LoadAsset<ItemTierDef>("RoR2/DLC1/Common/VoidTier1Def.asset");
        internal static ItemTierDef itemVoidTier2Def = LoadAsset<ItemTierDef>("RoR2/DLC1/Common/VoidTier2Def.asset");
        internal static ItemTierDef itemVoidTier3Def = LoadAsset<ItemTierDef>("RoR2/DLC1/Common/VoidTier3Def.asset");

        //the bundle to load assets from
        public static AssetBundle mainAssetBundle;

        public static T LoadAsset<T> (string path)
        {
            return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
        }


        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(Main.pluginInfo.Location);
            }
        }

        public static void Init()
        {
            //commandoMat = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            PopulateAssets();
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
        }
        //https://forum.unity.com/threads/generating-sprites-dynamically-from-png-or-jpeg-files-in-c.343735/#post-3177001

        public static Sprite LoadSprite(string path)
        {
            return mainAssetBundle.LoadAsset<Sprite>(path);
        }

        public static GameObject LoadObject(string path)
        {
            return mainAssetBundle.LoadAsset<GameObject>(path);
        }

        public static T LoadAddressable<T>(string assetPath)
        {
            var loadedAsset = Addressables.LoadAssetAsync<T>(assetPath).WaitForCompletion();
            return loadedAsset;
        }

        /*
        public static Material commandoMat;
        internal static Shader hotpoo = LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/HGStandard");

        #region helpers

        internal static Sprite LoadBuffSprite(string path)
        {
            return Addressables.LoadAssetAsync<BuffDef>(path).WaitForCompletion().iconSprite;
        }

        public static GameObject ConvertAllRenderersToHopooShader(this GameObject objectToConvert)
        {
            if (!objectToConvert) return objectToConvert;

            foreach (MeshRenderer i in objectToConvert.GetComponentsInChildren<MeshRenderer>())
            {
                if (i?.sharedMaterial != null)
                {
                    i.sharedMaterial.SetHotpooMaterial();
                }
            }

            foreach (SkinnedMeshRenderer i in objectToConvert.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (i?.sharedMaterial != null)
                {
                    i.sharedMaterial.SetHotpooMaterial();
                }
            }

            return objectToConvert;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            Modules.Content.AddNetworkSoundEventDef(networkSoundEventDef);

            return networkSoundEventDef;
        }

        public static T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            //handle dynamically loading from additional bundle if we want to here
            return MainAssetBundle.LoadAsset<T>(name);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, AssetBundle bundle)
        {
            GameObject newEffect = bundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = true;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            Modules.Effects.AddEffect(newEffect);

            return newEffect;
        }

        public static GameObject LoadCrosshair(string crosshairName)
        {
            if (LoadAsset<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null) return LoadAsset<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            return LoadAsset<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        internal static GameObject LoadSurvivorModel(string modelName)
        {
            GameObject model = LoadAsset<GameObject>(modelName);
            if (model == null)
            {
                Debug.LogError("Trying to load a null model- check to see if the name in your code matches the name of the object in Unity");
                return null;
            }

            return model.InstantiateClone(model.name, false);
        }

        #endregion helpers

        */
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