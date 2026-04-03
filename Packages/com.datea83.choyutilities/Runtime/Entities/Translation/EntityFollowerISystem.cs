using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EugeneC.ECS {

    /// <summary>
    ///     Entity follow gameObject transform
    /// </summary>
    public struct EntityTransformIData : IComponentData {

        public UnityObjectRef<Transform> Transform;
        public float3 Offset;
        public float SmoothFollowSpeed;

    }

    [UpdateInGroup(typeof(Eu_PostTransformSystemGroup), OrderFirst = true)]
    public partial struct EntityFollowerISystem : ISystem {

        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.Time.DeltaTime;

            foreach (var (entityTransform, lt, entity)
                     in SystemAPI.Query<RefRO<EntityTransformIData>, RefRW<LocalTransform>>().WithEntityAccess()) {
                var factor = entityTransform.ValueRO.SmoothFollowSpeed > 0
                    ? entityTransform.ValueRO.SmoothFollowSpeed * dt
                    : 1;

                var obj = entityTransform.ValueRO.Transform;

                if (obj.Value is null) {
                    if (!SystemAPI.HasComponent<DestroyIEnableableTag>(entity)) continue;
                    SystemAPI.SetComponentEnabled<DestroyIEnableableTag>(entity, true);

                    continue;
                }

                lt.ValueRW.Position = math.lerp(lt.ValueRO.Position,
                    (float3)obj.Value.position + entityTransform.ValueRO.Offset, factor);
                lt.ValueRW.Rotation = math.slerp(lt.ValueRO.Rotation, obj.Value.rotation, factor);
            }
        }

    }

    [UpdateInGroup(typeof(Eu_InitializationSystemGroup), OrderFirst = true)]
    public partial struct InitialDestroyFollowerISystem : ISystem {

        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (_, entity) in SystemAPI.Query<RefRO<EntityTransformIData>>()
                         .WithAbsent<DestroyIEnableableTag>().WithEntityAccess()) {
                ecb.AddComponent<DestroyIEnableableTag>(entity);
                ecb.SetComponentEnabled<DestroyIEnableableTag>(entity, false);
            }

            ecb.Playback(state.EntityManager);
        }

    }

}