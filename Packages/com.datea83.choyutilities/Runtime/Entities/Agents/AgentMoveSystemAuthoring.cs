using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS {

    [DisallowMultipleComponent]
    public class AgentMoveSystemAuthoring : MonoBehaviour {
        
        [SerializeField][Min(0.01f)] private float minSpeed = 1f, maxSpeed = 10f;
        [SerializeField] private bool hasRestTime = true;
        [SerializeField][Min(0.01f)] private float minRestTime = 1f, maxRestTime = 5f;

        private void OnValidate() {
            if (minSpeed > maxSpeed) 
                minSpeed = maxSpeed;
            
            if (minRestTime > maxRestTime) 
                minRestTime = maxRestTime;
        }

        private class Baker : Baker<AgentMoveSystemAuthoring> {

            public override void Bake(AgentMoveSystemAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.None);
                
                AddComponent(e, new AgentMoveSystemISingleton {
                    MinSpeed = authoring.minSpeed,
                    MaxSpeed = authoring.maxSpeed,
                    MinRestTime = authoring.minRestTime,
                    MaxRestTime = authoring.maxRestTime,
                    HasRestTime = authoring.hasRestTime
                });
            }
        }
    }
}