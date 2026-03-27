using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(ColliderCastSingletonAuthoring))]
    public sealed class MouseInputAuthoring : MonoBehaviour {

        private class MouseInputAuthoringBaker : Baker<MouseInputAuthoring> {

            public override void Bake(MouseInputAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new MouseInputISingleton());
            }

        }

    }

    public struct MouseInputISingleton : IComponentData {

        public float CurrentInput;
        public float PreviousInput;
        public float2 Position;

    }

}