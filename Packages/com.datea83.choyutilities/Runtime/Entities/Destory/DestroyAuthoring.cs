using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    public sealed class DestroyAuthoring : MonoBehaviour {

        private class DestroyBaker : Baker<DestroyAuthoring> {

            public override void Bake(DestroyAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<DestroyIEnableableTag>(e);
                SetComponentEnabled<DestroyIEnableableTag>(e, false);
            }

        }

    }

    public struct DestroyIEnableableTag : IComponentData, IEnableableComponent { }

    [UpdateInGroup(typeof(Eu_DestroySystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct DestroyEntityISystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var endFrameECB = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (_, entity)
                     in SystemAPI.Query<RefRO<DestroyIEnableableTag>>()
                         .WithAll<DestroyIEnableableTag>().WithEntityAccess())
                endFrameECB.DestroyEntity(entity);
        }

    }

}