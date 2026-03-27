using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS {

    [RequireComponent(typeof(DestroyAuthoring))]
    [DisallowMultipleComponent]
    public sealed class DestroyAfterTimeAuthoring : MonoBehaviour {

        [SerializeField] [Min(0.1f)] private float time;

        private class DestroyAfterTimeBaker : Baker<DestroyAfterTimeAuthoring> {

            public override void Bake(DestroyAfterTimeAuthoring authoring) {
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), new DestroyTimeIData { Value = authoring.time });
            }

        }

    }

    public struct DestroyTimeIData : IComponentData {

        public float Value;

    }

    [UpdateInGroup(typeof(Eu_DestroySystemGroup))]
    [UpdateBefore(typeof(DestroyEntityISystem))]
    [BurstCompile]
    public partial struct DestroyAfterTimeISystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var dt = SystemAPI.Time.DeltaTime;

            foreach (var (destroyTime, entity)
                     in SystemAPI.Query<RefRW<DestroyTimeIData>>()
                         .WithPresent<DestroyIEnableableTag>().WithEntityAccess()) {
                destroyTime.ValueRW.Value -= dt;

                if (destroyTime.ValueRW.Value > 0) continue;
                SystemAPI.SetComponentEnabled<DestroyIEnableableTag>(entity, true);
            }
        }

    }

}