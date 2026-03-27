using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    [RequireComponent(typeof(DestroyBufferEntryAuthoring))]
    public sealed class BoxAuthoring : MonoBehaviour {

        public class Baker : Baker<BoxAuthoring> {

            public override void Bake(BoxAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(e, new BoxIData());
            }

        }

    }

    public struct BoxIData : IComponentData {

        public float ExistTime;
        public float3 Velocity;

    }

}