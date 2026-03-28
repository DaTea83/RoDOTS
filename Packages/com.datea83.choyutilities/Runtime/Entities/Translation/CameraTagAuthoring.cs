using EugeneC.Utilities;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    public sealed class CameraTagAuthoring : MonoBehaviour {

        private class CameraTagBaker : Baker<CameraTagAuthoring> {

            public override void Bake(CameraTagAuthoring authoring) {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<CameraISingletonTag>(entity);
                AddComponent<InitializeCameraTargetITag>(entity);
            }

        }

    }

    public struct CameraISingletonTag : IComponentData { }

    public struct InitializeCameraTargetITag : IComponentData { }

    /// <summary>
    ///     Find any entity with the InitializeTag that doesn't have CameraTargetIData
    ///     Add the IData component
    ///     Set the transform reference of main camera to IData
    ///     Remove the InitializeTag
    /// </summary>
    [UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
    public partial struct InitializeCameraTargetISystem : ISystem {

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<InitializeCameraTargetITag>();
        }

        public void OnUpdate(ref SystemState state) {
            if (CameraController.Instance is null || Camera.main is null) return;
            var camTargetTransform = CameraController.Instance.transform;

            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);

            foreach (var (_, entity)
                     in SystemAPI.Query<RefRO<InitializeCameraTargetITag>>()
                         .WithNone<ObjTransformIData>().WithEntityAccess()) {
                ecb.AddComponent(entity, new ObjTransformIData {
                    Transform = new UnityObjectRef<Transform> {
                        Value = camTargetTransform
                    },
                    Offset = float3.zero,
                    SmoothFollowSpeed = 0
                });
                ecb.RemoveComponent<InitializeCameraTargetITag>(entity);
            }

            ecb.Playback(state.EntityManager);
        }

    }

}