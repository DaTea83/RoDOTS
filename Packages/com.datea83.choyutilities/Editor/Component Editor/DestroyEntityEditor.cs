using EugeneC.ECS;
using UnityEditor;

// ReSharper disable CheckNamespace

namespace EugeneC.Editor {

#if UNITY_EDITOR

    [CustomEditor(typeof(DestroyAuthoring))]
    public class DestroyEntityEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            EditorGUILayout.HelpBox(
                "Component will be disabled at start, after activation will instant despawn the entity",
                MessageType.Info);
        }

    }

#endif

}