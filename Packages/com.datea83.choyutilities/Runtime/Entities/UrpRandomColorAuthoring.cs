using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace EugeneC.ECS {

    /// <summary>
    ///     Any entity with URP lit material will be randomized with a color
    /// </summary>
    [DisallowMultipleComponent]
    public class UrpRandomColorAuthoring : MonoBehaviour {

        [SerializeField] private Color[] colors;

        private class Baker : Baker<UrpRandomColorAuthoring> {

            public override void Bake(UrpRandomColorAuthoring authoring) {
                if (authoring.colors is null || authoring.colors.Length == 0) return;
                var e = GetEntity(TransformUsageFlags.None);
                var buffer = AddBuffer<UrpRandomColorISingletonBuffer>(e);

                foreach (var color in authoring.colors)
                    buffer.Add(new UrpRandomColorISingletonBuffer
                        { Value = new float4(color.r, color.g, color.b, color.a) });
            }

        }

    }

    public struct UrpRandomColorISingletonBuffer : IBufferElementData {

        public float4 Value;

    }

    public struct UrpColorChangedITag : IComponentData { }

    [BurstCompile]
    [UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
    public partial struct UrpRandomColorISystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<UrpRandomColorISingletonBuffer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var buffer = SystemAPI.GetSingletonBuffer<UrpRandomColorISingletonBuffer>();
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (material, random, entity)
                     in SystemAPI.Query<RefRW<URPMaterialPropertyBaseColor>, RefRW<RandomIData>>()
                         .WithNone<UrpColorChangedITag>().WithEntityAccess()) {
                if (random.ValueRO.Value.state == 0) continue;

                var index = random.ValueRW.Value.NextInt(0, buffer.Length);
                material.ValueRW.Value = buffer[index].Value;

                ecb.AddComponent<UrpColorChangedITag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }

    }

}