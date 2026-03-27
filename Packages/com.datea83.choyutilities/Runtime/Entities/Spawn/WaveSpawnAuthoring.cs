using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    public class WaveSpawnAuthoring : MonoBehaviour {

        public GameObject prefab;
        public int2 size = new(100, 100);
        public float2 spacing = new(10, 10);
        public float height = 20;
        [Range(0, 2f)] public float speed = .2f;

        public class Baker : Baker<WaveSpawnAuthoring> {

            public override void Bake(WaveSpawnAuthoring authoring) {
                if (authoring.prefab is null) return;

                var entity = GetEntity(TransformUsageFlags.Renderable);
                var prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);

                AddComponent(entity, new WaveSpawnIData {
                    Prefab = prefab,
                    Size = authoring.size,
                    Spacing = authoring.spacing,
                    Height = authoring.height,
                    Speed = authoring.speed
                });
            }

        }

    }

    public struct WaveSpawnIData : IComponentData {

        public Entity Prefab;
        public int2 Size;
        public float2 Spacing;
        public float Height;
        public float Speed;

    }

    public struct WaveMoveIData : IComponentData {

        public float YOffset;
        public float Height;
        public float Speed;

    }

    [BurstCompile]
    [UpdateInGroup(typeof(Eu_InitializationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(InitializeRandomISystem))]
    public partial struct WaveSpawnISystem : ISystem {

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var em = state.EntityManager;

            foreach (var (wave, ltw, entity)
                     in SystemAPI.Query<RefRO<WaveSpawnIData>, RefRO<LocalToWorld>>().WithEntityAccess()) {
                if (wave.ValueRO.Prefab == Entity.Null) continue;
                var halfWidth = wave.ValueRO.Size.x * wave.ValueRO.Spacing.x * 0.5f;
                var halfHeight = wave.ValueRO.Size.y * wave.ValueRO.Height * 0.5f;
                var startPos = ltw.ValueRO.Position + new float3(halfWidth, 0, halfHeight);

                var total = wave.ValueRO.Size.x * wave.ValueRO.Size.y;
                using var instances = em.Instantiate(wave.ValueRO.Prefab, total, state.WorldUpdateAllocator);

                var count = 0;

                for (var x = 0; x < wave.ValueRO.Size.x; x++)
                for (var y = 0; y < wave.ValueRO.Size.y; y++, count++) {
                    var lt = em.GetComponentData<LocalTransform>(instances[count]);

                    lt.Position = startPos +
                                  new float3(x * wave.ValueRO.Spacing.x, 0, y * wave.ValueRO.Spacing.y);
                    em.SetComponentData(instances[count], lt);

                    ecb.AddComponent(instances[count], new WaveMoveIData {
                        YOffset = ltw.ValueRO.Position.y,
                        Height = wave.ValueRO.Height,
                        Speed = wave.ValueRO.Speed
                    });
                }

                ecb.RemoveComponent<WaveSpawnIData>(entity);
            }

            ecb.Playback(em);
        }

    }

}