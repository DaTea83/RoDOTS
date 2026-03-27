using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS {

    [RequireComponent(typeof(DestroyAuthoring))]
    [DisallowMultipleComponent]
    public class DestroyBufferEntryAuthoring : MonoBehaviour {

        public class Baker : Baker<DestroyBufferEntryAuthoring> {

            public override void Bake(DestroyBufferEntryAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddBuffer<DestroyBufferEntryIBuffer>(e);
            }

        }

    }

    public struct DestroyBufferEntryIBuffer : IBufferElementData {

        public float Value;

    }

    [UpdateInGroup(typeof(Eu_DestroySystemGroup))]
    [UpdateBefore(typeof(DestroyEntityISystem))]
    public partial struct DestroyBufferISystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var (buffer, entity) in SystemAPI.Query<DynamicBuffer<DestroyBufferEntryIBuffer>>()
                         .WithPresent<DestroyIEnableableTag>().WithEntityAccess()) {
                if (buffer.Length <= 0) continue;
                SystemAPI.SetComponentEnabled<DestroyIEnableableTag>(entity, true);
            }
        }

    }

}