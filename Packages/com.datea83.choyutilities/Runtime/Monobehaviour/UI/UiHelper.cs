using EugeneC.ECS;
using UnityEngine;

namespace EugeneC.Mono {

    public abstract class UiHelper : MonoBehaviour {

        [SerializeField] [Min(0.01f)] protected float transitionTime;

        public RectTransform[] uiTransforms;
        protected byte OwnId;
        protected RectTransform TransformRect;

        protected UiHandleSystemBase UISystem;

        private void OnValidate() {
            TransformRect = GetComponent<RectTransform>();
        }

        public void SetId(byte id) {
            OwnId = id;
        }

        public void SetAndSubSystem(UiHandleSystemBase system) {
            UISystem = system;
            UISystem.OnUiClicked += OnUiClicked;
            UISystem.OnUiEnterHover += OnUiEnterHover;
            UISystem.OnUiExitHover += OnUiExitHover;
        }

        protected virtual void OnUiExitHover(byte arg1, byte arg2) {
            if (arg1 != OwnId) return;
            var ui = uiTransforms[arg2];
            Debug.Log(ui.name);
        }

        protected virtual void OnUiEnterHover(byte arg1, byte arg2) {
            if (arg1 != OwnId) return;
            var ui = uiTransforms[arg2];
            Debug.Log(ui.name);
        }

        protected virtual void OnUiClicked(byte arg1, byte arg2) {
            if (arg1 != OwnId) return;
            var ui = uiTransforms[arg2];
            Debug.Log(ui.name);
        }

        public abstract void OnSpawn();

        public abstract float OnStartOpen();

        public abstract void OnEndOpen();

        public abstract float OnStartClose();

        public abstract void OnEndClose();

    }

}