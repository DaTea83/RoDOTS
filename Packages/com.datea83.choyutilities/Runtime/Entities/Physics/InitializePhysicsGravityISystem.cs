using Unity.Burst;
using Unity.Entities;
using Unity.Physics;

namespace EugeneC.ECS {

    [UpdateInGroup(typeof(Eu_InitializationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(InitializeRandomISystem))]
    public partial struct InitializePhysicsGravityISystem : ISystem {

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (_, entity)
                     in SystemAPI.Query<RefRO<PhysicsMass>>()
                         .WithNone<PhysicsGravityFactor>().WithEntityAccess())
                ecb.AddComponent(entity, new PhysicsGravityFactor {
                    Value = 1
                });

            ecb.Playback(state.EntityManager);
        }

    }

}