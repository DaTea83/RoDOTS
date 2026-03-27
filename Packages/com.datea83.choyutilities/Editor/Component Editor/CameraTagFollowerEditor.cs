using EugeneC.ECS;
using UnityEditor;

// ReSharper disable CheckNamespace

namespace EugeneC.Editor {

    [CustomEditor(typeof(CameraTagAuthoring))]
    public class CameraTagFollowerEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox("Don't put this in the camera", MessageType.Warning);
            EditorGUILayout.HelpBox("For camera use CameraTrackerController", MessageType.Warning);

            EditorGUILayout.HelpBox("This is used for the camera to track the attached entity's transform",
                MessageType.Info);

            EditorGUILayout.HelpBox("Make sure only have a single entity have this component in any given runtime!!!",
                MessageType.Warning);
        }

    }

}