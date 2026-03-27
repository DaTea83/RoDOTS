using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace EugeneC.ECS {

    public class FlatPlaneAuthoring : MonoBehaviour {

        public GameObject planePrefab;

        [Tooltip("Scale up the prefab to value + 100")]
        public uint planeSize = 1000;

        [Tooltip("The smaller the value is the denser the grid is")]
        public float unitsPerPlane = 2f;

        public float3 planeOffset;

        [Tooltip("Refresh the plane")] public byte seed;

        private byte _prevSeed;

        private void OnValidate() {
            if (_prevSeed == seed) return;
            if (World.DefaultGameObjectInjectionWorld is null) return;
            _prevSeed = seed;
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var query = entityManager.CreateEntityQuery(typeof(PlaneGenerationIData));

            if (!query.HasSingleton<PlaneGenerationIData>()) return;
            var levelEntity = query.GetSingletonEntity();
            entityManager.SetComponentEnabled<RegenerateLevelIFlag>(levelEntity, true);
        }

        private class FlatPlaneBaker : Baker<FlatPlaneAuthoring> {

            public override void Bake(FlatPlaneAuthoring authoring) {
                if (authoring.planePrefab is null) return;

                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new PlaneGenerationIData {
                    PlanePrefab = GetEntity(authoring.planePrefab, TransformUsageFlags.Dynamic),
                    PlaneSize = math.abs(authoring.planeSize),
                    UnitsPerPlane = math.abs(authoring.unitsPerPlane),
                    PlaneOffset = authoring.planeOffset
                });
                AddComponent<RegenerateLevelIFlag>(entity);
            }

        }

    }

    public struct PlaneGenerationIData : IComponentData {

        public Entity PlanePrefab;
        public float PlaneSize;
        public float UnitsPerPlane;
        public float3 PlaneOffset;

    }

    /// <summary>
    ///     Enableable component to inform the <see cref="PlaneGenerationSystemBase" /> that the level should be regenerated.
    /// </summary>
    public struct RegenerateLevelIFlag : IComponentData, IEnableableComponent { }

    /// <summary>
    ///     Material property override to set the tile scale
    /// </summary>
    [MaterialProperty("_Tile_Scale")]
    public struct TilingOverrideIData : IComponentData {

        public float Value;

    }

    [BurstCompile]
    [WorldSystemFilter(WorldSystemFilterFlags.EntitySceneOptimizations | WorldSystemFilterFlags.Editor |
                       WorldSystemFilterFlags.Default)]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PlaneGenerationSystemBase : SystemBase {

        [BurstCompile]
        protected override void OnUpdate() {
            var ecb = new EntityCommandBuffer(WorldUpdateAllocator);

            foreach (var (regenerateLevel, planeGeneration) in SystemAPI
                         .Query<EnabledRefRW<RegenerateLevelIFlag>, PlaneGenerationIData>()) {
                if (!regenerateLevel.ValueRW) continue;

                var instance = ecb.Instantiate(planeGeneration.PlanePrefab);
                var instScale = planeGeneration.PlaneSize + 100;

                ecb.SetComponent(instance,
                    LocalTransform.FromPositionRotationScale(planeGeneration.PlaneOffset, quaternion.identity,
                        instScale));

                ecb.AddComponent(instance,
                    new TilingOverrideIData { Value = instScale / planeGeneration.UnitsPerPlane });

                regenerateLevel.ValueRW = false;
            }

            ecb.Playback(EntityManager);
        }

    }

}