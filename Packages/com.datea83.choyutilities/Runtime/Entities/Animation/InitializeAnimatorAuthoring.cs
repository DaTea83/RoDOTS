using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EugeneC.ECS {
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GameObjectAuthoring))]
    public class InitializeAnimatorAuthoring : MonoBehaviour {
        
        private void OnValidate() {
            var author = GetComponent<GameObjectAuthoring>();
            if (author.prefab is null) {
                Debug.LogError($"{nameof(author.prefab)} is null");
                return;
            }
            
            if(!author.prefab.TryGetComponent(out Animator animator))
                Debug.LogError($"{nameof(author.prefab)} doesn't have an Animator");
        }

        private class InitializeAnimatorAuthoringBaker : Baker<InitializeAnimatorAuthoring> {
            public override void Bake(InitializeAnimatorAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<InitializeAnimatorITag>(e);
            }
        }
    }
    
    public struct AnimatorIData : IComponentData {
        public UnityObjectRef<Animator> Animator;
    }

    public struct AnimatorICleanup : ICleanupComponentData {
        public UnityObjectRef<Animator> Animator;
    }
    
    public struct InitializeAnimatorITag :  IComponentData {}

    [UpdateInGroup(typeof(Eu_InitializationSystemGroup))]
    public partial struct InitializeAnimatorISystem : ISystem {
        
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(state.WorldUpdateAllocator);
            
            foreach (var (obj, entity) in 
                     SystemAPI.Query<RefRO<GameObjectIData>>()
                         .WithAll<InitializeAnimatorITag>().WithEntityAccess()) {
                 
                var instance = Object.Instantiate(obj.ValueRO.Prefab.Value.gameObject);
                ecb.AddComponent(entity, new ObjTransformIData {
                    Transform = instance.transform,
                    SmoothFollowSpeed = 0,
                    Offset = float3.zero
                });
                ecb.AddComponent(entity, new AnimatorICleanup {
                    Animator = new UnityObjectRef<Animator> {
                        Value = instance.GetComponent<Animator>()
                    }
                });
                ecb.AddComponent(entity, new AnimatorIData {
                    Animator = instance.GetComponent<Animator>(),
                });
                
                ecb.RemoveComponent<InitializeAnimatorITag>(entity);
            }
            ecb.Playback(state.EntityManager);
        }
    }

    [UpdateInGroup(typeof(Eu_DestroySystemGroup))]
    public partial struct CleanupAnimatorISystem : ISystem {
        
        public void OnUpdate(ref SystemState state) {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
        
            foreach (var (cleanup, entity) 
                     in SystemAPI.Query<RefRO<AnimatorICleanup>>()
                         .WithNone<ObjTransformIData>()
                         .WithEntityAccess()) {
            
                if (cleanup.ValueRO.Animator.Value is not null) {
                    Object.Destroy(cleanup.ValueRO.Animator.Value.gameObject);
                }
                ecb.RemoveComponent<AnimatorICleanup>(entity);
            }
        
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}