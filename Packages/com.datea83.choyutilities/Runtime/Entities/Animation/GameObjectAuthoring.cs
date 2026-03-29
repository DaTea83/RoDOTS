using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS {
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DestroyAuthoring))]
    public class GameObjectAuthoring : MonoBehaviour {

        public GameObject prefab;
        
        private class GameObjectAuthoringBaker : Baker<GameObjectAuthoring> {
            public override void Bake(GameObjectAuthoring authoring) {
                if (authoring.prefab is null) return;
                var e = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(e, new GameObjectIData {
                    Prefab = new UnityObjectRef<GameObject> {
                        Value = authoring.prefab
                    }
                });
            }
        }
    }

    public struct GameObjectIData : IComponentData {
        public UnityObjectRef<GameObject> Prefab;
    }
}