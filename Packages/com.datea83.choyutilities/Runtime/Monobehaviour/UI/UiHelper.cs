using UnityEngine;

namespace EugeneC.Mono {

    [RequireComponent(typeof(RectTransform))]
    public abstract class UiHelper : MonoBehaviour {

        [SerializeField] [Min(0.01f)] protected float transitionTime;

        public abstract void OnSpawn();

        public abstract float OnStartOpen();

        public abstract void OnEndOpen();

        public abstract float OnStartClose();

        public abstract void OnEndClose();

    }

}