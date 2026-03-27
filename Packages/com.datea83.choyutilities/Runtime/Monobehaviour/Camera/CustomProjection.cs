using UnityEngine;

namespace EugeneC.Utilities {

    //TODO
    [RequireComponent(typeof(Camera))]
    public class CustomProjection : MonoBehaviour {

        public float left = -0.2F;
        public float right = 0.2F;
        public float top = 0.2F;
        public float bottom = -0.2F;

        private void OnValidate() {
            var cam = GetComponent<Camera>();
            var m = PerspectiveOffCenter(left, right, bottom, top, cam.nearClipPlane, cam.farClipPlane);
            cam.projectionMatrix = m;
        }

        private static Matrix4x4 PerspectiveOffCenter(float left,
            float right,
            float bottom,
            float top,
            float near,
            float far) {
            var x = 2.0F * near / (right - left);
            var y = 2.0F * near / (top - bottom);
            var a = (right + left) / (right - left);
            var b = (top + bottom) / (top - bottom);
            var c = -(far + near) / (far - near);
            var d = -(2.0F * far * near) / (far - near);
            var e = -1.0F;

            var m = new Matrix4x4 {
                [0, 0] = x,
                [0, 1] = 0,
                [0, 2] = a,
                [0, 3] = 0,
                [1, 0] = 0,
                [1, 1] = y,
                [1, 2] = b,
                [1, 3] = 0,
                [2, 0] = 0,
                [2, 1] = 0,
                [2, 2] = c,
                [2, 3] = d,
                [3, 0] = 0,
                [3, 1] = 0,
                [3, 2] = e,
                [3, 3] = 0
            };

            return m;
        }

    }

}