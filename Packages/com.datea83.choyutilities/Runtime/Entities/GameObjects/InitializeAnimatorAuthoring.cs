using Unity.Entities;
using UnityEngine;

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

}