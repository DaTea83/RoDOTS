using EugeneC.Utilities;
using UnityEngine;

namespace EugeneC.Mono {

    [AddComponentMenu("Eugene/Rotator(Mono)")]
    public class MonoRotate : MonoBehaviour {

        public EAxis rotateAxis;
        public float rotateSpeed;

        private void Update() {
            switch (rotateAxis) {
                case EAxis.X:

                    transform.Rotate(rotateSpeed * Time.deltaTime, 0, 0);

                    break;
                case EAxis.Y:

                    transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

                    break;
                case EAxis.Z:

                    transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);

                    break;
            }
        }

    }

}