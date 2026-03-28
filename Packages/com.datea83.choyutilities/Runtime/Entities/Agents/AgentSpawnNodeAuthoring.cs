using EugeneC.Utilities;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace EugeneC.ECS {

    [RequireComponent(typeof(AgentMoveNodeAuthoring))]
    [DisallowMultipleComponent]
    public class AgentSpawnNodeAuthoring : MonoBehaviour {

        //Prefabs must have the DestroyIData component
        [SerializeField] private DestroyAuthoring[] spawnPrefabs;
        [SerializeField] private bool spawnOnce;
        [SerializeField][Min(0.01f)] private float delay = 1f;
        [Tooltip("0 and below means it doesn't despawn")]
        [SerializeField] private float spawnExistTime = 60f;
        
        private class AgentSpawnNodeAuthoringBaker : Baker<AgentSpawnNodeAuthoring> {

            public override void Bake(AgentSpawnNodeAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Renderable);

                var buffer = AddBuffer<AgentSpawnIBuffer>(e);

                foreach (var prefab in authoring.spawnPrefabs) {
                    DependsOn(prefab);
                    buffer.Add(new AgentSpawnIBuffer { Prefab = GetEntity(prefab.gameObject, TransformUsageFlags.Dynamic) });
                }
                
                AddComponent(e, new AgentSpawnNodeIData {
                    SpawnOnce = authoring.spawnOnce,
                    DefaultSpawnDelay = authoring.delay,
                    ExistTime = authoring.spawnExistTime,
                });
            }
        }
    }

    [BurstCompile]
    [UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
    [UpdateBefore(typeof(InitializeAgentMoveISystem))]
    public partial struct AgentSpawnISystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AgentSpawnISingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var singleton = SystemAPI.GetSingleton<AgentSpawnISingleton>();

            if (singleton.SpawnLimit != 0)
                if (singleton.SpawnLimit > singleton.TotalSpawnCount) return;
            
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var et = SystemAPI.Time.ElapsedTime;
            var dt = SystemAPI.Time.DeltaTime;
            
            foreach (var (spawn, buffer,ltw, entity)
                     in SystemAPI.Query<RefRW<AgentSpawnNodeIData>, DynamicBuffer<AgentSpawnIBuffer>, RefRO<LocalToWorld>>()
                         .WithEntityAccess()) {
                
                spawn.ValueRW.CurrentSpawnDelay += dt;
                if (spawn.ValueRO.CurrentSpawnDelay < spawn.ValueRO.DefaultSpawnDelay) continue;
                spawn.ValueRW.CurrentSpawnDelay = 0;

                var index = entity.RandomValue(et, 0, buffer.Length);
                var newEntity = ecb.Instantiate(buffer[index].Prefab);
                singleton.CurrentSpawnCount++;
                singleton.TotalSpawnCount++;
                
                ecb.AddComponent(newEntity, new InitializeAgentIData {
                    Spawn = entity,
                    ExistTime = spawn.ValueRO.ExistTime,
                });
                ecb.SetComponent(newEntity, LocalTransform.FromPositionRotation(ltw.ValueRO.Position, ltw.ValueRO.Rotation));
                
                if (spawn.ValueRO.SpawnOnce) {
                    ecb.RemoveComponent<AgentMoveIEnableable>(entity);
                }
            }
            ecb.Playback(state.EntityManager);
            
            SystemAPI.SetSingleton(singleton);
        }

    }
}