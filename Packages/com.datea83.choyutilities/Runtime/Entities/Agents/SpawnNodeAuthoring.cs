using EugeneC.Utilities;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace EugeneC.ECS {

    [RequireComponent(typeof(MoveNodeAuthoring))]
    [RequireComponent(typeof(GroupTagAuthoring))]
    [DisallowMultipleComponent]
    public class SpawnNodeAuthoring : MonoBehaviour {

        //Prefabs must have the DestroyIData component
        [SerializeField] private AgentStatsAuthoring[] spawnPrefabs;
        [SerializeField] private bool spawnOnce;
        [SerializeField] private bool disabledOnStart;
        [SerializeField] [Min(0.01f)] private float delay = 1f;

        private class Baker : Baker<SpawnNodeAuthoring> {

            public override void Bake(SpawnNodeAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Renderable);

                var buffer = AddBuffer<SpawnNodeIBuffer>(e);

                foreach (var prefab in authoring.spawnPrefabs) {
                    DependsOn(prefab);

                    buffer.Add(new SpawnNodeIBuffer {
                        Prefab = GetEntity(prefab.gameObject, TransformUsageFlags.Dynamic)
                    });
                }

                AddComponent(e, new SpawnNodeIEnableable {
                    SpawnOnce = authoring.spawnOnce,
                    DefaultSpawnDelay = authoring.delay
                });

                if (authoring.disabledOnStart)
                    SetComponentEnabled<SpawnNodeIEnableable>(e, false);
            }

        }

    }

    [BurstCompile]
    [UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
    [UpdateBefore(typeof(InitializeAgentMoveISystem))]
    public partial struct AgentSpawnISystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AgentISingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var singleton = SystemAPI.GetSingleton<AgentISingleton>();

            if (singleton.SpawnLimit != 0)
                if (singleton.SpawnLimit <= singleton.TotalSpawnCount)
                    return;

            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var et = SystemAPI.Time.ElapsedTime;
            var dt = SystemAPI.Time.DeltaTime;

            foreach (var (spawn, buffer, ltw, entity)
                     in SystemAPI
                         .Query<RefRW<SpawnNodeIEnableable>, DynamicBuffer<SpawnNodeIBuffer>, RefRO<LocalToWorld>>()
                         .WithEntityAccess()) {
                spawn.ValueRW.CurrentSpawnDelay += dt;

                if (spawn.ValueRO.CurrentSpawnDelay < spawn.ValueRO.DefaultSpawnDelay) continue;
                spawn.ValueRW.CurrentSpawnDelay = 0;

                var index = entity.RandomValue(et, 0, buffer.Length);
                var newEntity = ecb.Instantiate(buffer[index].Prefab);
                singleton.CurrentSpawnCount++;
                singleton.TotalSpawnCount++;

                ecb.AddComponent(newEntity, new InitializeAgentIData {
                    Spawn = entity
                });

                ecb.SetComponent(newEntity, LocalTransform.FromPositionRotation(
                    ltw.ValueRO.Position, ltw.ValueRO.Rotation));

                if (spawn.ValueRO.SpawnOnce) ecb.SetComponentEnabled<SpawnNodeIEnableable>(entity, false);
            }

            ecb.Playback(state.EntityManager);

            SystemAPI.SetSingleton(singleton);
        }

    }

}