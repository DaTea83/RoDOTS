using EugeneC.Utilities;
using UnityEditor;

// ReSharper disable CheckNamespace

namespace EugeneC.Editor {

    [CustomEditor(typeof(CameraController))]
    public class CameraControllerEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var instance = (CameraController)target;

            EditorGUILayout.HelpBox(
                "Currently in ECS you can't just attach the camera to a subscene entity and called it a day",
                MessageType.Info);
            EditorGUILayout.HelpBox("That's where this singleton comes into play", MessageType.Info);
            EditorGUILayout.HelpBox("Attach this to the camera object", MessageType.Info);

            EditorGUILayout.HelpBox("Don't put the camera in the subscene, put it in normal hierarchy",
                MessageType.Warning);

            EditorGUILayout.HelpBox(
                "Keep Singleton true or not doesn't matter, if true it will just override the other scene's camera",
                MessageType.Info);

            base.OnInspectorGUI();
        }

    }

}