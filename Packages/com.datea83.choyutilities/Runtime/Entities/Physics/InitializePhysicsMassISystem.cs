using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace EugeneC.ECS {

    [BurstCompile]
    [UpdateInGroup(typeof(EuCManagedComponentSystem), OrderFirst = true)]
    [UpdateBefore(typeof(InitializeRandomISystem))]
    public partial struct InitializePhysicsMassISystem : ISystem {

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (mass, entity)
                     in SystemAPI.Query<RefRO<PhysicsMass>>().WithAll<InitializePhysicsMassDataITag>()
                         .WithNone<PhysicsMassIData>().WithEntityAccess()) {
                ecb.AddComponent(entity, new PhysicsMassIData {
                    InverseMass = mass.ValueRO.InverseMass,
                    InverseInertia = mass.ValueRO.InverseInertia
                });
                ecb.RemoveComponent<InitializePhysicsMassDataITag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }

    }

    public struct InitializePhysicsMassDataITag : IComponentData { }

    /// <summary>
    ///     Stores the data because changing to kinematic will set both data to zero
    /// </summary>
    /// <remarks>
    ///     The relationship between mass and inverse mass is : IM = 1 / (M * 100)
    /// </remarks>
    public struct PhysicsMassIData : IComponentData {

        public float InverseMass;
        public float3 InverseInertia;

    }

}