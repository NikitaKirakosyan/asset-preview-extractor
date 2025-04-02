#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace NikitaKirakosyan.AssetPreviewExtractor
{
    public class AssetPreviewExtractorWindow : EditorWindow
    {
        public GameObject[] assets;
        public string savePath = "Assets/PrefabPreviews";
        public bool extraSettings;
        public TextureSettings textureSettings;

        private Vector2 _scrollPosition;
        

        [MenuItem("Window/AssetPreviewExtractor/Asset Preview Extractor Window", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<AssetPreviewExtractorWindow>();
            window.titleContent = new GUIContent("Asset Preview Extractor Window");

            var width = 640f;
            var height = 480f;
            window.position = new Rect(Screen.width / 2f - width / 2f, Screen.height / 2f - height / 2f, width, height);
        }


        private void OnGUI()
        {
            var serializedObj = new SerializedObject(this);
            var assetsProp = serializedObj.FindProperty(nameof(assets));
            var textureSettingsProp = serializedObj.FindProperty(nameof(textureSettings));

            //Vertical Start
            GUILayout.BeginVertical();
            
            savePath = EditorGUILayout.TextField("Save Path", savePath);
            
            //Scroll Start
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            
            extraSettings = EditorGUILayout.Toggle("Extra Settings", extraSettings);
            if(extraSettings)
            {
                EditorGUILayout.PropertyField(textureSettingsProp, true);
                GUILayout.Space(15);
            }
            else
            {
                textureSettings = null;
            }
            
            EditorGUILayout.PropertyField(assetsProp, true);
            GUILayout.EndScrollView();
            //Scroll End
            
            if(GUILayout.Button("Generate previews", options: new[] { GUILayout.Height(32) }))
            {
                foreach(var asset in assets)
                    AssetPreviewExtractor.ExtractAndSavePreview(savePath, asset, textureSettings);
            }
            
            var folderToPing = AssetDatabase.LoadAssetAtPath<Object>($"{savePath}");
            if(folderToPing != null && GUILayout.Button("Ping folder", options: new[] { GUILayout.Height(32) }))
                EditorGUIUtility.PingObject(folderToPing);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            //Vertical End

            serializedObj.ApplyModifiedProperties();
        }
    }
}
#endif