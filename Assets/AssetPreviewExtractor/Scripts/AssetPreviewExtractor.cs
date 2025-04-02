#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace NikitaKirakosyan.AssetPreviewExtractor
{
    public static class AssetPreviewExtractor
    {
        private const string LogChannel = nameof(AssetPreviewExtractor);


        public static void ExtractAndSavePreview(string savePath, GameObject asset)
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
            newTexture.SetPixels(previewTexture.GetPixels());
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

            SetTextureAsSprite(filePath);

            Debug.Log($"[{LogChannel}] Preview saved at: {filePath}");
        }


        private static void SetTextureAsSprite(string assetPath)
        {
            var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if(textureImporter == null)
                return;

            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.spriteImportMode = SpriteImportMode.Single;
            textureImporter.mipmapEnabled = false;
            textureImporter.filterMode = FilterMode.Bilinear;
            textureImporter.SaveAndReimport();
        }
    }
}
#endif