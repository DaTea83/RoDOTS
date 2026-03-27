using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FolderColor {

#if UNITY_EDITOR

    public abstract class ProjectAssetViewerCustomisation {

        // Reference to the data
        public static AssetModificationData ModificationData = new();

        [InitializeOnLoadMethod]
        private static void Initialize() {
            LoadData();

            //for each object
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect) {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Ensure assetType is not null before accessing it
            if (ModificationData.assetModified != null && ModificationData.assetModified.Contains(assetPath)) {
                var t = ModificationData.assetModified.IndexOf(assetPath);

                var tex = (Texture2D)AssetDatabase.LoadAssetAtPath(ModificationData.assetModifiedTexturePath[t],
                    typeof(Texture2D));

                if (tex == null) {
                    ModificationData.assetModified.RemoveAt(t);
                    ModificationData.assetModifiedTexturePath.RemoveAt(t);
                    SaveData();

                    return;
                }

                if (selectionRect.height == 16)
                    GUI.DrawTexture(
                        new Rect(selectionRect.x + 1.5f, selectionRect.y, selectionRect.height, selectionRect.height),
                        tex);
                else
                    GUI.DrawTexture(
                        new Rect(selectionRect.x, selectionRect.y, selectionRect.height - 10,
                            selectionRect.height - 10), tex);
            }
        }

        // Add a menu item in the Unity Editor to open the custom modification window
        [MenuItem("Assets/Custom Folder", false, 100)]
        private static void CustomModificationMenuItem() {
            var guids = Selection.assetGUIDs;

            foreach (var guid in guids) {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (AssetDatabase.IsValidFolder(assetPath)) {
                    CustomWindowFileImage.ShowWindow(assetPath);

                    break;
                }
            }
        }

        // Validate function to enable/disable the menu item
        [MenuItem("Assets/Custom Folder", true)]
        private static bool ValidateCustomModificationMenuItem() {
            var guids = Selection.assetGUIDs;

            foreach (var guid in guids) {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (AssetDatabase.IsValidFolder(assetPath)) return true;
            }

            return false;
        }

        public static void SaveData() {
            // Create or update the modificationData
            ModificationData ??= new AssetModificationData();

            // Convert to JSON
            var jsonData = JsonUtility.ToJson(ModificationData);

            var path = FindScriptPathByName("ProjectAssetViewerCustomisation");
            path = path.Replace("Editor/ProjectAssetViewerCustomisation.cs", "SaveSetUp/FolderModificationData.json");

            File.WriteAllText(path, jsonData);
        }

        private static void LoadData() {
            var filePath = FindScriptPathByName("ProjectAssetViewerCustomisation");

            filePath = filePath.Replace("Editor/ProjectAssetViewerCustomisation.cs",
                "SaveSetUp/FolderModificationData.json");

            if (File.Exists(filePath)) {
                var jsonData = File.ReadAllText(filePath);

                ModificationData = JsonUtility.FromJson<AssetModificationData>(jsonData);
            }
        }

        public static string FindScriptPathByName(string scriptName) {
            var guids = AssetDatabase.FindAssets($"{scriptName} t:script");

            if (guids.Length == 0) {
                Debug.LogError($"Script with name '{scriptName}' not found!");

                return null;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);

            return path;
        }

        [Serializable]
        public class AssetModificationData {

            public List<string> assetModified = new();
            public List<string> assetModifiedTexturePath = new();

        }

    }

#endif

}