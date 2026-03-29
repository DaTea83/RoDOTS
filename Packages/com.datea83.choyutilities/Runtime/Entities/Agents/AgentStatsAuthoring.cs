using EugeneC.Utilities;
using Unity.Entities;
using UnityEngine;

namespace EugeneC.ECS {
    
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DestroyAuthoring))]
    public class AgentStatsAuthoring : MonoBehaviour {
        
        [SerializeField] private AgentAttributes attributes;
        
        private class AgentStatsAuthoringBaker : Baker<AgentStatsAuthoring> {
            public override void Bake(AgentStatsAuthoring authoring) {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                var stats = authoring.attributes;
                
                AddComponent(e, new AgentStatsIData {
                    MoveSpeed = stats.MoveSpeed,
                    RotationSpeed = stats.RotationSpeed,
                    RestTime = stats.RestTime,
                    HasRestTime = stats.HasRestTime,
                    ExistTime = stats.ExistTime
                });
            }
        }
    }

    public struct AgentStatsIData : IComponentData {
        public float MoveSpeed;
        public float RotationSpeed;
        public float ExistTime;
        public float RestTime;
        public bool HasRestTime;
    }
}