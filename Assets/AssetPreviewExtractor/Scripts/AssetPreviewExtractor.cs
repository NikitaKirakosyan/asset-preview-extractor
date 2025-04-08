#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NikitaKirakosyan.AssetPreviewExtractor
{
    public static class AssetPreviewExtractor
    {
        private const string LogChannel = nameof(AssetPreviewExtractor);
        
        private static readonly Color GreyColor = new (0.3215686f, 0.3215686f, 0.3215686f, 1f);


        public static void ExtractAndSavePreview(string savePath, GameObject asset, TextureSettings textureSettings)
        {
            if(asset == null)
                return;

            var previewTexture = AssetPreview.GetAssetPreview(asset);
            if(previewTexture == null)
            {
                Debug.LogError($"[{LogChannel}] Unable to get preview for {nameof(asset)}: {asset.name}! Try again.");
                return;
            }

            var newTexture = new Texture2D(previewTexture.width, previewTexture.height, TextureFormat.RGBA32, false);
            
            var pixels = previewTexture.GetPixels();
            if(textureSettings is { alphaIsTransparency: true })
            {
                for(var i = 0; i < pixels.Length; i++)
                {
                    var pixel = pixels[i];
                    if(Mathf.Approximately(pixel.r, GreyColor.r) && Mathf.Approximately(pixel.g, GreyColor.g) && Mathf.Approximately(pixel.b, GreyColor.b) && Mathf.Approximately(pixel.a, GreyColor.a))
                    {
                        pixel = Color.clear;
                        pixels[i] = pixel;
                    }
                }
            }
            
            newTexture.SetPixels(pixels);
            newTexture.Apply();

            var pngData = newTexture.EncodeToPNG();
            if(pngData == null)
            {
                Debug.LogError($"[{LogChannel}] Unable to encode to PNG!");
                return;
            }

            if(!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var filePath = $"{savePath}/{asset.name}_Icon.png";
            File.WriteAllBytes(filePath, pngData);
            AssetDatabase.ImportAsset(filePath);

            if(textureSettings != null)
                SetTextureAsSprite(filePath, textureSettings);

            Debug.Log($"[{LogChannel}] Preview saved at: {filePath}");
        }


        private static void SetTextureAsSprite(string assetPath, TextureSettings textureSettings)
        {
            var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if(textureImporter == null)
                return;

            textureImporter.textureType = textureSettings.textureType;
            textureImporter.spriteImportMode = textureSettings.spriteImportMode;
            textureImporter.spritePixelsPerUnit = textureSettings.spritePixelsPerUnit;
            textureImporter.sRGBTexture = textureSettings.sRGBTexture;
            textureImporter.alphaSource = textureSettings.alphaSource;
            textureImporter.alphaIsTransparency = textureSettings.alphaIsTransparency;
            textureImporter.isReadable = textureSettings.isReadable;
            textureImporter.mipmapEnabled = textureSettings.mipmapEnabled;
            textureImporter.wrapMode = textureSettings.wrapMode;
            textureImporter.filterMode = textureSettings.filterMode;
            textureImporter.SaveAndReimport();
        }
    }

    [Serializable]
    public class TextureSettings
    {
        public TextureImporterType textureType = TextureImporterType.Sprite;
        public SpriteImportMode spriteImportMode = SpriteImportMode.Single;
        public float spritePixelsPerUnit = 100;
        public bool sRGBTexture = true;
        public TextureImporterAlphaSource alphaSource = TextureImporterAlphaSource.FromInput;
        public bool alphaIsTransparency = true;
        public bool isReadable;
        public bool mipmapEnabled;
        public TextureWrapMode wrapMode = TextureWrapMode.Clamp;
        public FilterMode filterMode = FilterMode.Bilinear;
    }
}
#endif