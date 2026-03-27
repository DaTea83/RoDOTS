using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using ERotationOrder = Unity.Mathematics.math.RotationOrder;

namespace EugeneC.ECS {

    public struct RotatorIData : IComponentData {

        /// <summary>
        ///     Order the euler axes will be applied when converting to a quaternion before applying the rotation.
        /// </summary>
        public ERotationOrder RotationOrder;

        public float3 EulerRadiansPerSecond;

    }

    [DisallowMultipleComponent]
    public sealed class RotatorAuthoring : MonoBehaviour {

        public ERotationOrder rotationOrder;
        public float3 eulerRadiansPerSecond;

        private class RotatorBaker : Baker<RotatorAuthoring> {

            public override void Bake(RotatorAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new RotatorIData {
                    RotationOrder = authoring.rotationOrder,
                    EulerRadiansPerSecond = authoring.eulerRadiansPerSecond
                });
            }

        }

    }

    [UpdateInGroup(typeof(Eu_PreTransformSystemGroup))]
    [BurstCompile]
    public partial struct RotatorISystem : ISystem {

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, rotatorData) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<RotatorIData>>()) {
                var rotationThisFrame = quaternion.Euler(rotatorData.ValueRO.EulerRadiansPerSecond * deltaTime,
                    rotatorData.ValueRO.RotationOrder);
                transform.ValueRW = transform.ValueRW.Rotate(rotationThisFrame);
            }
        }

    }

}