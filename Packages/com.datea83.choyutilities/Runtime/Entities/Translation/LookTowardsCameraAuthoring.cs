using EugeneC.Utilities;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    public sealed class LookTowardsCameraAuthoring : MonoBehaviour {

        public class Baker : Baker<LookTowardsCameraAuthoring> {

            public override void Bake(LookTowardsCameraAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<LookTowardsCameraITag>(e);
            }

        }

    }

    public struct LookTowardsCameraITag : IComponentData { }

    [UpdateInGroup(typeof(Eu_PostTransformSystemGroup))]
    public partial struct LookTowardsCameraISystem : ISystem {

        public void OnUpdate(ref SystemState state) {
            if (CameraController.Instance is null || Camera.main is null) return;
            var camTargetTransform = CameraController.Instance.transform;

            foreach (var lt
                     in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<LookTowardsCameraITag>()) {
                var fwd = lt.ValueRO.Position + (float3)(camTargetTransform.rotation * math.forward());
                var up = (float3)(camTargetTransform.rotation * math.up());
                lt.ValueRW.Rotation = quaternion.LookRotationSafe(fwd, up);
            }
        }

    }

}