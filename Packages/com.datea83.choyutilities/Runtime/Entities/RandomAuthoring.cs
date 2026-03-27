using System;
using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    public sealed class RandomAuthoring : MonoBehaviour {

        [SerializeField] private ERandomInitializationType initializationType = ERandomInitializationType.None;
        [SerializeField] private uint manualOverrideValue;

        public class Baker : Baker<RandomAuthoring> {

            public override void Bake(RandomAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.None);

                switch (authoring.initializationType) {
                    case ERandomInitializationType.None:

                        AddComponent(e, new RandomIData { Value = Random.CreateFromIndex(0) });

                        break;
                    case ERandomInitializationType.ManualOverride:

                        AddComponent(e,
                            new RandomIData { Value = Random.CreateFromIndex(authoring.manualOverrideValue) });

                        break;
                    case ERandomInitializationType.SystemMilliseconds:

                        AddComponent<RandomIData>(e);
                        AddComponent<InitializeRandomIEnableableTag>(e);

                        break;
                    default:

                        throw new ArgumentOutOfRangeException();
                }
            }

        }

    }

    public enum ERandomInitializationType : byte {

        /// <summary>
        ///     None value generally should not be used. Will initialize <see cref="RandomIData" /> using index 0 during baking in
        ///     <see cref="RandomAuthoring.Baker.Bake" />.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Manual override value will initialize <see cref="RandomIData" /> using index
        ///     <see cref="RandomAuthoring.manualOverrideValue" /> during baking in <see cref="RandomAuthoring.Baker.Bake" />. This
        ///     can be helpful for debugging to get consistent random results.
        /// </summary>
        ManualOverride = 1,

        /// <summary>
        ///     System milliseconds value will initialize <see cref="RandomIData" /> using index from the system's millisecond time
        ///     value at the time of initialization in the <see cref="InitializeRandomISystem" />
        /// </summary>
        SystemMilliseconds = 2

    }

    public struct InitializeRandomIEnableableTag : IComponentData, IEnableableComponent { }

    public struct RandomIData : IComponentData {

        public Random Value;

    }

    [UpdateInGroup(typeof(Eu_InitializationSystemGroup), OrderFirst = true)]
    public partial struct InitializeRandomISystem : ISystem {

        public void OnUpdate(ref SystemState state) {
            var systemMilliseconds = (uint)Environment.TickCount;

            var index = 0u;

            foreach (var (random, initialize)
                     in SystemAPI.Query<RefRW<RandomIData>, EnabledRefRW<InitializeRandomIEnableableTag>>()
                         .WithOptions(EntityQueryOptions.IncludeSystems)) {
                random.ValueRW.Value = Random.CreateFromIndex(systemMilliseconds + index);
                index++;

                initialize.ValueRW = false;
            }
        }

    }

}