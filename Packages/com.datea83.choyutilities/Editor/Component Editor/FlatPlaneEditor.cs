using EugeneC.ECS;
using UnityEditor;
using UnityEngine;

// ReSharper disable CheckNamespace

namespace EugeneC.Editor {

    [CustomEditor(typeof(FlatPlaneAuthoring))]
    public class FLatPlaneEditor : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            var authoring = (FlatPlaneAuthoring)target;

            authoring.planePrefab =
                (GameObject)EditorGUILayout.ObjectField("Prefab", authoring.planePrefab, typeof(GameObject), false);
            authoring.planeSize = (uint)EditorGUILayout.IntField("Size", (int)authoring.planeSize);
            authoring.unitsPerPlane = EditorGUILayout.FloatField("Units per Plane", authoring.unitsPerPlane);
            authoring.planeOffset = EditorGUILayout.Vector3Field("Offset", authoring.planeOffset);
            authoring.seed = (byte)EditorGUILayout.IntField("Seed", authoring.seed);

            if (GUILayout.Button("This is a button")) authoring.seed = (byte)Random.Range(0, byte.MaxValue);

            serializedObject.ApplyModifiedProperties();
        }

    }

}