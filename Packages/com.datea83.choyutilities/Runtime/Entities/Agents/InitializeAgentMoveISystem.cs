using Unity.Burst;
using Unity.Entities;

namespace EugeneC.ECS {

    [UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
    public partial struct InitializeAgentMoveISystem : ISystem {

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            
            foreach (var (initial, stats, entity) in 
                     SystemAPI.Query<RefRO<InitializeAgentIData>, RefRO<AgentStatsIData>>()
                         .WithNone<AgentMoveIEnableable>().WithEntityAccess()) {
                
                ecb.AddComponent(entity, new AgentMoveIEnableable {
                    CurrentNode = initial.ValueRO.Spawn,
                });

                if (stats.ValueRO.ExistTime > 0) {
                    ecb.AddComponent(entity, new DestroyTimeIData{ Value = stats.ValueRO.ExistTime});
                    ecb.AddComponent(entity, new AgentMoveICleanupTag());
                }
                
                ecb.AddComponent<RandomIData>(entity);
                ecb.AddComponent<InitializeRandomIEnableableTag>(entity);
                
                ecb.RemoveComponent<InitializeAgentIData>(entity);
            }
            ecb.Playback(state.EntityManager);
        }

    }

}