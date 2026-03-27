using UnityEditor;
using UnityEngine;

namespace FolderColor {

#if UNITY_EDITOR

    public class CustomWindowFileImage : EditorWindow {

        private string _assetPath;

        private void OnGUI() {
            if (GUI.Button(new Rect(0, 0, 100, 100), "None")) {
                if (ProjectAssetViewerCustomisation.ModificationData.assetModified.Contains(_assetPath)) {
                    RemoveReference(_assetPath);
                    ProjectAssetViewerCustomisation.SaveData();
                }

                Close();
            }

            var path = ProjectAssetViewerCustomisation.FindScriptPathByName("CustomWindowFileImage");
            path = path.Replace("/Editor/CustomWindowFileImage.cs", "");

            var texturesPath = AssetDatabase.FindAssets("t:texture2D", new[] { path });

            var buttonsPerRow = 4;
            var buttonPadding = 10f;

            for (var i = 0; i < texturesPath.Length; i++) {
                var texture =
                    (Texture2D)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(texturesPath[i]),
                        typeof(Texture2D));

                var buttonWidth = (position.width - (buttonsPerRow + 1) * buttonPadding) / buttonsPerRow;
                var buttonHeight = 100f;

                var x = i % buttonsPerRow * (buttonWidth + buttonPadding) + buttonPadding;
                var y = Mathf.Floor(i / buttonsPerRow) * (buttonHeight + buttonPadding) + buttonPadding + 100;

                if (GUI.Button(new Rect(x, y, buttonWidth, buttonHeight), texture)) {
                    if (ProjectAssetViewerCustomisation.ModificationData.assetModified.Contains(_assetPath))
                        RemoveReference(_assetPath);

                    ProjectAssetViewerCustomisation.ModificationData.assetModified.Add(_assetPath);

                    ProjectAssetViewerCustomisation.ModificationData.assetModifiedTexturePath.Add(
                        AssetDatabase.GUIDToAssetPath(texturesPath[i]));
                    ProjectAssetViewerCustomisation.SaveData();

                    Close();
                }
            }
        }

        public static void ShowWindow(string assetPathGive) {
            var window = GetWindow<CustomWindowFileImage>("Custom Folder");
            window._assetPath = assetPathGive;
            window.Show();
        }

        private static void RemoveReference(string assetPath) {
            var i = ProjectAssetViewerCustomisation.ModificationData.assetModified.IndexOf(assetPath);
            ProjectAssetViewerCustomisation.ModificationData.assetModified.RemoveAt(i);
            ProjectAssetViewerCustomisation.ModificationData.assetModifiedTexturePath.RemoveAt(i);
        }

    }
#endif

}