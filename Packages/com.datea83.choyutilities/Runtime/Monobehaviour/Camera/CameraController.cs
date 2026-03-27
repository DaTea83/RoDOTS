using EugeneC.Singleton;
using UnityEngine;

namespace EugeneC.Utilities {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class CameraController : GenericSingleton<CameraController> {

        public Camera Cam => GetComponent<Camera>();

    }

}