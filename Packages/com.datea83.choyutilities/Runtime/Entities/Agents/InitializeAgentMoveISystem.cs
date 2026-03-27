using EugeneC.Utilities;
using Unity.Burst;
using Unity.Entities;

namespace EugeneC.ECS {

    [UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
    public partial struct InitializeAgentMoveISystem : ISystem {

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<AgentMoveSystemISingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            var et = SystemAPI.Time.ElapsedTime;
            var system = SystemAPI.GetSingleton<AgentMoveSystemISingleton>();
            
            foreach (var (initial, entity) in 
                     SystemAPI.Query<RefRO<InitializeAgentIData>>()
                         .WithNone<AgentMoveIEnableable>().WithEntityAccess()) {
                
                ecb.AddComponent(entity, new AgentMoveIEnableable {
                    CurrentNode = initial.ValueRO.Spawn,
                    Speed = entity.RandomValue(et, system.MinSpeed, system.MaxSpeed),
                    DefaultRestTime = entity.RandomValue(et, system.MinRestTime, system.MaxRestTime),
                });

                if (initial.ValueRO.ExistTime > 0) {
                    ecb.AddComponent(entity, new DestroyTimeIData{ Value = initial.ValueRO.ExistTime});
                }
                
                ecb.AddComponent<RandomIData>(entity);
                ecb.AddComponent<InitializeRandomIEnableableTag>(entity);
                
                ecb.RemoveComponent<InitializeAgentIData>(entity);
            }
            ecb.Playback(state.EntityManager);
        }

    }

}