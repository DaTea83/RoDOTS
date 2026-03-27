using UnityEditor;
using UnityEngine;

namespace EugeneC.Editor {

    internal static class RemoveMissingScriptsEditor {

        [MenuItem(EditorUtils.UtilityWindow + "Missing Scripts/Find")]
        internal static void FindMissingScripts() {
#if UNITY_2023_1_OR_NEWER
            foreach (var gameObject in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
#else
            foreach (var gameObject in GameObject.FindObjectsOfType<GameObject>(true))
#endif
            foreach (var component in gameObject.GetComponentsInChildren<Component>()) {
                if (component is not null) continue;
                Debug.Log($"GameObject: {gameObject.name} has missing Script", gameObject);

                break;
            }
        }

        [MenuItem(EditorUtils.UtilityWindow + "Missing Scripts/Remove")]
        internal static void RemoveMissingScripts() {
#if UNITY_2023_1_OR_NEWER
            foreach (var gameObject in Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include,
                         FindObjectsSortMode.None))
#else
            foreach (var gameObject in GameObject.FindObjectsOfType<GameObject>(true))
#endif
            foreach (var component in gameObject.GetComponentsInChildren<Component>()) {
                if (component is not null) continue;
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
                Debug.Log($"Removing {gameObject.name}'s missing Script", gameObject);
            }
        }

    }

}