using EugeneC.Utilities;
using UnityEngine;

namespace EugeneC.Mono {

    [RequireComponent(typeof(BoxCollider))]
    public class TriggerMethod : MonoBehaviour {

        [SerializeField] private LayerMask layer;
        [SerializeField] private string objectTag;

        [SerializeField] private string instanceClassName;
        [SerializeField] private string methodName;
        [SerializeField] private bool turnOffAfter;

        private BoxCollider _collider;

        // Start is called before the first frame update
        private void Start() {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.gameObject.layer != layer && !other.gameObject.CompareTag(objectTag)) return;

            UtilityMethods.CallGenericInstanceMethod(instanceClassName, methodName);

            if (turnOffAfter)
                gameObject.SetActive(false);
        }

    }

}