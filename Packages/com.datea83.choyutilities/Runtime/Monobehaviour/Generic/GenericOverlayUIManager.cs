using System;
using EugeneC.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace EugeneC.Singleton {

    //TODO
    [RequireComponent(typeof(UIDocument))]
    public abstract class GenericOverlayUIManager<TMono, TObj> : GenericSingleton<TMono>
        where TMono : MonoBehaviour
        where TObj : ScriptableObject {

        [SerializeField] protected TObj scriptableObject;
        [SerializeField] protected UIDocument overlayUI;
        [SerializeField] protected BindingSerializable[] bindings;

        protected VisualElement root => overlayUI?.rootVisualElement;

        private void OnEnable() {
            TryBindAll();
        }

        protected virtual void OnValidate() {
            overlayUI = GetComponent<UIDocument>();
        }

        protected virtual void TryBindAll() {
            if (scriptableObject is null) {
                Debug.LogWarning($"{GetType().Name}: scriptableObject is not assigned.", this);

                return;
            }

            if (overlayUI is null) {
                Debug.LogWarning($"{GetType().Name}: overlayUI is not assigned.", this);

                return;
            }

            if (root is null) {
                Debug.LogWarning($"{GetType().Name}: rootVisualElement is null.", this);

                return;
            }

            if (bindings is null || bindings.Length == 0) {
                Debug.LogWarning($"{GetType().Name}: No bindings assigned.", this);

                return;
            }

            for (var i = 0; i < bindings.Length; i++) {
                var binding = bindings[i];

                if (string.IsNullOrWhiteSpace(binding.bindName)) {
                    Debug.LogWarning($"{GetType().Name}: Binding at index {i} has an empty element name.", this);

                    continue;
                }

                var element = root.Q<VisualElement>(binding.bindName);

                if (element is null) {
                    Debug.LogWarning(
                        $"{GetType().Name}: Could not find VisualElement named '{binding.bindName}' for data '{scriptableObject.name}'.",
                        this);
                    OnBindFailed(binding);

                    continue;
                }

                OnBindSuccess(binding, element);
            }
        }

        protected virtual void OnBindFailed(BindingSerializable binding) { }

        protected abstract void OnBindSuccess(BindingSerializable binding, VisualElement element);

        [Serializable]
        public struct BindingSerializable {

            public EVisualElements bindType;
            public string bindName;

        }

    }

}