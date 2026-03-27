using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;

// Purpose of these is to help encapsulates custom define systems and organize them
namespace EugeneC.ECS {

    /// <summary>
    ///     System group containing systems related to initialization logic that should be executed towards the beginning of
    ///     the frame.
    /// </summary>
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(EndInitializationEntityCommandBufferSystem))]
    public partial class Eu_InitializationSystemGroup : ComponentSystemGroup { }

    /// <summary>
    ///     System group containing systems that deal with physics such as scheduling collision/trigger event jobs and
    ///     executing physics overlap queries.
    /// </summary>
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSimulationGroup))]
    [UpdateBefore(typeof(AfterPhysicsSystemGroup))]
    public partial class Eu_PhysicsSystemGroup : ComponentSystemGroup { }

    /// <summary>
    ///     System group containing systems that deal with moving entities via their LocalTransform component.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial class Eu_PreTransformSystemGroup : ComponentSystemGroup { }

    /// <summary>
    ///     System group containing systems related to entity interactions
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial class Eu_PostTransformSystemGroup : ComponentSystemGroup { }

    /// <summary>
    ///     System group containing systems that deal with triggering visual and audio effects.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(Eu_DestroySystemGroup))]
    public partial class Eu_EffectSystemGroup : ComponentSystemGroup { }

    /// <summary>
    ///     System group containing systems related to entity destruction.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(EndSimulationEntityCommandBufferSystem))]
    public partial class Eu_DestroySystemGroup : ComponentSystemGroup { }

}